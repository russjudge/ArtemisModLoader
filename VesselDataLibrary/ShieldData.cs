using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using RussLibrary.Xml;

namespace VesselDataLibrary
{
     [XmlConversionRoot("shields")]
    public class ShieldData : DependencyObject
    {
        //<shields front="80" back="80" />

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


    }
}
