﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.WPF;
using RussLibrary.Xml;
using RussLibrary;
using System.Windows;
using System.Reflection;
using System.Xml;
using System.IO;
using SharpCompress.Reader;
using System.Diagnostics;
using log4net;
using RussLibrary.Helpers;

namespace ArtemisModLoader.Xml
{
     [XmlConversionRoot("InstalledModConfigurations")]
    public class InstalledModConfigurations : DependencyObject
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(InstalledModConfigurations));
       
        
        
        public static InstalledModConfigurations Current
        {
            get; private set;
        }
            


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static InstalledModConfigurations()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (_log.IsInfoEnabled) { _log.Info("~~~~~~~~~~~~~~~~~~~~ Starting setup of InstalledModConfigurations (for performance info) ~~~~~~~~~~~~~~~~~~"); }
            ResetInstallation();
            if (_log.IsInfoEnabled) { _log.Info("~~~~~~~~~~~~~~~~~~~~ Ending setup of InstalledModConfigurations (for performance info) ~~~~~~~~~~~~~~~~~~"); }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        }
       
        public static void ResetInstallation()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            FileHelper.CreatePath(Locations.InstalledModsPath);
            Current = new InstalledModConfigurations();
            if (File.Exists(Locations.InstalledModDefinitionFile))
            {


                XmlConverter.ToObject(Locations.InstalledModDefinitionFile, Current);
            }
            foreach (ModConfiguration config in Current.Configurations.Configurations)
            {
                config.IsActive = ModManagement.IsActive(config.ID);
                config.AcceptChanges();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        private InstalledModConfigurations()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Configurations = new ConfigurationGroup();
            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "InstallEXE"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private static void InstallEXE(ModConfiguration config, string file)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (string.IsNullOrEmpty(file) || !file.EndsWith(".EXE", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Must be an EXE file to call InstallEXE Method.");
            }
            if (config != null)
            {

                Locations.MessageBoxShow(
                    AMLResources.Properties.Resources.EXEInstallWarning
                    + DataStrings.CRCR
                    + AMLResources.Properties.Resources.FullRestoreWarning,
                    MessageBoxButton.OK, MessageBoxImage.Information);
                //First get list of files
                ModManagement.DoMessage(AMLResources.Properties.Resources.LoadingListOfFiles);
#if EXETest
                string testPath = @"D:\Stuff\Downloads\Artemis backup";
                FileInfo[] PreInstall = new DirectoryInfo(testPath).GetFiles("*.*", SearchOption.AllDirectories);
#else
                FileInfo[] PreInstall = new DirectoryInfo(UserConfiguration.Current.GetArtemisInstallPath()).GetFiles("*.*", SearchOption.AllDirectories);
                ModManagement.DoMessage(AMLResources.Properties.Resources.InstallingModMessage);

                RunAdminProcess(file, string.Empty);
#endif
                ModManagement.DoMessage(AMLResources.Properties.Resources.LoadingListAddedByMod);
                FileInfo[] PostInstall = new DirectoryInfo(UserConfiguration.Current.GetArtemisInstallPath()).GetFiles("*.*", SearchOption.AllDirectories);
                if (VersionOK(UserConfiguration.Current.GetArtemisInstallPath()))
                {
                    //Now identify what changed, and copy changed files to work path.
                    string wrkPath = Path.Combine(Locations.InstalledModsPath, config.ID);
                    if (_log.IsInfoEnabled)
                    {
                        _log.InfoFormat("Work Path set to \"{0}\"", wrkPath);
                    }
                    Dictionary<string, FileInfo> PreInstallFiles = new Dictionary<string, FileInfo>();
                    foreach (FileInfo f in PreInstall)
                    {
                        string fn = f.FullName;
#if EXETest
                        fn = fn.Replace(testPath, Locations.ArtemisInstallPath);
#endif
                        PreInstallFiles.Add(fn, f);
                    }
                    ModManagement.DoMessage(AMLResources.Properties.Resources.CopyingToModInstallation);
                    foreach (FileInfo f in PostInstall)
                    {
                        if (_log.IsInfoEnabled)
                        {
                            _log.InfoFormat("Post install file: {0}", f.FullName);
                        }
                        string relative = f.DirectoryName.Substring(UserConfiguration.Current.GetArtemisInstallPath().Length);
                        if (relative.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
                        {
                            relative = relative.Substring(0, relative.Length - 1);
                        }
                        if (relative.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
                        {
                            relative = relative.Substring(1, relative.Length - 1);
                        }
                        string folder = Path.Combine(wrkPath, relative);
                        if (_log.IsInfoEnabled)
                        {
                            _log.InfoFormat("Relative path = \"{1}\".  Target folder set to \"{0}\"", folder, relative);
                        }

                        if (PreInstallFiles.ContainsKey(f.FullName))
                        {
                            if (_log.IsInfoEnabled)
                            {
                                _log.Info("File found amoung preinstall.");
                            }
                            if (f.Length != PreInstallFiles[f.FullName].Length || f.LastWriteTimeUtc.CompareTo(PreInstallFiles[f.FullName].LastWriteTimeUtc) != 0)
                            {
                                if (_log.IsInfoEnabled)
                                {
                                    _log.Info("File size or last modified date not match.");
                                }
                                DoCopy(folder, f);


                            }
                            else
                            {
                                if (_log.IsInfoEnabled)
                                {
                                    _log.Info("File matches perfectly.");
                                }
                            }
                        }
                        else
                        {
                            if (_log.IsInfoEnabled)
                            {
                                _log.Info("File was not found among pre-installed files.");
                            }
                            DoCopy(folder, f);


                        }

                    }
                }
#if !EXETest
                if (!string.IsNullOrEmpty(config.Uninstall))
                {
                    ModManagement.DoMessage(AMLResources.Properties.Resources.UninstallingEXEMod);

                    RunAdminProcess(Path.Combine(UserConfiguration.Current.GetArtemisInstallPath(), config.Uninstall), string.Empty);
                    System.Threading.Thread.Sleep(10000);
                }
                else
                {
                    Locations.MessageBoxShow(
                        AMLResources.Properties.Resources.PleaseRunUninstaller,
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                ModManagement.DoMessage(AMLResources.Properties.Resources.RestoringOriginalArtemis);
                // config.InstallMod(wrkPath);
                ModConfiguration stock = new ModConfiguration(Locations.MODStockDefinition);
                string StockPath = Path.Combine(Locations.InstalledModsPath, stock.ID);


                string xcopy = Path.Combine(Path.GetTempPath(), DataStrings.RestoreArtemisCmd);
                using (StreamWriter sw = new StreamWriter(xcopy))
                {
                    sw.WriteLine(string.Format(DataStrings.XcopyCommand, StockPath, UserConfiguration.Current.GetArtemisInstallPath()));
                }
                RunAdminProcess(xcopy, string.Empty);
#endif
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static void DoCopy(string folder, FileInfo f)
        {
            
            string targ = Path.Combine(folder, f.Name);
            
            ModManagement.DoMessage(string.Format(System.Globalization.CultureInfo.CurrentCulture, AMLResources.Properties.Resources.Copying, f.Name, targ));

            FileHelper.Copy(f.FullName, targ);
            
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        static void RunAdminProcess(string file, string arguments)
        {
            ProcessStartInfo strt2 = new ProcessStartInfo(file, arguments);
            //Must be run as administrator.
            if (System.Environment.OSVersion.Version.Major >= 6)
            {
                strt2.Verb = DataStrings.AdminVerb;
            }
            Process P2 = Process.Start(strt2);
            P2.WaitForExit();
        }
        public bool ModAlreadyInstalled(string id)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            if (Configurations != null)
            {
                foreach (ModConfiguration config in Configurations.Configurations)
                {
                    if (config.ID == id)
                    {
                        retVal = true;
                        break;
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static string[] GetAllXmlFiles(string source)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<string> retVal = new List<string>();
            FileAttributes attr = File.GetAttributes(source);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                foreach (FileInfo f in new DirectoryInfo(source).GetFiles("*" + DataStrings.XMLExtension, SearchOption.AllDirectories))
                {
                    retVal.Add(f.FullName);
                }
            }
            else if (source.EndsWith("EXE", StringComparison.OrdinalIgnoreCase))
            {
            }
            else
            {
                try
                {
                    using (Stream stream = File.OpenRead(source))
                    {
                        IReader reader = ReaderFactory.Open(stream);
                        while (reader.MoveToNextEntry())
                        {
                            if (!reader.Entry.IsDirectory)
                            {
                                if (reader.Entry.FilePath.EndsWith(DataStrings.XMLExtension, StringComparison.OrdinalIgnoreCase))
                                {
                                    string target = Path.Combine(Path.GetTempPath(), reader.Entry.FilePath);
                                    FileHelper.DeleteFile(target);
                                    if (_log.IsInfoEnabled)
                                    {
                                        _log.InfoFormat("Checking {0}, writing to {1}", reader.Entry.FilePath, target);
                                    }
                                    retVal.Add(target);
                                    reader.WriteEntryToDirectory(Path.GetTempPath(),
                                        SharpCompress.Common.ExtractOptions.ExtractFullPath |
                                        SharpCompress.Common.ExtractOptions.Overwrite);
                                    //reader.WriteEntryToFile(target,
                                    //    SharpCompress.Common.ExtractOptions.ExtractFullPath | SharpCompress.Common.ExtractOptions.Overwrite);
                                    FileInfo f = new FileInfo(target);
                                    f.LastWriteTime = reader.Entry.LastModifiedTime.Value;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    retVal = null;
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Error unpacking " + source + ".", ex);
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (retVal != null)
            {

                return retVal.ToArray();
            }
            else
            {
                return null;
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "vesselData")]
        static bool VersionOK(string source)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool AtLeastOne = true;
            bool retVal = true;
            string[] xmlFiles = GetAllXmlFiles(source);
            if (xmlFiles == null)
            {
                retVal = false;

            }
            else
            {
                foreach (string s in xmlFiles)
                {
                    AtLeastOne = false;
                    decimal ver = ModManagement.GetVesselDataVersion(s);
                    if (ver >= 1.56M)
                    {
                        AtLeastOne = true;

                        break;
                    }
                }
                if (!AtLeastOne)
                {
                    retVal = (Locations.MessageBoxShow(
                        AMLResources.Properties.Resources.IncompatibleVesselDataMessage
                        + DataStrings.CRCR
                        + AMLResources.Properties.Resources.InstallAnyhowQuestion,
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes);

                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Unpackaging")]
        public bool InstallMod(ModConfiguration mod, string source)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = false;
            if (mod != null && !string.IsNullOrEmpty(source))
            {
                if (ModAlreadyInstalled(mod.ID))
                {
                    retVal = false;
                }
                else
                {

                    if (!VersionOK(source))
                    {
                        return false;
                    }
                    FileHelper.CreatePath(Locations.ArtemisCopyPath);
                    string src = source;
                    FileAttributes attr = File.GetAttributes(src);

                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        ModManagement.DoMessage(AMLResources.Properties.Resources.InstallingMod);
                        mod.InstallMod(src);

                    }
                    else if (src.EndsWith("EXE", StringComparison.OrdinalIgnoreCase))
                    {
                        ModManagement.DoMessage(AMLResources.Properties.Resources.InstallingMod);
                        InstallEXE(mod, src);
                    }
                    else
                    {
                        ModManagement.DoMessage(AMLResources.Properties.Resources.UnpackagingMod);
                        src = mod.Unpackage(source);
                        if (string.IsNullOrEmpty(src))
                        {
                            return false;
                        }

                    }
                    this.Dispatcher.BeginInvoke(new Action<ModConfiguration>(AddToCollection), mod);

                    retVal = true;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        void AddToCollection(ModConfiguration mod)
        {
            Configurations.Configurations.Add(mod);
            this.Save();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public bool UninstallMod(string ID)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModConfiguration item = null;
            bool retVal = false;
            foreach (ModConfiguration config in Configurations.Configurations)
            {
                if (config.ID == ID)
                {
                    item = config;
                    break;

                }
            }
            if (item != null)
            {
                retVal = UninstallMod(item);
            }
            else
            {
                retVal = false;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public bool UninstallMod(ModConfiguration mod)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            bool retVal = true;
            if (mod != null)
            {
                if (Configurations.Configurations.Contains(mod))
                {
                    if (mod.UninstallMod())
                    {
                        Configurations.Configurations.Remove(mod);
                        Save();
                    }
                    else
                    {
                        retVal = false;
                    }
                }
                else
                {
                    retVal = false;
                }
            }
            else
            {
                retVal = false;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;

        }
        public void Save()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            XmlDocument doc = XmlConverter.ToXmlDocument(this, true);
            doc.Save(Locations.InstalledModDefinitionFile);
            foreach (ModConfiguration config in Configurations.Configurations)
            {
                config.AcceptChanges();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static readonly DependencyProperty ConfigurationsProperty =
           DependencyProperty.Register("Configurations", typeof(ConfigurationGroup),
           typeof(InstalledModConfigurations));
          [XmlConversion("Configurations")]
        public ConfigurationGroup Configurations
        {
            get
            {
                return (ConfigurationGroup)this.UIThreadGetValue(ConfigurationsProperty);

            }
            private set
            {
                this.UIThreadSetValue(ConfigurationsProperty, value);

            }
        }
    }
 }
