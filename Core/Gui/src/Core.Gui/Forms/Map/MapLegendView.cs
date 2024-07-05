// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Components.Gis.Data;
using Core.Components.Gis.Data.Removable;
using Core.Components.Gis.Forms;
using Core.Components.Gis.Helpers;
using Core.Gui.ContextMenu;
using Core.Gui.Helpers;
using Core.Gui.PresentationObjects.Map;
using GuiResources = Core.Gui.Properties.Resources;

namespace Core.Gui.Forms.Map
{
    /// <summary>
    /// The view which shows the data that is added to a <see cref="MapControl"/>.
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
                                                $@"Cannot create a {nameof(MapLegendView)} when the context menu builder provider is null.");
            }

            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
            InitializeComponent();

            RegisterTreeNodeInfos();

            treeViewControl.SelectedDataChanged += TreeViewControlSelectedDataChanged;
        }

        /// <summary>
        /// Gets or sets the <see cref="IMapControl"/>.
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

        public object Selection => treeViewControl.SelectedData;

        public object Data
        {
            get => (MapDataCollectionContext) treeViewControl.Data;
            set => treeViewControl.Data = value != null
                                              ? new MapDataCollectionContext((MapDataCollection) value, null)
                                              : null;
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
                CheckedState = context => context.WrappedData.IsVisible ? TreeNodeCheckedState.Checked : TreeNodeCheckedState.Unchecked,
                OnNodeChecked = FeatureBasedMapDataContextOnNodeChecked,
                CanRemove = (context, parent) => CanRemoveMapData((FeatureBasedMapData) context.WrappedData, parent),
                OnNodeRemoved = (context, parent) => RemoveFromParent((FeatureBasedMapData) context.WrappedData, parent),
                ContextMenuStrip = FeatureBasedMapDataContextContextMenuStrip
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapDataCollectionContext>
            {
                Text = context => context.WrappedData.Name,
                Image = context => GuiResources.folder,
                ExpandOnCreate = context => true,
                ChildNodeObjects = GetCollectionChildNodeObjects,
                CanDrag = (context, parentData) => context.ParentMapData != null,
                CanCheck = context => true,
                CheckedState = MapDataCollectionContextCheckedState,
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
                              ? GuiResources.MapLegendView_CreateZoomToExtentsItem_ZoomToAll_Tooltip
                              : GuiResources.MapLegendView_CreateZoomToExtentsItem_NoFeatures_ZoomToAllDisabled_Tooltip;
            }
            else
            {
                toolTip = GuiResources.MapLegendView_CreateZoomToExtentsItem_ZoomToAllDisabled_Tooltip;
            }

            return CreateZoomToExtentsItem(nodeData, toolTip, enabled);
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(MapDataCollection nodeData)
        {
            FeatureBasedMapData[] featureBasedMapDataItems = nodeData.GetFeatureBasedMapDataRecursively().ToArray();
            var isVisible = false;
            var hasFeatures = false;
            foreach (FeatureBasedMapData mapData in featureBasedMapDataItems)
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
                              ? GuiResources.MapLegendView_CreateZoomToExtentsItem_MapDataCollection_ZoomToAll_Tooltip
                              : GuiResources.MapLegendView_CreateZoomToExtentsItem_MapDataCollection_NoFeatures_ZoomToAllDisabled_Tooltip;
            }
            else
            {
                toolTip = GuiResources.MapLegendView_CreateZoomToExtentsItem_MapDataCollection_ZoomToAllDisabled_Tooltip;
            }

            return CreateZoomToExtentsItem(nodeData, toolTip, enabled);
        }

        private StrictContextMenuItem CreateZoomToExtentsItem(MapData nodeData, string toolTip, bool isEnabled)
        {
            return new StrictContextMenuItem($"&{GuiResources.ZoomToAll_DisplayName}",
                                             toolTip,
                                             GuiResources.ZoomToAllIcon,
                                             (sender, args) => MapControl?.ZoomToVisibleLayers(nodeData))
            {
                Enabled = isEnabled
            };
        }

        private static void NotifyMapDataParents(MapDataContext context)
        {
            foreach (MapDataCollection mapDataCollection in MapDataContextHelper.GetParentsFromContext(context))
            {
                mapDataCollection.NotifyObservers();
            }
        }

        #endregion

        #region FeatureBasedMapDataContext

        private static Image GetImage(FeatureBasedMapDataContext context)
        {
            switch (context.WrappedData)
            {
                case MapPointData _:
                    return GuiResources.PointsIcon;
                case MapLineData _:
                    return GuiResources.LineIcon;
                case MapPolygonData _:
                    return GuiResources.AreaIcon;
                default:
                    return GuiResources.folder;
            }
        }

        private static void FeatureBasedMapDataContextOnNodeChecked(FeatureBasedMapDataContext mapDataContext, object parentData)
        {
            mapDataContext.WrappedData.IsVisible = !mapDataContext.WrappedData.IsVisible;
            mapDataContext.WrappedData.NotifyObservers();

            NotifyMapDataParents(mapDataContext);
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
                if (mapData is MapPointData mapPointData)
                {
                    childObjects.Add(new MapPointDataContext(mapPointData, context));
                }

                if (mapData is MapLineData mapLineData)
                {
                    childObjects.Add(new MapLineDataContext(mapLineData, context));
                }

                if (mapData is MapPolygonData mapPolygonData)
                {
                    childObjects.Add(new MapPolygonDataContext(mapPolygonData, context));
                }

                if (mapData is MapDataCollection nestedCollection)
                {
                    childObjects.Add(new MapDataCollectionContext(nestedCollection, context));
                }
            }

            return childObjects.ToArray();
        }

        private static TreeNodeCheckedState MapDataCollectionContextCheckedState(MapDataCollectionContext context)
        {
            var mapDataCollection = (MapDataCollection) context.WrappedData;

            switch (mapDataCollection.GetVisibility())
            {
                case MapDataCollectionVisibility.Visible:
                    return TreeNodeCheckedState.Checked;
                case MapDataCollectionVisibility.NotVisible:
                    return TreeNodeCheckedState.Unchecked;
                case MapDataCollectionVisibility.Mixed:
                    return TreeNodeCheckedState.Mixed;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void MapDataCollectionContextOnNodeChecked(MapDataCollectionContext context, object parentData)
        {
            var mapDataCollection = (MapDataCollection) context.WrappedData;

            Dictionary<FeatureBasedMapData, bool> childStates = mapDataCollection.GetFeatureBasedMapDataRecursively().ToDictionary(child => child, child => child.IsVisible);
            Dictionary<MapDataCollection, MapDataCollectionVisibility> nestedCollectionVisibilityStates = MapDataCollectionHelper.GetNestedCollectionVisibilityStates(mapDataCollection);

            MapDataCollectionVisibility collectionOriginalVisibility = mapDataCollection.GetVisibility();

            mapDataCollection.IsVisible = collectionOriginalVisibility == MapDataCollectionVisibility.Mixed
                                          || collectionOriginalVisibility == MapDataCollectionVisibility.NotVisible;
            mapDataCollection.NotifyObservers();

            NotifyMapDataCollectionChildren(childStates);
            NotifyNestedMapDataCollections(nestedCollectionVisibilityStates);
            NotifyMapDataParents(context);
        }

        private static void NotifyNestedMapDataCollections(Dictionary<MapDataCollection, MapDataCollectionVisibility> childStates)
        {
            foreach (KeyValuePair<MapDataCollection, MapDataCollectionVisibility> child in childStates.Where(child => child.Key.GetVisibility() != child.Value))
            {
                child.Key.NotifyObservers();
            }
        }

        private static void NotifyMapDataCollectionChildren(Dictionary<FeatureBasedMapData, bool> childStates)
        {
            foreach (KeyValuePair<FeatureBasedMapData, bool> child in childStates.Where(child => child.Key.IsVisible != child.Value))
            {
                child.Key.NotifyObservers();
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
                                             .AddImportItem(
                                                 GuiResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer,
                                                 GuiResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer_ToolTip,
                                                 GuiResources.MapPlusIcon)
                                             .AddSeparator()
                                             .AddCustomItem(CreateZoomToExtentsItem(context))
                                             .AddSeparator()
                                             .AddPropertiesItem()
                                             .Build();
        }

        #endregion
    }
}