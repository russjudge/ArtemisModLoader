using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary;
using RussLibrary.Xml;
using System.Windows;
using RussLibrary.WPF;
using System.Globalization;
using System.Xml;

namespace VesselDataLibrary.Xml
{
     [XmlConversionRoot("beam_port")]
    public class BeamPort : DronePort, IXmlStorage
    {
         [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
         public BeamPort()
         {
             Storage = new List<XmlNode>();
         }
        /*
         * <beam_port x="-102.14" y="8.35" z="258.74" damage="12" arcwidth="0.4" cycletime="6.0" range="1000"/>
  <beam_port x=" 102.14" y="8.35" z="258.74" damage="12" arcwidth="0.4" cycletime="6.0" range="1000"/>
         * 
         * */

        


         public static readonly DependencyProperty ArcWidthProperty =
             DependencyProperty.Register("ArcWidth", typeof(double),
             typeof(BeamPort), new PropertyMetadata(OnVectorChanged));
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

         protected override void ProcessValidation()
         {
             base.ProcessValidation();
             
             if (ArcWidth <= 0)
             {
                 base.ValidationCollection.AddValidation(DataStrings.ArcWidth, ValidationValue.IsError,
                    string.Format(CultureInfo.CurrentCulture, 
                    Properties.Resources.GreaterThanZero, Properties.Resources.ArcWidth));
             }
            
         }

       
    }
}
