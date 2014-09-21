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
using ArtemisModLoader.Xml;
using RussLibrary;
namespace ArtemisModLoader.Controls
{
    /// <summary>
    /// Interaction logic for Variations.xaml
    /// </summary>
    public partial class Variations : UserControl
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(Variations));
        public Variations()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            InitializeComponent();
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }  public static readonly DependencyProperty ModProperty =
         DependencyProperty.Register("Mod", typeof(ModConfiguration),
         typeof(Variations));

        public ModConfiguration Mod
        {
            get
            {
                return (ModConfiguration)this.UIThreadGetValue(ModProperty);

            }
            set
            {
                this.UIThreadSetValue(ModProperty, value);

            }
        }
      

        private void Activate_Click(object sender, RoutedEventArgs e)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            Button btn = sender as Button;
            if (btn != null)
            {
                SubMod sm = btn.CommandParameter as SubMod;
                if (sm != null)
                {
                    Mod.ActivateSubMod(sm.Title);
                    ActiveModConfigurations.Current.SaveData();
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }
    }
}
