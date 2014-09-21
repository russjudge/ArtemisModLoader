using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArtemisComm.ShipAction2SubPackets
{
    public class EngSetCoolantSubPacket : IPackage
    {
        public static Packet GetPacket(ShipSystems system, int value)
        {
            EngSetCoolantSubPacket escsp = new EngSetCoolantSubPacket(system, value);
            ShipAction2Packet sap2 = new ShipAction2Packet(escsp);
            return new Packet(sap2);
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(EngSetCoolantSubPacket));
        public EngSetCoolantSubPacket(ShipSystems system, int value)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            System = system;
            Value = value;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }   
        }
        public EngSetCoolantSubPacket(byte[] byteArray)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            if (_log.IsInfoEnabled) { _log.InfoFormat("{0}--bytes in: {1}", MethodBase.GetCurrentMethod().ToString(), Utility.BytesToDebugString(byteArray)); }


            System = (ShipSystems)BitConverter.ToInt32(byteArray, 0);
            Value = BitConverter.ToInt32(byteArray, 4);
            


            if (_log.IsInfoEnabled) { _log.InfoFormat("{0}--Result bytes: {1}", MethodBase.GetCurrentMethod().ToString(), Utility.BytesToDebugString(this.GetBytes())); }

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }   
        }
        public ShipSystems System { get; set; }
        
        public int Value
        {
            get;
            set;
        }
        

        public byte[] GetBytes()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BitConverter.GetBytes((int)System));
            retVal.AddRange(BitConverter.GetBytes(Value));
            return retVal.ToArray();
        }
    }
}