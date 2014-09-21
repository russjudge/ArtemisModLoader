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
using System.Collections.ObjectModel;

namespace RussLibrary.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {

        public static void Show(IEnumerable<ISettingsPanel> panels, string configurationPath)
        {
            SettingsWindow win = new SettingsWindow();
            if (panels != null)
            {
                foreach (ISettingsPanel panel in panels)
                {
                    panel.SetConfigurationPath(configurationPath);
                    panel.LoadSettings();
                    win.Configuration.Add(panel);
                    TabItem t = new TabItem();
                    t.Content = panel;
                    TextBlock txt = new TextBlock();
                    txt.Text = panel.Header;
                    txt.FontWeight = FontWeights.Bold;
                    t.Header = txt;
                    win.tb.Items.Add(t);
                }
            }
            win.Show();
        }
        private SettingsWindow()
        {
            Configuration = new ObservableCollection<ISettingsPanel>();
            InitializeComponent();
           
        }
       
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("SettingsCollection", typeof(ObservableCollection<ISettingsPanel>),
            typeof(SettingsWindow));


        public ObservableCollection<ISettingsPanel> Configuration
        {
            get
            {
                return (ObservableCollection<ISettingsPanel>)this.UIThreadGetValue(ConfigurationProperty);

            }
            private set
            {
                this.UIThreadSetValue(ConfigurationProperty, value);

            }
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetMaxSize();
          
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            foreach (ISettingsPanel panel in Configuration)
            {
                panel.SaveSettings();
            }
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            foreach (ISettingsPanel panel in Configuration)
            {
                panel.CancelChanges();
            }
            this.Close();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "tx")]
        private void TestClick(object sender, RoutedEventArgs e)
        {
            object tx = tb;

        }
        
    }
}
