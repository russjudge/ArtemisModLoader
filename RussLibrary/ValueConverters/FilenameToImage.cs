using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using System.Drawing;
using RussLibrary.Helpers;

namespace RussLibrary.ValueConverters
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Filename")]
    public class FilenameToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource retVal = null;
           
            FileSystemInfo fsi = value as FileSystemInfo;
            if (fsi != null)
            {
                if ((fsi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    retVal = ImageHelper.ConvertBitmapToBitmapSource(RussLibrary.Properties.Resources.FolderClosed);
                }
                else
                {
                    retVal = GetIcon(fsi.FullName);

                  
                }
            }
            return retVal;
        }
        static ImageSource GetIcon(string file)
        {
            ImageSource retVal = null;
            using (System.Drawing.Icon icn = System.Drawing.Icon.ExtractAssociatedIcon(file))
            {
                using (Bitmap bmp = icn.ToBitmap())
                {
                    retVal = ImageHelper.ConvertBitmapToBitmapSource(bmp);
                }

            }
            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
