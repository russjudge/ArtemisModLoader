using System.Windows;
using System.Xml;
using RussLibrary;

namespace ArtemisModLoader
{
    
    public class VersionData : XmlBase
    {
        public VersionData(XmlNode node) : base(node) { }
        
        public static readonly DependencyProperty ValueProperty =
          DependencyProperty.Register("Value", typeof(string),
          typeof(VersionData), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public string Value
        {
            get
            {
                return (string)this.UIThreadGetValue(ValueProperty);

            }
            private set
            {
                this.UIThreadSetValue(ValueProperty, value);

            }
        }
    }
}
