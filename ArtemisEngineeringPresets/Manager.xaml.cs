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
using System.Collections.ObjectModel;
using RussLibrary;
using ArtemisModLoader;
using RussLibrary.Helpers;
using System.IO;
using RussLibrary.Windows;
using Microsoft.Win32;
namespace ArtemisEngineeringPresets
{
    /// <summary>
    /// Interaction logic for Manager.xaml
    /// </summary>
    public partial class Manager : UserControl, ISettingsPanel
    {
        readonly string ArtemisTarget = System.IO.Path.Combine(Locations.ArtemisCopyPath, "engineeringSettings.dat");

        const string LibraryFolder = "EngineeringPresetLibrary";

        readonly string LibraryPath = System.IO.Path.Combine(Locations.DataPath, LibraryFolder);
        public Manager()
        {
            InitializeComponent();
            this.Dispatcher.BeginInvoke(new Action(LoadData), System.Windows.Threading.DispatcherPriority.Loaded);
            this.Dispatcher.BeginInvoke(new Action(LoadLibrary), System.Windows.Threading.DispatcherPriority.Loaded);
        }

        void LoadData()
        {
            if (!EngineeringHelper.IsSupportedVersion)
            {
                if (Locations.MessageBoxShow("The current Artemis version is not known to be compatible with this process.\r\n\r\nProceeding risks corrupting data.\r\n\r\nDo you wish to proceed?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    Okay = false;
                    return;
                }
            }
            CurrentFile = null;
            try
            {
                PresetItems = new ObservableCollection<Preset>(EngineeringHelper.LoadPresetFile(ArtemisTarget));
            }
            catch (InvalidPresetFileException)
            {
                ProcessInvalidFile(ArtemisTarget, false);
            }

        }
        void ProcessInvalidFile(string file, bool fromLibrary)
        {
            
            FileInfo fle = new FileInfo(file);
            
            if (Locations.MessageBoxShow(string.Format(System.Globalization.CultureInfo.CurrentCulture, 
                "Engineering Preset file is invalid.\r\n\r\n{0}\r\n\r\nDelete?",
                file), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
               
                if (fromLibrary)
                {
                    while (Library.Contains(fle.Name))
                    {
                        Library.Remove(fle.Name);
                    }
                }
                fle.Delete();
            }
        }
        void SaveAs()
        {
            PromptDialog diag = new PromptDialog();
            diag.Title = "Engineering Preset";
            diag.Label = "Provide a name for this preset:";
            if (diag.ShowDialog() == true)
            {
                string f = null;
                if (diag.Text.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
                {
                    f = diag.Text;
                }
                else
                {
                    f = diag.Text + ".dat";
                }
                if (IsAlreadyInLibrary(f))
                {
                    if (Locations.MessageBoxShow("File is already in library.\r\n\r\nOverwrite?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    string file = System.IO.Path.Combine(LibraryPath, f);
                    EngineeringHelper.WritePresetFile(file, PresetItems);
                    CurrentFile = f;
                    Library.Add(f);
                }
            }
        }
        void Export()
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Filter = "Data files (*.dat)|*.dat|All Files (*.*)|*.*";
            diag.DefaultExt = ".dat";
            diag.Title = "Select file to export to";
            if (diag.ShowDialog() == true)
            {
                EngineeringHelper.WritePresetFile(diag.FileName, PresetItems);
            }
        }
        bool IsAlreadyInLibrary(string name)
        {
            bool exists = false;
            foreach (string fle in Library)
            {
                if (name == fle)
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }
        void Import()
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = "Data files (*.dat)|*.dat|All Files (*.*)|*.*";
            diag.DefaultExt = ".dat";
            diag.Title = "Select file to import";
            if (diag.ShowDialog() == true)
            {
                try
                {
                    PresetItems = new ObservableCollection<Preset>(EngineeringHelper.LoadPresetFile(diag.FileName));
                }
                catch (InvalidPresetFileException)
                {
                    Locations.MessageBoxShow("Engineering preset file is invalid.  Cannot import.", MessageBoxButton.OK, MessageBoxImage.Hand);
                }
                string file = new FileInfo(diag.FileName).Name;
                
                if (IsAlreadyInLibrary(file))
                {
                    PromptDialog diagP = new PromptDialog();
                    diagP.Title = "Engineering Preset";
                    diagP.Label = "The name already exists.   Provide a new name for the imported preset:";
                    if (diagP.ShowDialog() == true)
                    {
                        string f = null;
                        if (diagP.Text.EndsWith(".dat", StringComparison.OrdinalIgnoreCase))
                        {
                            file = diagP.Text;
                        }
                        else
                        {
                            file = diagP.Text + ".dat";
                        }

                        if (IsAlreadyInLibrary(f))
                        {
                            if (Locations.MessageBoxShow("File is already in library.\r\n\r\nOverwrite?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                            {
                                return;
                            }
                        }
                        f = System.IO.Path.Combine(LibraryPath, file);
                        EngineeringHelper.WritePresetFile(f, PresetItems);


                    }
                    else
                    {
                        CurrentFile = null;
                        return;
                    }
                }
                Library.Add(file);
                CurrentFile = file;

            }

        }
        void LoadLibrary()
        {

            FileHelper.CreatePath(LibraryPath);
            List<string> files = new List<string>();
            foreach (FileInfo f in new DirectoryInfo(LibraryPath).GetFiles())
            {
                files.Add(f.Name);
            }
            if (files.Count == 0)
            {
                try
                {
                    IList<Preset> collection = EngineeringHelper.LoadPresetFile(ArtemisTarget);

                    EngineeringHelper.WritePresetFile(System.IO.Path.Combine(LibraryPath, "Artemis.dat"), collection);
                    files.Add("Artemis.dat");
                }
                catch (InvalidPresetFileException)
                {
                    //Can be ignored--handling this issue is already part of elsewhere.
                }
            }
            Library = new ObservableCollection<string>(files);
        }
        public string CurrentFile { get; set; }

        public static readonly DependencyProperty LibraryProperty =
          DependencyProperty.Register("Library", typeof(ObservableCollection<string>),
          typeof(Manager));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<string> Library
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(LibraryProperty);
            }
            set
            {
                this.UIThreadSetValue(LibraryProperty, value);
            }
        }


        public static readonly DependencyProperty PresetItemsProperty =
          DependencyProperty.Register("PresetItems", typeof(ObservableCollection<Preset>),
          typeof(Manager));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<Preset> PresetItems
        {
            get
            {
                return (ObservableCollection<Preset>)this.UIThreadGetValue(PresetItemsProperty);
            }
            set
            {
                this.UIThreadSetValue(PresetItemsProperty, value);
            }
        }

        public static readonly DependencyProperty OkayProperty =
         DependencyProperty.Register("Okay", typeof(bool),
         typeof(Manager), new PropertyMetadata(true));

        public bool Okay
        {
            get
            {
                return (bool)this.UIThreadGetValue(OkayProperty);
            }
            set
            {
                this.UIThreadSetValue(OkayProperty, value);
            }
        }

        private void LoadStored_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                string dat = btn.CommandParameter as string;
                if (!string.IsNullOrEmpty(dat))
                {
                    string f = System.IO.Path.Combine(LibraryPath, dat);
                    try
                    {
                        PresetItems = new ObservableCollection<Preset>(EngineeringHelper.LoadPresetFile(f));
                    }
                    catch (InvalidPresetFileException)
                    {
                        ProcessInvalidFile(f, true);
                        
                    }
                }
           }
        }

        private void MakeActive_Click(object sender, RoutedEventArgs e)
        {
            EngineeringHelper.WritePresetFile(ArtemisTarget, PresetItems);
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            Import();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Export();
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentFile))
            {
                SaveAs();
            }
            else
            {
                string file = System.IO.Path.Combine(LibraryPath, CurrentFile);
                EngineeringHelper.WritePresetFile(file, PresetItems);
            }
        }

        private void DeleteStored_Click(object sender, RoutedEventArgs e)
        {
             Button btn = sender as Button;
             if (btn != null)
             {
                 string dat = btn.CommandParameter as string;
                 if (!string.IsNullOrEmpty(dat))
                 {
                     FileInfo f = new FileInfo(System.IO.Path.Combine(LibraryPath, dat));
                     if (f.Exists)
                     {
                         f.Delete();
                     }
                     while (Library.Contains(dat))
                     {
                         Library.Remove(dat);
                     }
                 }
             }
        }

        public void SaveSettings()
        {
           
        }

        public void CancelChanges()
        {
            
        }

        public void LoadSettings()
        {
            
        }

        public void SetConfigurationPath(string path)
        {
            
        }

        public string Header
        {
            get { return "Engineering Presets"; }
        }
    }
}
