using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using RussLibrary.Helpers;
using log4net;

namespace RussLibrary.Controls
{

    
    /// <summary>
    /// Interaction logic for FileExplorerControl.xaml
    /// </summary>
    public partial class FileExplorerControl : UserControl, IDisposable
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(FileExplorerControl));
        public FileExplorerControl()
        {
            CurrentFiles = new ObservableCollection<FileSystemInfo>();
            fsw = new FileSystemWatcher();
            fsw.EnableRaisingEvents = false;
            fsw.IncludeSubdirectories = false;
            fsw.Changed += new FileSystemEventHandler(fsw_Changed);
            fsw.Created += new FileSystemEventHandler(fsw_Changed);
            fsw.Deleted += new FileSystemEventHandler(fsw_Changed);
            fsw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName
                | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            fsw.Renamed += new RenamedEventHandler(fsw_Renamed);
            
            InitializeComponent();
        }
        FileSystemWatcher fsw = null;


        void fsw_Renamed(object sender, RenamedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(Refresh));
            
        }
        
        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(Refresh));
         
        }
      
        void SetWatcher()
        {
            fsw.Path = SelectedPath;
            fsw.Filter = "*.*";
            fsw.EnableRaisingEvents=true;
        }
     
        public static readonly DependencyProperty RootPathProperty =
            DependencyProperty.Register("RootPath", typeof(string),
            typeof(FileExplorerControl));


        public string RootPath
        {
            get
            {
                return (string)this.UIThreadGetValue(RootPathProperty);

            }
            set
            {
                this.UIThreadSetValue(RootPathProperty, value);

            }
        }

        void Refresh()
        {
            fsw.EnableRaisingEvents = false;
            SetCurrentFiles();
           
        }

        static void OnSelectedPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FileExplorerControl me = sender as FileExplorerControl;
            if (me != null)
            {
                me.Refresh();
            }
        }

        void SetCurrentFiles()
        {
            CurrentFiles.Clear();
            if (!string.IsNullOrEmpty(SelectedPath) && Directory.Exists(SelectedPath))
            {
                FileError = false;
                DirectoryInfo directory = new DirectoryInfo(SelectedPath);
                try
                {
                    foreach (FileSystemInfo f in directory.GetFileSystemInfos())
                    {
                        CurrentFiles.Add(f);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Error Getting File System Infos.  Selected Path=" + SelectedPath, ex);
                    }
                    FileError = true;
                }
                if (!FileError)
                {
                    SetWatcher();
                }
            }
            else
            {
                fsw.EnableRaisingEvents = false;
            }
        }
        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register("SelectedPath", typeof(string),
            typeof(FileExplorerControl), new PropertyMetadata(OnSelectedPathChanged));


        /// <summary>
        /// Gets or sets the selected path. (from folderbrowser control.)
        /// </summary>
        /// <value>
        /// The selected path.
        /// </value>
        public string SelectedPath
        {
            get
            {
                return (string)this.UIThreadGetValue(SelectedPathProperty);

            }
            set
            {
                this.UIThreadSetValue(SelectedPathProperty, value);

            }
        }


        public static readonly DependencyProperty FileErrorProperty =
            DependencyProperty.Register("FileError", typeof(bool),
            typeof(FileExplorerControl));


        /// <summary>
        /// Gets or sets the selected file. (From Listview of files).
        /// </summary>
        /// <value>
        /// The selected file.
        /// </value>
        public bool FileError
        {
            get
            {
                return (bool)this.UIThreadGetValue(FileErrorProperty);

            }
            set
            {
                this.UIThreadSetValue(FileErrorProperty, value);

            }
        }


        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(FileSystemInfo),
            typeof(FileExplorerControl));


        /// <summary>
        /// Gets or sets the selected file. (From Listview of files).
        /// </summary>
        /// <value>
        /// The selected file.
        /// </value>
        public FileSystemInfo SelectedFile
        {
            get
            {
                return (FileSystemInfo)this.UIThreadGetValue(SelectedFileProperty);

            }
            set
            {
                this.UIThreadSetValue(SelectedFileProperty, value);

            }
        }




        public static readonly DependencyProperty CurrentFilesProperty =
            DependencyProperty.Register("CurrentFiles", typeof(ObservableCollection<FileSystemInfo>),
            typeof(FileExplorerControl));


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<FileSystemInfo> CurrentFiles
        {
            get
            {
                return (ObservableCollection<FileSystemInfo>)this.UIThreadGetValue(CurrentFilesProperty);

            }
            set
            {
                this.UIThreadSetValue(CurrentFilesProperty, value);

            }
        }

        static void StartProcess(string file)
        {
            if (!string.IsNullOrEmpty(file) && File.Exists(file))
            {
                ProcessStartInfo strt = new ProcessStartInfo(file);

                strt.UseShellExecute = true;

                Process.Start(strt);
            }
        }
        private void Mouse_DoubleClick(object sender, MouseButtonEventArgs e)
        {

            FileSystemInfo fle = e.ListBoxItemContent() as FileSystemInfo;
            if (fle != null)
            {
                if ((fle.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    SelectedPath = fle.FullName;
                }
                else
                {
                    StartProcess(fle.FullName);
                }
            }
        }

        private void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FileSystemInfo fle = e.ListBoxItemContent() as FileSystemInfo;
            //FrameworkElement elem = sender as FrameworkElement;
            if (fle != null)
            {
                ListViewItem item = GetListViewItemClicked(e.OriginalSource, lv);
                if (item != null)
                {
                    if (e.RightButton == MouseButtonState.Pressed)
                    {
                        ShellContextMenu scm = new ShellContextMenu();
                        scm.CreateHandle();
                        scm.ShowContextMenu(fle.FullName, PointToScreen(item.TranslatePoint(new Point(0, 0), lv)));
                    }
                    else if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        startPoint = e.GetPosition(null);

                        ActiveFilename = fle.FullName;
                        ActiveItem = item;
                    }
                }
            }
        }
        ListViewItem ActiveItem = null;
        string ActiveFilename = null;
        private static ListViewItem GetListViewItemClicked(object sender, ListView treeView)
        {
            DependencyObject obj = null;
            ListViewItem retVal = null;
            FrameworkElement elem = sender as FrameworkElement;
            if (elem != null)
            {
                Point p = elem.TranslatePoint(new Point(0, 0), treeView);
                obj = treeView.InputHitTest(p) as DependencyObject;
                retVal = obj as ListViewItem;
                while (obj != null && retVal == null)
                {
                    obj = VisualTreeHelper.GetParent(obj);
                    retVal = obj as ListViewItem;
                }
            }
            return retVal;
        }

        private void lv_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;

            }
        }

        private void lv_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] newFiles = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (newFiles != null)
                {
                    foreach (string f in newFiles)
                    {
                        
                        string target = System.IO.Path.Combine(SelectedPath, new FileInfo(f).Name);
                        if (f != target)
                        {


                            FileInfo targ = new FileInfo(target);
                            bool CopyOkay = true;
                            if (File.Exists(target))
                            {

                                switch (MessageBox.Show(targ.Name + " exists.  Replace?", "Copy files", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                                {
                                    case MessageBoxResult.Cancel:
                                        CopyOkay = false;
                                        return;
                                    case MessageBoxResult.Yes:
                                        CopyOkay = true;
                                        break;
                                    case MessageBoxResult.No:
                                        CopyOkay = false;
                                        break;
                                }

                            }
                            if (CopyOkay)
                            {
                                FileHelper.DeleteFile(target);
                                FileHelper.Copy(f, target);
                            }
                        }
                        
                    }
                }
            }

        }
        Point startPoint = new Point();
        private void lv_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                && !string.IsNullOrEmpty(ActiveFilename) && ActiveItem != null)
            {
                List<string> fItem = new List<string>();
                fItem.Add(ActiveFilename);
                DataObject dragData = new DataObject(DataFormats.FileDrop, fItem.ToArray());

                DragDrop.DoDragDrop(ActiveItem, dragData, DragDropEffects.Copy);
             
            } 
        }

        private void lv_MouseUp(object sender, MouseButtonEventArgs e)
        {
            startPoint = new Point();
            ActiveItem = null;
            ActiveFilename = null;
        }

        bool isDisposed = false;
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    fsw.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
