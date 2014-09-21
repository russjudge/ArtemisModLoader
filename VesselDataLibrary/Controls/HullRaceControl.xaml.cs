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
using VesselDataLibrary.Xml;

namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for HullRaceControl.xaml
    /// </summary>
    public partial class HullRaceControl : UserControl
    {
        public HullRaceControl()
        {
            Group1 = Guid.NewGuid().ToString();
            Group2 = Guid.NewGuid().ToString();
            Group3 = Guid.NewGuid().ToString();
            InitializeComponent();
            
        }

        public static readonly DependencyProperty Group1Property =
           DependencyProperty.Register("Group1", typeof(string),
           typeof(HullRaceControl));

        public string Group1
        {
            get
            {
                return (string)this.UIThreadGetValue(Group1Property);
            }
            set
            {
                this.UIThreadSetValue(Group1Property, value);
            }
        }


        public static readonly DependencyProperty Group2Property =
           DependencyProperty.Register("Group2", typeof(string),
           typeof(HullRaceControl));

        public string Group2
        {
            get
            {
                return (string)this.UIThreadGetValue(Group2Property);
            }
            set
            {
                this.UIThreadSetValue(Group2Property, value);
            }
        }

        public static readonly DependencyProperty Group3Property =
           DependencyProperty.Register("Group3", typeof(string),
           typeof(HullRaceControl));

        public string Group3
        {
            get
            {
                return (string)this.UIThreadGetValue(Group3Property);
            }
            set
            {
                this.UIThreadSetValue(Group3Property, value);
            }
        }

        void SetInternalValuesFromDataValues()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;

                IsFriendlyShip = false;
                IsPlayerShip = false;
                IsEnemyShip = false;
                IsEnemySupport = false;
                IsEnemyStandard = false;
                IsEnemyLoner = false;
                IsEnemyElite = false;
                IsEnemyBiomech = false;
                IsEnemyWhaleLover = false;
                IsEnemyWhaleHater = false;
                IsEnemyWhaleIndifferent = true;
                if (!string.IsNullOrEmpty(Data.Keys))
                {
                    foreach (string modifier in Data.Keys.Split(' '))
                    {
                        SetEnemyValue(modifier);
                    }
                }
                IsUpdating = false;
            }
        }




        void SetEnemyValue(string key)
        {
            switch (key.ToUpperInvariant())
            {
                case "PLAYER":
                    IsPlayerShip = true;
                    break;
                case "FRIENDLY":
                    IsFriendlyShip = true;
                    break;
                case "ENEMY":
                    IsEnemyShip = true;
                    break;
                case "SUPPORT":
                    IsEnemySupport = true;
                    break;
                case "STANDARD":
                    IsEnemyStandard = true;
                    break;
                case "LONER":
                    IsEnemyLoner = true;
                    break;
                case "ELITE":
                    IsEnemyElite = true;
                    break;
                case "WHALEHATER":
                    IsEnemyWhaleIndifferent = false;
                    IsEnemyWhaleHater = true;
                    
                    break;
                case "WHALELOVER":
                    IsEnemyWhaleIndifferent = false;
                    IsEnemyWhaleLover = true;
                    break;
                case "BIOMECH":
                    IsEnemyBiomech = true;
                    break;

            }

        }
        static void OnDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            HullRaceControl me = sender as HullRaceControl;
            if (me != null)
            {
                me.SetInternalValuesFromDataValues();
            }
        }
        private bool IsUpdating = false;
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(HullRace),
            typeof(HullRaceControl), new PropertyMetadata(OnDataChanged));

        public HullRace Data
        {
            get
            {
                return (HullRace)this.UIThreadGetValue(DataProperty);
            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);
            }
        }
        void SetDataValuesFromInternalValues()
        {
            if (!IsUpdating)
            {
                IsUpdating = true;
                if (IsPlayerShip)
                {
                    Data.Keys = "player";
                }
                else if (IsFriendlyShip)
                {
                    Data.Keys = "friendly";
                }
                else //IsEnemyShip
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("enemy");
                    if (IsEnemyStandard)
                    {
                            sb.Append(" standard");
                    }
                    else if (IsEnemySupport)
                    {
                        sb.Append(" support");
                    }
                    else if (IsEnemyLoner)
                    {
                        sb.Append(" loner");

                    }
                    if (IsEnemyElite)
                    {
                        sb.Append(" elite");
                    }
                    if (IsEnemyBiomech)
                    {
                        sb.Append(" biomech");
                    }
                    if (IsEnemyWhaleHater)
                    {
                        sb.Append(" whalehater");
                    }
                    else if (IsEnemyWhaleLover)
                    {
                        sb.Append(" whalelover");
                    }
                    if (sb.ToString().ToUpperInvariant() == "ENEMY")
                    {
                        IsEnemyStandard = true;
                        sb.Append(" standard");
                    }
                    Data.Keys = sb.ToString();
                }
                IsUpdating = false;
            }
        }
        static void OnKeyChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            HullRaceControl me = sender as HullRaceControl;
            if (me != null)
            {
                me.SetDataValuesFromInternalValues();
            }
        }

        public static readonly DependencyProperty IsPlayerShipProperty =
            DependencyProperty.Register("IsPlayerShip", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsPlayerShip
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsPlayerShipProperty);
            }
            set
            {
                this.UIThreadSetValue(IsPlayerShipProperty, value);
            }
        }


        public static readonly DependencyProperty IsFriendlyShipProperty =
            DependencyProperty.Register("IsFriendlyShip", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsFriendlyShip
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsFriendlyShipProperty);
            }
            set
            {
                this.UIThreadSetValue(IsFriendlyShipProperty, value);
            }
        }


        public static readonly DependencyProperty IsEnemyShipProperty =
            DependencyProperty.Register("IsEnemyShip", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemyShip
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemyShipProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemyShipProperty, value);
            }
        }


        public static readonly DependencyProperty IsEnemyStandardProperty =
            DependencyProperty.Register("IsEnemyStandard", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemyStandard
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemyStandardProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemyStandardProperty, value);
            }
        }


        public static readonly DependencyProperty IsEnemySupportProperty =
            DependencyProperty.Register("IsEnemySupport", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemySupport
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemySupportProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemySupportProperty, value);
            }
        }


        public static readonly DependencyProperty IsEnemyLonerProperty =
            DependencyProperty.Register("IsEnemyLoner", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemyLoner
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemyLonerProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemyLonerProperty, value);
            }
        }



        public static readonly DependencyProperty IsEnemyEliteProperty =
            DependencyProperty.Register("IsEnemyElite", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemyElite
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemyEliteProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemyEliteProperty, value);
            }
        }

        public static readonly DependencyProperty IsEnemyBiomechProperty =
            DependencyProperty.Register("IsEnemyBiomech", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemyBiomech
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemyBiomechProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemyBiomechProperty, value);
            }
        }
        public static readonly DependencyProperty IsEnemyWhaleHaterProperty =
            DependencyProperty.Register("IsEnemyWhaleHater", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemyWhaleHater
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemyWhaleHaterProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemyWhaleHaterProperty, value);
            }
        }


        public static readonly DependencyProperty IsEnemyWhaleLoverProperty =
            DependencyProperty.Register("IsEnemyWhaleLover", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemyWhaleLover
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemyWhaleLoverProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemyWhaleLoverProperty, value);
            }
        }

        public static readonly DependencyProperty IsEnemyWhaleIndifferentProperty =
            DependencyProperty.Register("IsEnemyWhaleIndifferent", typeof(bool),
            typeof(HullRaceControl), new PropertyMetadata(OnKeyChange));

        public bool IsEnemyWhaleIndifferent
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsEnemyWhaleIndifferentProperty);
            }
            set
            {
                this.UIThreadSetValue(IsEnemyWhaleIndifferentProperty, value);
            }
        }

       
    }
}
