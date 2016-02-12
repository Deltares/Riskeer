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

using Core.Common.Controls.Views;
using Core.Common.Utils.Events;

namespace Core.Common.Gui.Forms.ViewManager
{
    /// <summary>
    /// Flags on where views can be located.
    /// </summary>
    [Flags]
    public enum ViewLocation
    {
        /// <summary>
        /// The location reserved for Document Views.
        /// </summary>
        Document = 0x0,
        /// <summary>
        /// Left of the location reserved for Document Views.
        /// </summary>
        Left = 0x1,
        /// <summary>
        /// Right of the location reserved for Document Views.
        /// </summary>
        Right = 0x2,
        /// <summary>
        /// Above the location reserved for Document Views.
        /// </summary>
        Top = 0x4,
        /// <summary>
        /// Below the location reserved for Document Views.
        /// </summary>
        Bottom = 0x8,
        /// <summary>
        /// Floating panel.
        /// </summary>
        Floating = 0x16
    };

    /// <summary>
    /// Manages currently displayed views.
    /// </summary>
    public interface IViewList : IList<IView>, IDisposable
    {
        /// <summary>
        /// Fired before active view has changed.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanging;

        /// <summary>
        /// Fired after active view has changed.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanged;

        /// <summary>
        /// Fired after the view elements in this view list have changed.
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// HACK: Hack to disable activation temporarily
        /// </summary>
        bool IgnoreActivation { get; set; }

        /// <summary>
        /// Gets or sets active view.
        /// </summary>
        IView ActiveView { get; set; }

        /// <summary>
        /// Returns all views.
        /// </summary>
        IEnumerable<IView> AllViews { get; }

        /// <summary>
        /// Adds a view to the view list and makes it active. 
        /// </summary>
        /// <param name="view">The view to be added.</param>
        /// <param name="viewLocation">The location where the view should be added.</param>
        void Add(IView view, ViewLocation viewLocation);

        /// <summary>
        /// Sets the tooltip of the view's container.
        /// </summary>
        /// <param name="view">The view already part of this list.</param>
        /// <param name="tooltip">The text of the tooltip.</param>
        void SetTooltip(IView view, string tooltip);

        /// <summary>
        /// Removes all views except <paramref name="viewToKeep"/>
        /// </summary>
        /// <param name="viewToKeep">The view that should be kept open and made the active view. 
        /// If set to null, all views will be closed and <see cref="ActiveView"/> will be null.</param>
        void RemoveAllExcept(IView viewToKeep);

        /// <summary>
        /// Removes all views in the list except for those specified.
        /// </summary>
        /// <param name="viewsToKeep">The views to keep.</param>
        void RemoveAllExcept(IView[] viewsToKeep);
    }
}