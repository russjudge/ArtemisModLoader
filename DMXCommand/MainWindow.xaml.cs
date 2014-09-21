using DMXCommander.Engine;
using Microsoft.Win32;
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

namespace DMXCommand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnOpenDMXFile(object sender, RoutedEventArgs e)
        {
            //if (this.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            //{
            //    this.Dispatcher.Invoke(new Action<object, RoutedEventArgs>(OnOpenDMXFile), sender, e);
            //}
            //else
            //{
                OpenFileDialog diag = new OpenFileDialog();
                diag.Title = "Select DMX File";
                diag.CheckFileExists = true;
                diag.CheckPathExists = true;
                diag.DefaultExt = "xml";
                diag.Filter = "Xml Files|*.xml|All Files|*.*";
                if (diag.ShowDialog() == true)
                {
                    DMX.LoadFile(diag.FileName);
                }
            //}
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void OnClosed(object sender, EventArgs e)
        {
            try
            {
                OpenDMX.Stop();
            }
            catch { }
        }
    }
}
