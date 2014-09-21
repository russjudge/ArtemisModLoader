using System;
using System.IO;
using Microsoft.Win32;
using System.Reflection;
using System.Collections.Generic;
using log4net;
using System.Windows;
using System.Text;
using RussLibrary.Helpers;

namespace ArtemisModLoader
{
    public static class Locations
    {
        public static Window MainWindow { get; set; }

        static readonly ILog _log = LogManager.GetLogger(typeof(Locations));
        public static string ConfigFile
        {
            get
            {
                return Path.Combine(Locations.DataPath, "config.ini");
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "x"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static Locations()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            object x = UserConfiguration.Current;
   
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string FindArtemisInstallPath()
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
                    Assembly assm = Assembly.GetEntryAssembly();

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
     

        //public static void SetArtemisInstallPath(string path)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    UserConfiguration.Current.ArtemisInstallPath = path;
         
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //}
     
       
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "EXE")]
        public const string ArtemisEXE = "Artemis.exe";
        /// <summary>
        /// Gets the artemis file to run.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ToRun")]
        public static string ArtemisFileToRun
        {
            get
            {

                string retVal = null;

                if (UserConfiguration.Current.UseArtemisExtender)
                {
                    retVal = UserConfiguration.Current.ArtemisExtenderCopy;
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
       
        public static MessageBoxResult MessageBoxShow(string message, MessageBoxButton button, MessageBoxImage image)
        {
            if (Application.Current.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                return (MessageBoxResult)Application.Current.Dispatcher.Invoke(new Func<string, MessageBoxButton, MessageBoxImage, MessageBoxResult>(MessageBoxShow), message, button, image);
            }
            else
            {
                ModManagement.SetStandbyBack();
                return MessageBox.Show(MainWindow, message, GeneralHelper.AssemblyTitle, button, image);
            }
        }

       


    }
}
