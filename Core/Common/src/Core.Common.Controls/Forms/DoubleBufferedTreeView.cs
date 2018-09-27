// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Core.Common.Controls.Forms
{
    /// <summary>
    /// Double buffered version of<see cref="TreeView"/>.
    /// </summary>
    /// <remarks>
    /// Also see http://dev.nomad-net.info/articles/double-buffered-tree-and-list-views).
    /// </remarks>
    public class DoubleBufferedTreeView : TreeView
    {
        private const int tvFirst = 0x1100;
        private const int tvmSetbkcolor = tvFirst + 29;
        private const int tvmSetextendedstyle = tvFirst + 44;
        private const int tvsExDoublebuffer = 0x0004;

        /// <summary>
        /// Creates a new instance of <see cref="DoubleBufferedTreeView"/>.
        /// </summary>
        public DoubleBufferedTreeView()
        {
            // Enable default double buffering processing
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Disable default CommCtrl painting on non-Vista systems
            if (!NativeMethods.IsWinVista)
            {
                SetStyle(ControlStyles.UserPaint, true);
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            UpdateExtendedStyles();

            if (!NativeMethods.IsWinXp)
            {
                NativeMethods.SendMessage(Handle, tvmSetbkcolor, IntPtr.Zero, (IntPtr) ColorTranslator.ToWin32(BackColor));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                var m = new Message
                {
                    HWnd = Handle,
                    Msg = NativeMethods.WmPrintclient,
                    WParam = e.Graphics.GetHdc(),
                    LParam = (IntPtr) NativeMethods.PrfClient
                };

                DefWndProc(ref m);

                e.Graphics.ReleaseHdc(m.WParam);
            }

            base.OnPaint(e);
        }

        private void UpdateExtendedStyles()
        {
            var style = 0;

            if (DoubleBuffered)
            {
                style |= tvsExDoublebuffer;
            }

            if (style != 0)
            {
                NativeMethods.SendMessage(Handle, tvmSetextendedstyle, (IntPtr) tvsExDoublebuffer, (IntPtr) style);
            }
        }

        private static class NativeMethods
        {
            public const int WmPrintclient = 0x0318;
            public const int PrfClient = 0x00000004;

            public static bool IsWinXp
            {
                get
                {
                    OperatingSystem os = Environment.OSVersion;
                    return os.Platform == PlatformID.Win32NT &&
                           (os.Version.Major > 5 || os.Version.Major == 5 && os.Version.Minor == 1);
                }
            }

            public static bool IsWinVista
            {
                get
                {
                    OperatingSystem os = Environment.OSVersion;
                    return os.Platform == PlatformID.Win32NT && os.Version.Major >= 6;
                }
            }

            [DllImport("user32.dll")]
            public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        }
    }
}