// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms.ViewHost
{
    /// <summary>
    /// Interface for an object capable of hosting document views and tool views.
    /// </summary>
    public interface IViewHost : IDisposable
    {
        /// <summary>
        /// Fired before <see cref="ActiveDocumentView"/> has changed.
        /// </summary>
        event EventHandler<EventArgs> ActiveDocumentViewChanging;

        /// <summary>
        /// Fired after <see cref="ActiveDocumentView"/> has changed.
        /// </summary>
        event EventHandler<EventArgs> ActiveDocumentViewChanged;

        /// <summary>
        /// Fired after the active document or tool view has changed.
        /// </summary>
        event EventHandler<ViewChangeEventArgs> ActiveViewChanged;

        /// <summary>
        /// Fired when a document view or a tool view has been opened.
        /// </summary>
        event EventHandler<ViewChangeEventArgs> ViewOpened;

        /// <summary>
        /// Fired when an already opened document view or tool view is brought to front.
        /// </summary>
        event EventHandler<ViewChangeEventArgs> ViewBroughtToFront;

        /// <summary>
        /// Fired when a document view or a tool view has been closed.
        /// </summary>
        event EventHandler<ViewChangeEventArgs> ViewClosed;

        /// <summary>
        /// Gets the added document views.
        /// </summary>
        IEnumerable<IView> DocumentViews { get; }

        /// <summary>
        /// Gets the added tool views.
        /// </summary>
        IEnumerable<IView> ToolViews { get; }

        /// <summary>
        /// Gets the active document view.
        /// </summary>
        IView ActiveDocumentView { get; }

        /// <summary>
        /// Adds a document view and makes it active.
        /// </summary>
        /// <param name="view">The document view to add.</param>
        /// <seealso cref="ActiveDocumentView"/>
        void AddDocumentView(IView view);

        /// <summary>
        /// Adds a tool view.
        /// </summary>
        /// <param name="view">The tool view add.</param>
        /// <param name="toolViewLocation">The location where the tool view should be added.</param>
        void AddToolView(IView view, ToolViewLocation toolViewLocation);

        /// <summary>
        /// Removes a document view or tool view.
        /// </summary>
        /// <param name="view">The view to remove.</param>
        void Remove(IView view);

        /// <summary>
        /// Brings a document view or tool view to the front.
        /// </summary>
        /// <param name="view">The view to bring to front.</param>
        void BringToFront(IView view);

        /// <summary>
        /// Sets the image for a document view or tool view.
        /// </summary>
        /// <param name="view">The view to set the image for.</param>
        /// <param name="image">The image.</param>
        void SetImage(IView view, Image image);
    }
}