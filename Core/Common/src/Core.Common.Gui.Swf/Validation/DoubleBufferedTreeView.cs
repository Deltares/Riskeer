using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Core.Common.Gui.Swf.Validation
{
    // TODO: remove it! merge into TreeView if necessary and use TreeView
    //from: http://www.codeproject.com/Articles/37253/Double-buffered-Tree-and-Listviews
    public class DoubleBufferedTreeView : TreeView
    {
        public const int WM_PRINTCLIENT = 0x0318;
        public const int PRF_CLIENT = 0x00000004;
        private const int TV_FIRST = 0x1100;
        private const int TVM_SETBKCOLOR = TV_FIRST + 29;
        private const int TVM_SETEXTENDEDSTYLE = TV_FIRST + 44;

        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        public DoubleBufferedTreeView()
        {
            // Enable default double buffering processing
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            // Disable default CommCtrl painting on non-Vista systems
            if (!IsWinVista)
            {
                SetStyle(ControlStyles.UserPaint, true);
            }
        }

        public static bool IsWinXP
        {
            get
            {
                OperatingSystem OS = Environment.OSVersion;
                return (OS.Platform == PlatformID.Win32NT) &&
                       ((OS.Version.Major > 5) || ((OS.Version.Major == 5) && (OS.Version.Minor == 1)));
            }
        }

        public static bool IsWinVista
        {
            get
            {
                OperatingSystem OS = Environment.OSVersion;
                return (OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6);
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            UpdateExtendedStyles();
            if (!IsWinXP)
            {
                SendMessage(Handle, TVM_SETBKCOLOR, IntPtr.Zero, (IntPtr) ColorTranslator.ToWin32(BackColor));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                Message m = new Message();
                m.HWnd = Handle;
                m.Msg = WM_PRINTCLIENT;
                m.WParam = e.Graphics.GetHdc();
                m.LParam = (IntPtr) PRF_CLIENT;
                DefWndProc(ref m);
                e.Graphics.ReleaseHdc(m.WParam);
            }
            base.OnPaint(e);
        }

        private void UpdateExtendedStyles()
        {
            int Style = 0;

            if (DoubleBuffered)
            {
                Style |= TVS_EX_DOUBLEBUFFER;
            }

            if (Style != 0)
            {
                SendMessage(Handle, TVM_SETEXTENDEDSTYLE, (IntPtr) TVS_EX_DOUBLEBUFFER, (IntPtr) Style);
            }
        }
    }
}