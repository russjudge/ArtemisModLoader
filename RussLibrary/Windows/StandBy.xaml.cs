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

namespace RussLibrary.Windows
{
    /// <summary>
    /// Interaction logic for StandBy.xaml
    /// </summary>
    public partial class StandBy : Window
    {
        public StandBy()
        {
            
            InitializeComponent();
            //this.Title = Locations.AssemblyTitle + " is processing.";

        }
        public void UpdateMessage(string message)
        {
            Message = message;
        }
        
        public static readonly DependencyProperty MessageProperty =
           DependencyProperty.Register("Message", typeof(string),
           typeof(StandBy));



        public string Message
        {
            get
            {
                return (string)this.UIThreadGetValue(MessageProperty);

            }
            set
            {
                this.UIThreadSetValue(MessageProperty, value);

            }
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void uc_Closed(object sender, EventArgs e)
        {
            
        }
    }
}
