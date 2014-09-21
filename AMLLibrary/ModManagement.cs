using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;
using ArtemisModLoader.EventArguments;
using ArtemisModLoader.Windows;
using ArtemisModLoader.Xml;
using log4net;
using Microsoft.Win32;
using RussLibrary.Helpers;
using RussLibrary.Windows;
using RussLibrary.Xml;
using SharpCompress.Reader;
using RussLibrary;

namespace ArtemisModLoader
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public static class ModManagement
    {

        public static void BrowseForFile()
        {
          
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = AMLResources.Properties.Resources.SupportedCompressedFiles + DataStrings.SupportedCompressedFilesFilter
                + "|" + AMLResources.Properties.Resources.AML + DataStrings.AMLFilter
                + "|" + AMLResources.Properties.Resources.AllFiles + DataStrings.AllFilesFilter;
            diag.Title = GeneralHelper.AssemblyTitle;
            diag.Multiselect = true;

            if (diag.ShowDialog() == true)
            {
                if (diag.FileNames != null)
                {
                    ModManagement.ProcessFiles(diag.FileNames);
                }

            }
        }
        public static void UpdateCheckProcess()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (ModManagement.CheckForUpdate())
            {
                if (Locations.MessageBoxShow(
                    string.Format(CultureInfo.InstalledUICulture,
                    "An update to Artemis has been detected.  Do you wish to apply that update to the copy that {0} runs?",
                    GeneralHelper.AssemblyTitle),
                   MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ModManagement.ProcessUpdate();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        /// <summary>
        /// Provides a list of paths valid for a configuration.  used for validating existence of files only.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        public static string[] SearchPrefixes(ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<string> retVal = new List<string>();
            if (config != null)
            {
                foreach (ModConfiguration c in InstalledModConfigurations.Current.Configurations.Configurations)
                {
                    if (c.ID == DataStrings.StockID && !string.IsNullOrEmpty(c.InstalledPath))
                    {
                        retVal.Add(c.InstalledPath);
                    }
                    else
                    {
                        foreach (StringItem item in config.DependsOn)
                        {
                            if (item.Text == c.ID)
                            {
                                if (!string.IsNullOrEmpty(c.InstalledPath))
                                {
                                    retVal.Add(c.InstalledPath);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal.ToArray();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static string GetActiveArtemisVersion()
        {
            Dictionary<DateTime, string> ArtemisVersionDictionary = GetArtemisVersionDictionary();
            FileInfo f = new FileInfo(Path.Combine(Locations.ArtemisCopyPath, "Artemis.exe"));
            DateTime checkDate = new DateTime(f.LastWriteTime.Year, f.LastWriteTime.Month, f.LastWriteTime.Day);
            //Exact date is unknown--it got missed.
            if (checkDate.CompareTo(new DateTime(2014, 6, 10)) < 0 && checkDate.CompareTo(new DateTime(2013, 8, 10)) > 0)
            {
                return "2.1";
            }
            else
            {
                if (ArtemisVersionDictionary.ContainsKey(checkDate))
                {
                    return ArtemisVersionDictionary[checkDate];
                }
                else
                {
                    return "Version unknown.";
                }
            }
        }
        static Dictionary<DateTime, string> GetArtemisVersionDictionary()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Dictionary<DateTime, string> retVal = new Dictionary<DateTime, string>();
            
            retVal.Add(new DateTime(2011, 07, 19), "v. 1.50");
            retVal.Add(new DateTime(2011, 12, 04), "v. 1.55");

            retVal.Add(new DateTime(2012, 01, 09), "v. 1.60");
            retVal.Add(new DateTime(2012, 03, 09), "v. 1.61");
            retVal.Add(new DateTime(2012, 04, 30), "v. 1.65");
            retVal.Add(new DateTime(2012, 09, 13), "v. 1.66");
            retVal.Add(new DateTime(2012, 09, 16), "v. 1.661");
            
            retVal.Add(new DateTime(2013, 01, 06), "v. 1.7");
            retVal.Add(new DateTime(2013, 03, 01), "v. 1.702");
            retVal.Add(new DateTime(2013, 08, 10), "v. 2.0");
            retVal.Add(new DateTime(2014, 06, 1), "v. 2.1"); //Exact date unknown--it got missed.
            retVal.Add(new DateTime(2014, 06, 11), "v. 2.1.1");

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        static readonly ILog _log = LogManager.GetLogger(typeof(ModManagement));
        private static Dictionary<string, ModConfiguration> _predefinedMods = null;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public static Dictionary<string, ModConfiguration> GetPredefinedMods()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (_predefinedMods == null)
            {
                _predefinedMods = new Dictionary<string, ModConfiguration>();
                if (Directory.Exists(Locations.PredefinedModsPath))
                {
                    foreach (System.IO.FileInfo fle in new System.IO.DirectoryInfo(Locations.PredefinedModsPath).GetFiles("*.aml"))
                    {
                        ModConfiguration config = new ModConfiguration(fle.FullName);

                        if (!_predefinedMods.ContainsKey(config.ID))
                        {
                            _predefinedMods.Add(config.ID, config);
                        }

                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return _predefinedMods;
        }
        public static void DoReset()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            while (ActiveModConfigurations.Current.DeactivateLastConfig()) ;
            ActiveModConfigurations.Current.DeactivateStock();
            ModConfiguration StockConfig = new ModConfiguration(Locations.MODStockDefinition);
            InstalledModConfigurations.Current.UninstallMod(StockConfig.ID);

            if (File.Exists(Locations.ActiveModsFile))
            {
                File.Delete(Locations.ActiveModsFile);
            }


            string stockPath = System.IO.Path.Combine(Locations.InstalledModsPath, StockConfig.ID);

            if (Directory.Exists(stockPath))
            {
                FileHelper.DeleteAllFiles(stockPath);
            }
            if (Directory.Exists(Locations.ArtemisCopyPath))
            {
                FileHelper.DeleteAllFiles(Locations.ArtemisCopyPath);
            }
            InstalledModConfigurations.ResetInstallation();
            ActiveModConfigurations.ResetActivations();
            ModManagement.DoInitialSetup();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static Queue<WebClient> AsyncActivity = new Queue<WebClient>();
        public static bool IsValidDownloadFile(string query)
        {

            if (!string.IsNullOrEmpty(query))
            {
                return (query.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                            || query.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                            || query.EndsWith(".rar", StringComparison.OrdinalIgnoreCase)
                            || query.EndsWith(".gzip", StringComparison.OrdinalIgnoreCase)
                            || query.EndsWith(".7zip", StringComparison.OrdinalIgnoreCase)
                            || query.EndsWith(".tar", StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return false;
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        public static string ExtractFileName(string url)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = url;
            if (!string.IsNullOrEmpty(url))
            {
                int i = url.LastIndexOf('/');
                retVal = url.Substring(i + 1);
                
                if (retVal.Contains("?"))
                {
                    string wrk = retVal.Substring(0, retVal.IndexOf('?'));

                    if (!IsValidDownloadFile(wrk))
                    {
                        //filename before "?" is not valid--try at end of line instead.
                        if (retVal.Contains("="))
                        {
                            wrk = retVal.Substring(retVal.LastIndexOf("=", StringComparison.OrdinalIgnoreCase) + 1);
                            if (!IsValidDownloadFile(wrk))
                            {
                                //not sure yet what to do--just go with original and give up.
                                wrk = null;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(wrk))
                    {
                        retVal = wrk;
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static bool StartDownload(ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            if (config != null)
            {
                string localFile = ExtractFileName(config.Download.Source);
               
                string PackageFile = Path.Combine(Locations.InstalledModsPath, localFile);
                FileHelper.CreatePath(Locations.InstalledModsPath);
                FileHelper.DeleteFile(PackageFile);

                config.PackagePath = PackageFile;
                WebClient web = null;
                try
                {
                    web = new WebClient();
                    web.DownloadProgressChanged += new DownloadProgressChangedEventHandler(web_DownloadProgressChanged);
                    web.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(web_DownloadFileCompleted);
                   
                    web.DownloadFileAsync(new Uri(config.Download.Source), PackageFile, config);
                    AsyncActivity.Enqueue(web);
                    retVal = true;
                    web = null;
                }
                finally
                {
                    if (web != null)
                    {
                        web.Dispose();
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;
        public static event EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DownloadComplete;
        public static event EventHandler<System.ComponentModel.AsyncCompletedEventArgs> Downloaded;
        public static event EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DownloadFailed;



        static void web_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (DownloadProgressChanged != null)
            {
                DownloadProgressChanged(sender, e);

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        static void web_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            try
            {
                if (!e.Cancelled)
                {


                    if (DownloadComplete != null)
                    {

                        DownloadComplete(sender, e);
                        AsyncActivity.Dequeue();

                    }
                    if (e.Error != null)
                    {
                        if (DownloadFailed != null)
                        {
                            DownloadFailed(sender, e);
                        }

                    }
                    else
                    {
                        if (Downloaded != null)
                        {
                            Downloaded(sender, e);
                        }
                    }

                }
            }
            finally
            {
                WebClient me = sender as WebClient;
                if (me != null)
                {
                    me.Dispose();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static bool IsInstalled(string ID)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retval = false;
            foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
            {
                if (config.ID == ID)
                {
                    retval = true;
                    break;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retval;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static bool IsActive(string ID)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retval = false;
            foreach (ModConfiguration config in ActiveModConfigurations.Current.Configurations.Configurations)
            {
                if (config.ID == ID)
                {
                    retval = true;
                    break;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retval;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
        public struct InstallData
        {
            public ModConfiguration Configuration { get; set; }
            public string Source { get; set; }
        }

        public static bool SetupInProgress { get; private set; }

        static void ProcessInitialSetup()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (_log.IsInfoEnabled) { _log.Info("~~~~~~~~~~~~~~~~~~~~ Starting DoInitialSetup in ModManagement (for performance info) ~~~~~~~~~~~~~~~~~~"); }
            ModConfiguration config = null;
            if (!ModManagement.IsInstalled(DataStrings.StockID))
            {
                
                ModManagement.InstallComplete += new EventHandler<ProcessEventArgs>(ModManagement_InstallComplete);
                config = new ModConfiguration(Locations.MODStockDefinition);
                string artyInstallPath = UserConfiguration.Current.GetArtemisInstallPath();
                ModManagement.BeginInstall(config, artyInstallPath);

            }
            else
            {
                if (!ModManagement.IsActive(DataStrings.StockID))
                {
                    if (config == null)
                    {
                        config = new ModConfiguration(Locations.MODStockDefinition);
                    }
                    ModManagement.Activate(config);
                }
                SetupInProgress = false;
            }
            if (_log.IsInfoEnabled) { _log.Info("~~~~~~~~~~~~~~~~~~~~ Ending DoInitialSetup in ModManagement (for performance info) ~~~~~~~~~~~~~~~~~~"); }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "x")]
        public static void DoInitialSetup()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            SetupInProgress = true;
            object x = InstalledModConfigurations.Current;
            x = ActiveModConfigurations.Current;
            ProcessInitialSetup();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void ModManagement_InstallComplete(object sender, ProcessEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.InstallComplete -= new EventHandler<ProcessEventArgs>(ModManagement_InstallComplete);
            ModConfiguration config = e.Configuration;
            if (!ModManagement.IsActive(config.ID))
            {
                ModManagement.Activate(config);
            }
            SetupInProgress = false;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void DoInstallSuccess(ModConfiguration configuration)
        {
            StopStandby();
            Locations.MessageBoxShow(
                string.Format(System.Globalization.CultureInfo.CurrentCulture,
                AMLResources.Properties.Resources.Installed, configuration.Title),
                MessageBoxButton.OK, MessageBoxImage.Information);
            RaiseInstallComplete(true, configuration);
        }
        static void DoInstallFail(ModConfiguration configuration)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(AMLResources.Properties.Resources.ProblemInstalling);
            sb.Append(DataStrings.Space);
            sb.AppendLine(configuration.Title);
            sb.AppendLine();
            sb.AppendLine(AMLResources.Properties.Resources.CorruptZipMessage);
            sb.AppendLine();
            sb.AppendLine(AMLResources.Properties.Resources.ManuallyDownload);
            sb.AppendLine();
            sb.Append(AMLResources.Properties.Resources.DefaultBrowser);
            StopStandby();
            Locations.MessageBoxShow(sb.ToString(),
                MessageBoxButton.OK, MessageBoxImage.Error);
            if (!string.IsNullOrEmpty(configuration.Download.Source))
            {
                Process.Start(configuration.Download.Source);
            }
            else if (!string.IsNullOrEmpty(configuration.Download.Webpage))
            {
                Process.Start(configuration.Download.Webpage);
            }
            RaiseInstallComplete(false, configuration);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void BeginInstall(InstallData data ) //(object state) // //
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //InstallData data = (InstallData)state;
            if (ModManagement.Install(data.Configuration, data.Source))
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<ModConfiguration>(DoInstallSuccess), data.Configuration);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(new Action<ModConfiguration>(DoInstallFail), data.Configuration);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
      
        public static void DoMessage(string message)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (progress != null)
            {
                progress.UpdateMessage(message);
            }

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        public static void RaiseInstallComplete(bool result, ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                Application.Current.Dispatcher.Invoke(new Action<bool, ModConfiguration>(RaiseInstallComplete), result, config);
            }
            else
            {
                StopStandby();
                if (InstallComplete != null)
                {

                    InstallComplete(null, new ProcessEventArgs(result, config));
                }
                NotifyMissionInstall();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        public static event EventHandler<ProcessEventArgs> InstallComplete;

        public static void StopStandby()
        {
            if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                Application.Current.Dispatcher.Invoke(new Action(StopStandby));
            }
            else
            {
                if (progress != null)
                {
                    UserConfiguration.Current.IsProcessing = false;
                    progress.Close();
                }
            }
        }
        public static void StartStandby(string title)
        {
            if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                Application.Current.Dispatcher.Invoke(new Action<string>(StartStandby), title);
            }
            else
            {
                UserConfiguration.Current.IsProcessing = true;
                progress = new StandBy();
                progress.Topmost = true;
                progress.Message = AMLResources.Properties.Resources.ProcessingPleaseStandBy;
                progress.Title = title;
                progress.Show();
            }
        }
        public static void SetStandbyBack()
        {
            if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                Application.Current.Dispatcher.Invoke(new Action(SetStandbyBack));
            }
            else
            {
                if (progress != null)
                {
                    progress.Topmost = false;
                }
            }
        }
        static void EndInstall(IAsyncResult result)
        {
        }
        public static void BeginInstall(ModConfiguration config, string source)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            InstallData data = new InstallData();
            data.Configuration = config;
            data.Source = source;

            StartStandby(
                string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.IsProcessing, GeneralHelper.AssemblyTitle));

            new Action<InstallData>(BeginInstall).BeginInvoke(data, new AsyncCallback(EndInstall), null);
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(BeginInstall), data);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static StandBy progress = null;
        /// <summary>
        /// Installs the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static bool Install(ModConfiguration configuration, string source)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = InstalledModConfigurations.Current.InstallMod(configuration, source);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Mod")]
        public static bool Install(string ModPath, string source)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModConfiguration config = new ModConfiguration(ModPath);
            bool retVal = Install(config, source);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }


        public static bool Uninstall(ModConfiguration configuration)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = InstalledModConfigurations.Current.UninstallMod(configuration);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static bool Uninstall(string ID)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = InstalledModConfigurations.Current.UninstallMod(ID);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static void Activate(ModConfiguration configuration)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = true;
            if (configuration != null && configuration.DependsOn != null)
            {
                foreach (StringItem iten in configuration.DependsOn)
                {
                    if (!string.IsNullOrEmpty(iten.Text))
                    {
                        if (retVal)
                        {
                            bool found = false;
                            foreach (ModConfiguration config in ActiveModConfigurations.Current.Configurations.Configurations)
                            {

                                if (config.ID == iten.Text)
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                retVal = false;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (!retVal)
                {
                    GetDependentInstalledMod(configuration);
                    return;
                }

            }
            if (configuration != null)
            {
                StartStandby("Activating " + configuration.Title);
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(DoActivate), configuration);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
           

        }
        static void GetDependentInstalledMod(ModConfiguration configuration)
        {
            bool IsInstalled = true;
            string DependentModTitle = string.Empty;
            foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
            {
                if (!IsInstalled)
                {
                    bool found = false;
                    foreach (StringItem iten in configuration.DependsOn)
                    {
                        if (config.ID == iten.Text)
                        {
                            found = true;
                            DependentModTitle = config.Title;
                            break;
                        }
                    }
                    if (!found)
                    {
                        IsInstalled = false;
                    }
                }
                else
                {
                    break;
                }
            }
            if (IsInstalled)
            {
                Locations.MessageBoxShow(
                    string.Format(CultureInfo.CurrentCulture,
                    AMLResources.Properties.Resources.DependsOnIsInstalled, DependentModTitle),
                    MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else
            {
                ProcessPredefined(configuration);

            }
        }
        static void ProcessPredefined(ModConfiguration configuration)
        {
            bool IsPredefined = true;
            string predefinedTitle = string.Empty;
            Dictionary<string, ModConfiguration> predefinedMods = GetPredefinedMods();
            bool found = false;
            foreach (ModConfiguration config in predefinedMods.Values)
            {
                if (config.ID == configuration.ID)
                {
                    found = true;
                    predefinedTitle = config.Title;
                    break;
                }
            }
            if (!found)
            {
                IsPredefined = false;
            }
            if (IsPredefined)
            {
                Locations.MessageBoxShow(
                    string.Format(CultureInfo.CurrentCulture,
                    AMLResources.Properties.Resources.DependsOnAPredefined, predefinedTitle),
                    MessageBoxButton.OK, MessageBoxImage.Hand);
            }
            else
            {
                Locations.MessageBoxShow(
                    AMLResources.Properties.Resources.DependsOnNotInstalled,
                    MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        public static void CheckInstalledModDefinitionVersions()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool changeMade = false;
            foreach (ModConfiguration predefinedConfig in ModManagement.GetPredefinedMods().Values)
            {
                foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
                {
                    if (config.ID == predefinedConfig.ID)
                    {
                        if (config.DefinitionVersion < predefinedConfig.DefinitionVersion)
                        {
                            if (config.Download.Source != predefinedConfig.Download.Source)
                            {
                                MessageBox.Show(config.Title + " has been updated, and the source file has changed.\r\n\r\nIf the mod itself has been updated, you will need to uninstall and re-install the mod to pick up the changes.", "Mod Definition Update", MessageBoxButton.OK, MessageBoxImage.Information);
                                config.Download.Source = predefinedConfig.Download.Source;
                                config.Download.Webpage = predefinedConfig.Download.Webpage;

                            }
                            config.BaseFiles = predefinedConfig.BaseFiles;
                            config.SubMods = predefinedConfig.SubMods;
                            config.DefinitionVersion = predefinedConfig.DefinitionVersion;
                            changeMade = true;
                        }
                        break;
                    }
                }

            }
            if (changeMade)
            {
                InstalledModConfigurations.Current.Save();
            }
            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        }
        static void DoActivate(object state)
        {
            ModConfiguration configuration = state as ModConfiguration;
            ActiveModConfigurations.Current.ActivateConfiguration(configuration);
            StopStandby();
            NotifyMissionInstall();
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static decimal GetVesselDataVersion(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //Going to assume the xml file is valid.
            decimal retVal = 0;

            XmlDocument vesselData = XmlConverter.LoadXmlFile(file);
            if (vesselData != null)
            {
                try
                {

                    XmlElement vessel = vesselData.DocumentElement;
                    if (vessel.Name == "vessel_data")
                    {
                        string version = vessel.GetAttribute("version");
                        if (!decimal.TryParse(version, out retVal))
                        {
                            retVal = 0;
                        }
                    }

                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Exception encountered getting vesselData version.", ex);
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        //public static bool Activate(string ID)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    bool retVal = false;
        //    ModConfiguration c = null;
        //    foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
        //    {
        //        if (ID == config.ID)
        //        {
        //            retVal = true;
        //            c = config;
        //            break;
        //        }
        //    }
        //    if (retVal)
        //    {
        //        retVal = Activate(c);
        //    }
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    return retVal;
        //}
        public static bool DeactivateLastMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string DeactivatedID = null;
            
            if (ActiveModConfigurations.Current.Configurations.Configurations.Count > 0)
            {
                DeactivatedID = ActiveModConfigurations.Current.Configurations.Configurations[ActiveModConfigurations.Current.Configurations.Configurations.Count - 1].ID;
            }
            bool retVal = ActiveModConfigurations.Current.DeactivateLastConfig();
            if (!string.IsNullOrEmpty(DeactivatedID) && retVal)
            {
                foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
                {
                    if (config.ID == DeactivatedID)
                    {
                        config.IsActive = false;
                        break;
                    }
                }
            }
            
            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }
        private static void DeactivateLastMod(object state)
        {
            bool WasPlaying = RussLibraryAudio.AudioServer.Current.IsPlaying;
            if (WasPlaying)
            {
                RussLibraryAudio.AudioServer.Current.Stop();
            }
            StartStandby("Deactivating Last Mod");
            DeactivateLastMod();
            ModManagement.SetStandbyBack();
            NotifyMissionInstall();
            StopStandby();
            
            if (WasPlaying)
            {
                RussLibraryAudio.AudioServer.Current.PlayNextInQueue();
            }
        }
        public static void BackgroundDeactivateLastMod()
        {
            
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(DeactivateLastMod));
        }

        public static bool CheckForUpdate()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            FileInfo CurrentInstall = new FileInfo(Path.Combine(UserConfiguration.Current.GetArtemisInstallPath(), Locations.ArtemisEXE));
            FileInfo CurrentCopy = new FileInfo(Locations.ArtemisFileToRun);


            bool retVal = false;
            if (CurrentCopy.Exists && CurrentInstall.Exists)
            {
                retVal = (CurrentCopy.Length != CurrentInstall.Length || CurrentCopy.LastWriteTimeUtc.CompareTo(CurrentInstall.LastWriteTimeUtc) != 0);
            }
            if (!CurrentInstall.Exists && CurrentCopy.Exists)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(RemoveCurrentCopy));
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }
        private static void RemoveCurrentCopy()
        {
            MessageBoxResult choice = Locations.MessageBoxShow("The Artemis Spaceship Bridge Simulator installation cannot be found.\r\n\r\nIf you have uninstalled Artemis Spaceship Simulator, you should also remove the copy used by Artemis Mod Loader.\r\n\r\nDo you wish to remove the copy of Artemis Spaceship Bridge Simulator?",
                 MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice == MessageBoxResult.Yes)
            {
                FileHelper.DeleteAllFiles(Locations.ArtemisCopyPath);

            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private static void MakeCurrentStockAMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
           
            
            //ModConfiguration OldStock = new ModConfiguration(Path.Combine(Locations.MyLocation, DataStrings.ModStockFile));
            ModConfiguration OldStock = new ModConfiguration();

            foreach (ModConfiguration config in InstalledModConfigurations.Current.Configurations.Configurations)
            {
                if (config.ID == DataStrings.StockID)
                {
                    OldStock.CopyProperties(config);
                    break;
                    
                }
            }
            FileInfo f = new FileInfo(Path.Combine(OldStock.InstalledPath, Locations.ArtemisEXE));
            Dictionary<DateTime, string> artemisVersions = GetArtemisVersionDictionary();
            DateTime DateKey = new DateTime(f.LastWriteTime.Year, f.LastWriteTime.Month, f.LastWriteTime.Day);
            string verMatch = "v. 1.661";
            if (artemisVersions.ContainsKey(DateKey))
            {
                verMatch = artemisVersions[DateKey];
            }
            else
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(f.FullName);
                string pv = fvi.ProductMajorPart.ToString(CultureInfo.InvariantCulture) + fvi.ProductMinorPart.ToString(CultureInfo.InvariantCulture).PadRight(6, '0') + fvi.ProductBuildPart.ToString(CultureInfo.InvariantCulture).PadRight(6, '0');
                string fv = fvi.FileMajorPart.ToString(CultureInfo.InvariantCulture) + fvi.FileMinorPart.ToString(CultureInfo.InvariantCulture).PadRight(6, '0') + fvi.FileBuildPart.ToString(CultureInfo.InvariantCulture).PadRight(6, '0');
                int pvv = 0;
                if (!int.TryParse(pv, out pvv))
                {
                    pvv= int.MinValue;
                }
                int fvv = 0;
                if (!int.TryParse(fv, out fvv))
                {
                    fvv = int.MinValue;
                }
                if (pvv <= 0 && fvv <= 0)
                {



                    verMatch = "Old version of Artemis dated " + DateKey.ToString();
                }
                else
                {
                    if (fvv > pvv)
                    {
                        verMatch = "v. " + fvi.FileMajorPart.ToString(CultureInfo.InvariantCulture) + "." + fvi.FileMinorPart.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        verMatch = "v. " + fvi.ProductMajorPart.ToString(CultureInfo.InvariantCulture) + "." + fvi.ProductMinorPart.ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
           // string oldPath = OldStock.InstalledPath;
            OldStock.IsActive = false;
            OldStock.InstalledPath = null;

            OldStock.ID = "Artemis_" + verMatch.Replace(".", string.Empty).Replace(" ", string.Empty) + "_Stock";
            OldStock.Title += " " + verMatch;
            //~~~~~
            string source = Path.Combine(Locations.InstalledModsPath, DataStrings.StockID);

            string Target = Path.Combine(Locations.InstalledModsPath, OldStock.ID);
            OldStock.InstalledPath = Target;
            FileHelper.CopyFiles(new DirectoryInfo(source), Target);

            InstalledModConfigurations.Current.Configurations.Configurations.Add(OldStock);
            InstalledModConfigurations.Current.Save();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        public static void ProcessUpdate()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool WasPlaying = RussLibraryAudio.AudioServer.Current.IsPlaying;
            if (WasPlaying)
            {
                RussLibraryAudio.AudioServer.Current.Stop();
            }
            MakeCurrentStockAMod();
            DoReset();
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(FinalNotify));
            

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
         

       
        static void FinalNotify(object state)
        {
            while (ModManagement.SetupInProgress)
            {
                System.Threading.Thread.Sleep(100);
            }
            Application.Current.Dispatcher.BeginInvoke(new Action(FinalNotify), System.Windows.Threading.DispatcherPriority.Background);
        }
        static void FinalNotify()
        {
            

            Locations.MessageBoxShow(AMLResources.Properties.Resources.ResetComplete 
                + DataStrings.CRCR 
                + string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.RestartRequired, AMLResources.Properties.Resources.Title), 
                MessageBoxButton.OK, MessageBoxImage.Information);

            System.Diagnostics.Process.Start(Assembly.GetEntryAssembly().Location);
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Browses for package.
        /// </summary>
        /// <param name="mod">The mod.</param>
        /// <returns></returns>
        public static bool BrowseForPackage(ModConfiguration mod)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter =
                AMLResources.Properties.Resources.SupportedCompressedFiles + DataStrings.SupportedCompressedFilesFilter
                + "|" + AMLResources.Properties.Resources.AllFiles + DataStrings.AllFilesFilter;
            diag.Title = GeneralHelper.AssemblyTitle;
            bool retVal = false;
            if (diag.ShowDialog() == true)
            {
                ModManagement.BeginInstall(mod, diag.FileName);

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        /// <summary>
        /// Processes the AML file.  Can only be for a Mod.  Is only AML file--the Mod package is not available with this.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        static bool ProcessAML(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            ModConfiguration config = new ModConfiguration(file);
            if (!string.IsNullOrEmpty(config.Download.Source) || !string.IsNullOrEmpty(config.Download.Webpage))
            {
                MessageBoxResult result =
                    Locations.MessageBoxShow(AMLResources.Properties.Resources.DownloadQuestion,
                    MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Cancel)
                {
                    retVal = false;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrEmpty(config.Download.Source))
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(AMLResources.Properties.Resources.DownloadFileNotFound);
                        sb.AppendLine(AMLResources.Properties.Resources.DefaultBrowser);
                        sb.AppendLine();
                        sb.Append(AMLResources.Properties.Resources.ManuallyDownload2);
                        Locations.MessageBoxShow(sb.ToString(), MessageBoxButton.OK, MessageBoxImage.Hand);
                        System.Diagnostics.Process.Start(config.Download.Webpage);
                        //Give user time to download.
                        System.Threading.Thread.Sleep(10000);
                        retVal = BrowseForPackage(config);
                    }
                    else
                    {

                        StartDownloadWithMonitor(config);
                        retVal = false;  //False means no notification.
                    }
                }
            }
            else
            {
                retVal = BrowseForPackage(config);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        static void StartDownloadWithMonitor(ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.Downloaded += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_Downloaded);
            ModManagement.DownloadFailed += new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadFailed);

            ModManagement.StartDownload(config);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void CleanupDownload()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.Downloaded -= new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_Downloaded);
            ModManagement.DownloadFailed -= new EventHandler<System.ComponentModel.AsyncCompletedEventArgs>(ModManagement_DownloadFailed);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void ModManagement_DownloadFailed(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            CleanupDownload();
            if (_log.IsErrorEnabled)
            {
                _log.Error("Download failure:", e.Error);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(AMLResources.Properties.Resources.DownloadFailedLabel);
            sb.AppendLine();
            sb.AppendLine(e.Error.Message);
            sb.AppendLine();
            sb.AppendLine(AMLResources.Properties.Resources.ManuallyDownload2);
            sb.AppendLine();
            sb.Append(AMLResources.Properties.Resources.DefaultBrowser);
            Locations.MessageBoxShow(sb.ToString(),
                MessageBoxButton.OK, MessageBoxImage.Error);

            ModConfiguration mod = e.UserState as ModConfiguration;
            if (string.IsNullOrEmpty(mod.Download.Source))
            {
                System.Diagnostics.Process.Start(mod.Download.Source);
            }
            else
            {
                System.Diagnostics.Process.Start(mod.Download.Webpage);
            }
            ModManagement.BrowseForPackage(mod);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        static void ModManagement_Downloaded(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            CleanupDownload();
            if (e != null)
            {
                ModConfiguration mod = e.UserState as ModConfiguration;

                ModManagement.BeginInstall(mod, mod.PackagePath);

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        /// <summary>
        /// Checks if Compressed file is a mission.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        static bool CompressedFileIsMission(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //Is a mission if there is only one file named MISS_*.xml.

            //Is a mod if there are more than one file named MISS_*.xml OR if there are NO files named MISS_*.xml
            // OR if artemis.ini is found, or if "vesselData.xml" is found, or if files under "dat" (not under "missions") found.

            int missionFileCount = 0;
            bool retVal = true;
            using (Stream stream = File.OpenRead(file))
            {
                IReader reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        if (reader.Entry.FilePath.EndsWith(DataStrings.XMLExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            string fName = reader.Entry.FilePath;
                            int i = fName.LastIndexOf('\\');
                            if (i < 0)
                            {
                                i = fName.LastIndexOf('/');

                            }
                            if (i >= 0)
                            {
                                fName = fName.Substring(i + 1);
                            }
                            if (fName.StartsWith(DataStrings.MissPrefix, StringComparison.OrdinalIgnoreCase))
                            {
                                missionFileCount++;
                                if (missionFileCount > 1)
                                {
                                    retVal = false;
                                    break;
                                }
                            }


                        }
                        if (reader.Entry.FilePath.EndsWith(DataStrings.ArtemisINI, StringComparison.OrdinalIgnoreCase))
                        {
                            retVal = false;
                            break;
                        }
                        if (reader.Entry.FilePath.ToUpperInvariant().Contains("DAT\\"))
                        {
                            if (!reader.Entry.FilePath.ToUpperInvariant().Contains("DAT\\MISSIONS\\"))
                            {
                                retVal = false;
                                break;
                            }
                        }
                        if (reader.Entry.FilePath.EndsWith(DataStrings.vesselData, StringComparison.OrdinalIgnoreCase))
                        {
                            retVal = false;
                            break;
                        }
                    }
                }
                if (missionFileCount != 1)
                {
                    retVal = false;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static KeyValuePair<string, string> GetMissionName(string compressedFile)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = null;
            string path = null;
            try
            {
                using (Stream stream = File.OpenRead(compressedFile))
                {
                    IReader reader = ReaderFactory.Open(stream);
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            if (reader.Entry.FilePath.StartsWith(DataStrings.MissPrefix, StringComparison.OrdinalIgnoreCase) && reader.Entry.FilePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                            {
                                retVal = reader.Entry.FilePath.Substring(0, reader.Entry.FilePath.Length - 4);
                                if (retVal.Contains("\\"))
                                {
                                    path = retVal.Substring(0, retVal.IndexOf('\\'));
                                    retVal = retVal.Substring(retVal.LastIndexOf('\\') + 1);
                                }
                                break;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Error process compressed file:", ex);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            return new KeyValuePair<string, string>(path, retVal);
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static bool ProcessCompressedMissionFile(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            KeyValuePair<string, string> missionData = GetMissionName(file);
            string missionName = missionData.Value;
            string rootPath = missionData.Key;
            if (!string.IsNullOrEmpty(missionName))
            {
                string missionPath = System.IO.Path.Combine(Locations.ArtemisMissionPath, missionName);
                string missionFileName = missionName + DataStrings.XMLExtension;
                string completedTarget = System.IO.Path.Combine(missionPath, missionFileName);

                if (Directory.Exists(missionPath) && File.Exists(completedTarget))
                {
                    Locations.MessageBoxShow(
                        string.Format(System.Globalization.CultureInfo.CurrentCulture,
                        AMLResources.Properties.Resources.AlreadyInstalled, missionName),
                        MessageBoxButton.OK, MessageBoxImage.Hand);
                    retVal = false;
                }
                else
                {

                    FileHelper.CreatePath(missionPath);

                    using (Stream stream = File.OpenRead(file))
                    {
                        IReader reader = ReaderFactory.Open(stream);
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                string targName = reader.Entry.FilePath;
                                string ExtractTarget = targName;
                                if (!string.IsNullOrEmpty(rootPath))
                                {
                                    targName = targName.Substring(rootPath.Length + 1);
                                }
                                string target = System.IO.Path.Combine(missionPath, targName);

                                FileHelper.DeleteFile(target);
                                reader.WriteEntryToDirectory(missionPath,
                                    SharpCompress.Common.ExtractOptions.ExtractFullPath
                                    | SharpCompress.Common.ExtractOptions.Overwrite);
                                if (ExtractTarget != targName)
                                {
                                    FileInfo extractTarg = new FileInfo(System.IO.Path.Combine(missionPath, ExtractTarget));
                                    DirectoryInfo extractDir = extractTarg.Directory;
                                    extractTarg.MoveTo(target);
                                    try
                                    {
                                        extractDir.Delete();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (_log.IsWarnEnabled)
                                        {
                                            _log.Warn("Unable to delete extraneous mission directory", ex);
                                        }
                                    }
                                }
                                FileInfo f = new FileInfo(target);
                                f.LastWriteTime = reader.Entry.LastModifiedTime.Value;
                            }
                        }
                    }
                    retVal = true;
                }
                NotifyMissionInstall();
            }
            else
            {
                Locations.MessageBoxShow(AMLResources.Properties.Resources.BadMissionName,
                    MessageBoxButton.OK, MessageBoxImage.Hand);
                retVal = false;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static void NotifyMissionInstall()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.SetStandbyBack();
            if (MissionsUpdated != null)
            {
                MissionsUpdated(null, EventArgs.Empty);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        /// <summary>
        /// Processes the compressed file.  Could be either a Mission file or a Mod.  Look for Mission
        /// file in the compressed file.  If one is found, look for vesselData.xml or other indicators
        /// to determine if this is a Mod or a mission.  If more than one mission file, it is a Mod.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static bool ProcessCompressed(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            //First scan zip file for a .aml file--if found, process .aml file, no download.
            try
            {
                ModConfiguration config = GetModConfigFile(file);
                if (config == null)
                {
                    //AML file not found.  Is this a Mission or a Mod?

                    if (CompressedFileIsMission(file))
                    {
                        retVal = ProcessCompressedMissionFile(file);
                    }
                    else
                    {
                        retVal = ProcessCompressedModNoAML(file);
                    }
                }
                else
                {
                    //AML file found.
                    BeginInstall(config, file);
                    retVal = false; //False means no messagebox.
                }
            }
            catch (Exception ex)
            {
                retVal = false;
                //Error may have occurred under the "GetModConfigfile"--bad compressed file.
                if (_log.IsErrorEnabled)
                {
                    _log.Error("Error processing compressed file", ex);
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(AMLResources.Properties.Resources.CorruptZipMessage2);
                sb.AppendLine();
                sb.Append(AMLResources.Properties.Resources.ErrorLabel);
                sb.AppendLine(ex.Message);
                sb.AppendLine();
                sb.Append(AMLResources.Properties.Resources.RetryDownload);
                Locations.MessageBoxShow(sb.ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static event EventHandler MissionsUpdated;
        static bool ProcessCompressedModNoAML(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModDefinitionSetup win = new ModDefinitionSetup();
            win.Configuration.PackagePath = file;
            win.Configuration.ID = new FileInfo(file).Name.Replace('.', '~');
            if (win.ShowDialog() == true)
            {
                ModManagement.BeginInstall(win.Configuration, file);

            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return false;
        }
        /// <summary>
        /// Processes the XML.  Should only be a mission file
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        static bool ProcessXML(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            //Is XML file only.
            FileInfo f = new FileInfo(file);
            string missionName = f.Name.Substring(0, f.Name.Length - 4);
            if (missionName.StartsWith(DataStrings.MissPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string missionPath = System.IO.Path.Combine(Locations.ArtemisMissionPath, missionName);
                if (Directory.Exists(missionPath) && File.Exists(System.IO.Path.Combine(missionPath, f.Name)))
                {
                    retVal = false;
                    Locations.MessageBoxShow(string.Format(System.Globalization.CultureInfo.CurrentCulture, AMLResources.Properties.Resources.AlreadyInstalled, missionName),
                        MessageBoxButton.OK, MessageBoxImage.Hand);
                }
                else
                {
                    
                    string target = System.IO.Path.Combine(missionPath, f.Name);
                    FileHelper.Copy(f.FullName, target);
                    
                    NotifyMissionInstall();
                    retVal = true;
                }
            }
            else
            {
                retVal = false;
                StringBuilder sb = new StringBuilder();
                //Mission files must start with "{0}" and end with "{1}".
                sb.AppendLine(AMLResources.Properties.Resources.NotMissionFile);
                sb.AppendLine();
                sb.AppendLine(string.Format(System.Globalization.CultureInfo.CurrentCulture, AMLResources.Properties.Resources.MissionFileStartWith, DataStrings.MissPrefix, DataStrings.XMLExtension));

                sb.AppendLine();
                sb.AppendLine(AMLResources.Properties.Resources.CannotProcess);
                Locations.MessageBoxShow(sb.ToString(), MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }

        /// <summary>
        /// Gets the mod config file from the compressed file (if there is one).  This is an immediate
        /// indication that this is a mod, not a mission.
        /// </summary>
        /// <param name="FileToProcess">The file to process.</param>
        /// <returns></returns>
        static ModConfiguration GetModConfigFile(string FileToProcess)
        {

            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModConfiguration config = null;
            string target = null;
            using (Stream stream = File.OpenRead(FileToProcess))
            {
                IReader reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        if (reader.Entry.FilePath.EndsWith(DataStrings.DefaultAMLExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            target = System.IO.Path.Combine(System.IO.Path.GetTempPath(), reader.Entry.FilePath);

                            reader.WriteEntryToDirectory(System.IO.Path.GetTempPath(),
                                SharpCompress.Common.ExtractOptions.ExtractFullPath | SharpCompress.Common.ExtractOptions.Overwrite);
                            break;
                        }
                    }
                }
            }
            if (!string.IsNullOrEmpty(target))
            {
                config = new ModConfiguration(target);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return config;
        }
        public static void ProcessFiles(string[] files)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool success = true;

            List<string> successful = new List<string>();
            if (files != null)
            {
                foreach (string file in files)
                {
                    success = ProcessFile(file);
                    if (!success)
                    {

                    }
                    else
                    {

                        successful.Add(file);
                    }
                }
                if (success)
                {
                    if (files.Length > 0)
                    {
                        Locations.MessageBoxShow(AMLResources.Properties.Resources.AllFilesInstalled, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        Locations.MessageBoxShow(AMLResources.Properties.Resources.SuccessfullyInstalled, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static bool ProcessFile(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            if (!string.IsNullOrEmpty(file))
            {

                //Now identify file type.  If "*.aml", prompt to process from web download, or browse for package.
                if (file.EndsWith(DataStrings.DefaultAMLExtension, StringComparison.OrdinalIgnoreCase))
                {
                    retVal = ProcessAML(file);
                }
                else if (file.EndsWith(DataStrings.XMLExtension, StringComparison.OrdinalIgnoreCase))
                {
                    //Assumed to be a mission file.
                    retVal = ProcessXML(file);
                }
                else
                {
                    //Compressed file assumed.
                    retVal = ProcessCompressed(file);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static void ProcessFileWithNotify(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (ProcessFile(file))
            {
                Locations.MessageBoxShow(AMLResources.Properties.Resources.SuccessfullyInstalled, MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        /// <summary>
        /// Determines whether [is in mod path or dependency path] [the specified path].
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="config">The config.</param>
        /// <returns>Null if not in mod or dependency path, or mod's installed path if it is.</returns>
        public static string IsInModPathOrDependencyPath(string path, ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            string prefix = null;
            if (!string.IsNullOrEmpty(path) && config != null)
            {
                if (!path.StartsWith(config.InstalledPath, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (StringItem item in config.DependsOn)
                    {
                        foreach (ModConfiguration cfg in InstalledModConfigurations.Current.Configurations.Configurations)
                        {
                            if (cfg.ID == item.Text || cfg.ID == DataStrings.StockID)
                            {
                                if (path.StartsWith(cfg.InstalledPath, StringComparison.OrdinalIgnoreCase))
                                {
                                    retVal = true;
                                    prefix = cfg.InstalledPath;
                                    break;
                                }
                            }
                        }
                        if (retVal)
                        {
                            break;
                        }
                    }
                    if (!retVal)
                    {
                        if (path.StartsWith(Locations.ArtemisCopyPath, StringComparison.OrdinalIgnoreCase))
                        {
                            //Is from copy, what is active?
                            retVal = true;
                            prefix = Locations.ArtemisCopyPath;
                            foreach (ModConfiguration cfg in ActiveModConfigurations.Current.Configurations.Configurations)
                            {
                                if (cfg.ID != DataStrings.StockID)
                                {
                                    foreach (StringItem item in config.DependsOn)
                                    {
                                        if (cfg.ID != item.Text)
                                        {
                                            retVal = false;
                                            break;

                                        }
                                    }

                                }
                                if (!retVal)
                                {
                                    break;
                                }
                            }
                        }
                    }

                }
                else
                {
                    retVal = true;
                    prefix = config.InstalledPath;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return prefix;
        
        }
    }
}
