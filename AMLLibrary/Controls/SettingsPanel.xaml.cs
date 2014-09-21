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
using RussLibrary.Controls;
using Microsoft.Win32;
using RussLibrary;
using RussLibrary.Windows;
using RussLibrary.Helpers;

namespace ArtemisModLoader.Controls
{
    /// <summary>
    /// Interaction logic for SettingsPanel.xaml
    /// </summary>
    public partial class SettingsPanel : UserControl, ISettingsPanel
    {
        public SettingsPanel()
        {
            InitializeComponent();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog())
            {
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
        }

        private void Extender_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = GeneralHelper.AssemblyTitle;
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
            System.Diagnostics.Process.Start("Notepad.exe", string.Format(System.Globalization.CultureInfo.CurrentCulture, 
                "\"{0}\"", System.IO.Path.Combine(Locations.ArtemisCopyPath, "controls.ini")));
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



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void EditDMX_Click(object sender, RoutedEventArgs e)
        {
            string DMXFile = System.IO.Path.Combine(Locations.ArtemisCopyPath, "dat", "DMXcommands.xml");
            EditorWindow.Show(null, DMXFile);
            Window win = new Window();
            win.Title = "DMX Commander";

            try
            {
                DMXCommander.Controls.DMXCommandControl DMX = new DMXCommander.Controls.DMXCommandControl();
                win.Closed += new EventHandler(win_Closed);
                DMX.LoadFile(DMXFile);
                win.Content = DMX;
                win.Show();
            }
            catch
            {
            }
        }

        void win_Closed(object sender, EventArgs e)
        {
            Window win = sender as Window;
            if (win != null)
            {
                DMXCommander.Controls.DMXCommandControl dmx = win.Content as DMXCommander.Controls.DMXCommandControl;
                if (dmx != null)
                {
                    dmx.Dispose();
                }
            }
        }

        public void SaveSettings()
        {
            UserConfiguration.Current.Save();
            UserConfiguration.Current.AcceptChanges();
        }

        public void LoadSettings()
        {
           
        }
        //string configurationPath;
        public void SetConfigurationPath(string path)
        {
            //configurationPath = path;
        }



        public void CancelChanges()
        {
            UserConfiguration.Current.RejectChanges();
        }
      


        public string Header
        {
            get
            {
                return "Settings";

            }
            
        }




    }
}
