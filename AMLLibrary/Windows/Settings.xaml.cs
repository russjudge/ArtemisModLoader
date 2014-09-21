using System.IO;
using System.Windows;
using Microsoft.Win32;
using RussLibrary;
using RussLibrary.Controls;
using System.Windows.Media;
namespace ArtemisModLoader.Windows
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
           
            InitializeComponent();
            //System.Windows.SystemParameters.PrimaryScreenHeight
            ImageBrush brsh = ArtemisModLoader.Helpers.FileHelper.GetRandomSkybox();
            if (brsh != null)
            {
                this.Background = brsh;
            }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog();
            diag.ShowNewFolderButton = false;

            if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool isOkay = true;
                if (!File.Exists(System.IO.Path.Combine(diag.SelectedPath, Locations.ArtemisEXE)))
                {
                    isOkay = (Locations.MessageBoxShow("Artemis executable not found.  Are you sure of this path?",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);

                }
                if (isOkay)
                {
                    UserConfiguration.Current.ArtemisInstallPath = diag.SelectedPath;
                }

            }
        }
       

        private void Extender_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = Locations.AssemblyTitle;
            diag.Multiselect = false;
            diag.CheckFileExists = true;
            diag.CheckPathExists = true;
            if (diag.ShowDialog() == true)
            {
                UserConfiguration.Current.ArtemisExtenderPath = diag.FileName;


            }
        }

        private void ControlsINI_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("Notepad.exe", string.Format("\"{0}\"", System.IO.Path.Combine(Locations.ArtemisCopyPath, "controls.ini")));
        }
        bool UpdateConfig = false;
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            UpdateConfig = false;
            
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            UpdateConfig = true;
           
            this.Close();
        }

        private void MissionEditor_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Please select the Artemis Mission Editor Tool";
            diag.Multiselect = false;
            diag.CheckFileExists = true;
            diag.CheckPathExists = true;
            diag.Filter = "Mission Editor|ArtemisMissionEditor.exe|All files|*.*";
            if (diag.ShowDialog() == true)
            {
                UserConfiguration.Current.MissionEditorPath = diag.FileName;


            }
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetMaxSize();
        }

        private void uc_Closed(object sender, System.EventArgs e)
        {
            if (UpdateConfig)
            {
                UserConfiguration.Current.Save();
                UserConfiguration.Current.AcceptChanges();
            }
            else
            {
                UserConfiguration.Current.RejectChanges();
            }
        }
        //EngineeringControl
        private void EditDMX_Click(object sender, RoutedEventArgs e)
        {
            EditorWindow.Show(null, System.IO.Path.Combine(Locations.ArtemisCopyPath, "dat", "DMXcommands.xml"));
           
        }

        public static readonly DependencyProperty EngineeringControlProperty =
          DependencyProperty.Register("EngineeringControl", typeof(UIElement),
          typeof(Settings));

        public UIElement EngineeringControl
        {
            get
            {
                return (UIElement)this.UIThreadGetValue(EngineeringControlProperty);
            }
            set
            {
                this.UIThreadSetValue(EngineeringControlProperty, value);
            }
        }


    }
}
