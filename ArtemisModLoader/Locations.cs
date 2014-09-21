using System;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using System.Collections.Generic;
using log4net;
using System.Windows;
using System.Text;

namespace ArtemisModLoader
{
    public static class Locations
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Locations));
        public static string ConfigFile
        {
            get
            {
                return Path.Combine(Locations.MyLocation, "config.ini");
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Locations()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            _ArtemisInstallPath = FindArtemisInstallPath();
            LoadConfiguration();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static string FindArtemisInstallPath()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = string.Empty;
            try
            {
                string[] RegistryPath = DataStrings.RegistryInstallLocation.Split('\\');

                RegistryKey wrkKey = Registry.LocalMachine;
                for (int i = 1; i < RegistryPath.Length; i++)
                {
                    wrkKey = wrkKey.OpenSubKey(RegistryPath[i]);
                }
                retVal = wrkKey.GetValue(string.Empty) as string;
                FileInfo f = new FileInfo(retVal);
                if (f.Exists)
                {
                    retVal = f.DirectoryName;
                }
                else
                {
                    f = new FileInfo(@"C:\Program Files\Artemis\Artemis.exe");
                    if (f.Exists)
                    {
                        retVal = f.DirectoryName;
                    }
                    else
                    {
                        f = new FileInfo(@"C:\Program Files (x86)\Aretmis\Artemis.exe");
                        if (f.Exists)
                        {
                            retVal = f.DirectoryName;
                        }
                        else
                        {
                            retVal = string.Empty;
                        }
                    }
                    
                }
            }
            catch
            {
                retVal = string.Empty;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        static void LoadConfiguration()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (File.Exists(ConfigFile))
            {
                using (StreamReader sr = new StreamReader(ConfigFile))
                {
                    string sLine = sr.ReadLine();
                    _ArtemisInstallPath = sLine;
                    if (!string.IsNullOrEmpty(_ArtemisInstallPath))
                    {
                        if (!File.Exists(Path.Combine(_ArtemisInstallPath, ArtemisEXE)))
                        {
                            _ArtemisInstallPath = FindArtemisInstallPath();
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
                                _useArtemisExtender = b;
                            }
                        }
                        if (sLine != null)
                        {
                            sLine = sr.ReadLine();
                            _artemisExtenderPath = sLine;
                        }
                    }

                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        static void SaveConfiguration()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            using (StreamWriter sw = new StreamWriter(ConfigFile))
            {
                sw.WriteLine(_ArtemisInstallPath);
                sw.WriteLine(UseArtemisExtender.ToString());
                sw.WriteLine(ArtemisExtenderPath);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        //static readonly ILog _log = LogManager.GetLogger(typeof(Locations));
        /// <summary>
        /// Path for Application data for Atermis Mod Loader
        /// </summary>
        public static string DataPath
        {
            get
            {
                string retVal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Russ Judge", "ArtemisModLoader");
                return retVal;
            }
        }
        static bool _useArtemisExtender;
        public static bool UseArtemisExtender
        {
            get { return _useArtemisExtender; }
            set
            {
                _useArtemisExtender = value;
                SaveConfiguration();
            }
        }

        public static string ArtemisExtenderConfig
        {
            get
            {
                string retVal = string.Empty;
                if (!string.IsNullOrEmpty(ArtemisExtenderPath))
                {
                    retVal = Path.Combine(new FileInfo(ArtemisExtenderPath).DirectoryName, "ArtemisPath.cfg");
                }
                return retVal;
            }
        }

        static string _artemisExtenderPath;
        public static string ArtemisExtenderPath 
        {
            get
            {
                return _artemisExtenderPath;
            }
            set
            {

                if (!string.IsNullOrEmpty(value) && File.Exists(value))
                {
                    _artemisExtenderPath = value;
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(ArtemisCopyPath);
                    if (File.Exists(ArtemisExtenderConfig))
                    {
                        using (StreamReader sr = new StreamReader(ArtemisExtenderConfig))
                        {
                            sb.AppendLine(sr.ReadToEnd());
                        }

                    }
                    using (StreamWriter sw = new StreamWriter(ArtemisExtenderConfig))
                    {
                        sw.Write(sb.ToString());
                    }
                }
                else
                {
                    if (File.Exists(ArtemisExtenderConfig))
                    {
                        StringBuilder sb = new StringBuilder();
                        bool writeRequired = false;
                        using (StreamReader sr = new StreamReader(ArtemisExtenderConfig))
                        {
                            string sLine = sr.ReadLine();
                            while (sLine != null)
                            {
                                if (sLine.ToUpperInvariant() != ArtemisCopyPath.ToUpperInvariant())
                                {
                                    sb.AppendLine(sLine);
                                }
                                else
                                {
                                    writeRequired = true;
                                }
                                sLine = sr.ReadLine();
                            }
                        }
                        if (writeRequired)
                        {
                            using (StreamWriter sr = new StreamWriter(ArtemisExtenderConfig))
                            {
                                sr.WriteLine(sb.ToString());
                            }
                        }
                        
                    }
                    _artemisExtenderPath = value;
                    
                }
                SaveConfiguration();
            }
        }
        /// <summary>
        /// Path where the Copy of Artemis is stored.
        /// </summary>
        public static string ArtemisCopyPath
        {
            get
            {
                string retVal = Path.Combine(DataPath, "ArtemisCopy");
               
                return retVal;
            }
        }
        public static string ArtemisMissionPath
        {
            get
            {
                string retVal = Path.Combine(ArtemisCopyPath, "dat", "Missions");
                return retVal;
            }
        }

        public static string IPAddressListFile
        {
            get
            {
                return Path.Combine(DataPath, "IPAddressList.dat");
            }
        }
        public static string[] GetIPList()
        {
            List<string> retVal = new List<string>();
            if (File.Exists(IPAddressListFile))
            {
                using (StreamReader sr = new StreamReader(IPAddressListFile))
                {
                    string sLine = sr.ReadLine();
                    while (sLine != null)
                    {
                        retVal.Add(sLine);
                        sLine = sr.ReadLine();
                    }
                }
            }
            return retVal.ToArray();

        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IP")]
        public static void AddToIPList(string IPAddress)
        {
            List<string> ipAddresses = new List<string>(GetIPList());
            if (!ipAddresses.Contains(IPAddress))
            {
                using (StreamWriter sw = new StreamWriter(IPAddressListFile, true))
                {
                    sw.WriteLine(IPAddress);
                }
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string GetCurrentIPAddress()
        {
            string retVal = null;
            try
            {
                string[] RegistryPath = DataStrings.RegistryIPAddressLocation.Split('\\');

                RegistryKey wrkKey = Registry.CurrentUser;
                for (int i = 1; i < RegistryPath.Length; i++)
                {
                    wrkKey = wrkKey.OpenSubKey(RegistryPath[i]);
                }
                retVal = wrkKey.GetValue(DataStrings.RegistryIPAddressLocationValue) as string;
                if (!string.IsNullOrEmpty(retVal))
                {
                    AddToIPList(retVal);
                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Error getting current IP Address:", ex);
                }
            }
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static bool SetIPAddress(string ipAddress)
        {
            bool retVal = false;
            try
            {
                string[] RegistryPath = DataStrings.RegistryIPAddressLocation.Split('\\');

                RegistryKey wrkKey = Registry.CurrentUser;
                for (int i = 1; i < RegistryPath.Length - 1; i++)
                {
                    wrkKey = wrkKey.OpenSubKey(RegistryPath[i]);
                }
                wrkKey = wrkKey.OpenSubKey(RegistryPath[RegistryPath.Length - 1], true);
                wrkKey.SetValue(DataStrings.RegistryIPAddressLocationValue, ipAddress);
                retVal = true;
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Error setting IP address in registry", ex);
                }
            }
            return retVal;
                
        }
        /// <summary>
        /// Path to the Installed Mods.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public static string InstalledModsPath
        {
            get
            {
                string retVal = Path.Combine(DataPath, "InstalledMods");
                return retVal;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "MOD")]
        public static string MODStockDefinition
        {
            get
            {
                return Path.Combine(Locations.PredefinedModsPath, "MOD_Stock.aml");
            }
        }
        
        /// <summary>
        /// Path to INI file that lists the installed Mods.
        /// </summary>
        public static string InstalledModDefinitionFile
        {
            get
            {
                string retVal = Path.Combine(InstalledModsPath, "InstalledMods.ini");
                return retVal;
            }
        }
        /// <summary>
      

       
        /// <summary>
        /// Gets my location. (Location of Artemis Mod Loader executable).
        /// </summary>
        public static string MyLocation
        {
            get
            {
                if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
                string retVal = string.Empty;
                try
                {
                    //Is null in unit tests.
                    Assembly assm = Assembly.GetExecutingAssembly();

                    FileInfo f = new FileInfo(assm.Location);
                    retVal = f.DirectoryName;
                }
                catch (NullReferenceException)
                {
                    
                }
                if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
                return retVal;
            }
        }
        /// <summary>
        /// Path to the pre-defined Mods.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public static string PredefinedModsPath
        {
            get
            {
                string retVal = Path.Combine(MyLocation, "ModDefinitions");
                return retVal;
            }
        }
        //INI:
        //Name=Defintion.xml

        /// <summary>
        /// File that tracks the list of currently active Mods.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public static string ActiveModsFile
        {
            get
            {
                string retVAl = Path.Combine(InstalledModsPath, "ActiveModList.xml");
                return retVAl;
            }
        }
        //INI:
        //<ActiveModList>
        //  <ModConfiguration Id="xxx" Sequence="1">
        //    <FileMap Target="relativetargetfile" Source="relativesource" />

        //To remove an active mod, delete all files in mod, then go through all active mods (in reverse order) and replace any missing files.


        /// <summary>
        /// Creates the path and all parent paths if they do not exist.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void CreatePath(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            DirectoryInfo d = new DirectoryInfo(path);

            if (!d.Parent.Exists)
            {
                CreatePath(d.Parent.FullName);
            }
            if (!d.Exists)
            {
                d.Create();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static void SetArtemisInstallPath(string path)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            _ArtemisInstallPath = path;
            SaveConfiguration();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        static string _ArtemisInstallPath = string.Empty;

        /// <summary>
        /// Gets the artemis install path from the registry.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string ArtemisInstallPath
        {
            get
            {
                return _ArtemisInstallPath;   
            }
        }
        /// <summary>
        /// Gets the current artemis install version.
        /// Not going to work, since Artemis programmer does not use version information.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string CurrentArtemisInstallVersion
        {
            get
            {

                string retVal = string.Empty;
                if (!string.IsNullOrEmpty(ArtemisInstallPath))
                {
                    string filename = Path.Combine(ArtemisInstallPath, ArtemisEXE);
                    if (File.Exists(filename))
                    {
                        System.AppDomain newdomain = System.AppDomain.CreateDomain("Artemis Mod Loader Work");
                        try
                        {

                            Assembly assm = newdomain.Load(File.ReadAllBytes(filename));
                            retVal = assm.GetName().Version.ToString();

                        }
                        catch { }
                        finally
                        {
                            System.AppDomain.Unload(newdomain);
                        }
                    }
                }
                return retVal;
            }
        }
        /// <summary>
        /// Gets the artemis copy install version.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string ArtemisCopyInstallVersion
        {
            get
            {
                string retVal = string.Empty;

                if (File.Exists(ArtemisFileToRun))
                {
                    System.AppDomain newdomain = System.AppDomain.CreateDomain("Artemis Mod Loader Work");
                    try
                    {

                        Assembly assm = newdomain.Load(File.ReadAllBytes(ArtemisFileToRun));
                        retVal = assm.GetName().Version.ToString();

                    }
                    catch (Exception Exception)
                    {
                        if (_log.IsWarnEnabled)
                        {
                            _log.Warn("Exception getting ArtemisCopyInstallVersion", Exception);
                        }
                    }
                    finally
                    {
                        System.AppDomain.Unload(newdomain);
                    }
                }

                return retVal;
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "EXE")]
        public const string ArtemisEXE = "Artemis.exe";
        /// <summary>
        /// Gets the artemis file to run.
        /// </summary>
        public static string ArtemisFileToRun
        {
            get
            {

                string retVal = null;

                if (UseArtemisExtender && !string.IsNullOrEmpty(ArtemisExtenderPath) && File.Exists(ArtemisExtenderPath))
                {
                    retVal = ArtemisExtenderPath;
                }
                else
                {
#if NOARTEMIS
                    retVal = @"c:\windows\system32\notepad.exe";
#else
                    retVal = Path.Combine(ArtemisCopyPath, ArtemisEXE);
#endif
                }
                return retVal;
            }
        }
        public static bool ReadyForRun
        {
            get
            {
                return File.Exists(ArtemisFileToRun);
            }
        }
        /// <summary>
        /// Copies all files down the directory tree, starting with the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="targetPath">The target path.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static List<KeyValuePair<string,string>> CopyFiles(DirectoryInfo source, string targetPath)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<KeyValuePair<string, string>> retVal = CopyFiles(source, targetPath, "*.*");

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static bool GeneralResult { get; set; }
        /// <summary>
        /// Deletes the file. (removes any read-only attribute to allow for delete).
        /// </summary>
        /// <param name="target">The target.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void DeleteFile(string target)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (File.Exists(target))
            {
                try
                {
                    FileAttributes attr = File.GetAttributes(target);

                    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attr = attr & ~FileAttributes.ReadOnly;
                        File.SetAttributes(target, attr);
                        if (_log.IsInfoEnabled)
                        {
                            _log.InfoFormat("Removing readonly attribute from \"{0}\"", target);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Error turning off readonly attribute", ex);
                    }
                }
                if (_log.IsInfoEnabled)
                {
                    _log.InfoFormat("Deleting \"{0}\"", target);
                }
                try
                {
                    File.Delete(target);
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Unable to delete " + target, ex);
                    }
                    GeneralResult = false;
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        public static void DeleteAllFiles(string target)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (Directory.Exists(target))
            {
                foreach (FileInfo f in new DirectoryInfo(target).GetFiles("*.*", SearchOption.AllDirectories))
                {
                    Locations.DeleteFile(f.FullName);

                }
                DeleteDirectoryTree(target);
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void DeleteDirectoryTree(string target)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            DirectoryInfo dir = new DirectoryInfo(target);

            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                DeleteDirectoryTree(d.FullName);
            }
            if (_log.IsInfoEnabled)
            {
                _log.InfoFormat("Deleting \"{0}\"", dir.FullName);
            }
            try
            {
                dir.Delete();
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Exception deleting directory " + dir.FullName, ex);
                }
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        /// <summary>
        /// Copies the files.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static List<KeyValuePair<string, string>> CopyFiles(DirectoryInfo source, string targetPath, string searchPattern)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<KeyValuePair<string, string>> targets = new List<KeyValuePair<string, string>>();
            if (source != null)
            {
                string root = null;
                foreach (FileInfo f in source.GetFiles(searchPattern, SearchOption.AllDirectories))
                {
                    if (root == null)
                    {
                        root = source.FullName;
                    }
                    string relative = f.DirectoryName.Substring(root.Length);
                    while (relative.StartsWith("\\", StringComparison.OrdinalIgnoreCase))
                    {
                        relative = relative.Substring(1);
                    }
                    while (relative.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
                    {
                        relative = relative.Substring(0, relative.Length - 1);
                    }
                    string target = Path.Combine(targetPath, relative);
                    CreatePath(target);
                    target = Path.Combine(target, f.Name);
                    Locations.DeleteFile(target);
                    targets.Add(new KeyValuePair<string, string>(f.FullName, target));
                    if (_log.IsInfoEnabled)
                    {
                        _log.InfoFormat("Copying \"{0}\" to \"{1}\"", f.FullName, target);
                    }
                    f.CopyTo(target);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return targets;

        }
        public static MessageBoxResult MessageBoxShow(string message, MessageBoxButton button, MessageBoxImage image)
        {
            if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                return (MessageBoxResult)Application.Current.Dispatcher.Invoke(new Func<string, MessageBoxButton, MessageBoxImage, MessageBoxResult>(MessageBoxShow), message, button, image);
            }
            else
            {
                return MessageBox.Show(message, Locations.AssemblyTitle, button, image);
            }
        }

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
            }
        }
        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
    }
}
