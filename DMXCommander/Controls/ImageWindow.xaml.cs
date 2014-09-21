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

namespace DMXCommander.Controls
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public ImageWindow()
        {
            InitializeComponent();
        }
        public void SetImage(string path)
        {
            if (this.Dispatcher != System.Windows.Threading.Dispatcher.CurrentDispatcher)
            {
                this.Dispatcher.Invoke(new Action<string>(SetImage), path);
            }
            else
            {
                Brush back = new ImageBrush(new BitmapImage(new Uri(path)));
                this.Background = back;
            }
        }
        public event EventHandler Abort;
        public event EventHandler PauseResume;
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:

                    e.Handled = true;
                    if (Abort != null)
                    {
                        Abort(this, EventArgs.Empty);
                    }
                    break;
                case Key.Space:
                    if (PauseResume != null)
                    {
                        PauseResume(this, EventArgs.Empty);
                    }
                    break;

            }

        }
    }
}
