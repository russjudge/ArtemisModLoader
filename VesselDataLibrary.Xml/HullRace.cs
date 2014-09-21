using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary;
using RussLibrary.Xml;
using System.Windows;
using RussLibrary.WPF;
using System.Xml;

namespace VesselDataLibrary.Xml
{
    [XmlConversionRoot("hullRace")]
    public class HullRace : ChangeDependencyObject, IXmlStorage
    {

        public HullRace()
        {
            Storage = new List<XmlNode>();
            Taunts = new TauntCollection();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static readonly DependencyProperty IDProperty =
         DependencyProperty.Register("ID", typeof(int),
         typeof(HullRace), new PropertyMetadata(OnItemChanged));
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
         typeof(HullRace), new PropertyMetadata(OnItemChanged));
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
         typeof(HullRace), new PropertyMetadata(OnItemChanged));
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



        public static readonly DependencyProperty TauntsProperty =
         DependencyProperty.Register("Taunts", typeof(TauntCollection),
         typeof(HullRace));
        [XmlConversion("taunt")]
        public TauntCollection Taunts
        {
            get
            {
                return (TauntCollection)this.UIThreadGetValue(TauntsProperty);

            }
            set
            {
                this.UIThreadSetValue(TauntsProperty, value);

            }
        }



        protected override void ProcessValidation()
        {
            if (ID < 0)
            {
                base.ValidationCollection.AddValidation("ID", ValidationValue.IsError,
                     Properties.Resources.HullRaceIDValidation);
            }
            if (string.IsNullOrEmpty(Name))
            {
                base.ValidationCollection.AddValidation(DataStrings.Name, ValidationValue.IsError,
                   Properties.Resources.HullRaceNameValidation);
            }

            //No validation performed on Keys as:
            //  1.  It is more complex
            //  2.  Use of radio buttons prevent invalid combinations.
        }

        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
