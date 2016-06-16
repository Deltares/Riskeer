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
using Core.Common.Gui;
using Core.Components.Charting.Data;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class controls the actions which are related to controlling visibility and updating contents of a <see cref="ChartLegendView"/>.
    /// </summary>
    public class ChartLegendController
    {
        private readonly IToolViewController toolViewController;
        private IView legendView;

        /// <summary>
        /// Fired when the legend has been opened.
        /// </summary>
        public EventHandler<EventArgs> OnOpenLegend;

        /// <summary>
        /// Creates a new instance of <see cref="ChartLegendController"/>.
        /// </summary>
        /// <param name="toolViewController">The <see cref="IToolViewController"/> to invoke actions upon.</param>
        public ChartLegendController(IToolViewController toolViewController)
        {
            if (toolViewController == null)
            {
                throw new ArgumentNullException("toolViewController", "Cannot create a ChartLegendController when the tool view controller is null.");
            }
            this.toolViewController = toolViewController;
        }

        /// <summary>
        /// Toggles the <see cref="ChartLegendView"/>.
        /// </summary>
        public void ToggleLegend()
        {
            if (IsLegendViewOpen())
            {
                CloseLegendView();
            }
            else
            {
                OpenLegendView();
            }
        }

        /// <summary>
        /// Checks whether a <see cref="ChartLegendView"/> is open.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="ChartLegendView"/> is open, <c>false</c> otherwise.</returns>
        public bool IsLegendViewOpen()
        {
            return toolViewController.IsToolWindowOpen<ChartLegendView>();
        }

        /// <summary>
        /// Open the <see cref="ChartLegendView"/>.
        /// </summary>
        private void OpenLegendView()
        {
            legendView = new ChartLegendView();
            toolViewController.OpenToolView(legendView);
            if (OnOpenLegend != null)
            {
                OnOpenLegend(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Closes the <see cref="ChartLegendView"/>.
        /// </summary>
        private void CloseLegendView()
        {
            toolViewController.CloseToolView(legendView);
            legendView.Dispose();
            legendView = null;
        }

        /// <summary>
        /// Updates the data for the <see cref="ChartLegendView"/> if it is open.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to show. If <c>null</c> the 
        /// data will be cleared.</param>
        public void Update(ChartData data)
        {
            if (IsLegendViewOpen())
            {
                legendView.Data = data;
            }
        }
    }
}