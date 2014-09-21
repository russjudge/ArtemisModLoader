using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;

namespace RussLibrary.Text
{

    /// <summary>
    /// should only be used on nullable types, unless the INI file the property is for will always be stored in the file.
    ///    a null value on the property will force the default (commented-out) in the INI file.
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments")]
    public sealed class INIConversionAttribute : System.Attribute
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Parameter"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
        public INIConversionAttribute(string ParameterName)
        {
            INIParameterName = ParameterName;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
        public string INIParameterName { get; private set; }
    }
}
