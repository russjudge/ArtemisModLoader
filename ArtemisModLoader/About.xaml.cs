using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Collections;
using System.Text;
using RussLibrary;
using ArtemisModLoader.Xml;
using RussLibrary.Helpers;
namespace ArtemisModLoader
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
           
            InitializeComponent();
            
        }



        public static readonly DependencyProperty AppPathLabelProperty =
           DependencyProperty.Register("AppPathLabel", typeof(string),
           typeof(About), new PropertyMetadata(string.Format(AMLResources.Properties.Resources.ArtemisCopyPath, GeneralHelper.AssemblyTitle)));
        public string AppPathLabel
        {
            get
            {
                return (string)this.UIThreadGetValue(AppPathLabelProperty);

            }
            private set
            {
                this.UIThreadSetValue(AppPathLabelProperty, value);

            }
        }

        public static readonly DependencyProperty AppPathProperty =
           DependencyProperty.Register("AppPath", typeof(string),
           typeof(About), new PropertyMetadata(Locations.ArtemisCopyPath));
        public string AppPath
        {
            get
            {
                return (string)this.UIThreadGetValue(AppPathProperty);

            }
            private set
            {
                this.UIThreadSetValue(AppPathProperty, value);

            }
        }

        public static readonly DependencyProperty AppTitleProperty =
           DependencyProperty.Register("AppTitle", typeof(string),
           typeof(About), new PropertyMetadata(GeneralHelper.AssemblyTitle));
        public string AppTitle
        {
            get
            {
                return (string)this.UIThreadGetValue(AppTitleProperty);

            }
            private set
            {
                this.UIThreadSetValue(AppTitleProperty, value);

            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Mods")]
        public static readonly DependencyProperty VersionProperty =
            DependencyProperty.Register("Version", typeof(string),
            typeof(About), new PropertyMetadata(GeneralHelper.AssemblyVersion));
        public string Version
        {
            get
            {
                return (string)this.UIThreadGetValue(VersionProperty);

            }
            private set
            {
                this.UIThreadSetValue(VersionProperty, value);

            }
        }

        private void CheckForLatest_click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(DataStrings.AMLUpdateURL);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {

            ModManagement.DoReset();
            System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(FinalNotify));

        }
        void FinalNotify(object state)
        {
            while (ModManagement.SetupInProgress)
            {
                System.Threading.Thread.Sleep(100);
            }
            this.Dispatcher.BeginInvoke(new Action(FinalNotify), System.Windows.Threading.DispatcherPriority.Background);
        }
        void FinalNotify()
        {
            

            Locations.MessageBoxShow(AMLResources.Properties.Resources.ResetComplete 
                + DataStrings.CRCR 
                + string.Format(AMLResources.Properties.Resources.RestartRequired, AMLResources.Properties.Resources.Title), 
                MessageBoxButton.OK, MessageBoxImage.Information);

            System.Diagnostics.Process.Start(Assembly.GetEntryAssembly().Location);
            App.Current.Shutdown();
        }

        private void GetResourcesProperties_Click(object sender, RoutedEventArgs e)
        {
            string wrkFle = System.IO.Path.GetTempFileName();
            using (StreamWriter sw = new StreamWriter(wrkFle))
            {
                sw.WriteLine(";Instructions for translating:");
                sw.WriteLine(";Left of the \"=\" must be left alone.  The text to the right of the \"=\" is what you need to translate.");
                sw.WriteLine();
                sw.WriteLine(";If the entry includes \"{0}\" and/or \"{1}\", these must remain in the entry--they are dynamic variables.");
                sw.WriteLine();
                sw.WriteLine(";When your translation is complete, please email (as an attachment) to russjudge@gmail.com, with Subject=\"Artemis Mod Loader Translation\".");
                sw.WriteLine(";In the body of the message, please indicate what language and country (if needed) the translation is for.");
                sw.WriteLine();
                sw.WriteLine(";Use the semi-colon at the beginning of the line to indicate a comment line.");
                sw.WriteLine();
                foreach (DictionaryEntry item in AMLResources.Properties.Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.InvariantCulture, false, true))
                {
                    sw.WriteLine(item.Key + "=" + item.Value);
                }
            }
            System.Diagnostics.Process.Start("Notepad.exe", string.Format("\"{0}\"", wrkFle));
        }

        private void ReadMe_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo strt = new System.Diagnostics.ProcessStartInfo(Path.Combine(Locations.MyLocation, "ReadMe.txt"));
            strt.UseShellExecute=true;
            System.Diagnostics.Process.Start(strt);
        }

        private void PayPal_click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.ProcessStartInfo strt = new System.Diagnostics.ProcessStartInfo("http://www.paypal.com");
            strt.UseShellExecute = true;
            System.Diagnostics.Process.Start(strt);
        }

    }
}
