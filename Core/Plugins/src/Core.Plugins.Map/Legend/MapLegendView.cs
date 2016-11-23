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
using System.IO;
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
using log4net;
using MapResources = Core.Plugins.Map.Properties.Resources;
using GuiResources = Core.Common.Gui.Properties.Resources;
using ILog = log4net.ILog;

namespace Core.Plugins.Map.Legend
{
    /// <summary>
    /// The view which shows the data that is added to a <see cref="MapControl"/>.
    /// </summary>
    public sealed partial class MapLegendView : UserControl, ISelectionProvider
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MapLegendView));

        private readonly IContextMenuBuilderProvider contextMenuBuilderProvider;
        private readonly IWin32Window parentWindow;

        public event EventHandler<EventArgs> SelectionChanged;

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
                throw new ArgumentNullException("contextMenuBuilderProvider", @"Cannot create a MapLegendView when the context menu builder provider is null.");
            }
            if (parentWindow == null)
            {
                throw new ArgumentNullException("parentWindow", @"Cannot create a MapLegendView when the parent window is null.");
            }

            this.contextMenuBuilderProvider = contextMenuBuilderProvider;
            this.parentWindow = parentWindow;
            InitializeComponent();
            Text = MapResources.General_Map;

            RegisterTreeNodeInfos();

            treeViewControl.SelectedDataChanged += TreeViewControlSelectedDataChanged;
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

        public object Selection
        {
            get
            {
                return treeViewControl.SelectedData;
            }
        }

        private void TreeViewControlSelectedDataChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, new EventArgs());
            }
        }

        private void RegisterTreeNodeInfos()
        {
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapPointData>
            {
                Text = mapPointData => mapPointData.Name,
                Image = mapPointData => MapResources.PointsIcon,
                ContextMenuStrip = (nodeData, parentData, treeView) => contextMenuBuilderProvider.Get(nodeData, treeView)
                                                                                                 .AddPropertiesItem()
                                                                                                 .Build(),
                CanDrag = (mapPointData, parentData) => true,
                CanCheck = mapPointData => true,
                IsChecked = mapPointData => mapPointData.IsVisible,
                OnNodeChecked = MapDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapLineData>
            {
                Text = mapLineData => mapLineData.Name,
                Image = mapLineData => MapResources.LineIcon,
                ContextMenuStrip = (nodeData, parentData, treeView) => contextMenuBuilderProvider.Get(nodeData, treeView)
                                                                                                 .AddPropertiesItem()
                                                                                                 .Build(),
                CanDrag = (mapLineData, parentData) => true,
                CanCheck = mapLineData => true,
                IsChecked = mapLineData => mapLineData.IsVisible,
                OnNodeChecked = MapDataOnNodeChecked
            });

            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo<MapPolygonData>
            {
                Text = mapPolygonData => mapPolygonData.Name,
                Image = mapPolygonData => MapResources.AreaIcon,
                ContextMenuStrip = (nodeData, parentData, treeView) => contextMenuBuilderProvider.Get(nodeData, treeView)
                                                                                                 .AddPropertiesItem()
                                                                                                 .Build(),
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
                ContextMenuStrip = MapDataCollectionContextMenuStrip
            });
        }

        #region MapData

        private static void MapDataOnNodeChecked(FeatureBasedMapData featureBasedMapData, object parentData)
        {
            featureBasedMapData.IsVisible = !featureBasedMapData.IsVisible;
            featureBasedMapData.NotifyObservers();
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

        private ContextMenuStrip MapDataCollectionContextMenuStrip(MapDataCollection mapDataCollection, object parentData, TreeViewControl treeView)
        {
            StrictContextMenuItem addMapLayerMenuItem = new StrictContextMenuItem(
                MapResources.MapLegendView_MapDataCollectionContextMenuStrip__Add_MapLayer,
                MapResources.MapLegendView_MapDataCollectionContextMenuStrip_Add_MapLayer_ToolTip,
                MapResources.MapPlusIcon,
                (sender, args) => ShowSelectShapeFileDialog(mapDataCollection));

            return contextMenuBuilderProvider.Get(mapDataCollection, treeView)
                .AddCustomItem(addMapLayerMenuItem)
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        #endregion

        #region ShapeFileImporter

        private void ShowSelectShapeFileDialog(MapDataCollection mapDataCollection)
        {
            using (var dialog = new OpenFileDialog
            {
                Filter = string.Format("{0} (*.shp)|*.shp", MapResources.MapLegendView_ShowSelectShapeFileDialog_Shape_file),
                Title = GuiResources.OpenFileDialog_Title
            })
            {
                if (dialog.ShowDialog(parentWindow) == DialogResult.OK)
                {
                    CheckDataFormat(dialog.FileName, Path.GetFileNameWithoutExtension(dialog.FileName), mapDataCollection);
                }
            }
        }

        private static void CheckDataFormat(string filePath, string title, MapDataCollection mapDataCollection)
        {
            try
            {
                var featureSet = Shapefile.OpenFile(filePath);

                FeatureBasedMapData importedData;

                switch (featureSet.FeatureType)
                {
                    case FeatureType.Point:
                    case FeatureType.MultiPoint:
                        using (ShapeFileReaderBase reader = new PointShapeFileReader(filePath))
                        {
                            importedData = GetShapeFileData(reader, title);
                        }
                        break;
                    case FeatureType.Line:
                        using (ShapeFileReaderBase reader = new PolylineShapeFileReader(filePath))
                        {
                            importedData = GetShapeFileData(reader, title);
                        }
                        break;
                    case FeatureType.Polygon:
                        using (ShapeFileReaderBase reader = new PolygonShapeFileReader(filePath))
                        {
                            importedData = GetShapeFileData(reader, title);
                        }
                        break;
                    default:
                        log.Error(MapResources.MapLegendView_CheckDataFormat_ShapeFile_Contains_Unsupported_Data);
                        return;
                }

                mapDataCollection.Add(importedData);

                log.Info(MapResources.MapLegendView_CheckDataFormat_Shapefile_Is_Imported);
                mapDataCollection.NotifyObservers();
            }
            catch (ArgumentException)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(MapResources.MapLegendView_CheckDataFormat_File_does_not_contain_geometries);
                log.Error(message);
            }
            catch (FileNotFoundException)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(MapResources.MapLegendView_CheckDataFormat_File_does_not_exist_or_misses_needed_files);
                log.Error(message);
            }
            catch (IOException)
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(MapResources.MapLegendView_CheckDataFormat_An_error_occurred_when_trying_to_read_the_file);
                log.Error(message);
            }
            catch (CriticalFileReadException e)
            {
                log.Error(e.Message);
            }
            catch (Exception)
            {
                // Because NullReferenceException or NotImplementedException when reading in a corrupt shape file
                // from a third party library is expected, we catch all the exceptions here.
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(MapResources.MapLegendView_CheckDataFormat_An_error_occurred_when_trying_to_read_the_file);
                log.Error(message);
            }
        }

        private static FeatureBasedMapData GetShapeFileData(ShapeFileReaderBase reader, string title)
        {
            return reader.ReadShapeFile(title);
        }

        #endregion
    }
}