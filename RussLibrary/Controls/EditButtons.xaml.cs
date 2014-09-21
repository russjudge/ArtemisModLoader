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

namespace RussLibrary.Controls
{
    /// <summary>
    /// Interaction logic for EditButtons.xaml
    /// </summary>
    public partial class EditButtons : UserControl
    {
        public EditButtons()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CommandTargetProperty =
           DependencyProperty.Register("CommandTarget", typeof(IInputElement),
           typeof(EditButtons));

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
