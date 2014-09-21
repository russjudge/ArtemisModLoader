using System;
using System.Diagnostics;
using System.Windows;
using ArtemisModLoader;
using ArtemisModLoader.Windows;
using ArtemisModLoader.Xml;
using RussLibrary;
using RussLibrary.Helpers;
using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using SharpCompress.Archive.Zip;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for ModDevelopmentManager.xaml
    /// </summary>
    public partial class ModDevelopmentManager : Window, IDisposable
    {
        public ModDevelopmentManager()
        {
            InitializeComponent();
            fsw = new FileSystemWatcher();
            fsw.Changed += new FileSystemEventHandler(fsw_Changed);
            fsw.Created += new FileSystemEventHandler(fsw_Created);
            fsw.Deleted += new FileSystemEventHandler(fsw_Deleted);
            fsw.IncludeSubdirectories = true;
            fsw.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;


            ImageBrush brsh = ArtemisModLoader.Helpers.FileHelper.GetRandomSkybox();
            if (brsh != null)
            {
                this.Background = brsh;
            }
           
            
        }
        ~ModDevelopmentManager()
        {
            Dispose(false);
        }
        void fsw_Deleted(object sender, FileSystemEventArgs e)
        {
            LoadFiles();
        }

        void fsw_Created(object sender, FileSystemEventArgs e)
        {
            LoadFiles();
        }

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            LoadFiles();
            
        }
        static void OnConfigurationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ModDevelopmentManager me = sender as ModDevelopmentManager;
            if (me != null)
            {
                me.LoadFiles();
            }
        }
        public static readonly DependencyProperty ConfigurationProperty =
           DependencyProperty.Register("Configuration", typeof(ModConfiguration),
           typeof(ModDevelopmentManager), new PropertyMetadata(OnConfigurationChanged));


        public ModConfiguration Configuration
        {
            get
            {
                return (ModConfiguration)this.UIThreadGetValue(ConfigurationProperty);

            }
            set
            {
                this.UIThreadSetValue(ConfigurationProperty, value);

            }
        }
        void ResetData()
        {
            Configuration = null;
            this.aINI.Reset();
            this.vdc.Reset();
        }
        private void New_Click(object sender, RoutedEventArgs e)
        {

            ResetData();

            ModConfiguration cfg = new ModConfiguration();
            cfg.ID = "{" + Guid.NewGuid().ToString() + "}";
            string wrkPath = System.IO.Path.Combine(Locations.InstalledModsPath, cfg.ID);

            FileHelper.CreatePath(wrkPath);
            FileHelper.CreatePath(Path.Combine(wrkPath, "dat"));
            cfg.InstalledPath = wrkPath;
            InstalledModConfigurations.Current.Configurations.Configurations.Add(cfg);
            InstalledModConfigurations.Current.Save();

            cfg.AcceptChanges();
            bool UseStock = true;
            if (ActiveModConfigurations.Current.Configurations.Configurations.Count > 1)
            {
                if (Locations.MessageBoxShow(
                    AMLResources.Properties.Resources.SetDependsOnActivated,
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    UseStock = false;
                    foreach (ModConfiguration config in ActiveModConfigurations.Current.Configurations.Configurations)
                    {
                        if (!config.ID.Contains(DataStrings.StockID))
                        {
                            cfg.DependsOn.Add(new StringItem(config.ID));
                            string fle = ArtemisModLoader.Helpers.FileHelper.LocateExpectedFileInMod("artemis.ini", config);
                            if (!string.IsNullOrEmpty(fle))
                            {
                                FileHelper.Copy(fle, Path.Combine(cfg.InstalledPath, "artemis.ini"));
                            }

                            fle = ArtemisModLoader.Helpers.FileHelper.LocateExpectedFileInMod("dat\\vesselData.xml", config);
                            if (!string.IsNullOrEmpty(fle))
                            {
                                FileHelper.Copy(fle, Path.Combine(cfg.InstalledPath, "dat", "vesselData.xml"));
                            }

                        }
                    }
                    cfg.AcceptChanges();
                }
                else
                {
                    UseStock = true;
                }
            }
            else
            {
                UseStock = true;
            }
            if (UseStock)
            {
                foreach (ModConfiguration config in ActiveModConfigurations.Current.Configurations.Configurations)
                {
                    if (config.ID == DataStrings.StockID)
                    {
                        string fle = ArtemisModLoader.Helpers.FileHelper.LocateExpectedFileInMod("artemis.ini", config);
                        if (!string.IsNullOrEmpty(fle))
                        {
                            FileHelper.Copy(fle, Path.Combine(cfg.InstalledPath, "artemis.ini"));
                        }

                        fle = ArtemisModLoader.Helpers.FileHelper.LocateExpectedFileInMod("dat\\vesselData.xml", config);
                        if (!string.IsNullOrEmpty(fle))
                        {
                            FileHelper.Copy(fle, Path.Combine(cfg.InstalledPath, "dat", "vesselData.xml"));
                        }
                    }
                }
            }
            Configuration = cfg;
        }
        private void DoOpen()
        {


            //Open installed
            if (InstalledModConfigurations.Current.Configurations.Configurations.Count > 1)
            {
                ResetData();
                ModSelector win = new ModSelector();
                if (win.ShowDialog() == true)
                {
                    Configuration = win.SelectedConfiguration;
                }
            }
            else
            {
                Locations.MessageBoxShow(AMLResources.Properties.Resources.NoModsInstalled,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void Open_click(object sender, RoutedEventArgs e)
        {
            DoOpen();
        }

        private void Save_click(object sender, RoutedEventArgs e)
        {
            if (Configuration != null)
            {
                //will always be an installed mod.
                InstalledModConfigurations.Current.Save();

                Locations.MessageBoxShow(Configuration.Title + " saved.", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }



        private void Deploy_Click(object sender, RoutedEventArgs e)
        {

            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = AMLResources.Properties.Resources.Title;
            diag.Filter = AMLResources.Properties.Resources.SupportedCompressedFiles + DataStrings.SupportedCompressedFilesFilter;
            diag.FileName = Configuration.Title.Replace(":", string.Empty).Replace(";", string.Empty).Replace("\\", string.Empty).Replace(" ", string.Empty).Replace("-", string.Empty) + ".zip";
            diag.DefaultExt = ".zip";
            if (diag.ShowDialog() == true)
            {
                FileHelper.DeleteFile(diag.FileName);


                SharpCompress.Common.CompressionInfo ci = new SharpCompress.Common.CompressionInfo();
                ci.DeflateCompressionLevel = SharpCompress.Compressor.Deflate.CompressionLevel.BestCompression;
                using (FileStream sw = new FileStream(diag.FileName, FileMode.Create, FileAccess.Write))
                {
                    //using (

                    SharpCompress.Writer.Zip.ZipWriter zw = new SharpCompress.Writer.Zip.ZipWriter(sw,
                    ci,
                    string.Format("Artemis Spaceship Bridge Simulator Mod: {0}\r\n\r\nMod ID: {1}\r\n\r\nPackaged by Artemis Mod Loader\r\n{2}\r\n\r\nDownload the latest Artemis Mod Loader from {3}",
                    Configuration.Title, Configuration.ID,
                    DateTime.Now.ToString(CultureInfo.CurrentCulture),
                    DataStrings.AMLUpdateURL));
                        
                        //)
                    //{
                        DirectoryInfo dir = new DirectoryInfo(Configuration.InstalledPath);
                        foreach (FileInfo f in dir.GetFiles("*.*", SearchOption.AllDirectories))
                        {
                            using (FileStream fs = f.OpenRead())
                            {
                                string relativePath = string.Empty;
                                if (f.FullName.Length > Configuration.InstalledPath.Length)
                                {
                                    relativePath = f.FullName.Substring(Configuration.InstalledPath.Length + 1);
                                    //relativePath = relativePath.Substring(0, relativePath.Length - f.Name.Length);
                                }
                                zw.Write(relativePath, fs, f.LastWriteTime);
                            }
                        }
                        string tmpPath = Path.Combine(Path.GetTempPath(), Configuration.Title.Replace(":", string.Empty).Replace(";", string.Empty).Replace("\\", string.Empty) + ".aml");
                        ModConfiguration config = new ModConfiguration();
                        config.CopyProperties(Configuration);
                        config.InstalledPath = null;
                        config.Sequence = 0;
                        config.ActiveFiles = null;
                        if (config.BaseFiles.Files.Count == 0)
                        {
                            config.BaseFiles = null;
                        }
                        if (config.SubMods.SubMods.Count == 0)
                        {
                            config.SubMods = null;
                        }
                        config.Save(tmpPath);
                        using (FileStream fs = new FileStream(tmpPath, FileMode.Open, FileAccess.Read))
                        {
                            FileInfo f = new FileInfo(tmpPath);
                            zw.Write(f.Name, fs, DateTime.Now);
                        }
                    //}
                }
                Locations.MessageBoxShow("Package saved.", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void Explorer_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Configuration.InstalledPath);
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetMaxSize();
            this.ReCenter();
            DoOpen();
        }

        private void uc_Activated(object sender, EventArgs e)
        {

        }
        bool DependentModsActive()
        {
            bool retVal = true;
            bool fail = false;
            foreach (ModConfiguration config in ActiveModConfigurations.Current.Configurations.Configurations)
            {
                if (config.ID != DataStrings.StockID && config.ID != Configuration.ID)
                {
                    foreach (StringItem item in Configuration.DependsOn)
                    {
                        if (item.Text != config.ID)
                        {
                            fail = true;
                            break;
                        }
                    }
                }
                if (fail)
                {
                    break;
                }
            }
            if (!fail)
            {
                foreach (StringItem item in Configuration.DependsOn)
                {
                    bool found = false;
                    foreach (ModConfiguration config in ActiveModConfigurations.Current.Configurations.Configurations)
                    {
                        if (config.ID == item.Text)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        fail = true;
                        break;
                    }

                }
            }

            return retVal;
        }
        private void Test_Click(object sender, RoutedEventArgs e)
        {

            if (ModManagement.IsActive(Configuration.ID))
            {
                if (Locations.MessageBoxShow("This Mod is currently active.  In order to apply your changes, the Mod must first be deactivated, the re-activated.\r\n\r\nDo you wish to deactivate then re-activate this Mod for testing?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    do
                    {
                        ModManagement.DeactivateLastMod();
                    } while (ModManagement.IsActive(Configuration.ID));
                    ModManagement.NotifyMissionInstall();
                }
                else
                {
                    Locations.MessageBoxShow("Test canceled.", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
            }
            if (!DependentModsActive())
            {
                if (Locations.MessageBoxShow("Active Mods differ from what this Mod depends on.\r\n\r\nDo you want to deactivate these Mods and activate the dependent Mods?",
                    MessageBoxButton.OK, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    while (ModManagement.DeactivateLastMod())
                    {
                    }
                    foreach (StringItem item in Configuration.DependsOn)
                    {
                        foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
                        {
                            if (config.ID == item.Text)
                            {
                                ModManagement.Activate(config);
                            }
                        }
                    }
                    ModManagement.NotifyMissionInstall();

                }
                else
                {
                    Locations.MessageBoxShow("Test canceled.  Please change Mod dependencies to match the list of Activated Mods.", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }
            }
           
           
            ModManagement.Activate(Configuration);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(WaitForActivation), Configuration.ID);



        }
        void WaitForActivation(object state)
        {
            string id = state as string;
            if (!string.IsNullOrEmpty(id))
            {
                do
                {
                    System.Threading.Thread.Sleep(1000);
                } while (!ModManagement.IsActive(id));
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(StartArtemis), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        void StartArtemis()
        {
            this.ShowInTaskbar = false;
            SessionMonitor win = new SessionMonitor();
            win.StartSession();
            this.DoAnimation();
            win.ShowDialog();
            this.ShowInTaskbar = true;
            this.WindowState = System.Windows.WindowState.Normal;
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

            for (double i = 500; i > 1; i -= 3)
            {
                this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;
                this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height;
                this.Width = i;
                this.Height = i;
                //System.Threading.Thread.Sleep(1);
            }
            this.Left = thisLeft;
            this.Top = thisTop;
            this.Width = thisWidth;
            this.Height = thisHeight;
            this.WindowState = System.Windows.WindowState.Minimized;


        }

        private void AddVariation_Click(object sender, RoutedEventArgs e)
        {
            SubMod sm = new SubMod();
            Configuration.SubMods.SubMods.Add(sm);
        }

        private void DeleteVariation_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                SubMod fm = btn.CommandParameter as SubMod;
                if (fm != null)
                {
                    Configuration.SubMods.SubMods.Remove(fm);
                }
            }
        }

        private void uc_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Configuration != null && vdc.Data != null && aINI.Data != null)
            {
                if (Configuration.IsChanged || vdc.Data.Changed || aINI.Data.Changed)
                {
                    switch (Locations.MessageBoxShow("Do you wish to save your changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                    {
                        case MessageBoxResult.Yes:
                            InstalledModConfigurations.Current.Save();
                            aINI.Data.Save();
                            e.Cancel = !vdc.DoSave();

                            break;
                        case MessageBoxResult.Cancel:
                            e.Cancel = true;
                            break;
                        case MessageBoxResult.No:
                            Configuration.RejectChanges();
                            break;
                    }
                }
            }
        }
        FileSystemWatcher fsw = null;
        void LoadFiles()
        {
            if (Configuration != null && !string.IsNullOrEmpty(Configuration.InstalledPath) && Directory.Exists(Configuration.InstalledPath))
            {
                Files = new FileGroup(new DirectoryInfo(Configuration.InstalledPath));

            }
            else
            {
                Files = null;
            }
        }
        public static readonly DependencyProperty FilesProperty =
           DependencyProperty.Register("Files", typeof(FileGroup),
           typeof(ModDevelopmentManager));


        public FileGroup Files
        {
            get
            {
                return (FileGroup)this.UIThreadGetValue(FilesProperty);

            }
            set
            {
                this.UIThreadSetValue(FilesProperty, value);

            }
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
