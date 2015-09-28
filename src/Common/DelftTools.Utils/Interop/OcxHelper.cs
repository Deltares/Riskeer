using System;
using System.Runtime.InteropServices;

namespace DelftTools.Utils.Interop
{
    /// <summary>
    /// Manages registration of external (windows) libraries
    /// </summary>
    public class OcxHelper
    {
        [DllImport("kernel32.dll")] static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("kernel32.dll")] static extern IntPtr LoadLibrary(string lpFilename);
        [DllImport("kernel32.dll")] static extern UIntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("user32.dll")] static extern IntPtr CallWindowProc(UIntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Registers ocx with windows
        /// </summary>
        /// <param name="ocxPath"></param>
        /// <returns></returns>
        public static bool Register(string ocxPath)
        {
            return (Register(ocxPath, true));
        }

        /// <summary>
        /// Deregisters ocx from windows
        /// </summary>
        /// <param name="ocxPath"></param>
        /// <returns></returns>
        public static bool UnRegister(string ocxPath)
        {
            return (Register(ocxPath, false));
        }

        private static bool Register(string ocxPath, bool register)
        {
            IntPtr lb = IntPtr.Zero;
            UIntPtr pa;

            try
            {
                lb = LoadLibrary(ocxPath);
                if (register)
                {
                    pa = GetProcAddress(lb, "DllRegisterServer");
                }
                else
                {
                    pa = GetProcAddress(lb, "DllUnregisterServer");
                }


                if (CallWindowProc(pa, IntPtr.Zero, 0, UIntPtr.Zero, IntPtr.Zero) == IntPtr.Zero)
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally
            {
                FreeLibrary(lb);
            }
        }

    }
}
