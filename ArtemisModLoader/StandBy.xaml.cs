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
using System.Windows.Shapes;
using RussLibrary;
namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for StandBy.xaml
    /// </summary>
    public partial class StandBy : Window
    {
        public StandBy()
        {
            ModManagement.MessageEvent += new EventHandler<MessageEventArgs>(ModManagement_MessageEvent);
            InitializeComponent();
            this.Title = Locations.AssemblyTitle + " is processing.";
          
        }

        void ModManagement_MessageEvent(object sender, MessageEventArgs e)
        {
            Message = e.Message;
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
    }
}
