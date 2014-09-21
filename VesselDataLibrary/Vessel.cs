using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RussLibrary.Xml;
using System.Windows;
using RussLibrary;
using System.Collections.ObjectModel;
namespace VesselDataLibrary
{
    [XmlConversionRoot("vessel")]
    public class Vessel : DependencyObject
    {
        public Vessel()
        {
            Art = new ArtDefinition();
            InternalDefinition = new InternalData();
            Shields = new ShieldData();
            Performance = new PerformanceData();
            BeamPorts = new ObservableCollection<BeamPort>();
            Torpedoes = new ObservableCollection<TorpedoStorage>();
            TorpedoTubes = new ObservableCollection<VectorObject>();
            EnginePorts = new ObservableCollection<VectorObject>();
            Description = new DescriptionObject();

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

        public static readonly DependencyProperty BroadTypeProperty =
            DependencyProperty.Register("BroadType", typeof(string),
            typeof(Vessel));
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

        public static readonly DependencyProperty ArtProperty =
            DependencyProperty.Register("Art", typeof(ArtDefinition),
            typeof(Vessel));
        [XmlConversion("art")]
        public ArtDefinition Art
        {
            get
            {
                return (ArtDefinition)this.UIThreadGetValue(ArtProperty);

            }
            set
            {
                this.UIThreadSetValue(ArtProperty, value);

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


        public static readonly DependencyProperty BeamPortsProperty =
           DependencyProperty.Register("BeamPorts", typeof(ObservableCollection<BeamPort>),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("beam_port")]
        public ObservableCollection<BeamPort> BeamPorts
        {
            get
            {
                return (ObservableCollection<BeamPort>)this.UIThreadGetValue(BeamPortsProperty);
            }
            set
            {
                this.UIThreadSetValue(BeamPortsProperty, value);
            }
        }


        public static readonly DependencyProperty TorpedoTubesProperty =
           DependencyProperty.Register("TorpedoTubes", typeof(ObservableCollection<VectorObject>),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("torpedo_tube")]
        public ObservableCollection<VectorObject> TorpedoTubes
        {
            get
            {
                return (ObservableCollection<VectorObject>)this.UIThreadGetValue(TorpedoTubesProperty);
            }
            set
            {
                this.UIThreadSetValue(TorpedoTubesProperty, value);
            }
        }




        public static readonly DependencyProperty TorpedoesProperty =
           DependencyProperty.Register("Torpedoes", typeof(ObservableCollection<TorpedoStorage>),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("torpedo_storage")]
        public ObservableCollection<TorpedoStorage> Torpedoes
        {
            get
            {
                return (ObservableCollection<TorpedoStorage>)this.UIThreadGetValue(TorpedoesProperty);
            }
            set
            {
                this.UIThreadSetValue(TorpedoesProperty, value);
            }
        }



        public static readonly DependencyProperty EnginePortsProperty =
           DependencyProperty.Register("EnginePorts", typeof(ObservableCollection<VectorObject>),
           typeof(Vessel));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("engine_port")]
        public ObservableCollection<VectorObject> EnginePorts
        {
            get
            {
                return (ObservableCollection<VectorObject>)this.UIThreadGetValue(EnginePortsProperty);
            }
            set
            {
                this.UIThreadSetValue(EnginePortsProperty, value);
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


    }
}
