﻿#if DONOTCOMPILE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;
using System.Globalization;

namespace RussLibrary.Helpers
{

    public class IconExtractor : IDisposable
    {
        public static Icon GetIcon(string file, int index)
        {
            Icon retVal = null;
            using (IconExtractor ext = new IconExtractor(file))
            {
                retVal = ext.GetIcon(index);
            }
            return retVal;
        }
        #region Win32 interop.

     

        #region Consts.

        private const int LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        private const int RT_ICON = 3;
        private const int RT_GROUP_ICON = 14;

        private const int MAX_PATH = 260;

        private const int ERROR_SUCCESS = 0;
        private const int ERROR_FILE_NOT_FOUND = 2;
        private const int ERROR_BAD_EXE_FORMAT = 193;
        private const int ERROR_RESOURCE_TYPE_NOT_FOUND = 1813;

        private const int sICONDIR = 6;            // sizeof(ICONDIR) 
        private const int sICONDIRENTRY = 16;      // sizeof(ICONDIRENTRY)
        private const int sGRPICONDIRENTRY = 14;   // sizeof(GRPICONDIRENTRY)

        #endregion

      
        #endregion

        #region Managed Types

       
        #endregion

        #region Private Fields

        private IntPtr _hModule = IntPtr.Zero;
        private NativeMethods.IconResInfo _resInfo = null;

        private Icon[] _iconCache = null;

        #endregion

        #region Public Properties

        private string _filename = null;

        // Full path 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Filename")]
        public string Filename
        {
            get { return this._filename; }
        }

        public int IconCount
        {
            get { return this._resInfo.IconNames.Count; }
        }

        #endregion

        #region Contructor/Destructor and relatives

