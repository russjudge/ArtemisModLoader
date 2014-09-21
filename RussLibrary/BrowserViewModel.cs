using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Collections.ObjectModel;
using System.IO;

namespace RussLibrary
{

    public class BrowserViewModel : ViewModelBase
    {
        private string _selectedFolder;
        public event EventHandler SelectedFolderChanged;
        static FolderViewModel GetFolderFromStack(string path, IList<FolderViewModel> folders)
        {
            FolderViewModel retVal = null;
            foreach (FolderViewModel fvm in folders)
            {
                if (fvm.FolderPath == path)
                {
                    retVal = fvm;
                    break;
                }
               

            }
            if (retVal == null)
            {
                foreach (FolderViewModel fvm in folders)
                {
                    retVal = GetFolderFromStack(path, fvm.Folders);
                }
            }
            return retVal;
        }
        public string SelectedFolder
        {
            get
            {
                return _selectedFolder;
            }
            set
            {
                if (_selectedFolder != value)
                {
                    FolderViewModel oldfvm = GetFolderFromStack(_selectedFolder, Folders);
                    FolderViewModel newfvm = GetFolderFromStack(value, Folders);
                    if (oldfvm != null && oldfvm.IsSelected)
                    {
                        oldfvm.IsSelected = false;
                    }
                    _selectedFolder = value;

                    if (newfvm != null && !newfvm.IsSelected)
                    {
                        newfvm.IsSelected = true;
                    }
                    OnPropertyChanged("SelectedFolder");
                    if (SelectedFolderChanged != null)
                    {
                        SelectedFolderChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public ObservableCollection<FolderViewModel> Folders
        {
            get;
            private set;
        }

        string _path = string.Empty;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _path = value.TrimEnd('\\');
                }
                else
                {
                    _path = value;
                }
                if (!string.IsNullOrEmpty(_path) && Directory.Exists(_path))
                {
                    LoadFolders();
                }
                else
                {
                    ClearFolders();
                }
                OnPropertyChanged("Path");
            }
        }
        void ClearFolders()
        {
            Folders.Clear();
        }
        void LoadFolders()
        {
            try
            {
                FolderViewModel rootFolder = new FolderViewModel();
                rootFolder.Root = this;
                rootFolder.FolderPath = Path;
                rootFolder.FolderName = ".";
                Folders.Add(rootFolder);
                
                ////foreach (DirectoryInfo dir in new DirectoryInfo(Path).GetDirectories())
                ////{
                ////    try
                ////    {
                ////        Folders.Add(new FolderViewModel { Root = this, FolderPath = dir.FullName, FolderName = dir.Name });
                ////    }
                ////    catch (UnauthorizedAccessException ae)
                ////    {
                ////        Console.WriteLine(ae.Message);
                ////    }
                ////    catch (IOException ie)
                ////    {
                ////        Console.WriteLine(ie.Message);
                ////    }
                ////}
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
        public BrowserViewModel()
        {
            Folders = new ObservableCollection<FolderViewModel>();
            //Environment.GetLogicalDrives().ToList().ForEach(it =>
            //    Folders.Add(new FolderViewModel { Root = this, FolderPath = it.TrimEnd('\\'), 
            //        FolderName = it.TrimEnd('\\'), FolderIcon = "Images\\HardDisk.ico" }));
        }
    }
}
