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
using RussLibrary;
using System.Windows.Controls.Primitives;

namespace DMXCommander.Controls
{
    /// <summary>
    /// Interaction logic for CueControl.xaml
    /// </summary>
    public partial class CueControl : UserControl
    {
        public CueControl()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty DataProperty =
         DependencyProperty.Register("Data", typeof(ObservableCollection<string>),
         typeof(CueControl));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<string> Data
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(DataProperty);
            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);
            }
        }


        public static readonly RoutedEvent ActivateCueEvent =
          EventManager.RegisterRoutedEvent(
          "ActivateCue", RoutingStrategy.Direct,
          typeof(RoutedEventHandler),
          typeof(CueControl));

        public event RoutedEventHandler ActivateCue
        {
            add { AddHandler(ActivateCueEvent, value); }
            remove { RemoveHandler(ActivateCueEvent, value); }
        }
        public static readonly RoutedEvent DeactivateCueEvent =
         EventManager.RegisterRoutedEvent(
         "DeactivateCue", RoutingStrategy.Direct,
         typeof(RoutedEventHandler),
         typeof(CueControl));

        public event RoutedEventHandler DeactivateCue
        {
            add { AddHandler(DeactivateCueEvent, value); }
            remove { RemoveHandler(DeactivateCueEvent, value); }
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;
            if (btn != null)
            {
                TextBlock txt = btn.Content as TextBlock;
                if (txt != null)
                {
                    this.RaiseEvent(new RoutedEventArgs(ActivateCueEvent, txt.Text));
                }
            }
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = sender as ToggleButton;
            if (btn != null)
            {
                TextBlock txt = btn.Content as TextBlock;
                this.RaiseEvent(new RoutedEventArgs(DeactivateCueEvent, txt.Text));
            }
        }
        public void SortByName()
        {
            List<string> cues = new List<string>(Data);
            cues.Sort();
            Data = new ObservableCollection<string>(cues);
        }
        public void SortByPriority()
        {

        }
    }
}
