﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ViewHost;
using Core.Components.Gis.Data;
using Core.Plugins.Map.Properties;

namespace Core.Plugins.Map.Legend
{
    /// <summary>
    /// This class controls the actions which are related to controlling visibility and updating contents of a <see cref="MapLegendView"/>.
    /// </summary>
    public class MapLegendController
    {
        private readonly IViewController viewController;
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;
        private readonly IWin32Window parentWindow;

        /// <summary>
        /// Fired when the map legend has been opened.
        /// </summary>
        public EventHandler<EventArgs> OnOpenLegend;

        private IView legendView;

        /// <summary>
        /// Creates a new instance of <see cref="MapLegendController"/>.
        /// </summary>
        /// <param name="viewController">The <see cref="IViewController"/> to invoke actions upon.</param>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> to create context menus.</param>
        /// <param name="parentWindow">The <see cref="IWin32Window"/> to show dialogs.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewController"/> or <paramref name="contextMenuBuilderProvider"/> is <c>null</c>.</exception>
        public MapLegendController(IViewController viewController, IContextMenuBuilderProvider contextMenuBuilderProvider, IWin32Window parentWindow)
        {
            if (viewController == null)
            {
                throw new ArgumentNullException("viewController", @"Cannot create a MapLegendController when the view controller is null.");
            }
            if (contextMenuBuilderProvider == null)
            {
                throw new ArgumentNullException("contextMenuBuilderProvider", @"Cannot create a MapLegendController when the context menu builder provider is null.");
            }
            if (parentWindow == null)
            {
                throw new ArgumentNullException("parentWindow", @"Cannot create a MapLegendController when the parent window is null");
            }
            this.viewController = viewController;
            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
            this.parentWindow = parentWindow;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MapLegendView"/> is visible.
        /// </summary>
        public bool IsMapLegendViewOpen
        {
            get
            {
                return legendView != null && viewController.ViewHost.ToolViews.Contains(legendView);
            }
        }

        /// <summary>
        /// Toggles the visibility of the <see cref="MapLegendView"/>.
        /// </summary>
        public void ToggleView()
        {
            if (IsMapLegendViewOpen)
            {
                CloseLegendView();
            }
            else
            {
                OpenLegendView();
            }
        }

        /// <summary>
        /// Updates the data for the <see cref="MapLegendView"/> if it is open.
        /// </summary>
        /// <param name="data">The <see cref="MapData"/> to show. If <c>null</c> the data 
        /// will be cleared.</param>
        public void Update(MapData data)
        {
            if (IsMapLegendViewOpen)
            {
                legendView.Data = data;
            }
        }

        /// <summary>
        /// Opens the <see cref="MapLegendView"/>.
        /// </summary>
        private void OpenLegendView()
        {
            legendView = new MapLegendView(contextMenuBuilderProvider, parentWindow);

            viewController.ViewHost.AddToolView(legendView, ToolViewLocation.Left);
            viewController.ViewHost.SetImage(legendView, Resources.MapIcon);

            if (OnOpenLegend != null)
            {
                OnOpenLegend(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Closes the <see cref="MapLegendView"/>.
        /// </summary>
        private void CloseLegendView()
        {
            viewController.ViewHost.Remove(legendView);
            legendView = null;
        }
    }
}