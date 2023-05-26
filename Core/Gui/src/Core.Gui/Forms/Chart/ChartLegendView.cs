// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using Core.Gui.ContextMenu;
using Core.Gui.PresentationObjects.Chart;
using GuiResources = Core.Gui.Properties.Resources;

namespace Core.Gui.Forms.Chart
{
    /// <summary>
    /// This class defines a view which shows the data that have been added to a <see cref="IChartControl"/>.
    /// </summary>
    public sealed partial class ChartLegendView : UserControl, ISelectionProvider, IView
    {
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;
        private IChartControl chartControl;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="ChartLegendView"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> to create context menus.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contextMenuBuilderProvider"/> is <c>null</c>.</exception>
        public ChartLegendView(IContextMenuBuilderProvider contextMenuBuilderProvider)
        {
            if (contextMenuBuilderProvider == null)
            {
                throw new ArgumentNullException(nameof(contextMenuBuilderProvider),
                                                $@"Cannot create a {nameof(ChartLegendView)} when the context menu builder provider is null.");
            }

            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
            InitializeComponent();

            RegisterTreeNodeInfos();

            treeViewControl.SelectedDataChanged += TreeViewControlSelectedDataChanged;
        }

        /// <summary>
        /// Gets or sets the <see cref="IChartControl"/>.
        /// </summary>
        public IChartControl ChartControl
        {
            private get
            {
                return chartControl;
            }
            set
            {
                chartControl = value;
                Data = value?.Data;
            }
        }

        public object Selection => treeViewControl.SelectedData is ChartDataContext chartDataContext
                                       ? chartDataContext.WrappedData
                                       : treeViewControl.SelectedData;

        public object Data
        {
            get => (ChartData) treeViewControl.Data;
            set => treeViewControl.Data = (ChartData) value;
        }

