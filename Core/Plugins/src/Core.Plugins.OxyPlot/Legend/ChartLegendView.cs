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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;
using OxyPlotResources = Core.Plugins.OxyPlot.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class defines a view which shows the data that have been added to a <see cref="ChartControl"/>.
    /// </summary>
    public sealed partial class ChartLegendView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartLegendView"/>.
        /// </summary>
        public ChartLegendView()
        {
            InitializeComponent();
            Text = OxyPlotResources.General_Chart;

            RegisterTreeNodeInfos();
        }

        public object Data
        {
            get
            {
                return (ChartData) treeViewControl.Data;
            }
            set
            {
                treeViewControl.Data = (ChartData) value;
            }
        }

        private void RegisterTreeNodeInfos()
        {
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartPointData>
            {
                Text = pointData => pointData.Name,
                Image = pointData => OxyPlotResources.PointsIcon,
                CanDrag = (pointData, parentData) => true,
                CanCheck = pointData => true,
                IsChecked = pointData => pointData.IsVisible,
                OnNodeChecked = PointBasedChartDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartLineData>
            {
                Text = lineData => lineData.Name,
                Image = lineData => OxyPlotResources.LineIcon,
                CanDrag = (lineData, parentData) => true,
                CanCheck = lineData => true,
                IsChecked = lineData => lineData.IsVisible,
                OnNodeChecked = PointBasedChartDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartAreaData>
            {
                Text = areaData => areaData.Name,
                Image = areaData => OxyPlotResources.AreaIcon,
                CanDrag = (areaData, parentData) => true,
                CanCheck = areaData => true,
                IsChecked = areaData => areaData.IsVisible,
                OnNodeChecked = PointBasedChartDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartMultipleAreaData>
            {
                Text = multipleAreaData => multipleAreaData.Name,
                Image = multipleAreaData => OxyPlotResources.AreaIcon,
                CanDrag = (multipleAreaData, parentData) => true,
                CanCheck = multipleAreaData => true,
                IsChecked = multipleAreaData => multipleAreaData.IsVisible,
                OnNodeChecked = ChartMultipleAreaDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartDataCollection>
            {
                Text = collection => collection.Name,
                Image = collection => GuiResources.folder,
                ChildNodeObjects = collection => collection.List.Reverse().Cast<object>().ToArray(),
                CanDrag = (multipleAreaData, parentData) => true,
                CanDrop = ChartControlCanDrop,
                CanInsert = ChartControlCanInsert,
                OnDrop = ChartControlOnDrop
            });
        }

        # region ChartData

        private void PointBasedChartDataOnNodeChecked(PointBasedChartData pointBasedChartData, object parentData)
        {
            pointBasedChartData.IsVisible = !pointBasedChartData.IsVisible;
            NotifyObserversOfData(pointBasedChartData);
        }

        private void ChartMultipleAreaDataOnNodeChecked(ChartMultipleAreaData chartMultipleAreaData, object parentData)
        {
            chartMultipleAreaData.IsVisible = !chartMultipleAreaData.IsVisible;
            NotifyObserversOfData(chartMultipleAreaData);
        }

        private void NotifyObserversOfData(ChartData chartData)
        {
            chartData.NotifyObservers();

            var observableParent = Data as IObservable;
            if (observableParent != null)
            {
                observableParent.NotifyObservers();
            }
        }

        # endregion

        # region ChartDataCollection

        private static bool ChartControlCanDrop(object draggedData, object targetData)
        {
            return draggedData is ChartData;
        }

        private static bool ChartControlCanInsert(object draggedData, object targetData)
        {
            return draggedData is ChartData;
        }

        private static void ChartControlOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
        {
            var chartData = (ChartData) droppedData;
            var source = (ChartDataCollection)oldParentData;
            var target = (ChartDataCollection) newParentData;

            source.List.Remove(chartData);
            target.List.Insert(target.List.Count - position, chartData);
            target.NotifyObservers();
        }

        # endregion
    }
}