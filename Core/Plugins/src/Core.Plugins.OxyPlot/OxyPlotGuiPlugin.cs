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

using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Plugin;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Plugins.OxyPlot.Commands;
using Core.Plugins.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot
{
    /// <summary>
    /// This class ties all the components together to enable charting interaction.
    /// </summary>
    public class OxyPlotGuiPlugin : PluginBase
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
            chartingRibbon = CreateRibbon(chartLegendController, Gui);

            chartLegendController.ToggleLegend();
            Gui.ActiveViewChanged += GuiOnActiveViewChanged;
            activated = true;
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<ChartDataCollection, ChartDataView>
            {
                Image = Resources.ChartIcon,
                GetViewName = (v, o) => Resources.OxyPlotGuiPlugin_GetViewInfos_Diagram
            };
        }

        public override void Dispose()
        {
            if (activated)
            {
                Gui.ActiveViewChanged -= GuiOnActiveViewChanged;
            }
            base.Dispose();
        }

        /// <summary>
        /// Creates a new <see cref="ChartLegendController"/>.
        /// </summary>
        /// <param name="toolViewController">The <see cref="IToolViewController"/> to use for the controller
        /// <see cref="ChartLegendController"/>.</param>
        /// <returns>A new <see cref="ChartLegendController"/> instance.</returns>
        private ChartLegendController CreateLegendController(IToolViewController toolViewController)
        {
            var controller = new ChartLegendController(toolViewController);
            controller.OnOpenLegend += (s,e) => UpdateComponentsForActiveView();
            return controller;
        }

        /// <summary>
        /// Creates the <see cref="ChartingRibbon"/> and the commands that will be used when clicking on the buttons.
        /// </summary>
        /// <param name="chartLegendController">The <see cref="ChartLegendController"/> to use for the 
        /// <see cref="ChartingRibbon"/>.</param>
        /// <param name="documentViewController">The controller for Document Views.</param>
        /// <returns>A new <see cref="ChartingRibbon"/> instance.</returns>
        private static ChartingRibbon CreateRibbon(ChartLegendController chartLegendController, IDocumentViewController documentViewController)
        {
            return new ChartingRibbon
            {
                ToggleLegendViewCommand = new ToggleLegendViewCommand(chartLegendController)
            };
        }

        private void GuiOnActiveViewChanged(object sender, ActiveViewChangeEventArgs activeViewChangeEventArgs)
        {
            UpdateComponentsForActiveView();
        }

        /// <summary>
        /// Updates the components which the <see cref="OxyPlotGuiPlugin"/> knows about so that it reflects
        /// the currently active view.
        /// </summary>
        private void UpdateComponentsForActiveView()
        {
            var chartView = Gui.ActiveView as IChartView;
            if (chartView != null)
            {
                chartingRibbon.Chart = chartView.Chart;
                chartLegendController.Update(chartView.Chart.Data);
            }
            else
            {
                chartingRibbon.Chart = null;
                chartLegendController.Update(null);
            }
        }
    }
}