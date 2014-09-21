using System.Collections.ObjectModel;
using System.Windows;
using System.Xml;
using RussLibrary;

namespace ArtemisModLoader
{
    
    public class SubMod : XmlBase
    {
        public SubMod(XmlNode node) : base(node) { }


        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof(ObservableCollection<FileMap>),
            typeof(SubMod), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public ObservableCollection<FileMap> Files
        {
            get
            {
                return (ObservableCollection<FileMap>)this.UIThreadGetValue(FilesProperty);

            }
            private set
            {
                this.UIThreadSetValue(FilesProperty, value);

            }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string),
            typeof(SubMod), new UIPropertyMetadata(OnItemChanged));
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


        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool),
            typeof(SubMod), new UIPropertyMetadata(OnItemChanged));
        [XmlBaseAttribute]
        public bool IsActive
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsActiveProperty);

            }
            set
            {
                this.UIThreadSetValue(IsActiveProperty, value);

            }
        }
       
    }
}
