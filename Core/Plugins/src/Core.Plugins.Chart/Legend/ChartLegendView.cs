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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Plugins.Chart.PresentationObjects;
using ChartResources = Core.Plugins.Chart.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.Chart.Legend
{
    /// <summary>
    /// This class defines a view which shows the data that have been added to a <see cref="IChartControl"/>.
    /// </summary>
    public sealed partial class ChartLegendView : UserControl, IView
    {
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;
        private IChartControl chartControl;

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
                                                $@"Cannot create a {typeof(ChartLegendView).Name} when the context menu builder provider is null.");
            }

            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
            InitializeComponent();
            Text = ChartResources.General_Chart;

            RegisterTreeNodeInfos();
        }

        public IChartControl ChartControl
        {
            get
            {
                return chartControl;
            }
            set
            {
                chartControl = value;
                Data = value?.Data;
            }
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
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartDataContext>
            {
                Text = context => context.WrappedData.Name,
                Image = GetImage,
                ChildNodeObjects = ChartDataContextGetChildNodeObjects,
                CanDrag = (context, o) => !(context.WrappedData is ChartMultipleAreaData),
                CanCheck = context => !(context.WrappedData is ChartDataCollection),
                IsChecked = context => context.WrappedData.IsVisible,
                OnNodeChecked = ChartDataContextOnNodeChecked,
                CanDrop = ChartDataContextCanDropAndInsert,
                CanInsert = ChartDataContextCanDropAndInsert,
                OnDrop = ChartDataContextOnDrop,
                ContextMenuStrip = (nodeData, parentData, treeView) => contextMenuBuilderProvider.Get(nodeData, treeView)
                                                                                                 .AddCustomItem(CreateZoomToExtentsItem(nodeData.WrappedData))
                                                                                                 .Build()
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<ChartDataCollection>
            {
                Text = collection => collection.Name,
                Image = collection => GuiResources.folder,
                ChildNodeObjects = GetCollectionChildNodeObjects,
                CanDrag = (multipleAreaData, parentData) => true,
                CanDrop = ChartDataCollectionCanDropAndInsert,
                CanInsert = ChartDataCollectionCanDropAndInsert,
                OnDrop = ChartDataCollectionOnDrop,
                ContextMenuStrip = (nodeData, parentData, treeView) => contextMenuBuilderProvider.Get(nodeData, treeView)
                                                                                                 .AddCustomItem(CreateZoomToExtentsItem(nodeData))
                                                                                                 .Build()
            });
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(ChartData nodeData)
        {
            bool hasFeatures = nodeData.HasData;
            bool enabled = nodeData.IsVisible && hasFeatures;
            string toolTip;

            if (nodeData.IsVisible)
            {
                toolTip = hasFeatures
                              ? ChartResources.ChartLegendView_CreateZoomToExtentsItem_ZoomToAll_Tooltip
                              : ChartResources.ChartLegendView_CreateZoomToExtentsItem_NoFeatures_ZoomToAllDisabled_Tooltip;
            }
            else
            {
                toolTip = ChartResources.ChartLegendView_CreateZoomToExtentsItem_ZoomToAllDisabled_Tooltip;
            }

            return CreateZoomToExtentsItem(nodeData, toolTip, enabled);
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(ChartData nodeData, string toolTip, bool isEnabled)
        {
            return new StrictContextMenuItem($"&{ChartResources.Ribbon_ZoomToAll}",
                                             toolTip,
                                             ChartResources.ZoomToAllIcon,
                                             (sender, args) =>
                                             {
                                                 ChartControl?.ZoomToAllVisibleLayers(nodeData);
                                             })
            {
                Enabled = isEnabled
            };
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(ChartDataCollection nodeData)
        {
            ChartData[] chartDatas = GetChartDataRecursively(nodeData).ToArray();
            var isVisible = false;
            var hasFeatures = false;
            foreach (ChartData chartData in chartDatas)
            {

                if (chartData.IsVisible)
                {
                    isVisible = true;

                    if (chartData.HasData)
                    {
                        hasFeatures = true;
                        break;
                    }
                }
            }

            bool enabled = isVisible && hasFeatures;

            string toolTip;

            if (isVisible)
            {
                toolTip = hasFeatures
                              ? ChartResources.ChartLegendView_CreateZoomToExtentsItem_ChartDataCollection_ZoomToAll_Tooltip
                              : ChartResources.ChartLegendView_CreateZoomToExtentsItem_ChartDataCollection_NoData_ZoomToAllDisabled_Tooltip;
            }
            else
            {
                toolTip = ChartResources.ChartLegendView_CreateZoomToExtentsItem_ChartDataCollection_ZoomToAllDisabled_Tooltip;
            }

            return CreateZoomToExtentsItem(nodeData, toolTip, enabled);
        }


        /// <summary>
        /// Gets all the <see cref="ChartData"/> recursively in the given <paramref name="chartDataCollection"/>.
        /// </summary>
        /// <param name="chartDataCollection">The collection to get all <see cref="ChartData"/> from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ChartData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="chartDataCollection"/> is <c>null</c>.</exception>
        public static IEnumerable<ChartData> GetChartDataRecursively(ChartDataCollection chartDataCollection)
        {
            if (chartDataCollection == null)
            {
                throw new ArgumentNullException(nameof(chartDataCollection));
            }

            var chartDataList = new List<ChartData>();

            foreach (ChartData chartData in chartDataCollection.Collection)
            {
                var nestedChartDataCollection = chartData as ChartDataCollection;
                if (nestedChartDataCollection != null)
                {
                    chartDataList.AddRange(GetChartDataRecursively(nestedChartDataCollection));
                    continue;
                }

                chartDataList.Add(chartData);
            }

            return chartDataList;
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

        #region ChartDataContext

        private Image GetImage(ChartDataContext context)
        {
            if (context.WrappedData is ChartPointData)
            {
                return ChartResources.PointsIcon;
            }

            if (context.WrappedData is ChartLineData || context.WrappedData is ChartMultipleLineData)
            {
                return ChartResources.LineIcon;
            }

            if (context.WrappedData is ChartAreaData || context.WrappedData is ChartMultipleAreaData)
            {
                return ChartResources.AreaIcon;
            }

            return GuiResources.folder;
        }

        private object[] ChartDataContextGetChildNodeObjects(ChartDataContext chartDataContext)
        {
            var collection = chartDataContext.WrappedData as ChartDataCollection;
            return collection != null ? GetChildNodeObjects(collection) : new object[0];
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

        private void ChartDataContextOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
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
            var targetDataContext = targetData as ChartDataContext;
            object targetDataObject = targetDataContext != null ? targetDataContext.ParentChartData : targetData;

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