using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace RussLibrary.Xml
{
    [Serializable]
    public class XmlConversionException : Exception
    {
        public XmlConversionException() : base("Invalid Conversion") { }
        public XmlConversionException(string message) : base(message) { }
        protected XmlConversionException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public XmlConversionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
