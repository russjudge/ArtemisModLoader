using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using log4net;
using System.Runtime.InteropServices;

namespace RussLibrary
{

    internal static class NativeMethods
    {


        //////public class ResourceName
        //////{
        //////    public IntPtr Id { get; private set; }
        //////    public string Name { get; private set; }

        //////    private IntPtr _bufPtr = IntPtr.Zero;

        //////    public ResourceName(IntPtr lpName)
        //////    {
        //////        if (((uint)lpName >> 16) == 0) // #define IS_INTRESOURCE(_r) ((((ULONG_PTR)(_r)) >> 16) == 0)
        //////        {
        //////            this.Id = lpName;
        //////            this.Name = null;
        //////        }
        //////        else
        //////        {
        //////            this.Id = IntPtr.Zero;
        //////            this.Name = Marshal.PtrToStringAuto(lpName);
        //////        }
        //////    }

        //////    public IntPtr GetValue()
        //////    {
        //////        if (this.Name == null)
        //////        {
        //////            return this.Id;
        //////        }
        //////        else
        //////        {
        //////            this._bufPtr = Marshal.StringToHGlobalAuto(this.Name);
        //////            return this._bufPtr;
        //////        }
        //////    }

        //////    public void Free()
        //////    {
        //////        if (this._bufPtr != IntPtr.Zero)
        //////        {
        //////            try { Marshal.FreeHGlobal(this._bufPtr); }
        //////            catch { }

        //////            this._bufPtr = IntPtr.Zero;
        //////        }
        //////    }
        //////}


        ////public class IconResInfo
        ////{
        ////    public List<ResourceName> IconNames = new List<ResourceName>();
        ////}

        ////[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Auto)]
        ////public delegate bool EnumResNameProc(IntPtr hModule, int lpszType, IntPtr lpszName, IconResInfo lParam);

        ////[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        ////public static extern IntPtr LoadLibrary(string lpFileName);

        ////[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        ////public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, int dwFlags);

        ////[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        ////public static extern bool FreeLibrary(IntPtr hModule);

        ////[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        ////public static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

        ////[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        ////public static extern bool EnumResourceNames(
        ////    IntPtr hModule, int lpszType, EnumResNameProc lpEnumFunc, IconResInfo lParam);

        ////[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        ////public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, int lpType);

        ////[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        ////public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        ////[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        ////public static extern IntPtr LockResource(IntPtr hResData);

        ////[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        ////public static extern int SizeofResource(IntPtr hModule, IntPtr hResInfo);

        // Specifies how TrackPopupMenuEx positions the shortcut menu horizontally
        [Flags]
        internal enum TPM : uint
        {
            LEFTBUTTON = 0x0000,
            RIGHTBUTTON = 0x0002,
            LEFTALIGN = 0x0000,
            CENTERALIGN = 0x0004,
            RIGHTALIGN = 0x0008,
            TOPALIGN = 0x0000,
            VCENTERALIGN = 0x0010,
            BOTTOMALIGN = 0x0020,
            HORIZONTAL = 0x0000,
            VERTICAL = 0x0040,
            NONOTIFY = 0x0080,
            RETURNCMD = 0x0100,
            RECURSE = 0x0001,
            HORPOSANIMATION = 0x0400,
            HORNEGANIMATION = 0x0800,
            VERPOSANIMATION = 0x1000,
            VERNEGANIMATION = 0x2000,
            NOANIMATION = 0x4000,
            LAYOUTRTL = 0x8000
        }

        #region DLL Import

        // Retrieves the IShellFolder interface for the desktop folder, which is the root of the Shell's namespace.
        [DllImport("shell32.dll")]
        internal static extern Int32 SHGetDesktopFolder(out IntPtr ppshf);

        // Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf, converts it to a string, and places the result in a buffer. 
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "2"), DllImport("shlwapi.dll", EntryPoint = "StrRetToBuf", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Int32 StrRetToBuf(IntPtr pstr, IntPtr pidl, StringBuilder pszBuf, int cchBuf);
        //#region correct StrRetToBuf
        //[StructLayout(LayoutKind.Explicit, Size = 264)]
        //public struct STRRET
        //{
        //    [FieldOffset(0)]
        //    public UInt32 uType;    // One of the STRRET_* values

        //    [FieldOffset(4)]
        //    public IntPtr pOleStr;    // must be freed by caller of GetDisplayNameOf

        //    [FieldOffset(4)]
        //    public IntPtr pStr;        // NOT USED

        //    [FieldOffset(4)]
        //    public UInt32 uOffset;    // Offset into SHITEMID

        //    [FieldOffset(4)]
        //    public IntPtr cStr;        // Buffer to fill in (ANSI)
        //}
        //internal static extern Int32 StrRetToBuf(ref STRRET pstr, IntPtr pidl, StringBuilder pszBuf, uint cchBuf);
        //#endregion



        // The TrackPopupMenuEx function displays a shortcut menu at the specified location and tracks the selection of items on the shortcut menu. The shortcut menu can appear anywhere on the screen.
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern uint TrackPopupMenuEx(IntPtr hmenu, TPM flags, int x, int y, IntPtr hwnd, IntPtr lptpm);

        // The CreatePopupMenu function creates a drop-down menu, submenu, or shortcut menu. The menu is initially empty. You can insert or append menu items by using the InsertMenuItem function. You can also use the InsertMenu function to insert menu items and the AppendMenu function to append menu items.
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr CreatePopupMenu();

        // The DestroyMenu function destroys the specified menu and frees any memory that the menu occupies.
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyMenu(IntPtr hMenu);

        // Determines the default menu item on the specified menu
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), 
        DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint GetMenuDefaultItem(IntPtr hMenu, uint fByPos, uint gmdiFlags);

        #endregion
        // ************************************************************************
        // Filter function delegate
        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        // ************************************************************************
        // ************************************************************************
        // Win32: SetWindowsHookEx()
        ////[DllImport("user32.dll")]
        ////internal static extern IntPtr SetWindowsHookEx(HookType code,
        ////    HookProc func,
        ////    IntPtr hInstance,
        ////    int threadID);
        // ************************************************************************

      

        // ************************************************************************
        // Win32: UnhookWindowsHookEx()
        //////[DllImport("user32.dll")]
        //////internal static extern int UnhookWindowsHookEx(IntPtr hhook);
        // ************************************************************************

     


        //// ************************************************************************
        //// Win32: CallNextHookEx()
        //[DllImport("user32.dll")]
        //internal static extern int CallNextHookEx(IntPtr hhook,
        //    int code, IntPtr wParam, IntPtr lParam);
        //// ************************************************************************




        [DllImport("Shlwapi", EntryPoint = "AssocGetPerceivedType")]
        private static extern int AssocGetPerceivedType(
            IntPtr extension, out IntPtr PERCEIVED, out IntPtr flag, out IntPtr PerceivedType);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "RussLibrary.NativeMethods.AssocGetPerceivedType(System.IntPtr,System.IntPtr@,System.IntPtr@,System.IntPtr@)")]
        public static string GetPerceivedType(string extension)
        {
            IntPtr result = IntPtr.Zero;
            IntPtr dummy = IntPtr.Zero;
            IntPtr intPtr_aux = Marshal.StringToHGlobalUni(extension);
            AssocGetPerceivedType(intPtr_aux, out dummy, out dummy, out result);
            string s = Marshal.PtrToStringAuto(result);
            return s;
        }




        #region Menu Methods
        //http://social.msdn.microsoft.com/Forums/en-US/winforms/thread/e82b3622-ac23-4629-a47c-5b1136c673da
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        public static extern IntPtr GetSystemMenu(IntPtr hwnd, int revert);

        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        public static extern int GetMenuItemCount(IntPtr hmenu);

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        public static extern int RemoveMenu(IntPtr hmenu, int npos, int wflags);

        [DllImport("user32.dll", EntryPoint = "DrawMenuBar", SetLastError = true)]
        public static extern int DrawMenuBar(IntPtr hwnd);

        public const int MF_BYPOSITION = 0x0400;
        public const int MF_DISABLED = 0x0002;

        #endregion

    }
}
