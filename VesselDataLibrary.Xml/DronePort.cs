using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;
using RussLibrary.Xml;
using RussLibrary;
using System.Globalization;
using RussLibrary.WPF;
namespace VesselDataLibrary.Xml
{
    [XmlConversionRoot("drone_port")]
    public class DronePort : VectorObject
    {
         [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DronePort()
         {
            
         }
        /*
         * <beam_port x="-102.14" y="8.35" z="258.74" damage="12" arcwidth="0.4" cycletime="6.0" range="1000"/>
  <beam_port x=" 102.14" y="8.35" z="258.74" damage="12" arcwidth="0.4" cycletime="6.0" range="1000"/>
         * 
         * */

         protected override void OnVectorItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
         {
             
             
             if (VectorItemChanged != null)
             {
                 VectorItemChanged(this, EventArgs.Empty);
             }

         }

         public event EventHandler VectorItemChanged;


         public static readonly DependencyProperty DamageProperty =
             DependencyProperty.Register("Damage", typeof(double),
             typeof(DronePort), new PropertyMetadata(OnItemChanged));
         [XmlConversion("damage")]
         public double Damage
         {
             get
             {
                 return (double)this.UIThreadGetValue(DamageProperty);
             }
             set
             {
                 this.UIThreadSetValue(DamageProperty, value);
             }
         }



         public static readonly DependencyProperty CycleTimeProperty =
             DependencyProperty.Register("CycleTime", typeof(double),
             typeof(DronePort), new PropertyMetadata(OnItemChanged));
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
             DependencyProperty.Register(DataStrings.Range, typeof(int),
             typeof(DronePort), new PropertyMetadata(OnVectorChanged));
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
         protected override void ProcessValidation()
         {
             base.ProcessValidation();
             if (Damage <= 0)
             {
                 base.ValidationCollection.AddValidation("Damage", ValidationValue.IsError,
                   string.Format(CultureInfo.CurrentCulture, 
                   Properties.Resources.GreaterThanZero, Properties.Resources.Damage));
             }
            
             if (CycleTime <= 0)
             {
                 base.ValidationCollection.AddValidation("CycleTime", ValidationValue.IsError,
                    string.Format(CultureInfo.CurrentCulture, 
                    Properties.Resources.GreaterThanZero, Properties.Resources.CycleTime));
             }
             
             if (Range <= 0)
             {
                 base.ValidationCollection.AddValidation(DataStrings.Range, ValidationValue.IsError,
                    string.Format(CultureInfo.CurrentCulture, 
                    Properties.Resources.GreaterThanZero, Properties.Resources.Range));

             }
             else if (Range < 100)
             {
                 base.ValidationCollection.AddValidation(DataStrings.Range, ValidationValue.IsWarnState,
                    Properties.Resources.ShortRange);
                 
             }
         }
    }
}
