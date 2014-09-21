using System.Windows;
using System.Xml;
using RussLibrary;
namespace ArtemisModLoader
{
    
    public class FileMap : XmlBase
    {
        public FileMap(XmlNode node) : base(node) { }

        public FileMap(string source, string target)
            : base((string)null)
        {
            Source = source;
            Target = target;
        }
        public FileMap(string source, string target, bool forSubMod)
            : base((string)null)
        {
            Source = source;
            Target = target;
            ForSubMod = forSubMod;
        }


        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(string),
            typeof(FileMap), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public string Source
        {
            get
            {
                return (string)this.UIThreadGetValue(SourceProperty);

            }
            private set
            {
                this.UIThreadSetValue(SourceProperty, value);

            }
        }
        public static readonly DependencyProperty TargetProperty =
           DependencyProperty.Register("Target", typeof(string),
           typeof(FileMap), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public string Target
        {
            get
            {
                return (string)this.UIThreadGetValue(TargetProperty);

            }
            private set
            {
                this.UIThreadSetValue(TargetProperty, value);

            }
        }


        public static readonly DependencyProperty ForSubModProperty =
           DependencyProperty.Register("ForSubMod", typeof(bool),
           typeof(FileMap), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public bool ForSubMod
        {
            get
            {
                return (bool)this.UIThreadGetValue(ForSubModProperty);

            }
            private set
            {
                this.UIThreadSetValue(ForSubModProperty, value);

            }
        }

       
    }
}
