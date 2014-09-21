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
using RussLibrary.WPF;

namespace RussLibrary.Controls
{
    /// <summary>
    /// Interaction logic for ValidationTextBox.xaml
    /// </summary>
    public partial class ValidationTextBox : UserControl
    {
        public ValidationTextBox()
        {
            InitializeComponent();
           
        }
        public static readonly DependencyProperty ValidationProperty =
            DependencyProperty.Register("Validation", typeof(ValidationObject),
            typeof(ValidationTextBox));

        public ValidationObject Validation
        {
            get
            {
                return (ValidationObject)this.UIThreadGetValue(ValidationProperty);
            }
            set
            {
                this.UIThreadSetValue(ValidationProperty, value);
            }
        }
        public static readonly DependencyProperty TextProperty =
           DependencyProperty.Register("Text", typeof(string),
           typeof(ValidationTextBox));

        public string Text
        {
            get
            {
                return (string)this.UIThreadGetValue(TextProperty);
            }
            set
            {
                this.UIThreadSetValue(TextProperty, value);
            }
        }
    }
}
