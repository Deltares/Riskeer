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
using Core.Common.Gui;
using Core.Components.DotSpatial;
using Core.Components.DotSpatial.Data;

namespace Core.Plugins.DotSpatial.Legend
{
    /// <summary>
    /// This class controls the actions which are related to controlling visibility and updating contents of a <see cref="MapLegendView"/>.
    /// </summary>
    public class MapLegendController
    {
        private readonly IToolViewController toolViewController;
        private IView legendView;

        public EventHandler<EventArgs> OnOpenLegend;

        /// <summary>
        /// Creates a new instance of <see cref="MapLegendController"/>
        /// </summary>
        /// <param name="toolViewController">The <see cref="DotSpatialGuiPlugin"/> to invoke actions upon.</param>
        public MapLegendController(IToolViewController toolViewController)
        {
            if (toolViewController == null)
            {
                throw new ArgumentNullException("toolViewController", "Cannot create a MapLegendController when the plugin is null.");
            }
            this.toolViewController = toolViewController;
        }

        /// <summary>
        /// Toggles the <see cref="MapLegendView"/>.
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
        /// Checks whether a <see cref="MapLegendView"/> is open.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="MapLegendView"/> is open, <c>false</c> otherwise.</returns>
        public bool IsLegendViewOpen()
        {
            return toolViewController.IsToolWindowOpen<MapLegendView>();
        }

        /// <summary>
        /// Open the <see cref="MapLegendView"/>.
        /// </summary>
        private void OpenLegendView()
        {
            legendView = new MapLegendView();
            toolViewController.OpenToolView(legendView);
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
            toolViewController.CloseToolView(legendView);
            legendView.Dispose();
            legendView = null;
        }

        /// <summary>
        /// Updates the data for the <see cref="MapLegendView"/> if it is open.
        /// </summary>
        /// <param name="data">The <see cref="BaseMap"/> for which to show data. If <c>null</c> the 
        /// data will be cleared.</param>
        public void Update(MapData data)
        {
            if (IsLegendViewOpen())
            {
                legendView.Data = data;
            }
        }
    }
}
