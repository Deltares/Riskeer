using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    public static class ControlHelper
    {
        [DllImport("user32.dll")]
        public extern static int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string textSubAppName, string textSubIdList);

        // Import GetFocus() from user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        public static Control GetFocusControl()
        {
            Control focusControl = null;
            var focusHandle = GetFocus();
            if (focusHandle != IntPtr.Zero)
            {
                // returns null if handle is not to a .NET control
                focusControl = Control.FromHandle(focusHandle);
            }

            return focusControl;
        }
        
        /// <summary>
        /// Call this method on a view if you want to trigger data binding. For example when switching between views, closing a 
        /// view or when performing a save.
        /// </summary>
        /// <param name="containerControl">control to unfocus / trigger validation</param>
        /// <param name="notSwitchingToOtherControl">If 'true', it messes with switch to another control/tab, but it is required 
        /// if you close a view or want the current view to commit any changes.</param>
        public static void UnfocusActiveControl(IContainerControl containerControl, bool notSwitchingToOtherControl=false)
        {
            if (containerControl == null)
                return;

            while (containerControl.ActiveControl is IContainerControl)
                containerControl = containerControl.ActiveControl as IContainerControl;

            var control = containerControl as Control;
            if (notSwitchingToOtherControl && control != null && !control.ContainsFocus)
                control.Focus();
            
            containerControl.ActiveControl = null; //unfocus current control, to force binding to happen
        }
        
        public static IEnumerable<T> GetChildControls<T>(Control control)
        {
            var controls = control.Controls.Cast<Control>();
            return control.Controls.OfType<T>().Concat<T>(controls.SelectMany(GetChildControls<T>));
        }
    }
}