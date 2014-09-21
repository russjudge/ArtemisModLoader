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
namespace ArtemisEngineeringPresets
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ShipSystemControl : UserControl
    {
        public ShipSystemControl()
        {

            InitializeComponent();
          
        }
        static void OnLevelsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        public static readonly DependencyProperty LevelsProperty =
         DependencyProperty.Register("Levels", typeof(SystemLevel),
         typeof(ShipSystemControl), new PropertyMetadata(OnLevelsChanged));

        public SystemLevel Levels
        {
            get
            {
                return (SystemLevel)this.UIThreadGetValue(LevelsProperty);
            }
            set
            {
                this.UIThreadSetValue(LevelsProperty, value);
            }
        }
        

        
    }
}
