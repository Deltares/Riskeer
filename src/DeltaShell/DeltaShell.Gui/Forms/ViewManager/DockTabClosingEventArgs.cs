using System;
using DelftTools.Controls;

namespace DeltaShell.Gui.Forms.ViewManager
{
    public class DockTabClosingEventArgs : EventArgs
    {
        /// <summary>
        /// View trying to close (because a tab is being closed)
        /// </summary>
        public IView View { get; set; }

        /// <summary>
        /// Specifies the close action should be cancelled
        /// </summary>
        public bool Cancel { get; set; }
    }
}