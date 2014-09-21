using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary.WPF;
using RussLibrary.Xml;
using System.Xml;

namespace VesselDataLibrary.Xml
{

    public class TauntCollection : ChangeDependentCollection<Taunt>, IXmlStorage
    {
        public TauntCollection()
        {
            Storage = new List<XmlNode>();
        }
        static readonly ILog _log = LogManager.GetLogger(typeof(TauntCollection));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        #region IXmlStorage Members

        public IList<System.Xml.XmlNode> Storage { get; private set; }

        #endregion

        protected override void ProcessValidation()
        {
            
        }
    }
}
