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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Core.Common.Gui
{
    public static class ControlHelper
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string textSubAppName, string textSubIdList);

        /// <summary>
        /// Call this method on a view if you want to trigger data binding. For example when switching between views, closing a 
        /// view or when performing a save.
        /// </summary>
        /// <param name="containerControl">control to unfocus / trigger validation</param>
        /// <param name="notSwitchingToOtherControl">If 'true', it messes with switch to another control/tab, but it is required 
        /// if you close a view or want the current view to commit any changes.</param>
        public static void UnfocusActiveControl(IContainerControl containerControl, bool notSwitchingToOtherControl = false)
        {
            if (containerControl == null)
            {
                return;
            }

            while (containerControl.ActiveControl is IContainerControl)
            {
                containerControl = containerControl.ActiveControl as IContainerControl;
            }

            var control = containerControl as Control;
            if (notSwitchingToOtherControl && control != null && !control.ContainsFocus)
            {
                control.Focus();
            }

            containerControl.ActiveControl = null; //unfocus current control, to force binding to happen
        }

        // Import GetFocus() from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();
    }
}