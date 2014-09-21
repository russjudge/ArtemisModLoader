using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary;
using RussLibrary.Xml;
using System.Windows;
using System.Collections.ObjectModel;

namespace ArtemisModLoader.Xml
{
    
    public class FileCollection : ChangeDependencyObject
    {
        public FileCollection()
        {
            Files = new ObservableCollection<FileMap>();
        }
        public static readonly DependencyProperty FilesProperty =
            DependencyProperty.Register("Files", typeof(ObservableCollection<FileMap>),
            typeof(SubMod), new UIPropertyMetadata(OnItemChanged));
        [XmlConversion("FileMap")]
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

    }
}
