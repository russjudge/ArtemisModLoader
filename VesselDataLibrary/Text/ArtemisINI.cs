using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;
using System.Text;
using System.Reflection;
using System.Xml;
using RussLibrary.Xml;
using RussLibrary;
using RussLibrary.WPF;
using RussLibrary.Text;
using System.Collections;
namespace VesselDataLibrary.Text
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
    public class ArtemisINI : ChangeDependencyObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INI")]
        public string INIPath { get; private set; }
        public ArtemisINI()
        {
        }
        public ArtemisINI(string path)
        {
            //1=Artemis
            //2 = Intrepid
            //3 = Aegis
            //4 = Horatio
            //5 = Excalibur
            //6 = Hera
            ValidShips = new ObservableCollection<DictionaryEntry>();
            ValidShips.Add(new DictionaryEntry(0, "No restriction"));
            ValidShips.Add(new DictionaryEntry(1, "Artemis"));
            ValidShips.Add(new DictionaryEntry(2, "Intrepid"));
            ValidShips.Add(new DictionaryEntry(3, "Aegis"));
            ValidShips.Add(new DictionaryEntry(4, "Horatio"));
            ValidShips.Add(new DictionaryEntry(5, "Excalibur"));
            ValidShips.Add(new DictionaryEntry(6, "Hera"));
            ValidShips.Add(new DictionaryEntry(7, "Ceres"));
            ValidShips.Add(new DictionaryEntry(8, "Diana"));
            INIPath = path;
            if (!string.IsNullOrEmpty(path))
            {
                INIConverter.ToObject(path, this);
            }
        }
        public void Save()
        {
            INIConverter.ToINI(this, INIPath);
        }
        public void Save(string path)
        {
            INIPath = path;
            Save();
        }

        public static readonly DependencyProperty CameraPitchProperty =
            DependencyProperty.Register("CameraPitch", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cameraPitch")]
        public double CameraPitch
        {
            get
            {
                return (double)this.UIThreadGetValue(CameraPitchProperty);
            }
            set
            {
                this.UIThreadSetValue(CameraPitchProperty, value);
            }
        }


        public static readonly DependencyProperty CameraDistanceProperty =
            DependencyProperty.Register("CameraDistance", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cameraDistance")]
        public double CameraDistance
        {
            get
            {
                return (double)this.UIThreadGetValue(CameraDistanceProperty);
            }
            set
            {
                this.UIThreadSetValue(CameraDistanceProperty, value);
            }
        }


        public static readonly DependencyProperty NetworkPortProperty =
            DependencyProperty.Register("NetworkPort", typeof(int),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("networkPort")]
        public int NetworkPort
        {
            get
            {
                return (int)this.UIThreadGetValue(NetworkPortProperty);
            }
            set
            {
                this.UIThreadSetValue(NetworkPortProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty JumpTimeCoeffProperty =
            DependencyProperty.Register("JumpTimeCoeff", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("jumpTimeCoeff")]
        public double JumpTimeCoeff
        {
            get
            {
                return (double)this.UIThreadGetValue(JumpTimeCoeffProperty);
            }
            set
            {
                this.UIThreadSetValue(JumpTimeCoeffProperty, value);
            }
        }
        //1=Artemis
        //2 = Intrepid
        //3 = Aegis
        //4 = Horatio
        //5 = Excalibur
        //6 = Hera
        static void OnShipRestrictionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArtemisINI me = sender as ArtemisINI;
            if (me != null)
            {
                if (!me.ShipRestrictionChanging)
                {
                    me.ShipRestrictionChanging = true;
                    if ((int)me.ShipRestriction.Key == 0)
                    {
                        me.ClientSide = null;
                    }
                    else
                    {
                        me.ClientSide = (int)me.ShipRestriction.Key;
                    }
                    me.ShipRestrictionChanging = false;
                    OnItemChanged(sender, e);
                }
            }
        }
        public static readonly DependencyProperty ShipRestrictionProperty =
           DependencyProperty.Register("ShipRestriction", typeof(DictionaryEntry),
           typeof(ArtemisINI), new PropertyMetadata(OnShipRestrictionChanged));

        public DictionaryEntry ShipRestriction
        {
            get
            {
                return (DictionaryEntry)this.UIThreadGetValue(ShipRestrictionProperty);
            }
            set
            {
                this.UIThreadSetValue(ShipRestrictionProperty, value);
            }
        }

        public ObservableCollection<DictionaryEntry> ValidShips { get; private set; }
        bool ShipRestrictionChanging = false;
        static void OnClientSideChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArtemisINI me = sender as ArtemisINI;
            if (me != null)
            {
                if (!me.ShipRestrictionChanging)
                {
                    me.ShipRestrictionChanging = true;
                    if (me.ClientSide == null)
                    {
                        me.ShipRestriction = me.ValidShips[0];
                    }
                    else
                    {
                        me.ShipRestriction = me.ValidShips[me.ClientSide.Value];
                    }
                    me.ShipRestrictionChanging = false;
                    OnItemChanged(sender, e);
                }
            }
        
        }
        public static readonly DependencyProperty ClientSideProperty =
            DependencyProperty.Register("ClientSide", typeof(int?),
            typeof(ArtemisINI), new PropertyMetadata(OnClientSideChanged));
        [INIConversion("clientSide")]
        public int? ClientSide
        {
            get
            {
                return (int?)this.UIThreadGetValue(ClientSideProperty);
            }
            set
            {
                this.UIThreadSetValue(ClientSideProperty, value);
            }
        }


        public static readonly DependencyProperty ClientMainScreenProperty =
            DependencyProperty.Register("ClientMainScreen", typeof(bool?),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("clientMainScreen")]
        public bool? ClientMainScreen
        {
            get
            {
                return (bool?)this.UIThreadGetValue(ClientMainScreenProperty);
            }
            set
            {
                this.UIThreadSetValue(ClientMainScreenProperty, value);
            }
        }


        public static readonly DependencyProperty ClientHelmProperty =
            DependencyProperty.Register("ClientHelm", typeof(bool?),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("clientHelm")]
        public bool? ClientHelm
        {
            get
            {
                return (bool?)this.UIThreadGetValue(ClientHelmProperty);
            }
            set
            {
                this.UIThreadSetValue(ClientHelmProperty, value);
            }
        }


        public static readonly DependencyProperty ClientWeaponProperty =
            DependencyProperty.Register("ClientWeapon", typeof(bool?),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("clientWeapon")]
        public bool? ClientWeapon
        {
            get
            {
                return (bool?)this.UIThreadGetValue(ClientWeaponProperty);
            }
            set
            {
                this.UIThreadSetValue(ClientWeaponProperty, value);
            }
        }


        public static readonly DependencyProperty ClientEngineerProperty =
            DependencyProperty.Register("ClientEngineer", typeof(bool?),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("clientEngineer")]
        public bool? ClientEngineer
        {
            get
            {
                return (bool?)this.UIThreadGetValue(ClientEngineerProperty);
            }
            set
            {
                this.UIThreadSetValue(ClientEngineerProperty, value);
            }
        }


        public static readonly DependencyProperty ClientScienceProperty =
            DependencyProperty.Register("ClientScience", typeof(bool?),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("clientScience")]
        public bool? ClientScience
        {
            get
            {
                return (bool?)this.UIThreadGetValue(ClientScienceProperty);
            }
            set
            {
                this.UIThreadSetValue(ClientScienceProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Comms")]
        public static readonly DependencyProperty ClientCommsProperty =
            DependencyProperty.Register("ClientComms", typeof(bool?),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Comms"), INIConversion("clientComms")]
        public bool? ClientComms
        {
            get
            {
                return (bool?)this.UIThreadGetValue(ClientCommsProperty);
            }
            set
            {
                this.UIThreadSetValue(ClientCommsProperty, value);
            }
        }


        public static readonly DependencyProperty ClientObserverProperty =
            DependencyProperty.Register("ClientObserver", typeof(bool?),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("clientObserver")]
        public bool? ClientObserver
        {
            get
            {
                return (bool?)this.UIThreadGetValue(ClientObserverProperty);
            }
            set
            {
                this.UIThreadSetValue(ClientObserverProperty, value);
            }
        }


        public static readonly DependencyProperty UseJoystickProperty =
            DependencyProperty.Register("UseJoystick", typeof(bool),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("useJoystick")]
        public bool UseJoystick
        {
            get
            {
                return (bool)this.UIThreadGetValue(UseJoystickProperty);
            }
            set
            {
                this.UIThreadSetValue(UseJoystickProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TouchScreen")]
        public static readonly DependencyProperty TouchScreenProperty =
            DependencyProperty.Register("TouchScreen", typeof(bool),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TouchScreen"), INIConversion("touchScreen")]
        public bool TouchScreen
        {
            get
            {
                return (bool)this.UIThreadGetValue(TouchScreenProperty);
            }
            set
            {
                this.UIThreadSetValue(TouchScreenProperty, value);
            }
        }


        public static readonly DependencyProperty ForceAddressProperty =
            DependencyProperty.Register("ForceAddress", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("forceAddress")]
        public string ForceAddress
        {
            get
            {
                return (string)this.UIThreadGetValue(ForceAddressProperty);
            }
            set
            {
                this.UIThreadSetValue(ForceAddressProperty, value);
            }
        }


        public static readonly DependencyProperty LightingIPAddressProperty =
            DependencyProperty.Register("LightingIPAddress", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("lightingIPAddress")]
        public string LightingIPAddress
        {
            get
            {
                return (string)this.UIThreadGetValue(LightingIPAddressProperty);
            }
            set
            {
                this.UIThreadSetValue(LightingIPAddressProperty, value);
            }
        }


        public static readonly DependencyProperty StationEnergyProperty =
            DependencyProperty.Register("StationEnergy", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("stationEnergy")]
        public double StationEnergy
        {
            get
            {
                return (double)this.UIThreadGetValue(StationEnergyProperty);
            }
            set
            {
                this.UIThreadSetValue(StationEnergyProperty, value);
            }
        }


        public static readonly DependencyProperty PlayerShieldRechargeRateProperty =
            DependencyProperty.Register("PlayerShieldRechargeRate", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("playerShieldRechargeRate")]
        public double PlayerShieldRechargeRate
        {
            get
            {
                return (double)this.UIThreadGetValue(PlayerShieldRechargeRateProperty);
            }
            set
            {
                this.UIThreadSetValue(PlayerShieldRechargeRateProperty, value);
            }
        }


        public static readonly DependencyProperty EnemyShieldRechargeRateProperty =
            DependencyProperty.Register("EnemyShieldRechargeRate", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("enemyShieldRechargeRate")]
        public double EnemyShieldRechargeRate
        {
            get
            {
                return (double)this.UIThreadGetValue(EnemyShieldRechargeRateProperty);
            }
            set
            {
                this.UIThreadSetValue(EnemyShieldRechargeRateProperty, value);
            }
        }


        public static readonly DependencyProperty StationSensorRangeProperty =
            DependencyProperty.Register("StationSensorRange", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("stationSensorRange")]
        public double StationSensorRange
        {
            get
            {
                return (double)this.UIThreadGetValue(StationSensorRangeProperty);
            }
            set
            {
                this.UIThreadSetValue(StationSensorRangeProperty, value);
            }
        }


        public static readonly DependencyProperty PlayerSensorRangeProperty =
            DependencyProperty.Register("PlayerSensorRange", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("playerSensorRange")]
        public double PlayerSensorRange
        {
            get
            {
                return (double)this.UIThreadGetValue(PlayerSensorRangeProperty);
            }
            set
            {
                this.UIThreadSetValue(PlayerSensorRangeProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyUseCoeffProperty =
            DependencyProperty.Register("EnergyUseCoeff", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyUseCoeff")]
        public double EnergyUseCoeff
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyUseCoeffProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyUseCoeffProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffBeamsProperty =
            DependencyProperty.Register("EnergyCoeffBeams", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffBeams")]
        public double EnergyCoeffBeams
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffBeamsProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffBeamsProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffTubesProperty =
            DependencyProperty.Register("EnergyCoeffTubes", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffTubes")]
        public double EnergyCoeffTubes
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffTubesProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffTubesProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffTacticalProperty =
            DependencyProperty.Register("EnergyCoeffTactical", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffTactical")]
        public double EnergyCoeffTactical
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffTacticalProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffTacticalProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffManeuverProperty =
            DependencyProperty.Register("EnergyCoeffManeuver", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffManeuver")]
        public double EnergyCoeffManeuver
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffManeuverProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffManeuverProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffImpulseProperty =
            DependencyProperty.Register("EnergyCoeffImpulse", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffImpulse")]
        public double EnergyCoeffImpulse
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffImpulseProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffImpulseProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffWarpProperty =
            DependencyProperty.Register("EnergyCoeffWarp", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffWarp")]
        public double EnergyCoeffWarp
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffWarpProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffWarpProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shlds")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffFrontShldsProperty =
            DependencyProperty.Register("EnergyCoeffFrontShlds", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shlds"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffFrontShlds")]
        public double EnergyCoeffFrontShlds
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffFrontShldsProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffFrontShldsProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shlds")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffRearShldsProperty =
            DependencyProperty.Register("EnergyCoeffRearShlds", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Shlds"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffRearShlds")]
        public double EnergyCoeffRearShlds
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffRearShldsProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffRearShldsProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff")]
        public static readonly DependencyProperty EnergyCoeffJumpProperty =
            DependencyProperty.Register("EnergyCoeffJump", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Coeff"), INIConversion("energyCoeffJump")]
        public double EnergyCoeffJump
        {
            get
            {
                return (double)this.UIThreadGetValue(EnergyCoeffJumpProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyCoeffJumpProperty, value);
            }
        }


        public static readonly DependencyProperty PlayerBeamDelayProperty =
            DependencyProperty.Register("PlayerBeamDelay", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("playerBeamDelay")]
        public double PlayerBeamDelay
        {
            get
            {
                return (double)this.UIThreadGetValue(PlayerBeamDelayProperty);
            }
            set
            {
                this.UIThreadSetValue(PlayerBeamDelayProperty, value);
            }
        }


        public static readonly DependencyProperty OverloadThresholdProperty =
            DependencyProperty.Register("OverloadThreshold", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("overloadThreshold")]
        public double OverloadThreshold
        {
            get
            {
                return (double)this.UIThreadGetValue(OverloadThresholdProperty);
            }
            set
            {
                this.UIThreadSetValue(OverloadThresholdProperty, value);
            }
        }


        public static readonly DependencyProperty OverloadHeatProperty =
            DependencyProperty.Register("OverloadHeat", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("overloadHeat")]
        public double OverloadHeat
        {
            get
            {
                return (double)this.UIThreadGetValue(OverloadHeatProperty);
            }
            set
            {
                this.UIThreadSetValue(OverloadHeatProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty LowStartStationTorp1Property =
            DependencyProperty.Register("LowStartStationTorp1", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("lowStartStationTorp1")]
        public double LowStartStationTorp1
        {
            get
            {
                return (double)this.UIThreadGetValue(LowStartStationTorp1Property);
            }
            set
            {
                this.UIThreadSetValue(LowStartStationTorp1Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty HighStartStationTorp1Property =
            DependencyProperty.Register("HighStartStationTorp1", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("highStartStationTorp1")]
        public double HighStartStationTorp1
        {
            get
            {
                return (double)this.UIThreadGetValue(HighStartStationTorp1Property);
            }
            set
            {
                this.UIThreadSetValue(HighStartStationTorp1Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty DamageTorp1Property =
            DependencyProperty.Register("DamageTorp1", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("damageTorp1")]
        public double DamageTorp1
        {
            get
            {
                return (double)this.UIThreadGetValue(DamageTorp1Property);
            }
            set
            {
                this.UIThreadSetValue(DamageTorp1Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty SpeedTorp1Property =
            DependencyProperty.Register("SpeedTorp1", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("speedTorp1")]
        public double SpeedTorp1
        {
            get
            {
                return (double)this.UIThreadGetValue(SpeedTorp1Property);
            }
            set
            {
                this.UIThreadSetValue(SpeedTorp1Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty MinutesToProduceTorp1Property =
            DependencyProperty.Register("MinutesToProduceTorp1", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("minutesToProduceTorp1")]
        public double MinutesToProduceTorp1
        {
            get
            {
                return (double)this.UIThreadGetValue(MinutesToProduceTorp1Property);
            }
            set
            {
                this.UIThreadSetValue(MinutesToProduceTorp1Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty LowStartStationTorp2Property =
            DependencyProperty.Register("LowStartStationTorp2", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("lowStartStationTorp2")]
        public double LowStartStationTorp2
        {
            get
            {
                return (double)this.UIThreadGetValue(LowStartStationTorp2Property);
            }
            set
            {
                this.UIThreadSetValue(LowStartStationTorp2Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty HighStartStationTorp2Property =
            DependencyProperty.Register("HighStartStationTorp2", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("highStartStationTorp2")]
        public double HighStartStationTorp2
        {
            get
            {
                return (double)this.UIThreadGetValue(HighStartStationTorp2Property);
            }
            set
            {
                this.UIThreadSetValue(HighStartStationTorp2Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty DamageTorp2Property =
            DependencyProperty.Register("DamageTorp2", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("damageTorp2")]
        public double DamageTorp2
        {
            get
            {
                return (double)this.UIThreadGetValue(DamageTorp2Property);
            }
            set
            {
                this.UIThreadSetValue(DamageTorp2Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty SpeedTorp2Property =
            DependencyProperty.Register("SpeedTorp2", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("speedTorp2")]
        public double SpeedTorp2
        {
            get
            {
                return (double)this.UIThreadGetValue(SpeedTorp2Property);
            }
            set
            {
                this.UIThreadSetValue(SpeedTorp2Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty MinutesToProduceTorp2Property =
            DependencyProperty.Register("MinutesToProduceTorp2", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("minutesToProduceTorp2")]
        public double MinutesToProduceTorp2
        {
            get
            {
                return (double)this.UIThreadGetValue(MinutesToProduceTorp2Property);
            }
            set
            {
                this.UIThreadSetValue(MinutesToProduceTorp2Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty LowStartStationTorp3Property =
            DependencyProperty.Register("LowStartStationTorp3", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("lowStartStationTorp3")]
        public double LowStartStationTorp3
        {
            get
            {
                return (double)this.UIThreadGetValue(LowStartStationTorp3Property);
            }
            set
            {
                this.UIThreadSetValue(LowStartStationTorp3Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty HighStartStationTorp3Property =
            DependencyProperty.Register("HighStartStationTorp3", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("highStartStationTorp3")]
        public double HighStartStationTorp3
        {
            get
            {
                return (double)this.UIThreadGetValue(HighStartStationTorp3Property);
            }
            set
            {
                this.UIThreadSetValue(HighStartStationTorp3Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty DamageTorp3Property =
            DependencyProperty.Register("DamageTorp3", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("damageTorp3")]
        public double DamageTorp3
        {
            get
            {
                return (double)this.UIThreadGetValue(DamageTorp3Property);
            }
            set
            {
                this.UIThreadSetValue(DamageTorp3Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty SpeedTorp3Property =
            DependencyProperty.Register("SpeedTorp3", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("speedTorp3")]
        public double SpeedTorp3
        {
            get
            {
                return (double)this.UIThreadGetValue(SpeedTorp3Property);
            }
            set
            {
                this.UIThreadSetValue(SpeedTorp3Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty MinutesToProduceTorp3Property =
            DependencyProperty.Register("MinutesToProduceTorp3", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("minutesToProduceTorp3")]
        public double MinutesToProduceTorp3
        {
            get
            {
                return (double)this.UIThreadGetValue(MinutesToProduceTorp3Property);
            }
            set
            {
                this.UIThreadSetValue(MinutesToProduceTorp3Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty LowStartStationTorp4Property =
            DependencyProperty.Register("LowStartStationTorp4", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("lowStartStationTorp4")]
        public double LowStartStationTorp4
        {
            get
            {
                return (double)this.UIThreadGetValue(LowStartStationTorp4Property);
            }
            set
            {
                this.UIThreadSetValue(LowStartStationTorp4Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty HighStartStationTorp4Property =
            DependencyProperty.Register("HighStartStationTorp4", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("highStartStationTorp4")]
        public double HighStartStationTorp4
        {
            get
            {
                return (double)this.UIThreadGetValue(HighStartStationTorp4Property);
            }
            set
            {
                this.UIThreadSetValue(HighStartStationTorp4Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty DamageTorp4Property =
            DependencyProperty.Register("DamageTorp4", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("damageTorp4")]
        public double DamageTorp4
        {
            get
            {
                return (double)this.UIThreadGetValue(DamageTorp4Property);
            }
            set
            {
                this.UIThreadSetValue(DamageTorp4Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty SpeedTorp4Property =
            DependencyProperty.Register("SpeedTorp4", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("speedTorp4")]
        public double SpeedTorp4
        {
            get
            {
                return (double)this.UIThreadGetValue(SpeedTorp4Property);
            }
            set
            {
                this.UIThreadSetValue(SpeedTorp4Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp")]
        public static readonly DependencyProperty MinutesToProduceTorp4Property =
            DependencyProperty.Register("MinutesToProduceTorp4", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Torp"), INIConversion("minutesToProduceTorp4")]
        public double MinutesToProduceTorp4
        {
            get
            {
                return (double)this.UIThreadGetValue(MinutesToProduceTorp4Property);
            }
            set
            {
                this.UIThreadSetValue(MinutesToProduceTorp4Property, value);
            }
        }


        public static readonly DependencyProperty CueShieldsDownProperty =
            DependencyProperty.Register("CueShieldsDown", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueShieldsDown")]
        public string CueShieldsDown
        {
            get
            {
                return (string)this.UIThreadGetValue(CueShieldsDownProperty);
            }
            set
            {
                this.UIThreadSetValue(CueShieldsDownProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueShieldsDownVolProperty =
            DependencyProperty.Register("CueShieldsDownVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueShieldsDownVol")]
        public double CueShieldsDownVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueShieldsDownVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueShieldsDownVolProperty, value);
            }
        }


        public static readonly DependencyProperty CueShieldsUpProperty =
            DependencyProperty.Register("CueShieldsUp", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueShieldsUp")]
        public string CueShieldsUp
        {
            get
            {
                return (string)this.UIThreadGetValue(CueShieldsUpProperty);
            }
            set
            {
                this.UIThreadSetValue(CueShieldsUpProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueShieldsUpVolProperty =
            DependencyProperty.Register("CueShieldsUpVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueShieldsUpVol")]
        public double CueShieldsUpVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueShieldsUpVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueShieldsUpVolProperty, value);
            }
        }
        public static readonly DependencyProperty CueAIExplodeProperty =
           DependencyProperty.Register("CueAIExplode", typeof(string),
           typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueAIShipExplode")]
        public string CueAIExplode
        {
            get
            {
                return (string)this.UIThreadGetValue(CueAIExplodeProperty);
            }
            set
            {
                this.UIThreadSetValue(CueAIExplodeProperty, value);
            }
        }


        //public static readonly DependencyProperty CueEnemyExplodeProperty =
        //    DependencyProperty.Register("CueEnemyExplode", typeof(string),
        //    typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        //[INIConversion("cueEnemyExplode")]
        //public string CueEnemyExplode
        //{
        //    get
        //    {
        //        return (string)this.UIThreadGetValue(CueEnemyExplodeProperty);
        //    }
        //    set
        //    {
        //        this.UIThreadSetValue(CueEnemyExplodeProperty, value);
        //    }
        //}


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueAIExplodeVolProperty =
            DependencyProperty.Register("CueAIExplodeVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueAIShipExplodeVol")]
        public double CCueAIExplodeVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueAIExplodeVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueAIExplodeVolProperty, value);
            }
        }


        public static readonly DependencyProperty CueRedAlertProperty =
            DependencyProperty.Register("CueRedAlert", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueRedAlert")]
        public string CueRedAlert
        {
            get
            {
                return (string)this.UIThreadGetValue(CueRedAlertProperty);
            }
            set
            {
                this.UIThreadSetValue(CueRedAlertProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueRedAlertVolProperty =
            DependencyProperty.Register("CueRedAlertVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueRedAlertVol")]
        public double CueRedAlertVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueRedAlertVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueRedAlertVolProperty, value);
            }
        }


        public static readonly DependencyProperty CueInternalDamageAlarmProperty =
            DependencyProperty.Register("CueInternalDamageAlarm", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueInternalDamageAlarm")]
        public string CueInternalDamageAlarm
        {
            get
            {
                return (string)this.UIThreadGetValue(CueInternalDamageAlarmProperty);
            }
            set
            {
                this.UIThreadSetValue(CueInternalDamageAlarmProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueInternalDamageAlarmVolProperty =
            DependencyProperty.Register("CueInternalDamageAlarmVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueInternalDamageAlarmVol")]
        public double CueInternalDamageAlarmVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueInternalDamageAlarmVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueInternalDamageAlarmVolProperty, value);
            }
        }


        public static readonly DependencyProperty CueHullHitProperty =
            DependencyProperty.Register("CueHullHit", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueHullHit")]
        public string CueHullHit
        {
            get
            {
                return (string)this.UIThreadGetValue(CueHullHitProperty);
            }
            set
            {
                this.UIThreadSetValue(CueHullHitProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueHullHitVolProperty =
            DependencyProperty.Register("CueHullHitVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueHullHitVol")]
        public double CueHullHitVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueHullHitVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueHullHitVolProperty, value);
            }
        }


        public static readonly DependencyProperty CueShieldHitProperty =
            DependencyProperty.Register("CueShieldHit", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueShieldHit")]
        public string CueShieldHit
        {
            get
            {
                return (string)this.UIThreadGetValue(CueShieldHitProperty);
            }
            set
            {
                this.UIThreadSetValue(CueShieldHitProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueShieldHitVolProperty =
            DependencyProperty.Register("CueShieldHitVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueShieldHitVol")]
        public double CueShieldHitVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueShieldHitVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueShieldHitVolProperty, value);
            }
        }


        public static readonly DependencyProperty CuePlayerBeamProperty =
            DependencyProperty.Register("CuePlayerBeam", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cuePlayerBeam")]
        public string CuePlayerBeam
        {
            get
            {
                return (string)this.UIThreadGetValue(CuePlayerBeamProperty);
            }
            set
            {
                this.UIThreadSetValue(CuePlayerBeamProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CuePlayerBeamVolProperty =
            DependencyProperty.Register("CuePlayerBeamVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cuePlayerBeamVol")]
        public double CuePlayerBeamVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CuePlayerBeamVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CuePlayerBeamVolProperty, value);
            }
        }


        public static readonly DependencyProperty CuePlayerTorpedoProperty =
            DependencyProperty.Register("CuePlayerTorpedo", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cuePlayerTorpedo")]
        public string CuePlayerTorpedo
        {
            get
            {
                return (string)this.UIThreadGetValue(CuePlayerTorpedoProperty);
            }
            set
            {
                this.UIThreadSetValue(CuePlayerTorpedoProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CuePlayerTorpedoVolProperty =
            DependencyProperty.Register("CuePlayerTorpedoVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cuePlayerTorpedoVol")]
        public double CuePlayerTorpedoVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CuePlayerTorpedoVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CuePlayerTorpedoVolProperty, value);
            }
        }


        public static readonly DependencyProperty CueStationDockProperty =
            DependencyProperty.Register("CueStationDock", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueStationDock")]
        public string CueStationDock
        {
            get
            {
                return (string)this.UIThreadGetValue(CueStationDockProperty);
            }
            set
            {
                this.UIThreadSetValue(CueStationDockProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueStationDockVolProperty =
            DependencyProperty.Register("CueStationDockVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueStationDockVol")]
        public double CueStationDockVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueStationDockVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueStationDockVolProperty, value);
            }
        }


        public static readonly DependencyProperty CueEngineLoopProperty =
            DependencyProperty.Register("CueEngineLoop", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueEngineLoop")]
        public string CueEngineLoop
        {
            get
            {
                return (string)this.UIThreadGetValue(CueEngineLoopProperty);
            }
            set
            {
                this.UIThreadSetValue(CueEngineLoopProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueEngineLoopVolProperty =
            DependencyProperty.Register("CueEngineLoopVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueEngineLoopVol")]
        public double CueEngineLoopVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueEngineLoopVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueEngineLoopVolProperty, value);
            }
        }



        public static readonly DependencyProperty CueUI00Property =
            DependencyProperty.Register("CueUI00", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI00")]
        public string CueUI00
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI00Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI00Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI00VolProperty =
            DependencyProperty.Register("CueUI00Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI00Vol")]
        public double CueUI00Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI00VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI00VolProperty, value);
            }
        }


        public static readonly DependencyProperty CueUI01Property =
            DependencyProperty.Register("CueUI01", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI01")]
        public string CueUI01
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI01Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI01Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI01VolProperty =
            DependencyProperty.Register("CueUI01Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI01Vol")]
        public double CueUI01Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI01VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI01VolProperty, value);
            }
        }


        public static readonly DependencyProperty CueUI02Property =
            DependencyProperty.Register("CueUI02", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI02")]
        public string CueUI02
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI02Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI02Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI02VolProperty =
            DependencyProperty.Register("CueUI02Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI02Vol")]
        public double CueUI02Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI02VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI02VolProperty, value);
            }
        }


        public static readonly DependencyProperty CueUI03Property =
            DependencyProperty.Register("CueUI03", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI03")]
        public string CueUI03
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI03Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI03Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI03VolProperty =
            DependencyProperty.Register("CueUI03Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI03Vol")]
        public double CueUI03Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI03VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI03VolProperty, value);
            }
        }


        public static readonly DependencyProperty CueUI04Property =
            DependencyProperty.Register("CueUI04", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI04")]
        public string CueUI04
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI04Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI04Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI04VolProperty =
            DependencyProperty.Register("CueUI04Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI04Vol")]
        public double CueUI04Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI04VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI04VolProperty, value);
            }
        }


        public static readonly DependencyProperty CueUI05Property =
            DependencyProperty.Register("CueUI05", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI05")]
        public string CueUI05
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI05Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI05Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI05VolProperty =
            DependencyProperty.Register("CueUI05Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI05Vol")]
        public double CueUI05Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI05VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI05VolProperty, value);
            }
        }


        public static readonly DependencyProperty CueUI06Property =
            DependencyProperty.Register("CueUI06", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI06")]
        public string CueUI06
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI06Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI06Property, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI06VolProperty =
            DependencyProperty.Register("CueUI06Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI06Vol")]
        public double CueUI06Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI06VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI06VolProperty, value);
            }
        }









        //cue property begin

        public static readonly DependencyProperty CueUI07Property =
            DependencyProperty.Register("CueUI07", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI07")]
        public string CueUI07
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI07Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI07Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI07VolProperty =
            DependencyProperty.Register("CueUI07Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI07Vol")]
        public double CueUI07Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI07VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI07VolProperty, value);
            }
        }

        //cue property end






        //cue property begin

        public static readonly DependencyProperty CueUI08Property =
            DependencyProperty.Register("CueUI08", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI08")]
        public string CueUI08
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI08Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI08Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI08VolProperty =
            DependencyProperty.Register("CueUI08Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI08Vol")]
        public double CueUI08Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI08VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI08VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI09Property =
            DependencyProperty.Register("CueUI09", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI09")]
        public string CueUI09
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI09Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI09Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI09VolProperty =
            DependencyProperty.Register("CueUI09Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI09Vol")]
        public double CueUI09Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI09VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI09VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI10Property =
            DependencyProperty.Register("CueUI10", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI10")]
        public string CueUI
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI10Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI10Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI10VolProperty =
            DependencyProperty.Register("CueUI10Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI10Vol")]
        public double CueUI10Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI10VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI10VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI11Property =
            DependencyProperty.Register("CueUI11", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI11")]
        public string CueUI11
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI11Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI11Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI11VolProperty =
            DependencyProperty.Register("CueUI11Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI11Vol")]
        public double CueUI11Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI11VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI11VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI12Property =
            DependencyProperty.Register("CueUI12", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI12")]
        public string CueUI12
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI12Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI12Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI12VolProperty =
            DependencyProperty.Register("CueUI12Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI12Vol")]
        public double CueUI12Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI12VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI12VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI13Property =
            DependencyProperty.Register("CueUI13", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI13")]
        public string CueUI13
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI13Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI13Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI13VolProperty =
            DependencyProperty.Register("CueUI13Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI13Vol")]
        public double CueUI13Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI13VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI13VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI14Property =
            DependencyProperty.Register("CueUI14", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI14")]
        public string CueUI14
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI14Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI14Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI15VolProperty =
            DependencyProperty.Register("CueUI15Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI15Vol")]
        public double CueUI15Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI15VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI15VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI16Property =
            DependencyProperty.Register("CueUI16", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI16")]
        public string CueUI16
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI16Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI16Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI16VolProperty =
            DependencyProperty.Register("CueUI16Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI16Vol")]
        public double CueUI16Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI16VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI16VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI15Property =
            DependencyProperty.Register("CueUI15", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI15")]
        public string CueUI15
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI15Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI15Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI14VolProperty =
            DependencyProperty.Register("CueUI14Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI14Vol")]
        public double CueUI14Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI14VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI14VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI17Property =
            DependencyProperty.Register("CueUI17", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI17")]
        public string CueUI17
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI17Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI17Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI17VolProperty =
            DependencyProperty.Register("CueUI17Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI17Vol")]
        public double CueUI17Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI17VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI17VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI18Property =
            DependencyProperty.Register("CueUI18", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI18")]
        public string CueUI18
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI18Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI18Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI18VolProperty =
            DependencyProperty.Register("CueUI18Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI18Vol")]
        public double CueUI18Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI18VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI18VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI19Property =
            DependencyProperty.Register("CueUI19", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI19")]
        public string CueUI19
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI19Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI19Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI19VolProperty =
            DependencyProperty.Register("CueUI19Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI19Vol")]
        public double CueUI19Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI19VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI19VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI20Property =
            DependencyProperty.Register("CueUI20", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI20")]
        public string CueUI20
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI20Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI20Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI20VolProperty =
            DependencyProperty.Register("CueUI20Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI20Vol")]
        public double CueUI20Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI20VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI20VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI21Property =
            DependencyProperty.Register("CueUI21", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI21")]
        public string CueUI21
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI21Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI21Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI21VolProperty =
            DependencyProperty.Register("CueUI21Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI21Vol")]
        public double CueUI21Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI21VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI21VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI22Property =
            DependencyProperty.Register("CueUI22", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI22")]
        public string CueUI22
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI22Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI22Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI22VolProperty =
            DependencyProperty.Register("CueUI22Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI22Vol")]
        public double CueUI22Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI22VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI22VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI23Property =
            DependencyProperty.Register("CueUI23", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI23")]
        public string CueUI23
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI23Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI23Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI23VolProperty =
            DependencyProperty.Register("CueUI23Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI23Vol")]
        public double CueUI23Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI23VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI23VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI24Property =
            DependencyProperty.Register("CueUI24", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI24")]
        public string CueUI24
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI24Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI24Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI24VolProperty =
            DependencyProperty.Register("CueUI24Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI24Vol")]
        public double CueUI24Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI24VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI24VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI25Property =
            DependencyProperty.Register("CueUI25", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI25")]
        public string CueUI25
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI25Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI25Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI25VolProperty =
            DependencyProperty.Register("CueUI25Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI25Vol")]
        public double CueUI25Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI25VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI25VolProperty, value);
            }
        }

        //cue property end



        //cue property begin

        public static readonly DependencyProperty CueUI26Property =
            DependencyProperty.Register("CueUI26", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueUI26")]
        public string CueUI26
        {
            get
            {
                return (string)this.UIThreadGetValue(CueUI26Property);
            }
            set
            {
                this.UIThreadSetValue(CueUI26Property, value);
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueUI26VolProperty =
            DependencyProperty.Register("CueUI26Vol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueUI26Vol")]
        public double CueUI26Vol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueUI26VolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueUI26VolProperty, value);
            }
        }

        //cue property end










        public static readonly DependencyProperty CueWarpFailProperty =
            DependencyProperty.Register("CueWarpFail", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueWarpFail")]
        public string CueWarpFail
        {
            get
            {
                return (string)this.UIThreadGetValue(CueWarpFailProperty);
            }
            set
            {
                this.UIThreadSetValue(CueWarpFailProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueWarpFailVolProperty =
            DependencyProperty.Register("CueWarpFailVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueWarpFailVol")]
        public double CueWarpFailVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueWarpFailVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueWarpFailVolProperty, value);
            }
        }


        public static readonly DependencyProperty CueWarpTravelProperty =
            DependencyProperty.Register("CueWarpTravel", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [INIConversion("cueWarpTravel")]
        public string CueWarpTravel
        {
            get
            {
                return (string)this.UIThreadGetValue(CueWarpTravelProperty);
            }
            set
            {
                this.UIThreadSetValue(CueWarpTravelProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueWarpTravelVolProperty =
            DependencyProperty.Register("CueWarpTravelVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueWarpTravelVol")]
        public double CueWarpTravelVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueWarpTravelVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueWarpTravelVolProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Warmup")]
        public static readonly DependencyProperty CueJumpWarmupProperty =
            DependencyProperty.Register("CueJumpWarmup", typeof(string),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Warmup"), INIConversion("cueJumpWarmup")]
        public string CueJumpWarmup
        {
            get
            {
                return (string)this.UIThreadGetValue(CueJumpWarmupProperty);
            }
            set
            {
                this.UIThreadSetValue(CueJumpWarmupProperty, value);
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Warmup")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol")]
        public static readonly DependencyProperty CueJumpWarmupVolProperty =
            DependencyProperty.Register("CueJumpWarmupVol", typeof(double),
            typeof(ArtemisINI), new PropertyMetadata(OnItemChanged));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Warmup"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vol"), INIConversion("cueJumpWarmupVol")]
        public double CueJumpWarmupVol
        {
            get
            {
                return (double)this.UIThreadGetValue(CueJumpWarmupVolProperty);
            }
            set
            {
                this.UIThreadSetValue(CueJumpWarmupVolProperty, value);
            }
        }



        protected override void ProcessValidation()
        {

        }
    }
}
