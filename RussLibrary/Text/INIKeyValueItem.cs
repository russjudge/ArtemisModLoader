using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace RussLibrary.Text
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
    public class INIKeyValueItem
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(INIKeyValueItem));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public INIKeyValueItem(string key, string value, bool useDefault)
        {
            Key = key;
            Value = value;
            UseDefault = useDefault;
        }
        public INIKeyValueItem(string dataLine)
        {
            KeyValuePair<string, string> wrkData = new KeyValuePair<string, string>();
            if (!string.IsNullOrEmpty(dataLine))
            {
                if (dataLine.Length > 2)
                {
                    if (dataLine[0] == ';')
                    {
                        if (dataLine[1] != ' ')
                        {
                            UseDefault = true;
                            wrkData = ItemPair(dataLine.Substring(1));

                            Key = wrkData.Key;
                            Value = wrkData.Value;
                        }
                        else
                        {
                            Key = null;
                            Value = null;
                            UseDefault = false;
                        }
                    }
                    else
                    {
                        wrkData = ItemPair(dataLine);
                        Key = wrkData.Key;
                        Value = wrkData.Value;
                        UseDefault = false;
                    }

                }
                else
                {
                    Key = null;
                    Value = null;
                    UseDefault = false;
                }
            }
            else
            {
                Key = null;
                Value = null;
                UseDefault = false;
            }

        }
        static KeyValuePair<string, string> ItemPair(string line)
        {
            KeyValuePair<string, string> retVal = new KeyValuePair<string, string>();
            if (line.Contains('='))
            {
                string[] data = line.Split('=');
                if (data.Length > 2)
                {
                    for (int i = 2; i < data.Length; i++)
                    {
                        data[1] += "=" + data[i];  //In case there were unexpected equals signs.
                    }
                }
                retVal = new KeyValuePair<string, string>(data[0], data[1]);
            }
            else
            {
                retVal = new KeyValuePair<string, string>(null, null);
            }
            return retVal;
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool UseDefault { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (UseDefault)
            {
                sb.Append(";");
            }
            if (Key != null)
            {
                sb.Append(Key);
                sb.Append("=");

                sb.Append(Value);

            }
            return sb.ToString();
        }
    }
}
