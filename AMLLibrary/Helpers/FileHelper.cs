using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Windows;
using System.IO;
using ArtemisModLoader.Xml;
using RussLibrary.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using log4net;
using System.Reflection;
namespace ArtemisModLoader.Helpers
{
    public static class FileHelper
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(FileHelper));
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        public static void FileSelectionControl_InvalidFilePath(object sender, RoutedEventArgs e, ModConfiguration configuration)
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            if (e != null)
            {
                KeyValuePair<string, string> result =
                    ArtemisModLoader.Helpers.FileHelper.ProcessNotInModFile(e.OriginalSource as string, configuration);
                e.Source = result.Key;

                if (e.Source == null)
                {
                    e.Handled = true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Value))
                    {
                        FileSelectionControl fsc = sender as FileSelectionControl;
                        fsc.AlternatePrefix = result.Value;
                    }
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static ImageBrush GetRandomSkybox()
        {
            if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
            ImageBrush brsh = null;
            try
            {
                int num = new Random().Next(0, 5);
                string[] directions = { "BK", "DN", "FR", "LF", "RT", "UP" };
                int dir = new Random().Next(0, directions.Length - 1);
                string skybox = null;
                int retrys = 0;
                while (string.IsNullOrEmpty(skybox) && !System.IO.File.Exists(skybox) && ++retrys < 20)
                {
                    skybox = System.IO.Path.Combine(Locations.ArtemisCopyPath, "art",
                        "sb" + num.ToString("00", CultureInfo.InvariantCulture),
                        string.Format(CultureInfo.InvariantCulture, "skybox_{0}.jpg", directions[dir]));
                }
                if (!string.IsNullOrEmpty(skybox) && System.IO.File.Exists(skybox))
                {

                    using (FileStream fs = new FileStream(skybox, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            int byRead = -1;
                            byte[] bytes = new byte[32768];
                            do
                            {
                                byRead = fs.Read(bytes, 0, bytes.Length);
                                if (byRead > 0)
                                {
                                    ms.Write(bytes, 0, byRead);
                                }
                            } while (byRead > 0);
                            ms.Seek(0, SeekOrigin.Begin);

                            BitmapImage img = new BitmapImage();
                            img.BeginInit();
                            img.StreamSource = ms;
                            img.CacheOption = BitmapCacheOption.OnLoad;
                            img.EndInit();
                            img.Freeze();

                            brsh = new ImageBrush();
                            //brsh.TileMode = TileMode.Tile;
                            brsh.Stretch = Stretch.UniformToFill;
                            brsh.ImageSource = img;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                if (_log.IsWarnEnabled)
                {
                    _log.Warn("Exception on loading random skybox.", ex);
                }
            }
            if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
            return brsh;
        }

        /// <summary>
        /// Processes the not in mod file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="mod">The mod.</param>
        /// <returns>Destination of file, if it should change.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Mod")]
        private static KeyValuePair<string,string> ProcessNotInModFile(string source, ModConfiguration mod)
        {

      //      When selecting file items, no issue if within mod prefix.
      //If within Artemis copy, test if dependent mods and only dependent mods are active: no issue.
      //Otherwise auto copy selected file into current mod.
            string retVal = source;

            string alternatePrefix = null;
            if (mod !=null && !string.IsNullOrEmpty(source))
            {
                
                alternatePrefix = ModManagement.IsInModPathOrDependencyPath(source, mod);
                
                if (alternatePrefix != null)
                {
                    retVal = source;

                    //Need a way to pass back temporary alternate prefix for this.

                    //Need what InstalledPath of matching Mod to get prefix.

                }
                else
                {
                    //If not among dependent list, copy file in as below.
                    retVal = DoFileDialog(source, mod.InstalledPath);
                   
                }
            }
            return new KeyValuePair<string,string>(retVal, alternatePrefix);
        }
        private static string DoFileDialog(string source, string initialDirectory)
        {
            string retVal = null;
            Locations.MessageBoxShow(AMLResources.Properties.Resources.NotInModPath
                      + DataStrings.CRCR
                      + AMLResources.Properties.Resources.SelectTarget + DataStrings.CRCR + "Initial directory will be the root of your mod.",
                       MessageBoxButton.OK, MessageBoxImage.Information);
            //e.ORiginalsource should have source file.
            SaveFileDialog diag = new SaveFileDialog();
            diag.InitialDirectory = initialDirectory;
            diag.OverwritePrompt = true;
            diag.AddExtension = true;
            diag.Title = AMLResources.Properties.Resources.SelectTarget;
            diag.FileName = source;
            FileInfo f = new FileInfo(source);
            diag.DefaultExt = f.Extension;
            diag.Filter = f.Extension + " files (*." + f.Extension + ")|*." + f.Extension + "|All Files|*.*";


            

            if (diag.ShowDialog() == true)
            {
                retVal = diag.FileName;
                File.Copy(source, diag.FileName);
            }
            else
            {
                retVal = null;

            }
            return retVal;
        }


        /// <summary>
        /// Locates the expected file in mod.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="config">The config.</param>
        /// <returns></returns>
        public static string LocateExpectedFileInMod(string fileName, ModConfiguration config)
        {
            string retVal = string.Empty;
            if (config != null)
            {
                foreach (FileMap fm in config.BaseFiles.Files)
                {
                    if (fm.Target.EndsWith(fileName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (File.Exists(Path.Combine(config.InstalledPath, fm.Source)))
                        {
                            retVal = Path.Combine(config.InstalledPath, fm.Source);
                        }
                        break;
                    }
                }

                if (string.IsNullOrEmpty(retVal))
                {
                    if (File.Exists(Path.Combine(config.InstalledPath, fileName)))
                    {
                        retVal = Path.Combine(config.InstalledPath, fileName);
                    }
                }
            }
            return retVal;
        }
    }
}
