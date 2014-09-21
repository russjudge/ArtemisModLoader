using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using log4net;
using System.Net;
using SharpCompress.Reader;
using System.Reflection;
using System.Xml;
using System.Windows;
using System.Threading;
using Microsoft.Win32;
using System.Text;
using System.Globalization;
namespace ArtemisModLoader
{

    public static class ModManagement
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ModManagement));

       
        static Queue<WebClient> AsyncActivity = new Queue<WebClient>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        public static string ExtractFileName(string url)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = url;
            if (!string.IsNullOrEmpty(url))
            {
                int i = url.LastIndexOf('/');
                retVal = url.Substring(i + 1);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void StartDownload(ModConfiguration config)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (config != null)
            {
                string localFile = ExtractFileName(config.Download.Source);

                string PackageFile = Path.Combine(Locations.InstalledModsPath, localFile);
                Locations.CreatePath(Locations.InstalledModsPath);
                Locations.DeleteFile(PackageFile);
                
                config.PackagePath = PackageFile;
                WebClient web = null;
                try
                {
                    web = new WebClient();
                    web.DownloadProgressChanged += new DownloadProgressChangedEventHandler(web_DownloadProgressChanged);
                    web.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(web_DownloadFileCompleted);
                    web.DownloadFileAsync(new Uri(config.Download.Source), PackageFile, config);
                    AsyncActivity.Enqueue(web);
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
            foreach (ModConfiguration config in InstalledModConfigurations.Instance.Configurations)
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
            foreach (ModConfiguration config in ActiveModConfigurations.Instance.Configurations)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
        private struct InstallData
        {
            public ModConfiguration Configuration { get; set; }
            public string Source { get; set; }
        }

        public static bool SetupInProgress { get; private set; }
        public static void DoInitialSetup()
        {
            SetupInProgress = true;
            
            ModConfiguration config = new ModConfiguration(Locations.MODStockDefinition);
            if (!ModManagement.IsInstalled(config.ID))
            {
                ModManagement.InstallComplete += new EventHandler<ProcessEventArgs>(ModManagement_InstallComplete);
                ModManagement.BeginInstall(config, Locations.ArtemisInstallPath);
           
            }
            else
            {
                if (!ModManagement.IsActive(config.ID))
                {
                    ModManagement.Activate(config);
                }
                SetupInProgress = false;
            }
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void BeginInstall(object state)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            InstallData data = (InstallData)state;
            if (ModManagement.Install(data.Configuration, data.Source))
            {

                Locations.MessageBoxShow(string.Format(System.Globalization.CultureInfo.CurrentCulture, AMLResources.Properties.Resources.Installed, data.Configuration.Title),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                RaiseInstallComplete(true, data.Configuration);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(AMLResources.Properties.Resources.ProblemInstalling);
                sb.Append(DataStrings.Space);
                sb.AppendLine(data.Configuration.Title);
                sb.AppendLine();
                sb.AppendLine(AMLResources.Properties.Resources.CorruptZipMessage);
                sb.AppendLine();
                sb.AppendLine(AMLResources.Properties.Resources.ManuallyDownload);
                sb.AppendLine();
                sb.Append(AMLResources.Properties.Resources.DefaultBrowser);
                Locations.MessageBoxShow(sb.ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                if (!string.IsNullOrEmpty(data.Configuration.Download.Source))
                {
                    Process.Start(data.Configuration.Download.Source);
                }
                else if (!string.IsNullOrEmpty(data.Configuration.Download.Webpage))
                {
                    Process.Start(data.Configuration.Download.Webpage);
                }
                RaiseInstallComplete(false, data.Configuration);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static event EventHandler<MessageEventArgs> MessageEvent;
        public static void DoMessage(string message)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (MessageEvent != null)
            {
                MessageEvent(null, new MessageEventArgs(message)); 
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
                if (progress != null)
                {
                    progress.Close();
                }
                if (InstallComplete != null)
                {

                    InstallComplete(null, new ProcessEventArgs(result, config));
                }
                NotifyMissionInstall();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        public static event EventHandler<ProcessEventArgs> InstallComplete;
        
        public static void BeginInstall(ModConfiguration config, string source)
        {
            InstallData data = new InstallData();
            data.Configuration = config;
            data.Source = source;
            progress = new StandBy();
            progress.Message = AMLResources.Properties.Resources.ProcessingPleaseStandBy;
            progress.Show();
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(BeginInstall), data);
        }
        static StandBy progress = null;
        private static bool Install(ModConfiguration configuration, string source)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = InstalledModConfigurations.Instance.InstallMod(configuration, source);
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
            bool retVal = InstalledModConfigurations.Instance.UninstallMod(configuration);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static bool Uninstall(string ID)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = InstalledModConfigurations.Instance.UninstallMod(ID);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static bool Activate(ModConfiguration configuration)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            if (configuration != null && !string.IsNullOrEmpty(configuration.DependsOn))
            {
                foreach (ModConfiguration config in ActiveModConfigurations.Instance.Configurations)
                {
                    if (config.ID == configuration.DependsOn)
                    {
                        retVal = true;
                        break;
                    }
                }
                if (!retVal)
                {
                    bool IsInstalled = false;
                    string DependentModTitle=string.Empty;
                    foreach (ModConfiguration config in InstalledModConfigurations.Instance.Configurations)
                    {
                        if (config.ID == configuration.DependsOn)
                        {
                            IsInstalled = true;
                            DependentModTitle = config.Title;
                            break;
                        }
                    }
                    if (IsInstalled)
                    {
                        Locations.MessageBoxShow(string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.DependsOnIsInstalled, DependentModTitle), MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                    else
                    {
                        bool IsPredefined = false;
                        string predefinedTitle = string.Empty;
                        foreach (string config in PredefinedMods.PreDefinedList)
                        {
                            if (config == configuration.ID)
                            {
                                IsPredefined = true;
                                predefinedTitle = PredefinedMods.PredefinedModDictionary[config].Title;
                                break;
                            }
                        }
                        if (IsPredefined)
                        {
                            Locations.MessageBoxShow(string.Format(CultureInfo.CurrentCulture, AMLResources.Properties.Resources.DependsOnAPredefined, predefinedTitle), MessageBoxButton.OK, MessageBoxImage.Hand);
                        }
                        else
                        {
                            Locations.MessageBoxShow(AMLResources.Properties.Resources.DependsOnNotInstalled, MessageBoxButton.OK, MessageBoxImage.Hand);
                        }
                    }
                    return retVal;
                }
                
            }
            retVal = ActiveModConfigurations.Instance.ActivateConfiguration(configuration);
            NotifyMissionInstall();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static decimal GetVesselDataVersion(string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            //Going to assume the xml file is valid.
            decimal retVal = 0;
            XmlDocument vesselData = new XmlDocument();
            try
            {
                vesselData.Load(file);
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
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static bool Activate(string ID)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            ModConfiguration c = null;
            foreach (ModConfiguration config in InstalledModConfigurations.Instance.Configurations)
            {
                if (ID == config.ID)
                {
                    retVal = true;
                    c = config;
                    break;
                }
            }
            if (retVal)
            {
                retVal = Activate(c);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static bool DeactivateLastMod()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string DeactivatedID = null;
            if (ActiveModConfigurations.Instance.Configurations.Count > 0)
            {
                DeactivatedID = ActiveModConfigurations.Instance.Configurations[ActiveModConfigurations.Instance.Configurations.Count - 1].ID;
            }
            bool retVal = ActiveModConfigurations.Instance.DeactivateLastConfig();
            if (!string.IsNullOrEmpty(DeactivatedID) && retVal)
            {
                foreach (ModConfiguration config in InstalledModConfigurations.Instance.Configurations)
                {
                    if (config.ID == DeactivatedID)
                    {
                        config.IsActive = false;
                    }
                }
            }
            NotifyMissionInstall();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }


        public static bool CheckForUpdate()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            FileInfo CurrentInstall = new FileInfo(Path.Combine(Locations.ArtemisInstallPath, Locations.ArtemisEXE));
            FileInfo CurrentCopy = new FileInfo(Locations.ArtemisFileToRun);


            bool retVal = false;
            if (CurrentCopy.Exists && CurrentInstall.Exists)
            {
                retVal = (CurrentCopy.Length != CurrentInstall.Length || CurrentCopy.LastWriteTimeUtc.CompareTo(CurrentInstall.LastWriteTimeUtc) != 0);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }
        public static void ProcessUpdate()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            List<ModConfiguration> Configs = new List<ModConfiguration>();
            foreach (ModConfiguration config in ActiveModConfigurations.Instance.Configurations)
            {
                Configs.Add(config);

            }
            while (DeactivateLastMod()) { }

            ActiveModConfigurations.Instance.DeactivateStock();

            Install(Locations.MODStockDefinition, Locations.ArtemisInstallPath);

            foreach (ModConfiguration config in Configs)
            {
                Activate(config);
            }

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
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
            diag.Title = Locations.AssemblyTitle;
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
                    Locations.MessageBoxShow(string.Format(System.Globalization.CultureInfo.CurrentCulture, AMLResources.Properties.Resources.AlreadyInstalled, missionName),
                        MessageBoxButton.OK, MessageBoxImage.Hand);
                    retVal = false;
                }
                else
                {

                    Locations.CreatePath(missionPath);

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

                                Locations.DeleteFile(target);
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
        static void NotifyMissionInstall()
        {
            if (MissionsUpdated != null)
            {
                MissionsUpdated(null, EventArgs.Empty);
            }
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
                    Locations.CreatePath(missionPath);
                    string target = System.IO.Path.Combine(missionPath, f.Name);
                    f.CopyTo(target);
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
    }
}
