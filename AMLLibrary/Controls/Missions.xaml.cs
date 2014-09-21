using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ArtemisModLoader.Mission;
using log4net;
using RussLibrary;
using RussLibrary.Helpers;
using MissionStudio;
using RussLibraryAudio;
namespace ArtemisModLoader.Controls
{
    /// <summary>
    /// Interaction logic for Missions.xaml
    /// </summary>
    public partial class Missions : UserControl
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(Missions));
        public Missions()
        {

         
            
            MissionList = new ObservableCollection<big_message>();
            ModManagement.MissionsUpdated += new EventHandler(ModManagement_MissionsUpdated);
            InitializeComponent();
            LoadMissions();
        }

        void ModManagement_MissionsUpdated(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(LoadMissions));
        }
        void ReLoadMissions(object state)
        {
            System.Threading.Thread.Sleep(100);

        }
        void LoadMissions()
        {

            if (!Directory.Exists(Locations.ArtemisMissionPath))
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ReLoadMissions));
            }
            else
            {
                MissionList.Clear();
                DirectoryInfo missionDir = new DirectoryInfo(Locations.ArtemisMissionPath);

                foreach (FileInfo f in missionDir.GetFiles("MISS_*.xml", SearchOption.AllDirectories))
                {
                    big_message m = new big_message(f.FullName);
                    if (m != null)
                    {
                        MissionList.Add(m);
                    }
                }
            }
        }
        
     
        public static readonly DependencyProperty MissionListProperty =
           DependencyProperty.Register("MissionList", typeof(ObservableCollection<big_message>),
           typeof(Missions));

        public ObservableCollection<big_message> MissionList
        {
            get
            {
                return (ObservableCollection<big_message>)this.UIThreadGetValue(MissionListProperty);

            }
            private set
            {
                this.UIThreadSetValue(MissionListProperty, value);

            }
        }

        private void Remove_click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                big_message miss = btn.CommandParameter as big_message;
                if (miss != null)
                {
                    if (Locations.MessageBoxShow(AMLResources.Properties.Resources.AreYouSure 
                        + DataStrings.CRCR
                        + AMLResources.Properties.Resources.ThisIsNotRecoverable, 
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {

                        FileInfo f = new FileInfo(miss.MissionPath);
                        FileHelper.DeleteAllFiles(f.DirectoryName);
                        MissionList.Remove(miss);
                    }
                }
            }
        }



        private void MissionEditor_Click(object sender, RoutedEventArgs e)
        {

            Button btn = sender as Button;
            if (btn != null)
            {
                big_message miss = btn.CommandParameter as big_message;
                if (miss != null)
                {
                    string parms = string.Format(System.Globalization.CultureInfo.CurrentCulture, UserConfiguration.Current.MissionEditorParameters,
                        Path.Combine(Locations.ArtemisCopyPath, "dat", "vesselData.xml"),
                        miss.MissionPath);
                    System.Diagnostics.Process.Start(
                        UserConfiguration.Current.MissionEditorPath,
                        parms);
                }
            }
        }

        private void MissionStudio_Click(object sender, RoutedEventArgs e)
        {
             Button btn = sender as Button;
             if (btn != null)
             {
                 big_message miss = btn.CommandParameter as big_message;
                 if (miss != null)
                 {
                     ScriptControl.AddButtonRowContent(new PlayPlayListButton());
                     ScriptControl.SetArtemisInstallPath(Locations.ArtemisCopyPath);
                     ScriptControl.Show(miss.MissionPath);
                 }
             }
        }
       
    }
}
