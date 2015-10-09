using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace DelftTools.Utils.Interop
{
    public abstract class NativeLibrary : IDisposable
    {
        private IntPtr lib = IntPtr.Zero;

        protected NativeLibrary(string fileName)
        {
            lib = LoadLibrary(fileName);
        }

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
                        string.Format("Could not find / load {3}.{2}Error: {4} - {0}{2}File: {5}\\{1}",
                                      exception.Message, dllFileName, Environment.NewLine, dllFileName, error, directory));
                }
            }
        }

        public static IDisposable SwitchDllSearchDirectory(string dllDirectory)
        {
            return new SwitchDllSearchDirectoryHelper(dllDirectory);
        }

        public void Dispose()
        {
            if (lib == IntPtr.Zero)
            {
                return;
            }

            FreeLibrary(lib);

            lib = IntPtr.Zero;
        }

        protected IntPtr Library
        {
            get
            {
                if (lib == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Plug-in library is not loaded");
                }

                return lib;
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int GetDllDirectory(int nBufferLength, StringBuilder lpPathName);

        ~NativeLibrary()
        {
            Dispose();
        }

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