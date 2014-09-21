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

namespace ArtemisEngineeringPresets
{
    /// <summary>
    /// Interaction logic for PresetSettingsControl.xaml
    /// </summary>
    public partial class PresetSettingsControl : UserControl
    {
        public PresetSettingsControl()
        {
            InitializeComponent();
        }
        static void OnPresetItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            PresetSettingsControl me = sender as PresetSettingsControl;
            if (me != null)
            {
                me.tb.SelectedIndex = -1;
                me.Dispatcher.BeginInvoke(new Action(me.SelectFirstItem), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
        void SelectFirstItem()
        {
            tb.SelectedIndex = 0;
        }
        public static readonly DependencyProperty PresetItemsProperty =
           DependencyProperty.Register("PresetItems", typeof(ObservableCollection<Preset>),
           typeof(PresetSettingsControl), new PropertyMetadata(OnPresetItemsChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public ObservableCollection<Preset> PresetItems
        {
            get
            {
                return (ObservableCollection<Preset>)this.UIThreadGetValue(PresetItemsProperty);
            }
            set
            {
                this.UIThreadSetValue(PresetItemsProperty, value);
            }
        }
    }
}
