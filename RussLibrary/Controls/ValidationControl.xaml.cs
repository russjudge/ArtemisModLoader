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
    /// Interaction logic for ValidationControl.xaml
    /// </summary>
    public partial class ValidationControl : UserControl
    {
        public ValidationControl()
        {
            InitializeComponent();
            
        }
        static void OnValidationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ValidationControl me = sender as ValidationControl;
            if (me != null)
            {
              
                //ValidationObject newobj = e.NewValue as ValidationObject;
                //ValidationObject oldobj = e.OldValue as ValidationObject;
            }
        }
        public static readonly DependencyProperty ValidationProperty =
            DependencyProperty.Register("Validation", typeof(ValidationObject),
            typeof(ValidationControl), new PropertyMetadata(OnValidationChanged));

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

        static void OnInnerContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ValidationControl me = sender as ValidationControl;
            if (me != null)
            {

                if (me.cc.Content == null)
                {
                   
                    me.cc.Content = me.InnerContent;
                }
            }
        }

       

        public static readonly DependencyProperty InnerContentProperty =
          DependencyProperty.Register("Content", typeof(object),
          typeof(ValidationControl), new PropertyMetadata(OnInnerContentChanged));

        //public static readonly DependencyProperty InnerContentProperty =
        //   DependencyProperty.Register("InnerContent", typeof(object),
        //   typeof(ValidationControl), new PropertyMetadata(OnInnerContentChanged));

        public new object Content
        {
            get
            {
                return (object)this.UIThreadGetValue(InnerContentProperty);
            }
            set
            {
                this.UIThreadSetValue(InnerContentProperty, value);
            }
        }
        public object InnerContent
        {
            get
            {
                return (object)this.UIThreadGetValue(InnerContentProperty);
            }
            set
            {
                this.UIThreadSetValue(InnerContentProperty, value);
            }
        }

    }
}
