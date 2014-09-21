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
using RussLibrary;
using DMXCommander.Xml;
using RussLibrary.Xml;
using System.IO;
using DMXCommander.Engine;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Configuration;
using log4net;

namespace DMXCommander.Controls
{
    /// <summary>
    /// Interaction logic for DMXCommandControl.xaml
    /// </summary>
    public partial class DMXCommandControl : UserControl, IDisposable
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(DMXCommandControl));
        public DMXCommandControl()
        {
            if (!bool.TryParse(ConfigurationManager.AppSettings["TestMode"], out InTestMode))
            {
                InTestMode = false;
            }
            InitializeComponent();
            Data = new DMXCommandFile();
            fsw = new FileSystemWatcher();
            fsw.Changed += new FileSystemEventHandler(fsw_Changed);

            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.DirectoryName;



        }



        ~DMXCommandControl()
        {

            Dispose(false);

        }


        void SetWatcher(string path)
        {
            FileInfo f = new FileInfo(path);
            fsw.Path = f.DirectoryName;
            fsw.Filter = f.Name;
            fsw.EnableRaisingEvents = true;
        }


        void fsw_Changed(object sender, FileSystemEventArgs e)
        {
            if (MessageBox.Show("Something else has changed the DMX Command file.\r\n\r\nDo you wish to reload?", "DMX Commander", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                Data.Dispatcher.Invoke(new Action<string>(LoadFile), WorkFile);

            }
        }
        private string WorkFile = null;
        public void New()
        {
            if (Data.Changed)
            {
                switch (MessageBox.Show("Do you wish to save the current file first?", "DMX Commander", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        if (!Save())
                        {
                            if (!SaveAs())
                            {
                                return;
                            }
                        }
                        break;

                }
            }
            WorkFile = null;
            this.Data = new DMXCommandFile();
            Data.AcceptChanges();

        }
        public bool SaveAs()
        {

            bool retVal = false;
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = "DMX Commander: Select file to save";
            diag.FileName = WorkFile;
            diag.DefaultExt = ".xml";
            diag.AddExtension = true;
            //diag.CheckFileExists = true;
            diag.CheckPathExists = true;
            diag.OverwritePrompt = true;
            if (diag.ShowDialog() == true)
            {

                WorkFile = diag.FileName;
                retVal = Save();
            }
            return retVal;

        }

        public bool Save()
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(WorkFile))
            {

                return SaveAs();
            }
            System.Xml.XmlDocument doc = XmlConverter.ToXmlDocument(Data, true);
            try
            {
                fsw.EnableRaisingEvents = false;
                doc.Save(WorkFile);
                SetWatcher(WorkFile);
                Data.AcceptChanges();
                retVal = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to save file. \r\n\r\nError from Windows:\r\n\r\n" + ex.Message, "DMX Commander", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return retVal;
        }
        public void LoadFile(string path)
        {
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                Reset();
                Data = XmlConverter.ToObject(path, typeof(DMXCommandFile)) as DMXCommandFile;
                Data.AcceptChanges();
                int priority = 0;
                if (Data != null)
                {
                    Data.AcceptChanges();
                    WorkFile = path;
                    SetWatcher(WorkFile);
                    List<string> cc = new List<string>();


                    foreach (EventObject e in Data.Events)
                    {
                        e.Priority = priority++;
                        e.DeactivateMe += e_DeactivateMe;
                        if (!cc.Contains(e.EventType))
                        {
                            cc.Add(e.EventType);
                        }
                    }

                    cc.Sort();
                    Cues = new ObservableCollection<string>(cc);
                }
            }
            else
            {
                MessageBox.Show("File Not Found:\r\n\r\n" + path, "DMX Commander", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void e_DeactivateMe(object sender, EventArgs e)
        {
            EventObject ev = sender as EventObject;
            if (ev != null)
            {
                Controller.Current.DeactivateEvent(ev);
            }
        }


        public void Reset()
        {

            fsw.EnableRaisingEvents = false;
            Data = null;

        }
        FileSystemWatcher fsw = null;
        public static readonly DependencyProperty CuesProperty =
         DependencyProperty.Register("Cues", typeof(ObservableCollection<string>),
         typeof(DMXCommandControl));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<string> Cues
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(CuesProperty);
            }
            set
            {
                this.UIThreadSetValue(CuesProperty, value);
            }
        }



        public static readonly DependencyProperty DataProperty =
         DependencyProperty.Register("Data", typeof(DMXCommandFile),
         typeof(DMXCommandControl));

        public DMXCommandFile Data
        {
            get
            {
                return (DMXCommandFile)this.UIThreadGetValue(DataProperty);
            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);
            }
        }
        bool isDisposed = false;
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    fsw.Dispose();

                    try
                    {
                        OpenDMX.Stop();
                    }
                    catch { }

                    isDisposed = true;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnActivateCue(object sender, RoutedEventArgs e)
        {

            string cue = e.OriginalSource as string;
            foreach (EventObject ev in Data.Events)
            {
                if (ev.EventType == cue)
                {
                    Controller.Current.ActivateEvent(ev);
                }
            }

        }

        private void OnDeactivateCue(object sender, RoutedEventArgs e)
        {

            string cue = e.OriginalSource as string;

            foreach (EventObject ev in Data.Events)
            {
                if (ev.EventType == cue)
                {
                    Controller.Current.DeactivateEvent(ev);
                }
            }

        }
        void DoConfiguration()
        {
            Window win = new Window();
            win.Title = "DMX Commander--Channel Definition";
            win.Content = new ChannelDefintionControl();
            win.SizeToContent = SizeToContent.Width;
            win.Height = 300;
            win.ShowDialog();

        }
        private void OnChannelConfiguration(object sender, RoutedEventArgs e)
        {
            DoConfiguration();
        }

        private void OnMoveUp(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                EventObject ev = btn.CommandParameter as EventObject;
                if (ev != null)
                {
                    if (ev.Priority > 0)
                    {
                        int pos = ev.Priority;
                        ev.Priority--;
                        Data.Events.Remove(ev);


                        Data.Events.Insert(ev.Priority, ev);

                        for (int i = pos; i < Data.Events.Count; i++)
                        {
                            Data.Events[i].Priority = i;
                        }


                    }
                }
            }
        }

        private void OnMoveDown(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                EventObject ev = btn.CommandParameter as EventObject;
                if (ev != null)
                {
                    if (ev.Priority < Data.Events.Count - 1)
                    {
                        int pos = ev.Priority;


                        Data.Events.Remove(ev);


                        Data.Events.Insert(pos + 1, ev);

                        for (int i = pos; i < Data.Events.Count; i++)
                        {
                            Data.Events[i].Priority = i;
                        }


                    }
                }
            }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                EventObject ev = btn.CommandParameter as EventObject;
                if (ev != null)
                {
                    int pos = ev.Priority;
                    Data.Events.Remove(ev);
                    for (int i = pos; i < Data.Events.Count; i++)
                    {
                        Data.Events[i].Priority = i;
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void OnAddEvent(object sender, RoutedEventArgs e)
        {
            EventObject ev = new EventObject();

            ev.Priority = Data.Events.Count;
            Data.Events.Add(ev);



        }

        private void OnEventTypeChanged(object sender, RoutedEventArgs e)
        {
            if (e != null)
            {
                string ev = e.OriginalSource as string;
                if (!string.IsNullOrEmpty(ev) && Cues != null)
                {
                    if (!Cues.Contains(ev))
                    {
                        Cues.Add(ev);
                    }
                }
            }
        }

        private void OnNew(object sender, RoutedEventArgs e)
        {
            New();

        }

        private void OnSaveAs(object sender, RoutedEventArgs e)
        {
            SaveAs();

        }

        bool InTestMode = false;
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(WorkFile) && System.IO.File.Exists(WorkFile))
            {
                GeneralHelper.ResetChannelList();
            }
            else
            {
                if (DMXConfigurationFile.Current.Definitions.Count == 0)
                {
                    DoConfiguration();
                }
            }
            try
            {
                if (!InTestMode)
                {
                    OpenDMX.Start();
                }
            }
            catch (Exception ex)
            {
                if (_log.IsErrorEnabled)
                {
                    _log.Error("Error opening DMX Device:", ex);
                }
                MessageBox.Show("There was an error starting the DMX Device.\r\nYou will need to restart this application to get it to work with your device after you fix the issue.\r\n\r\nError:\r\n\r\n" + ex.Message,
                    "DMX Commander", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

        }

        private void OnRunScript(object sender, RoutedEventArgs e)
        {
            ScriptEditor ctl = new ScriptEditor();
            Window win = new Window();
            win.Title = "Script Editor";
            win.Content = ctl;
            win.Show();
            //OpenFileDialog diag = new OpenFileDialog();
            //diag.Title = "DMX Commander: Select script to run";
            //diag.Filter = "DMX Script files (*.DMXCommand)|*.DMXCommand|All Files (*.*)|*.*";
            //diag.DefaultExt = ".DMXCommand";
            //diag.CheckFileExists = true;
            //bool success = true;
            //if (diag.ShowDialog() == true)
            //{
            //    success = ScriptEngine.Current.Run(diag.FileName);

            //    if (success)
            //    {
            //        MessageBox.Show("Script run complete.", "DMX Commander", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //    else
            //    {
                    
            //        MessageBox.Show("Script run failed:\r\n\r\n" + ScriptEngine.Current.ErrorListing, "DMX Commander", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
        }
    }
}
