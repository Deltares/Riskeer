using System;
using System.Runtime.InteropServices;
using Core.Common.Utils.Aop;

namespace Core.Common.Utils.Tests.Aop.TestClasses
{
    [TraceDllImportCalls]
    public class DllImports
    {
        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);
    }
}