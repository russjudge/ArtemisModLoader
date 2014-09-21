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
using System.Windows.Shapes;
using ArtemisModLoader.Xml;
using RussLibrary;
using RussLibrary.Controls;

namespace ArtemisModLoader.Windows
{
    /// <summary>
    /// Interaction logic for ModManager.xaml
    /// </summary>
    public partial class ModManager : Window
    {
        public ModManager()
        {
            ModManagement.DownloadComplete += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadComplete);
            ModManagement.DownloadProgressChanged += new EventHandler<System.Net.DownloadProgressChangedEventArgs>(ModManagement_DownloadProgressChanged);
            InitializeComponent();

            ImageBrush brsh = ArtemisModLoader.Helpers.FileHelper.GetRandomSkybox();
            if (brsh != null)
            {
                this.Background = brsh;
            }
           
        }
        
        public static readonly DependencyProperty DownloadProgressProperty =
            DependencyProperty.Register("DownloadProgress", typeof(int),
            typeof(ModManager));

        public int DownloadProgress
        {
            get
            {
                return (int)this.UIThreadGetValue(DownloadProgressProperty);

            }
            private set
            {
                this.UIThreadSetValue(DownloadProgressProperty, value);

            }
        }
        public static readonly DependencyProperty BytesProcessedProperty =
          DependencyProperty.Register("BytesProcessed", typeof(long),
          typeof(ModManager));

        public long BytesProcessed
        {
            get
            {
                return (long)this.UIThreadGetValue(BytesProcessedProperty);

            }
            private set
            {
                this.UIThreadSetValue(BytesProcessedProperty, value);

            }
        }
        public static readonly DependencyProperty TotalBytesProperty =
            DependencyProperty.Register("TotalBytes", typeof(long),
            typeof(ModManager));

        public long TotalBytes
        {
            get
            {
                return (long)this.UIThreadGetValue(TotalBytesProperty);

            }
            private set
            {
                this.UIThreadSetValue(TotalBytesProperty, value);

            }
        }
        void ModManagement_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            pb.Visibility = System.Windows.Visibility.Visible;
            pbText.Visibility = System.Windows.Visibility.Visible;
            DownloadProgress = e.ProgressPercentage;
            BytesProcessed = e.BytesReceived;
            TotalBytes = e.TotalBytesToReceive;
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        void ModManagement_DownloadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            pb.Visibility = System.Windows.Visibility.Collapsed;
            pbText.Visibility = System.Windows.Visibility.Collapsed;
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.BrowseForFile();

            //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }


        private void ModUninstalled(object sender, RoutedEventArgs e)
        {
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //Only if a pre-defined mod.

            ModConfiguration mod = e.OriginalSource as ModConfiguration;
            if (ModManagement.GetPredefinedMods().ContainsKey(mod.ID))
            {
                
                predefinedMods.AppendConfig(mod);
            }
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void uc_Drop(object sender, DragEventArgs e)
        {
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }


            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null)
                {
                    ModManagement.ProcessFiles(files);
                }

            }

            //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void uc_DragOver(object sender, DragEventArgs e)
        {

            e.Effects = DragDropEffects.None;

            // If the DataObject contains string data, extract it.
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            }


        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetMaxSize();
            
        }

        private void uc_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Properties.Settings.Default.Save();
        }

        private void OnCopy(object sender, RoutedEventArgs e)
        {
              //<ctl:FolderBrowserControl
              //  SelectedPath="{Binding Path=SelectedPath, Mode=TwoWay, ElementName=uc}"
              //  RootPath="{Binding RootPath, Mode=OneWay, ElementName=uc}"
              //  Grid.Column="0" Background="#B9FFFFFF" />
            using (System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog())
            {
                diag.Description = "Please DO NOT violate your license agreement.  Remember, only six copies--one for each station--is valid.";
                diag.ShowNewFolderButton = true;

                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    RussLibrary.Helpers.FileHelper.CopyFiles(new System.IO.DirectoryInfo(Locations.ArtemisCopyPath), diag.SelectedPath);
                }
            }
           
                
           
        }

      
    }
}
