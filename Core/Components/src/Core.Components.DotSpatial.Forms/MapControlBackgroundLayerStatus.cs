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
    /// <see cref="ImageBasedMapData"/> used to create a background layer in a map control.
    /// </summary>
    internal class MapControlBackgroundLayerStatus : BackgroundLayerStatus
    {
        private BackgroundLayerStatus backgroundLayerStatus;

        public override BruTileLayer BackgroundLayer
        {
            get
            {
                return backgroundLayerStatus?.BackgroundLayer;
            }
        }

        public override bool PreviousBackgroundLayerCreationFailed
        {
            get
            {
                return backgroundLayerStatus?.PreviousBackgroundLayerCreationFailed ?? base.PreviousBackgroundLayerCreationFailed;
            }
        }

        public override void ClearConfiguration(bool expectRecreationOfSameBackgroundLayer = false)
        {
            backgroundLayerStatus?.ClearConfiguration(expectRecreationOfSameBackgroundLayer);
        }

        protected override void OnLayerInitializationSuccessful(BruTileLayer backgroundLayer, ImageBasedMapData dataSource)
        {
            if (backgroundLayerStatus == null || !backgroundLayerStatus.HasSameConfiguration(dataSource))
            {
                backgroundLayerStatus = BackgroundLayerStatusFactory.CreateBackgroundLayerStatus(dataSource);
            }

            backgroundLayerStatus.LayerInitializationSuccessful(backgroundLayer, dataSource);
        }

        protected override bool OnHasSameConfiguration(ImageBasedMapData mapData)
        {
            return backgroundLayerStatus?.HasSameConfiguration(mapData) ?? false;
        }
    }
}