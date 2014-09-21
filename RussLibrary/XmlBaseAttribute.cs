using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace RussLibrary
{
    [Obsolete("Use XmlConversionAttribute (with XmlConverter) instead.")]
    [System.AttributeUsage(AttributeTargets.Property)]
    public sealed class XmlBaseAttribute : System.Attribute
    {
       
    }
}
