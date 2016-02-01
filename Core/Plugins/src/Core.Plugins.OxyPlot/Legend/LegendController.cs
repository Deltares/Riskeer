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
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class controls the actions which are related to controlling visibility and updating contents of a <see cref="LegendView"/>.
    /// </summary>
    public class LegendController
    {
        private readonly IToolViewController toolViewController;
        private IView legendView;

        /// <summary>
        /// Creates a new instance of <see cref="LegendController"/>.
        /// </summary>
        /// <param name="toolViewController">The <see cref="OxyPlotGuiPlugin"/> to invoke actions upon.</param>
        public LegendController(IToolViewController toolViewController)
        {
            if (toolViewController == null)
            {
                throw new ArgumentNullException("toolViewController", "Cannot create a LegendController when the plugin is null.");
            }
            this.toolViewController = toolViewController;
        }

        /// <summary>
        /// Toggles the <see cref="LegendView"/>.
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
        /// Checks whether a <see cref="LegendView"/> is open.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="LegendView"/> is open, <c>false</c> otherwise.</returns>
        public bool IsLegendViewOpen()
        {
            return toolViewController.IsToolWindowOpen<LegendView>();
        }

        /// <summary>
        /// Open the <see cref="LegendView"/>.
        /// </summary>
        private void OpenLegendView()
        {
            legendView = new LegendView();
            toolViewController.OpenToolView(legendView);
        }

        /// <summary>
        /// Closes the <see cref="LegendView"/>.
        /// </summary>
        private void CloseLegendView()
        {
            toolViewController.CloseToolView(legendView);
            legendView.Dispose();
            legendView = null;
        }

        /// <summary>
        /// Updates the data for the <see cref="LegendView"/> if it is open.
        /// </summary>
        /// <param name="data">The <see cref="BaseChart"/> for which to show data. If <c>null</c> the 
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