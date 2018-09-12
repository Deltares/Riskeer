// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ViewHost;
using Core.Components.Chart.Forms;
using Core.Plugins.Chart.Properties;

namespace Core.Plugins.Chart.Legend
{
    /// <summary>
    /// This class controls the actions which are related to controlling visibility and updating contents of a <see cref="ChartLegendView"/>.
    /// </summary>
    public class ChartLegendController
    {
        private readonly IViewController viewController;
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;

        /// <summary>
        /// Fired when the chart legend has been opened.
        /// </summary>
        public event EventHandler<EventArgs> OnOpenLegend;

        private ChartLegendView legendView;

        /// <summary>
        /// Creates a new instance of <see cref="ChartLegendController"/>.
        /// </summary>
        /// <param name="viewController">The <see cref="IViewController"/> to invoke actions upon.</param>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> to create context menus.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="viewController"/> 
        /// or <paramref name="contextMenuBuilderProvider"/> is <c>null</c>.</exception>
        public ChartLegendController(IViewController viewController, IContextMenuBuilderProvider contextMenuBuilderProvider)
        {
            if (viewController == null)
            {
                throw new ArgumentNullException(nameof(viewController),
                                                $@"Cannot create a {typeof(ChartLegendController).Name} when the view controller is null.");
            }
            if (contextMenuBuilderProvider == null)
            {
                throw new ArgumentNullException(nameof(contextMenuBuilderProvider),
                                                $@"Cannot create a {typeof(ChartLegendController).Name} when the context menu builder provider is null.");
            }
            this.viewController = viewController;
            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ChartLegendView"/> is visible.
        /// </summary>
        public bool IsLegendViewOpen
        {
            get
            {
                return legendView != null && viewController.ViewHost.ToolViews.Contains(legendView);
            }
        }

        /// <summary>
        /// Toggles the visibility of the <see cref="ChartLegendView"/>.
        /// </summary>
        public void ToggleView()
        {
            if (IsLegendViewOpen)
            {
                CloseLegendView();
            }
            else
            {
                OpenLegendView();
            }
        }

        /// <summary>
        /// Updates the data for the <see cref="ChartLegendView"/> if it is open.
        /// </summary>
        /// <param name="chartControl">The <see cref="IChartControl"/> for which to show a
        /// legend. If <c>null</c> the data will be cleared.</param>
        public void Update(IChartControl chartControl)
        {
            if (IsLegendViewOpen)
            {
                legendView.ChartControl = chartControl;
            }
        }

        /// <summary>
        /// Open the <see cref="ChartLegendView"/>.
        /// </summary>
        private void OpenLegendView()
        {
            legendView = new ChartLegendView(contextMenuBuilderProvider);

            viewController.ViewHost.AddToolView(legendView, ToolViewLocation.Left);
            viewController.ViewHost.SetImage(legendView, Resources.ChartIcon);

            OnOpenLegend?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Closess the <see cref="ChartLegendView"/>.
        /// </summary>
        private void CloseLegendView()
        {
            viewController.ViewHost.Remove(legendView);
            legendView = null;
        }
    }
}