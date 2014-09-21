using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Management;
using System.Windows.Input;
using System.Net;
using System.IO;
using System.Windows;
using log4net.Appender;
using System.Windows.Threading;
using System.Threading;

namespace RussLibrary.Helpers
{

    public static class GeneralHelper
    {
        
       
        static readonly ILog _log = LogManager.GetLogger(typeof(GeneralHelper));

        public static void DoInvoke(Delegate delegateItem, params object[] parameter)
        {
            DispatcherOperation op = 
                Dispatcher.CurrentDispatcher.BeginInvoke(delegateItem,
                DispatcherPriority.Background, parameter);


            while (op.Status != DispatcherOperationStatus.Completed)
            {
                System.Windows.Forms.Application.DoEvents();
                //DoEvents();
            } 
        }
        private static void DoEvents()
        {
            DispatcherFrame f = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = true;
            }, f);
            Dispatcher.PushFrame(f);
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e")]
        public static void ShowApplicationError(Exception e)
        {
            if (_log.IsFatalEnabled)
            {
                _log.Fatal("Error caught", e);
                GeneralHelper.LogSystemData();
                GeneralHelper.FlushLogFile();
            }
            
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("We have blown up with the following information.");

            sb.AppendLine();
            sb.AppendFormat("If this problem persists, email the log file located at \"{0}\" to the programmer at {1}\r\n\r\n",
                GeneralHelper.LastLogFile, RussLibrary.Properties.Resources.MyEmail);
            if (e != null)
            {
                sb.AppendLine("Error: ");
                sb.Append(e.Message);
               
            }
            sb.AppendLine();
            sb.AppendLine("Would you like to open Explorer to this location?");
            if (MessageBox.Show(sb.ToString(), GeneralHelper.AssemblyTitle, MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("Explorer.exe", GeneralHelper.ApplicationDataPath);
                System.Diagnostics.Process.Start(startInfo);
            }

        }
        public static FileAppender[] GetLogFiles()
        {
            List<FileAppender> LogFiles = new List<FileAppender>();
            log4net.Appender.IAppender[] appenders = _log.Logger.Repository.GetAppenders();
            foreach (IAppender append in appenders)
            {
                if (append.GetType() == typeof(FileAppender))
                {

                    FileAppender app = (FileAppender)append;


                    LogFiles.Add(app);
                    break;
                }
            }
            return LogFiles.ToArray();
        }
        static object LockObject = new object();
        public static string LastLogFile { get; private set; }
        private static void FlushLogFile()
        {
            foreach (FileAppender f in GetLogFiles())
            {
                FlushLogFile(f);
            }
        }
        private static void FlushLogFile(FileAppender file)
        {
            string fle = file.File;
            if (!string.IsNullOrEmpty(fle))
            {
                lock (LockObject)
                {

                    //log4net.Layout.ILayout layout = file.Layout;
                    file.Close();
                    //file = null;
                    LastLogFile = fle + "_" + DateTime.Now.ToString("yyyyMMdd") + ".log";

                    try
                    {
                        if (System.IO.File.Exists(LastLogFile))
                        {
                            System.IO.File.Delete(LastLogFile);
                        }
                        System.Threading.Thread.Sleep(100);

                        System.IO.File.Move(fle, LastLogFile);

                        string configFile = Assembly.GetEntryAssembly().Location + ".config";

                        FileInfo f = new FileInfo(configFile);

                        log4net.Config.XmlConfigurator.ConfigureAndWatch(f);

                    }
                    catch (Exception ex)
                    {
                        if (_log.IsWarnEnabled)
                        {
                            _log.Warn("Exception trying to flush log file:", ex);
                        }
                    }
                }

            }
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(PurgeOldLogFiles), file);
        }
        static void PurgeOldLogFiles(object state)
        {
            string file = state as string;
            if (!string.IsNullOrEmpty(file))
            {
                foreach (FileInfo f in new FileInfo(file).Directory.GetFiles("*.log"))
                {
                    if (f.CreationTime.CompareTo(DateTime.Today.AddDays(-1)) < 0)
                    {
                        f.Delete();
                    }
                }
            }
        }
        public static string ApplicationDataPath
        {

            get
            {
                FileInfo fle = new FileInfo(System.Reflection.Assembly.GetEntryAssembly().Location);

                string retVal = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AssemblyCompany, fle.Name.Substring(0, fle.Name.Length - fle.Extension.Length));
                FileHelper.CreatePath(retVal);
                return retVal;
            }

        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version.ToString();
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static void LogSystemData()
        {
            LogReferenceInfo();
            LogSystemInfo();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static void LogReferenceInfo()
        {
            Assembly asm = Assembly.GetEntryAssembly();

            try
            {
                _log.FatalFormat("Entry Assembly: {0}", asm.GetName().FullName);
            }
            catch { }
            try
            {
                _log.FatalFormat("Version: {0}", asm.GetName().Version.ToString());

            }
            catch { }
            

            foreach (AssemblyName n in asm.GetReferencedAssemblies())
            {
                
           
                try
                {

                    _log.FatalFormat("Referencing: {0}", n.FullName);

                }
                catch { }
            }
            try
            {
                _log.FatalFormat("Operating System: {0}", Environment.OSVersion.VersionString);
            }
            catch { }
            try
            {
                _log.FatalFormat("CLR version: {0}", Environment.Version.ToString());
            }
            catch { }
        }
        ////static void LogSecurityInfo()
        ////{
        ////    //try
        ////    //{
        ////    //    _log.FatalFormat("Machine Name: {0}", Environment.MachineName);
        ////    //}
        ////    //catch { }

        ////    //try
        ////    //{
        ////    //    System.Security.Principal.WindowsIdentity thisUser = System.Security.Principal.WindowsIdentity.GetCurrent();
        ////    //    _log.FatalFormat("Windows Logged in Username: {0}", thisUser.Name);



        ////    //}
        ////    //catch { }

        ////    ////try
        ////    ////{
        ////    ////    System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        ////    ////    System.Net.IPAddress[] addresses = hostEntry.AddressList;

        ////    ////    _log.FatalFormat("This IP Address: {0}", addresses[addresses.Length - 1].ToString());
        ////    ////}
        ////    ////catch
        ////    ////{ }
        ////}
        //static void LogRamInfo()
        //{
        //    // new ObjectQuery("SELECT BankLabel, Capacity, Caption, Description, Manufacturer, Speed FROM Win32_PhysicalMemory");

        //    //using (ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery))
        //    //{
        //    //    ManagementObjectCollection oReturnCollection = oSearcher.Get();

        //    //    foreach (ManagementObject oReturn in oReturnCollection)
        //    //    {
        //    //        try
        //    //        {
        //    //            _log.FatalFormat("Physical memory: {0}", oReturn["Capacity"]);
        //    //        }
        //    //        catch { }
        //    //        try
        //    //        {
        //    //            _log.FatalFormat("         Bank Label:   {0}", oReturn["BankLabel"]);
        //    //        }
        //    //        catch { }
        //    //        try
        //    //        {
        //    //            _log.FatalFormat("         Caption:      {0}", oReturn["Caption"]);
        //    //        }
        //    //        catch { }
        //    //        try
        //    //        {
        //    //            _log.FatalFormat("         Description:  {0}", oReturn["Description"]);
        //    //        }
        //    //        catch { }
        //    //        try
        //    //        {
        //    //            _log.FatalFormat("         Manufacturer: {0}", oReturn["Manufacturer"]);
        //    //        }
        //    //        catch { }
        //    //        try
        //    //        {
        //    //            _log.FatalFormat("         Speed:        {0}", oReturn["Speed"]);
        //    //        }
        //    //        catch { }
        //    //    }
        //    //}
        //}
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        static void LogSystemInfo()
        {
            try
            {
                ManagementScope oMs = new ManagementScope();
                ObjectQuery oQuery = null;
                
                oQuery = new ObjectQuery("SELECT CurrentTimeZone, Description, FreePhysicalMemory, FreeVirtualMemory,"
                    + " InstallDate, LastBootUpTime, Manufacturer, MaxNumberOfProcesses, MaxProcessMemorySize,"
                    + " Name, NumberOfProcesses, Organization, OSArchitecture, OSProductSuite, OSType, ServicePackMajorVersion,"
                    + " ServicePackMinorVersion, Status, TotalVirtualMemorySize, TotalVisibleMemorySize, Version,"
                    + " WindowsDirectory FROM Win32_OperatingSystem");

                using (ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oMs, oQuery))
                {
                    ManagementObjectCollection oReturnCollection = oSearcher.Get();
                    foreach (ManagementObject oReturn in oReturnCollection)
                    {
                        try
                        {
                            _log.FatalFormat("Total Visible Memory Size:  {0}", oReturn["TotalVisibleMemorySize"]);
                        }
                        catch { }
                        try
                        {
                            _log.FatalFormat("Total Virtual Memory Size:  {0}", oReturn["TotalVirtualMemorySize"]);
                        }
                        catch { }
                        try
                        {

                            _log.FatalFormat("Max Process Memory Size:    {0}", oReturn["MaxProcessMemorySize"]);
                        }
                        catch { }
                        try
                        {
                            _log.FatalFormat("Free Physical Memory:       {0}", oReturn["FreePhysicalmemory"]);
                        }
                        catch { }
                        try
                        {
                            _log.FatalFormat("Free Virtual Memory:        {0}", oReturn["FreeVirtualMemory"]);
                        }
                        catch { }
                        try
                        {
                            _log.FatalFormat("Max Number Of Processes:    {0}", oReturn["MaxNumberOfProcesses"]);
                        }
                        catch { }
                        try
                        {
                            _log.FatalFormat("Last Boot-Up Time:          {0}", oReturn["LastBootUpTime"]);
                        }
                        catch { }
                        try
                        {
                            _log.FatalFormat("Number Of Processes:        {0}", oReturn["NumberOfProcesses"]);
                        }
                        catch { }
                       
                        try
                        {
                            _log.FatalFormat("Service Pack Major Version: {0}", oReturn["ServicePackMajorVersion"]);
                        }
                        catch { }
                        try
                        {
                            _log.FatalFormat("Service Pack Minor Version: {0}", oReturn["ServicePackMinorVersion"]);
                        }
                        catch { }
                       
                        try
                        {
                            _log.FatalFormat("OS Architecture:            {0}", oReturn["OSArchitecture"]);
                        }
                        catch { }
                        try
                        {

                            _log.FatalFormat("Status:                     {0}", oReturn["Status"]);
                        }
                        catch { }
                        
                        try
                        {
                            _log.FatalFormat("Current Time Zone:          {0}", oReturn["CurrentTimeZone"]);
                        }
                        catch { }
                       
                    }
                }
                //Installed versions of .NET

                try
                {
                    _log.FatalFormat(".NET 3.0 Service Pack:      {0}", ((int)Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("NET Framework Setup").OpenSubKey("NDP").OpenSubKey("v3.0").GetValue("SP", 0)));
                }
                catch { }
                try
                {
                    _log.FatalFormat(".NET 3.5 Service Pack:      {0}", ((int)Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("NET Framework Setup").OpenSubKey("NDP").OpenSubKey("v3.5").GetValue("SP", 0)));
                }
                catch { }
                try
                {
                    _log.FatalFormat(".NET 4.0 Installed?:        {0}", ((int)Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("NET Framework Setup").OpenSubKey("NDP").OpenSubKey("v4").OpenSubKey("Full").GetValue("Install", 0)));
                }
                catch { }
                try
                {
                    _log.FatalFormat(".NET 4.0 Service Pack:      {0}", ((int)Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("NET Framework Setup").OpenSubKey("NDP").OpenSubKey("v4").OpenSubKey("Full").GetValue("Servicing", 0)));
                }
                catch { }
            }
            catch { }
        }
        static bool VersionCompare(string installedVersion, string currentVersion)
        {
            bool retVal = false;
            string[] nodesInstalled = installedVersion.Split('.');
            string[] nodesCurrent = currentVersion.Split('.');

            int installed = 0;
            int current = 0;
            //example: 3.1.2
            //installedVersion from assemblyInfo.cs
            //example: 3.1.2.5555
            
            for (int i = 0; i < 3; i++) //Just going to compare the first 3 nodes.
            {
                installed = 0;
                current = 0;
                if (i < nodesInstalled.Length)
                {
                    if (!int.TryParse(nodesInstalled[i], out installed))
                    {
                        break;
                    }
                }
                if (i < nodesCurrent.Length)
                {
                    if (!int.TryParse(nodesCurrent[i], out current))
                    {
                        break;
                    }
                }
                if (current > installed)
                {
                    retVal = true;
                    break;
                }
                else if (installed > current)
                {
                    //user is running a newer version than I've updated for release.
                    retVal = false;
                    break;
                }

            }

            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string UpdateCheck(Uri address, string installedVersion)
        {
            //https://dl.dropbox.com/u/14746342/AML_versionData.txt
            bool retVal = false;

            string UpdateURL = null;
            try
            {
                using (WebClient web = new WebClient())
                {
                   
                    string fileData = web.DownloadString(address);
                    string[] data = fileData.Split('\r');
                    
                    string currentVersion = data[0].Trim('\n', ' ', '\r', '\t');
                    
                    retVal = VersionCompare(installedVersion, currentVersion);
                    if (retVal && data.Length > 1)
                    {
                        UpdateURL = data[1].Trim('\n', ' ', '\r', '\t');
                    }
                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Error downloading version data for update check.", ex);
                }
            }
            return UpdateURL;
        }
        
    }
}
