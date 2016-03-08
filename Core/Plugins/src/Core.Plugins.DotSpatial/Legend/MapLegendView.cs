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

using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Builders;
using Core.Components.DotSpatial.Forms;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Readers;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatialResources = Core.Plugins.DotSpatial.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.DotSpatial.Legend
{
    /// <summary>
    /// The view which shows the data that is added to a <see cref="MapControl"/>.
    /// </summary>
    public sealed partial class MapLegendView : UserControl, IView
    {
        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;
        private readonly IWin32Window parentWindow;
        private readonly TreeViewControl treeViewControl;

        /// <summary>
        /// Creates a new instance of <see cref="MapLegendView"/>.
        /// </summary>
        /// <param name="contextMenuBuilderProvider">The <see cref="IContextMenuBuilderProvider"/> to create context menus.</param>
        /// <param name="parentWindow"></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contextMenuBuilderProvider"/> or <paramref name="parentWindow"/> is <c>null</c>.</exception>
        public MapLegendView(IContextMenuBuilderProvider contextMenuBuilderProvider, IWin32Window parentWindow)
        {
            if (contextMenuBuilderProvider == null)
            {
                throw new ArgumentNullException("contextMenuBuilderProvider", "Cannot create a MapLegendView when the context menu builder provider is null");
            }
            if (parentWindow == null)
            {
                throw new ArgumentNullException("parentWindow" ,"Cannot create a MapLegendView when the parent window is null");
            }

            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
            this.parentWindow = parentWindow;
            InitializeComponent();
            Text = DotSpatialResources.General_Map;

            treeViewControl = new TreeViewControl
            {
                Dock = DockStyle.Fill
            };
            Controls.Add(treeViewControl);

            RegisterTreeNodeInfos();
        }

        public object Data
        {
            get
            {
                return (MapData) treeViewControl.Data;
            }
            set
            {
                if (IsDisposed)
                {
                    return;
                }

                treeViewControl.Data = (MapData) value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Data = null;

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void RegisterTreeNodeInfos()
        {
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapPointData>
            {
                Text = mapPointData => mapPointData.Name,
                Image = mapPointData => DotSpatialResources.PointsIcon,
                CanDrag = (mapPointData, parentData) => true,
                CanCheck = mapPointData => true,
                IsChecked = mapPointData => mapPointData.IsVisible,
                OnNodeChecked = PointBasedMapDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapLineData>
            {
                Text = mapLineData => mapLineData.Name,
                Image = mapLineData => DotSpatialResources.LineIcon,
                CanDrag = (mapLineData, parentData) => true,
                CanCheck = mapLineData => true,
                IsChecked = mapLineData => mapLineData.IsVisible,
                OnNodeChecked = PointBasedMapDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapPolygonData>
            {
                Text = mapPolygonData => mapPolygonData.Name,
                Image = mapPolygonData => DotSpatialResources.AreaIcon,
                CanDrag = (mapPolygonData, parentData) => true,
                CanCheck = mapPolygonData => true,
                IsChecked = mapPolygonData => mapPolygonData.IsVisible,
                OnNodeChecked = PointBasedMapDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapMultiLineData>
            {
                Text = mapMultiLineData => mapMultiLineData.Name,
                Image = mapMultiLineData => DotSpatialResources.LineIcon,
                CanDrag = (mapMultiLineData, parentData) => true,
                CanCheck = mapMultiLineData => true,
                IsChecked = mapMultiLineData => mapMultiLineData.IsVisible,
                OnNodeChecked = MapMultiLineDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapDataCollection>
            {
                Text = mapDataCollection => mapDataCollection.Name,
                Image = mapDataCollection => GuiResources.folder,
                ChildNodeObjects = mapDataCollection => mapDataCollection.List.Reverse().Cast<object>().ToArray(),
                CanDrop = MapControlCanDrop,
                CanInsert = MapControlCanInsert,
                OnDrop = MapControlOnDrop,
                ContextMenuStrip = MapDataCollectionContextMenuStrip
            });
        }

        #region MapData

        private static void MapMultiLineDataOnNodeChecked(MapMultiLineData mapMultiLineData, object parentData)
        {
            mapMultiLineData.IsVisible = !mapMultiLineData.IsVisible;
            mapMultiLineData.NotifyObservers();

            var observableParent = parentData as IObservable;
            if (observableParent != null)
            {
                observableParent.NotifyObservers();
            }
        }

        private static void PointBasedMapDataOnNodeChecked(FeatureBasedMapData featureBasedMapData, object parentData)
        {
            featureBasedMapData.IsVisible = !featureBasedMapData.IsVisible;
            featureBasedMapData.NotifyObservers();

            var observableParent = parentData as IObservable;
            if (observableParent != null)
            {
                observableParent.NotifyObservers();
            }
        }

        #endregion

        # region MapDataCollection

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
            var mapData = (MapData)droppedData;
            var target = (MapDataCollection)newParentData;

            target.List.Remove(mapData);
            target.List.Insert(target.List.Count - position, mapData); // Note: target is the same as the previous parent in this case
            target.NotifyObservers();
        }

        private ContextMenuStrip MapDataCollectionContextMenuStrip(MapDataCollection mapDataCollection, object parentData, TreeViewControl treeView)
        {
            StrictContextMenuItem addMapLayerMenuItem = new StrictContextMenuItem(
                DotSpatialResources.MapLegendView_MapDataCollectionContextMenuStrip__Add_MapLayer, 
                DotSpatialResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer_ToolTip, 
                DotSpatialResources.MapPlusIcon, 
                (sender, args) => ShowSelectShapeFileDialog(sender, args, mapDataCollection));

            return contextMenuBuilderProvider.Get(mapDataCollection, treeView).AddCustomItem(addMapLayerMenuItem).Build();
        }

        # endregion

        #region ShapeFileImporter

        private void ShowSelectShapeFileDialog(object sender, EventArgs eventArgs, MapDataCollection mapDataCollection)
        {
            var windowTitle = DotSpatialResources.MapLegendView_ShowSelectShapeFileDialog_Select_Shape_File_;
            using (var dialog = new OpenFileDialog
            {
                Filter = string.Format("{0} (*.shp)|*.shp", DotSpatialResources.MapLegendView_ShowSelectShapeFileDialog_Shape_file),
                Multiselect = false,
                Title = windowTitle,
                RestoreDirectory = true,
                CheckFileExists = false,
            })
            {
                if (dialog.ShowDialog(parentWindow) == DialogResult.OK)
                {
                    CheckDataFormat(dialog.FileName, System.IO.Path.GetFileNameWithoutExtension(dialog.FileName), mapDataCollection);
                }
            }
        }

        private void CheckDataFormat(string filePath, string title, MapDataCollection mapDataCollection)
        {
            FeatureBasedMapData importedData;

            var featureSet = Shapefile.OpenFile(filePath);

            switch (featureSet.FeatureType)
            {
                case FeatureType.Point:
                    using (ShapeFileReaderBase reader = new PointShapeFileReader(filePath))
                    {
                        importedData = GetShapeFileData(reader, filePath, title);
                    }
                    break;
                case FeatureType.Line:
                    using (ShapeFileReaderBase reader = new PolylineShapeFileReader(filePath))
                    {
                        importedData = GetShapeFileData(reader, filePath, title);
                    }
                    break;
                case FeatureType.Polygon:
                    using (ShapeFileReaderBase reader = new PolygonShapeFileReader(filePath))
                    {
                        importedData = GetShapeFileData(reader, filePath, title);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
            
            
            mapDataCollection.List.Add(importedData);
            mapDataCollection.NotifyObservers();
        }

        private FeatureBasedMapData GetShapeFileData(ShapeFileReaderBase reader, string filePath, string title)
        {
            try
            {
                return reader.ReadLine(title);
            }
            catch (ElementReadException e)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build("Het bestand bevat data, welke niet ondersteund wordt.");
                throw new CriticalFileReadException(message, e);
            }
        }

        #endregion
    }
}