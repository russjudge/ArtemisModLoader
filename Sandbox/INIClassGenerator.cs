using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary.Text;
using System.IO;

namespace Sandbox
{

    public static class INIClassGenerator
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(INIClassGenerator));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public static void CreateClass(string INIFile, string NameSpace, string ClassName, string targetPath)
        {
            INIContainer container = new INIContainer(INIFile);
            ClassGenerator cg = new ClassGenerator(NameSpace, ClassName, true, string.Empty, "[INIConversion(\"{0}\")]");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(cg.GetClassPrefix(string.Empty, "ChangeDependencyObject"));
            foreach (INIKeyValueItem item in container.Values.Values)
            {
                string propertyName = item.Key.Substring(0,1).ToUpperInvariant() + item.Key.Substring(1);
                double val = double.NaN;
                string type = "string";

                if (double.TryParse(item.Value, out val))
                {
                    type = "double";
                }

                sb.AppendLine(cg.GetDependencyProperty(propertyName, type, item.Key));
            }
            sb.AppendLine(cg.GetClassSuffix());

            using (StreamWriter sw = new StreamWriter(targetPath))
            {
                sw.Write(sb.ToString());
            }
        }
    }
}
