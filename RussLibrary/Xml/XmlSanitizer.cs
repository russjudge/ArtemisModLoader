using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;
using System.Xml;

namespace RussLibrary.Xml
{

    /// <summary>
    /// Removes comments and invalid characters from XML.
    /// </summary>
    public static class XmlSanitizer
    {
        public static string GetXmlString(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            string retVal =  LoadString(path);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        static string ReadPath(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string data = null;
            using (StreamReader sr = new StreamReader(path))
            {
                data = sr.ReadToEnd();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return data;
        }
        static string LoadString(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string data = ReadPath(path);
           
            StringBuilder sb = new StringBuilder(data.Length);
            int i = 0;
           
            bool withinQuote = false;

            do
            {
                //Looing for comments
                if (!withinQuote)
                {
                    i = ProcessComment(i, data, sb);
                    

                    if (data[i] == '\"')
                    {
                        withinQuote = !withinQuote;
                    }
                }
                else
                {
                    
                    //within quote.  Need to remove invalid characters.
                    if (data[i] == '&')
                    {
                        //check if is "&amp;" or one of the other valid escapes--if it is we are done, else replace with &amp;.
                        CheckAmp(i, data, sb);

                    }
                    else
                    {
                        AddSpecial(data[i], sb);  
                    }
                    if (data[i] == '\"')
                    {
                        withinQuote = !withinQuote;
                    }
                }
            } while (++i < data.Length);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return sb.ToString();
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(XmlSanitizer));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)")]
        static int ProcessComment(int i, string data, StringBuilder sb)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool skipCode = false;
            if (data[i] == '<')
            {
                //<!--
                int j = i + 1;
                while (++j < data.Length && j < i + 3) { }
                int k = j + 1;
                int l = k;
                if (data.Substring(i, j - i + 1) == "<!--")  //Comment start.
                {

                    while (k < data.Length && l < data.Length && data.Substring(k, l - k + 1) != "-->")
                    {
                        k++;
                        l = k + 2;
                        if (l > data.Length)
                        {
                            l = data.Length;
                        }
                        if (k > data.Length)
                        {
                            k = data.Length;
                        }


                    }
                    if (l < data.Length)
                    {
                        string wrk = data.Substring(i, l - i + 1);
                        string pre = wrk.Substring(0, 4);
                        string mid = wrk.Substring(4, wrk.Length - 7).Replace("--", string.Empty);
                        string suff = wrk.Substring(wrk.Length - 3);
                        wrk = pre + mid + suff;

                        string[] wrk2 = wrk.Split('\r');
                        List<string> www = new List<string>();
                        foreach (string w in wrk2)
                        {
                            if (w.EndsWith("-", StringComparison.OrdinalIgnoreCase) && !w.EndsWith("<!--", StringComparison.OrdinalIgnoreCase))
                            {
                                www.Add(w.Substring(0, w.Length - 1));
                            }
                            else
                            {
                                www.Add(w);
                            }
                        }

                        sb.Append(string.Join("\r", www.ToArray()));
                        i = l;
                        skipCode = true;
                    }
                }
            }


            if (!skipCode)
            {


                sb.Append(data[i]);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return i;
        }
        static void CheckAmp(int i, string data, StringBuilder sb)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool isOkay = false;
            //check if is "&amp;" or one of the other valid escapes--if it is we are done, else replace with &amp;.
            int j = i;
            while (++j < data.Length && data[j] != ';' && data[j] != '\"')  //find ";" or quote.
            { }

            if (j < data.Length)
            {
                if (data[j] == ';')
                {
                    string wrk = data.Substring(i, j - i + 1).ToUpperInvariant();
                    isOkay = (wrk == "&AMP;" || wrk == "&GT;" || wrk == "&LT;" || wrk == "&QUOT;" || wrk == "&APOS;");

                }
            }
            if (!isOkay)
            {
                sb.Append("&amp;");
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void AddSpecial(char c, StringBuilder sb)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            switch (c)
            {
                case '>':
                    sb.Append("&gt;");
                    break;
                case '<':
                    sb.Append("&lt;");
                    break;
                case '\'':
                    sb.Append("&apos;");
                    break;
                default:
                    sb.Append(c);
                    break;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
    }
}
