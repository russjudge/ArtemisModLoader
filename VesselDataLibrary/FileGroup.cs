using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.IO;
using RussLibrary;

namespace VesselDataLibrary
{
    public class FileGroup : DependencyObject
    {
        public FileGroup(DirectoryInfo directory)
        {
            Directories = new ObservableCollection<FileGroup>();
            Files = new ObservableCollection<FileInfo>();
            if (directory != null)
            {
                foreach (DirectoryInfo d in directory.GetDirectories())
                {
                    Directories.Add(new FileGroup(d));
                }
                foreach (FileInfo f in directory.GetFiles())
                {
                    Files.Add(f);
                }
            }

        }
        public static readonly DependencyProperty FilesProperty =
           DependencyProperty.Register("Files", typeof(ObservableCollection<FileInfo>),
           typeof(FileGroup));


        public ObservableCollection<FileInfo> Files
        {
            get
            {
                return (ObservableCollection<FileInfo>)this.UIThreadGetValue(FilesProperty);

            }
            private set
            {
                this.UIThreadSetValue(FilesProperty, value);

            }
        }


        public static readonly DependencyProperty DirectoriesProperty =
           DependencyProperty.Register("Directories", typeof(ObservableCollection<FileGroup>),
           typeof(FileGroup));


        public ObservableCollection<FileGroup> Directories
        {
            get
            {
                return (ObservableCollection<FileGroup>)this.UIThreadGetValue(DirectoriesProperty);

            }
            private set
            {
                this.UIThreadSetValue(DirectoriesProperty, value);

            }
        }
        
    }
}
