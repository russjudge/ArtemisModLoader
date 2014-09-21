using log4net;
using RussLibrary.Helpers;
using System.Reflection;
using System.Windows;


namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(App));


        public const string UpdateCheckURIDebugger = "https://dl.dropboxusercontent.com/u/14746342/AML_versionData_debug.txt";

        public const string UpdateCheckURI = "https://dl.dropboxusercontent.com/u/14746342/AML_versionData.txt";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            GeneralHelper.LogSystemData();
            if (ArtemisModLoader.Properties.Settings.Default.UpdateRequired)
            {
                ModManagement.CheckInstalledModDefinitionVersions();
                ArtemisModLoader.Properties.Settings.Default.Upgrade();
                ArtemisModLoader.Properties.Settings.Default.UpdateRequired = false;
                ArtemisModLoader.Properties.Settings.Default.Save();
            }
           

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
       
        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ArtemisModLoader.Properties.Settings.Default.Save();
            if (RussLibraryAudio.AudioServer.Current.IsPlaying)
            {
                RussLibraryAudio.AudioServer.Current.Stop();
            }

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            try
            {
                if (RussLibraryAudio.AudioServer.Current.IsPlaying)
                {
                    RussLibraryAudio.AudioServer.Current.Stop();
                }
            }
            catch { }
            GeneralHelper.ShowApplicationError(e.Exception);
           

            Shutdown(1);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
       
    }
}
