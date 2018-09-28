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

using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.Gis.Data;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// Class responsible for keeping track of various status information related to the
    /// <see cref="WellKnownTileSourceMapData"/> used to create a background layer in a map control.
    /// </summary>
    internal class WellKnownBackgroundLayerStatus : BackgroundLayerStatus
    {
        private WellKnownTileSource? wellKnownTileSource;

        public override void ClearConfiguration(bool expectRecreationOfSameBackgroundLayer = false)
        {
            wellKnownTileSource = null;

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

        protected override void OnLayerInitializationSuccessful(BruTileLayer backgroundLayer, ImageBasedMapData dataSource)
        {
            var wellKnownTileSourceMapData = dataSource as WellKnownTileSourceMapData;
            if (wellKnownTileSourceMapData == null)
            {
                PreviousBackgroundLayerCreationFailed = true;
                return;
            }

            wellKnownTileSource = wellKnownTileSourceMapData.TileSource;

            BackgroundLayer = backgroundLayer;
            PreviousBackgroundLayerCreationFailed = false;
        }

        protected override bool OnHasSameConfiguration(ImageBasedMapData mapData)
        {
            var wellKnownTileSourceMapData = mapData as WellKnownTileSourceMapData;
            return wellKnownTileSourceMapData != null
                   && Equals(wellKnownTileSourceMapData.TileSource, wellKnownTileSource);
        }
    }
}