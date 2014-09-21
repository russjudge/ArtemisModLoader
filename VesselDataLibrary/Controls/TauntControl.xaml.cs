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
using VesselDataLibrary.Xml;
using RussLibrary;
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for TauntControl.xaml
    /// </summary>
    public partial class TauntControl : UserControl
    {
        public TauntControl()
        {
            InitializeComponent();
           
        }

        public static readonly DependencyProperty TauntsProperty =
            DependencyProperty.Register("Taunts", typeof(TauntCollection),
            typeof(TauntControl));

        public TauntCollection Taunts
        {
            get
            {
                return (TauntCollection)this.UIThreadGetValue(TauntsProperty);
            }
            set
            {
                this.UIThreadSetValue(TauntsProperty, value);
            }
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                Taunt t = b.CommandParameter as Taunt;
                if (t != null)
                {
                    Taunts.Remove(t);
                }
            }
        }

        private void OnAdd(object sender, RoutedEventArgs e)
        {
            Taunts.Add(new Taunt());
        }

    }
}
