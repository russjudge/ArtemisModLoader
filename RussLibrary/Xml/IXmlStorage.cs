using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RussLibrary.Xml
{
    public interface IXmlStorage
    {
        IList<XmlNode> Storage { get; }
    }
}
