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

using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Properties;

using OxyPlotResources = Core.Plugins.OxyPlot.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class defines a view which shows the data that have been added to a <see cref="BaseChart"/>.
    /// </summary>
    public sealed partial class LegendView : UserControl, IView
    {
        /// <summary>
        /// Creates a new instance of <see cref="LegendView"/>.
        /// </summary>
        public LegendView()
        {
            InitializeComponent();
            Text = Resources.General_Chart;

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<PointData>
            {
                Text = pointData => OxyPlotResources.ChartData_Point_data_label,
                Image = pointData => OxyPlotResources.PointsIcon,
                CanDrag = (pointData, sourceNode) => DragOperations.Move,
                CanCheck = pointData => true,
                IsChecked = pointData => pointData.IsVisible,
                OnNodeChecked = PointDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<LineData>
            {
                Text = lineData => OxyPlotResources.ChartData_Line_data_label,
                Image = lineData => OxyPlotResources.LineIcon,
                CanDrag = (lineData, sourceNode) => DragOperations.Move,
                CanCheck = lineData => true,
                IsChecked = lineData => lineData.IsVisible,
                OnNodeChecked = LineDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<AreaData>
            {
                Text = areaData => OxyPlotResources.ChartData_Area_data_label,
                Image = areaData => OxyPlotResources.AreaIcon,
                CanDrag = (areaData, sourceNode) => DragOperations.Move,
                CanCheck = areaData => true,
                IsChecked = areaData => areaData.IsVisible,
                OnNodeChecked = AreaDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartDataCollection>
            {
                Text = baseChart => OxyPlotResources.General_Chart,
                Image = baseChart => GuiResources.folder,
                ChildNodeObjects = baseChart => baseChart.List.Reverse().Cast<object>().ToArray(),
                CanDrop = BaseChartCanDrop,
                CanInsert = BaseChartCanInsert,
                OnDrop = BaseChartOnDrop
            });
        }

        public object Data
        {
            get
            {
                return (ChartData) treeViewControl.Data;
            }
            set
            {
                if (IsDisposed)
                {
                    return;
                }

                treeViewControl.Data = (ChartData) value;
            }
        }

        # region ChartData

        private void PointDataOnNodeChecked(PointData pointData, object parentData)
        {
            PointBasedChartDataOnNodeChecked(pointData, parentData);
        }

        private void LineDataOnNodeChecked(LineData lineData, object parentData)
        {
            PointBasedChartDataOnNodeChecked(lineData, parentData);
        }

        private void AreaDataOnNodeChecked(AreaData areaData, object parentData)
        {
            PointBasedChartDataOnNodeChecked(areaData, parentData);
        }

        private void PointBasedChartDataOnNodeChecked(PointBasedChartData pointBasedChartData, object parentData)
        {
            pointBasedChartData.IsVisible = !pointBasedChartData.IsVisible;
            pointBasedChartData.NotifyObservers();

            var observableParent = parentData as IObservable;
            if (observableParent != null)
            {
                observableParent.NotifyObservers();
            }
        }

        # endregion

        # region ChartDataCollection

        private DragOperations BaseChartCanDrop(TreeNode sourceNode, TreeNode targetNode, DragOperations validOperations)
        {
            if (sourceNode.Tag is ChartData)
            {
                return validOperations;
            }

            return DragOperations.None;
        }

        private bool BaseChartCanInsert(TreeNode sourceNode, TreeNode targetNode)
        {
            return sourceNode.Tag is ChartData;
        }

        private void BaseChartOnDrop(TreeNode sourceNode, TreeNode previousParentNode, DragOperations operation, int position)
        {
            var draggedData = (ChartData)sourceNode.Tag;
            var target = (ChartDataCollection)previousParentNode.Tag;

            target.List.Remove(draggedData);
            target.List.Insert(target.List.Count - position, draggedData); // Note: target is the same as the previous parent in this case
            target.NotifyObservers();
        }

        # endregion
    }
}