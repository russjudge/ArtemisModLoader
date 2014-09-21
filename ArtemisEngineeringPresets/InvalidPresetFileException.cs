using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Runtime.Serialization;

namespace ArtemisEngineeringPresets
{
    [Serializable]
    public class InvalidPresetFileException :Exception
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(InvalidPresetFileException));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public InvalidPresetFileException() : base("Preset file is invalid") { }
        public InvalidPresetFileException(string message) : base(message) { }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public InvalidPresetFileException(string message, string file) : base(string.Format("File: {0}: {1}", file, message)) { }
        protected InvalidPresetFileException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public InvalidPresetFileException(string message, Exception ex) : base(message, ex) { }
    }
}
