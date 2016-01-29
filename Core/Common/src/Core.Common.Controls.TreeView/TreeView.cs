using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BaseResources = Core.Common.Base.Properties.Resources;

namespace Core.Common.Controls.TreeView
{
    public class TreeView : System.Windows.Forms.TreeView
    {
        private readonly TreeViewController controller;

        public TreeView()
        {
            controller = new TreeViewController(this);

            DrawMode = TreeViewDrawMode.OwnerDrawAll;
            LabelEdit = true;
            HideSelection = false;

            // Enable default double buffering processing
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Disable default CommCtrl painting on non-Vista systems
            if (!NativeInterop.IsWinVista)
            {
                SetStyle(ControlStyles.UserPaint, true);
            }
        }

        public TreeViewController TreeViewController
        {
            get
            {
                return controller;
            }
        }

        # region Logic for preventing expand/collapse on double click

        protected override void DefWndProc(ref Message m)
        {
            const int wmLbuttondblclk = 515;
            const int wmErasebkgnd = 0x0014;

            if (m.Msg == wmLbuttondblclk)
            {
                return; // Don't handle double click
            }

            if (m.Msg == wmErasebkgnd)
            {
                return; // Don't clear background as this only causes flicker
            }

            base.DefWndProc(ref m);
        }

        # endregion

        # region Double buffered tree view related logic (see http://dev.nomad-net.info/articles/double-buffered-tree-and-list-views)

        private const int tvFirst = 0x1100;
        private const int tvmSetbkcolor = tvFirst + 29;
        private const int tvmSetextendedstyle = tvFirst + 44;
        private const int tvsExDoublebuffer = 0x0004;

        private void UpdateExtendedStyles()
        {
            var style = 0;

            if (DoubleBuffered)
            {
                style |= tvsExDoublebuffer;
            }

            if (style != 0)
            {
                NativeInterop.SendMessage(Handle, tvmSetextendedstyle, (IntPtr) tvsExDoublebuffer, (IntPtr) style);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            UpdateExtendedStyles();

            if (!NativeInterop.IsWinXp)
            {
                NativeInterop.SendMessage(Handle, tvmSetbkcolor, IntPtr.Zero, (IntPtr) ColorTranslator.ToWin32(BackColor));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                var m = new Message
                {
                    HWnd = Handle,
                    Msg = NativeInterop.WmPrintclient,
                    WParam = e.Graphics.GetHdc(),
                    LParam = (IntPtr) NativeInterop.PrfClient
                };

                DefWndProc(ref m);

                e.Graphics.ReleaseHdc(m.WParam);
            }

            base.OnPaint(e);
        }

        private static class NativeInterop
        {
            public const int WmPrintclient = 0x0318;
            public const int PrfClient = 0x00000004;

            public static bool IsWinXp
            {
                get
                {
                    OperatingSystem os = Environment.OSVersion;
                    return (os.Platform == PlatformID.Win32NT) &&
                           ((os.Version.Major > 5) || ((os.Version.Major == 5) && (os.Version.Minor == 1)));
                }
            }

            public static bool IsWinVista
            {
                get
                {
                    OperatingSystem os = Environment.OSVersion;
                    return (os.Platform == PlatformID.Win32NT) && (os.Version.Major >= 6);
                }
            }

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        }

        # endregion
    }
}