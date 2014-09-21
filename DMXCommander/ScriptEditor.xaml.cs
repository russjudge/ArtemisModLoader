using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using Microsoft.Win32;
using RussLibrary;
using DMXCommander.Engine;
using System.Reflection;

namespace DMXCommander
{
    /// <summary>
    /// Interaction logic for ScriptEditor.xaml
    /// </summary>
    public partial class ScriptEditor : UserControl
    {
        public ScriptEditor()
        {
            LogData = new ObservableCollection<string>();
            InitializeComponent();
        }

        public static readonly DependencyProperty LogDataProperty =
            DependencyProperty.Register("LogData", typeof (ObservableCollection<string>),
                typeof (ScriptEditor));

        public ObservableCollection<string> LogData
        {
            get
            {
                return (ObservableCollection<string>) this.UIThreadGetValue(LogDataProperty);

            }
            set
            {
                this.UIThreadSetValue(LogDataProperty, value);

            }
        }

        public static readonly DependencyProperty ScriptDataProperty =
            DependencyProperty.Register("ScriptData", typeof (string),
                typeof (ScriptEditor));

        public string ScriptData
        {
            get
            {
                return (string) this.UIThreadGetValue(ScriptDataProperty);

            }
            set
            {
                this.UIThreadSetValue(ScriptDataProperty, value);

            }
        }

        public static readonly DependencyProperty SaveFileProperty =
            DependencyProperty.Register("SaveFile", typeof (string),
                typeof (ScriptEditor));

        public string SaveFile
        {
            get
            {
                return (string) this.UIThreadGetValue(SaveFileProperty);

            }
            set
            {
                this.UIThreadSetValue(SaveFileProperty, value);

            }
        }



        private void OnNew(object sender, RoutedEventArgs e)
        {
            SaveFile = string.Empty;
            ScriptData = string.Empty;
            LogData = new ObservableCollection<string>();
        }

        private void OnOpen(object sender, RoutedEventArgs e)
        {
            LogData = new ObservableCollection<string>();
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = "Open Script";
            diag.Filter = "DMX Script (*.DMXCommand)|*.DMXCommand|All Files (*.*)|*.*";
            diag.CheckFileExists = true;
            if (diag.ShowDialog() == true)
            {
                SaveFile = diag.FileName;
                using (StreamReader sr = new StreamReader(SaveFile))
                {
                    ScriptData = sr.ReadToEnd();
                }
                changed = false;
            }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(SaveFile))
            {
                SaveAs();
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(SaveFile))
                {
                    sw.Write(ScriptData);
                }
                changed = false;
            }
        }

        private void SaveAs()
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "Save Script";
            diag.Filter = "DMX Script (*.DMXCommand)|*.DMXCommand|All Files (*.*)|*.*";
            diag.DefaultExt = "DMXCommand";
            if (diag.ShowDialog() == true)
            {
                SaveFile = diag.FileName;
                Save();
            }
        }

        private void OnSaveAs(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void OnRun(object sender, RoutedEventArgs e)
        {

            btnRun.IsEnabled = false;
            if (changed)
            {
                if (MessageBox.Show("Do you wish to save your changes?", "DMX Script", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Save();
                }
            }
            ScriptEngine.Current.LogEvent += Current_LogEvent;
            ScriptEngine.Current.RunComplete += Current_RunComplete;
            ScriptEngine.Current.Run(this.SaveFile);


        }

        void Current_RunComplete(object sender, EventArgs e)
        {
            ScriptEngine.Current.LogEvent -= Current_LogEvent;
            ScriptEngine.Current.RunComplete -= Current_RunComplete;
            btnRun.IsEnabled = true;
        }

        void Current_LogEvent(object sender, EventArgs e)
        {
            if (sender != null)
            {
                LogData.Add(sender.ToString());
            }
        }

        private void OnReference(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(System.IO.Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName, "DMX_ScriptReference.txt"));
        }
        bool changed = false;
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            changed = true;
        }
    }
}
