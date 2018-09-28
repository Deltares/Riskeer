// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Removable;
using Core.Components.Gis.Forms;
using MapResources = Core.Plugins.Map.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.Map.Legend
{
    /// <summary>
    /// The view which shows the data that is added to a <see cref="Components.DotSpatial.Forms.MapControl"/>.
    /// </summary>
    public sealed partial class MapLegendView : UserControl, ISelectionProvider, IView
    {
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;

        private IMapControl mapControl;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="MapLegendView"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> to create context menus.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contextMenuBuilderProvider"/> is <c>null</c>.</exception>
        public MapLegendView(IContextMenuBuilderProvider contextMenuBuilderProvider)
        {
            if (contextMenuBuilderProvider == null)
            {
                throw new ArgumentNullException(nameof(contextMenuBuilderProvider),
                                                $@"Cannot create a {typeof(MapLegendView).Name} when the context menu builder provider is null.");
            }

            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
            InitializeComponent();
            Text = MapResources.General_Map;

            RegisterTreeNodeInfos();

            treeViewControl.SelectedDataChanged += TreeViewControlSelectedDataChanged;
        }

        /// <summary>
        /// Gets or sets the <see cref="IMapControl"/> for which this legend view is configured
        /// to show the internal map data for.
        /// </summary>
        public IMapControl MapControl
        {
            private get
            {
                return mapControl;
            }
            set
            {
                mapControl = value;
                Data = value?.Data;
            }
        }

        public object Selection
        {
            get
            {
                return treeViewControl.SelectedData;
            }
        }

        public object Data
        {
            get
            {
                return (MapData) treeViewControl.Data;
            }
            set
            {
                treeViewControl.Data = (MapData) value;
            }
        }

        private void TreeViewControlSelectedDataChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, new EventArgs());
        }

        private void RegisterTreeNodeInfos()
        {
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapPointData>
            {
                Text = mapPointData => mapPointData.Name,
                Image = mapPointData => MapResources.PointsIcon,
                ContextMenuStrip = (nodeData, parentData, treeView) => CreateFeatureBasedMapDataContextMenu(nodeData, treeView),
                CanRemove = CanRemoveMapData,
                OnNodeRemoved = RemoveFromParent,
                CanDrag = (mapPointData, parentData) => true,
                CanCheck = mapPointData => true,
                IsChecked = mapPointData => mapPointData.IsVisible,
                OnNodeChecked = MapDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapLineData>
            {
                Text = mapLineData => mapLineData.Name,
                Image = mapLineData => MapResources.LineIcon,
                ContextMenuStrip = (nodeData, parentData, treeView) => CreateFeatureBasedMapDataContextMenu(nodeData, treeView),
                CanRemove = CanRemoveMapData,
                OnNodeRemoved = RemoveFromParent,
                CanDrag = (mapLineData, parentData) => true,
                CanCheck = mapLineData => true,
                IsChecked = mapLineData => mapLineData.IsVisible,
                OnNodeChecked = MapDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapPolygonData>
            {
                Text = mapPolygonData => mapPolygonData.Name,
                Image = mapPolygonData => MapResources.AreaIcon,
                ContextMenuStrip = (nodeData, parentData, treeView) => CreateFeatureBasedMapDataContextMenu(nodeData, treeView),
                CanRemove = CanRemoveMapData,
                OnNodeRemoved = RemoveFromParent,
                CanDrag = (mapPolygonData, parentData) => true,
                CanCheck = mapPolygonData => true,
                IsChecked = mapPolygonData => mapPolygonData.IsVisible,
                OnNodeChecked = MapDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapDataCollection>
            {
                Text = mapDataCollection => mapDataCollection.Name,
                Image = mapDataCollection => GuiResources.folder,
                ChildNodeObjects = mapDataCollection => mapDataCollection.Collection.Reverse().Cast<object>().ToArray(),
                CanDrop = MapControlCanDrop,
                CanInsert = MapControlCanInsert,
                OnDrop = MapControlOnDrop,
                ContextMenuStrip = (mapDataCollection, parentData, treeView) => contextMenuBuilderProvider.Get(mapDataCollection, treeView)
                                                                                                          .AddCustomImportItem(
                                                                                                              MapResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer,
                                                                                                              MapResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer_ToolTip,
                                                                                                              MapResources.MapPlusIcon)
                                                                                                          .AddSeparator()
                                                                                                          .AddCustomItem(CreateZoomToExtentsItem(mapDataCollection))
                                                                                                          .AddSeparator()
                                                                                                          .AddPropertiesItem()
                                                                                                          .Build()
            });
        }

        private ContextMenuStrip CreateFeatureBasedMapDataContextMenu(FeatureBasedMapData nodeData, TreeViewControl treeView)
        {
            return contextMenuBuilderProvider.Get(nodeData, treeView)
                                             .AddCustomItem(CreateZoomToExtentsItem(nodeData))
                                             .AddSeparator()
                                             .AddDeleteItem()
                                             .AddSeparator()
                                             .AddPropertiesItem()
                                             .Build();
        }

        private static bool CanRemoveMapData(MapData mapData, object parent)
        {
            return mapData is IRemovable && parent is MapDataCollection;
        }

        private static void RemoveFromParent(MapData dataToRemove, object parentData)
        {
            if (CanRemoveMapData(dataToRemove, parentData))
            {
                var collection = (MapDataCollection) parentData;
                collection.Remove(dataToRemove);
                collection.NotifyObservers();
            }
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(MapData nodeData, string toolTip, bool isEnabled)
        {
            return new StrictContextMenuItem($"&{MapResources.Ribbon_ZoomToAll}",
                                             toolTip,
                                             MapResources.ZoomToAllIcon,
                                             (sender, args) => MapControl?.ZoomToAllVisibleLayers(nodeData))
            {
                Enabled = isEnabled
            };
        }

        #region MapData

        private static void MapDataOnNodeChecked(FeatureBasedMapData featureBasedMapData, object parentData)
        {
            featureBasedMapData.IsVisible = !featureBasedMapData.IsVisible;
            featureBasedMapData.NotifyObservers();
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(FeatureBasedMapData nodeData)
        {
            bool hasFeatures = nodeData.Features.Any();
            bool enabled = nodeData.IsVisible && hasFeatures;
            string toolTip;

            if (nodeData.IsVisible)
            {
                toolTip = hasFeatures
                              ? MapResources.MapLegendView_CreateZoomToExtentsItem_ZoomToAll_Tooltip
                              : MapResources.MapLegendView_CreateZoomToExtentsItem_NoFeatures_ZoomToAllDisabled_Tooltip;
            }
            else
            {
                toolTip = MapResources.MapLegendView_CreateZoomToExtentsItem_ZoomToAllDisabled_Tooltip;
            }

            return CreateZoomToExtentsItem(nodeData, toolTip, enabled);
        }

        #endregion

        #region MapDataCollection

        private static bool MapControlCanDrop(object draggedData, object targetData)
        {
            return draggedData is MapData;
        }

        private static bool MapControlCanInsert(object draggedData, object targetData)
        {
            return draggedData is MapData;
        }

        private static void MapControlOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
        {
            var mapData = (MapData) droppedData;
            var target = (MapDataCollection) newParentData;

            target.Remove(mapData);
            target.Insert(target.Collection.Count() - position, mapData); // Note: target is the same as the previous parent in this case
            target.NotifyObservers();
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(MapDataCollection nodeData)
        {
            FeatureBasedMapData[] featureBasedMapDatas = nodeData.GetFeatureBasedMapDataRecursively().ToArray();
            var isVisible = false;
            var hasFeatures = false;
            foreach (FeatureBasedMapData mapData in featureBasedMapDatas)
            {
                if (mapData.IsVisible)
                {
                    isVisible = true;

                    if (mapData.Features.Any())
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
                              ? MapResources.MapLegendView_CreateZoomToExtentsItem_MapDataCollection_ZoomToAll_Tooltip
                              : MapResources.MapLegendView_CreateZoomToExtentsItem_MapDataCollection_NoFeatures_ZoomToAllDisabled_Tooltip;
            }
            else
            {
                toolTip = MapResources.MapLegendView_CreateZoomToExtentsItem_MapDataCollection_ZoomToAllDisabled_Tooltip;
            }

            return CreateZoomToExtentsItem(nodeData, toolTip, enabled);
        }

        #endregion
    }
}