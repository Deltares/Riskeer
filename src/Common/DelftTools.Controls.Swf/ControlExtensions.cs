using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    ///<summary>
    /// Extension methods for the class Control
    ///</summary>
    public static class ControlExtensions
    {
        private const int WM_SETREDRAW = 11;

        ///<summary>
        /// Gets the first found control of type in the control container. Search is recursive.
        ///</summary>
        ///<param name="control">The control container to search in</param>
        ///<typeparam name="T">The type of control to search for</typeparam>
        ///<returns>Returns the first control found, returns null otherwise</returns>
        public static T GetFirstControlOfType<T>(this Control control) where T : Control
        {
            return GetAllControlsRecursive<T>(control).FirstOrDefault();
        }

        ///<summary>
        /// Gets all child controls (recursive) of a control container (parent)
        ///</summary>
        ///<param name="container">The parent container</param>
        ///<typeparam name="T">The type of control to look for</typeparam>
        ///<returns>A list of controls of type <typeparamref name="T"/></returns>
        public static IEnumerable<T> GetAllControlsRecursive<T>(this Control container) where T : Control
        {
            return GetAllControlsRecursive(container).OfType<T>();
        }

        ///<summary>
        /// Gets all child controls (recursive) of a control container (parent)
        ///</summary>
        ///<param name="container">The parent container</param>
        ///<returns>A list of all child controls></returns>
        public static IEnumerable<Control> GetAllControlsRecursive(this Control container)
        {
            return GetAllControlsRecursive(container.Controls);
        }

        ///<summary>
        /// Gets all child controls (recursive) of a control container (parent)
        ///</summary>
        ///<param name="controlCollection">The parent container</param>
        ///<returns>A list of all child controls></returns>
        public static IEnumerable<Control> GetAllControlsRecursive(this Control.ControlCollection controlCollection)
        {
            foreach (Control child in controlCollection)
            {
                foreach (Control parent in GetAllControlsRecursive(child).Where(parent => parent != null))
                {
                    yield return parent;
                }
                if (child != null)
                {
                    yield return child;
                }
            }
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        public static void SuspendDrawing(this Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(this Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }
    }
}