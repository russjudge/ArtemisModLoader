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
using System.Reflection;
using log4net;


namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for InstalledMods.xaml
    /// </summary>
    public partial class InstalledMods : UserControl
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(InstalledMods));
        public InstalledMods()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            InitializeComponent();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

       
        
        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                ModConfiguration mod = btn.CommandParameter as ModConfiguration;
                if (mod != null)
                {
                    ModManagement.Activate(mod);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                ModConfiguration mod = btn.CommandParameter as ModConfiguration;
                if (mod != null)
                {
                    ModManagement.Uninstall(mod);
                    this.RaiseEvent(new RoutedEventArgs(ModUninstalledEvent, mod));
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
        public static readonly RoutedEvent ModUninstalledEvent =
            EventManager.RegisterRoutedEvent(
            "ModUninstalled", RoutingStrategy.Direct,
            typeof(RoutedEventHandler),
            typeof(InstalledMods));

        public event RoutedEventHandler ModUninstalled
        {
            add { AddHandler(ModUninstalledEvent, value); }
            remove { RemoveHandler(ModUninstalledEvent, value); }
        }

       
    }
}
