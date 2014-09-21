using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using RussLibrary.WPF;
using RussLibrary.Xml;

namespace VesselDataLibrary.Xml
{
    public class BeamPortCollection : ChangeDependentCollection<BeamPort>, IXmlStorage
    {
        public IList<System.Xml.XmlNode> Storage { get; private set; }
        public BeamPortCollection()
        {
            Storage = new List<XmlNode>();
        }


        protected override void ProcessValidation()
        {

        }
    }
}
