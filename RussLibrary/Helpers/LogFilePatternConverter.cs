using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace RussLibrary.Helpers
{

    public class LogFilePatternConverter : log4net.Util.PatternConverter
    {


        protected override void Convert(System.IO.TextWriter writer, object state)
        {
            if (writer != null)
            {
                writer.Write(System.IO.Path.Combine(GeneralHelper.ApplicationDataPath, "app.log"));
            }
        }
    }
}
