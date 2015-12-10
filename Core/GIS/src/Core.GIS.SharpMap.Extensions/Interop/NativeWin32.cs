using System;
using System.Runtime.InteropServices;

namespace Core.GIS.SharpMap.Extensions.Interop
{
    public static class NativeWin32
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr handle);

        [DllImport("gdi32")]
        public static extern bool DeleteObject(IntPtr hObject);

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        } 
    }
}