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
using System.Collections.Generic;
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
using MapFunctionPan = Core.Components.DotSpatial.MapFunctions.MapFunctionPan;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// This class describes a map view with configured projection and function mode.
    /// </summary>
    public sealed class MapControl : Control, IMapControl, IObserver
    {
        private readonly MapFeatureLayerFactory mapFeatureLayerFactory = new MapFeatureLayerFactory();

        private Map map;
        private IMapFunction mapFunctionSelectionZoom;
        private MouseCoordinatesMapExtension mouseCoordinatesMapExtension;
        private IMapFunction mapFunctionPanning;

        /// <summary>
        /// Creates a new instance of <see cref="MapControl"/>.
        /// </summary>
        public MapControl()
        {
            InitializeMapView();
            TogglePanning();

            Data = new MapDataCollection(new List<MapData>(), "Root");
            Data.Attach(this);

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
            if (!IsPanningEnabled)
            {
                ResetDefaultInteraction();
                map.FunctionMode = FunctionMode.Pan;
                IsPanningEnabled = true;
                map.Cursor = Cursors.Hand;

                if (mapFunctionPanning == null)
                {
                    mapFunctionPanning = new MapFunctionPan(map);
                }
                map.ActivateMapFunction(mapFunctionPanning);
            }
        }

        public void ToggleRectangleZooming()
        {
            if (!IsRectangleZoomingEnabled)
            {
                ResetDefaultInteraction();
                map.FunctionMode = FunctionMode.Select;
                IsRectangleZoomingEnabled = true;

                if (mapFunctionSelectionZoom == null)
                {
                    mapFunctionSelectionZoom = new MapFunctionSelectionZoom(map);
                }
                map.ActivateMapFunction(mapFunctionSelectionZoom);
            }
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

        public void UpdateObserver()
        {
            DrawFeatureSets();
        }

        protected override void Dispose(bool disposing)
        {
            map.Dispose();
            mouseCoordinatesMapExtension.Dispose();
            base.Dispose(disposing);
        }

        private static void AddPadding(Extent extent)
        {
            var padding = Math.Min(extent.Height, extent.Width)*0.05;
            if (Math.Max(extent.Height, extent.Width) + padding <= Double.MaxValue)
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
        }

        private void DrawFeatureSets()
        {
            map.ClearLayers();
            if (Data != null)
            {
                foreach (IMapFeatureLayer mapLayer in mapFeatureLayerFactory.Create(Data))
                {
                    map.Layers.Add(mapLayer);
                }
            }
        }

        private void InitializeMapView()
        {
            map = new Map
            {
                ProjectionModeDefine = ActionMode.Never,
                Dock = DockStyle.Fill,
                FunctionMode = FunctionMode.Pan,
                ZoomOutFartherThanMaxExtent = true
            };

            mouseCoordinatesMapExtension = new MouseCoordinatesMapExtension(map);
            ToggleMouseCoordinatesVisibility();

            Controls.Add(map);
        }
    }
}