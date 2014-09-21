using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ArtemisComm
{
    public class VersionPacket : IPackage
    {
        //**CONFIRMED
        static readonly ILog _log = LogManager.GetLogger(typeof(VersionPacket));   
        public VersionPacket()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }   

            Version = 2.0F;  //Current version of Artemis as of 12/5/2013.
        }
        public VersionPacket(byte[] byteArray)
        {
            if (byteArray != null)
            {
                if (_log.IsInfoEnabled) { _log.InfoFormat("{0}--bytes in: {1}", MethodBase.GetCurrentMethod().ToString(), Utility.BytesToDebugString(byteArray)); }


                if (byteArray.Length > 3)  //Protection in case of bad packet.
                {
                    Unknown = BitConverter.ToInt32(byteArray, 0);
                    if (_log.IsInfoEnabled) { _log.InfoFormat("Unknown={0}", Unknown); }
                }
                if (byteArray.Length > 7)  //Protection in case of bad packet.
                {
                    Version = BitConverter.ToSingle(byteArray, 4);
                    Packet.CurrentActiveArtemisVersion = Version;
                    if (_log.IsInfoEnabled) { _log.InfoFormat("Version={0}", Version); }
                }
                if (_log.IsInfoEnabled) { _log.InfoFormat("{0}--Result bytes: {1}", MethodBase.GetCurrentMethod().ToString(), Utility.BytesToDebugString(this.GetBytes())); }
            }
        }
        public byte[] GetBytes()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BitConverter.GetBytes(Unknown));
            retVal.AddRange(BitConverter.GetBytes(Version));
            return retVal.ToArray();
        }
        public int Unknown { get; set; }
        public float Version { get; set; }
//Unknown (int)

//Version (float)
    }
}
