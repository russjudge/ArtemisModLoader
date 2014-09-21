using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using log4net;
using Microsoft.Win32;
using RussLibrary;
namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for PredefinedMods.xaml
    /// </summary>
    public partial class PredefinedMods : UserControl
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(PredefinedMods));
        public static Dictionary<string, ModConfiguration> PredefinedModDictionary = new Dictionary<string, ModConfiguration>();
        public PredefinedMods()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Definitions = new ObservableCollection<ModConfiguration>();
            List<string> predefined = new List<string>();
            ModConfiguration config = null;
            PredefinedModDictionary.Clear();
            try
            {
                foreach (System.IO.FileInfo fle in new System.IO.DirectoryInfo(Locations.PredefinedModsPath).GetFiles())
                {
                    config = new ModConfiguration(fle.FullName);
                    PredefinedModDictionary.Add(config.ID, config);
                    predefined.Add(config.ID);
                    AppendConfig(config);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Error reading list of Pre-defined MODS.", ex);
                }
            }
            PreDefinedList = new ReadOnlyCollection<string>(predefined);
            
            InitializeComponent();
            if (Definitions.Count < 1)
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        public static ReadOnlyCollection<string> PreDefinedList { get; private set; }
        public void AppendConfig(ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (!ModManagement.IsInstalled(config.ID))
            {
                Definitions.Add(config);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static readonly DependencyProperty DefinitionsProperty =
           DependencyProperty.Register("Definitions", typeof(ObservableCollection<ModConfiguration>),
           typeof(PredefinedMods));

        public ObservableCollection<ModConfiguration> Definitions
        {
            get
            {
                return (ObservableCollection<ModConfiguration>)this.UIThreadGetValue(DefinitionsProperty);

            }
            private set
            {
                this.UIThreadSetValue(DefinitionsProperty, value);

            }
        }
        
        void StartDownload(ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.Downloaded += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_Downloaded);
            ModManagement.DownloadFailed += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadFailed);

            ModManagement.StartDownload(config);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void CleanupDownload()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.Downloaded -= new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_Downloaded);
            ModManagement.DownloadFailed -= new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadFailed);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void ModManagement_DownloadFailed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            CleanupDownload();
            Locations.MessageBoxShow("Download Failed:\r\n\r\n"
                + e.Error.ToString()
                + "\r\n\r\nPlease manually download the file and select it.",
                MessageBoxButton.OK, MessageBoxImage.Error);
            ModConfiguration mod = e.UserState as ModConfiguration;
            if (string.IsNullOrEmpty(mod.Download.Source))
            {
                System.Diagnostics.Process.Start(mod.Download.Source);
            }
            else
            {
                System.Diagnostics.Process.Start(mod.Download.Webpage);
            }
            BrowseForPackage(mod);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        void ModManagement_Downloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            CleanupDownload();
            if (e != null)
            {
                ModConfiguration mod = e.UserState as ModConfiguration;
                ModManagement.InstallComplete += new EventHandler<ProcessEventArgs>(ModManagement_InstallComplete);
                ModManagement.BeginInstall(mod, mod.PackagePath);
             
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        void ModManagement_InstallComplete(object sender, ProcessEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.InstallComplete -= new EventHandler<ProcessEventArgs>(ModManagement_InstallComplete);
            if (e.Result)
            {
                Definitions.Remove(e.Configuration);
            }
            else
            {
                BrowseForPackage(e.Configuration);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void ModManagement_InstallComplete2(object sender, ProcessEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.InstallComplete -= new EventHandler<ProcessEventArgs>(ModManagement_InstallComplete2);
            if (e.Result)
            {
                Definitions.Remove(e.Configuration);
            }
            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        private void InstallFromWeb_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                ModConfiguration mod = btn.CommandParameter as ModConfiguration;
                if (mod != null)
                {
                    
                    bool NeedPackageSpecified = true;
                    
                    if (!string.IsNullOrEmpty(mod.Download.Source))
                    {
                        StartDownload(mod);
                    }
                    else if (NeedPackageSpecified)
                    {
                        BrowseForPackage(mod);

                    }

                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void BrowseForPackage(ModConfiguration mod)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = AMLResources.Properties.Resources.SupportedCompressedFiles + DataStrings.SupportedCompressedFilesFilter
                + "|" + AMLResources.Properties.Resources.AllFiles + DataStrings.AllFilesFilter;
            diag.Title = Locations.AssemblyTitle;
            if (diag.ShowDialog() == true)
            {
                ModManagement.InstallComplete+=new EventHandler<ProcessEventArgs>(ModManagement_InstallComplete2);
                ModManagement.BeginInstall(mod, diag.FileName);
              
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        private void BrowseToWeb_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                ModConfiguration mod = btn.CommandParameter as ModConfiguration;
                if (mod != null)
                {
                    System.Diagnostics.Process.Start(mod.Download.Webpage);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void InstallFromPC_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                ModConfiguration mod = btn.CommandParameter as ModConfiguration;
                if (mod != null)
                {
                    BrowseForPackage(mod);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
    }
}
