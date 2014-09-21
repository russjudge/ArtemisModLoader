using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ArtemisComm.BigRedButtonOfDeath.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            InitializeComponent();
            new Controller(this);
        }


        public static readonly DependencyProperty HostProperty =
           DependencyProperty.Register("Host", typeof(string),
               typeof(MainWindow), new PropertyMetadata("192.168.15.127"));

        public string Host
        {
            get
            {
                return (string)this.UIThreadGetValue(HostProperty);

            }
            set
            {
                this.UIThreadSetValue(HostProperty, value);

            }
        }

        public static readonly DependencyProperty PortProperty =
          DependencyProperty.Register("Port", typeof(int),
              typeof(MainWindow), new PropertyMetadata(2010));

        public int Port
        {
            get
            {
                return (int)this.UIThreadGetValue(PortProperty);

            }
            set
            {
                this.UIThreadSetValue(PortProperty, value);

            }
        }

        public void AlertAboutArtemisVersionConflict(string message)
        {
            if (this.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                this.Dispatcher.Invoke(new Action<string>(AlertAboutArtemisVersionConflict), message);
            }
            else
            {
                MessageBox.Show(message);
            }
        }
        PlayerShip[] ships = null;
        void UserShipSelect()
        {
   
            ShipSelector win = new ShipSelector(ships);
            if (win.ShowDialog() == true)
            {
                selectedShip = win.SelectedShip;
            }
           
        }

        int selectedShip = -1;
        public int GetShipSelection(PlayerShip[] shipList)
        {
           
            ships = shipList;

            this.Dispatcher.Invoke(new Action(UserShipSelect));
            //this.Dispatcher.Invoke(() => (new Action(UserShipSelect))());
            return selectedShip;

        }

        public event EventHandler ConnectRequested;




        //public static readonly DependencyProperty ShipsProperty =
        //  DependencyProperty.Register("Ships", typeof(ObservableCollection<string>),
        //      typeof(MainWindow));
        
        //public ObservableCollection<string> Ships
        //{
        //    get
        //    {
        //        return (ObservableCollection<string>)this.GetValue(ShipsProperty);

        //    }
        //    set
        //    {
        //        this.SetValue(ShipsProperty, value);

        //    }
        //}




        public static readonly DependencyProperty RedAlertEnabledProperty =
          DependencyProperty.Register("RedAlertEnabled", typeof(bool),
              typeof(MainWindow));

        public bool RedAlertEnabled
        {
            get
            {
                return (bool)this.UIThreadGetValue(RedAlertEnabledProperty);

            }
            set
            {
                this.UIThreadSetValue(RedAlertEnabledProperty, value);

            }
        }

        public static readonly DependencyProperty ShieldsRaisedProperty =
          DependencyProperty.Register("ShieldsRaised", typeof(bool),
              typeof(MainWindow));

        public bool ShieldsRaised
        {
            get
            {
                return (bool)this.UIThreadGetValue(ShieldsRaisedProperty);

            }
            set
            {
                this.UIThreadSetValue(ShieldsRaisedProperty, value);

            }
        }



        public static readonly DependencyProperty ConnectionStartedProperty =
          DependencyProperty.Register("ConnectionStarted", typeof(bool),
              typeof(MainWindow));

        public bool ConnectionStarted
        {
            get
            {
                return (bool)this.UIThreadGetValue(ConnectionStartedProperty);

            }
            set
            {
                this.UIThreadSetValue(ConnectionStartedProperty, value);

            }
        }


        public static readonly DependencyProperty GameRunningProperty =
          DependencyProperty.Register("GameRunning", typeof(bool),
              typeof(MainWindow));

        public bool GameRunning
        {
            get
            {
                return (bool)this.UIThreadGetValue(GameRunningProperty);

            }
            set
            {
                this.UIThreadSetValue(GameRunningProperty, value);

            }
        }


        public static readonly DependencyProperty SimulationRunningProperty =
          DependencyProperty.Register("SimulationRunning", typeof(bool),
              typeof(MainWindow));

        public bool SimulationRunning
        {
            get
            {
                return (bool)this.UIThreadGetValue(SimulationRunningProperty);

            }
            set
            {
                this.UIThreadSetValue(SimulationRunningProperty, value);

            }
        }


        public static readonly DependencyProperty SelfDestructRunningProperty =
          DependencyProperty.Register("SelfDestructRunning", typeof(bool),
              typeof(MainWindow));

        public bool SelfDestructRunning
        {
            get
            {
                return (bool)this.UIThreadGetValue(SelfDestructRunningProperty);

            }
            set
            {
                this.UIThreadSetValue(SelfDestructRunningProperty, value);

            }
        }
        public event EventHandler StartSelfDestruct;

        public event EventHandler CancelSelfDestruct;

        public event EventHandler DisconnectRequested;

        public void ConnectionLostWarning()
        {
            
        }

        public void ConnectionFailed()
        {
            ConnectionStarted = false;
        }

        public void GameStarted()
        {
            GameRunning = true;
            SimulationRunning = true;
        }

        public void GameEnded()
        {
            GameRunning = false;
            SimulationRunning = false;
            SelfDestructRunning = false;
        }
        public void SimulationEnded()
        {
            SimulationRunning = false;
            SelfDestructRunning = false;
        }
        private void OnConnect(object sender, RoutedEventArgs e)
        {
            if (ConnectRequested != null)
            {
                ConnectionStarted = true;
                ConnectRequested(this, EventArgs.Empty);
                
            }
        }

        private void OnSelfDestruct(object sender, RoutedEventArgs e)
        {
            if (StartSelfDestruct != null)
            {
                SelfDestructRunning = true;
                StartSelfDestruct(this, EventArgs.Empty);
                
            }
        }

        private void OnResetSelfDestruct(object sender, RoutedEventArgs e)
        {
            if (CancelSelfDestruct != null)
            {
                CancelSelfDestruct(this, EventArgs.Empty);
                SelfDestructRunning = false;
            }

        }

        private void OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }



        bool isDisposed = false;
        public event EventHandler Disposing;
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (Disposing != null)
                    {
                        Disposing(this, EventArgs.Empty);
                    }
                    isDisposed = true;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
