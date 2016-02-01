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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
        /// <param name="removeTabFromDockingbar"></param>
        void Remove(IView view, bool removeTabFromDockingbar);

        /// <summary>
        /// Sets the tooltip of the container of the view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="tooltip"></param>
        void SetToolTip(IView view, string tooltip);

        /// <summary>
        /// Sets the image of the container of the view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="image"></param>
        void SetImage(IView view, Image image);

        /// <summary>
        /// Activates view.
        /// </summary>
        /// <param name="view"></param>
        void ActivateView(IView view);
    }
}