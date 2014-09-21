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
namespace VesselDataLibrary.Controls
{
    /// <summary>
    /// Interaction logic for SelectMergeOption.xaml
    /// </summary>
    public partial class SelectMergeOption : Window
    {
        public SelectMergeOption()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
        }

        private void Cancel_click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        public static readonly DependencyProperty KeepSourceProperty =
            DependencyProperty.Register("KeepSource", typeof(bool),
            typeof(SelectMergeOption));

        public bool KeepSource
        {
            get
            {
                return (bool)this.UIThreadGetValue(KeepSourceProperty);
            }
            set
            {
                this.UIThreadSetValue(KeepSourceProperty, value);
            }
        }

        public static readonly DependencyProperty KeepTargetProperty =
           DependencyProperty.Register("KeepTarget", typeof(bool),
           typeof(SelectMergeOption));

        public bool KeepTarget
        {
            get
            {
                return (bool)this.UIThreadGetValue(KeepTargetProperty);
            }
            set
            {
                this.UIThreadSetValue(KeepTargetProperty, value);
            }
        }


        public static readonly DependencyProperty PromptProperty =
           DependencyProperty.Register("Prompt", typeof(bool),
           typeof(SelectMergeOption), new PropertyMetadata(true));

        public bool Prompt
        {
            get
            {
                return (bool)this.UIThreadGetValue(PromptProperty);
            }
            set
            {
                this.UIThreadSetValue(PromptProperty, value);
            }
        }


    }
}
