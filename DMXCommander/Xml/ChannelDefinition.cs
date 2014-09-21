using RussLibrary.WPF;
using RussLibrary.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;
using RussLibrary;

namespace DMXCommander.Xml
{
    [XmlConversionRoot("Definitions")]
    public class ChannelDefinition: ChangeDependencyObject, IXmlStorage
    {

        public ChannelDefinition()
        {
            Storage = new List<XmlNode>();
            
        }



        public static readonly DependencyProperty ChannelProperty =
            DependencyProperty.Register("Channel", typeof(int),
            typeof(ChannelDefinition), new PropertyMetadata(OnItemChanged));
        [XmlConversion("Channel")]
        public int Channel
        {
            get
            {
                return (int)this.UIThreadGetValue(ChannelProperty);

            }
            set
            {
                this.UIThreadSetValue(ChannelProperty, value);

            }
        }



        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
            typeof(ChannelDefinition), new PropertyMetadata(OnItemChanged));
        [XmlConversion("Label")]
        public string Label
        {
            get
            {
                return (string)this.UIThreadGetValue(LabelProperty);

            }
            set
            {
                this.UIThreadSetValue(LabelProperty, value);

            }
        }

        public static readonly DependencyProperty TestValueProperty =
        DependencyProperty.Register("TestValue", typeof(byte),
        typeof(ChannelDefinition));
        
        public byte TestValue
        {
            get
            {
                return (byte)this.UIThreadGetValue(TestValueProperty);

            }
            set
            {
                this.UIThreadSetValue(TestValueProperty, value);

            }
        }


        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register("Group", typeof(string),
            typeof(ChannelDefinition), new PropertyMetadata(OnItemChanged));
        [XmlConversion("Group")]
        public string Group
        {
            get
            {
                return (string)this.UIThreadGetValue(GroupProperty);

            }
            set
            {
                this.UIThreadSetValue(GroupProperty, value);

            }
        }





        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Fireable")]
        public static readonly DependencyProperty RealTimeFireableProperty =
            DependencyProperty.Register("RealTimeFireable", typeof(bool),
            typeof(ChannelDefinition), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Fireable"), XmlConversion("RealTimeFireable")]
        public bool RealTimeFireable
        {
            get
            {
                return (bool)this.UIThreadGetValue(RealTimeFireableProperty);

            }
            set
            {
                this.UIThreadSetValue(RealTimeFireableProperty, value);

            }
        }






        protected override void ProcessValidation()
        {
        }
        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
