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
using RussLibrary;
namespace RussLibraryAudio
{
    /// <summary>
    /// Interaction logic for PlayPlayListButton.xaml
    /// </summary>
    public partial class PlayPlayListButton : UserControl
    {
        public PlayPlayListButton()
        {
            InitializeComponent();
        }


        private void PlayMusic_click(object sender, RoutedEventArgs e)
        {
            if (RussLibraryAudio.AudioServer.Current.IsPlaying)
            {
           
                RussLibraryAudio.AudioServer.Current.Stop();
            }
            else
            {
           
                if (RussLibraryAudio.AudioServer.Current.FilesQueued == 0)
                {
         
                    RussLibraryAudio.AudioServer.Current.MoveNext();  //skip first one since it plays at startup.
                }
                RussLibraryAudio.AudioServer.Current.PlayNextInQueue();
            }


        }

    }
}
