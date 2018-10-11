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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Removable;
using Core.Components.Gis.Forms;
using Core.Plugins.Map.PresentationObjects;
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
                var mapDataContext = treeViewControl.SelectedData as MapDataContext;
                if (mapDataContext != null)
                {
                    return mapDataContext.WrappedData;
                }

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
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<FeatureBasedMapDataContext>
            {
                Text = context => context.WrappedData.Name,
                Image = GetImage,
                CanDrag = (context, parent) => true,
                CanCheck = context => true,
                IsChecked = context => context.WrappedData.IsVisible,
                OnNodeChecked = FeatureBasedMapDataContextOnNodeChecked,
                CanDrop = FeatureBasedMapDataContextCanDropAndInsert,
                CanInsert = FeatureBasedMapDataContextCanDropAndInsert,
                OnDrop = FeatureBasedMapDataContextOnDrop,
                CanRemove = (context, parent) => CanRemoveMapData((FeatureBasedMapData) context.WrappedData, parent),
                OnNodeRemoved = (context, parent) => RemoveFromParent((FeatureBasedMapData) context.WrappedData, parent),
                ContextMenuStrip = MapDataContextContextMenuStrip
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapDataCollection>
            {
                Text = mapDataCollection => mapDataCollection.Name,
                Image = mapDataCollection => GuiResources.folder,
                ChildNodeObjects = GetCollectionChildNodeObjects,
                CanDrag = (collection, parentData) => true,
                CanDrop = MapDataCollectionCanDropAndInsert,
                CanInsert = MapDataCollectionCanDropAndInsert,
                OnDrop = MapDataCollectionOnDrop,
                ContextMenuStrip = MapDataCollectionContextMenuStrip
            });
        }

        private static object[] GetChildNodeObjects(MapDataCollection mapDataCollection)
        {
            return mapDataCollection.Collection.Reverse()
                                    .Select(mapData => new MapDataContext(mapData, mapDataCollection))
                                    .Cast<object>().ToArray();
        }

        private void NotifyObserversOfData(MapData mapData)
        {
            mapData.NotifyObservers();

            var observableParent = Data as IObservable;
            observableParent?.NotifyObservers();
        }

        #region MapData

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

        #endregion

        #region MapDataContext

        private static Image GetImage(FeatureBasedMapDataContext context)
        {
            if (context.WrappedData is MapPointData)
            {
                return MapResources.PointsIcon;
            }

            if (context.WrappedData is MapLineData)
            {
                return MapResources.LineIcon;
            }

            if (context.WrappedData is MapPolygonData)
            {
                return MapResources.AreaIcon;
            }

            return GuiResources.folder;
        }

        private void FeatureBasedMapDataContextOnNodeChecked(FeatureBasedMapDataContext mapDataContext, object parentData)
        {
            mapDataContext.WrappedData.IsVisible = !mapDataContext.WrappedData.IsVisible;
            NotifyObserversOfData(mapDataContext.WrappedData);
        }

        private static bool FeatureBasedMapDataContextCanDropAndInsert(object draggedData, object targetData)
        {
            var draggedDataContext = (FeatureBasedMapDataContext) draggedData;
            var targetDataContext = (MapDataContext) targetData;

            return draggedDataContext.ParentMapData.Equals(targetDataContext.WrappedData);
        }

        private static void FeatureBasedMapDataContextOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
        {
            var mapContext = (FeatureBasedMapDataContext) droppedData;
            var sourceContext = oldParentData as MapDataCollectionContext;

            var mapData = (FeatureBasedMapData) mapContext.WrappedData;
            var parent = (MapDataCollection) (sourceContext != null ? sourceContext.WrappedData : oldParentData);

            parent.Remove(mapData);
            parent.Insert(parent.Collection.Count() - position, mapData);
            parent.NotifyObservers();
        }

        private static bool CanRemoveMapData(FeatureBasedMapData mapData, object parent)
        {
            return mapData is IRemovable && parent is MapDataCollection;
        }

        private static void RemoveFromParent(FeatureBasedMapData dataToRemove, object parentData)
        {
            if (CanRemoveMapData(dataToRemove, parentData))
            {
                var collection = (MapDataCollection) parentData;
                collection.Remove(dataToRemove);
                collection.NotifyObservers();
            }
        }

        private ContextMenuStrip MapDataContextContextMenuStrip(FeatureBasedMapDataContext mapDataContext, object parentData, TreeViewControl treeView)
        {
            return contextMenuBuilderProvider.Get(mapDataContext.WrappedData, treeView)
                                             .AddCustomItem(CreateZoomToExtentsItem((FeatureBasedMapData) mapDataContext.WrappedData))
                                             .AddSeparator()
                                             .AddDeleteItem()
                                             .AddSeparator()
                                             .AddPropertiesItem()
                                             .Build();
        }

        #endregion

        #region MapDataCollection

        private static object[] GetCollectionChildNodeObjects(MapDataCollection mapDataCollection)
        {
            return GetChildNodeObjects(mapDataCollection);
        }

        private static bool MapDataCollectionCanDropAndInsert(object draggedData, object targetData)
        {
            var draggedDataContext = (MapDataContext) draggedData;
            var targetDataContext = targetData as MapDataContext;
            object targetDataObject = targetDataContext != null ? targetDataContext.ParentMapData : targetData;

            return draggedDataContext.ParentMapData.Equals(targetDataObject);
        }

        private static void MapDataCollectionOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
        {
            var mapDataContext = (MapDataContext) droppedData;

            MapData mapData = mapDataContext.WrappedData;
            var parent = (MapDataCollection) oldParentData;

            parent.Remove(mapData);
            parent.Insert(parent.Collection.Count() - position, mapData); // Note: target is the same as the previous parent in this case
            parent.NotifyObservers();
        }

        private ContextMenuStrip MapDataCollectionContextMenuStrip(MapDataCollection mapDataCollection, object parentData, TreeViewControl treeView)
        {
            return contextMenuBuilderProvider.Get(mapDataCollection, treeView)
                                             .AddCustomImportItem(
                                                 MapResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer,
                                                 MapResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer_ToolTip,
                                                 MapResources.MapPlusIcon)
                                             .AddSeparator()
                                             .AddCustomItem(CreateZoomToExtentsItem(mapDataCollection))
                                             .AddSeparator()
                                             .AddPropertiesItem()
                                             .Build();
        }

        #endregion
    }
}