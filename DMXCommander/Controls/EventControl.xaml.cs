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
using System.Collections.ObjectModel;

namespace DMXCommander.Controls
{
    /// <summary>
    /// Interaction logic for EventControl.xaml
    /// </summary>
    public partial class EventControl : UserControl
    {
        public EventControl()
        {
            Cues = GeneralHelper.GetCues();
            InitializeComponent();
        }
        public static readonly DependencyProperty DataProperty =
          DependencyProperty.Register("Data", typeof(EventObject),
          typeof(EventControl));

        public EventObject Data
        {
            get
            {
                return (EventObject)this.UIThreadGetValue(DataProperty);

            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);

            }
        }


        public static readonly DependencyProperty CuesProperty =
         DependencyProperty.Register("Cues", typeof(ObservableCollection<string>),
         typeof(EventControl));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<string> Cues
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(CuesProperty);
            }
            set
            {
                this.UIThreadSetValue(CuesProperty, value);
            }
        }

        private void OnAddTimeBlock(object sender, RoutedEventArgs e)
        {
            TimeBlock tb = new TimeBlock();
            Data.TimeBlocks.Add(tb);
        }

        private void OnMoveUp(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                TimeBlock ev = btn.CommandParameter as TimeBlock;
                if (ev != null)
                {
                    int pos = Data.TimeBlocks.IndexOf(ev);
                    if (pos > 0)
                    {
                        Data.TimeBlocks.Remove(ev);
                        Data.TimeBlocks.Insert(pos - 1, ev);

                    }
                }
            }
        }

        private void OnMoveDown(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                TimeBlock ev = btn.CommandParameter as TimeBlock;
                if (ev != null)
                {
                    int pos = Data.TimeBlocks.IndexOf(ev);
                    if (pos < Data.TimeBlocks.Count - 1)
                    {
                        Data.TimeBlocks.Remove(ev);


                        Data.TimeBlocks.Insert(pos + 1, ev);

                    }
                }
            }
        }

        public static readonly RoutedEvent EventTypeChangedEvent =
            EventManager.RegisterRoutedEvent(
            "EventTypeChanged", RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(EventControl));

        public event RoutedEventHandler EventTypeChanged
        {
            add { AddHandler(EventTypeChangedEvent, value); }
            remove { RemoveHandler(EventTypeChangedEvent, value); }
        }


        private void OnSelectedEventTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox me = sender as ComboBox;
            if (me != null && me.SelectedValue != null)
            {
                RaiseEvent(new RoutedEventArgs(EventTypeChangedEvent, me.SelectedValue));
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
             Button btn = sender as Button;
             if (btn != null)
             {
                 TimeBlock ev = btn.CommandParameter as TimeBlock;
                 if (ev != null)
                 {
                     Data.TimeBlocks.Remove(ev);
                 }
             }
        }

    }
}
