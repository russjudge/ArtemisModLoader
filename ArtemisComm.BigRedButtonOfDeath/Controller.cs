using ArtemisComm.GameMessageSubPackets;
using ArtemisComm.ObjectStatusUpdateSubPackets;
using ArtemisComm.ShipAction2SubPackets;
using ArtemisComm.ShipAction3SubPackets;
using ArtemisComm.ShipActionSubPackets;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArtemisComm.BigRedButtonOfDeath
{
    public class Controller : IDisposable
    {
        public const decimal MinimumSupportedArtemisVersion = 2.0M;
        public const decimal ExpectedArtemisVersion = 2.0M;

        public Controller(IView view)
        {
            View = view;
            Subscribe();

        }
        void Subscribe()
        {
            View.ConnectRequested += View_ConnectRequested;
            View.StartSelfDestruct += View_StartSelfDestruct;
            View.CancelSelfDestruct += View_CancelSelfDestruct;
            View.DisconnectRequested += View_DisconnectRequested;
            View.Disposing += View_Disposing;
        }
        void UnSubscribe()
        {
            View.ConnectRequested -= View_ConnectRequested;
            View.StartSelfDestruct -= View_StartSelfDestruct;
            View.CancelSelfDestruct -= View_CancelSelfDestruct;
            View.DisconnectRequested -= View_DisconnectRequested;
            View.Disposing -= View_Disposing;
        }
        void View_Disposing(object sender, EventArgs e)
        {
            Dispose();
        }

        void View_DisconnectRequested(object sender, EventArgs e)
        {
            Disconnect();
        }
        void Disconnect()
        {
            if (connector != null)
            {
                ConnectorUnsubscribe();
                connector.Dispose();
                connector = null;
            }
        }
        bool selfDestructStarted = false;
        void View_CancelSelfDestruct(object sender, EventArgs e)
        {

            CancelSelfDestruct();
        }

        void View_StartSelfDestruct(object sender, EventArgs e)
        {

            StartSelfDestruct();
        }

        void View_ConnectRequested(object sender, EventArgs e)
        {
            Connect();
            
        }
        void CancelSelfDestruct()
        {
            if (GameInProgress)
            {
                plyr.Stop();
                ShieldsMustBeDown = false;
                ZeroWarpEnergy();
                selfDestructStarted = false;
            }
        }
        
        bool shieldsRaised = false;
        bool redAlertEnabled = false;
        void StartSelfDestruct()
        {
            if (GameInProgress)
            {
                AnnounceAlert();
                if (!redAlertEnabled)
                {
                    SendRedAlert();
                }

                SendDropShields();
                SetEngineeringSettings();
                selfDestructStarted = true;
            }
        }
        bool ShieldsMustBeDown = false;
        void SendDropShields()
        {
            if (GameInProgress)
            {
                ShieldsMustBeDown = true;
                if (shieldsRaised)
                {
                    //connector.SendPackage(ToggleShieldsSubPacket.GetPacket());
                    connector.SendToggleShields(serverID);
                }
            }
        }
        void SendRedAlert()
        {
            if (GameInProgress)
            {
                //connector.SendPackage(ToggleRedAlertSubPacket.GetPacket());
                connector.SendToggleRedAlert(serverID);
            }
        }
        void SetEngineeringSettings()
        {
            if (GameInProgress)
            {
                ZeroAllCoolant();
                ZeroAllButWarpEnergy();
                MaxWarpEnergy();
            }
        }
        void ZeroAllButWarpEnergy()
        {
            if (GameInProgress)
            {
                foreach (ShipSystems st in Enum.GetValues(typeof(ShipSystems)))
                {
                    if (st != ShipSystems.WarpJumpDrive)
                    {
                        //connector.SendPackage(EngSetEnergySubPacket.GetPacket(st, 0));
                        connector.SendEngSetEngerySubPacket(serverID, st, 0);
                    }
                }
            }
        }
        void MaxWarpEnergy()
        {
            if (GameInProgress)
            {
                //connector.SendPackage(EngSetEnergySubPacket.GetPacket(ShipSystems.WarpJumpDrive, 1));
                connector.SendEngSetEngerySubPacket(serverID, ShipSystems.WarpJumpDrive, 1);
            }
        }
        void ZeroWarpEnergy()
        {
            if (GameInProgress)
            {
                //connector.SendPackage(EngSetEnergySubPacket.GetPacket(ShipSystems.WarpJumpDrive, 0));
                connector.SendEngSetEngerySubPacket(serverID, ShipSystems.WarpJumpDrive, 0);
            }
        }
        void ZeroAllCoolant()
        {
            if (GameInProgress)
            {
                foreach (ShipSystems st in Enum.GetValues(typeof(ShipSystems)))
                {
                    //connector.SendPackage(EngSetCoolantSubPacket.GetPacket(st, 0));
                    connector.SendEngSetCoolantSubPacket(serverID, st, 0);
                }
            }
            
        }
        System.Media.SoundPlayer plyr = new System.Media.SoundPlayer();
        void AnnounceAlert()
        {
            if (GameInProgress)
            {
                plyr.SoundLocation = Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName, "Alert.wav");
                plyr.PlayLooping();
            }
        }

        void ConnectorSubscribe()
        {
            connector.ConnectionLost += connector_ConnectionLost;
            connector.ObjectStatusUpdatePacketReceived += connector_ObjectStatusUpdatePacketReceived;
            connector.GameMessagePacketReceived += connector_GamesMessagePacketReceived;
            connector.GameStartPacketReceived += connector_GameStartPacketReceived;
           
            connector.Connected += connector_Connected;
        }
        void ConnectorUnsubscribe()
        {
            connector.ConnectionLost -= connector_ConnectionLost;
            connector.ObjectStatusUpdatePacketReceived -= connector_ObjectStatusUpdatePacketReceived;
            connector.GameMessagePacketReceived -= connector_GamesMessagePacketReceived;
            connector.Connected -= connector_Connected;
            connector.GameStartPacketReceived -= connector_GameStartPacketReceived;
        }
        void connector_GameStartPacketReceived(object sender, PackageEventArgs e)
        {
            GameInProgress = true;
            View.GameStarted();
        }
        int SelectedShip = 0;
        bool shipSelected = false;
        void connector_GamesMessagePacketReceived(object sender, PackageEventArgs e)
        {
            //GameStart and GameOver are all that matter.
            if (e != null && e.ReceivedPacket != null)
            {
                GameMessagePacket p = e.ReceivedPacket.Package as GameMessagePacket;
                if (p != null)
                {
                    if (p.SubPacketType == GameMessageSubPackets.GameMessageSubPacketTypes.GameEndSubPacket)
                    {
                        plyr.Stop();
                        GameInProgress = false;
                        View.GameEnded();
                    }
                    if (p.SubPacketType == GameMessageSubPackets.GameMessageSubPacketTypes.EndSimulationSubPacket)
                    {
                        plyr.Stop();
                        GameInProgress = false;
                        View.SimulationEnded();
                    }
                    if (p.SubPacketType == GameMessageSubPackets.GameMessageSubPacketTypes.AllShipSettingsSubPacket)
                    {
                        AllShipSettingsSubPacket allships = p.SubPacket as AllShipSettingsSubPacket;
                        if (allships != null && allships.Ships != null && !shipSelected)
                        {
                            
                            SelectedShip = View.GetShipSelection(allships.Ships);
                            shipSelected = true;
                            SelectStationAndReady();
                        }
                    }
                }
            }

        }
        bool GameInProgress = false;
        System.Timers.Timer Ready2Timer = null;
        void StartReady2Timer()
        {
            Ready2Timer = new System.Timers.Timer(15000);
            Ready2Timer.Elapsed += Ready2Timer_Elapsed;
            Ready2Timer.Start();
        }

        void Ready2Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            Ready2Timer.Stop();
            if (GameInProgress)
            {
                SendReady2();
                Ready2Timer.Start();
            }
        }
        Guid serverID;
        void connector_Connected(object sender, ConnectionEventArgs e)
        {
            serverID = e.ID;
            WarningDisconnectedSent = false;
        }

        void connector_ObjectStatusUpdatePacketReceived(object sender, PackageEventArgs e)
        {
            if (GameInProgress)
            {
                if (e != null)
                {

                    if (e.ReceivedPacket != null)
                    {
                        ObjectStatusUpdatePacket objectStat = e.ReceivedPacket.Package as ObjectStatusUpdatePacket;
                        if (objectStat != null)
                        {
                            if (objectStat.SubPacketType == ObjectStatusUpdateSubPacketTypes.MainPlayerUpdateSubPacket)
                            {
                                MainPlayerUpdateSubPacket mainPlayer = objectStat.SubPacket as MainPlayerUpdateSubPacket;
                                if (mainPlayer != null)
                                {
                                    if (mainPlayer.RedAlert != null && (mainPlayer.ShipNumber != null && mainPlayer.ShipNumber == (SelectedShip + 1)) || mainPlayer.ShipNumber == null)
                                    {
                                        redAlertEnabled = Convert.ToBoolean(mainPlayer.RedAlert.Value);
                                        View.RedAlertEnabled = redAlertEnabled;
                                    }
                                    if (mainPlayer.ShieldState != null && (mainPlayer.ShipNumber != null && mainPlayer.ShipNumber == (SelectedShip + 1)) || mainPlayer.ShipNumber == null)
                                    {
                                        shieldsRaised = Convert.ToBoolean(mainPlayer.ShieldState.Value);
                                        View.ShieldsRaised = shieldsRaised;
                                        if (shieldsRaised && ShieldsMustBeDown)
                                        {
                                            SendDropShields();
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
        }
       
        void Connect()
        {

            Disconnect();
            if (!string.IsNullOrEmpty(View.Host) && View.Port > 0)
            {
                connector = new PacketProcessing();


                ConnectorSubscribe();
                connector.SetPort(View.Port);
                connector.SetServerHost(View.Host);
                connector.StartServerConnection();

                
           

            }
            reConnectCount++;
        }
        void SelectStationAndReady()
        {
            if (connector != null)
            {
                //Select Ship

                if (SelectedShip != 0)
                {
                    //@@@@@@
                    connector.SendSetShipSubPacket(serverID, SelectedShip + 1);
                    //connector.SendPackage(SetShipSubPacket.GetPackage(SelectedShip + 1));
                }
                //Select Station
                connector.SendSetStationSubPacket(serverID, StationTypes.Observer, true);
                
                //connector.SendPackage(SetStationSubPacket.GetPacket(StationTypes.Engineering, true));

                //Ready
                
                connector.SendReadySubPacket(serverID);

            }


        }
        void SendReady2()
        {
            if (GameInProgress)
            {
                if (connector != null)
                {
                    connector.SendReady2SubPacket(serverID);
                    //connector.SendPackage(Ready2SubPacket.GetPacket());

                }
                if (WarningDisconnectedSent)
                {
                    Connect();

                }
            }
        }
        bool WarningDisconnectedSent = false;

        int reConnectCount = 0;
        void connector_ConnectionLost(object sender, ConnectionEventArgs e)
        {
            //try to re-establish.
            if (!WarningDisconnectedSent && reConnectCount < 3)
            {
                Ready2Timer.Stop();
                View.ConnectionLostWarning();
                WarningDisconnectedSent = true;
                Ready2Timer.Start();
            }
            else
            {
                View.ConnectionFailed();
            }

            
        }
        

        PacketProcessing connector = null;
        IView View = null;
        bool isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (!isDisposed)
                {
                    if (connector != null)
                    {
                        ConnectorUnsubscribe();
                        connector.Dispose();
                    }
                    UnSubscribe();
                    isDisposed = true;
                }
            }
        }
        
       
    }
}
