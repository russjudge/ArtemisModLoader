using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using log4net;
using Microsoft.Win32;
using RussLibrary;
using SharpCompress.Reader;
using ArtemisModLoader.Xml;
using ArtemisModLoader.Windows;
using VesselDataLibrary.Controls;
namespace ArtemisModLoader
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        static readonly ILog _log = LogManager.GetLogger(typeof(MainWindow));
        public MainWindow()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.DownloadComplete += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadComplete);
            ModManagement.DownloadProgressChanged += new EventHandler<System.Net.DownloadProgressChangedEventArgs>(ModManagement_DownloadProgressChanged);
            InitializeComponent();
            this.Title = Locations.AssemblyTitle;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static readonly DependencyProperty DownloadProgressProperty =
          DependencyProperty.Register("DownloadProgress", typeof(int),
          typeof(MainWindow));

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
          typeof(MainWindow));

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
      typeof(MainWindow));

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
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            pb.Visibility = System.Windows.Visibility.Visible;
            pbText.Visibility = System.Windows.Visibility.Visible;
            DownloadProgress = e.ProgressPercentage;
            BytesProcessed = e.BytesReceived;
            TotalBytes = e.TotalBytesToReceive;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        void ModManagement_DownloadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            pb.Visibility = System.Windows.Visibility.Collapsed;
            pbText.Visibility = System.Windows.Visibility.Collapsed;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        //void ProcessFile(string FileToProcess)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    if (!string.IsNullOrEmpty(FileToProcess))
        //    {
        //        //Now identify file type.  If "*.aml", prompt to process from web download, or browse for package.
        //        if (FileToProcess.EndsWith(".aml", StringComparison.OrdinalIgnoreCase))
        //        {
        //            bool success = ProcessAML(FileToProcess);
        //        }
        //        else
        //        {
        //            //First scan zip file for a .aml file--if found, process .aml file, no download.
        //            ModConfiguration config = GetModConfigFile(FileToProcess);
        //            if (config == null)
        //            {
        //                ProcessNoAML(FileToProcess);
        //            }
        //            else
        //            {
        //                ModManagement.BeginInstall(config, FileToProcess);
                      
        //            }


        //        }
        //    }
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //}
        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            BrowseForFile();
            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        //ModConfiguration GetModConfigFile(string FileToProcess)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    ModConfiguration config = null;
        //    string target = null;
        //    using (Stream stream = File.OpenRead(FileToProcess))
        //    {
        //        IReader reader = ReaderFactory.Open(stream);
        //        while (reader.MoveToNextEntry())
        //        {
        //            if (!reader.Entry.IsDirectory)
        //            {
        //                if (reader.Entry.FilePath.EndsWith(".aml", StringComparison.OrdinalIgnoreCase))
        //                {
        //                    target = System.IO.Path.Combine(System.IO.Path.GetTempPath(), reader.Entry.FilePath);

        //                    reader.WriteEntryToDirectory(System.IO.Path.GetTempPath(),
        //                        SharpCompress.Common.ExtractOptions.ExtractFullPath | SharpCompress.Common.ExtractOptions.Overwrite);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    if (!string.IsNullOrEmpty(target))
        //    {
        //        config = new ModConfiguration(target);
        //    }
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    return config;
        //}
        //void ProcessNoAML(string FileToProcess)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    ModDefinitionSetup win = new ModDefinitionSetup();
        //    win.Configuration.PackagePath = FileToProcess;
        //    win.Configuration.ID = new FileInfo(FileToProcess).Name.Replace('.', '~');
        //    if (win.ShowDialog() == true)
        //    {
        //        ModManagement.BeginInstall(win.Configuration, FileToProcess);
        //    }
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //}
        //bool ProcessAML(string fileToProcess)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    bool retVal = false;
        //    ModConfiguration config = new ModConfiguration(fileToProcess);
        //    if (!string.IsNullOrEmpty(config.Download.Source) || ! string.IsNullOrEmpty(config.Download.Webpage))
        //    {
        //        MessageBoxResult result = Locations.MessageBoxShow("Do you wish to download the package from the web?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        //        if (result == MessageBoxResult.Cancel)
        //        {
        //            retVal = false;
        //        }
        //        else if (result == MessageBoxResult.Yes)
        //        {
        //            if (string.IsNullOrEmpty(config.Download.Source))
        //            {
        //                System.Diagnostics.Process.Start(config.Download.Webpage);
        //                retVal = ModManagement.BrowseForPackage(config);
        //            }
        //            else
        //            {
        //                StartDownload(config);
        //                retVal = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        retVal = ModManagement.BrowseForPackage(config);
        //    }
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    return retVal;
        //}
        //void StartDownload(ModConfiguration config)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    ModManagement.Downloaded += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_Downloaded);
        //    ModManagement.DownloadFailed += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadFailed);

        //    ModManagement.StartDownload(config);
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //}
        //////void CleanupDownload()
        //////{
        //////    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //////    ModManagement.Downloaded -= new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_Downloaded);
        //////    ModManagement.DownloadFailed -= new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadFailed);
        //////    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //////}
        //////void ModManagement_DownloadFailed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //////{
        //////    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //////    CleanupDownload();
        //////    MessageBox.Show("Download Failed:\r\n\r\n" + e.Error.ToString() + "\r\n\r\nPlease manually download the file and select it.", "Aretmis Mod Loader", MessageBoxButton.OK, MessageBoxImage.Error);
        //////    ModConfiguration mod = e.UserState as ModConfiguration;
        //////    if (string.IsNullOrEmpty(mod.Download.Source))
        //////    {
        //////        System.Diagnostics.Process.Start(mod.Download.Source);
        //////    }
        //////    else
        //////    {
        //////        System.Diagnostics.Process.Start(mod.Download.Webpage);
        //////    }
        //////    ModManagement.BrowseForPackage(mod);
        //////    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //////}

        //////void ModManagement_Downloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //////{
        //////    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //////    CleanupDownload();
        //////    if (e != null)
        //////    {
        //////        ModConfiguration mod = e.UserState as ModConfiguration;

        //////        ModManagement.BeginInstall(mod, mod.PackagePath);
                
        //////    }
        //////    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //////}

        void BrowseForFile()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = string.Empty;
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = AMLResources.Properties.Resources.SupportedCompressedFiles  + DataStrings.SupportedCompressedFilesFilter
                + "|" + AMLResources.Properties.Resources.AML + DataStrings.AMLFilter
                + "|" +AMLResources.Properties.Resources.AllFiles + DataStrings.AllFilesFilter;
            diag.Title = Locations.AssemblyTitle;
            diag.Multiselect = true; 

            if (diag.ShowDialog() == true)
            {
                if (diag.FileNames != null)
                {
                    ModManagement.ProcessFiles(diag.FileNames);
                }
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        
        }
        
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (this.SelectedIPAddress != Locations.GetCurrentIPAddress())
            {
                if (!Locations.SetIPAddress(this.SelectedIPAddress))
                {
                    Locations.MessageBoxShow("Unable to set the Server IP address.\r\n\r\nReview log file for details as to why.",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
           

           
            this.ShowInTaskbar = false;
            SessionMonitor win = new SessionMonitor();
            win.StartSession();
            this.DoAnimation();
            win.ShowDialog();
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.WindowState.Normal;

            string newIP = Locations.GetCurrentIPAddress();
            if (!IPAddresses.Contains(newIP))
            {
                Locations.AddToIPList(newIP);
                IPAddresses.Add(newIP);
                SelectedIPAddress = newIP;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        double thisWidth = 0;
        double thisHeight = 0;
        double thisTop = 0;
        double thisLeft = 0;
        void DoAnimation()
        {
            thisWidth = this.Width;
            thisHeight = this.Height;
            thisTop = this.Top;
            thisLeft = this.Left;

            for (double i = 500; i > 1; i-=3)
            {
                this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;
                this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height; 
                this.Width = i;
                this.Height= i;
                //System.Threading.Thread.Sleep(1);
            }
            this.Left = thisLeft;
            this.Top = thisTop;
            this.Width = thisWidth;
            this.Height = thisHeight;
            this.WindowState = System.Windows.WindowState.Minimized;


        }
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ArtemisModLoader.Windows.Settings win = new ArtemisModLoader.Windows.Settings();
            win.Owner = this;
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            win.ShowDialog();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void ModUninstalled(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //Only if a pre-defined mod.

            ModConfiguration mod = e.OriginalSource as ModConfiguration;
            if (ModManagement.GetPredefinedMods().ContainsKey(mod.ID))
            {
                predefinedMods.AppendConfig(mod);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            About win = new About();
            win.Owner = this;
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            win.ShowDialog();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void uc_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void uc_Drop(object sender, DragEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }


            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null)
                {
                    ModManagement.ProcessFiles(files);
                }
               
            }

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
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


        public static readonly DependencyProperty IPAddressesProperty =
            DependencyProperty.Register("IPAddresses", typeof(ObservableCollection<string>),
            typeof(MainWindow), new PropertyMetadata(new ObservableCollection<string>(Locations.GetIPList())));
        
        public ObservableCollection<string> IPAddresses
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(IPAddressesProperty);

            }
            private set
            {
                this.UIThreadSetValue(IPAddressesProperty, value);

            }
        }

        public static readonly DependencyProperty SelectedIPAddressProperty =
            DependencyProperty.Register("SelectedIPAddress", typeof(string),
            typeof(MainWindow), new PropertyMetadata(Locations.GetCurrentIPAddress()));

        public string SelectedIPAddress
        {
            get
            {
                return (string)this.UIThreadGetValue(SelectedIPAddressProperty);

            }
            private set
            {
                this.UIThreadSetValue(SelectedIPAddressProperty, value);

            }
        }

        private void GetAndroid_Click(object sender, RoutedEventArgs e)
        {
            
            System.Diagnostics.Process.Start(DataStrings.AndroidClientWebsite);
        }

        private void uc_Closed(object sender, EventArgs e)
        {
            Properties.Settings.Default.MainWidth = this.Width;
            Properties.Settings.Default.MainHeight = this.Height;
            Properties.Settings.Default.Save();
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetMaxSize();
            if (Properties.Settings.Default.MainWidth > 0)
            {
                this.SizeToContent = System.Windows.SizeToContent.Height;
                this.Width = Properties.Settings.Default.MainWidth;
            }
            if (Properties.Settings.Default.MainHeight > 0)
            {
                this.SizeToContent = System.Windows.SizeToContent.Manual;
                this.Height = Properties.Settings.Default.MainHeight;
            }
            this.Dispatcher.BeginInvoke(new Action(UpdateCheckProcess));
        }
        void UpdateCheckProcess()
        {
            if (ModManagement.CheckForUpdate())
            {
                if (Locations.MessageBoxShow(string.Format("An update to Artemis has been detected.  Do you wish to apply that update to the copy that {0} runs?", Locations.AssemblyTitle),
                   MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ModManagement.ProcessUpdate();
                }
            }
        }
        private void RemoveIPAddress_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                string IP = btn.CommandParameter as string;
                if (!string.IsNullOrEmpty(IP))
                {
                    int idx = 0;
                    while (IP == SelectedIPAddress && idx < IPAddresses.Count)
                    {
                        SelectedIPAddress = IPAddresses[idx++];
                    }
                    IPAddresses.Remove(IP);
                    using (StreamWriter sw = new StreamWriter(Locations.IPAddressListFile, false))
                    {
                        foreach (string ipAddr in IPAddresses)
                        {
                            sw.WriteLine(ipAddr);
                        }
                    }
                }
            }
        }

        private void ModManager_Click(object sender, RoutedEventArgs e)
        {
            ModDevelopmentManager win = new ModDevelopmentManager();
            win.Show();
        }

    }
}
