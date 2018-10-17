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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
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
                var mapDataContext = (MapDataContext) treeViewControl.SelectedData;
                return mapDataContext.WrappedData;
            }
        }

        public object Data
        {
            get
            {
                return (MapDataCollectionContext) treeViewControl.Data;
            }
            set
            {
                treeViewControl.Data = value != null
                                           ? new MapDataCollectionContext((MapDataCollection) value, null)
                                           : null;
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
                ContextMenuStrip = FeatureBasedMapDataContextContextMenuStrip
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapDataCollectionContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => GuiResources.folder,
                ChildNodeObjects = GetCollectionChildNodeObjects,
                CanDrag = (context, parentData) => context.ParentMapData != null,
                CanCheck = context => true,
                IsChecked = context => context.WrappedData.IsVisible,
                OnNodeChecked = MapDataCollectionContextOnNodeChecked,
                CanDrop = MapDataCollectionCanDropAndInsert,
                CanInsert = MapDataCollectionCanDropAndInsert,
                OnDrop = MapDataCollectionOnDrop,
                ContextMenuStrip = MapDataCollectionContextMenuStrip
            });
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(MapDataContext context)
        {
            StrictContextMenuItem zoomToExtentsItem = null;

            if (context is MapDataCollectionContext)
            {
                zoomToExtentsItem = CreateZoomToExtentsItem((MapDataCollection) context.WrappedData);
            }

            if (context is FeatureBasedMapDataContext)
            {
                zoomToExtentsItem = CreateZoomToExtentsItem((FeatureBasedMapData) context.WrappedData);
            }

            return zoomToExtentsItem;
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

        #region FeatureBasedMapDataContext

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

        private static void FeatureBasedMapDataContextOnNodeChecked(FeatureBasedMapDataContext mapDataContext, object parentData)
        {
            mapDataContext.WrappedData.IsVisible = !mapDataContext.WrappedData.IsVisible;
            mapDataContext.WrappedData.NotifyObservers();
        }

        private static bool FeatureBasedMapDataContextCanDropAndInsert(object draggedData, object targetData)
        {
            var draggedDataContext = (MapDataContext) draggedData;
            var targetDataContext = (MapDataContext) targetData;

            return draggedDataContext.ParentMapData.WrappedData.Equals(targetDataContext.WrappedData);
        }

        private static void FeatureBasedMapDataContextOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
        {
            var mapContext = (MapDataContext) droppedData;
            var sourceContext = oldParentData as MapDataCollectionContext;

            MapData mapData = mapContext.WrappedData;
            var parent = (MapDataCollection) (sourceContext != null ? sourceContext.WrappedData : oldParentData);

            parent.Remove(mapData);
            parent.Insert(parent.Collection.Count() - position, mapData);
            parent.NotifyObservers();
        }

        private static bool CanRemoveMapData(FeatureBasedMapData mapData, object parent)
        {
            return mapData is IRemovable && parent is MapDataCollectionContext;
        }

        private static void RemoveFromParent(FeatureBasedMapData dataToRemove, object parentData)
        {
            if (CanRemoveMapData(dataToRemove, parentData))
            {
                var mapDataCollectionContext = (MapDataCollectionContext) parentData;
                var collection = (MapDataCollection) mapDataCollectionContext.WrappedData;
                collection.Remove(dataToRemove);
                collection.NotifyObservers();
            }
        }

        private ContextMenuStrip FeatureBasedMapDataContextContextMenuStrip(FeatureBasedMapDataContext mapDataContext, object parentData, TreeViewControl treeView)
        {
            return contextMenuBuilderProvider.Get(mapDataContext, treeView)
                                             .AddCustomItem(CreateZoomToExtentsItem(mapDataContext))
                                             .AddSeparator()
                                             .AddDeleteItem()
                                             .AddSeparator()
                                             .AddPropertiesItem()
                                             .Build();
        }

        #endregion

        #region MapDataCollectionContext

        private static object[] GetCollectionChildNodeObjects(MapDataCollectionContext context)
        {
            var childObjects = new List<object>();
            var collection = (MapDataCollection) context.WrappedData;

            foreach (MapData mapData in collection.Collection.Reverse())
            {
                var featureBasedMapData = mapData as FeatureBasedMapData;
                if (featureBasedMapData != null)
                {
                    childObjects.Add(new FeatureBasedMapDataContext(featureBasedMapData, context));
                }

                var nestedCollection = mapData as MapDataCollection;
                if (nestedCollection != null)
                {
                    childObjects.Add(new MapDataCollectionContext(nestedCollection, context));
                }
            }

            return childObjects.ToArray();
        }

        private void MapDataCollectionContextOnNodeChecked(MapDataCollectionContext context, object parentData)
        {
            var mapDataCollection = (MapDataCollection) context.WrappedData;
            mapDataCollection.IsVisible = !mapDataCollection.IsVisible;
            mapDataCollection.NotifyObservers();

            NotifyMapDataCollectionChildren(mapDataCollection);
        }

        private static void NotifyMapDataCollectionChildren(MapDataCollection collection)
        {
            foreach (MapData mapData in collection.Collection)
            {
                mapData.NotifyObservers();

                var nestedCollection = mapData as MapDataCollection;
                if (nestedCollection != null)
                {
                    NotifyMapDataCollectionChildren(nestedCollection);
                }
            }
        }

        private static bool MapDataCollectionCanDropAndInsert(object draggedData, object targetData)
        {
            var draggedDataContext = (MapDataContext) draggedData;
            var targetDataContext = (MapDataContext) targetData;
            object targetDataObject = targetDataContext.WrappedData;

            return draggedDataContext.ParentMapData.WrappedData.Equals(targetDataObject);
        }

        private static void MapDataCollectionOnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl control)
        {
            var mapDataContext = (MapDataContext) droppedData;
            MapData mapData = mapDataContext.WrappedData;

            var parentContext = (MapDataCollectionContext) oldParentData;
            var parent = (MapDataCollection) parentContext.WrappedData;

            parent.Remove(mapData);
            parent.Insert(parent.Collection.Count() - position, mapData); // Note: target is the same as the previous parent in this case
            parent.NotifyObservers();
        }

        private ContextMenuStrip MapDataCollectionContextMenuStrip(MapDataCollectionContext context, object parentData, TreeViewControl treeView)
        {
            return contextMenuBuilderProvider.Get(context, treeView)
                                             .AddCustomImportItem(
                                                 MapResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer,
                                                 MapResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer_ToolTip,
                                                 MapResources.MapPlusIcon)
                                             .AddSeparator()
                                             .AddCustomItem(CreateZoomToExtentsItem(context))
                                             .AddSeparator()
                                             .AddPropertiesItem()
                                             .Build();
        }

        #endregion
    }
}