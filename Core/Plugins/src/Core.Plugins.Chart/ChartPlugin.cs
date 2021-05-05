// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Core.Gui;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.PropertyClasses.Chart;
using Core.Plugins.Chart.Legend;

namespace Core.Plugins.Chart
{
    /// <summary>
    /// This class ties all the components together to enable charting interaction.
    /// </summary>
    public class ChartPlugin : PluginBase
    {
        private bool activated;
        private IChartView currentChartView;
        private ChartLegendController chartLegendController;

        public override void Activate()
        {
            chartLegendController = CreateLegendController(Gui);
            chartLegendController.ToggleView();

            Gui.ViewHost.ViewOpened += OnViewOpened;
            Gui.ViewHost.ViewBroughtToFront += OnViewBroughtToFront;
            Gui.ViewHost.ViewClosed += OnViewClosed;
            Gui.ViewHost.ActiveDocumentViewChanged += OnActiveDocumentViewChanged;

            activated = true;
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<ChartDataCollection, ChartDataCollectionProperties>();
            yield return new PropertyInfo<ChartLineData, ChartLineDataProperties>();
            yield return new PropertyInfo<ChartAreaData, ChartAreaDataProperties>();
            yield return new PropertyInfo<ChartMultipleAreaData, ChartMultipleAreaDataProperties>();
            yield return new PropertyInfo<ChartMultipleLineData, ChartMultipleLineDataProperties>();
            yield return new PropertyInfo<ChartPointData, ChartPointDataProperties>();
        }

        protected override void Dispose(bool disposing)
        {
            if (activated && disposing)
            {
                Gui.ViewHost.ViewOpened -= OnViewOpened;
                Gui.ViewHost.ViewBroughtToFront -= OnViewBroughtToFront;
                Gui.ViewHost.ViewClosed -= OnViewClosed;
                Gui.ViewHost.ActiveDocumentViewChanged -= OnActiveDocumentViewChanged;
            }

            base.Dispose(disposing);
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

        private void OnViewOpened(object sender, ViewChangeEventArgs e)
        {
            UpdateComponentsForView(e.View as IChartView);
        }

        private void OnViewBroughtToFront(object sender, ViewChangeEventArgs e)
        {
            UpdateComponentsForView(e.View as IChartView);
        }

        private void OnViewClosed(object sender, ViewChangeEventArgs e)
        {
            if (ReferenceEquals(currentChartView, e.View))
            {
                UpdateComponentsForView(null);
            }
        }

        private void OnActiveDocumentViewChanged(object sender, EventArgs e)
        {
            UpdateComponentsForActiveDocumentView();
        }

        private void UpdateComponentsForActiveDocumentView()
        {
            UpdateComponentsForView(Gui.ViewHost.ActiveDocumentView as IChartView);
        }

        private void UpdateComponentsForView(IChartView chartView)
        {
            if (ReferenceEquals(currentChartView, chartView))
            {
                return;
            }

            currentChartView = chartView;
            chartLegendController.Update(chartView?.Chart);
        }
    }
}