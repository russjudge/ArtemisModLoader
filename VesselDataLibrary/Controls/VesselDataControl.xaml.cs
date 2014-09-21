using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using ArtemisModLoader;
using ArtemisModLoader.Xml;
using Microsoft.Win32;
using RussLibrary;
using RussLibrary.Xml;
using VesselDataLibrary.Xml;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Xml;
using System.Text;
using RussLibrary.Controls;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class VesselDataControl : UserControl, IDisposable
    {
        void LoadCommandBindings()
        {

            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Save, new ExecutedRoutedEventHandler(DoSave)));

            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.Open, new ExecutedRoutedEventHandler(DoOpen)));

            this.CommandBindings.Add(
                new CommandBinding(ApplicationCommands.New, new ExecutedRoutedEventHandler(DoNew)));

            this.CommandBindings.Add(
               new CommandBinding(ApplicationCommands.SaveAs, new ExecutedRoutedEventHandler(DoSaveAs)));
        }


        void DoSave(object sender, ExecutedRoutedEventArgs e)
        {
            fsw.EnableRaisingEvents = false;
            if (string.IsNullOrEmpty(WorkFile) || !WorkFile.StartsWith(Configuration.InstalledPath, StringComparison.OrdinalIgnoreCase))
            {
                DoSavePromptForFilename();
            }
            else
            {
                DoSave();
            }
        }
        public VesselDataControl()
        {

            InitializeComponent();
            LoadCommandBindings();

            fsw = new FileSystemWatcher();
            fsw.Changed += new FileSystemEventHandler(fsw_Changed);

            fsw.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.DirectoryName;


        }
        ~VesselDataControl()
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
            if (Locations.MessageBoxShow("Something else has changed the vesselData file.\r\n\r\nDo you wish to reload?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {

                Data.Dispatcher.Invoke(new Action(LoadFile));

            }
        }

        void LoadFile()
        {
            Reset();
      
            Data = XmlConverter.ToObject(WorkFile, typeof(VesselDataObject)) as VesselDataObject;
            Data.AcceptChanges();
            if (Data.Vessels.Count > 0)
            {
                SelectedVessel = Data.Vessels[0];
            }
            SetWatcher(WorkFile);
        }
        public void Reset()
        {

            fsw.EnableRaisingEvents = false;
            Data = null;

        }


        public static readonly DependencyProperty ShowButtonsProperty =
           DependencyProperty.Register("ShowButtons", typeof(bool),
           typeof(VesselDataControl), new PropertyMetadata(true));

        public bool ShowButtons
        {
            get
            {
                return (bool)this.UIThreadGetValue(ShowButtonsProperty);
            }
            set
            {
                this.UIThreadSetValue(ShowButtonsProperty, value);
            }
        }
        FileSystemWatcher fsw = null;
        static void OnDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VesselDataControl me = sender as VesselDataControl;
            if (me != null)
            {
                VesselDataObject d = e.OldValue as VesselDataObject;
                if (d != null)
                {
                    if (!string.IsNullOrEmpty(me.WorkFile) && File.Exists(me.WorkFile))
                    {
                        me.SetWatcher(me.WorkFile);

                    }
                    if (d.HullRaces != null)
                    {
                        d.HullRaces.CollectionChanged -=
                            new System.Collections.Specialized.NotifyCollectionChangedEventHandler(me.HullRaces_CollectionChanged);
                        me.HullRaceEventSubscribed = false;
                    }

                }
                me.LoadHullRaceFilter();
                //me.LoadVesselView();
            }
        }
        bool HullRaceEventSubscribed = false;
        void LoadHullRaceFilter()
        {
            if (Data != null)
            {
                HullRaceFilter = new HullRaceCollection();
                HullRace allRaces = new HullRace();
                allRaces.ID = -1;
                allRaces.Name = "Any race";
                HullRaceFilter.Add(allRaces);
                if (Data.HullRaces != null)
                {
                    foreach (HullRace race in Data.HullRaces)
                    {
                        HullRaceFilter.Add(race);
                    }

                    if (!HullRaceEventSubscribed)
                    {
                        Data.HullRaces.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HullRaces_CollectionChanged);
                        HullRaceEventSubscribed = true;
                    }
                }
            }
        }
        bool VesselFilter(object v)
        {
            bool retval = true;
            Vessel vsl = v as Vessel;
            if (vsl != null)
            {
                retval = (SelectedHullRace == null || vsl.Side == SelectedHullRace.ID || SelectedHullRace.ID < 0);
            }
            return retval;
        }
        void LoadVesselView()
        {
      
            if (Data != null && Data.Vessels != null)
            {
                VesselsView = new CollectionView(Data.Vessels);
            }
            else
            {
                VesselsView = null;
            }
            VesselsView.Filter = new Predicate<object>(VesselFilter);
           
        }



        public static readonly DependencyProperty VesselsViewProperty =
          DependencyProperty.Register("VesselsView", typeof(CollectionView),
          typeof(VesselDataControl));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("vessel")]
        public CollectionView VesselsView
        {
            get
            {
                return (CollectionView)this.UIThreadGetValue(VesselsViewProperty);

            }
            set
            {
                this.UIThreadSetValue(VesselsViewProperty, value);

            }
        }






        void HullRaces_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            LoadHullRaceFilter();
        }

        static void OnSelectedHullRaceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
           
        }
        public static readonly DependencyProperty SelectedHullRaceProperty =
            DependencyProperty.Register("SelectedHullRace", typeof(HullRace),
            typeof(VesselDataControl), new PropertyMetadata(OnSelectedHullRaceChanged));

        public HullRace SelectedHullRace
        {
            get
            {
                return (HullRace)this.UIThreadGetValue(SelectedHullRaceProperty);
            }
            set
            {
                this.UIThreadSetValue(SelectedHullRaceProperty, value);
            }
        }

        public static readonly DependencyProperty HullRaceFilterProperty =
            DependencyProperty.Register("HullRaceFilter", typeof(HullRaceCollection),
            typeof(VesselDataControl));

        public HullRaceCollection HullRaceFilter
        {
            get
            {
                return (HullRaceCollection)this.UIThreadGetValue(HullRaceFilterProperty);
            }
            set
            {
                this.UIThreadSetValue(HullRaceFilterProperty, value);
            }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(VesselDataObject),
            typeof(VesselDataControl), new PropertyMetadata(OnDataChanged));

        public VesselDataObject Data
        {
            get
            {
                return (VesselDataObject)this.UIThreadGetValue(DataProperty);
            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);
            }
        }



        void DoNew(object sender, ExecutedRoutedEventArgs e)
        {
            DoNew();
        }
        void DoSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            fsw.EnableRaisingEvents = false;
            DoSavePromptForFilename();

        }
        void DoOpen(object sender, ExecutedRoutedEventArgs e)
        {
            DoOpen();
        }
        void DoOpen()
        {
            if (!DoPrompt())
            {
                fsw.EnableRaisingEvents = false;
                OpenFileDialog diag = new OpenFileDialog();
                diag.Title = AMLResources.Properties.Resources.Title;
                diag.Filter = AMLResources.Properties.Resources.XML + ArtemisModLoader.DataStrings.XMLFilter
                    + "|" + AMLResources.Properties.Resources.AllFiles + ArtemisModLoader.DataStrings.AllFilesFilter;
                diag.Multiselect = false;
                diag.CheckFileExists = true;
                diag.InitialDirectory = Configuration.InstalledPath;

                if (diag.ShowDialog() == true)
                {
                    WorkFile = diag.FileName;

                    if (System.IO.File.Exists(WorkFile))
                    {

                        Data = XmlConverter.ToObject(WorkFile, typeof(VesselDataObject)) as VesselDataObject;
                        SetWatcher(WorkFile);

                    }
                    else
                    {
                        Data = new VesselDataObject();

                    }
                    if (Data != null)
                    {
                        Data.AcceptChanges();
                    }
                }
            }
        }
        void DoNew()
        {
            if (!DoPrompt())
            {
                Data = new VesselDataObject();
                
                fsw.EnableRaisingEvents = false;
                Data.AcceptChanges();
            }
        }
        bool DoPrompt()
        {
            bool retVal = false;
            if (Data.Changed)
            {
                switch (Locations.MessageBoxShow("Do you wish to save?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        DoSave();
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        retVal = true;
                        break;
                }
            }
            return retVal;
        }
        string WorkFile = null;


        private bool ValidateVesselData()
        {
            return Data.DoValidation();
        }
        public bool DoSave()
        {
            fsw.EnableRaisingEvents = false;
            if (!ValidateVesselData())
            {
                if (Locations.MessageBoxShow("Some vessels have invalid data. Please review the data.\r\n\r\nDo you continue saving?",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return false;
                }
            }

            Data.Save(WorkFile);
            SetWatcher(WorkFile);

            return true;
        }
        void DoSavePromptForFilename()
        {
            fsw.EnableRaisingEvents = false;
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = AMLResources.Properties.Resources.Title;
            diag.Filter = AMLResources.Properties.Resources.XML + ArtemisModLoader.DataStrings.XMLFilter
                + "|" + AMLResources.Properties.Resources.AllFiles + ArtemisModLoader.DataStrings.AllFilesFilter;
            diag.FileName = System.IO.Path.Combine(Configuration.InstalledPath, "dat", "vesselData.xml");
            diag.CheckPathExists = true;
            diag.AddExtension = true;
            diag.DefaultExt = ArtemisModLoader.DataStrings.XMLExtension;
            diag.InitialDirectory = System.IO.Path.Combine(Configuration.InstalledPath, "dat");
            if (diag.ShowDialog() == true)
            {
                WorkFile = diag.FileName;

                DoSave();

            }
        }


        private void Merge_click(object sender, RoutedEventArgs e)
        {
            fsw.EnableRaisingEvents = false;

            SelectMergeOption win = new SelectMergeOption();
            if (win.ShowDialog() == true)
            {

                OpenFileDialog diag = new OpenFileDialog();
                diag.Title = "Select vesselData file";
                diag.Filter = "VesselData files (*vesselData*.xml)|*vesselData*.xml|All Files|*.*";
                diag.CheckFileExists = true;
                diag.CheckPathExists = true;

                if (diag.ShowDialog() == true)
                {
                    VesselDataObject ImportObject = XmlConverter.ToObject(diag.FileName, typeof(VesselDataObject)) as VesselDataObject;
                    MergeRacesAndVessels(ImportObject, win.KeepSource, win.KeepTarget, win.Prompt);

                }
                SetWatcher(WorkFile);
            }
        }

        void MergeRacesAndVessels(VesselDataObject ImportObject, bool KeepSource, bool KeepTarget, bool Prompt)
        {

            List<DictionaryEntry> PromptEntriesRaces = new List<DictionaryEntry>();

            List<HullRace> matchedRacesTarget = new List<HullRace>();
            Dictionary<int, HullRace> matchedRacesSource = new Dictionary<int, HullRace>();
            Dictionary<int, HullRace> workRaceDictionary = new Dictionary<int, HullRace>();

            foreach (HullRace hr in ImportObject.HullRaces)
            {
                workRaceDictionary.Add(hr.ID, hr);
            }
            foreach (HullRace hr in Data.HullRaces)
            {
                if (workRaceDictionary.ContainsKey(hr.ID))
                {
                    matchedRacesTarget.Add(hr);
                    matchedRacesSource.Add(hr.ID, workRaceDictionary[hr.ID]);
                    workRaceDictionary.Remove(hr.ID);
                }
            }
            foreach (HullRace hr in workRaceDictionary.Values)
            {
                Data.HullRaces.Add(hr);
            }
            foreach (HullRace hr in matchedRacesTarget)
            {
                if (KeepSource)
                {
                    Data.HullRaces.Remove(hr);
                    Data.HullRaces.Add(matchedRacesSource[hr.ID]);
                }
                else if (KeepTarget)
                {

                }
                else if (Prompt)
                {
                    PromptEntriesRaces.Add(new DictionaryEntry(hr, matchedRacesSource[hr.ID]));
                }
            }


            List<DictionaryEntry> PromptEntriesVessels = new List<DictionaryEntry>();

            List<Vessel> matchedVesselsTarget = new List<Vessel>();
            Dictionary<int, Vessel> matchedVesselsSource = new Dictionary<int, Vessel>();
            Dictionary<int, Vessel> workVesselsDictionary = new Dictionary<int, Vessel>();

            foreach (Vessel hr in ImportObject.Vessels)
            {
                workVesselsDictionary.Add(hr.UniqueID, hr);
            }
            foreach (Vessel hr in Data.Vessels)
            {
                if (workVesselsDictionary.ContainsKey(hr.UniqueID))
                {
                    matchedVesselsTarget.Add(hr);
                    matchedVesselsSource.Add(hr.UniqueID, workVesselsDictionary[hr.UniqueID]);
                    workVesselsDictionary.Remove(hr.UniqueID);
                }
            }
            foreach (Vessel hr in workVesselsDictionary.Values)
            {
                Data.Vessels.Add(hr);
            }
            foreach (Vessel hr in matchedVesselsTarget)
            {
                if (KeepSource)
                {
                    Data.Vessels.Remove(hr);
                    Data.Vessels.Add(matchedVesselsSource[hr.UniqueID]);
                }
                else if (KeepTarget)
                {

                }
                else if (Prompt)
                {
                    PromptEntriesVessels.Add(new DictionaryEntry(hr, matchedVesselsSource[hr.UniqueID]));
                }
            }


            bool processed = true;
            if (PromptEntriesRaces.Count > 0 || PromptEntriesVessels.Count > 0)
            {
                MergeWindow win = new MergeWindow();
                win.RaceConflicts = new ObservableCollection<DictionaryEntry>(PromptEntriesRaces);
                win.VesselConflicts = new ObservableCollection<DictionaryEntry>(PromptEntriesVessels);
                win.TargetHullRaces = Data.HullRaces;

                win.SourceHullRaces = ImportObject.HullRaces;

                win.Configuration = this.Configuration;
                if (win.ShowDialog() == true)
                {
                    foreach (HullRace race in win.RaceResolved)
                    {
                        for (int i = 0; i < Data.HullRaces.Count; i++)
                        {
                            if (Data.HullRaces[i].ID == race.ID)
                            {
                                Data.HullRaces[i] = race;
                                break;
                            }
                        }
                    }
                    foreach (Vessel vessel in win.VesselResolved)
                    {
                        for (int i = 0; i < Data.Vessels.Count; i++)
                        {
                            if (Data.Vessels[i].UniqueID == vessel.UniqueID)
                            {
                                Data.Vessels[i] = vessel;
                                break;
                            }
                        }
                    }
                    processed = true;
                }
                else
                {
                    processed = false;
                    Locations.MessageBoxShow("Conflicts not resolved.  Current mod races and vessels kept.\r\n\r\nVessels and races which did not conflict were merged in.", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            if (processed)
            {
                Locations.MessageBoxShow("Files merged.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void SaveAs_click(object sender, RoutedEventArgs e)
        {
            DoSavePromptForFilename();
        }

        private void AddRace_click(object sender, RoutedEventArgs e)
        {
            HullRace race = new HullRace();
            race.ID = Data.HullRaces.Count;
            Data.HullRaces.Add(race);
        }

        private void AddVessel_click(object sender, RoutedEventArgs e)
        {
            Vessel v = new Vessel();
            v.UniqueID = Data.Vessels.Count;
            bool NotOK = false;
            do
            {
                foreach (Vessel v1 in Data.Vessels)
                {
                    if (v1.UniqueID == v.UniqueID)
                    {
                        v.UniqueID++;
                        NotOK = true;
                    }
                }
            } while (NotOK);

            Data.Vessels.Add(v);
            SelectedVessel = v;
        }

        private void DeleteRace_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                HullRace race = b.CommandParameter as HullRace;
                if (race != null)
                {
                    if (Locations.MessageBoxShow(AMLResources.Properties.Resources.AreYouSure, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        HullRaceCollection r = new HullRaceCollection();
                        foreach (HullRace oldr in Data.HullRaces)
                        {
                            if (oldr != race)
                            {
                                r.Add(oldr);
                            }
                        }
                        Data.HullRaces = r;
                        ValidateVesselData();
                    }
                }
            }
        }
        public static readonly DependencyProperty SelectedVesselProperty =
          DependencyProperty.Register("SelectedVessel", typeof(Vessel),
          typeof(VesselDataControl));

        public Vessel SelectedVessel
        {
            get
            {
                return (Vessel)this.UIThreadGetValue(SelectedVesselProperty);
            }
            set
            {
                this.UIThreadSetValue(SelectedVesselProperty, value);
            }
        }
        private void DeleteVessel_Click(object sender, RoutedEventArgs e)
        {

            Button b = sender as Button;
            if (b != null)
            {
                Vessel v = b.CommandParameter as Vessel;
                if (v != null)
                {
                    if (Locations.MessageBoxShow(AMLResources.Properties.Resources.AreYouSure, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        Data.Vessels.Remove(v);
                    }
                }
            }
        }
        

        void LoadFirstVesselDataFile()
        {
            //first find it.  Look in expected location, then search everywhere.

            

            string path = ArtemisModLoader.Helpers.FileHelper.LocateExpectedFileInMod("dat\\vesseldata.xml", Configuration);

            if (!string.IsNullOrEmpty(path))
            {
                Data = new VesselDataObject();
               

                XmlConverter.ToObject(path, Data);

                if (Data != null)
                {
                    Data.AcceptChanges();
                    WorkFile = path;
                    SetWatcher(WorkFile);
                }
               

            }
            else
            {


            }

        }
        static void OnConfigurationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

            VesselDataControl me = sender as VesselDataControl;
            if (me != null)
            {

                me.Dispatcher.BeginInvoke(new Action(me.LoadFirstVesselDataFile), System.Windows.Threading.DispatcherPriority.Render);
            }

        }
        public static readonly DependencyProperty ConfigurationProperty =
          DependencyProperty.Register("Configuration", typeof(ModConfiguration),
          typeof(VesselDataControl), new PropertyMetadata(OnConfigurationChanged));


        public ModConfiguration Configuration
        {
            get
            {
                return (ModConfiguration)this.UIThreadGetValue(ConfigurationProperty);

            }
            set
            {
                this.UIThreadSetValue(ConfigurationProperty, value);

            }
        }

        private void EditInNotepad_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(WorkFile) && System.IO.File.Exists(WorkFile))
            {
                ProcessStartInfo strt = new ProcessStartInfo(WorkFile);
                strt.UseShellExecute = true;
                strt.Verb = "Edit";
                Process.Start(strt);
            }
        }

        private void SortByVesselID_Click(object sender, RoutedEventArgs e)
        {
            if (Data.Vessels.SortType == VesselSortType.UniqueIDAscending)
            {
                Data.Vessels.SortType = VesselSortType.UniqueIDDescending;
            }
            else
            {
                Data.Vessels.SortType = VesselSortType.UniqueIDAscending;
            }
        }


        private void SortByRaceID_Click(object sender, RoutedEventArgs e)
        {
            if (Data.Vessels.SortType == VesselSortType.SideAscending)
            {
                Data.Vessels.SortType = VesselSortType.SideDescending;
            }
            else
            {
                Data.Vessels.SortType = VesselSortType.SideAscending;
            }
        }

      
        
        //////static void OnXmlEditorActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //////{
        //////    VesselDataControl me = sender as VesselDataControl;
        //////    if (me.xmlEditorActive)
        //////    {
        //////        me.DataToEditor();
        //////    }
        //////    else
        //////    {
        //////        me.EditorToData();
        //////    }
        //////}

        //////public static readonly DependencyProperty xmlEditorActiveProperty =
        ////// DependencyProperty.Register("xmlEditorActive", typeof(bool),
        ////// typeof(VesselDataControl), new PropertyMetadata(OnXmlEditorActiveChanged));


        //////public bool xmlEditorActive
        //////{
        //////    get
        //////    {
        //////        return (bool)this.UIThreadGetValue(xmlEditorActiveProperty);

        //////    }
        //////    set
        //////    {
        //////        this.UIThreadSetValue(xmlEditorActiveProperty, value);

        //////    }
        //////}
        //////void EditorToData()
        //////{
        //////    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        //////    FormattedXmlResult result =
        //////        RussLibrary.Controls.XmlEditor.FormatXml(xmlEditor.Content.Document.Text, true, true);
        //////    if (result.ResultCode <= FormattedXmlResultCode.Warnings)
        //////    {

        //////        doc.LoadXml(xmlEditor.Content.Document.Text);

        //////        XmlConverter.ToObject(doc, this.Data);
        //////        xmlEditor.DataFile = null;
             
        //////        if (result.ResultCode == FormattedXmlResultCode.Warnings)
        //////        {
        //////            Locations.MessageBoxShow(result.ErrorMessages, MessageBoxButton.OK, MessageBoxImage.Warning);
        //////        }
        //////    }
        //////    else
        //////    {
        //////        Locations.MessageBoxShow("Unable to convert data from editor:\r\n\r\n" + result.ErrorMessages, MessageBoxButton.OK, MessageBoxImage.Error);
        //////    }
        //////    SetWatcher(WorkFile);
        //////}
        //////void FileToEditor()
        //////{
        //////    xmlEditor.Load(WorkFile);
        //////    fsw.EnableRaisingEvents = false;
        //////    //SetWatcher(WorkFile);
        //////}
        //////void DataToEditor()
        //////{
        //////    System.Xml.XmlDocument doc = XmlConverter.ToXmlDocument(Data);
        //////    xmlEditor.Content.Clear();
        //////    xmlEditor.Content.Document.Text = doc.OuterXml;
        //////    xmlEditor.DataFile = WorkFile;
        //////    fsw.EnableRaisingEvents = false;
        //////   //// //doc.PreserveWhitespace = true;
        //////   //// string temp = System.IO.Path.GetTempFileName();
        //////   //////
        //////   ////// xmlEditor.Content.Document.Text = doc.OuterXml;

        //////   //// try
        //////   //// {
        //////   ////     StringBuilder sb = new StringBuilder();
        //////   ////     using (MemoryStream ms = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(
        //////   ////         doc.OuterXml.Replace("\t", string.Empty))))
        //////   ////     {
        //////   ////         bool OmitDeclaration = !doc.OuterXml.Trim().StartsWith("<?xml", StringComparison.OrdinalIgnoreCase);

        //////   ////         XmlWriterSettings settings = new XmlWriterSettings
        //////   ////         {

        //////   ////             NewLineHandling = NewLineHandling.Entitize,
        //////   ////             NewLineOnAttributes = false,
        //////   ////             Indent = true,
        //////   ////             IndentChars = "\t",
        //////   ////             NewLineChars = Environment.NewLine,
        //////   ////             ConformanceLevel = ConformanceLevel.Auto,
        //////   ////             OmitXmlDeclaration = OmitDeclaration
        //////   ////         };
        //////   ////         using (XmlReader reader = XmlReader.Create(ms))
        //////   ////         {

        //////   ////             using (XmlWriter writer = XmlWriter.Create(sb, settings))
        //////   ////             {
        //////   ////                 writer.WriteNode(reader, false);

        //////   ////             }
        //////   ////         }
        //////   ////         using (MemoryStream ms2 = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(sb.ToString())))
        //////   ////         {
        //////   ////             xmlEditor.Content.Load(ms2);
        //////   ////         }
        //////   ////     }
                
               
        //////   //// }
        //////   //// catch (Exception ex)
        //////   //// {
        //////   //// }

        //////}

        private void EditInXmlEditor_Click(object sender, RoutedEventArgs e)
        {
            EditorWindow.Show("Artemis Mod Loader VesselData.xml", WorkFile);
           
        }
        bool isDisposed = false;
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    fsw.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
