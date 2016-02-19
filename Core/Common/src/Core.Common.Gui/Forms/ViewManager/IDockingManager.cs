// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// Interface for objects controlling view docking behavior.
    /// </summary>
    public interface IDockingManager : IDisposable
    {
        /// <summary>
        /// Occurs when the docking-bar of a view trying to close. 
        /// </summary>
        event EventHandler<DockTabClosingEventArgs> ViewBarClosing;

        /// <summary>
        /// Occurs when a view has been activated.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ViewActivated;

        /// <summary>
        /// Occurs when a 'mouse down' event occurs on the docking control.
        /// </summary>
        event Action<object, MouseEventArgs, IView> ViewSelectionMouseDown;

        /// <summary>
        /// Gets the views that are available within the docking control.
        /// </summary>
        IEnumerable<IView> Views { get; }

        /// <summary>
        /// Adds the view at specified docking location.
        /// </summary>
        /// <param name="view">View to add.</param>
        /// <param name="location">Location of the view.</param>
        void Add(IView view, ViewLocation location);

        /// <summary>
        /// Removes the view from docking-bars. If the docking-bar is empty, it is also removed.
        /// </summary>
        /// <param name="view">View to remove.</param>
        /// <param name="removeTabFromDockingbar">When set to <c>true</c>, the docking-bar
        /// that hosts <paramref name="view"/> is closed. Otherwise only the view is removed
        /// from the docking-bar.</param>
        void Remove(IView view, bool removeTabFromDockingbar);

        /// <summary>
        /// Sets the tooltip of the container corresponding to the view.
        /// </summary>
        /// <param name="view">A view added to the docking control.</param>
        /// <param name="tooltip">The tooltip text.</param>
        void SetToolTip(IView view, string tooltip);

        /// <summary>
        /// Sets the image of the container corresponding to the view.
        /// </summary>
        /// <param name="view">A view added to the docking control.</param>
        /// <param name="image">The image.</param>
        void SetImage(IView view, Image image);

        /// <summary>
        /// Activates the view.
        /// </summary>
        /// <param name="view">The view to activate.</param>
        void ActivateView(IView view);
    }
}