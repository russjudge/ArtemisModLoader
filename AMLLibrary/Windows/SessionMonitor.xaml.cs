using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using RussLibrary;
using RussLibrary.Helpers;
namespace ArtemisModLoader.Windows
{
    /// <summary>
    /// Interaction logic for SessionMonitor.xaml
    /// </summary>
    public partial class SessionMonitor : Window
    {
        public SessionMonitor()
        {
            InitializeComponent();
        }
        List<System.Diagnostics.Process> processes = new List<System.Diagnostics.Process>();

        System.Windows.Threading.DispatcherTimer timer;
        public void StartSession()
        {
            if (RussLibraryAudio.AudioServer.Current.IsPlaying)
            {
                RussLibraryAudio.AudioServer.Current.Stop();
            }
            if (!System.IO.File.Exists(Locations.ArtemisFileToRun))
            {
                Locations.MessageBoxShow("Artemis executable not found.\r\n\r\nPlease be sure that Artemis is installed, then restart Artemis Mod Loader.", MessageBoxButton.OK, MessageBoxImage.Stop);

            }
            else
            {
                if (processes.Count == 0)
                {
                    timer = new System.Windows.Threading.DispatcherTimer();
                    timer.Interval = new TimeSpan(5000);

                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Start();
                }

                ProcessStartInfo strt = new ProcessStartInfo(Locations.ArtemisFileToRun);
                strt.WorkingDirectory = Locations.ArtemisCopyPath;
                if (UserConfiguration.Current.UseArtemisExtender)
                {
                    strt.Verb = DataStrings.AdminVerb;
                }

                Process prc = System.Diagnostics.Process.Start(strt);


                processes.Add(prc);
                ProcessCount = processes.Count;
            }

        }

        void timer_Tick(object sender, EventArgs e)
        {
            List<Process> prcList = new List<Process>();
            foreach (Process prc in processes)
            {
                if (prc.HasExited)
                {
                    prcList.Add(prc);
                }
            }
            foreach (Process prc in prcList)
            {

                processes.Remove(prc);
                prc.Dispose();
            }
            ProcessCount = processes.Count;
           
        }


       

        public static void OnProcessCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SessionMonitor me = sender as SessionMonitor;
            if (me != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(GeneralHelper.AssemblyTitle);
                sb.AppendLine();
                sb.Append(AMLResources.Properties.Resources.SessionCountLabel);
                sb.Append(DataStrings.Space);
                sb.AppendLine(me.ProcessCount.ToString());
                sb.AppendLine();
                sb.Append(AMLResources.Properties.Resources.PlayAnotherArtemis);
                if (me.NotifyIcon != null)
                {
                    me.NotifyIcon.ToolTipText = sb.ToString();
                }
                me.txCount.Text = me.ProcessCount.ToString();
                if (me.ProcessCount < 1)
                {
                   
                    me.Close();
                }
            }
        }
        public static readonly DependencyProperty ProcessCountProperty =
          DependencyProperty.Register("ProcessCount", typeof(int),
          typeof(SessionMonitor), new PropertyMetadata(OnProcessCountChanged));

        public int ProcessCount
        {
            get
            {
                return (int)this.UIThreadGetValue(ProcessCountProperty);

            }
            set
            {
                this.UIThreadSetValue(ProcessCountProperty, value);

            }
        }

        private void Play_Artemis(object sender, RoutedEventArgs e)
        {
            StartSession();
        }

        private void uc_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ProcessCount > 0)
            {
                e.Cancel = true;
            }
        }
        void DoPopUp()
        {
            NotifyIcon.ShowBalloonTip(GeneralHelper.AssemblyTitle, AMLResources.Properties.Resources.PlayAnotherArtemis, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }
        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(DoPopUp));
        }

        private void uc_Closed(object sender, EventArgs e)
        {
            if (NotifyIcon != null)
            {
                NotifyIcon.Dispose();
                NotifyIcon = null;
            }
        }
      
    }
}

