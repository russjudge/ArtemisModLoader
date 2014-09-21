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
        }


        public static readonly DependencyProperty HostProperty =
           DependencyProperty.Register("Host", typeof(string),
               typeof(MainWindow), new PropertyMetadata("192.168.15.127"));

        public string Host
        {
            get
            {
                return (string)this.GetValue(HostProperty);

            }
            set
            {
                this.SetValue(HostProperty, value);

            }
        }
        public static readonly DependencyProperty PortProperty =
          DependencyProperty.Register("Port", typeof(int),
              typeof(MainWindow), new PropertyMetadata(2010));

        public int Port
        {
            get
            {
                return (int)this.GetValue(PortProperty);

            }
            set
            {
                this.SetValue(PortProperty, value);

            }
        }

        public void AlertAboutArtemisVersionConflict(string message)
        {
            throw new NotImplementedException();
        }

        public int GetShipSelection(PlayerShip[] shipList)
        {
            throw new NotImplementedException();
        }

        public event EventHandler ConnectRequested;

        public bool RedAlertEnabled
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool ShieldsRaised
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler StartSelfDestruct;

        public event EventHandler CancelSelfDestruct;

        public event EventHandler DisconnectRequested;

        public void ConnectionLostWarning()
        {
            throw new NotImplementedException();
        }

        public void ConnectionFailed()
        {
            throw new NotImplementedException();
        }

        public void GameStarted()
        {
            throw new NotImplementedException();
        }

        public void GameEnded()
        {
            throw new NotImplementedException();
        }
    }
}
