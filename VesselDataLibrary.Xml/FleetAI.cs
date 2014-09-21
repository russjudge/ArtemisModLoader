using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary;
using RussLibrary.Xml;
using RussLibrary.WPF;
using System.Windows;
using System.Xml;
namespace VesselDataLibrary.Xml
{
    //[XmlConversionRoot("fleet_ai", true)]
    [XmlConversionRoot("fleet_ai")]
    public class FleetAI : ChangeDependencyObject, IXmlStorage
    {
        public FleetAI()
        {
            Storage = new List<XmlNode>();
        }
        public static readonly DependencyProperty CommonalityProperty =
            DependencyProperty.Register("Commonality", typeof(int),
            typeof(FleetAI), new PropertyMetadata(OnItemChanged));
        //[XmlConversion("commonality", true)]
        [XmlConversion("commonality")]
        public int Commonality
        {
            get
            {
                return (int)this.UIThreadGetValue(CommonalityProperty);
            }
            set
            {
                this.UIThreadSetValue(CommonalityProperty, value);
            }
        }
        protected override void ProcessValidation()
        {
            if (Commonality < 0)
            {
                base.ValidationCollection.AddValidation("Commonality", ValidationValue.IsError,
                     "Must be greater than or equal to zero.");
            }
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
