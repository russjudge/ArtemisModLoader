using DMXCommander.Xml;
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
using RussLibrary.Windows;
using RussLibrary.Xml;
using RussLibrary.Helpers;
using System.Xml;
using System.Collections.ObjectModel;
using DMXCommander.Engine;
namespace DMXCommander.Controls
{
    /// <summary>
    /// Interaction logic for ChannelDefintionControl.xaml
    /// </summary>
    public partial class ChannelDefintionControl : UserControl, IDisposable
    {
        public ChannelDefintionControl()
        {
            AvailableChannels = new ObservableCollection<int>();
            for (short i = 0; i < 512; i++)
            {
                AvailableChannels.Add(i);
            }

            Data = DMXConfigurationFile.Current;
            try
            {
                if (OpenDMX.IsStarted)
                {
                    OpenDMX.Stop();
                }
                OpenDMX.Start();
                DMXDeviceEnabled = OpenDMX.IsStarted;
            }
            catch
            { 
            }
            InitializeComponent();

        }


        bool DMXDeviceEnabled = false;


        public static readonly DependencyProperty AvailableChannelsProperty =
            DependencyProperty.Register("AvailableChannels", typeof(ObservableCollection<int>),
            typeof(ChannelDefintionControl));

        public ObservableCollection<int> AvailableChannels
        {
            get
            {
                return (ObservableCollection<int>)this.UIThreadGetValue(AvailableChannelsProperty);

            }
            private set
            {
                this.UIThreadSetValue(AvailableChannelsProperty, value);

            }
        }

        
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(DMXConfigurationFile),
            typeof(ChannelDefintionControl));
        
        public DMXConfigurationFile Data
        {
            get
            {
                return (DMXConfigurationFile)this.UIThreadGetValue(DataProperty);

            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);

            }
        }

        private void OnAddChannel(object sender, RoutedEventArgs e)
        {
            if (Data.Definitions.Count < 512)
            {
                ChannelDefinition def = new ChannelDefinition();
                def.Channel = Convert.ToInt16(Data.Definitions.Count);
                Data.Definitions.Add(def);
            }
            else
            {
                MessageBox.Show("Maximum Channels defined already.", "DMX Commander", MessageBoxButton.OK, MessageBoxImage.Hand);

            }
        }

        private void OnAddGroup(object sender, RoutedEventArgs e)
        {
            PromptDialog diag = new PromptDialog();
            diag.Title = "Channel Definition";
            diag.Text = "Please enter group name:";
            if (diag.ShowDialog() == true)
            {
                GroupName grp = new GroupName();
                grp.Name = diag.Text;
                Data.Groups.Add(grp);
            }

        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            DMXConfigurationFile.Save();
            MessageBox.Show("Configuration Saved.",
                "DMX Commander Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        System.Timers.Timer TestTimer = null;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
        private void OnSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (DMXDeviceEnabled)
            {
                Slider me = sender as Slider;
                if (me.Value > 0)
                {
                    if (TestTimer == null)
                    {
                        TestTimer = new System.Timers.Timer(35);
                        TestTimer.Elapsed += TestTimer_Elapsed;
                        TestTimer.AutoReset = true;
                        TestTimer.Start();
                    }
                }
                else
                {
                    if (TestTimer != null)
                    {
                        bool AllZero = true;
                        foreach (ChannelDefinition def in Data.Definitions)
                        {
                            if (def.TestValue > 0)
                            {
                                AllZero = false;
                                break;
                            }
                        }
                        if (AllZero)
                        {
                            StopTimer = true;

                        }
                    }
                }
            }
        }
        bool StopTimer = false;
        void TestTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (DMXDeviceEnabled)
            {
                if (Data != null && Data.Definitions != null)
                {
                    foreach (ChannelDefinition def in Data.Definitions)
                    {
                        //DMXEngine.Current.Write(def.Channel, def.TestValue);
                        OpenDMX.SetDmxValue(def.Channel, def.TestValue);

                    }

                }
                else
                {
                    StopTimer = true;
                }
            }
            else
            {
                StopTimer = true;
            }
            if (StopTimer)
            {
                TestTimer.Stop();
                TestTimer.Dispose();
                TestTimer = null;
                StopTimer = false;
            }

        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (TestTimer != null)
                    {
                        TestTimer.Stop();
                        TestTimer.Dispose();
                        TestTimer = null;
                    }
                    isDisposed = true;
                }
            }
        }
        bool isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Dispose(true);
            if (DMXDeviceEnabled)
            {
                foreach (ChannelDefinition def in Data.Definitions)
                {
                    if (def.TestValue > 0)
                    {
                        def.TestValue = 0;
                        OpenDMX.SetDmxValue(def.Channel, def.TestValue);
                    }
                }
            }
        }
    }
}
