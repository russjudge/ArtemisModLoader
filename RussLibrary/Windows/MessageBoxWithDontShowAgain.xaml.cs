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
    /// Interaction logic for MessageBoxWithDontShowAgain.xaml
    /// </summary>
    public partial class MessageBoxWithDontShowAgain : Window
    {
        private MessageBoxWithDontShowAgain()
        {
            InitializeComponent();
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
        public static MessageBoxResult Show(string messageboxText, out bool donotShowAgain)
        {
            bool d = false;
            MessageBoxResult retVal = Show(null,
                messageboxText,
                string.Empty, MessageBoxButton.OK, MessageBoxImage.None,
                MessageBoxResult.OK, MessageBoxOptions.None, out d);


            donotShowAgain = d;
            return retVal;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "7#")]
        public static MessageBoxResult Show(Window owner, string messageBoxText, 
            string caption, MessageBoxButton button,
            MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, out bool donotShowAgain)
        {
            donotShowAgain = false;
            MessageBoxResult retVal = defaultResult;
            MessageBoxWithDontShowAgain win = new MessageBoxWithDontShowAgain();

            if (win.ShowDialog() == true)
            {
                
            }
            MessageBox.Show(owner, messageBoxText, caption, button, icon, defaultResult, options);
            return retVal;
        }
    }
}
