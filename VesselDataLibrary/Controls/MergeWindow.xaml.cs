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
using RussLibrary;
using System.Collections;
using System.Collections.ObjectModel;
using RussLibrary.WPF;
using VesselDataLibrary.Xml;
using ArtemisModLoader;
using ArtemisModLoader.Xml;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for MergeWindow.xaml
    /// </summary>
    public partial class MergeWindow : Window
    {
        public MergeWindow()
        {
            RaceResolved = new ObservableCollection<HullRace>();
            VesselResolved = new ObservableCollection<Vessel>();
            InitializeComponent();
            

            
            
        }
        public static readonly DependencyProperty ConfigurationProperty =
      DependencyProperty.Register("Configuration", typeof(ModConfiguration),
      typeof(MergeWindow));


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



        public static readonly DependencyProperty SourceHullRacesProperty =
           DependencyProperty.Register("SourceHullRaces", typeof(HullRaceCollection),
           typeof(MergeWindow));

        public HullRaceCollection SourceHullRaces
        {
            get
            {
                return (HullRaceCollection)this.UIThreadGetValue(SourceHullRacesProperty);
            }
            set
            {
                this.UIThreadSetValue(SourceHullRacesProperty, value);
            }
        }




        public static readonly DependencyProperty TargetHullRacesProperty =
           DependencyProperty.Register("TargetHullRaces", typeof(HullRaceCollection),
           typeof(MergeWindow));

        public HullRaceCollection TargetHullRaces
        {
            get
            {
                return (HullRaceCollection)this.UIThreadGetValue(TargetHullRacesProperty);
            }
            set
            {
                this.UIThreadSetValue(TargetHullRacesProperty, value);
            }
        }



        public static readonly DependencyProperty RaceConflictsProperty =
           DependencyProperty.Register("RaceConflicts", typeof(ObservableCollection<DictionaryEntry>),
           typeof(MergeWindow));

        public ObservableCollection<DictionaryEntry> RaceConflicts
        {
            get
            {
                return (ObservableCollection<DictionaryEntry>)this.UIThreadGetValue(RaceConflictsProperty);
            }
            set
            {
                this.UIThreadSetValue(RaceConflictsProperty, value);
            }
        }



        public static readonly DependencyProperty RaceResolvedProperty =
           DependencyProperty.Register("RaceResolved", typeof(ObservableCollection<HullRace>),
           typeof(MergeWindow));

        public ObservableCollection<HullRace> RaceResolved
        {
            get
            {
                return (ObservableCollection<HullRace>)this.UIThreadGetValue(RaceResolvedProperty);
            }
            set
            {
                this.UIThreadSetValue(RaceResolvedProperty, value);
            }
        }




        public static readonly DependencyProperty VesselConflictsProperty =
         DependencyProperty.Register("VesselConflicts", typeof(ObservableCollection<DictionaryEntry>),
         typeof(MergeWindow));

        public ObservableCollection<DictionaryEntry> VesselConflicts
        {
            get
            {
                return (ObservableCollection<DictionaryEntry>)this.UIThreadGetValue(VesselConflictsProperty);
            }
            set
            {
                this.UIThreadSetValue(VesselConflictsProperty, value);
            }
        }



        public static readonly DependencyProperty VesselResolvedProperty =
           DependencyProperty.Register("VesselResolved", typeof(ObservableCollection<Vessel>),
           typeof(MergeWindow));

        public ObservableCollection<Vessel> VesselResolved
        {
            get
            {
                return (ObservableCollection<Vessel>)this.UIThreadGetValue(VesselResolvedProperty);
            }
            set
            {
                this.UIThreadSetValue(VesselResolvedProperty, value);
            }
        }

        private void Target_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                DictionaryEntry entry = (DictionaryEntry)btn.CommandParameter;
                ChangeDependencyObject obj = entry.Key as ChangeDependencyObject;
                if (obj != null)
                {
                    obj.Tag = btn.Tag;
                }

            }
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            bool isInvalid = false;
            List<DictionaryEntry> raceConflictsToRemove = new List<DictionaryEntry>();

            foreach (DictionaryEntry entry in RaceConflicts)
            {
                HullRace race = (HullRace)entry.Key;
                
                if (race.Tag == null)
                {
                    isInvalid = true;
                }
                else
                {
                    if (race.Tag.ToString() == "Source")
                    {
                        race = (HullRace)entry.Value;
                    }
                    RaceResolved.Add(race);
                    raceConflictsToRemove.Add(entry);
                }
            }
            List<DictionaryEntry> vesselConflictsToRemove = new List<DictionaryEntry>();
            foreach (DictionaryEntry entry in VesselConflicts)
            {

                Vessel vessel = (Vessel)entry.Key;

                if (vessel.Tag == null)
                {
                    isInvalid = true;
                }
                else
                {
                    if (vessel.Tag.ToString() == "Source")
                    {
                        vessel = (Vessel)entry.Value;
                    }
                    VesselResolved.Add(vessel);
                    vesselConflictsToRemove.Add(entry);
                }
            }
            foreach (DictionaryEntry entry in raceConflictsToRemove)
            {
                RaceConflicts.Remove(entry);
            }
            foreach (DictionaryEntry entry in vesselConflictsToRemove)
            {
                VesselConflicts.Remove(entry);
            }
            if (isInvalid)
            {
                Locations.MessageBoxShow("Some conflicts remain unresolved.  Please resolve them.", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                DialogResult = true;
                this.Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.RemoveClose();
            this.SetMaxSize();
        }

    }
}
