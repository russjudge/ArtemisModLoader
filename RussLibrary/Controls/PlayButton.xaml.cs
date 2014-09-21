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
using System.IO;
using System.Media;

namespace RussLibrary.Controls
{
    /// <summary>
    /// Interaction logic for PlayButton.xaml
    /// </summary>
    public partial class PlayButton : Button
    {
        public PlayButton()
        {
            InitializeComponent();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Filename) && File.Exists(Filename))
            {

                using (SoundPlayer plr = new SoundPlayer(Filename))
                {
                    try
                    {
                        plr.Play();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Problem playing file:\r\n\r\n" + ex.Message, "Play sound", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Filename")]
        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.Register("Filename", typeof(string),
            typeof(PlayButton));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Filename")]
        public string Filename
        {
            get
            {
                return (string)this.UIThreadGetValue(FilenameProperty);

            }
            set
            {
                this.UIThreadSetValue(FilenameProperty, value);

            }
        }
    }
}
