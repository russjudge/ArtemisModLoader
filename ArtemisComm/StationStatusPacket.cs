using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArtemisComm
{
    public class StationStatusPacket : IPackage
    {

        //**CONFIRMED
        static readonly ILog _log = LogManager.GetLogger(typeof(StationStatusPacket));   
        public StationStatusPacket()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }   

        }
        public StationStatusPacket(byte[] byteArray)
        {
            if (byteArray != null)
            {
                if (_log.IsInfoEnabled) { _log.InfoFormat("{0}--bytes in: {1}", MethodBase.GetCurrentMethod().ToString(), Utility.BytesToDebugString(byteArray)); }


                ShipNumber = BitConverter.ToInt32(byteArray, 0);
                if (_log.IsInfoEnabled) { _log.InfoFormat("ShipNumber={0}", ShipNumber); }
                List<BridgeStationStatuses> stat = new List<BridgeStationStatuses>();

                if (byteArray.Length > 4)
                {
                    MainScreen = (BridgeStationStatuses)byteArray[4];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("MainScreen={0}", MainScreen); }
                }
                if (byteArray.Length > 5)
                {
                    Helm = (BridgeStationStatuses)byteArray[5];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("Helm={0}", Helm); }
                }
                if (byteArray.Length > 6)
                {
                    Weapons = (BridgeStationStatuses)byteArray[6];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("Weapons={0}", Weapons); }
                }
                if (byteArray.Length > 7)
                {
                    Engineering = (BridgeStationStatuses)byteArray[7];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("Engineering={0}", Engineering); }
                }
                if (byteArray.Length > 8)
                {
                    Science = (BridgeStationStatuses)byteArray[8];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("Science={0}", Science); }
                }
                if (byteArray.Length > 9)
                {
                    Communications = (BridgeStationStatuses)byteArray[9];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("Communications={0}", Communications); }
                }

                if (byteArray.Length > 10)
                {
                    Observer = (BridgeStationStatuses)byteArray[10];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("Observer={0}", Observer); }
                }

                if (byteArray.Length > 11)
                {
                    CaptainMap = (BridgeStationStatuses)byteArray[11];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("CaptainMap={0}", CaptainMap); }
                }

                if (byteArray.Length > 12)
                {
                    GameMaster = (BridgeStationStatuses)byteArray[12];
                    if (_log.IsInfoEnabled) { _log.InfoFormat("GameMaster={0}", GameMaster); }
                }
                if (_log.IsInfoEnabled) { _log.InfoFormat("{0}--Result bytes: {1}", MethodBase.GetCurrentMethod().ToString(), Utility.BytesToDebugString(this.GetBytes())); }
            }
        }
        public byte[] GetBytes()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BitConverter.GetBytes(ShipNumber));
            retVal.Add((byte)MainScreen);
            retVal.Add((byte)Helm);
            retVal.Add((byte)Weapons);
            retVal.Add((byte)Engineering);
            retVal.Add((byte)Science);
            retVal.Add((byte)Communications);
            retVal.Add((byte)Observer);
            retVal.Add((byte)CaptainMap);
            retVal.Add((byte)GameMaster);
            
            return retVal.ToArray();
        }

        /// <summary>
        /// Gets or sets the ship number.  One based.
        /// </summary>
        /// <value>
        /// The ship number.
        /// </value>
        public int ShipNumber { get; set; }

        public BridgeStationStatuses MainScreen { get; set; }
        public BridgeStationStatuses Helm { get; set; }
        public BridgeStationStatuses Weapons { get; set; }
        public BridgeStationStatuses Engineering { get; set; }
        public BridgeStationStatuses Science { get; set; }
        public BridgeStationStatuses Communications { get; set; }
        public BridgeStationStatuses Observer { get; set; }
        public BridgeStationStatuses CaptainMap { get; set; }
        public BridgeStationStatuses GameMaster { get; set; }

    }
}