        /// <summary>
        /// Load the specified executable file or DLL, and get ready to extract the icons.
        /// </summary>
        /// <param name="filename">The name of a file from which icons will be extracted.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "filename")]
        public IconExtractor(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            this._hModule = NativeMethods.LoadLibrary(filename);
            if (this._hModule == IntPtr.Zero)
            {
                this._hModule = NativeMethods.LoadLibraryEx(filename, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (this._hModule == IntPtr.Zero)
                {
                    switch (Marshal.GetLastWin32Error())
                    {
                        case ERROR_FILE_NOT_FOUND:
                            throw new FileNotFoundException("Specified file '" + filename + "' not found.");

                        case ERROR_BAD_EXE_FORMAT:
                            throw new ArgumentException("Specified file '" + filename + "' is not an executable file or DLL.");

                        default:
                            throw new Win32Exception();
                    }
                }
            }

            StringBuilder buf = new StringBuilder(MAX_PATH);
            int len = NativeMethods.GetModuleFileName(this._hModule, buf, buf.Capacity + 1);
            int LastError = Marshal.GetLastWin32Error();
            if (len != 0)
            {
                this._filename = buf.ToString();
            }
            else
            {
                switch (LastError)
                {
                    case ERROR_SUCCESS:
                        this._filename = filename;
                        break;

                    default:
                        throw new Win32Exception();
                }
            }

            this._resInfo = new NativeMethods.IconResInfo();
            bool success = NativeMethods.EnumResourceNames(this._hModule, RT_GROUP_ICON, EnumResNameCallBack, this._resInfo);

            // bool EnumResourceNames(IntPtr hModule, int lpszType, EnumResNameProc lpEnumFunc, IconResInfo lParam);
            if (!success)
            {
                throw new Win32Exception();
            }

            this._iconCache = new Icon[this.IconCount];
        }

        ~IconExtractor()
        {
            Dispose(false);
        }

        bool isDisposed = false;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                if (isDisposing)
                {
                    if (this._hModule != IntPtr.Zero)
                    {
                        try { NativeMethods.FreeLibrary(this._hModule); }
                        catch { }

                        this._hModule = IntPtr.Zero;
                    }

                    if (this._iconCache != null)
                    {
                        foreach (Icon i in this._iconCache)
                        {
                            if (i != null)
                            {
                                try { i.Dispose(); }
                                catch { }
                            }
                        }

                        this._iconCache = null;
                    }
                    isDisposed = true;
                }
                
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Extract an icon from the loaded executable file or DLL. 
        /// </summary>
        /// <param name="iconIndex">The zero-based index of the icon to be extracted.</param>
        /// <returns>A System.Drawing.Icon object which may consists of multiple icons.</returns>
        /// <remarks>Always returns new copy of the Icon. It should be disposed by the user.</remarks>
        public Icon GetIcon(int iconIndex)
        {
            if (this._hModule == IntPtr.Zero)
            {
                throw new ObjectDisposedException("IconExtractor");
            }

            if (iconIndex < 0 || this.IconCount <= iconIndex)
            {
                throw new ArgumentException(
                    "iconIndex is out of range. It should be between 0 and " + (this.IconCount - 1).ToString(CultureInfo.CurrentCulture) + ".");
            }

            if (this._iconCache[iconIndex] == null)
            {
                this._iconCache[iconIndex] = CreateIcon(iconIndex);
            }

            return (Icon)this._iconCache[iconIndex].Clone();
        }

        /// <summary>
        /// Split an Icon consists of multiple icons into an array of Icon each consist of single icons.
        /// </summary>
        /// <param name="icon">The System.Drawing.Icon to be split.</param>
        /// <returns>An array of System.Drawing.Icon each consist of single icons.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static Icon[] SplitIcon(Icon icon)
        {
            if (icon == null)
            {
                throw new ArgumentNullException("icon");
            }

            // Get multiple .ico file image.
            byte[] srcBuf = null;
            using (MemoryStream stream = new MemoryStream())
            {
                icon.Save(stream);
                srcBuf = stream.ToArray();
            }

            List<Icon> splitIcons = new List<Icon>();
            {
                int count = BitConverter.ToInt16(srcBuf, 4); // ICONDIR.idCount

                for (int i = 0; i < count; i++)
                {
                    using (MemoryStream destStream = new MemoryStream())
                    using (BinaryWriter writer = new BinaryWriter(destStream))
                    {
                        // Copy ICONDIR and ICONDIRENTRY.
                        writer.Write(srcBuf, 0, sICONDIR - 2);
                        writer.Write((short)1);    // ICONDIR.idCount == 1;

                        writer.Write(srcBuf, sICONDIR + sICONDIRENTRY * i, sICONDIRENTRY - 4);
                        writer.Write(sICONDIR + sICONDIRENTRY);    // ICONDIRENTRY.dwImageOffset = sizeof(ICONDIR) + sizeof(ICONDIRENTRY)

                        // Copy picture and mask data.
                        int imgSize = BitConverter.ToInt32(srcBuf, sICONDIR + sICONDIRENTRY * i + 8);       // ICONDIRENTRY.dwBytesInRes
                        int imgOffset = BitConverter.ToInt32(srcBuf, sICONDIR + sICONDIRENTRY * i + 12);    // ICONDIRENTRY.dwImageOffset
                        writer.Write(srcBuf, imgOffset, imgSize);

                        // Create new icon.
                        destStream.Seek(0, SeekOrigin.Begin);
                        splitIcons.Add(new Icon(destStream));
                    }
                }
            }

            return splitIcons.ToArray();
        }

        public override string ToString()
        {
            string text = string.Format(CultureInfo.CurrentCulture, "IconExtractor (Filename: '{0}', IconCount: {1})", this.Filename, this.IconCount);
            return text;
        }

        #endregion

        #region Private Methods

        private bool EnumResNameCallBack(IntPtr hModule, int lpszType, IntPtr lpszName, NativeMethods.IconResInfo lParam)
        {
            // Callback function for EnumResourceNames().

            if (lpszType == RT_GROUP_ICON)
            {
                lParam.IconNames.Add(new NativeMethods.ResourceName(lpszName));
            }

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        private Icon CreateIcon(int iconIndex)
        {
            // Get group icon resource.
            byte[] srcBuf = GetResourceData(this._hModule, this._resInfo.IconNames[iconIndex], RT_GROUP_ICON);

            // Convert the resouce into an .ico file image.
            using (MemoryStream destStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(destStream))
            {
                int count = BitConverter.ToUInt16(srcBuf, 4); // ICONDIR.idCount
                int imgOffset = sICONDIR + sICONDIRENTRY * count;

                // Copy ICONDIR.
                writer.Write(srcBuf, 0, sICONDIR);

                for (int i = 0; i < count; i++)
                {
                    // Copy GRPICONDIRENTRY converting into ICONDIRENTRY.
                    writer.BaseStream.Seek(sICONDIR + sICONDIRENTRY * i, SeekOrigin.Begin);
                    writer.Write(srcBuf, sICONDIR + sGRPICONDIRENTRY * i, sICONDIRENTRY - 4);   // Common fields of structures
                    writer.Write(imgOffset);                                                    // ICONDIRENTRY.dwImageOffset

                    // Get picture and mask data, then copy them.
                    IntPtr nID = (IntPtr)BitConverter.ToUInt16(srcBuf, sICONDIR + sGRPICONDIRENTRY * i + 12); // GRPICONDIRENTRY.nID
                    byte[] imgBuf = GetResourceData(this._hModule, nID, RT_ICON);

                    writer.BaseStream.Seek(imgOffset, SeekOrigin.Begin);
                    writer.Write(imgBuf, 0, imgBuf.Length);

                    imgOffset += imgBuf.Length;
                }

                destStream.Seek(0, SeekOrigin.Begin);
                return new Icon(destStream);
            }
        }

        private static byte[] GetResourceData(IntPtr hModule, IntPtr lpName, int lpType)
        {
            // Get binary image of the specified resource.

            IntPtr hResInfo = NativeMethods.FindResource(hModule, lpName, lpType);
            if (hResInfo == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            IntPtr hResData = NativeMethods.LoadResource(hModule, hResInfo);
            if (hResData == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            IntPtr hGlobal = NativeMethods.LockResource(hResData);
            if (hGlobal == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            int resSize = NativeMethods.SizeofResource(hModule, hResInfo);
            if (resSize == 0)
            {
                throw new Win32Exception();
            }

            byte[] buf = new byte[resSize];
            Marshal.Copy(hGlobal, buf, 0, buf.Length);

            return buf;
        }

        private byte[] GetResourceData(IntPtr hModule, NativeMethods.ResourceName name, int lpType)
        {
            try
            {
                IntPtr lpName = name.GetValue();
                return GetResourceData(hModule, lpName, lpType);
            }
            finally
            {
                name.Free();
            }
        }

        #endregion
    }
}

#endif