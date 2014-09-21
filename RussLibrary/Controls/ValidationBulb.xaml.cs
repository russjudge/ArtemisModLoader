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
    /// Interaction logic for ValidationBulb.xaml
    /// </summary>
    public partial class ValidationBulb : UserControl
    {
        public ValidationBulb()
        {
            InitializeComponent();
        }

        static void OnValidationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ValidationBulb me = sender as ValidationBulb;
            if (me != null)
            {
                //ValidationObject newobj = e.NewValue as ValidationObject;
                //ValidationObject oldobj = e.OldValue as ValidationObject;
                //switch (newobj.Code)
                //{
                //    case ValidationValue.IsError:
                //        me.imgError.Visibility = Visibility.Visible;
                //        me.imgWarn.Visibility = Visibility.Collapsed;
                //        break;
                //    case ValidationValue.IsWarnState:
                //        me.imgError.Visibility = Visibility.Collapsed;
                //        me.imgWarn.Visibility = Visibility.Visible;
                //        break;

                //    case ValidationValue.IsValid:
                //        me.imgError.Visibility = Visibility.Collapsed;
                //        me.imgWarn.Visibility = Visibility.Collapsed;
                //        break;
                //}
               
            }
        }
        public static readonly DependencyProperty ValidationProperty =
            DependencyProperty.Register("Validation", typeof(ValidationObject),
            typeof(ValidationBulb), new PropertyMetadata(OnValidationChanged));

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
    }
}
