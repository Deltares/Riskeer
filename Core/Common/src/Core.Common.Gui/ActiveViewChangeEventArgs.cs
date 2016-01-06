using System;
using Core.Common.Controls;
using Core.Common.Forms.Views;

namespace Core.Common.Gui
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