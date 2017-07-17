// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Core.Common.Gui.Forms
{
    /// <summary>
    /// Helper methods related to <see cref="Control"/> instances.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Sends the specified message to a window or windows. The <see cref="SendMessage"/>
        /// function calls the window procedure for the specified window and does not 
        /// return until the window procedure has processed the message.
        /// </summary>
        /// <param name="hWnd">A handle to the window whose window procedure will receive 
        /// the message. If this parameter is HWND_BROADCAST ((HWND)0xffff), the message 
        /// is sent to all top-level windows in the system, including disabled or invisible 
        /// unowned windows, overlapped windows, and pop-up windows; but the message is not 
        /// sent to child windows. Message sending is subject to UIPI. The thread of a 
        /// process can send messages only to message queues of threads in processes of 
        /// lesser or equal integrity level.</param>
        /// <param name="wMsg">The message to be sent. For lists of the system-provided 
        /// messages, see https://msdn.microsoft.com/en-us/library/windows/desktop/ms644927(v=vs.85).aspx#system_defined.
        /// </param>
        /// <param name="wParam">Additional message-specific information.</param>
        /// <param name="lParam">Additional message-specific information.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        /// <summary>
        /// Causes a window to use a different set of visual style information than its class normally uses.
        /// </summary>
        /// <param name="hWnd">Handle to the window whose visual style information is to be changed.</param>
        /// <param name="textSubAppName">Pointer to a string that contains the application 
        /// name to use in place of the calling application's name. If this parameter is <c>null</c>, 
        /// the calling application's name is used.</param>
        /// <param name="textSubIdList">Pointer to a string that contains a semicolon-separated
        /// list of CLSID names to use in place of the actual list passed by the window's 
        /// class. If this parameter is <c>null</c>, the ID list from the calling class is used.</param>
        /// <returns></returns>
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string textSubAppName, string textSubIdList);
    }
}