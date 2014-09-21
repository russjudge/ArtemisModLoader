using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows;

namespace RussLibrary.Helpers
{

    public static class ImageHelper
    {
        //static readonly ILog _log = LogManager.GetLogger(typeof(ImageHelper));
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Starting {0}", MethodBase.GetCurrentMethod().ToString()); }
        //if (_log.IsDebugEnabled) { _log.DebugFormat("Ending {0}", MethodBase.GetCurrentMethod().ToString()); }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "src"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        static public System.Drawing.Bitmap ConvertBitmapSourceToBitmap(BitmapSource src)
        {
            System.Drawing.Bitmap retval = null;
            if (src != null)
            {
                //int width = src.PixelWidth;
                //int height = src.PixelHeight;
                //int stride = width * ((src.Format.BitsPerPixel + 7) / 8);

                //byte[] bits = new byte[height * stride];

                using (MemoryStream strm = new MemoryStream())
                {
                    BitmapEncoder encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(src));
                    encoder.Save(strm);
                    retval = new System.Drawing.Bitmap(strm);
                }
            }
            return retval;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        static public BitmapSource ConvertBitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            BitmapSource bm = null;
            if (bitmap != null)
            {
                IntPtr hBitmap = bitmap.GetHbitmap();
                BitmapSizeOptions sizeOptions = BitmapSizeOptions.FromEmptyOptions();
                bm = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
                bm.Freeze();
            }
            return bm;
        }
    }
}
