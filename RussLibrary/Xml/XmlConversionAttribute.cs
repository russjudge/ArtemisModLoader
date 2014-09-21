using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace RussLibrary.Xml
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), System.AttributeUsage(AttributeTargets.Property)]
    public sealed class XmlConversionAttribute : System.Attribute
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Xml")]
        public XmlConversionAttribute(string XmlNodeNameMap)
        {
            NodeNameMap = XmlNodeNameMap;
        }
        //public XmlConversionAttribute(params object[] match)
        //{
        //}
        //public ReadOnlyCollection<DictionaryEntry>
        public string NodeNameMap { get; private set; }

        public bool ExcludeIfEmptyZeroOrNull { get; private set; }
    }
}
