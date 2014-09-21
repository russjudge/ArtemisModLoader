using System.Collections.ObjectModel;
using System.Windows;
using System.Xml;
using RussLibrary;
namespace ArtemisModLoader
{

    public class Mission : XmlBase
    {
        public Mission(XmlNode node) : base(node) { }

        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string),
            typeof(VersionIssues), new UIPropertyMetadata(OnItemChanged));

        public string Name
        {
            get
            {
                return (string)this.UIThreadGetValue(NameProperty);

            }
            private set
            {
                this.UIThreadSetValue(NameProperty, value);

            }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(VersionIssues), new UIPropertyMetadata(OnItemChanged));

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


        public static readonly DependencyProperty ForStockProperty =
            DependencyProperty.Register("ForStock", typeof(bool),
            typeof(VersionIssues), new UIPropertyMetadata(OnItemChanged));

        public bool ForStock
        {
            get
            {
                return (bool)this.UIThreadGetValue(ForStockProperty);

            }
            private set
            {
                this.UIThreadSetValue(ForStockProperty, value);

            }
        }
        public static readonly DependencyProperty CompatibleSubModsProperty =
            DependencyProperty.Register("CompatibleSubMods", typeof(ObservableCollection<SubModItem>),
            typeof(VersionIssues), new UIPropertyMetadata(OnItemChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public ObservableCollection<SubModItem> CompatibleSubMods
        {
            get
            {
                return (ObservableCollection<SubModItem>)this.UIThreadGetValue(CompatibleSubModsProperty);

            }
            private set
            {
                this.UIThreadSetValue(CompatibleSubModsProperty, value);

            }
        }

    }
}
