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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "DMX"), 
    XmlConversionRoot("DMXConfigurationFile")]
    public class DMXConfigurationFile: ChangeDependencyObject, IXmlStorage
    {
        public DMXConfigurationFile()
        {
            Storage = new List<XmlNode>();
            Definitions = new ChannelDefinitionCollection();
            Groups = new GroupNameCollection();
            
        }




        public static readonly DependencyProperty GroupsProperty =
           DependencyProperty.Register("Groups", typeof(GroupNameCollection),
           typeof(DMXCommandFile), new PropertyMetadata(OnItemChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"),
        XmlConversion("Groups")]
        public GroupNameCollection Groups
        {
            get
            {
                return (GroupNameCollection)this.UIThreadGetValue(GroupsProperty);
            }
            set
            {
                this.UIThreadSetValue(GroupsProperty, value);
            }

        }

        public static readonly DependencyProperty DefinitionsProperty =
           DependencyProperty.Register("Definitions", typeof(ChannelDefinitionCollection),
           typeof(DMXCommandFile), new PropertyMetadata(OnItemChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"),
        XmlConversion("Definitions")]
        public ChannelDefinitionCollection Definitions
        {
            get
            {
                return (ChannelDefinitionCollection)this.UIThreadGetValue(DefinitionsProperty);
            }
            set
            {
                this.UIThreadSetValue(DefinitionsProperty, value);
            }

        }
        public IList<System.Xml.XmlNode> Storage { get; private set; }

        protected override void ProcessValidation()
        {
            
        }
        static string ConfigPath = System.IO.Path.Combine(RussLibrary.Helpers.GeneralHelper.ApplicationDataPath, "config.xml");
        static DMXConfigurationFile _current = null;
        public static void Save()
        {
            XmlDocument doc = XmlConverter.ToXmlDocument(_current, true);
            doc.Save(ConfigPath);
        }
        public static DMXConfigurationFile Current
        {
            get
            {
                if (_current == null)
                {
                    if (System.IO.File.Exists(ConfigPath))
                    {
                        _current = XmlConverter.ToObject(ConfigPath, typeof(DMXConfigurationFile)) as DMXConfigurationFile;
                    }
                    else
                    {
                        _current = new DMXConfigurationFile();
                    }
            
                }
                return _current;
            }
        }
    }
}