        private void RegisterTreeNodeInfos()
        {
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartDataContext>
            {
                Text = context => context.WrappedData.Name,
                Image = GetImage,
                ChildNodeObjects = ChartDataContextGetChildNodeObjects,
                CanDrag = (context, o) => !(context.WrappedData is ChartMultipleAreaData),
                CanCheck = context => !(context.WrappedData is ChartDataCollection),
                CheckedState = context => context.WrappedData.IsVisible ? TreeNodeCheckedState.Checked : TreeNodeCheckedState.Unchecked,
                OnNodeChecked = ChartDataContextOnNodeChecked,
                CanDrop = ChartDataContextCanDropAndInsert,
                CanInsert = ChartDataContextCanDropAndInsert,
                OnDrop = ChartDataContextOnDrop,
                ContextMenuStrip = (nodeData, parentData, treeView) => contextMenuBuilderProvider.Get(nodeData.WrappedData, treeView)
                                                                                                 .AddCustomItem(CreateZoomToExtentsItem(nodeData.WrappedData))
                                                                                                 .AddSeparator()
                                                                                                 .AddPropertiesItem()
                                                                                                 .Build()
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartDataCollection>
            {
                Text = collection => collection.Name,
                Image = collection => GuiResources.folder,
                ExpandOnCreate = collection => true,
                ChildNodeObjects = GetCollectionChildNodeObjects,
                CanDrag = (collection, parentData) => true,
                CanDrop = ChartDataCollectionCanDropAndInsert,
                CanInsert = ChartDataCollectionCanDropAndInsert,
                OnDrop = ChartDataCollectionOnDrop,
                ContextMenuStrip = (nodeData, parentData, treeView) => contextMenuBuilderProvider.Get(nodeData, treeView)
                                                                                                 .AddCustomItem(CreateZoomToExtentsItem(nodeData))
                                                                                                 .AddSeparator()
                                                                                                 .AddPropertiesItem()
                                                                                                 .Build()
            });
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(ChartData nodeData)
        {
            bool hasData = nodeData.HasData;
            bool enabled = nodeData.IsVisible && hasData;
            string toolTip;

            if (nodeData.IsVisible)
            {
                toolTip = hasData
                              ? GuiResources.ChartLegendView_CreateZoomToExtentsItem_ZoomToAll_Tooltip
                              : GuiResources.ChartLegendView_CreateZoomToExtentsItem_NoData_ZoomToAllDisabled_Tooltip;
            }
            else
            {
                toolTip = GuiResources.ChartLegendView_CreateZoomToExtentsItem_ZoomToAllDisabled_Tooltip;
            }

            return CreateZoomToExtentsItem(nodeData, toolTip, enabled);
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(ChartData nodeData, string toolTip, bool isEnabled)
        {
            return new StrictContextMenuItem($"&{GuiResources.ZoomToAll_DisplayName}",
                                             toolTip,
                                             GuiResources.ZoomToAllIcon,
                                             (sender, args) => ChartControl?.ZoomToVisibleSeries(nodeData))
            {
                Enabled = isEnabled
            };
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(ChartDataCollection nodeData)
        {
            ChartData[] chartDataItems = nodeData.GetChartDataRecursively().ToArray();
            var isVisible = false;
            var hasData = false;
            foreach (ChartData chartData in chartDataItems)
            {
                if (chartData.IsVisible)
                {
                    isVisible = true;

                    if (chartData.HasData)
                    {
                        hasData = true;
                        break;
                    }
                }
            }

            bool enabled = isVisible && hasData;

            string toolTip;

            if (isVisible)
            {
                toolTip = hasData
                              ? GuiResources.ChartLegendView_CreateZoomToExtentsItem_ChartDataCollection_ZoomToAll_Tooltip
                              : GuiResources.ChartLegendView_CreateZoomToExtentsItem_ChartDataCollection_NoData_ZoomToAllDisabled_Tooltip;
            }
            else
            {
                toolTip = GuiResources.ChartLegendView_CreateZoomToExtentsItem_ChartDataCollection_ZoomToAllDisabled_Tooltip;
            }

            return CreateZoomToExtentsItem(nodeData, toolTip, enabled);
        }

        private static object[] GetChildNodeObjects(ChartDataCollection chartDataCollection)
        {
            return chartDataCollection.Collection.Reverse().Select(chartData => new ChartDataContext(chartData, chartDataCollection)).Cast<object>().ToArray();
        }

        private void NotifyObserversOfData(ChartData chartData)
        {
            chartData.NotifyObservers();

            var observableParent = Data as IObservable;
            observableParent?.NotifyObservers();
        }

        private void TreeViewControlSelectedDataChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        #region ChartDataContext

        private static Image GetImage(ChartDataContext context)
        {
            switch (context.WrappedData)
            {
                case ChartPointData _:
                    return GuiResources.PointsIcon;
                case ChartLineData _:
                case ChartMultipleLineData _:
                    return GuiResources.LineIcon;
                case ChartAreaData _:
                case ChartMultipleAreaData _:
                    return GuiResources.AreaIcon;
                default:
                    return GuiResources.folder;
            }
        }

        private static object[] ChartDataContextGetChildNodeObjects(ChartDataContext chartDataContext)
        {
            return chartDataContext.WrappedData is ChartDataCollection collection
                       ? GetChildNodeObjects(collection)
                       : new object[0];
        }

        private void ChartDataContextOnNodeChecked(ChartDataContext chartDataContext, object parentData)
        {
            chartDataContext.WrappedData.IsVisible = !chartDataContext.WrappedData.IsVisible;
            NotifyObserversOfData(chartDataContext.WrappedData);
        }

        private static bool ChartDataContextCanDropAndInsert(object draggedData, object targetData)
        {
            var draggedDataContext = (ChartDataContext) draggedData;
            var targetDataContext = (ChartDataContext) targetData;

            return draggedDataContext.ParentChartData.Equals(targetDataContext.WrappedData);
        }

        private static void ChartDataContextOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
        {
            var chartContext = (ChartDataContext) droppedData;
            var sourceContext = oldParentData as ChartDataContext;

            ChartData chartData = chartContext.WrappedData;
            var parent = (ChartDataCollection) (sourceContext != null ? sourceContext.WrappedData : oldParentData);

            parent.Remove(chartData);
            parent.Insert(parent.Collection.Count() - position, chartData);
            parent.NotifyObservers();
        }

        #endregion

        #region ChartDataCollection

        private static object[] GetCollectionChildNodeObjects(ChartDataCollection chartDataCollection)
        {
            return GetChildNodeObjects(chartDataCollection);
        }

        private static bool ChartDataCollectionCanDropAndInsert(object draggedData, object targetData)
        {
            var draggedDataContext = (ChartDataContext) draggedData;
            object targetDataObject = targetData is ChartDataContext targetDataContext
                                          ? targetDataContext.ParentChartData
                                          : targetData;

            return draggedDataContext.ParentChartData.Equals(targetDataObject);
        }

        private static void ChartDataCollectionOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
        {
            var chartContext = (ChartDataContext) droppedData;

            ChartData chartData = chartContext.WrappedData;
            var parent = (ChartDataCollection) oldParentData;

            parent.Remove(chartData);
            parent.Insert(parent.Collection.Count() - position, chartData);
            parent.NotifyObservers();
        }

        #endregion
    }
}