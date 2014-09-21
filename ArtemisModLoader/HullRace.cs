using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary;
using System.Xml;
using System.Windows;

namespace ArtemisModLoader
{
    public class HullRace : XmlBase
    {
        /*
         * Sample Xml from file:
         * <hullRace ID="0" name="USFP" keys="player"/>
         * */
        public HullRace(XmlNode node) : base(node)
        { 
        }


        public static readonly DependencyProperty IDProperty =
         DependencyProperty.Register("ID", typeof(int),
         typeof(HullRace), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public int ID
        {
            get
            {
                return (int)this.UIThreadGetValue(IDProperty);

            }
            private set
            {
                this.UIThreadSetValue(IDProperty, value);

            }
        }


        public static readonly DependencyProperty nameProperty =
         DependencyProperty.Register("name", typeof(string),
         typeof(HullRace), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public string name
        {
            get
            {
                return (string)this.UIThreadGetValue(nameProperty);

            }
            private set
            {
                this.UIThreadSetValue(nameProperty, value);

            }
        }


        public static readonly DependencyProperty keysProperty =
         DependencyProperty.Register("keys", typeof(string),
         typeof(HullRace), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public string keys
        {
            get
            {
                return (string)this.UIThreadGetValue(keysProperty);

            }
            private set
            {
                this.UIThreadSetValue(keysProperty, value);

            }
        }
    }
}
