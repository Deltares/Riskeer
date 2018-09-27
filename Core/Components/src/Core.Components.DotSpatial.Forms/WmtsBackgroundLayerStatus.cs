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
    /// <see cref="WmtsMapData"/> used to create a background layer in a map control.
    /// </summary>
    internal class WmtsBackgroundLayerStatus : BackgroundLayerStatus
    {
        private string sourceCapabilitiesUrl;
        private string selectedCapabilityId;
        private string preferredFormat;

        public override void ClearConfiguration(bool expectRecreationOfSameBackgroundLayer = false)
        {
            sourceCapabilitiesUrl = null;
            selectedCapabilityId = null;
            preferredFormat = null;

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
            var wmtsDataSource = dataSource as WmtsMapData;
            if (wmtsDataSource == null)
            {
                PreviousBackgroundLayerCreationFailed = true;
                return;
            }

            sourceCapabilitiesUrl = wmtsDataSource.SourceCapabilitiesUrl;
            selectedCapabilityId = wmtsDataSource.SelectedCapabilityIdentifier;
            preferredFormat = wmtsDataSource.PreferredFormat;

            BackgroundLayer = backgroundLayer;
            PreviousBackgroundLayerCreationFailed = false;
        }

        protected override bool OnHasSameConfiguration(ImageBasedMapData mapData)
        {
            var wmtsDataSource = mapData as WmtsMapData;
            return wmtsDataSource != null
                   && Equals(wmtsDataSource.SourceCapabilitiesUrl, sourceCapabilitiesUrl)
                   && Equals(wmtsDataSource.SelectedCapabilityIdentifier, selectedCapabilityId)
                   && Equals(wmtsDataSource.PreferredFormat, preferredFormat);
        }
    }
}