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
using log4net;
using System.Reflection;

namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for ActivatedMods.xaml
    /// </summary>
    public partial class ActivatedMods : UserControl
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ActivatedMods));
        public ActivatedMods()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            InitializeComponent();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }


        private void ToAndroid_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                ModConfiguration config = btn.CommandParameter as ModConfiguration;
                if (config != null)
                {
                    Locations.MessageBoxShow(AMLResources.Properties.Resources.AttachAndroid,
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    System.Windows.Forms.FolderBrowserDialog diag = new System.Windows.Forms.FolderBrowserDialog();
                    diag.Description = AMLResources.Properties.Resources.BrowseToFolder;
                    if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Locations.CopyFiles(new System.IO.DirectoryInfo(config.InstalledPath), diag.SelectedPath, "*.snt");
                        Locations.CopyFiles(new System.IO.DirectoryInfo(config.InstalledPath), diag.SelectedPath, "*.xml");
                        Locations.MessageBoxShow(
                            AMLResources.Properties.Resources.CopyComplete 
                            + DataStrings.CRCR
                            + AMLResources.Properties.Resources.ApplyToAndroid,
                             MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }
     

        private void Deactivate_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                ModConfiguration mod = btn.CommandParameter as ModConfiguration;
                if (mod != null)
                {
                    ModManagement.DeactivateLastMod();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                ModConfiguration mod = btn.CommandParameter as ModConfiguration;
                if (mod != null)
                {
                    System.Diagnostics.Process.Start(mod.Download.Webpage);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
    }
}
