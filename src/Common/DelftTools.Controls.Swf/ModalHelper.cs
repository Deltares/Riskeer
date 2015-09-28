using System;
using System.Windows.Forms;

namespace DelftTools.Controls.Swf
{
    public static class ModalHelper
    {
        private static WeakReference mainWindowRef;
        public static IWin32Window MainWindow
        {
            private get
            {
                if (mainWindowRef != null)
                    return (IWin32Window) mainWindowRef.Target;
                return null;
            }
            set { mainWindowRef = value != null ? new WeakReference(value) : null; }
        }

        public static DialogResult ShowModal(Form f)
        {
            if (MainWindow == null)
                throw new InvalidOperationException("Main window not injected");

            return f.ShowDialog(MainWindow);
        }
    }
}