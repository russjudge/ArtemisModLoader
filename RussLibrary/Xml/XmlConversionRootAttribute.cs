using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RussLibrary.Xml
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), System.AttributeUsage(AttributeTargets.Class)]
    public sealed class XmlConversionRootAttribute : System.Attribute
    {
        
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Xml")]
        //public XmlConversionRootAttribute(string XmlRootName, bool excludeIfEmptyZeroOrNull)
        //{
        //    RootNodeName = XmlRootName;
        //    ExcludeIfEmptyZeroOrNull = excludeIfEmptyZeroOrNull;
        //}
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Xml")]
        public XmlConversionRootAttribute(string XmlRootNodeName)
        {
            RootNodeName = XmlRootNodeName;
        }
        public bool ExcludeIfEmptyZeroOrNull { get; private set; }
        public string RootNodeName { get; private set; }
    }
}
