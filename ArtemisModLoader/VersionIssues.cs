using System.Windows;
using System.Xml;
using RussLibrary;

namespace ArtemisModLoader
{
    
    public class VersionIssues : XmlBase
    {
        public VersionIssues(XmlNode node) : base(node) { }

        public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(string),
         typeof(VersionIssues), new UIPropertyMetadata(OnItemChanged));
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

        public static readonly DependencyProperty MatchProperty =
         DependencyProperty.Register("Match", typeof(string),
         typeof(VersionIssues), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public string Match
        {
            get
            {
                return (string)this.UIThreadGetValue(MatchProperty);

            }
            private set
            {
                this.UIThreadSetValue(MatchProperty, value);

            }
        }

        public static readonly DependencyProperty CanInstallProperty =
         DependencyProperty.Register("CanInstall", typeof(bool),
         typeof(VersionIssues), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public bool CanInstall
        {
            get
            {
                return (bool)this.UIThreadGetValue(CanInstallProperty);

            }
            private set
            {
                this.UIThreadSetValue(CanInstallProperty, value);

            }
        }
        
      

       
    }
}
