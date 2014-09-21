using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Windows;
using System.Collections.ObjectModel;
using RussLibrary;
namespace ArtemisEngineeringPresets
{

    public class Preset : DependencyObject
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(Preset));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        public Preset()
        {
            Initialize(false);
        }
        public Preset(IList<int> energyLevels, IList<int> coolantLevels)
        {
            Initialize(false);
            if (energyLevels != null && coolantLevels != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    SystemLevels[i].EnergyLevel = energyLevels[i];
                    SystemLevels[i].CoolantLevel = coolantLevels[i];
                    SystemLevels[i].Index = i;
                    //TotalCoolantLevel += coolantLevels[i];
                }
            }
            AcceptChanges();

        }
        private Preset(bool isForOriginal)
        {
            Initialize(isForOriginal);
        }
        void Initialize(bool isForOriginal)
        {
            SystemLevels = new ObservableCollection<SystemLevel>();

            TotalCoolantLevel = 0;
            for (int i = 0; i < 8; i++)
            {
                SystemLevel s = new SystemLevel(100, 0);
                s.Index = i;
                s.ValueChanged += new RoutedEventHandler(s_ValueChanged);
                
                SystemLevels.Add(s);
            }
           

            if (!isForOriginal)
            {
                Original = new Preset(true);
            }
            else
            {
               
                Original = null;
            }

        }
        object lockObject = new object();
        void s_ValueChanged(object sender, RoutedEventArgs e)
        {
            lock (lockObject)
            {
                Changed = true;
                TotalCoolantLevel = 0;
                foreach (SystemLevel coolant in SystemLevels)
                {
                    TotalCoolantLevel += coolant.CoolantLevel;
                }
            }

        }
        public static int CoolantLevelToPreventOverheat(int energyLevel)
        {

            if (energyLevel <= 100)
                return 0;
            else if (energyLevel <= 150)
                return 0 + (energyLevel - 100) / 25;
            else if (energyLevel <= 190.0)
                return 2 + (energyLevel - 150) / 20;
            else if (energyLevel <= 220.0)
                return 4 + (energyLevel - 190) / 15;
            else if (energyLevel <= 250)
                return 6 + (energyLevel - 220) / 15;
            else
                return 8;
        }

        void SetOriginal()
        {
            Original = new Preset(true);
            for (int i = 0; i < 8; i++)
            {
                Original.SystemLevels[i].EnergyLevel = this.SystemLevels[i].EnergyLevel;
                Original.SystemLevels[i].CoolantLevel = this.SystemLevels[i].CoolantLevel;
                
            }
        }
        public void AcceptChanges()
        {
            Changed = false;
            SetOriginal();
        }
        public void RejectChanges()
        {
            for (int i = 0; i < 8; i++)
            {
                this.SystemLevels[i].EnergyLevel = Original.SystemLevels[i].EnergyLevel;
                this.SystemLevels[i].CoolantLevel = Original.SystemLevels[i].CoolantLevel;
            }
            Changed = false;
        }
        void EnergyLevels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
        }

        public Preset Original { get; private set; }


        public static readonly DependencyProperty ChangedProperty =
          DependencyProperty.Register("Changed", typeof(bool),
          typeof(Preset));

        public bool Changed
        {
            get
            {
                return (bool)this.UIThreadGetValue(ChangedProperty);
            }
            set
            {
                this.UIThreadSetValue(ChangedProperty, value);
            }
        }



        public static readonly DependencyProperty SystemLevelsProperty =
          DependencyProperty.Register("SystemLevels", typeof(ObservableCollection<SystemLevel>),
          typeof(Preset));

        public ObservableCollection<SystemLevel> SystemLevels
        {
            get
            {
                return (ObservableCollection<SystemLevel>)this.UIThreadGetValue(SystemLevelsProperty);
            }
            private set
            {
                this.UIThreadSetValue(SystemLevelsProperty, value);
            }
        }



        public static readonly DependencyProperty TotalCoolantLevelProperty =
            DependencyProperty.Register("TotalCoolantLevel", typeof(int),
            typeof(Preset));

        public int TotalCoolantLevel
        {
            get
            {
                return (int)this.UIThreadGetValue(TotalCoolantLevelProperty);
            }
            set
            {
                this.UIThreadSetValue(TotalCoolantLevelProperty, value);
            }
        }


        //public static readonly DependencyProperty EnergyLevelsProperty =
        //  DependencyProperty.Register("EnergyLevels", typeof(ObservableCollection<int>),
        //  typeof(Preset));

        //public ObservableCollection<int> EnergyLevels
        //{
        //    get
        //    {
        //        return (ObservableCollection<int>)this.UIThreadGetValue(EnergyLevelsProperty);
        //    }
        //    set
        //    {
        //        this.UIThreadSetValue(EnergyLevelsProperty, value);
        //    }
        //}
        //public static readonly DependencyProperty CoolantLevelsProperty =
        //DependencyProperty.Register("CoolantLevels", typeof(ObservableCollection<int>),
        //typeof(Preset));

        //public ObservableCollection<int> CoolantLevels
        //{
        //    get
        //    {
        //        return (ObservableCollection<int>)this.UIThreadGetValue(CoolantLevelsProperty);
        //    }
        //    set
        //    {
        //        this.UIThreadSetValue(CoolantLevelsProperty, value);
        //    }
        //}
    }
}
