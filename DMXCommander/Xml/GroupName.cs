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
 
    public class GroupName: ChangeDependencyObject, IXmlStorage
    {

        public GroupName()
        {
            Storage = new List<XmlNode>();
            
        }




        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string),
            typeof(GroupName), new PropertyMetadata(OnItemChanged));
        [XmlConversion("Name")]
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




        protected override void ProcessValidation()
        {
        }
        public IList<System.Xml.XmlNode> Storage { get; private set; }
    }
}
