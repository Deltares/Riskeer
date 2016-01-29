// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Core.Common.Controls.TreeView
{
    public class TreeView : System.Windows.Forms.TreeView
    {
        private readonly TreeViewController controller;

        public TreeView()
        {
            controller = new TreeViewController(this);

            StateImageList = new ImageList();

            StateImageList.Images.Add(CreateCheckBoxGlyph(CheckBoxState.UncheckedNormal));
            StateImageList.Images.Add(CreateCheckBoxGlyph(CheckBoxState.CheckedNormal));

            DrawMode = TreeViewDrawMode.OwnerDrawText;
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

        private Image CreateCheckBoxGlyph(CheckBoxState state)
        {
            Bitmap result = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(result))
            {
                Size glyphSize = CheckBoxRenderer.GetGlyphSize(g, state);
                CheckBoxRenderer.DrawCheckBox(g,
                  new Point((result.Width - glyphSize.Width) / 2, (result.Height - glyphSize.Height) / 2), state);
            }
            return result;
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