using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary;
using RussLibrary.Xml;
using System.Windows;

namespace VesselDataLibrary
{
     [XmlConversionRoot("beam_port")]
    public class BeamPort : VectorObject
    {
        /*
         * <beam_port x="-102.14" y="8.35" z="258.74" damage="12" arcwidth="0.4" cycletime="6.0" range="1000"/>
  <beam_port x=" 102.14" y="8.35" z="258.74" damage="12" arcwidth="0.4" cycletime="6.0" range="1000"/>
         * 
         * */

         public static readonly DependencyProperty DamageProperty =
             DependencyProperty.Register("Damage", typeof(int),
             typeof(BeamPort));
         [XmlConversion("damage")]
         public int Damage
         {
             get
             {
                 return (int)this.UIThreadGetValue(DamageProperty);
             }
             set
             {
                 this.UIThreadSetValue(DamageProperty, value);
             }
         }


         public static readonly DependencyProperty ArcWidthProperty =
             DependencyProperty.Register("ArcWidth", typeof(double),
             typeof(BeamPort));
         [XmlConversion("arcwidth")]
         public double ArcWidth
         {
             get
             {
                 return (double)this.UIThreadGetValue(ArcWidthProperty);
             }
             set
             {
                 this.UIThreadSetValue(ArcWidthProperty, value);
             }
         }


         public static readonly DependencyProperty CycleTimeProperty =
             DependencyProperty.Register("CycleTime", typeof(double),
             typeof(BeamPort));
         [XmlConversion("cycletime")]
         public double CycleTime
         {
             get
             {
                 return (double)this.UIThreadGetValue(CycleTimeProperty);
             }
             set
             {
                 this.UIThreadSetValue(CycleTimeProperty, value);
             }
         }


         public static readonly DependencyProperty RangeProperty =
             DependencyProperty.Register("Range", typeof(int),
             typeof(BeamPort));
         [XmlConversion("range")]
         public int Range
         {
             get
             {
                 return (int)this.UIThreadGetValue(RangeProperty);
             }
             set
             {
                 this.UIThreadSetValue(RangeProperty, value);
             }
         }
    }
}
