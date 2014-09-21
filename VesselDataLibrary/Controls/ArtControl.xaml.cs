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
using ArtemisModLoader.Xml;
using RussLibrary;
using VesselDataLibrary.Xml;
using RussLibrary.Controls;
using ArtemisModLoader.Helpers;
using System.Collections.ObjectModel;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for ArtControl.xaml
    /// </summary>
    public partial class ArtControl : UserControl
    {
        public ArtControl()
        {
            InitializeComponent();
            
        }

        public static readonly DependencyProperty SearchPrefixesProperty =
          DependencyProperty.Register("SearchPrefixes", typeof(ObservableCollection<string>),
          typeof(ArtControl));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<string> SearchPrefixes
        {
            get
            {
                return (ObservableCollection<string>)this.UIThreadGetValue(SearchPrefixesProperty);

            }
            set
            {
                this.UIThreadSetValue(SearchPrefixesProperty, value);

            }
        }


        public static readonly DependencyProperty SelectedArtProperty =
           DependencyProperty.Register("SelectedArt", typeof(ArtDefinition),
           typeof(ArtControl));

        public ArtDefinition SelectedArt
        {
            get
            {
                return (ArtDefinition)this.UIThreadGetValue(SelectedArtProperty);
            }
            set
            {
                this.UIThreadSetValue(SelectedArtProperty, value);
            }
        }


        static void OnDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArtControl me = sender as ArtControl;
            if (me != null)
            {
                if (me.Data != null && me.Data.Art != null && me.Data.Art.Count > 0)
                {
                    if (me.Index == 0)
                    {
                        me.SelectedArt = me.Data.Art[0];
                    }
                    else
                    {
                        me.Index = 0;
                    }
                }
                else
                {
                    me.Index = -1;
                }
            }
        }
        static void OnIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ArtControl me = sender as ArtControl;
            if (me != null)
            {
                if (me.Index >= 0)
                {
                    me.SelectedArt = me.Data.Art[me.Index];

                }
                else
                {
                    me.SelectedArt = null;
                }
            }
        }
        public static readonly DependencyProperty IndexProperty =
         DependencyProperty.Register("Index", typeof(int),
         typeof(ArtControl), new PropertyMetadata(OnIndexChanged));

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
        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(Vessel),
           typeof(ArtControl), new PropertyMetadata(OnDataChanged));

        public Vessel Data
        {
            get
            {
                return (Vessel)this.UIThreadGetValue(DataProperty);
            }
            set
            {
                this.UIThreadSetValue(DataProperty, value);
            }
        }
        public static readonly DependencyProperty ConfigurationProperty =
         DependencyProperty.Register("Configuration", typeof(ModConfiguration),
         typeof(ArtControl));


        public ModConfiguration Configuration
        {
            get
            {
                return (ModConfiguration)this.UIThreadGetValue(ConfigurationProperty);

            }
            set
            {
                this.UIThreadSetValue(ConfigurationProperty, value);

            }
        }

        private void FileSelectionControl_InvalidFilePath(object sender, RoutedEventArgs e)
        {
            FileHelper.FileSelectionControl_InvalidFilePath(sender, e, Configuration);
            
        }

        private void DeleteArt_Click(object sender, RoutedEventArgs e)
        {
            int idx = Index;
            Data.Art.Remove(this.SelectedArt);
            if (Data.Art.Count <= idx)
            {
                Index = Data.Art.Count - 1;
            }
            else
            {
                Index = idx;
            }
        }

        private void AddArt_Click(object sender, RoutedEventArgs e)
        {
            ArtDefinition art = new ArtDefinition();
            art.AcceptChanges();
            Data.Art.Add(art);
            Index = Data.Art.Count - 1;
        }

    }
}
