using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using RussLibrary.Xml;
using RussLibrary.WPF;
using System.Globalization;
using System.Xml;

namespace VesselDataLibrary.Xml
{
    [XmlConversionRoot("torpedo_storage")]
    public class TorpedoStorage : ChangeDependencyObject, IXmlStorage
    {
        public TorpedoStorage()
        {
            Storage = new List<XmlNode>();
        }
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


        protected override void ProcessValidation()
        {
            if (TorpedoType < 0 || TorpedoType > 3)
            {
                this.ValidationCollection.AddValidation("TorpedoType", ValidationValue.IsError, 
                Properties.Resources.TorpedoTypeValidation);
            }
            if (Quantity < 0)
            {
                this.ValidationCollection.AddValidation("Quantity", ValidationValue.IsError,
                    string.Format(CultureInfo.CurrentCulture, 
                    Properties.Resources.GreaterThanZero, Properties.Resources.Quantity));
            }
        }

        public IList<XmlNode> Storage { get; private set; }
    }
}
