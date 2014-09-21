using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary.WPF;
using RussLibrary.Xml;
using RussLibrary;
using System.Xml;
namespace VesselDataLibrary.Xml
{

    public class ArtDefinitionCollection : ChangeDependentCollection<ArtDefinition>, IXmlStorage
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(ArtDefinitionCollection));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public IList<System.Xml.XmlNode> Storage { get; private set; }
        public ArtDefinitionCollection()
        {
             Storage = new List<XmlNode>();
        }

        protected override void ProcessValidation()
        {
      
        }
    }
}
