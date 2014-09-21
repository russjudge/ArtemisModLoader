using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ArtemisModLoader.Mission;
using log4net;
using Microsoft.Win32;
using RussLibrary;
using RussLibrary.Helpers;
namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for Missions.xaml
    /// </summary>
    public partial class Missions : UserControl
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Missions));
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
        //public void AddMission()
        //{
        //    //Could be zip or xml.
        //    OpenFileDialog diag = new OpenFileDialog();
        //    diag.Title = Locations.AssemblyTitle + " " + AMLResources.Properties.Resources.AddMission;
        //    diag.Filter = AMLResources.Properties.Resources.XML + DataStrings.XMLFilter +
        //        "|" + AMLResources.Properties.Resources.SupportedCompressedFiles + DataStrings.SupportedCompressedFilesFilter;
        //    diag.Multiselect = true;
        //    if (diag.ShowDialog() == true)
        //    {
        //        if (diag.FileNames != null)
        //        {
        //            ModManagement.ProcessFiles(diag.FileNames);
        //        }
               
            
        //    }

        //}
     
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

        //KeyValuePair<string, string> GetMissionName(string compressedFile)
        //{
        //    string retVal = null;
        //    string path = null;
        //    try
        //    {
        //        using (Stream stream = File.OpenRead(compressedFile))
        //        {
        //            IReader reader = ReaderFactory.Open(stream);
        //            while (reader.MoveToNextEntry())
        //            {
        //                if (!reader.Entry.IsDirectory)
        //                {
        //                    if (reader.Entry.FilePath.StartsWith("MISS_", StringComparison.OrdinalIgnoreCase) && reader.Entry.FilePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        //                    {
        //                        retVal = reader.Entry.FilePath.Substring(0, reader.Entry.FilePath.Length - 4);
        //                        if (retVal.Contains("\\"))
        //                        {
        //                            path = retVal.Substring(0, retVal.IndexOf('\\'));
        //                            retVal = retVal.Substring(retVal.LastIndexOf('\\') + 1);
        //                        }
        //                        break;
        //                    }

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (_log.IsWarnEnabled)
        //        {
        //            _log.Warn("Error process compressed file:", ex);
        //        }
        //    }
        //    return new KeyValuePair<string, string>(path, retVal);
        //}
        //void ProcessFile(string file)
        //{
        //    if (file.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
        //    {
        //        //Is XML file only.
        //        FileInfo f = new FileInfo(file);
        //        string missionName = f.Name.Substring(0, f.Name.Length - 4);
        //        string missionPath = System.IO.Path.Combine(Locations.ArtemisMissionPath, missionName);
        //        if (Directory.Exists(missionPath) && File.Exists(System.IO.Path.Combine(missionPath, f.Name)))
        //        {
        //            MessageBox.Show(missionName + " is already installed.  Cannot install.", "Artemis Mod Loader Mission Add", MessageBoxButton.OK, MessageBoxImage.Hand);
        //        }
        //        else
        //        {
        //            Locations.CreatePath(missionPath);
        //            string target =System.IO.Path.Combine(missionPath, f.Name); 
        //            f.CopyTo(target);
        //            MissionList.Add(new big_message(target));
        //        }
        //    }
        //    else
        //    {
        //        //Assume compress file.  Look for xml mission file.
               
                
        //        KeyValuePair<string, string> missionData = GetMissionName(file);
        //        string missionName = missionData.Value;
        //        string rootPath = missionData.Key;
        //        if (!string.IsNullOrEmpty(missionName))
        //        {
        //            string missionPath = System.IO.Path.Combine(Locations.ArtemisMissionPath, missionName);
        //            string missionFileName= missionName + ".xml";
        //            string completedTarget = System.IO.Path.Combine(missionPath, missionFileName);

        //            if (Directory.Exists(missionPath) && File.Exists(completedTarget))
        //            {
        //                MessageBox.Show(missionName + " is already installed.  Cannot install.", "Artemis Mod Loader Mission Add", MessageBoxButton.OK, MessageBoxImage.Hand);
        //            }
        //            else
        //            {

        //                Locations.CreatePath(missionPath);

        //                using (Stream stream = File.OpenRead(file))
        //                {
        //                    IReader reader = ReaderFactory.Open(stream);
        //                    while (reader.MoveToNextEntry())
        //                    {
        //                        if (!reader.Entry.IsDirectory)
        //                        {
        //                            string targName = reader.Entry.FilePath;
        //                            string ExtractTarget = targName;
        //                            if (!string.IsNullOrEmpty(rootPath))
        //                            {
        //                                targName = targName.Substring(rootPath.Length + 1);
        //                            }
        //                            string target = System.IO.Path.Combine(missionPath, targName);

        //                            Locations.DeleteFile(target);
        //                            reader.WriteEntryToDirectory(missionPath,
        //                                SharpCompress.Common.ExtractOptions.ExtractFullPath
        //                                | SharpCompress.Common.ExtractOptions.Overwrite);
        //                            if (ExtractTarget != targName)
        //                            {
        //                                FileInfo extractTarg = new FileInfo(System.IO.Path.Combine(missionPath, ExtractTarget));
        //                                DirectoryInfo extractDir = extractTarg.Directory;
        //                                extractTarg.MoveTo(target);
        //                                try
        //                                {
        //                                    extractDir.Delete();
        //                                }
        //                                catch (Exception ex)
        //                                {
        //                                    if (_log.IsWarnEnabled)
        //                                    {
        //                                        _log.Warn("Unable to delete extraneous mission directory", ex);
        //                                    }
        //                                }
        //                            }
        //                            FileInfo f = new FileInfo(target);
        //                            f.LastWriteTime = reader.Entry.LastModifiedTime.Value;
        //                        }
        //                    }
        //                }
        //                MissionList.Add(new big_message(completedTarget));
                        
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("Mission name indeterminate.  Unable to install.", "Artemis Mod Loader Mission Add", MessageBoxButton.OK, MessageBoxImage.Hand);
        //        }
        //    }
        //}
        //public void uc_Drop(object sender, DragEventArgs e)
        //{
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //    if (e.Data.GetDataPresent(DataFormats.FileDrop))
        //    {
        //        string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
        //        foreach (string file in files)
        //        {
        //            ProcessFile(file);
        //        }
        //        if (files.Length > 1)
        //        {
        //            MessageBox.Show("Missions installed.", "Artemis Mod Loader Add Mission", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        else
        //        {
        //            MessageBox.Show("Mission installed.", "Artemis Mod Loader Add Mission", MessageBoxButton.OK, MessageBoxImage.Information);
        //        }
        //        e.Handled = true;
        //    }
        //    if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        //}

       
    }
}
