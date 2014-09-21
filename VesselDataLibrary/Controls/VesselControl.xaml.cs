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
using ArtemisModLoader.Xml;
using RussLibrary;
using VesselDataLibrary.Xml;
using ArtemisModLoader;
using Microsoft.Win32;
using System.IO;
using ArtemisModLoader.Helpers;
using System.Collections.ObjectModel;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for VesselControl.xaml
    /// </summary>
    public partial class VesselControl : UserControl
    {
        public VesselControl()
        {
            InitializeComponent();
          
        }
        public static readonly DependencyProperty SearchPrefixesProperty =
          DependencyProperty.Register("SearchPrefixes", typeof(ObservableCollection<string>),
          typeof(VesselControl));

        public ObservableCollection<string> SearchPrefixes
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(SearchPrefixesProperty);

            }
            set
            {
                this.UIThreadSetValue(SearchPrefixesProperty, value);

            }
        }


        public static readonly DependencyProperty HullRacesProperty =
           DependencyProperty.Register("HullRaces", typeof(HullRaceCollection),
           typeof(VesselControl));

        public HullRaceCollection HullRaces
        {
            get
            {
                return (HullRaceCollection)this.UIThreadGetValue(HullRacesProperty);
            }
            set
            {
                this.UIThreadSetValue(HullRacesProperty, value);
            }
        }
        static void OnWallRatioChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VesselControl me = sender as VesselControl;
            if (me != null)
            {
                foreach (BeamPort bp in me.Data.BeamPorts)
                {
                    double wallRatio = me.beamExpander.ActualHeight / bp.Range / (me.Data.BeamPorts.Count);
                    if (wallRatio < me.WallRatio)
                    {
                        me.WallRatio = wallRatio;
                        
                    }
                }
            }
        }
        public static readonly DependencyProperty WallRatioProperty =
        DependencyProperty.Register("WallRatio", typeof(double),
        typeof(VesselControl), new PropertyMetadata(0.05D, OnWallRatioChanged));

        public double WallRatio
        {
            get
            {
                return (double)this.UIThreadGetValue(WallRatioProperty);

            }
            set
            {
                this.UIThreadSetValue(WallRatioProperty, value);

            }
        }
        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(Vessel),
           typeof(VesselControl));

        public Vessel Data
        {
            get
            {
                return (Vessel)this.UIThreadGetValue(DataProperty);
            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);
            }
        }
        static void OnConfigurationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VesselControl v = sender as VesselControl;
            if (v != null && v.Configuration != null)
            {
                v.SearchPrefixes = new ObservableCollection<string>(ModManagement.SearchPrefixes(v.Configuration));
            }
        }
        public static readonly DependencyProperty ConfigurationProperty =
         DependencyProperty.Register("Configuration", typeof(ModConfiguration),
         typeof(VesselControl), new PropertyMetadata(OnConfigurationChanged));


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

        private void AddBroadType_Click(object sender, RoutedEventArgs e)
        {
            Data.BroadTypeList.Insert(0, string.Empty);
            Data.BroadType = Data.BroadTypeList[0];
        }

        private void FileSelectionControl_InvalidFilePath(object sender, RoutedEventArgs e)
        {
            FileHelper.FileSelectionControl_InvalidFilePath(sender, e, Configuration);

        }

        private void AddBeamPort_click(object sender, RoutedEventArgs e)
        {
            BeamPort bp = new BeamPort();
            this.Data.BeamPorts.Add(bp);
    
            beamExpander.IsExpanded = true;
        }
        void AdjustWallRatio()
        {
            double LargestRange = 0;

            foreach (BeamPort bp in Data.BeamPorts)
            {
                if (bp.Range > LargestRange)
                {
                    LargestRange = bp.Range;
                }
            }
            WallRatio = 200 / (LargestRange * 2);

        }
        private void DeleteBeamPort_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                BeamPort bp = btn.CommandParameter as BeamPort;
                if (btn != null)
                {
                    this.Data.BeamPorts.Remove(bp);
                    AdjustWallRatio();
                }
            }
        }

        private void AddTubePort_click(object sender, RoutedEventArgs e)
        {
            VectorObject obj = new VectorObject();
            this.Data.TorpedoTubes.Add(obj);
            torptupeExpander.IsExpanded = true;
        }

        private void DeleteTubePort_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                VectorObject bp = btn.CommandParameter as VectorObject;
                if (btn != null)
                {
                    this.Data.TorpedoTubes.Remove(bp);
                }
            }
        }

        private void AddEnginePort_click(object sender, RoutedEventArgs e)
        {
            VectorObject obj = new VectorObject();
            this.Data.EnginePorts.Add(obj);
            EnginePortExpander.IsExpanded = true;
        }

        private void DeleteEnginePort_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                VectorObject bp = btn.CommandParameter as VectorObject;
                if (btn != null)
                {
                    this.Data.EnginePorts.Remove(bp);
                }
            }
        }

        private void AddImpulsePoint_click(object sender, RoutedEventArgs e)
        {
            VectorObject obj = new VectorObject();
            this.Data.ImpulsePoints.Add(obj);
            impulseExpander.IsExpanded = true;
        }

        private void DeleteImpulsePoint_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                VectorObject bp = btn.CommandParameter as VectorObject;
                if (btn != null)
                {
                    this.Data.ImpulsePoints.Remove(bp);
                }
            }
        }

        private void AddManeuverPoint_click(object sender, RoutedEventArgs e)
        {
            VectorObject obj = new VectorObject();
            this.Data.ManeuverPoints.Add(obj);
            ManeuverExpander.IsExpanded = true;
        }

        private void DeleteManeuverPoint_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                VectorObject bp = btn.CommandParameter as VectorObject;
                if (btn != null)
                {
                    this.Data.ManeuverPoints.Remove(bp);
                }
            }
        }

        private void AddDronePort_click(object sender, RoutedEventArgs e)
        {
            DronePort obj = new DronePort();
            this.Data.DronePorts.Add(obj);
           
            if (!droneExpander.IsExpanded)
            {

                droneExpander.IsExpanded = true;

            }
           
        }

        private void DeleteDronePort_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                DronePort bp = btn.CommandParameter as DronePort;
                if (btn != null)
                {
                    this.Data.DronePorts.Remove(bp);
                    //bp.VectorItemChanged -= new EventHandler(bp_VectorItemChanged);
                    //AdjustWallRatio();
                }
            }
        }
       

        void bp_VectorItemChanged(object sender, EventArgs e)
        {
            AdjustWallRatio();  
        }
        
        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
         
        }

        private void beamExpander_Expanded(object sender, RoutedEventArgs e)
        {
           
        }

        private void BeamPortControl_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustWallRatio();
            BeamPortControl bp = sender as BeamPortControl;
            if (bp != null)
            {
                bp.Beam.VectorItemChanged += new EventHandler(bp_VectorItemChanged);
            }
        }

        private void BeamPortControl_Unloaded(object sender, RoutedEventArgs e)
        {
            BeamPortControl bp = sender as BeamPortControl;
            if (bp != null)
            {
                bp.Beam.VectorItemChanged -= new EventHandler(bp_VectorItemChanged);
            }
        }

    }
}
