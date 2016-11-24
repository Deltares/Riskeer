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
using Core.Components.DotSpatial.Converter;
using Core.Components.DotSpatial.MapFunctions;
using Core.Components.Gis.Data;
using Core.Components.Gis.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// This class describes a map view with configured projection and function mode.
    /// </summary>
    public sealed class MapControl : Control, IMapControl
    {
        private readonly Cursor defaultCursor = Cursors.Default;

        private Map map;
        private MapFunctionPan mapFunctionPan;
        private MapFunctionSelectionZoom mapFunctionSelectionZoom;
        private MouseCoordinatesMapExtension mouseCoordinatesMapExtension;

        private readonly RecursiveObserver<MapDataCollection, MapData> mapDataObserver;

        /// <summary>
        /// Creates a new instance of <see cref="MapControl"/>.
        /// </summary>
        public MapControl()
        {
            InitializeMapView();
            TogglePanning();

            Data = new MapDataCollection("Root");

            mapDataObserver = new RecursiveObserver<MapDataCollection, MapData>(DrawFeatureSets, mdc => mdc.Collection)
            {
                Observable = Data
            };

            DrawFeatureSets();
        }

        public bool IsPanningEnabled { get; private set; }

        public bool IsRectangleZoomingEnabled { get; private set; }

        public bool IsMouseCoordinatesVisible { get; private set; }

        public MapDataCollection Data { get; private set; }

        public void ZoomToAllVisibleLayers()
        {
            IEnvelope envelope = CreateEnvelopeForAllVisibleLayers();
            if (!envelope.IsNull)
            {
                var extent = envelope.ToExtent();
                AddPadding(extent);
                map.ViewExtents = extent;
            }
        }

        public void TogglePanning()
        {
            ResetDefaultInteraction();

            IsPanningEnabled = true;

            map.FunctionMode = FunctionMode.Pan;
        }

        public void ToggleRectangleZooming()
        {
            ResetDefaultInteraction();

            IsRectangleZoomingEnabled = true;

            map.ActivateMapFunction(mapFunctionSelectionZoom);
        }

        public void ToggleMouseCoordinatesVisibility()
        {
            if (!IsMouseCoordinatesVisible)
            {
                mouseCoordinatesMapExtension.Activate();
                IsMouseCoordinatesVisible = true;
            }
            else
            {
                mouseCoordinatesMapExtension.Deactivate();
                IsMouseCoordinatesVisible = false;
            }
        }

        public void ResetMapData()
        {
            Data = null;
        }

        protected override void Dispose(bool disposing)
        {
            map.Dispose();
            mouseCoordinatesMapExtension.Dispose();
            mapDataObserver.Dispose();

            base.Dispose(disposing);
        }

        private static void AddPadding(Extent extent)
        {
            var padding = Math.Min(extent.Height, extent.Width)*0.05;
            if (Math.Max(extent.Height, extent.Width) + padding <= double.MaxValue)
            {
                extent.ExpandBy(padding);
            }
        }

        private IEnvelope CreateEnvelopeForAllVisibleLayers()
        {
            IEnvelope envelope = new Envelope();
            foreach (IMapLayer layer in map.Layers.Where(layer => layer.IsVisible && !layer.Extent.IsEmpty()))
            {
                envelope.ExpandToInclude(layer.Extent.ToEnvelope());
            }
            return envelope;
        }

        private void ResetDefaultInteraction()
        {
            IsPanningEnabled = false;
            IsRectangleZoomingEnabled = false;

            map.FunctionMode = FunctionMode.None;
        }

        private void DrawFeatureSets()
        {
            map.ClearLayers();
            if (Data != null)
            {
                foreach (IMapFeatureLayer mapLayer in MapFeatureLayerFactory.Create(Data))
                {
                    map.Layers.Add(mapLayer);
                }
            }
        }

        private void InitializeMapView()
        {
            map = new DotSpatialMap
            {
                ProjectionModeDefine = ActionMode.Never,
                Dock = DockStyle.Fill,
                ZoomOutFartherThanMaxExtent = true
            };

            // Configure the map pan function
            mapFunctionPan = map.MapFunctions.OfType<MapFunctionPan>().First();
            mapFunctionPan.FunctionActivated += MapFunctionActivateFunction;
            mapFunctionPan.MouseDown += MapFunctionPanOnMouseDown;
            mapFunctionPan.MouseUp += MapFunctionOnMouseUp;

            // Add and configure the map selection zoom function
            mapFunctionSelectionZoom = new MapFunctionSelectionZoom(map);
            map.MapFunctions.Add(mapFunctionSelectionZoom);
            mapFunctionSelectionZoom.FunctionActivated += MapFunctionActivateFunction;
            mapFunctionSelectionZoom.MouseDown += MapFunctionSelectionZoomOnMouseDown;
            mapFunctionSelectionZoom.MouseUp += MapFunctionOnMouseUp;

            mouseCoordinatesMapExtension = new MouseCoordinatesMapExtension(map);
            ToggleMouseCoordinatesVisibility();

            Controls.Add(map);
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
    }
}