using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary;
using RussLibrary.Xml;
using System.Windows;

namespace VesselDataLibrary
{
    [XmlConversionRoot("hullRace")]
    public class HullRace : DependencyObject
    {



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static readonly DependencyProperty IDProperty =
         DependencyProperty.Register("ID", typeof(int),
         typeof(HullRace));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID"), XmlConversion("ID")]
        public int ID
        {
            get
            {
                return (int)this.UIThreadGetValue(IDProperty);

            }
            set
            {
                this.UIThreadSetValue(IDProperty, value);

            }
        }


        public static readonly DependencyProperty NameProperty =
         DependencyProperty.Register("Name", typeof(string),
         typeof(HullRace));
        [XmlConversion("name")]
        public string Name
        {
            get
            {
                return (string)this.UIThreadGetValue(NameProperty);

            }
            set
            {
                this.UIThreadSetValue(NameProperty, value);

            }
        }


        public static readonly DependencyProperty KeysProperty =
         DependencyProperty.Register("Keys", typeof(string),
         typeof(HullRace));
        [XmlConversion("keys")]
        public string Keys
        {
            get
            {
                return (string)this.UIThreadGetValue(KeysProperty);

            }
            set
            {
                this.UIThreadSetValue(KeysProperty, value);

            }
        }
    }
}
