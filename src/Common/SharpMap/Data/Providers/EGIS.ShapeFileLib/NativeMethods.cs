using System;
using System.Runtime.InteropServices;

namespace SharpMap.Data.Providers.EGIS.ShapeFileLib
{
    internal sealed class NativeMethods
    {
        internal const uint PAGE_READONLY = 0x02;
        internal const uint PAGE_READWRITE = 0x04;    


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern Microsoft.Win32.SafeHandles.SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess,
                                                                            uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
                                                                            uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFileMapping(Microsoft.Win32.SafeHandles.SafeFileHandle hFile, IntPtr lpAttributes,
                                                        uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        internal static IntPtr MapFile(System.IO.FileStream fs)
        {
            return CreateFileMapping(fs.SafeFileHandle, IntPtr.Zero, /*fs.CanWrite ? PAGE_READWRITE :*/ PAGE_READONLY, 0, 0, null);
        }

        internal enum FileMapAccess { FILE_MAP_WRITE = 0x02, FILE_MAP_READ = 0x04 };

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int UnmapViewOfFile(IntPtr lpBaseAddress);

        //HANDLE CreateFileMapping(HANDLE hFile,LPSECURITY_ATTRIBUTES lpAttributes, DWORD flProtect, DWORD dwMaximumSizeHigh, DWORD dwMaximumSizeLow, LPCTSTR lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int CloseHandle(IntPtr handle);
    }
}