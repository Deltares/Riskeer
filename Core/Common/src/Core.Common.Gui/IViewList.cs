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
using Core.Common.Controls.Views;
using Core.Common.Utils.Events;

namespace Core.Common.Gui
{
    [Flags]
    public enum ViewLocation
    {
        Document = 0x0,
        Left = 0x1,
        Right = 0x2,
        Top = 0x4,
        Bottom = 0x8,
        Floating = 0x16
    };

    /// <summary>
    /// Manages currently displayed views
    /// </summary>
    public interface IViewList : IList<IView>, IDisposable
    {
        /// <summary>
        /// Fired before active view has been changed.
        /// </summary>
        event EventHandler<ActiveViewChangeEventArgs> ActiveViewChanging;

        /// <summary>
        /// Fired after active view has been changed.
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
        /// Gets or sets active view, when view is active - its window is activated.
        /// </summary>
        IView ActiveView { get; set; }

        /// <summary>
        /// Returns all views. Including views inside composite views
        /// </summary>
        IEnumerable<IView> AllViews { get; }

        /// <summary>
        /// Adds a view to the UI. 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="viewLocation"></param>
        void Add(IView view, ViewLocation viewLocation);

        /// <summary>
        /// Sets the tooltip of the view
        /// </summary>
        /// <param name="view"></param>
        /// <param name="tooltip"></param>
        void SetTooltip(IView view, string tooltip);

        /// <summary>
        /// Updates the name of the view
        /// </summary>
        /// <param name="view"></param>
        void UpdateViewName(IView view);

        /// <summary>
        /// Overloaded Clear, removes all views except <paramref name="viewToKeep"/>
        /// </summary>
        /// <param name="viewToKeep"></param>
        void Clear(IView viewToKeep);
    }
}