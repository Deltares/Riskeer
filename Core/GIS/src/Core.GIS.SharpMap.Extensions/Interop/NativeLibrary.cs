using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Core.GIS.SharpMap.Extensions.Properties;

namespace Core.GIS.SharpMap.Extensions.Interop
{
    public static class NativeLibrary
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);

        //private: use SwitchDllSearchDirectory with a using instead
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern void SetDllDirectory(string lpPathName);

        public static string GetDllDirectory()
        {
            var tmp = new StringBuilder(4096);
            GetDllDirectory(4096, tmp);
            return tmp.ToString();
        }

        /// <summary>
        /// Call this from a static constructor in a class that has DllImport external methods. This 
        /// method uses LoadLibrary to load the correct dll for the current process (32bit or 64bit) 
        /// before DllImport has the chance to resolve the external calls. As long as the dll name is 
        /// the same this works.
        /// </summary>
        /// <param name="dllFileName">The dll file to load.</param>
        /// <param name="baseDirectory">The directory where x64 and x86 are situated.</param>
        public static void LoadNativeDllForCurrentPlatform(string dllFileName, string baseDirectory)
        {
            var platform = Environment.Is64BitProcess ? "x64" : "x86";
            var nativeDirectory = Path.Combine(baseDirectory, platform);

            LoadNativeDll(dllFileName, nativeDirectory);
        }

        /// <summary>
        /// Call this from a static constructor in a class that has DllImport external methods. This 
        /// method uses LoadLibrary to load the correct dll for the current process (32bit or 64bit) 
        /// before DllImport has the chance to resolve the external calls. As long as the dll name is 
        /// the same this works.
        /// </summary>
        /// <param name="dllFileName">The dll file to load.</param>
        /// <param name="directory">The directory to load the dll from.</param>
        public static void LoadNativeDll(string dllFileName, string directory)
        {
            using (SwitchDllSearchDirectory(directory))
            {
                // attempt to load the library
                var ptr = LoadLibrary(dllFileName);
                if (ptr == IntPtr.Zero)
                {
                    var error = Marshal.GetLastWin32Error();
                    var exception = new Win32Exception(error);
                    throw new FileNotFoundException(
                        string.Format(Resources.NativeLibrary_LoadNativeDll_Could_not_find_load_0_Error_1_2_File_3_0_,
                                      dllFileName, error, exception.Message, directory, dllFileName));
                }
            }
        }

        public static IDisposable SwitchDllSearchDirectory(string dllDirectory)
        {
            return new SwitchDllSearchDirectoryHelper(dllDirectory);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetDllDirectory(int nBufferLength, StringBuilder lpPathName);

        private class SwitchDllSearchDirectoryHelper : IDisposable // ???
        {
            private readonly string oldDirectory;

            public SwitchDllSearchDirectoryHelper(string dllDirectory)
            {
                oldDirectory = GetDllDirectory();
                SetDllDirectory(dllDirectory);
            }

            public void Dispose()
            {
                SetDllDirectory(oldDirectory);
            }
        }
    }
}