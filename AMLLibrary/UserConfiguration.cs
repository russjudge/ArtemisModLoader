using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using RussLibrary;
using RussLibrary.Text;
using System.IO;
using RussLibrary.Helpers;
using RussLibrary.WPF;
using log4net;
using System.Reflection;
namespace ArtemisModLoader
{
    public class UserConfiguration : ChangeDependencyObject
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(UserConfiguration));


        public static readonly DependencyProperty IsProcessingProperty =
          DependencyProperty.Register("IsProcessing", typeof(bool),
          typeof(UserConfiguration));


        public bool IsProcessing
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsProcessingProperty);

            }
            set
            {
                this.UIThreadSetValue(IsProcessingProperty, value);

            }
        }




        private UserConfiguration() { }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static UserConfiguration()
        {
            bool DataConverted = false;
            if (File.Exists(Locations.ConfigFile))
            {
                using (StreamReader sr = new StreamReader(Locations.ConfigFile))
                {
                    string sLine = sr.ReadLine();
                    if (!sLine.Contains("="))
                    {
                        Current = new UserConfiguration();
                        Current.ArtemisInstallPath = sLine;
                        if (!string.IsNullOrEmpty(Current.ArtemisInstallPath))
                        {
                            if (!File.Exists(Path.Combine(Current.ArtemisInstallPath, Locations.ArtemisEXE)))
                            {
                                Current.ArtemisInstallPath = Locations.FindArtemisInstallPath();
                            }
                        }
                        if (sLine != null)
                        {
                            sLine = sr.ReadLine();
                            if (!string.IsNullOrEmpty(sLine))
                            {
                                bool b;
                                if (bool.TryParse(sLine, out b))
                                {
                                    Current.UseArtemisExtender = b;
                                }
                            }
                            if (sLine != null)
                            {
                                sLine = sr.ReadLine();
                                Current.ArtemisExtenderPath = sLine;
                                if (sLine != null)
                                {
                                    sLine = sr.ReadLine();
                                    Current.ArtemisExtenderCopy = sLine;
                                }
                            }
                        }
                        DataConverted = true;
                    }
                }
            }
            if (DataConverted)
            {
                FileHelper.DeleteFile(Locations.ConfigFile);
                Current.Save();
            }
            else
            {
                Current = INIConverter.ToObject(Locations.ConfigFile, new UserConfiguration()) as UserConfiguration;
            }
            if (string.IsNullOrEmpty(Current.ArtemisExtenderCopy) || !File.Exists(Current.ArtemisExtenderCopy))
            {
                Current.UseArtemisExtender = false;
            }
            Current.Original = new UserConfiguration();

            Current.AcceptChanges();
        }

        
        public string ArtemisExtenderConfig
        {
            get
            {
                string retVal = string.Empty;
                if (!string.IsNullOrEmpty(ArtemisExtenderCopy))
                {
                    retVal = Path.Combine(new FileInfo(ArtemisExtenderCopy).DirectoryName, "ArtemisPath.cfg");
                }
                return retVal;
            }
        }

        private void MakeArtemisExtenderCopy()
        {
            ArtemisExtenderCopy = Path.Combine(Locations.DataPath, "ArtemisExtender", new FileInfo(ArtemisExtenderPath).Name);
            string targetPath = new FileInfo(ArtemisExtenderCopy).DirectoryName;
     
            FileHelper.CopyFiles(new FileInfo(ArtemisExtenderPath).Directory, targetPath);
            if (File.Exists(ArtemisExtenderConfig))
            {
                File.Delete(ArtemisExtenderConfig);
            }
            using (StreamWriter sw = new StreamWriter(ArtemisExtenderConfig))
            {
                sw.Write(Locations.ArtemisCopyPath);
            }
        }
        
        [INIConversion("ArtemisExtenderCopy")]
        public string ArtemisExtenderCopy { get; set; }
        public void Save()
        {
            INIConverter.ToINI(this, Locations.ConfigFile);
        }
        public static UserConfiguration Current { get; private set; }


        public static readonly DependencyProperty ConfigurationVersionProperty =
          DependencyProperty.Register("ConfigurationVersion", typeof(double),
          typeof(UserConfiguration), new PropertyMetadata(1D));
        [INIConversion("ConfigurationVersion")]
        public double ConfigurationVersion
        {
            get
            {
                return (double)this.UIThreadGetValue(ConfigurationVersionProperty);

            }
            set
            {
                this.UIThreadSetValue(ConfigurationVersionProperty, value);

            }
        }
        public void PromptUserForArtemisPath()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(PromptUserForArtemisPath));
            //}
            //else
            //{

                ArtemisInstallPath = Locations.FindArtemisInstallPath();
                if (string.IsNullOrEmpty(ArtemisInstallPath) || !Directory.Exists(ArtemisInstallPath))
                {
                    //Application.Current.Dispatcher.Invoke(
                    //    new Func<string, MessageBoxButton, MessageBoxImage, MessageBoxResult>(Locations.MessageBoxShow),
                    //    AMLResources.Properties.Resources.CannotLocateArtemisInstall
                    //    + DataStrings.CRCR
                    //    + AMLResources.Properties.Resources.PleaseSpecifyLocation,
                    //    MessageBoxButton.OK, MessageBoxImage.Hand);

                    //MessageBox.Show(AMLResources.Properties.Resources.CannotLocateArtemisInstall
                    //    + DataStrings.CRCR
                    //    + AMLResources.Properties.Resources.PleaseSpecifyLocation, "Artemis Mod Loader",
                    //    MessageBoxButton.OK, MessageBoxImage.Hand);

                  
                    Locations.MessageBoxShow(
                        AMLResources.Properties.Resources.CannotLocateArtemisInstall
                        + DataStrings.CRCR
                        + AMLResources.Properties.Resources.PleaseSpecifyLocation,
                        MessageBoxButton.OK, MessageBoxImage.Hand);
                    using (System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog())
                    {
                        diag.Description = "Cannot locate the Artemis installation.  Please specify the location where Artemis is installed.";
                        diag.ShowNewFolderButton = false;
                        if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            ArtemisInstallPath = diag.SelectedPath;
                            Save();

                        }
                    }
                }
                else
                {
                    Save();
                }
            //}
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetArtemisInstallPath()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (string.IsNullOrEmpty(ArtemisInstallPath) || !Directory.Exists(ArtemisInstallPath))
            {
               PromptUserForArtemisPath();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return ArtemisInstallPath;
        }
        public static readonly DependencyProperty ArtemisInstallPathProperty =
          DependencyProperty.Register("ArtemisInstallPath", typeof(string),
          typeof(UserConfiguration));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods"), INIConversion("ArtemisInstallPath")]
        public string ArtemisInstallPath
        {
            get
            {
                return (string)this.UIThreadGetValue(ArtemisInstallPathProperty);

            }
            set
            {
                this.UIThreadSetValue(ArtemisInstallPathProperty, value);

            }
        }


        public static readonly DependencyProperty UseArtemisExtenderProperty =
          DependencyProperty.Register("UseArtemisExtender", typeof(bool),
          typeof(UserConfiguration));
        [INIConversion("UseArtemisExtender")]
        public bool UseArtemisExtender
        {
            get
            {
                return (bool)this.UIThreadGetValue(UseArtemisExtenderProperty);

            }
            set
            {
                this.UIThreadSetValue(UseArtemisExtenderProperty, value);

            }
        }


        static void OnArtemisExtenderPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UserConfiguration me = sender as UserConfiguration;
            if (me != null)
            {
                if (!string.IsNullOrEmpty(me.ArtemisExtenderPath) && File.Exists(me.ArtemisExtenderPath))
                {
                    me.MakeArtemisExtenderCopy();
                    me.UseArtemisExtender = true;
                }
                else
                {
                    me.UseArtemisExtender = false;
                }

            }
        }
        public static readonly DependencyProperty ArtemisExtenderPathProperty =
          DependencyProperty.Register("ArtemisExtenderPath", typeof(string),
          typeof(UserConfiguration), new PropertyMetadata(OnArtemisExtenderPathChanged));
        [INIConversion("ArtemisExtenderPath")]
        public string ArtemisExtenderPath
        {
            get
            {
                return (string)this.UIThreadGetValue(ArtemisExtenderPathProperty);

            }
            set
            {
                this.UIThreadSetValue(ArtemisExtenderPathProperty, value);

            }
        }


        public static readonly DependencyProperty UseMissionEditorProperty =
          DependencyProperty.Register("UseMissionEditor", typeof(bool),
          typeof(UserConfiguration));

        public bool UseMissionEditor
        {
            get
            {
                return (bool)this.UIThreadGetValue(UseMissionEditorProperty);

            }
            set
            {
                this.UIThreadSetValue(UseMissionEditorProperty, value);

            }
        }


        public static readonly DependencyProperty MissionEditorPathProperty =
          DependencyProperty.Register("MissionEditorPath", typeof(string),
          typeof(UserConfiguration), new PropertyMetadata(OnMissionCommandLineChange));
        [INIConversion("MissionEditorPath")]
        public string MissionEditorPath
        {
            get
            {
                return (string)this.UIThreadGetValue(MissionEditorPathProperty);

            }
            set
            {
                this.UIThreadSetValue(MissionEditorPathProperty, value);

            }
        }
        static void OnMissionCommandLineChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            UserConfiguration me = sender as UserConfiguration;
            if (me != null)
            {
                StringBuilder sb = new StringBuilder();
                if (!string.IsNullOrEmpty(me.MissionEditorPath))
                {
                    sb.Append(me.MissionEditorPath);
                    if (!string.IsNullOrEmpty(me.MissionEditorParameters))
                    {
                        sb.Append(" ");
                        sb.Append(me.MissionEditorParameters);
                    }
                    me.MissionEditorCommandLine = sb.ToString();
                }

                me.UseMissionEditor = (!string.IsNullOrEmpty(me.MissionEditorPath)
                       && !string.IsNullOrEmpty(me.MissionEditorParameters)
                       && File.Exists(me.MissionEditorPath));
            }
        }
        public static readonly DependencyProperty MissionEditorParametersProperty =
          DependencyProperty.Register("MissionEditorParameters", typeof(string),
          typeof(UserConfiguration), new PropertyMetadata("\"{0}\" -v \"{1}\""));
        
        public string MissionEditorParameters
        {
            get
            {
                return (string)this.UIThreadGetValue(MissionEditorParametersProperty);

            }
            set
            {
                this.UIThreadSetValue(MissionEditorParametersProperty, value);

            }
        }


        public static readonly DependencyProperty MissionEditorCommandLineProperty =
          DependencyProperty.Register("MissionEditorCommandLine", typeof(string),
          typeof(UserConfiguration));
        
        public string MissionEditorCommandLine
        {
            get
            {
                return (string)this.UIThreadGetValue(MissionEditorCommandLineProperty);

            }
            private set
            {
                this.UIThreadSetValue(MissionEditorCommandLineProperty, value);

            }
        }

        protected override void ProcessValidation()
        {
            
        }
    }
}
