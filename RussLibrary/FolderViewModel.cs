using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;
using System.Collections.ObjectModel;

namespace RussLibrary
{

    public class FolderViewModel : ViewModelBase
    {
        private bool _isSelected;
        private bool _isExpanded;
       

        public BrowserViewModel Root
        {
            get;
            set;
        }

       

        public string FolderName
        {
            get;
            set;
        }

        public string FolderPath
        {
            get;
            set;
        }

        public ObservableCollection<FolderViewModel> Folders
        {
            get;
            private set;
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;

                    OnPropertyChanged("IsSelected");
                    if (_isSelected)
                    {
                        IsExpanded = true; //Default windows behaviour of expanding the selected folder
                        if (Root.SelectedFolder != FolderPath)
                        {
                            Root.SelectedFolder = FolderPath;
                        }
                    }
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;

                    OnPropertyChanged("IsExpanded");

                   
                    LoadFolders();
                }
               
            }
        }

        private void LoadFolders()
        {
            try
            {
                if (Folders.Count > 0)
                    return;

                string[] dirs = null;

                string fullPath = Path.Combine(FolderPath, FolderName);

                if (FolderName.Contains(':'))//This is a drive
                    fullPath = string.Concat(FolderName, "\\");
                else
                    fullPath = FolderPath;

                dirs = Directory.GetDirectories(fullPath);

                Folders.Clear();

                foreach (string dir in dirs)
                    Folders.Add(new FolderViewModel {
                        Root = this.Root,
                        FolderName = Path.GetFileName(dir),
                        FolderPath = Path.GetFullPath(dir) });

                

               
            }
            catch (UnauthorizedAccessException ae)
            {
                Console.WriteLine(ae.Message);
            }
            catch (IOException ie)
            {
                Console.WriteLine(ie.Message);
            }
        }

        public FolderViewModel()
        {
            Folders = new ObservableCollection<FolderViewModel>();
        }
    }
}
