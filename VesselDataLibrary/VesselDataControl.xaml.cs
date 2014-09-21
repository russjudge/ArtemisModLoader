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
using RussLibrary.Xml;
using System.Xml;
using Microsoft.Win32;
using ArtemisModLoader;
using VesselDataLibrary.Xml;
namespace VesselDataLibrary
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class VesselDataControl : UserControl
    {

        public VesselDataControl()
        {
            
            InitializeComponent();
        }
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(VesselDataObject),
            typeof(VesselDataControl));

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

        private void New_click(object sender, RoutedEventArgs e)
        {
            Data = new VesselDataObject();
            Data.EndInit();
        }
        string WorkFile = null;
        private void Open_click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Title = AMLResources.Properties.Resources.Title;
            diag.Filter = AMLResources.Properties.Resources.XML + ArtemisModLoader.DataStrings.XMLFilter
                + "|" + AMLResources.Properties.Resources.AllFiles + ArtemisModLoader.DataStrings.AllFilesFilter;
            diag.Multiselect = false;
            diag.CheckFileExists = true;

            if (diag.ShowDialog() == true)
            {
                WorkFile = diag.FileName;
                if (System.IO.File.Exists(WorkFile))
                {
                    XmlDocument doc = XmlConverter.LoadXmlFile(WorkFile);

                    if (doc != null)
                    {
                        Data = XmlConverter.ToObject(doc, typeof(VesselDataObject)) as VesselDataObject;
                        
                    }
                }
                else
                {
                    Data = new VesselDataObject();
                    
                }
                if (Data != null)
                {
                    Data.EndInit();
                }
            }
        }
        void DoSave()
        {
            Data.Version = ArtemisModLoader.DataStrings.VesselDataCurrentVersion;
            XmlDocument doc = XmlConverter.ToXmlDocument(Data);

            doc.Save(WorkFile);
        }
        void DoSavePromptForFilename()
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = AMLResources.Properties.Resources.Title;
            diag.Filter = AMLResources.Properties.Resources.XML + ArtemisModLoader.DataStrings.XMLFilter
                + "|" + AMLResources.Properties.Resources.AllFiles + ArtemisModLoader.DataStrings.AllFilesFilter;
            diag.CheckFileExists = true;
            diag.CheckPathExists = true;
            diag.AddExtension = true;
            diag.DefaultExt = ArtemisModLoader.DataStrings.XMLExtension;
            if (diag.ShowDialog() == true)
            {
                WorkFile = diag.FileName;
                DoSave();
            }
        }
        private void Save_click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(WorkFile))
            {
                DoSavePromptForFilename();
            }
            else
            {
                DoSave();
            }
            
        }

        private void Merge_click(object sender, RoutedEventArgs e)
        {
            Locations.MessageBoxShow("Not yet implemented.", MessageBoxButton.OK, MessageBoxImage.Hand);
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
            Data.Vessels.Add(v);
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
                        Data.HullRaces.Remove(race);
                    }
                }
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


    }
}
