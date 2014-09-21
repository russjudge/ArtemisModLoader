using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace RussLibrary.Controls
{

    public class FormattedXmlResult
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(FormattedXmlResult));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        internal FormattedXmlResult(string resultXml, string errorMessages, int resultCode)
        {
            ResultXml = resultXml;
            ErrorMessages = errorMessages;
            ResultCode = (FormattedXmlResultCode)resultCode;
        }
        public string ResultXml { get; private set; }
        public string ErrorMessages { get; private set; }
        public FormattedXmlResultCode ResultCode { get; private set; }
    }
}
