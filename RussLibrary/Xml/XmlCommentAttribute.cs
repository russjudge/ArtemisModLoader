using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace RussLibrary.Xml
{
    [System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class XmlCommentAttribute : Attribute
    {
        public XmlCommentAttribute(string comment)
        {
            Comment = comment;
        }
        public string Comment { get; private set; }
    }
}
