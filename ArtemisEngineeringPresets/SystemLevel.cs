using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;
using RussLibrary;
namespace ArtemisEngineeringPresets
{

    public class SystemLevel : UIElement
    {
        public SystemLevel()
        {
        }
        public SystemLevel(int energy, int coolant)
        {
            EnergyLevel = energy;
            CoolantLevel = coolant;
        }

        public void AcceptChanges()
        {
            Changed = false;
        }
        //static readonly ILog _log = LogManager.GetLogger(typeof(SystemLevel));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
            "ValueChanged", RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(SystemLevel));

        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }


        static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SystemLevel me = sender as SystemLevel;
            if (me != null)
            {
                if (me.Changed)
                {
                    
                }
            }
        }
        public static readonly DependencyProperty ChangedProperty =
         DependencyProperty.Register("Changed", typeof(bool),
         typeof(SystemLevel), new PropertyMetadata(OnChanged));

        public bool Changed
        {
            get
            {
                return (bool)this.UIThreadGetValue(ChangedProperty);
            }
            private set
            {
                this.UIThreadSetValue(ChangedProperty, value);
            }
        }

        public string SystemName
        {
            get
            {
                string[] systems = { "Beam", "Torpedo", "Sensors", "Maneuvering", "Impulse", "Warp/Jump", "Front Shield", "Rear Shield" };
                return systems[Index];
            }
        }
        public static readonly DependencyProperty IndexProperty =
        DependencyProperty.Register("Index", typeof(int),
        typeof(SystemLevel));

        public int Index
        {
            get
            {
                return (int)this.UIThreadGetValue(IndexProperty);
            }
            set
            {
                this.UIThreadSetValue(IndexProperty, value);
            }
        }



        static void OnEnergyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SystemLevel me = sender as SystemLevel;
            if (me != null)
            {
                me.CoolantNeed = Preset.CoolantLevelToPreventOverheat(me.EnergyLevel);
                me.Changed = true;
            }
        }

        public static readonly DependencyProperty EnergyLevelProperty =
         DependencyProperty.Register("EnergyLevel", typeof(int),
         typeof(SystemLevel), new PropertyMetadata(OnEnergyChanged));

        public int EnergyLevel
        {
            get
            {
                return (int)this.UIThreadGetValue(EnergyLevelProperty);
            }
            set
            {
                this.UIThreadSetValue(EnergyLevelProperty, value);
            }
        }
        static void OnCoolantChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SystemLevel me = sender as SystemLevel;
            if (me != null)
            {
                me.Changed = true;
                me.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }

        public static readonly DependencyProperty CoolantLevelProperty =
         DependencyProperty.Register("CoolantLevel", typeof(int),
         typeof(SystemLevel), new PropertyMetadata(OnCoolantChanged));

        public int CoolantLevel
        {
            get
            {
                return (int)this.UIThreadGetValue(CoolantLevelProperty);
            }
            set
            {
                this.UIThreadSetValue(CoolantLevelProperty, value);
            }
        }




        public static readonly DependencyProperty CoolantNeedProperty =
         DependencyProperty.Register("CoolantNeed", typeof(int),
         typeof(SystemLevel));

        public int CoolantNeed
        {
            get
            {
                return (int)this.UIThreadGetValue(CoolantNeedProperty);
            }
            private set
            {
                this.UIThreadSetValue(CoolantNeedProperty, value);
            }
        }
    }
}
