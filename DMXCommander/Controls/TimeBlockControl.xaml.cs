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

namespace DMXCommander.Controls
{
    /// <summary>
    /// Interaction logic for TimeBlockControl.xaml
    /// </summary>
    public partial class TimeBlockControl : UserControl
    {
        public TimeBlockControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(TimeBlock),
           typeof(TimeBlockControl));

        public TimeBlock Data
        {
            get
            {
                return (TimeBlock)this.UIThreadGetValue(DataProperty);

            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);

            }
        }

        private void OnAddSetting(object sender, RoutedEventArgs e)
        {
            SetValue value = new SetValue();
            Data.Values.Add(value);

        }

        private void OnMoveUp(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                SetValue value = btn.CommandParameter as SetValue;
                if (value != null)
                {
                    int pos = Data.Values.IndexOf(value);
                    if (pos > 0)
                    {
                        Data.Values.Remove(value);
                        Data.Values.Insert(pos - 1, value);

                    }
                }
            }
        }

        private void OnMoveDown(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                SetValue value = btn.CommandParameter as SetValue;
                if (value != null)
                {
                    int pos = Data.Values.IndexOf(value);
                    if (pos < Data.Values.Count - 1)
                    {
                        Data.Values.Remove(value);


                        Data.Values.Insert(pos + 1, value);

                    }
                }
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                SetValue value = btn.CommandParameter as SetValue;
                if (value != null)
                {
                    Data.Values.Remove(value);
                }
            }
        }

      
    }
}
