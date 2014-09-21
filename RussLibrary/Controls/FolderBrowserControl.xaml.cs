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
using RussLibrary;
using System.IO;

namespace RussLibrary.Controls
{
    /// <summary>
    /// Interaction logic for FolderBrowserControl.xaml
    /// </summary>
    public partial class FolderBrowserControl : UserControl, IDisposable
    {
        public FolderBrowserControl()
        {
            _viewModel = new BrowserViewModel();
            _viewModel.SelectedFolderChanged += new EventHandler(_viewModel_SelectedFolderChanged);
            InitializeComponent();
            fsw = new FileSystemWatcher();
            fsw.EnableRaisingEvents = false;
            fsw.IncludeSubdirectories = true;
            fsw.Changed += new FileSystemEventHandler(fsw_Changed);
            fsw.Created += new FileSystemEventHandler(fsw_Changed);
            fsw.Deleted += new FileSystemEventHandler(fsw_Changed);
            fsw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName
                | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            fsw.Renamed += new RenamedEventHandler(fsw_Renamed);
            

        }
        FileSystemWatcher fsw = null;


        void fsw_Renamed(object sender, RenamedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(Refresh));
        }
        ~FolderBrowserControl()
        {

            Dispose(false);

        }
      

      
        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(Refresh));
           
        }


        void Refresh()
        {
            string wrk = RootPath;
            RootPath = string.Empty;
            RootPath = wrk;

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void SetWatcher()
        {
            try
            {
                fsw.Path = RootPath;
                fsw.Filter = "*.*";
                fsw.EnableRaisingEvents = true;
            }
            catch { }
        }




        void _viewModel_SelectedFolderChanged(object sender, EventArgs e)
        {
            SelectedPath = _viewModel.SelectedFolder;
        }
        private BrowserViewModel _viewModel;

        public BrowserViewModel ViewModel
        {
            get
            {
                return _viewModel;
            }
        }


        static void OnRootPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderBrowserControl me = sender as FolderBrowserControl;
            if (me != null)
            {
                me.ViewModel.Path = me.RootPath;
                me.ViewModel.SelectedFolder = me.RootPath;
                //if (me.ViewModel.Folders.Count > 0)
                //{
                //    me.ViewModel.Folders[0].IsSelected = true;
                //}
                if (!string.IsNullOrEmpty(me.RootPath))
                {
                    me.SetWatcher();
                }
            }
        }
        static void OnSelectedPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            FolderBrowserControl me = sender as FolderBrowserControl;
            if (me != null)
            {
                me.ViewModel.SelectedFolder = me.SelectedPath;
            }
        }

        public static readonly DependencyProperty SelectedPathProperty =
            DependencyProperty.Register("SelectedPath", typeof(string),
            typeof(FolderBrowserControl), new PropertyMetadata(OnSelectedPathChanged));


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

        public static readonly DependencyProperty RootPathProperty =
            DependencyProperty.Register("RootPath", typeof(string),
            typeof(FolderBrowserControl), new PropertyMetadata(OnRootPathChanged));


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

        private void FolderTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        static FolderViewModel GetFolderItem(TreeViewItem item)
        {
            FolderViewModel fvm = null;
            if (item != null)
            {
                fvm = item.Tag as FolderViewModel;
                if (fvm != null)
                {
                    if (!fvm.IsSelected)
                    {
                        fvm.IsSelected = true;
                    }
                }
            }
            return fvm;
        }

        private void tv_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            TreeViewItem item = GetTreeViewItemClicked(e.OriginalSource, tv);
            FolderViewModel fvm = GetFolderItem(item);
            if (fvm != null)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    ShellContextMenu scm = new ShellContextMenu();
                    scm.CreateHandle();
                    scm.ShowContextMenu(fvm.FolderPath, PointToScreen(item.TranslatePoint(new Point(0, 0), tv)));
                }
            }
        }

        
        private static TreeViewItem GetTreeViewItemClicked(object sender, TreeView treeView)
        {
            DependencyObject obj = null;
            TreeViewItem retVal = null;
            FrameworkElement elem = sender as FrameworkElement;
            if (elem != null)
            {
                Point p = elem.TranslatePoint(new Point(0, 0), treeView);
                obj = treeView.InputHitTest(p) as DependencyObject;
                retVal = obj as TreeViewItem;
                while (obj != null && retVal == null)
                {
                    obj = VisualTreeHelper.GetParent(obj);
                    retVal = obj as TreeViewItem;
                }
            }
            return retVal;
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
