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
using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Plugins.Chart.Commands;
using Core.Plugins.Chart.Legend;
using Core.Plugins.Chart.PresentationObjects;
using Core.Plugins.Chart.PropertyClasses;

namespace Core.Plugins.Chart
{
    /// <summary>
    /// This class ties all the components together to enable charting interaction.
    /// </summary>
    public class ChartPlugin : PluginBase
    {
        private ChartingRibbon chartingRibbon;
        private ChartLegendController chartLegendController;
        private bool activated;

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return chartingRibbon;
            }
        }

        public override void Activate()
        {
            chartLegendController = CreateLegendController(Gui);
            chartingRibbon = CreateRibbon(chartLegendController);

            chartLegendController.ToggleView();
            Gui.ViewHost.ActiveDocumentViewChanged += OnActiveDocumentViewChanged;
            activated = true;
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<ChartDataCollection, ChartDataCollectionProperties>();
            yield return new PropertyInfo<ChartLineData, ChartLineDataProperties>();
            yield return new PropertyInfo<ChartMultipleAreaData, ChartMultipleAreaDataProperties>();
            yield return new PropertyInfo<ChartMultipleLineData, ChartMultipleLineDataProperties>();
            yield return new PropertyInfo<ChartPointData, ChartPointDataProperties>();
            yield return new PropertyInfo<PointBasedChartData, PointBasedChartDataProperties>();
        }

        public override void Dispose()
        {
            if (activated)
            {
                Gui.ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
            }

            base.Dispose();
        }

        /// <summary>
        /// Creates a new <see cref="ChartLegendController"/>.
        /// </summary>
        /// <param name="viewController">The <see cref="IViewController"/> to use for the controller
        /// <see cref="ChartLegendController"/>.</param>
        /// <returns>A new <see cref="ChartLegendController"/> instance.</returns>
        private ChartLegendController CreateLegendController(IViewController viewController)
        {
            var controller = new ChartLegendController(viewController, Gui);
            controller.OnOpenLegend += (s, e) => UpdateComponentsForActiveDocumentView();
            return controller;
        }

        /// <summary>
        /// Creates the <see cref="ChartingRibbon"/> and the commands that will be used when clicking on the buttons.
        /// </summary>
        /// <param name="chartLegendController">The <see cref="ChartLegendController"/> to use for the 
        /// <see cref="ChartingRibbon"/>.</param>
        /// <returns>A new <see cref="ChartingRibbon"/> instance.</returns>
        private static ChartingRibbon CreateRibbon(ChartLegendController chartLegendController)
        {
            return new ChartingRibbon
            {
                ToggleLegendViewCommand = new ToggleLegendViewCommand(chartLegendController)
            };
        }

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            UpdateComponentsForActiveDocumentView();
        }

        /// <summary>
        /// Updates the components which the <see cref="ChartPlugin"/> knows about so that it reflects
        /// the currently active view.
        /// </summary>
        private void UpdateComponentsForActiveDocumentView()
        {
            var chartView = Gui.ViewHost.ActiveDocumentView as IChartView;
            IChartControl chartControl = chartView?.Chart;

            chartLegendController.Update(chartControl);
            chartingRibbon.Chart = chartControl;
        }
    }
}