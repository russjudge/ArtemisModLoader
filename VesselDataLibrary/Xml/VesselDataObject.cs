using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using RussLibrary.Xml;
using RussLibrary;
using RussLibrary.WPF;
using ArtemisModLoader;
using System.Xml;
namespace VesselDataLibrary.Xml
{
    //XmlConversionRoot("vessel_data", false)
    [XmlConversionRoot("vessel_data"), 
    XmlComment("Modified by Artemis Mod Loader\r\nDownload the latest version of Artemis Mod Loader from http://px2owffng8.embed.tal.ki/20121226/artemis-mod-loader-2176572/?viewmark=1")]
    public class VesselDataObject : ChangeDependencyObject, IXmlStorage
    {
        //PerformValidation once data is loaded, and again when saving.
        public VesselDataObject()
        {
            Storage = new List<XmlNode>();
            HullRaces = new HullRaceCollection();
            Vessels = new VesselCollection();
            Version = DataStrings.VesselDataCurrentVersion;
            HullRaces.ObjectChanged += new EventHandler(HullRaces_ObjectChanged);
            Vessels.ObjectChanged += new EventHandler(Vessels_ObjectChanged);
        }

        void Vessels_ObjectChanged(object sender, EventArgs e)
        {
            SetChanged();
        }

        void HullRaces_ObjectChanged(object sender, EventArgs e)
        {
            SetChanged();
        }
        public bool IsSaving { get; private set; }
        public override void BeginInitialization()
        {
            base.BeginInitialization();
            HullRaces.BeginInitialization();
            Vessels.BeginInitialization();
        }
        public override void EndInitialization()
        {
            HullRaces.EndInitialization();
            Vessels.EndInitialization();
            base.EndInitialization();
        }
        public override void AcceptChanges()
        {
            Version = DataStrings.VesselDataCurrentVersion;
            HullRaces.AcceptChanges();
            Vessels.AcceptChanges();
                
            base.AcceptChanges();
        }
        public override void RejectChanges()
        {
            HullRaces.RejectChanges();
            Vessels.RejectChanges();
            base.RejectChanges();
        }
        /*
         * Sample Xml from file:
         *
         *  <vessel_data version="1.66">

  <hullRace ID="0" name="USFP" keys="player"/>
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
        </vessel_data>
         * */
        public static readonly DependencyProperty VersionProperty =
          DependencyProperty.Register("Version", typeof(string),
          typeof(VesselDataObject));

        [XmlConversion("version")]
        public string Version
        {
            get
            {
                return (string)this.UIThreadGetValue(VersionProperty);

            }
            set
            {
                this.UIThreadSetValue(VersionProperty, value);

            }
        }

        public static readonly DependencyProperty HullRacesProperty =
          DependencyProperty.Register("HullRaces", typeof(HullRaceCollection),
          typeof(VesselDataObject));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("hullRace")]
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


        public static readonly DependencyProperty VesselsProperty =
          DependencyProperty.Register("Vessels", typeof(VesselCollection),
          typeof(VesselDataObject));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), XmlConversion("vessel")]
        public VesselCollection Vessels
        {
            get
            {
                return (VesselCollection)this.UIThreadGetValue(VesselsProperty);

            }
            set
            {
                this.UIThreadSetValue(VesselsProperty, value);

            }
        }


        public void AddVessel()
        {
            Vessel v = Vessels.AddNewVessel();
            if (HullRaces.Count > 0)
            {
                v.Side = HullRaces[0].ID;
            }
            PerformValidation();
        }
       

        public void PerformValidation()
        {


            ProcessValidation();
          
        }
       
        public void Save(string file)
        {
            IsSaving = true;
            XmlDocument PreDoc = XmlConverter.ToXmlDocument(this);

            foreach (Vessel v in Vessels)
            {
                if (v.BroadType != "player")
                {
                    v.InternalDefinition = null;
                    v.Torpedoes = null;

                }
                else
                {
                    v.FleetAICommonality = null;
                    //Set FleetAICommonality to null.
                }
                if (v.Description == null || string.IsNullOrEmpty(v.Description.Text))
                {
                    v.Description = null;
                }
                if (v.FleetAICommonality != null && v.FleetAICommonality.Commonality <= 0)
                {
                    v.FleetAICommonality = null;
                }
                if (v.Performance.TopSpeed == 0 && v.Performance.TurnRate == 0)
                {
                    v.Performance = null;
                }
                if (v.Carrier != null && v.Carrier.Compliment <= 0)
                {
                    v.Carrier = null;
                }


            }
            Version = ArtemisModLoader.DataStrings.VesselDataCurrentVersion;
            XmlDocument doc = XmlConverter.ToXmlDocument(this);

            doc.Save(file);
            this.HullRaces.Clear();
            this.Vessels.Clear();
            XmlConverter.ToObject(PreDoc, this);
            AcceptChanges();


            PerformValidation();

            IsSaving = false;
            
            
            
        }
        public bool DoValidation()
        {
            OverallValidity = true;
            ProcessValidation();
            return OverallValidity;
        }
        bool OverallValidity = true;
        protected override void ProcessValidation()
        {
            OverallValidity = true;
            for (int i = 0; i < Vessels.Count; i++)
            {

                bool validSide = false;
                foreach (HullRace race in HullRaces)
                {
                    if (Vessels[i].Side == race.ID)
                    {
                        validSide = true;
                        break;
                    }
                }
                if (!validSide)
                {
                    OverallValidity = false;
                    Vessels[i].ValidationCollection.AddValidation(DataStrings.Side, ValidationValue.IsError, AMLResources.Properties.Resources.VesselSideInvalid);
                }
            }
        }

        public IList<XmlNode> Storage { get; private set; }
    }
}
