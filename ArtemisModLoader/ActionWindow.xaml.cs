using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using ArtemisModLoader.Controls;
using ArtemisModLoader.Windows;
using log4net;
using MissionStudio;
using RussLibrary;
using RussLibrary.Helpers;
using RussLibrary.Windows;
using RussLibraryAudio;
using VesselDataLibrary.Controls;
using VesselDataLibrary.Text;
using System.Net;
using ArtemisModLoader.Text;
using RussLibrary.Text;

namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for ActionWindow.xaml
    /// </summary>
    public partial class ActionWindow : Window
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ActionWindow));
        public ActionWindow()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }   
            InitializeComponent();
            Locations.MainWindow = this;
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
           
         
           
        }
        void InitializeApplication()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            string ArtyInstallPath = UserConfiguration.Current.GetArtemisInstallPath();
            ContinueStartupProcess();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void ContinueStartupProcess()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }

            Uri UpdateURI = new Uri(App.UpdateCheckURI);
            if (System.Diagnostics.Debugger.IsAttached)
            {
                UpdateURI = new Uri(App.UpdateCheckURIDebugger);
            }

            string UpdateURL = GeneralHelper.UpdateCheck(UpdateURI, GeneralHelper.AssemblyVersion);
            
            if (!string.IsNullOrEmpty(UpdateURL))
            {
                if (MessageBox.Show(this,
                    "An update for Artemis Mod Loader has been detected.\r\n\r\nDo you wish to download the update?\r\n\r\nArtemis Mod Loader will restart when complete.", "Artemis Mod Loader",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    ProcessUpdate(UpdateURL);
                    //Shut down handled by Process Update.
                    //this.Close();
                    //Application.Current.Shutdown();
                    if (!System.Diagnostics.Debugger.IsAttached)
                    {
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Would normally 'return' here to restart.  Debugger is attached, so skipping.");
                    }
                }

            }

            FinishStartup();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        void FinishStartup()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManagement.DoInitialSetup();
            //while (ModManagement.SetupInProgress)
            //{
            //    System.Threading.Thread.Sleep(100);
            //}
            string path = System.IO.Path.Combine(Locations.DataPath, "audio.ini");

            AudioConfiguration audioConfig =
                INIConverter.ToObject(path, typeof(AudioConfiguration)) as AudioConfiguration;

            if (audioConfig.StartupMusic)
            {
                string playfile = Path.Combine(Locations.ArtemisCopyPath, "dat", "Artemis Main Screen.ogg");

                if (File.Exists(playfile))
                {
                    RussLibraryAudio.AudioServer.Current.PlayAsync(playfile);
                }
            }


            this.Dispatcher.BeginInvoke(new Action(ModManagement.UpdateCheckProcess), System.Windows.Threading.DispatcherPriority.Loaded);
            this.Dispatcher.BeginInvoke(new Action<AudioConfiguration>(LoadAudioData), audioConfig);
            this.Dispatcher.BeginInvoke(new Action(DoStartup));
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void LoadAudioData(AudioConfiguration audioConfig)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            audioConfig.SetAudioCollection();
            if (audioConfig.AudioCollection.Count == 0)
            {
                audioConfig.LoadDefault();
            }
            audioConfig.ResetAudioServer();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        void ProcessUpdate(object state)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            
            bool success = true;
            string URL = state as string;
            string file = URL.Substring(URL.LastIndexOf('/') + 1);
            string fullPath = Path.Combine(Path.GetTempPath(), file);
            if (!string.IsNullOrEmpty(URL))
            {
                try
                {
                    FileHelper.DeleteFile(fullPath);
                    using (WebClient w = new WebClient())
                    {
                        Uri address = new Uri(URL);
                        w.DownloadFile(address, fullPath);
                    }
                }
                catch (Exception ex)
                {
                    if (_log.IsWarnEnabled)
                    {
                        _log.Warn("Error downloading updated installer", ex);
                    }
                    Locations.MessageBoxShow("Failed to download update.\r\n\r\nPlease try again later.\r\n\r\nError:\r\n" + ex.Message,
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    success = false;
                }
            }
            this.Dispatcher.BeginInvoke(new Action(CloseStandBy));
            
            if (success && !System.Diagnostics.Debugger.IsAttached)
            {
                this.Dispatcher.BeginInvoke(new Action<string>(CloseDown), fullPath);
            }
            else
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    if (success)
                    {
                        Locations.MessageBoxShow("Update would have been successful--Debugger is attached.  Update is downlaoded.",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        Locations.MessageBoxShow("Update would have been Unsuccessful--Debugger is attached.  Proceeding normally.",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }

                this.Dispatcher.BeginInvoke(new Action(FinishStartup));
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void CloseStandBy()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (standBy != null)
            {
                standBy.Close();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        void CloseDown(string fullPath)
        {

            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            System.Diagnostics.Process.Start(fullPath);
            Application.Current.Shutdown();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        }
        static StandBy standBy = null;
        void ProcessUpdate(string URL)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            standBy = new StandBy();
            standBy.Show();
            System.Threading.ThreadPool.QueueUserWorkItem(ProcessUpdate, URL);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }





        void DoStartup()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ArtemisINI data = LoadINI();
            if (data != null)
            {
                Port = data.NetworkPort;
            }
            ImageBrush brsh = ArtemisModLoader.Helpers.FileHelper.GetRandomSkybox();
            if (brsh != null)
            {
                this.Background = brsh;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        ArtemisINI LoadINI()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ArtemisINI artemisData = null;
            string INIPath = System.IO.Path.Combine(Locations.ArtemisCopyPath, "artemis.ini");
            if (File.Exists(INIPath))
            {
                artemisData = new ArtemisINI(INIPath);
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return artemisData;
        }

        private void RemoveIPAddress_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                string IP = btn.CommandParameter as string;
                if (!string.IsNullOrEmpty(IP))
                {
                    int idx = 0;
                    while (IP == SelectedIPAddress && idx < IPAddresses.Count)
                    {
                        SelectedIPAddress = IPAddresses[idx++];
                    }
                    IPAddresses.Remove(IP);
                    using (StreamWriter sw = new StreamWriter(Locations.IPAddressListFile, false))
                    {
                        foreach (string ipAddr in IPAddresses)
                        {
                            sw.WriteLine(ipAddr);
                        }
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }


        public static readonly DependencyProperty PortProperty =
            DependencyProperty.Register("Port", typeof(int),
            typeof(ActionWindow));

        public int Port
        {
            get
            {
                return (int)this.UIThreadGetValue(PortProperty);

            }
            private set
            {
                this.UIThreadSetValue(PortProperty, value);

            }
        }

        public static readonly DependencyProperty IPAddressesProperty =
            DependencyProperty.Register("IPAddresses", typeof(ObservableCollection<string>),
            typeof(ActionWindow), new PropertyMetadata(new ObservableCollection<string>(Locations.GetIPList())));

        public ObservableCollection<string> IPAddresses
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(IPAddressesProperty);

            }
            private set
            {
                this.UIThreadSetValue(IPAddressesProperty, value);

            }
        }

        public static readonly DependencyProperty SelectedIPAddressProperty =
            DependencyProperty.Register("SelectedIPAddress", typeof(string),
            typeof(ActionWindow), new PropertyMetadata(Locations.GetCurrentIPAddress()));

        public string SelectedIPAddress
        {
            get
            {
                return (string)this.UIThreadGetValue(SelectedIPAddressProperty);

            }
            private set
            {
                this.UIThreadSetValue(SelectedIPAddressProperty, value);

            }
        }
       


        private void RunArtemis_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }   
            if (this.SelectedIPAddress != Locations.GetCurrentIPAddress())
            {
                if (!Locations.SetIPAddress(this.SelectedIPAddress))
                {
                    Locations.MessageBoxShow("Unable to set the Server IP address.\r\n\r\nReview log file for details as to why.",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            Window thiswin = Window.GetWindow(this);


            thiswin.ShowInTaskbar = false;
            SessionMonitor win = new SessionMonitor();
            win.StartSession();
            DoAnimation();
            win.ShowDialog();
            thiswin.ShowInTaskbar = true;
            thiswin.WindowState = System.Windows.WindowState.Normal;

            string newIP = Locations.GetCurrentIPAddress();
            if (!IPAddresses.Contains(newIP))
            {
                Locations.AddToIPList(newIP);
                IPAddresses.Add(newIP);
                SelectedIPAddress = newIP;
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }


        void DoAnimation()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Window thisWindow = Window.GetWindow(this);

            double thisWidth = thisWindow.Width;
            double thisHeight = thisWindow.Height;
            double thisTop = thisWindow.Top;
            double thisLeft = thisWindow.Left;

            for (double i = 500; i > 1; i -= 3)
            {
                thisWindow.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;
                thisWindow.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height;
                thisWindow.Width = i;
                thisWindow.Height = i;
                //System.Threading.Thread.Sleep(1);
            }
            thisWindow.Left = thisLeft;
            thisWindow.Top = thisTop;
            thisWindow.Width = thisWidth;
            thisWindow.Height = thisHeight;
            thisWindow.WindowState = System.Windows.WindowState.Minimized;

            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void ModManager_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModDevelopmentManager win = new ModDevelopmentManager();
            win.Show();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            About win = new About();
            win.Owner = this;
            win.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            win.ShowDialog();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            List<ISettingsPanel> list = new List<ISettingsPanel>();
            SettingsPanel pnl = new SettingsPanel();
            list.Add(pnl);
            ArtemisEngineeringPresets.Manager pnl2 = new ArtemisEngineeringPresets.Manager();
            list.Add(pnl2);
            AudioSettingsPanel pnl3 = new AudioSettingsPanel();
            list.Add(pnl3);
            SettingsWindow.Show(list, Locations.DataPath);
            //ImageBrush brsh = ArtemisModLoader.Helpers.FileHelper.GetRandomSkybox();
            //if (brsh != null)
            //{
            //    this.Background = brsh;
            //}
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ModManager win = new ModManager();
            win.Show();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
       

        private void GetAndroid_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            System.Diagnostics.Process.Start(DataStrings.AndroidClientWebsite);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Application.Current.Shutdown();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void PortApply_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ArtemisINI data = LoadINI();
            if (data != null)
            {
                data.NetworkPort = Port;
                data.Save();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void MissionStudio_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ScriptControl.AddButtonRowContent(new PlayPlayListButton());
            ScriptControl.SetArtemisInstallPath(Locations.ArtemisCopyPath);
            ScriptControl.Show();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            
        }

        private void PayPal_click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            System.Diagnostics.ProcessStartInfo strt = new System.Diagnostics.ProcessStartInfo("http://www.paypal.com");
            strt.UseShellExecute = true;
            System.Diagnostics.Process.Start(strt);
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void ActionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            InitializeApplication();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
    }
}