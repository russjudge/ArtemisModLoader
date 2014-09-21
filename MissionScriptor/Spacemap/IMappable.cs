using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MissionStudio.Spacemap
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mappable")]
    public interface IMappable
    {
        //Don't forget that an expression can be used here--thus these won't map if an expression is here.
        //  Use NaN if not mappable due to expression being used.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
        double X { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
        double Y { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Z")]
        double Z { get; set; }
        string ObjectName { get; set; }
        SpaceObjectType ObjectType { get; set; }
        IList Attributes { get; set; }
    }
}
