using System;
using DelftTools.Controls;

namespace DelftTools.Shell.Gui
{
    public class ActiveViewChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Current view.
        /// </summary>
        public IView View { get; set; }

        /// <summary>
        /// Previous view.
        /// </summary>
        public IView OldView { get; set; }
    }
}