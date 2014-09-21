using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using RussLibrary.Xml;
namespace VesselDataLibrary
{
  
    public class VectorObject : DependencyObject
    {
        //x="-102.14" y="8.35" z="258.74"

  //      <torpedo_tube x="0" y="8.35" z="258.74"/>
  //<torpedo_tube x="0" y="8.35" z="258.74"/>

  //      <engine_port x=" 82.93" y="5" z="-240.89" />
  //<engine_port x="-82.93" y="5" z="-240.89" />
  //<engine_port x="0" y="-9.22" z="-300" />
  //<engine_port x="0" y="29.64" z="-300" />


        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double),
            typeof(VectorObject));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X"), XmlConversion("x")]
        public double X
        {
            get
            {
                return (double)this.UIThreadGetValue(XProperty);

            }
            set
            {
                this.UIThreadSetValue(XProperty, value);

            }
        }


        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double),
            typeof(VectorObject));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y"), XmlConversion("y")]
        public double Y
        {
            get
            {
                return (double)this.UIThreadGetValue(YProperty);

            }
            set
            {
                this.UIThreadSetValue(YProperty, value);

            }
        }



        public static readonly DependencyProperty ZProperty =
            DependencyProperty.Register("Z", typeof(double),
            typeof(VectorObject));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Z"), XmlConversion("z")]
        public double Z
        {
            get
            {
                return (double)this.UIThreadGetValue(ZProperty);

            }
            set
            {
                this.UIThreadSetValue(ZProperty, value);

            }
        }

    }
}
