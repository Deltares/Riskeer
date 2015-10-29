using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Core.Common.Controls.Swf
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