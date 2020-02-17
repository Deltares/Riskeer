// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Chart.Data;
using Core.Components.Gis.Data;
using Core.Components.PointedTree.Data;
using Core.Components.Stack.Data;
using Demo.Riskeer.Properties;
using Demo.Riskeer.Ribbons;
using Demo.Riskeer.Views;
using Riskeer.Common.Data.AssessmentSection;
using ChartResources = Core.Plugins.Chart.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Demo.Riskeer.GUIs
{
    /// <summary>
    /// UI plug-in that provides access to the demo projects for Riskeer.
    /// </summary>
    public class DemoProjectPlugin : PluginBase
    {
        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new RiskeerDemoProjectRibbon(Gui, Gui.ViewCommands);
            }
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<ChartDataCollection, ChartDataView>
            {
                Image = ChartResources.ChartIcon,
                GetViewName = (v, o) => ChartResources.OxyPlotPlugin_GetViewInfos_Diagram
            };

            yield return new ViewInfo<MapData, MapDataView>
            {
                Image = CoreCommonGuiResources.DocumentHS,
                GetViewName = (v, o) => CoreCommonGuiResources.DotSpatialPlugin_GetViewInfoObjects_Map
            };

            yield return new ViewInfo<StackChartData, StackChartDataView>
            {
                Image = CoreCommonGuiResources.DocumentHS,
                GetViewName = (v, o) => ChartResources.OxyPlotPlugin_GetViewInfos_Diagram
            };

            yield return new ViewInfo<GraphNode, PointedTreeGraphView>
            {
                Image = Resources.FaultTreeIcon,
                GetViewName = (v, o) => Resources.General_FaultTree
            };

            yield return new ViewInfo<BackgroundData, AssemblyView>
            {
                Image = Resources.AssemblyResultTotal,
                GetViewName = (v, o) => Resources.General_Assembly,
                CreateInstance = data => new AssemblyView(data, new DialogBasedInquiryHelper(Gui.MainWindow))
            };
        }
    }
}