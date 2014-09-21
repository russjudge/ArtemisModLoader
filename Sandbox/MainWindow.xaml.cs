using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using ArtemisEngineeringPresets;
using ArtemisModLoader;
using ArtemisModLoader.Windows;
using ArtemisModLoader.Xml;
using Microsoft.Win32;
using RussLibrary;
using RussLibrary.Helpers;
using VesselDataLibrary.Controls;
using VesselDataLibrary.Xml;
namespace Sandbox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Beam = new BeamPort();
            Files = new ObservableCollection<FileMap>();
            InitializeComponent();
            ModManagement.Downloaded += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_Downloaded);
            ModManagement.DownloadFailed += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadFailed);
            Configuration = InstalledModConfigurations.Current.Configurations.Configurations[0];

         
        }

        public static readonly DependencyProperty ConfigurationProperty =
          DependencyProperty.Register("Configuration", typeof(ModConfiguration),
          typeof(MainWindow));

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


        public static readonly DependencyProperty FilesProperty =
          DependencyProperty.Register("Files", typeof(ObservableCollection<FileMap>),
          typeof(MainWindow));

        public ObservableCollection<FileMap> Files
        {
            get
            {
                return (ObservableCollection<FileMap>)this.UIThreadGetValue(FilesProperty);
            }
            set
            {
                this.UIThreadSetValue(FilesProperty, value);
            }
        }






        public static readonly DependencyProperty BeamProperty =
           DependencyProperty.Register("Beam", typeof(BeamPort),
           typeof(MainWindow));

        public BeamPort Beam
        {
            get
            {
                return (BeamPort)this.UIThreadGetValue(BeamProperty);

            }
            set
            {
                this.UIThreadSetValue(BeamProperty, value);

            }
        }
        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(VesselDataObject),
           typeof(MainWindow));

        public VesselDataObject Data
        {
            get
            {
                return (VesselDataObject)this.UIThreadGetValue(DataProperty);
            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);
            }
        }
        

        private void LoadVesselData_Click(object sender, RoutedEventArgs e)
        {
            ModDevelopmentManager win = new ModDevelopmentManager();
            win.Show();
        }

        private void XMLGen_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Indiag = new OpenFileDialog();
            Indiag.Title = "Select XML file to process";
            Indiag.Multiselect = false;
            Indiag.Filter = "Xml Files (*.xml)|*.xml|all Files|*.*";
            if (Indiag.ShowDialog() == true)
            {
                System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog();
                diag.Description = "Select folder to save classes to";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //try
                    //{
                    XmlClassGenerator gen = new XmlClassGenerator();
                    gen.GenerateClasses(Indiag.FileName, diag.SelectedPath,
                        MessageBox.Show("Do you wish to make DependencyObjects?",
                        "Generate Classes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show("Problem:\r\n\r\n" + ex.ToString());
                    //}
                    MessageBox.Show("Done.");
                }
            }
            
        }

        private void ModDef_click(object sender, RoutedEventArgs e)
        {
            ModDefinitionSetup win = new ModDefinitionSetup();
            win.Show();
        }
        List<ModConfiguration> TestConfigs = null;
        private void TestAllDownloads()
        {
            TestConfigs = new List<ModConfiguration>();
            foreach (ModConfiguration config in ModManagement.GetPredefinedMods().Values)
            {
                if (config.ID != DataStrings.StockID)
                {
                    if (!string.IsNullOrEmpty(config.Download.Source))
                    {

                        if (ModManagement.StartDownload(config))
                        {
                            TestConfigs.Add(config);
                        }
                    }
                }
            }
            //ModManagement.Downloaded -= new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_Downloaded);
            //ModManagement.DownloadFailed -= new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadFailed);
        }

        void ModManagement_DownloadFailed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e != null)
            {
                ModConfiguration mod = e.UserState as ModConfiguration;
                MessageBox.Show(mod.Title + " failed to download");
                UpdateTestConfigs(mod);

            }
        }
        void UpdateTestConfigs(ModConfiguration mod)
        {
            if (TestConfigs.Contains(mod))
            {
                TestConfigs.Remove(mod);

            }
            if (TestConfigs.Count < 1)
            {
                MessageBox.Show("done");
            }
        }
        void ModManagement_Downloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

            if (e != null)
            {
                ModConfiguration mod = e.UserState as ModConfiguration;

                if (!FileHelper.IsValidCompressedFile(mod.PackagePath))
                {
                    MessageBox.Show(mod.Title + " had invalid compressed file.");
                }
                UpdateTestConfigs(mod);
            }
        }

        private void TestDownloads_Click(object sender, RoutedEventArgs e)
        {
            TestAllDownloads();
        }

        private void INIGen_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog oDiag = new OpenFileDialog();
            oDiag.Title = "Select INI File";
            oDiag.Filter = "INI Files|*.ini|all Files|*.*";
            string INIFle = null;
            if (oDiag.ShowDialog() == true)
            {
                INIFle = oDiag.FileName;
                SaveFileDialog sDiag = new SaveFileDialog();
                sDiag.Title = "Select Class filename";
                sDiag.DefaultExt = ".cs";
                sDiag.Filter = "C# files|*.cs|All Files|*.*";
                if (sDiag.ShowDialog() == true)
                {
                    FileInfo targFle = new FileInfo(sDiag.FileName);
                    string classname = targFle.Name.Substring(0, targFle.Name.Length - targFle.Extension.Length);
                    INIClassGenerator.CreateClass(INIFle, "~~~~", classname, targFle.FullName);
                    MessageBox.Show("Done");
                }
            }
            
        }
       
        private void XamlGen_click(object sender, RoutedEventArgs e)
        {
             OpenFileDialog oDiag = new OpenFileDialog();
            oDiag.Title = "Select C# File";
            oDiag.Filter = "C# Files|*.cs|All Files|*.*";
   
            if (oDiag.ShowDialog() == true)
            {
                XamlGenerator.ProcessFileForGrid(oDiag.FileName);
               
            }

        }

        private void AddBaseFile_Click(object sender, RoutedEventArgs e)
        {
            Configuration.BaseFiles.Files.Add(new FileMap());
        }

        private void ActivateStock_Click(object sender, RoutedEventArgs e)
        {
            bool WasPlaying = RussLibraryAudio.AudioServer.Current.IsPlaying;
            if (WasPlaying)
            {
                RussLibraryAudio.AudioServer.Current.Stop();
            }
            if (!ModManagement.IsInstalled(DataStrings.StockID))
            {
                if (!Directory.Exists(System.IO.Path.Combine(Locations.InstalledModsPath, DataStrings.StockID)))
                {
                    Locations.MessageBoxShow("Stock Mod is not installed and Stock Mod install folder does not exist--cannot continue.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    ModConfiguration configuration = new ModConfiguration(DataStrings.ModStockFile);
                    configuration.InstalledPath = System.IO.Path.Combine(Locations.InstalledModsPath, DataStrings.StockID);
                    InstalledModConfigurations.Current.Configurations.Configurations.Insert(0, configuration);
                    InstalledModConfigurations.Current.Save();
                }
            }
            foreach (ModConfiguration configuration in InstalledModConfigurations.Current.Configurations.Configurations)
            {
                if (configuration.ID == DataStrings.StockID)
                {
                    ModManagement.Activate(configuration);
                    Locations.MessageBoxShow("Activated stock.", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            if (WasPlaying)
            {
                RussLibraryAudio.AudioServer.Current.PlayNextInQueue();
            }
            Locations.MessageBoxShow("done.", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Preset_Click(object sender, RoutedEventArgs e)
        {

            RussLibraryAudio.AudioServer.Current.PlayAsync(@"F:\Applications\Artemis\Artemis\dat\At Present - Tense - C.ogg");

            System.Threading.ThreadPool.QueueUserWorkItem(wait);
        }
        void wait(object state)
        {
            //System.Threading.Thread.Sleep(5000);
            //for (int i = 0; i < 20; i++)
            //{
            //    System.Threading.Thread.Sleep(1000);
            //    RussLibraryAudio.AudioServer.Current.SetVolume(RussLibraryAudio.AudioServer.Current.GetVolume() / 2);
            //}
        }
        private void LoadEngineering_Click(object sender, RoutedEventArgs e)
        {
           // engPreset.PresetItems = new ObservableCollection<Preset>(EngineeringHelper.LoadPresetFile(Path.Combine(Locations.ArtemisCopyPath, "engineeringSettings.dat")));
        }

      

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            RussLibraryAudio.AudioServer.Current.Stop();
        }
    }
}
