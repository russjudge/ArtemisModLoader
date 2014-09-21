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
namespace MissionStudio
{
    /// <summary>
    /// Interaction logic for ScriptContextMenu.xaml
    /// </summary>
    public partial class ScriptContextMenu : ContextMenu
    {
        public ScriptContextMenu()
        {
            InitializeComponent();
            
        }
        public static readonly DependencyProperty CommandTargetProperty =
          DependencyProperty.Register("CommandTarget", typeof(IInputElement),
          typeof(ScriptContextMenu));

        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)this.UIThreadGetValue(CommandTargetProperty);
            }
            set
            {
                this.UIThreadSetValue(CommandTargetProperty, value);
            }
        }
    }
}
