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
using System.Collections.ObjectModel;
using VesselDataLibrary.Xml;
using RussLibrary;

namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for BeamPortStack.xaml
    /// </summary>
    public partial class BeamPortStack : UserControl
    {
        public BeamPortStack()
        {
            InitializeComponent();
            
        }
        static void OnBeamPortsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamPortStack me = sender as BeamPortStack;
            if (me != null)
            {
                BeamPortCollection old = e.OldValue as BeamPortCollection;
                if (old != null)
                {
                    old.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(me.BeamPorts_CollectionChanged); 
                }
                if (me.BeamPorts != null)
                {
                    me.BeamPorts.CollectionChanged+=new System.Collections.Specialized.NotifyCollectionChangedEventHandler(me.BeamPorts_CollectionChanged);
                   // me.SelectedIndex = me.BeamPorts.Count;
                }
            }
        }

        void BeamPorts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //SetSelectedItemMask();
            SelectedIndex = e.NewStartingIndex;
        }
        public static readonly DependencyProperty BeamPortsProperty =
           DependencyProperty.Register("BeamPorts", typeof(BeamPortCollection),
           typeof(BeamPortStack), new PropertyMetadata(OnBeamPortsChanged));

        public BeamPortCollection BeamPorts
        {
            get
            {
                return (BeamPortCollection)this.UIThreadGetValue(BeamPortsProperty);
            }
            set
            {
                this.UIThreadSetValue(BeamPortsProperty, value);
            }
        }

        static void OnWallRatioChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        

        public static readonly DependencyProperty WallRatioProperty =
          DependencyProperty.Register("WallRatio", typeof(double),
          typeof(BeamPortStack), new PropertyMetadata(OnWallRatioChanged));

        public double WallRatio
        {
            get
            {
                return (double)this.UIThreadGetValue(WallRatioProperty);

            }
            set
            {
                this.UIThreadSetValue(WallRatioProperty, value);

            }
        }

        static void OnSelectedIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BeamPortStack me = sender as BeamPortStack;
            if (me != null)
            {
                me.SetSelectedItemMask();
                
            }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register("SelectedIndex", typeof(int),
        typeof(BeamPortStack), new PropertyMetadata(-1, OnSelectedIndexChanged));

        public int SelectedIndex
        {
            get
            {
                return (int)this.UIThreadGetValue(SelectedIndexProperty);

            }
            set
            {
                this.UIThreadSetValue(SelectedIndexProperty, value);

            }
        }

        public static readonly DependencyProperty SelectedItemProperty =
        DependencyProperty.Register("SelectedItem", typeof(BeamPort),
        typeof(BeamPortStack));

        public BeamPort SelectedItem
        {
            get
            {
                return (BeamPort)this.UIThreadGetValue(SelectedItemProperty);

            }
            set
            {
                this.UIThreadSetValue(SelectedItemProperty, value);

            }
        }

        void SetSelectedItemMask()
        {
            if (SelectedIndex > -1 && SelectedIndex < BeamPorts.Count)
            {
                SelectedItemMask.Visibility = Visibility.Visible;
                SelectedItem = ic.Items[SelectedIndex] as BeamPort;
                SelectedItemMask.ArcWidth = SelectedItem.ArcWidth;
                SelectedItemMask.Range = SelectedItem.Range;
                SelectedItemMask.X = SelectedItem.X;
                SelectedItemMask.Z = SelectedItem.Z;

            }
            else
            {
                SelectedItemMask.Visibility = Visibility.Collapsed;
            }
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            SetSelectedItemMask();
        }


    }
}
