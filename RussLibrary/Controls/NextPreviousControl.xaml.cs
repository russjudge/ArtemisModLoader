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

namespace RussLibrary.Controls
{
    /// <summary>
    /// Interaction logic for NextPreviousControl.xaml
    /// </summary>
    public partial class NextPreviousControl : UserControl
    {
        public NextPreviousControl()
        {
            InitializeComponent();
        }
        static void OnCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NextPreviousControl me = sender as NextPreviousControl;
            if (me != null)
            {
                if (me.Count > 0)
                {
                    if (me.Index != 0)
                    {
                        me.Index = 0;
                    }
                    else
                    {
                        me.IsLastItem = (me.Count <= me.Index + 1);
                    }
                }
                else
                {
                    me.Index = -1;
                }
            }
        }
        public static readonly DependencyProperty IsLastItemProperty =
           DependencyProperty.Register("IsLastItem", typeof(bool),
           typeof(NextPreviousControl));

        public bool IsLastItem
        {
            get
            {
                return (bool)this.UIThreadGetValue(IsLastItemProperty);
            }
            set
            {
                this.UIThreadSetValue(IsLastItemProperty, value);
            }
        }

        public static readonly DependencyProperty CountProperty =
           DependencyProperty.Register("Count", typeof(int),
           typeof(NextPreviousControl), new PropertyMetadata(OnCountChanged));

        public int Count
        {
            get
            {
                return (int)this.UIThreadGetValue(CountProperty);
            }
            set
            {
                this.UIThreadSetValue(CountProperty, value);
            }
        }
        public static readonly DependencyProperty IndexProperty =
         DependencyProperty.Register("Index", typeof(int),
         typeof(NextPreviousControl), new PropertyMetadata(OnIndexChanged));

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
        static void OnIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            NextPreviousControl me = sender as NextPreviousControl;
            if (me != null)
            {
                me.IsLastItem = (me.Count <= me.Index + 1);
            }
        }
        private void First_Click(object sender, RoutedEventArgs e)
        {
            Index = 0;
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            Index--;
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Index++;
        }

        private void Last_Click(object sender, RoutedEventArgs e)
        {
            Index = Count - 1;
        }
    }
}
