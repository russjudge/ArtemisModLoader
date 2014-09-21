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
using Microsoft.Win32;
using System.IO;
namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for ModDefinitionSetup.xaml
    /// </summary>
    public partial class ModDefinitionSetup : Window
    {
        public ModDefinitionSetup()
        {
            Configuration = new ModConfiguration();
            InitializeComponent();
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

        private void NewGUID_click(object sender, RoutedEventArgs e)
        {
            Configuration.ID = Guid.NewGuid().ToString();
        }
    

        private void Filename_click(object sender, RoutedEventArgs e)
        {
            Configuration.ID = new FileInfo(Configuration.PackagePath).Name.Replace('.', '~');
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

    }
}
