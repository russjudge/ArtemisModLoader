using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using RussLibrary.Xml;
using RussLibrary.WPF;
using ArtemisModLoader;
using System.Globalization;
using System.Xml;

namespace VesselDataLibrary.Xml
{
     [XmlConversionRoot("shields")]
    public class ShieldData : ChangeDependencyObject, IXmlStorage 
    {
        //<shields front="80" back="80" />
         public ShieldData()
         {
             Storage = new List<XmlNode>();
         }
        public static readonly DependencyProperty FrontProperty =
            DependencyProperty.Register("Front", typeof(int),
            typeof(ShieldData));
        [XmlConversion("front")]
        public int Front
        {
            get
            {
                return (int)this.UIThreadGetValue(FrontProperty);
            }
            set
            {
                this.UIThreadSetValue(FrontProperty, value);
            }
        }


        public static readonly DependencyProperty BackProperty =
            DependencyProperty.Register("Back", typeof(int),
            typeof(ShieldData));
        [XmlConversion("back")]
        public int Back
        {
            get
            {
                return (int)this.UIThreadGetValue(BackProperty);
            }
            set
            {
                this.UIThreadSetValue(BackProperty, value);
            }
        }



        protected override void ProcessValidation()
        {
            if (Front < 0)
            {
                this.ValidationCollection.AddValidation(DataStrings.Front, ValidationValue.IsError,
                    string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.GreaterThanZero,
                    AMLResources.Properties.Resources.FrontShield));
            }
            if (Back < 0)
            {
                this.ValidationCollection.AddValidation(DataStrings.Back, ValidationValue.IsError,
                  string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.GreaterThanZero,
                  AMLResources.Properties.Resources.BackShield));
            }
        }

        public IList<XmlNode> Storage { get; private set; }
    }
}
