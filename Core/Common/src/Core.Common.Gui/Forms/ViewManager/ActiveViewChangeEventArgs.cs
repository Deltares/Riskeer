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

using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// Event arguments for changes to the <see cref="IViewList.ActiveView"/>.
    /// </summary>
    public class ActiveViewChangeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveViewChangeEventArgs"/> class.
        /// </summary>
        /// <param name="currentActiveView">The current active view.</param>
        public ActiveViewChangeEventArgs(IView currentActiveView)
        {
            View = currentActiveView;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveViewChangeEventArgs"/> class.
        /// </summary>
        /// <param name="currentActiveView">The current active view.</param>
        /// <param name="previousActiveView">The previously active view.</param>
        public ActiveViewChangeEventArgs(IView currentActiveView, IView previousActiveView)
        {
            View = currentActiveView;
            OldView = previousActiveView;
        }

        /// <summary>
        /// The current active view.
        /// </summary>
        public IView View { get; private set; }

        /// <summary>
        /// The previously active view. Is not null when switching from one view to another.
        /// </summary>
        public IView OldView { get; private set; }
    }
}