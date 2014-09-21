using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using RussLibrary.Xml;

namespace VesselDataLibrary
{
    [XmlConversionRoot("torpedo_storage")]
    public class TorpedoStorage : DependencyObject
    {
  //      <torpedo_storage type="0" amount="8" />  <!-- Type 1 Homing"-->
  //<torpedo_storage type="1" amount="2" />  <!-- Type 4 LR Nuke-->
  //<torpedo_storage type="2" amount="6" />  <!-- Type 6 Mine"-->
  //<torpedo_storage type="3" amount="4" />  <!-- Type 9 ECM"-->
        public static readonly DependencyProperty TorpedoTypeProperty =
            DependencyProperty.Register("TorpedoType", typeof(int),
            typeof(TorpedoStorage));
        [XmlConversion("type")]
        public int TorpedoType
        {
            get
            {
                return (int)this.UIThreadGetValue(TorpedoTypeProperty);
            }
            set
            {
                this.UIThreadSetValue(TorpedoTypeProperty, value);
            }
        }


        public static readonly DependencyProperty QuantityProperty =
            DependencyProperty.Register("Quantity", typeof(int),
            typeof(TorpedoStorage));
        [XmlConversion("amount")]
        public int Quantity
        {
            get
            {
                return (int)this.UIThreadGetValue(QuantityProperty);
            }
            set
            {
                this.UIThreadSetValue(QuantityProperty, value);
            }
        }

    }
}
