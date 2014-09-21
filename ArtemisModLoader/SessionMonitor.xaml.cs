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
using System.Windows.Shapes;
using System.Diagnostics;
using RussLibrary;
namespace ArtemisModLoader
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
            if (processes.Count == 0)
            {
                timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = new TimeSpan(10000);

                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
            ProcessStartInfo strt = new ProcessStartInfo(Locations.ArtemisFileToRun);
            strt.WorkingDirectory = Locations.ArtemisCopyPath;
            if (Locations.UseArtemisExtender)
            {
                strt.Verb = "RunAs";
            }
            Process prc = System.Diagnostics.Process.Start(strt);
           
            
            processes.Add(prc);
            ProcessCount = processes.Count;
            
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
                me.NotifyIcon.ToolTipText = Locations.AssemblyTitle + "\r\n\r\nCurrent Artemis Session Count: " + me.ProcessCount.ToString() + "\r\n\r\nClick to start another Artemis session.";
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
            NotifyIcon.ShowBalloonTip(Locations.AssemblyTitle, "Click here to start another Artemis Session", Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }
        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(DoPopUp));
        }
      
    }
}
