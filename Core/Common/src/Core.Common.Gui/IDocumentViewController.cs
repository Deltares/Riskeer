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

using Core.Common.Controls.Views;
using Core.Common.Gui.Forms.ViewManager;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface for controller that controls Document Views in the application.
    /// </summary>
    public interface IDocumentViewController
    {
        /// <summary>
        /// Fired when the active view in the document pane changes.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        /// <summary>
        /// Gets the currently active document <see cref="IView"/>.
        /// </summary>
        IView ActiveView { get; }

        /// <summary>
        ///  Gets all document views currently opened in the gui.
        /// </summary>
        IViewList DocumentViews { get; }

        /// <summary>
        /// Resolves document views
        /// </summary>
        IViewResolver DocumentViewsResolver { get; }

        /// <summary>
        /// Suspends view removal on item delete. Useful to avoid unnecessary checks (faster item removal).
        /// </summary>
        bool IsViewRemoveOnItemDeleteSuspended { get; set; }

        /// <summary>
        /// Update the tool tip for every view currently open. Reasons for doing so 
        /// include the modification of the tree structure which is reflected in a tool tip.
        /// </summary>
        void UpdateToolTips();
    }
}