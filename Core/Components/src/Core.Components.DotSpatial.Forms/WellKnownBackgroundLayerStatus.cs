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
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.Gis.Data;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// Class responsible for keeping track of various status information related to the
    /// <see cref="WellKnownTileSourceMapData"/> used to create a background layer in a map control.
    /// </summary>
    internal class WellKnownBackgroundLayerStatus : IBackgroundLayerStatus
    {
        public bool PreviousBackgroundLayerCreationFailed { get; private set; }

        public BruTileLayer BackgroundLayer { get; private set; }

        public virtual void Dispose()
        {
            BackgroundLayer?.Dispose();
        }

        public void LayerInitializationSuccessful(BruTileLayer backgroundLayer, ImageBasedMapData dataSource)
        {
            if (backgroundLayer == null)
            {
                throw new ArgumentNullException(nameof(backgroundLayer));
            }
            if (dataSource == null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }
            var wellKnownTileSourceMapData = dataSource as WellKnownTileSourceMapData;
            if (wellKnownTileSourceMapData == null)
            {
                PreviousBackgroundLayerCreationFailed = true;
                return;
            }

            WellKnownTileSource = wellKnownTileSourceMapData.TileSource;

            BackgroundLayer = backgroundLayer;
            PreviousBackgroundLayerCreationFailed = false;
        }

        public void LayerInitializationFailed()
        {
            ClearConfiguration();
            PreviousBackgroundLayerCreationFailed = true;
        }

        public void ClearConfiguration(bool expectRecreationOfSameBackgroundLayer = false)
        {
            WellKnownTileSource = null;

            if (BackgroundLayer != null)
            {
                BackgroundLayer.Dispose();
                BackgroundLayer = null;
            }

            if (!expectRecreationOfSameBackgroundLayer)
            {
                PreviousBackgroundLayerCreationFailed = false;
            }
        }

        public bool HasSameConfiguration(ImageBasedMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            var wellKnownTileSourceMapData = mapData as WellKnownTileSourceMapData;
            if (wellKnownTileSourceMapData == null)
            {
                return false;
            }
            return Equals(wellKnownTileSourceMapData.TileSource, WellKnownTileSource);
        }

        private WellKnownTileSource? WellKnownTileSource { get; set; }
    }
}