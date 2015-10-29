using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Gui;

namespace Application.Ringtoets.Forms.ViewManager
{
    /// <summary>
    /// Interface providing view docking control. Implemented in DotNetBar for now.
    /// </summary>
    public interface IDockingManager : IDisposable
    {
        /// <summary>
        /// Occurs when the bar of a view trying to close. 
        /// </summary>
        event EventHandler<DockTabClosingEventArgs> ViewBarClosing;

        /// <summary>
        /// Occurs when a view got activated by clicked or entering it otherways
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ViewActivated;

        event Action<object, MouseEventArgs, IView> ViewSelectionMouseDown;

        IEnumerable<IView> Views { get; }

        /// <summary>
        /// Adds view at specified location
        /// </summary>
        /// <param name="view">View to add</param>
        /// <param name="location">Location of the view</param>
        void Add(IView view, ViewLocation location);

        /// <summary>
        /// Removes view and container from bars. If bar is empty it is also removed
        /// </summary>
        /// <param name="view"></param>
        void Remove(IView view, bool removeTabFromDockingbar);

        /// <summary>
        /// Sets the tooltip of the container of the view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="tooltip"></param>
        void SetToolTip(IView view, string tooltip);

        /// <summary>
        /// Activates view.
        /// </summary>
        /// <param name="viewText"></param>
        void ActivateView(IView view);
    }
}