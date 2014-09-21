using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using RussLibrary.Xml;
using RussLibrary.WPF;
using System.Windows;
using RussLibrary;
using System.Xml;

namespace VesselDataLibrary.Xml
{

    [XmlConversionRoot("carrier")]
    public class Carrier : ChangeDependencyObject, IXmlStorage
    {
        public Carrier()
        {
            Storage = new List<XmlNode>();
        }
        public static readonly DependencyProperty ComplimentProperty =
            DependencyProperty.Register("Compliment", typeof(int),
            typeof(Carrier), new PropertyMetadata(OnItemChanged));
      
        [XmlConversion("compliment")]
        public int Compliment
        {
            get
            {
                return (int)this.UIThreadGetValue(ComplimentProperty);
            }
            set
            {
                this.UIThreadSetValue(ComplimentProperty, value);
            }
        }
        protected override void ProcessValidation()
        {
            if (Compliment < 0)
            {
                base.ValidationCollection.AddValidation("Compliment", ValidationValue.IsError,
                     "Must be greater than or equal to zero.");
            }
        }

        public IList<XmlNode> Storage { get; private set; }
    }
}

