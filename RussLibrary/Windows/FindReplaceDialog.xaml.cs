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

namespace RussLibrary.Windows
{
    /// <summary>
    /// Interaction logic for FindReplaceDialog.xaml
    /// </summary>
    public partial class FindReplaceDialog : Window
    {
        
        public FindReplaceDialog()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SearchTextProperty =
          DependencyProperty.Register("SearchText", typeof(string),
          typeof(FindReplaceDialog));

        public string SearchText
        {
            get
            {
                return (string)this.UIThreadGetValue(SearchTextProperty);
            }
            set
            {
                this.UIThreadSetValue(SearchTextProperty, value);
            }
        }


        public static readonly DependencyProperty EnableReplaceProperty =
          DependencyProperty.Register("EnableReplace", typeof(bool),
          typeof(FindReplaceDialog));

        public bool EnableReplace
        {
            get
            {
                return (bool)this.UIThreadGetValue(EnableReplaceProperty);
            }
            set
            {
                this.UIThreadSetValue(EnableReplaceProperty, value);
            }
        }


        public static readonly DependencyProperty ReplaceTextProperty =
          DependencyProperty.Register("ReplaceText", typeof(string),
          typeof(FindReplaceDialog));

        public string ReplaceText
        {
            get
            {
                return (string)this.UIThreadGetValue(ReplaceTextProperty);
            }
            set
            {
                this.UIThreadSetValue(ReplaceTextProperty, value);
            }
        }
        public event EventHandler FindNext;
        public event EventHandler Replace;
        public event EventHandler ReplaceAll;

        private void FindNext_Click(object sender, RoutedEventArgs e)
        {
            if (FindNext != null)
            {
                FindNext(this, EventArgs.Empty);
            }
        }

        private void ReplaceNext_Click(object sender, RoutedEventArgs e)
        {
            if (Replace != null)
            {
                Replace(this, EventArgs.Empty);
            }
        }

        private void ReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            if (ReplaceAll != null)
            {
                ReplaceAll(this, EventArgs.Empty);
            }
        }

        private void uc_Loaded(object sender, RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, txStart);
        }
    }
}
