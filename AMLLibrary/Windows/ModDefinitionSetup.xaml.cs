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
using System.IO;
using Microsoft.Win32;
namespace ArtemisModLoader.Windows
{
    /// <summary>
    /// Interaction logic for ModDefinitionSetup.xaml
    /// </summary>
    public partial class ModDefinitionSetup : Window
    {
        public ModDefinitionSetup()
        {

            Configuration = new ModConfiguration();

            Configuration.AcceptChanges();
            InitializeComponent();
            ImageBrush brsh = ArtemisModLoader.Helpers.FileHelper.GetRandomSkybox();
            if (brsh != null)
            {
                this.Background = brsh;
            }
        }

        //ID
        //zip package
        //Title
        //Description
        //Download source
        //Download Web page.
        public static readonly DependencyProperty ConfigurationProperty =
            DependencyProperty.Register("Configuration", typeof(ModConfiguration),
            typeof(ModDefinitionSetup));


        public ModConfiguration Configuration
        {
            get
            {
                return (ModConfiguration)this.UIThreadGetValue(ConfigurationProperty);

            }
            private set
            {
                this.UIThreadSetValue(ConfigurationProperty, value);

            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.Title = AMLResources.Properties.Resources.SaveModDefinitionFile;
            diag.Filter = AMLResources.Properties.Resources.AML + DataStrings.AMLFilter;
            diag.DefaultExt = DataStrings.DefaultAMLExtension;
            diag.OverwritePrompt = true;
            if (diag.ShowDialog() == true)
            {
                Configuration.Save(diag.FileName);
                this.DialogResult = true;
                this.Close();
            }
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetMaxSize();
        }

    }
}
