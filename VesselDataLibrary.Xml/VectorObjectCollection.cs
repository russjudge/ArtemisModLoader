using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary.Xml;
using System.Xml;

namespace VesselDataLibrary.Xml
{
    public class VectorObjectCollection : ChangeDependentCollection<VectorObject>, IXmlStorage
    {
        public IList<System.Xml.XmlNode> Storage { get; private set; }
        public VectorObjectCollection()
        {
             Storage = new List<XmlNode>();
        }

        protected override void ProcessValidation()
        {
      
        }
    }
}
