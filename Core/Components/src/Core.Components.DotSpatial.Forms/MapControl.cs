// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.Views;
using Core.Common.Util.Drawing;
using Core.Components.DotSpatial.Forms.Properties;
using Core.Components.DotSpatial.Layer;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.DotSpatial.MapFunctions;
using Core.Components.Gis.Data;
using Core.Components.Gis.Exceptions;
using Core.Components.Gis.Features;
using Core.Components.Gis.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using log4net;
using Timer = System.Timers.Timer;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// This class describes a map control with configured projection and function mode.
    /// </summary>
    public partial class MapControl : UserControl, IMapControl, ISelectionProvider
    {
        private const int updateTimerInterval = 10;

        private static readonly PrivateFontCollection privateFontCollection = new PrivateFontCollection();
        private static readonly Font font = FontHelper.CreateFont(Resources.Symbols, privateFontCollection);

        private readonly ILog log = LogManager.GetLogger(typeof(MapControl));
        private readonly Cursor defaultCursor = Cursors.Default;
        private readonly RecursiveObserver<MapDataCollection, MapDataCollection> mapDataCollectionObserver;
        private readonly Observer backGroundMapDataObserver;
        private readonly List<DrawnMapData> drawnMapDataList = new List<DrawnMapData>();
        private readonly MapControlBackgroundLayerStatus backgroundLayerStatus = new MapControlBackgroundLayerStatus();
        private readonly List<IFeatureBasedMapDataLayer> mapDataLayersToUpdate = new List<IFeatureBasedMapDataLayer>();

        private Map map;
        private bool removing;
        private MapFunctionSelectionZoom mapFunctionSelectionZoom;
        private RdNewMouseCoordinatesMapExtension mouseCoordinatesMapExtension;
        private MapDataCollection data;
        private ImageBasedMapData backgroundMapData;
        private Timer updateTimer;

        /// <summary>
        /// Creates a new instance of <see cref="MapControl"/>.
        /// </summary>
        public MapControl()
        {
            InitializeComponent();

            SetFonts();

            InitializeMap();

            // panToolStripButton.PerformClick();
            map.FunctionMode = FunctionMode.Select;
            showCoordinatesToolStripButton.PerformClick();

            mapDataCollectionObserver = new RecursiveObserver<MapDataCollection, MapDataCollection>(HandleMapDataCollectionChange, mdc => mdc.Collection);
            backGroundMapDataObserver = new Observer(HandleBackgroundMapDataChange);

            InitializeUpdateTimer();
        }

        public MapDataCollection Data
        {
            get
            {
                return data;
            }
            set
            {
                if (HasMapData)
                {
                    ClearAllMapData(true);
                }

                data = value;

                mapDataCollectionObserver.Observable = data;

                if (HasMapData && !removing)
                {
                    DrawInitialMapData();
                }
            }
        }

        public ImageBasedMapData BackgroundMapData
        {
            get
            {
                return backgroundMapData;
            }
            set
            {
                if (HasMapData)
                {
                    ClearAllMapData(false);
                }

                backgroundMapData = value;
                backGroundMapDataObserver.Observable = backgroundMapData;

                if (HasMapData && !removing)
                {
                    DrawInitialMapData();
                }
            }
        }

        public void RemoveAllData()
        {
            removing = true;
            Data = null;
            BackgroundMapData = null;
            removing = false;
        }

        protected override void Dispose(bool disposing)
        {
            map.Dispose();
            mouseCoordinatesMapExtension.Dispose();
            mapDataCollectionObserver.Dispose();
            backGroundMapDataObserver.Dispose();
            backgroundLayerStatus.Dispose();

            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private ProjectionInfo Projection
        {
            get
            {
                return map.Projection;
            }
            set
            {
                ProjectionInfo oldProjectInfo = map.Projection;
                map.Projection = value;
                ReprojectViewExtents(oldProjectInfo, value);
            }
        }

        private bool HasMapData
        {
            get
            {
                return backgroundMapData != null || data != null;
            }
        }

        private void SetFonts()
        {
            panToolStripButton.Font = font;
            zoomToRectangleToolStripButton.Font = font;
            zoomToVisibleLayersToolStripButton.Font = font;
            showCoordinatesToolStripButton.Font = font;
        }

        private void InitializeMap()
        {
            map = new DotSpatialMap
            {
                ProjectionModeDefine = ActionMode.Never,
                Dock = DockStyle.Fill,
                ZoomOutFartherThanMaxExtent = true,
                Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem
            };

            MapFunctionPan mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();
            mapFunctionPan.FunctionActivated += MapFunctionActivateFunction;
            mapFunctionPan.MouseDown += MapFunctionPanOnMouseDown;
            mapFunctionPan.MouseUp += MapFunctionOnMouseUp;

            MapFunctionSelect mapFunctionSelect = map.MapFunctions.OfType<MapFunctionSelect>().First();
            mapFunctionSelect.FunctionActivated += MapFunctionActivateFunction;
            mapFunctionSelect.MouseDown += MapFunctionPanOnMouseDown;
            mapFunctionSelect.MouseUp += MapFunctionOnMouseUp;

            mapFunctionSelectionZoom = new MapFunctionSelectionZoom(map);
            map.MapFunctions.Add(mapFunctionSelectionZoom);
            mapFunctionSelectionZoom.FunctionActivated += MapFunctionActivateFunction;
            mapFunctionSelectionZoom.MouseDown += MapFunctionSelectionZoomOnMouseDown;
            mapFunctionSelectionZoom.MouseUp += MapFunctionOnMouseUp;

            mouseCoordinatesMapExtension = new RdNewMouseCoordinatesMapExtension(map);

            tableLayoutPanel.Controls.Add(map, 0, 0);
        }

        private void ReprojectViewExtents(ProjectionInfo projectFrom, ProjectionInfo projectTo)
        {
            double[] viewExtentXY =
            {
                map.ViewExtents.MinX,
                map.ViewExtents.MinY,
                map.ViewExtents.MaxX,
                map.ViewExtents.MaxY
            };
            double[] viewExtentZ =
            {
                0.0,
                0.0
            };

            Reproject.ReprojectPoints(viewExtentXY, viewExtentZ, projectFrom, projectTo, 0, 2);

            map.ViewExtents = new Extent(viewExtentXY);
        }

        private void ReprojectLayers(IEnumerable<IMapLayer> layersToReproject)
        {
            foreach (IMapLayer mapLayer in layersToReproject)
            {
                if (!mapLayer.Projection.Equals(Projection))
                {
                    mapLayer.Reproject(Projection);
                    mapLayer.Invalidate();
                }
            }
        }

        #region Background layer

        /// <summary>
        /// Attempts to initialize the background layer.
        /// </summary>
        /// <returns><c>true</c> if initialization of the background layer was successful,
        /// <c>false</c> otherwise.
        /// </returns>
        private bool InitializeBackgroundLayer()
        {
            BruTileLayer backgroundLayer;

            try
            {
                backgroundLayer = ImageBasedMapDataLayerFactory.Create(backgroundMapData);
            }
            catch (ConfigurationInitializationException e)
            {
                if (!backgroundLayerStatus.PreviousBackgroundLayerCreationFailed)
                {
                    string fullMessage = string.Format(Resources.MapControl_HandleBruTileInitializationException_Message_0_therefore_cannot_show_background_layer,
                                                       e.Message);
                    log.Error(fullMessage, e);
                }

                backgroundLayerStatus.LayerInitializationFailed();
                return false;
            }

            if (backgroundLayer == null)
            {
                return false;
            }

            backgroundLayerStatus.LayerInitializationSuccessful(backgroundLayer, backgroundMapData);

            return true;
        }

        private void InsertBackgroundLayer()
        {
            if (backgroundMapData.IsConfigured)
            {
                if (InitializeBackgroundLayer())
                {
                    InsertBackgroundLayerAndReprojectExistingLayers();
                }
            }
            else
            {
                Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem;
                ReprojectLayers(map.Layers);
            }
        }

        private void InsertBackgroundLayerAndReprojectExistingLayers()
        {
            IMapLayer[] existingMapLayers = map.Layers.ToArray();

            Projection = backgroundLayerStatus.BackgroundLayer.Projection;
            map.Layers.Insert(0, backgroundLayerStatus.BackgroundLayer);

            ReprojectLayers(existingMapLayers);
        }

        private void HandleBackgroundMapDataChange()
        {
            if (backgroundLayerStatus.BackgroundLayer != null)
            {
                if (!backgroundLayerStatus.HasSameConfiguration(backgroundMapData))
                {
                    map.Layers.Remove(backgroundLayerStatus.BackgroundLayer);
                    backgroundLayerStatus.ClearConfiguration();

                    InsertBackgroundLayer();
                }
                else
                {
                    backgroundLayerStatus.BackgroundLayer.IsVisible = backgroundMapData.IsVisible;
                    backgroundLayerStatus.BackgroundLayer.Transparency = Convert.ToSingle(backgroundMapData.Transparency);
                }
            }
            else
            {
                InsertBackgroundLayer();
            }
        }

        #endregion

        #region DrawMapData

        /// <summary>
        /// Lookup class for administration related to drawn map data layers.
        /// </summary>
        private class DrawnMapData
        {
            /// <summary>
            /// The feature based map data which the drawn <see cref="FeatureBasedMapDataLayer"/> is based upon.
            /// </summary>
            public FeatureBasedMapData FeatureBasedMapData { get; set; }

            /// <summary>
            /// The drawn map data layer.
            /// </summary>
            public IFeatureBasedMapDataLayer FeatureBasedMapDataLayer { get; set; }

            /// <summary>
            /// The observer attached to <see cref="FeatureBasedMapData"/> and responsible for updating <see cref="FeatureBasedMapDataLayer"/>.
            /// </summary>
            public Observer Observer { get; set; }
        }

        private void ClearAllMapData(bool expectedRecreationOfBackgroundLayer)
        {
            foreach (DrawnMapData drawnMapData in drawnMapDataList)
            {
                drawnMapData.Observer.Dispose();
            }

            drawnMapDataList.Clear();

            map.ClearLayers();
            Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem;

            backgroundLayerStatus?.ClearConfiguration(expectedRecreationOfBackgroundLayer);
        }

        private void DrawInitialMapData()
        {
            if (backgroundMapData != null && backgroundMapData.IsConfigured && InitializeBackgroundLayer())
            {
                Projection = backgroundLayerStatus.BackgroundLayer.Projection;
                map.Layers.Add(backgroundLayerStatus.BackgroundLayer);
            }

            if (Data != null)
            {
                foreach (FeatureBasedMapData featureBasedMapData in Data.GetFeatureBasedMapDataRecursively())
                {
                    DrawMapData(featureBasedMapData);
                }
            }
        }

        private void DrawMapData(FeatureBasedMapData featureBasedMapData)
        {
            IFeatureBasedMapDataLayer featureBasedMapDataLayer = FeatureBasedMapDataLayerFactory.Create(featureBasedMapData);

            var drawnMapData = new DrawnMapData
            {
                FeatureBasedMapData = featureBasedMapData,
                FeatureBasedMapDataLayer = featureBasedMapDataLayer
            };

            drawnMapData.Observer = new Observer(() =>
            {
                mapDataLayersToUpdate.Add(drawnMapData.FeatureBasedMapDataLayer);
                StartUpdateTimer();
            })
            {
                Observable = featureBasedMapData
            };

            drawnMapDataList.Add(drawnMapData);

            if (!Projection.Equals(featureBasedMapDataLayer.Projection))
            {
                featureBasedMapDataLayer.Reproject(Projection);
            }

            if (featureBasedMapDataLayer.SelectionEnabled)
            {
                featureBasedMapDataLayer.SelectionChanged += (sender, args) => HandleMapLayerSelection(drawnMapData);
            }

            map.Layers.Add(featureBasedMapDataLayer);
        }

        private void HandleMapLayerSelection(DrawnMapData drawnMapData)
        {
            var selectedMapFeatures = new List<MapFeature>();

            foreach (IFeature selectedFeature in drawnMapData.FeatureBasedMapDataLayer.Selection.ToFeatureList())
            {
                if (drawnMapData.FeatureBasedMapData.FeatureLookup.ContainsKey(selectedFeature))
                {
                    selectedMapFeatures.Add(drawnMapData.FeatureBasedMapData.FeatureLookup[selectedFeature]);
                }
            }

            Selection = selectedMapFeatures.ToArray();
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private void DrawMissingMapDataOnCollectionChange(IEnumerable<FeatureBasedMapData> mapDataThatShouldBeDrawn,
                                                          IDictionary<FeatureBasedMapData, DrawnMapData> drawnMapDataLookup)
        {
            foreach (FeatureBasedMapData mapDataToDraw in mapDataThatShouldBeDrawn.Where(mapDataToDraw => !drawnMapDataLookup.ContainsKey(mapDataToDraw)))
            {
                DrawMapData(mapDataToDraw);
            }
        }

        private void HandleMapDataCollectionChange()
        {
            List<FeatureBasedMapData> mapDataThatShouldBeDrawn = Data.GetFeatureBasedMapDataRecursively().ToList();
            Dictionary<FeatureBasedMapData, DrawnMapData> drawnMapDataLookup = drawnMapDataList.ToDictionary(dmd => dmd.FeatureBasedMapData, dmd => dmd);

            DrawMissingMapDataOnCollectionChange(mapDataThatShouldBeDrawn, drawnMapDataLookup);
            RemoveRedundantMapDataOnCollectionChange(mapDataThatShouldBeDrawn, drawnMapDataLookup);

            drawnMapDataLookup = drawnMapDataList.ToDictionary(dmd => dmd.FeatureBasedMapData, dmd => dmd);

            ReorderMapDataOnCollectionChange(mapDataThatShouldBeDrawn, drawnMapDataLookup);
        }

        private void ReorderMapDataOnCollectionChange(IList<FeatureBasedMapData> mapDataThatShouldBeDrawn,
                                                      IDictionary<FeatureBasedMapData, DrawnMapData> drawnMapDataLookup)
        {
            int shiftedIndex = backgroundLayerStatus.BackgroundLayer != null ? 1 : 0;

            for (var i = 0; i < mapDataThatShouldBeDrawn.Count; i++)
            {
                map.Layers.Move(drawnMapDataLookup[mapDataThatShouldBeDrawn[i]].FeatureBasedMapDataLayer, i + shiftedIndex);
            }
        }

        private void RemoveMapData(DrawnMapData drawnMapDataToRemove)
        {
            if (drawnMapDataToRemove.FeatureBasedMapDataLayer.SelectionEnabled)
            {
                drawnMapDataToRemove.FeatureBasedMapDataLayer.SelectionChanged -= (sender, args) => HandleMapLayerSelection(drawnMapDataToRemove);
            }
            
            drawnMapDataToRemove.Observer.Dispose();
            drawnMapDataList.Remove(drawnMapDataToRemove);

            map.Layers.Remove(drawnMapDataToRemove.FeatureBasedMapDataLayer);
        }

        private void RemoveRedundantMapDataOnCollectionChange(IEnumerable<FeatureBasedMapData> mapDataThatShouldBeDrawn,
                                                              IDictionary<FeatureBasedMapData, DrawnMapData> drawnMapDataLookup)
        {
            foreach (FeatureBasedMapData featureBasedMapData in drawnMapDataLookup.Keys.Except(mapDataThatShouldBeDrawn))
            {
                RemoveMapData(drawnMapDataLookup[featureBasedMapData]);
            }
        }

        #endregion

        #region Map Interaction

        public void ZoomToVisibleLayers()
        {
            ZoomToVisibleLayers(Data);
        }

        public void ZoomToVisibleLayers(MapData mapData)
        {
            Envelope envelope = CreateEnvelopeForVisibleLayers(mapData);

            if (!envelope.IsNull)
            {
                Extent extent = envelope.ToExtent();

                AddPadding(extent);

                map.ViewExtents = extent;
            }
        }

        private static void AddPadding(Extent extent)
        {
            double padding = Math.Min(extent.Height, extent.Width) * 0.05;

            if (Math.Max(extent.Height, extent.Width) + padding <= double.MaxValue)
            {
                extent.ExpandBy(padding);
            }
        }

        /// <summary>
        /// Defines the area taken up by the visible map-data based on the provided map-data.
        /// </summary>
        /// <param name="mapData">The data to determine the visible extent for.</param>
        /// <returns>The area definition.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="mapData"/> is
        /// not part of the drawn map features.</exception>
        private Envelope CreateEnvelopeForVisibleLayers(MapData mapData)
        {
            if (mapData is MapDataCollection collection)
            {
                return CreateEnvelopeForVisibleLayers(collection);
            }

            DrawnMapData drawnMapData = drawnMapDataList.FirstOrDefault(dmd => dmd.FeatureBasedMapData.Equals(mapData));

            if (drawnMapData == null)
            {
                throw new ArgumentException($@"Can only zoom to {nameof(MapData)} that is part of this {nameof(MapControl)}s drawn {nameof(mapData)}.",
                                            nameof(mapData));
            }

            var envelope = new Envelope();

            if (LayerHasVisibleExtent(drawnMapData.FeatureBasedMapDataLayer))
            {
                envelope.ExpandToInclude(drawnMapData.FeatureBasedMapDataLayer.Extent.ToEnvelope());
            }

            return envelope;
        }

        /// <summary>
        /// Defines the area taken up by the visible map-data based on the provided map-data.
        /// </summary>
        /// <param name="mapData">The data to determine the visible extent for.</param>
        /// <returns>The area definition.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="mapData"/> or
        /// any of its children is not part of the drawn map features.</exception>
        private Envelope CreateEnvelopeForVisibleLayers(MapDataCollection mapData)
        {
            var envelope = new Envelope();

            foreach (MapData childMapData in mapData.Collection)
            {
                envelope.ExpandToInclude(CreateEnvelopeForVisibleLayers(childMapData));
            }

            return envelope;
        }

        private static bool LayerHasVisibleExtent(IMapLayer layer)
        {
            return layer.IsVisible && !layer.Extent.IsEmpty();
        }

        private void MapFunctionActivateFunction(object sender, EventArgs e)
        {
            map.Cursor = defaultCursor;
        }

        private void MapFunctionOnMouseUp(object sender, GeoMouseArgs e)
        {
            map.Cursor = defaultCursor;
        }

        private void MapFunctionPanOnMouseDown(object sender, GeoMouseArgs geoMouseArgs)
        {
            map.Cursor = geoMouseArgs.Button != MouseButtons.Right ? Cursors.Hand : defaultCursor;
        }

        private void MapFunctionSelectionZoomOnMouseDown(object sender, GeoMouseArgs geoMouseArgs)
        {
            switch (geoMouseArgs.Button)
            {
                case MouseButtons.Left:
                    map.Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    map.Cursor = map.IsBusy
                                     ? Cursors.SizeNWSE
                                     : defaultCursor;
                    break;
            }
        }

        private void PanToolStripButtonClick(object sender, EventArgs e)
        {
            if (panToolStripButton.Checked)
            {
                return;
            }

            map.FunctionMode = FunctionMode.Pan;

            panToolStripButton.Checked = true;
            zoomToRectangleToolStripButton.Checked = false;
        }

        private void ZoomToRectangleToolStripButtonClick(object sender, EventArgs e)
        {
            if (zoomToRectangleToolStripButton.Checked)
            {
                return;
            }

            map.FunctionMode = FunctionMode.None;
            map.ActivateMapFunction(mapFunctionSelectionZoom);

            panToolStripButton.Checked = false;
            zoomToRectangleToolStripButton.Checked = true;
        }

        private void ZoomToVisibleLayersToolStripButtonClick(object sender, EventArgs e)
        {
            ZoomToVisibleLayers();
        }

        private void ShowCoordinatesToolStripButtonClick(object sender, EventArgs e)
        {
            showCoordinatesToolStripButton.Checked = !showCoordinatesToolStripButton.Checked;

            if (showCoordinatesToolStripButton.Checked)
            {
                mouseCoordinatesMapExtension.Activate();
            }
            else
            {
                mouseCoordinatesMapExtension.Deactivate();
            }
        }

        #endregion

        #region Update timer

        private void InitializeUpdateTimer()
        {
            updateTimer = new Timer
            {
                Interval = updateTimerInterval,
                SynchronizingObject = this
            };

            updateTimer.Elapsed += (sender, args) =>
            {
                updateTimer.Stop();
                UpdateMapDataLayers();
            };
        }

        private void StartUpdateTimer()
        {
            if (updateTimer.Enabled)
            {
                updateTimer.Stop();
            }

            updateTimer.Start();
        }

        private void UpdateMapDataLayers()
        {
            foreach (IFeatureBasedMapDataLayer mapDataLayerToUpdate in mapDataLayersToUpdate.Distinct())
            {
                mapDataLayerToUpdate.Update();
            }

            mapDataLayersToUpdate.Clear();
        }

        #endregion

        public event EventHandler<EventArgs> SelectionChanged;
        public object Selection { get; private set; }
    }
}