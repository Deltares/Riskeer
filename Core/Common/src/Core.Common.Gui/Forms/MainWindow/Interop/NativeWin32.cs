using System.Runtime.InteropServices;

namespace Core.Common.Gui.Forms.MainWindow.Interop
{
    public static class NativeWin32
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool EnableWindow(HandleRef hWnd, bool enable);
    }
}