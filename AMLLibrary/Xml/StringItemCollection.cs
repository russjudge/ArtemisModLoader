using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary.Xml;
using System.Xml;

namespace ArtemisModLoader.Xml
{
    public class StringItemCollection : ChangeDependentCollection<StringItem>, IXmlStorage
    {
        public StringItemCollection()
        {

            Storage = new List<XmlNode>();
        }
        public IList<System.Xml.XmlNode> Storage { get; private set; }
        protected override void ProcessValidation()
        {
           
        }
    }
}
