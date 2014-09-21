using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace MissionStudio.Helpers
{
    internal static class Locations
    {
        public static string DataPath
        {
            get
            {
                string retVal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Russ Judge", "ArtemisModLoader");
                return retVal;
            }
        }

        public static string MissionPath
        {
            get
            {
                return Path.Combine(DataPath, "MissionData", "Templates");
            }
        }
        static Locations()
        {
            ArtemisInstallPath = FindArtemisInstallPath();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string FindArtemisInstallPath()
        {
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string retVal = string.Empty;
            try
            {
                string[] RegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\windows\CurrentVersion\App Paths\Artemis.exe".Split('\\');

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
            //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return retVal;
        }
        public static string ArtemisInstallPath
        {
            get;
            set;
        }
       
    }
}
