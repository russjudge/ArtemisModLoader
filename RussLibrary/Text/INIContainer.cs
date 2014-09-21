using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;

namespace RussLibrary.Text
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
    public class INIContainer
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(INIContainer));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public INIContainer()
        {
            Values = new Dictionary<string, INIKeyValueItem>();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
        public INIContainer(string INIPath)
        {
            Values = new Dictionary<string, INIKeyValueItem>();
            LoadFile(INIPath);
        }
        public void AddEntry(string dataLine)
        {

            UpdateEntry(new INIKeyValueItem(dataLine));
        }
        public void UpdateEntry(INIKeyValueItem item)
        {
            if (item != null && !string.IsNullOrEmpty(item.Key))
            {
                if (Values.ContainsKey(item.Key))
                {
                    Values[item.Key] = item;
                }
                else
                {
                    Values.Add(item.Key, item);
                }
            }
        }
        public void LoadFile(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string s = string.Empty;
                    while (s != null)
                    {
                        s = sr.ReadLine();
                        AddEntry(s);
                    }
                }
            }
        }


        public void SaveFile(string path)
        {

            List<INIKeyValueItem> unusedItems = new List<INIKeyValueItem>(Values.Values);

            

            StringBuilder sb = new StringBuilder();
            List<string> lines = new List<string>();
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string inLine = string.Empty;
                    
                    while (inLine != null)
                    {
                        inLine = sr.ReadLine();
                        if (inLine != null)
                        {
                            lines.Add(inLine);
                        }
                    }
                }
                List<string> newOutput = new List<string>();

                for (int i = lines.Count - 1; i >= 0; i--)
                {
                    INIKeyValueItem item = new INIKeyValueItem(lines[i]);
                    if (item.Key != null)
                    {


                        if (Values.ContainsKey(item.Key))
                        {
                            if (item.UseDefault && !Values[item.Key].UseDefault)
                            {
                                newOutput.Add(lines[i]);
                                // sb.AppendLine(lines[i]);
                            }
                            //sb.AppendLine(Values[item.Key].ToString());
                            newOutput.Add(Values[item.Key].ToString());
                            if (unusedItems.Contains(Values[item.Key]))
                            {
                                unusedItems.Remove(Values[item.Key]);
                            }
                        }
                        else
                        {
                            newOutput.Add(item.ToString());
                            //sb.AppendLine(item.ToString());
                        }
                    }
                    else
                    {
                        //sb.AppendLine(lines[i]);
                        newOutput.Add(lines[i]);
                    }
                    
                }
                string LastEntry = null;
                List<string> Final = new List<string>();
                foreach (string l in newOutput)
                {
                    if (l.Contains('='))
                    {
                        INIKeyValueItem item = new INIKeyValueItem(l);
                        if (item.Key != LastEntry)
                        {
                            Final.Add(l);
                            LastEntry = item.Key;
                        }

                    }
                    else
                    {
                        Final.Add(l);
                    }
                }
                for (int i = Final.Count - 1; i >= 0; i--)
                {
                    sb.AppendLine(Final[i]);
                }


            }
            foreach (INIKeyValueItem item in unusedItems)
            {
                sb.AppendLine(item.ToString());
            }

            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(sb.ToString());
            }
        }
        public Dictionary<string, INIKeyValueItem> Values { get;private set; }
    }
}
