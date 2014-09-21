using System.Windows;
using System.Xml;
using RussLibrary;
namespace ArtemisModLoader
{
    
    public class SubModItem : XmlBase
    {
        public SubModItem(XmlNode node) : base(node) { }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(SubModItem), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public string Title
        {
            get
            {
                return (string)this.UIThreadGetValue(TitleProperty);

            }
            private set
            {
                this.UIThreadSetValue(TitleProperty, value);

            }
        }

        public static readonly DependencyProperty IsCompatibleProperty =
            DependencyProperty.Register("IsCompatible", typeof(bool),
            typeof(SubModItem), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public bool IsCompatible
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsCompatibleProperty);

            }
            private set
            {
                this.UIThreadSetValue(IsCompatibleProperty, value);

            }
        }
    }
}
