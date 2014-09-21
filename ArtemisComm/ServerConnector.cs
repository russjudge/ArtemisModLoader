using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ArtemisComm
{
    public class ServerConnector : IDisposable
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ServerConnector));
        public ServerConnector(string host, int port)
        {
            Host = host;
            Port = port;
        }


        public string Host { get; private set; }
        public int Port { get; private set; }

        TcpClient Client { get; set; }

        System.Threading.Thread ListeningThread { get; set; }
        System.Threading.Thread ProcessThread { get; set; }
        System.Threading.Thread SendingThread { get; set; }
        public void Start()
        {
            if (_log.IsInfoEnabled) { _log.Info("Starting Server Connection"); }
            ThreadStart start = new ThreadStart(ProcessMessage);
            ListeningThread = new Thread(start);

            start = new ThreadStart(HandleQueue);

            ProcessThread = new Thread(start);
            ProcessThread.Start();

            ListeningThread.Start();

            start = new ThreadStart(ProcessSender);
            SendingThread = new Thread(start);

            SendingThread.Start();
            if (_log.IsInfoEnabled) { _log.Info("Server Connection Initialized"); }
            
        }
        bool abort = false;

        public void Stop()
        {
            
            abort = true;
            mreSender.Set();
            mreListener.Set();
            Client.Close();
            if (_log.IsInfoEnabled) { _log.Info("Server Connection Stopped"); }
        }
        public void SendPackage(Packet p)
        {
            if (p != null)
            {
                if (_log.IsInfoEnabled) { _log.InfoFormat("Sending Data to server: {0}", p.PacketType.ToString()); }
                SendQueue.Enqueue(p.GetBytes());
                mreSender.Set();
            }
        }

        void ProcessSender()
        {
            try
            {
                do
                {
                    mreSender.WaitOne();
                    if (!abort)
                    {
                        if (ServerStream != null)
                        {
                            while (SendQueue.Count > 0)
                            {
                                if (_log.IsInfoEnabled) { _log.Info("Sending data to server"); }
                                byte[] buff = SendQueue.Dequeue();
                                ServerStream.Write(buff, 0, buff.Length);
                            }
                        }
                        
                    }
                    if (!abort)
                    {
                        mreSender.Reset();
                    }
                } while (!abort);
            }
            catch (ThreadAbortException)
            {

            }
            catch (SocketException e)
            {
                if (_log.IsWarnEnabled) { _log.Warn("Exception from socket", e); }
            }

        }
        Queue<byte[]> SendQueue = new Queue<byte[]>();

        NetworkStream ServerStream = null;


        void ProcessMessage()
        {
            try
            {
                if (Client != null)
                {
                    Client.Close();
                }
                Client = new TcpClient();
                if (_log.IsInfoEnabled) { _log.Info("Connecting to server"); }
                Client.Connect(Host, Port);
                if (_log.IsInfoEnabled) { _log.Info("Server connected--getting stream"); }

                ServerStream = Client.GetStream();
                if (this.Connected != null)
                {
                    Connected(this, EventArgs.Empty);
                }
                if (_log.IsInfoEnabled) { _log.Info("Got server stream"); }
                byte[] buff = null;
                List<byte> buffer = null;
                int bytesRead = 0;
                int currentBlock = 0;
                
                do
                {

                    do
                    {
                        buff = new byte[8];
                        currentBlock = 0;
                        buffer = new List<byte>();
                        bytesRead = ServerStream.Read(buff, 0, buff.Length);
                        if (bytesRead > 0)
                        {
                            if (_log.IsInfoEnabled) { _log.InfoFormat("From Server: {0} bytes (1)", bytesRead.ToString()); }
                            byte[] wrkByte = new byte[bytesRead];
                            Array.Copy(buff, 0, wrkByte, 0, bytesRead);
                            currentBlock += bytesRead;

                            buffer.AddRange(buff);

                            //Code here to fix error with packet and try to self-adjust.  This may cause packets to be ignored.
                            if (buffer.Count >= 4)
                            {
                                uint headerID = 0;
                                do
                                {
                                    headerID = BitConverter.ToUInt32(buffer.ToArray(), 0);
                                    if (headerID != StandardID)
                                    {
                                        buffer.RemoveAt(0);
                                    }
                                } while (buffer.Count > 3 && headerID != StandardID);
                            }
                        }

                    } while (buffer.Count < 8 && bytesRead > 0);
                    if (bytesRead > 0)
                    {
                        
                        int ln = BitConverter.ToInt32(buffer.ToArray(), 4);
                        if (_log.IsInfoEnabled)
                        {
                            _log.InfoFormat("Total block length that needs read: {0}", ln.ToString());
                        }
                        buff = new byte[ln];
                        int remainToRead = ln - 8;
                        do
                        {


                            bytesRead = ServerStream.Read(buff, 0, remainToRead);
                            if (bytesRead > 0)
                            {
                                if (_log.IsInfoEnabled) { _log.InfoFormat("From Server: {0} bytes (2)--needing {1} bytes for block", bytesRead.ToString(), ln.ToString()); }
                                byte[] wrkByte = new byte[bytesRead];
                                Array.Copy(buff, 0, wrkByte, 0, bytesRead);
                                currentBlock += bytesRead;

                                buffer.AddRange(wrkByte);
                                remainToRead -= bytesRead;
                            }



                        } while (buffer.Count < ln && bytesRead > 0);
                        if (bytesRead > 0)
                        {

                            if (_log.IsInfoEnabled) { _log.InfoFormat("Putting {0} byte block from server into queue.", buffer.Count.ToString()); }
                            byte[] b = buffer.ToArray();
                            ProcessQueue.Enqueue(b);
                            mreListener.Set();


                        }
                    }
                    else
                    {
                        abort = true;
                        mreListener.Set();
                        mreSender.Set();
                    }

                } while (!abort);

            }
            catch (ThreadAbortException)
            {

            }
            catch (System.IO.IOException e)
            {
                if (_log.IsWarnEnabled) { _log.Warn("Exception from socket", e); }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                if (_log.IsWarnEnabled) { _log.Warn("Exception from socket", e); }
            }
            if (this.ConnectionLost != null)
            {
                ConnectionLost(this, EventArgs.Empty);
            }
          
        }
        public const uint StandardID = 0xdeadbeef;

        Queue<byte[]> ProcessQueue = new Queue<byte[]>();
        void HandleQueue()
        {

            do
            {
                lock (ProcessQueue)
                {
                    while (ProcessQueue.Count > 0)
                    {
                        byte[] byteArray = ProcessQueue.Dequeue();
                        if (_log.IsInfoEnabled) { _log.InfoFormat("@@@@@@@@  From Server: Pulled {0} byte block from ProcessQueue", byteArray.Length.ToString()); }
                        try
                        {
                            Packet p = new Packet(byteArray);
                            if (_log.IsInfoEnabled) { _log.InfoFormat("From Server: byte-block translates to Packet {0} pulled from queue, raising event", p.PacketType.ToString()); }
                            if (PackageReceived != null)
                            {

                                PackageReceived(this, new PackageEventArgs(p));
                            }
                            string methodName = p.PacketType.ToString() + "Received";
                            if (_log.IsInfoEnabled) { _log.InfoFormat("Preparing to raise event: {0}", methodName); }
                            Type t = this.GetType();
                            if (t.GetEvent(methodName) != null)
                            {

                                //var eventInfo = this.GetType().GetEvent(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


                                var eventDelegate = (MulticastDelegate)t.GetField(methodName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this);
                                if (eventDelegate != null)
                                {
                                    foreach (var handler in eventDelegate.GetInvocationList())
                                    {
                                        try
                                        {
                                            handler.Method.Invoke(handler.Target, new object[] { this, new PackageEventArgs(p) });
                                        }
                                        catch (Exception ex)
                                        {
                                            if (_log.IsWarnEnabled)
                                            {
                                                _log.Warn("Exception raising event", ex);
                                            }

                                        }
                                    }
                                }


                            }
                            else
                            {
                                if (UndefinedPacketReceived != null)
                                {
                                    UndefinedPacketReceived(this, new PackageEventArgs(p));
                                }
                            }


                        }
                        catch (Exception ex)
                        {
                            if (_log.IsWarnEnabled)
                            {
                                _log.Warn("Exception processing bytes to packet", ex);
                            }
                        }
                        if (abort)
                        {
                            break;
                        }
                    }
                    

                }
                if (!abort)
                {
                    mreListener.Reset();
                    mreListener.WaitOne();
                }
            } while (!abort);
        }
        private ManualResetEvent mreListener = new ManualResetEvent(false);
        private ManualResetEvent mreSender = new ManualResetEvent(false);

        /// <summary>
        /// Occurs when [package received].  Generic data.  Not Necessary to use if using all the other events.
        /// </summary>
        public event EventHandler<PackageEventArgs> PackageReceived;
        public event EventHandler<PackageEventArgs> AudioCommandPacketReceived;
        public event EventHandler<PackageEventArgs> CommsIncomingPacketReceived;
        public event EventHandler<PackageEventArgs> CommsOutgoingPacketReceived;
        public event EventHandler<PackageEventArgs> DestroyObjectPacketReceived;
        public event EventHandler<PackageEventArgs> EngGridUpdatePacketReceived;
        public event EventHandler<PackageEventArgs> GameMessagePacketReceived;
        public event EventHandler<PackageEventArgs> IncomingAudioPacketReceived;
        public event EventHandler<PackageEventArgs> ObjectStatusUpdatePacketReceived;
        public event EventHandler<PackageEventArgs> ShipActionPacketReceived;
        public event EventHandler<PackageEventArgs> ShipAction2PacketReceived;
        public event EventHandler<PackageEventArgs> ShipAction3PacketReceived;
        public event EventHandler<PackageEventArgs> StationStatusPacketReceived;
        public event EventHandler<PackageEventArgs> GameStartPacketReceived;
        public event EventHandler<PackageEventArgs> Unknown2PacketReceived;
        public event EventHandler<PackageEventArgs> IntelPacketReceived;
        public event EventHandler<PackageEventArgs> VersionPacketReceived;
        public event EventHandler<PackageEventArgs> WelcomePacketReceived;

        public event EventHandler Connected;
        public event EventHandler ConnectionLost;

        public bool IsConnected
        {
            get
            {
                return Client.Connected;
            }
        }
        public void ClearSendQueue()
        {
            SendQueue.Clear();
        }
        /// <summary>
        /// Occurs when [undefined packet received].  If the packet is not known (such as due to a new version of Artemis), this event is raised instead.
        /// </summary>
        public event EventHandler<PackageEventArgs> UndefinedPacketReceived;
        bool isDisposed = false;
        private void Dispose(bool Disposing)
        {
            if (!isDisposed)
            {
                if (Disposing)
                {
                   
                    abort = true;
                    if (mreSender != null)
                    {
                        mreSender.Set();
                        mreSender.Dispose();
                    }
                    if (mreListener != null)
                    {
                        mreListener.Set();
                        mreListener.Dispose();
                    }
                    if (ListeningThread != null && ListeningThread.ThreadState == ThreadState.Running)
                    {
                        ListeningThread.Abort();
                        
                    }
                    if (ProcessThread != null && ProcessThread.ThreadState == ThreadState.Running)
                    {
                        ProcessThread.Abort();
                    }
                    if (SendingThread != null && SendingThread.ThreadState == ThreadState.Running)
                    {
                        SendingThread.Abort();
                    }
                    if (ServerStream != null)
                    {
                        ServerStream.Dispose();
                    }
                    isDisposed = true;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
//Server to Client
//CommsIncomingPacket (0xd672c35f)
//DestroyObjectPacket (0xcc5a3e30)
//EngGridUpdatePacket (0x077e9f3c)
//IncomingAudioPacket (0xae88e058)
//GameMessagePacket (0xf754c8fe)
//... AllShipSettingsPacket (subtype 0x0f)
//... GameMessagePacket (subtypes 0x06 and 0x0a)
//... GameStartPacket (subtype 0x00)
//... JumpStatusPacket (subtypes 0x0c and 0x0d)
//... Unknown (subtypes 0x08 and 0x09)
//ObjectStatusUpdatePacket (0x80803df9)
//... EnemyUpdatePacket (subtype 0x04; now apparently used for both enemies and civilians)
//... EngPlayerUpdatePacket (subtype 0x03)
//... GenericMeshPacket (subtype 0x0d?)
//... GenericUpdatePacket (subtypes 0x06, 0x07, 0x09-0x0c, 0x0e?)
//... MainPlayerUpdatePacket (subtype 0x01)
//... OtherShipUpdatePacket (subtype 0x06 under Artemis 1.7, now defunct?)
//... StationPacket (subtype 0x05?)
//... WeapPlayerUpdatePacket (subtype 0x02)
//... WhaleUpdatePacket (subtype 0x0f?)
//StationStatusPacket (0x19c6e2d4)
//VersionPacket (0xe548e74a)
//WelcomePacket (0x6d04b3da)
//Unknown (0x3de66711, 0xf5821226)
//Client to Server
//AudioCommandPacket (0x6aadc57f)
//CommsOutgoingPacket (0x574c4c4b)
//ShipActionPacket (0x4c821d3c)
//... CaptainSelectPacket (subtype 0x11)
//... DiveRisePacket (subtype 0x1b)
//... EngSetAutoDamconPacket (subtype 0x0c)
//... FireTubePacket (subtype 0x08)
//... HelmRequestDockPacket (subtype 0x07)
//... HelmSetWarpPacket (subtype 0x00)
//... HelmToggleReversePacket (subtype 0x18)
//... ReadyPacket (subtype 0x0f)
//... ReadyPacket2 (subtype 0x19)
//... SciScanPacket (subtype 0x13)
//... SciSelectPacket (subtype 0x10)
//... SetBeamFreqPacket (subtype 0x0b)
//... SetMainScreenPacket (subtype 0x01)
//... SetShipPacket (subtype 0x0d)
//... SetShipSettingsPacket (subtype 0x16)
//... SetStationPacket (subtype 0x0e)
//... SetWeaponsTargetPacket (subtype 0x02)
//... ToggleAutoBeamsPacket (subtype 0x03)
//... TogglePerspectivePacket (subtype 0x1a)
//... ToggleRedAlertPacket (subtype 0x0a)
//... ToggleShieldsPacket (subtype 0x04)
//... UnloadTubePacket (subtype 0x09)
//ShipActionPacket2 (0x69cc01d9)
//... ConvertTorpedoPacket (subtype 0x03)
//... EngSendDamconPacket (subtype 0x04)
//... EngSetCoolantPacket (subtype 0x00)
//... LoadTubePacket (subtype 0x02)
//ShipActionPacket3 (0x0351a5ac)
//... EngSetEnergyPacket (subtype 0x04)
//... HelmJumpPacket (subtype 0x05)
//... HelmSetImpulsePacket (subtype 0x00)
//... HelmSetSteeringPacket (subtype 0x01)