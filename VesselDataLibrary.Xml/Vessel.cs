using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using log4net;
using RussLibrary;
using RussLibrary.WPF;
using RussLibrary.Xml;
using System.Collections.Generic;
using System.Xml;
namespace VesselDataLibrary.Xml
{
    //[XmlConversionRoot("vessel", false)]
    [XmlConversionRoot("vessel")]
    public class Vessel : ChangeDependencyObject, IXmlStorage
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Vessel));
        public Vessel()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Storage = new List<XmlNode>();
            Art = new ArtDefinitionCollection();
            Art.Add(new ArtDefinition());
            Art.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            InternalDefinition = new InternalData();
            InternalDefinition.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            Shields = new ShieldData();
            Shields.ObjectChanged += new System.EventHandler(Art_ObjectChanged);

            Performance = new PerformanceData();
            Performance.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            BeamPorts = new BeamPortCollection();
            BeamPorts.ObjectChanged+=new System.EventHandler(Art_ObjectChanged);

            Torpedoes = new TorpedoStorageCollection();
            Torpedoes.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            TorpedoTubes = new VectorObjectCollection();
            TorpedoTubes.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            EnginePorts = new VectorObjectCollection();
            EnginePorts.ObjectChanged += new System.EventHandler(Art_ObjectChanged);


            Description = new DescriptionObject();
            Description.ObjectChanged += new System.EventHandler(Art_ObjectChanged);


            ManeuverPoints = new VectorObjectCollection();
            ManeuverPoints.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            ImpulsePoints = new VectorObjectCollection();
            ImpulsePoints.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            FleetAICommonality = new FleetAI();
            FleetAICommonality.ObjectChanged += new System.EventHandler(Art_ObjectChanged);

            
            DronePorts = new DronePortCollection();
            DronePorts.ObjectChanged+=new System.EventHandler(Art_ObjectChanged);

            Carrier = new Carrier();
            Carrier.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            ProductionCoefficient = new Production();
            ProductionCoefficient.ObjectChanged += new System.EventHandler(Art_ObjectChanged);
            if (BroadTypeList != null)
            {
                BroadType = BroadTypeList[0];
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        

        void Art_ObjectChanged(object sender, System.EventArgs e)
        {
            SetChanged();
        }
        public override void AcceptChanges()
        {
            if (Art != null)
            {
                Art.AcceptChanges();
            }
            if (FleetAICommonality != null)
            {
                FleetAICommonality.AcceptChanges();
            }
            if (Carrier != null)
            {
                Carrier.AcceptChanges();
            }
            if (InternalDefinition != null)
            {
                InternalDefinition.AcceptChanges();
            }
            Shields.AcceptChanges();
            if (Performance != null)
            {
                Performance.AcceptChanges();
            }
            if (BeamPorts != null)
            {
                BeamPorts.AcceptChanges();
                
            }
            if (DronePorts != null)
            {
                DronePorts.AcceptChanges();
                
            }
            if (Torpedoes != null)
            {
                Torpedoes.AcceptChanges();
            }


            if (TorpedoTubes != null)
            {
                TorpedoTubes.AcceptChanges();
            }
            if (EnginePorts != null)
            {
                EnginePorts.AcceptChanges();
            }

            if (ManeuverPoints != null)
            {
                ManeuverPoints.AcceptChanges();
            }

            if (ImpulsePoints != null)
            {
                ImpulsePoints.AcceptChanges();
            }


            if (Description != null)
            {
                Description.AcceptChanges();
            }
            base.AcceptChanges();
        }
        public override void RejectChanges()
        {
            if (Art != null)
            {
                Art.RejectChanges();
            }
            if (FleetAICommonality != null)
            {
                FleetAICommonality.RejectChanges();
            }
            if (Carrier != null)
            {
                Carrier.RejectChanges();
            }
            if (InternalDefinition != null)
            {
                InternalDefinition.RejectChanges();
            }
            Shields.RejectChanges();
            if (Performance != null)
            {
                Performance.RejectChanges();
            }
            if (BeamPorts != null)
            {
                BeamPorts.RejectChanges();
               
            }
            if (DronePorts != null)
            {
               DronePorts.RejectChanges();
                
            }
            if (Torpedoes != null)
            {
                Torpedoes.RejectChanges();
            }


            if (TorpedoTubes != null)
            {
                TorpedoTubes.RejectChanges();
            }
            if (EnginePorts != null)
            {
                EnginePorts.RejectChanges();
            }

            if (ManeuverPoints != null)
            {
                ManeuverPoints.RejectChanges();
            }

            if (ImpulsePoints != null)
            {
                ImpulsePoints.RejectChanges();
            }
            if (Description != null)
            {
                Description.RejectChanges();
            }
            base.RejectChanges();
        }
       

        public override void BeginInitialization()
        {
            
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            base.BeginInitialization();
            if (FleetAICommonality != null)
            {
                FleetAICommonality.BeginInitialization();
            }
            if (Carrier != null)
            {
                Carrier.BeginInitialization();
            }
            if (Art != null)
            {
                Art.BeginInitialization();
            }
            if (InternalDefinition != null)
            {
                InternalDefinition.BeginInitialization();
            }
            Shields.BeginInitialization();
            if (Performance != null)
            {
                Performance.BeginInitialization();
            }
            if (DronePorts != null)
            {
                DronePorts.BeginInitialization();
                
            }
            if (BeamPorts != null)
            {
                BeamPorts.BeginInitialization();

            }
   


            if (TorpedoTubes != null)
            {
                TorpedoTubes.BeginInitialization();
            }
            if (EnginePorts != null)
            {
                EnginePorts.BeginInitialization();
            }

            if (ManeuverPoints != null)
            {
                ManeuverPoints.BeginInitialization();
            }

            if (ImpulsePoints != null)
            {
                ImpulsePoints.BeginInitialization();
            }

            if (Torpedoes != null)
            {
                Torpedoes.BeginInitialization();
            }
            if (Description != null)
            {
                Description.BeginInitialization();
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        
        public override void  EndInitialization()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (Art != null)
            {
                Art.EndInitialization();
            }
            if (FleetAICommonality != null)
            {
                FleetAICommonality.EndInitialization();
            }
            if (InternalDefinition != null)
            {
                InternalDefinition.EndInitialization();
            }
            if (Carrier != null)
            {
                Carrier.EndInitialization();
            }
            Shields.EndInitialization();
            if (Performance != null)
            {
                Performance.EndInitialization();
            }
            if (DronePorts != null)
            {
                DronePorts.EndInitialization();
                
            }
            if (BeamPorts != null)
            {
                BeamPorts.EndInitialization();
                
            }
            if (TorpedoTubes != null)
            {
                TorpedoTubes.EndInitialization();
            }
            if (EnginePorts != null)
            {
                EnginePorts.EndInitialization();
            }

            if (ManeuverPoints != null)
            {
                ManeuverPoints.EndInitialization();
            }

            if (ImpulsePoints != null)
            {
                ImpulsePoints.EndInitialization();
            }

           

            

            if (Torpedoes != null)
            {
                Torpedoes.EndInitialization();
            }
            if (Description != null)
            {
                Description.EndInitialization();
            }
            base.EndInitialization();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
        /*
       * Sample Xml from file:
       *
<vessel    uniqueID="0"    side="0"       classname="Light Cruiser" broadType="player">
  <art     meshfile="dat/artemis.dxs"    diffuseFile="dat/artemis_diffuse.png"    
           glowFile="dat/artemis_illum.png"    specularFile="dat/artemis_specular.png" scale="0.2" pushRadius="150"/>
  <internal_data file="dat/artemis.snt" />
  <shields front="80" back="80" />
  <performance turnrate="0.004" topspeed="0.6" />
  <beam_port x="-102.14" y="8.35" z="258.74" damage="12" arcwidth="0.4" cycletime="6.0" range="1000"/>
  <beam_port x=" 102.14" y="8.35" z="258.74" damage="12" arcwidth="0.4" cycletime="6.0" range="1000"/>
  <torpedo_tube x="0" y="8.35" z="258.74"/>
  <torpedo_tube x="0" y="8.35" z="258.74"/>
  <torpedo_storage type="0" amount="8" />  <!-- Type 1 Homing"-->
  <torpedo_storage type="1" amount="2" />  <!-- Type 4 LR Nuke-->
  <torpedo_storage type="2" amount="6" />  <!-- Type 6 Mine"-->
  <torpedo_storage type="3" amount="4" />  <!-- Type 9 ECM"-->
  <engine_port x=" 82.93" y="5" z="-240.89" />
  <engine_port x="-82.93" y="5" z="-240.89" />
  <engine_port x="0" y="-9.22" z="-300" />
  <engine_port x="0" y="29.64" z="-300" />
  <long_desc text="USFP Cruiser^Standard long patrol vessel of the USFP.^2 forward beams^2 Torpedo tubes^Stores for 2 nukes, 8 homing, 6 mines, 4 ECM." />
</vessel>
      
       * */




        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
        public static readonly DependencyProperty UniqueIDProperty =
         DependencyProperty.Register("UniqueID", typeof(int),
         typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID"), XmlConversion("uniqueID")]
        public int UniqueID
        {
            get
            {
                return (int)this.UIThreadGetValue(UniqueIDProperty);

            }
            set
            {
                this.UIThreadSetValue(UniqueIDProperty, value);

            }
        }




        public static readonly DependencyProperty SideProperty =
         DependencyProperty.Register("Side", typeof(int),
         typeof(Vessel));
        [XmlConversion("side")]
        public int Side
        {
            get
            {
                return (int)this.UIThreadGetValue(SideProperty);

            }
            set
            {
                this.UIThreadSetValue(SideProperty, value);

            }
        }



        public static readonly DependencyProperty ClassNameProperty =
         DependencyProperty.Register("ClassName", typeof(string),
         typeof(Vessel));
        [XmlConversion("classname")]
        public string ClassName
        {
            get
            {
                return (string)this.UIThreadGetValue(ClassNameProperty);

            }
            set
            {
                this.UIThreadSetValue(ClassNameProperty, value);

            }
        }

        static void OnUserBroadTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Vessel me = sender as Vessel;
            if (me != null)
            {
                if (!me.BroadTypeChanging)
                {
                    me.BroadTypeChanging = true;
                    me.BroadType = (me.UserBroadType
                        + " " + (me.IsWarship ? "warship" : "")
                        + " " + (me.IsAnomalyEater ? "anomalyeater" : "")
                        + " " + (me.IsAsteroidEater ? "asteroideater" : "")
                        + " " + (me.IsSentient ? "sentient" : "")).Trim();
                    while (me.BroadType.Contains("  "))
                    {
                        me.BroadType = me.BroadType.Replace("  ", " ");
                    }
                    me.BroadTypeChanging = false;
                }
            }
        }


        public static readonly DependencyProperty IsWarshipProperty =
          DependencyProperty.Register("IsWarship", typeof(bool),
          typeof(Vessel), new PropertyMetadata(OnUserBroadTypeChanged));

        public bool IsWarship
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsWarshipProperty);

            }
            set
            {
                this.UIThreadSetValue(IsWarshipProperty, value);

            }
        }


        public static readonly DependencyProperty IsSentientProperty =
          DependencyProperty.Register("IsSentient", typeof(bool),
          typeof(Vessel), new PropertyMetadata(OnUserBroadTypeChanged));

        public bool IsSentient
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsSentientProperty);

            }
            set
            {
                this.UIThreadSetValue(IsSentientProperty, value);

            }
        }


        public static readonly DependencyProperty IsAsteroidEaterProperty =
          DependencyProperty.Register("IsAsteroidEater", typeof(bool),
          typeof(Vessel), new PropertyMetadata(OnUserBroadTypeChanged));

        public bool IsAsteroidEater
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsAsteroidEaterProperty);

            }
            set
            {
                this.UIThreadSetValue(IsAsteroidEaterProperty, value);

            }
        }



        public static readonly DependencyProperty IsAnomalyEaterProperty =
          DependencyProperty.Register("IsAnomalyEater", typeof(bool),
          typeof(Vessel), new PropertyMetadata(OnUserBroadTypeChanged));

        public bool IsAnomalyEater
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsAnomalyEaterProperty);

            }
            set
            {
                this.UIThreadSetValue(IsAnomalyEaterProperty, value);

            }
        }
        public static readonly DependencyProperty UserBroadTypeProperty =
            DependencyProperty.Register("UserBroadType", typeof(string),
            typeof(Vessel), new PropertyMetadata(OnUserBroadTypeChanged));

        public string UserBroadType
        {
            get
            {
                return (string)this.UIThreadGetValue(UserBroadTypeProperty);

            }
            set
            {
                this.UIThreadSetValue(UserBroadTypeProperty, value);

            }
        }

        bool BroadTypeChanging = false;

        static void OnBroadTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Vessel me = sender as Vessel;
            if (me != null)
            {
                //"anomalyeater", "asteroideater", "sentient"
                if (!me.BroadTypeChanging)
                {
                    me.BroadTypeChanging = true;
                    me.IsAnomalyEater = me.BroadType.Contains("anomalyeater");
                    me.IsAsteroidEater = me.BroadType.Contains("asteroideater");
                    me.IsSentient = me.BroadType.Contains("sentient");
                    me.IsWarship = me.BroadType.Contains("warship");
                    string[] work = me.BroadType.Split(' ');
                    foreach (string w in work)
                    {
                        if (!string.IsNullOrEmpty(w) && w != "sentient" && w != "anomalyeater" && w != "asteroideater" && w != "warship")
                        {
                            me.UserBroadType = w;
                        }
                    }
                    me.BroadTypeChanging = false;
                }
            }
        }
        public static readonly DependencyProperty ProductionCoefficientProperty =
            DependencyProperty.Register("ProductionCoefficient", typeof(Production),
            typeof(Vessel));
        [XmlConversion("production")]
        public Production ProductionCoefficient
        {
            get
            {
                return (Production)this.UIThreadGetValue(ProductionCoefficientProperty);

            }
            set
            {
                this.UIThreadSetValue(ProductionCoefficientProperty, value);

            }
        }



        public static readonly DependencyProperty BroadTypeProperty =
            DependencyProperty.Register("BroadType", typeof(string),
            typeof(Vessel), new PropertyMetadata(OnBroadTypeChanged));
        [XmlConversion("broadType")]
        public string BroadType
        {
            get
            {
                return (string)this.UIThreadGetValue(BroadTypeProperty);

            }
            set
            {
                this.UIThreadSetValue(BroadTypeProperty, value);

            }
        }


        public static readonly DependencyProperty BroadTypeListProperty =
            DependencyProperty.Register("BroadTypeList", typeof(ObservableCollection<string>),
            typeof(Vessel), new PropertyMetadata(new ObservableCollection<string>(
                new string[] {"small", "medium", "large", "carrier", "fighter", "player", "base", "warship", "science", "cargo", "luxury", "transport"})));
    
        public ObservableCollection<string> BroadTypeList
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(BroadTypeListProperty);

            }
            private set
            {
                this.UIThreadSetValue(BroadTypeListProperty, value);

            }
        }


        public static readonly DependencyProperty DescriptionProperty =
           DependencyProperty.Register("Description", typeof(DescriptionObject),
           typeof(Vessel));
        [XmlConversion("long_desc")]
        public DescriptionObject Description
        {
            get
            {
                return (DescriptionObject)this.UIThreadGetValue(DescriptionProperty);
            }
            set
            {
                this.UIThreadSetValue(DescriptionProperty, value);
            }
        }




        public static readonly DependencyProperty FleetAICommonalityProperty =
         DependencyProperty.Register("FleetAICommonality", typeof(FleetAI),
         typeof(Vessel));
        [XmlConversion("fleet_ai")]
        public FleetAI FleetAICommonality
        {
            get
            {
                return (FleetAI)this.UIThreadGetValue(FleetAICommonalityProperty);

            }
            set
            {
                this.UIThreadSetValue(FleetAICommonalityProperty, value);

            }
        }
        public static readonly DependencyProperty CarrierProperty =
        DependencyProperty.Register("Carrier", typeof(Carrier),
        typeof(Vessel));
        [XmlConversion("carrier")]
        public Carrier Carrier
        {
            get
            {
                return (Carrier)this.UIThreadGetValue(CarrierProperty);

            }
            set
            {
                this.UIThreadSetValue(CarrierProperty, value);

            }
        }
        public static readonly DependencyProperty ArtProperty =
            DependencyProperty.Register("Art", typeof(ArtDefinitionCollection),
            typeof(Vessel));
        [XmlConversion("art")]
        public ArtDefinitionCollection Art
        {
            get
            {
                return (ArtDefinitionCollection)this.UIThreadGetValue(ArtProperty);

            }
            private set
            {
                this.UIThreadSetValue(ArtProperty, value);

            }
        }



        public static readonly DependencyProperty ShieldsProperty =
            DependencyProperty.Register("Shields", typeof(ShieldData),
            typeof(Vessel));
        [XmlConversion("shields")]
        public ShieldData Shields
        {
            get
            {
                return (ShieldData)this.UIThreadGetValue(ShieldsProperty);
            }
            set
            {
                this.UIThreadSetValue(ShieldsProperty, value);
            }
        }

        public static readonly DependencyProperty PerformanceProperty =
           DependencyProperty.Register("Performance", typeof(PerformanceData),
           typeof(Vessel));
        [XmlConversion("performance")]
        public PerformanceData Performance
        {
            get
            {
                return (PerformanceData)this.UIThreadGetValue(PerformanceProperty);
            }
            set
            {
                this.UIThreadSetValue(PerformanceProperty, value);
            }
        }


        public static readonly DependencyProperty InternalDefinitionProperty =
            DependencyProperty.Register("InternalDefinition", typeof(InternalData),
            typeof(Vessel));
        [XmlConversion("internal_data")]
        public InternalData InternalDefinition
        {
            get
            {
                return (InternalData)this.UIThreadGetValue(InternalDefinitionProperty);

            }
            set
            {
                this.UIThreadSetValue(InternalDefinitionProperty, value);

            }
        }

        public static readonly DependencyProperty BeamPortsProperty =
           DependencyProperty.Register("BeamPorts", typeof(BeamPortCollection),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"),
        XmlConversion("beam_port")]
        public BeamPortCollection BeamPorts
        {
            get
            {
                return (BeamPortCollection)this.UIThreadGetValue(BeamPortsProperty);
            }
            set
            {
                this.UIThreadSetValue(BeamPortsProperty, value);
            }
        }



        public static readonly DependencyProperty DronePortsProperty =
           DependencyProperty.Register("DronePorts", typeof(DronePortCollection),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"),
        XmlConversion("drone_port")]
        public DronePortCollection DronePorts
        {
            get
            {
                return (DronePortCollection)this.UIThreadGetValue(DronePortsProperty);
            }
            set
            {
                this.UIThreadSetValue(DronePortsProperty, value);
            }
        }



        public static readonly DependencyProperty ManeuverPointsProperty =
           DependencyProperty.Register("ManeuverPoints", typeof(VectorObjectCollection),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("maneuver_point")]
        public VectorObjectCollection ManeuverPoints
        {
            get
            {
                return (VectorObjectCollection)this.UIThreadGetValue(ManeuverPointsProperty);
            }
            set
            {
                this.UIThreadSetValue(ManeuverPointsProperty, value);
            }
        }

        public static readonly DependencyProperty ImpulsePointsProperty =
           DependencyProperty.Register("ImpulsePoints", typeof(VectorObjectCollection),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("impulse_point")]
        public VectorObjectCollection ImpulsePoints
        {
            get
            {
                return (VectorObjectCollection)this.UIThreadGetValue(ImpulsePointsProperty);
            }
            set
            {
                this.UIThreadSetValue(ImpulsePointsProperty, value);
            }
        }


        public static readonly DependencyProperty TorpedoTubesProperty =
           DependencyProperty.Register("TorpedoTubes", typeof(VectorObjectCollection),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("torpedo_tube")]
        public VectorObjectCollection TorpedoTubes
        {
            get
            {
                return (VectorObjectCollection)this.UIThreadGetValue(TorpedoTubesProperty);
            }
            set
            {
                this.UIThreadSetValue(TorpedoTubesProperty, value);
            }
        }




        public static readonly DependencyProperty TorpedoesProperty =
           DependencyProperty.Register("Torpedoes", typeof(TorpedoStorageCollection),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("torpedo_storage")]
        public TorpedoStorageCollection Torpedoes
        {
            get
            {
                return (TorpedoStorageCollection)this.UIThreadGetValue(TorpedoesProperty);
            }
            set
            {
                this.UIThreadSetValue(TorpedoesProperty, value);
            }
        }



        public static readonly DependencyProperty EnginePortsProperty =
           DependencyProperty.Register("EnginePorts", typeof(VectorObjectCollection),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("engine_port")]
        public VectorObjectCollection EnginePorts
        {
            get
            {
                return (VectorObjectCollection)this.UIThreadGetValue(EnginePortsProperty);
            }
            set
            {
                this.UIThreadSetValue(EnginePortsProperty, value);
            }
        }




        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        protected override void ProcessValidation()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (!string.IsNullOrEmpty(BroadType))
            {
                if (BroadType.ToLowerInvariant() != BroadType)
                {
                    BroadType = BroadType.ToLowerInvariant();
                }
                string[] wrk = BroadType.Split(' ');

                foreach (string w in wrk)
                {
                    if (!string.IsNullOrEmpty(w) && w != "anomalyeater" && w != "asteroideater" && w != "sentient")
                    {
                        if (!BroadTypeList.Contains(w))
                        {
                            BroadTypeList.Add(w);
                        }
                    }
                }
                
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        public IList<XmlNode> Storage { get; private set; }
    }
}
