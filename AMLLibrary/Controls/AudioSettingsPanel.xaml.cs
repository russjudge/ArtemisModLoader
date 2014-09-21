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
using RussLibrary.Windows;
using ArtemisModLoader.Text;
using RussLibrary;
using RussLibrary.Text;
using Microsoft.Win32;
namespace ArtemisModLoader.Controls
{
    /// <summary>
    /// Interaction logic for AudioSettingsPanel.xaml
    /// </summary>
    public partial class AudioSettingsPanel : UserControl, ISettingsPanel
    {
        public AudioSettingsPanel()
        {
            InitializeComponent();
        }

        public void SaveSettings()
        {
            INIConverter.ToINI(AudioConfig, configurationPath);
            AudioConfig.ResetAudioServer();
        }
       


        public void CancelChanges()
        {
            AudioConfig.RejectChanges();
        }

        public void LoadSettings()
        {
            AudioConfig = INIConverter.ToObject(configurationPath, typeof(AudioConfiguration)) as AudioConfiguration;
            AudioConfig.SetAudioCollection();
            if (AudioConfig.AudioCollection.Count == 0)
            {
                AudioConfig.LoadDefault();
                AudioConfig.ResetAudioServer();
            }
            AudioConfig.AcceptChanges();
        }

        public void SetConfigurationPath(string path)
        {
            configurationPath = System.IO.Path.Combine(path, "audio.ini");
        }
        string configurationPath = null;
        public string Header
        {
            get { return "Audio Settings"; }
        }

        public static readonly DependencyProperty AudioConfigProperty =
          DependencyProperty.Register("AudioConfig", typeof(AudioConfiguration),
          typeof(AudioSettingsPanel));

        public AudioConfiguration AudioConfig
        {
            get
            {
                return (AudioConfiguration)this.UIThreadGetValue(AudioConfigProperty);
            }
            set
            {
                this.UIThreadSetValue(AudioConfigProperty, value);
            }
        }

        private void Add_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Select sound file";
            diag.Filter = "Audio File (*.ogg, *.wma, *.aiff, *.mp3)|*.ogg;*.wma;*.aiff;*.mp3|All Files(*.*)|*.*";
            if (diag.ShowDialog() == true)
            {
                AudioConfig.AudioCollection.Add(diag.FileName);
            }

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                string file = btn.CommandParameter as string;
                if (!string.IsNullOrEmpty(file))
                {
                    AudioConfig.AudioCollection.Remove(file);
                }
            }
        }
    }
}
