using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ArtemisModLoader;
using ArtemisModLoader.Helpers;
using ArtemisModLoader.Xml;
using Microsoft.Win32;
using RussLibrary;
using VesselDataLibrary.Text;
using System.IO;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for ArtemisINIControl.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
    public partial class ArtemisINIControl : UserControl, IDisposable
    {
        public ArtemisINIControl()
        {
            InitializeComponent();
            fsw = new FileSystemWatcher();
            fsw.Changed += new FileSystemEventHandler(fsw_Changed);
            fsw.NotifyFilter = NotifyFilters.LastWrite;
            
        }
        ~ArtemisINIControl()
        {

            Dispose(false);

        }
        FileSystemWatcher fsw = null;

        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            if (Locations.MessageBoxShow("Something else has changed the artemis.ini file.\r\n\r\nDo you wish to reload?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Data.Dispatcher.Invoke(new Action(LoadINIFile));
            }
        }

        public static readonly DependencyProperty SearchPrefixesProperty =
            DependencyProperty.Register("SearchPrefixes", typeof(ObservableCollection<string>),
            typeof(ArtemisINIControl));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<string> SearchPrefixes
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(SearchPrefixesProperty);

            }
            set
            {
                this.UIThreadSetValue(SearchPrefixesProperty, value);

            }
        }



        public static readonly DependencyProperty DataProperty =
          DependencyProperty.Register("Data", typeof(ArtemisINI),
          typeof(ArtemisINIControl));
        
        public ArtemisINI Data
        {
            get
            {
                return (ArtemisINI)this.UIThreadGetValue(DataProperty);
            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);
            }
        }
        void LoadINIFile(object state)
        {
            while (Configuration != null && string.IsNullOrEmpty(Configuration.InstalledPath))
            {
                System.Threading.Thread.Sleep(250);
            }
            this.Dispatcher.BeginInvoke(new Action(LoadINIFile));
        }
        void LoadINIFile()
        {
            Reset();
            if (Configuration != null && !string.IsNullOrEmpty(Configuration.InstalledPath))
            {
                ini = ArtemisModLoader.Helpers.FileHelper.LocateExpectedFileInMod("artemis.ini", Configuration);
                if (string.IsNullOrEmpty(ini))
                {
                    for (int i = Configuration.DependsOn.Count - 1; i >= 0; i--)
                    {
                        foreach (ModConfiguration config in ActiveModConfigurations.Current.Configurations.Configurations)
                        {
                            if (config.ID == Configuration.DependsOn[i].Text)
                            {
                                ini = ArtemisModLoader.Helpers.FileHelper.LocateExpectedFileInMod("artemis.ini", config);
                                if (!string.IsNullOrEmpty(ini))
                                {
                                    if (System.IO.File.Exists(ini))
                                    {
                                        string targ = System.IO.Path.Combine(Configuration.InstalledPath, "artemis.ini");
                                        RussLibrary.Helpers.FileHelper.Copy(ini, targ);
                                        ini = targ;

                                    }
                                }
                                break;
                            }

                        }
                        if (!string.IsNullOrEmpty(ini))
                        {
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ini))
                {
                    Data = new ArtemisINI(ini);
                }
                if (Data != null)
                {
                    SetWatcher(Data.INIPath);
                    Data.AcceptChanges();
                }
            }
        }
        string ini = null;
        public void Reset()
        {
            fsw.EnableRaisingEvents = false;
            Data = null;
       
           
            SearchPrefixes = null;
        }
        static void OnConfigurationChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArtemisINIControl me = sender as ArtemisINIControl;
            if (me != null && me.Configuration != null)
            {
                me.SearchPrefixes = new ObservableCollection<string>(ModManagement.SearchPrefixes(me.Configuration));
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(me.LoadINIFile));
            }
        }
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(ModConfiguration),
            typeof(ArtemisINIControl), new PropertyMetadata(OnConfigurationChange));


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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            fsw.EnableRaisingEvents = false;
            Data.Save();
            
            Data.AcceptChanges();
            SetWatcher(Data.INIPath);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            fsw.EnableRaisingEvents = false;
            OpenFileDialog diag = new OpenFileDialog();
            //TODO: put in language resource
            diag.Title = "Select Artemis.ini file";

            diag.InitialDirectory = Configuration.InstalledPath;
            diag.Filter = "Artemis INI files(*artemis*.ini)|*artemis*.ini|INI files (*.ini)|*.ini|All files (*.*)|*.*";

            if (diag.ShowDialog() == true)
            {
                Reset();
                Data = new ArtemisINI(diag.FileName);
                SetWatcher(Data.INIPath);
                Data.AcceptChanges();
            }
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            fsw.EnableRaisingEvents = false;
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Save Artemis.ini as...";
            diag.InitialDirectory = Configuration.InstalledPath;
            diag.FileName = System.IO.Path.Combine(Configuration.InstalledPath, "artemis.ini");
            diag.Filter = "Artemis INI files(*artemis*.ini)|*artemis*.ini|INI files (*.ini)|*.ini|All files (*.*)|*.*";

            if (diag.ShowDialog() == true)
            {
                
                Data.Save(diag.FileName);
                SetWatcher(Data.INIPath);
                Data.AcceptChanges();
            }
        }
        private void SetWatcher(string path)
        {
            FileInfo f = new FileInfo(path);
            fsw.Path = f.DirectoryName;
            fsw.Filter = f.Name;
            fsw.EnableRaisingEvents = true;
        }
        private void FileSelectionControl_InvalidFilePath(object sender, RoutedEventArgs e)
        {
            FileHelper.FileSelectionControl_InvalidFilePath(sender, e, Configuration);
        }

        private void EditArtemisINI_Click(object sender, RoutedEventArgs e)
        {
            
            if (!string.IsNullOrEmpty(Data.INIPath))
            {
                ProcessStartInfo strt = new ProcessStartInfo(Data.INIPath);
                strt.Verb = "Edit";
                strt.UseShellExecute = true;

                Process.Start(strt);
    
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Data.RejectChanges();
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
