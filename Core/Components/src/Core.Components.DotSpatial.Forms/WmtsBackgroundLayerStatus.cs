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
    /// <see cref="WmtsMapData"/> used to create a background layer in a map view.
    /// </summary>
    internal class WmtsBackgroundLayerStatus : IDisposable
    {
        public void Dispose()
        {
            BackgroundLayer?.Dispose();
        }

        /// <summary>
        /// Gets a value indicating that the most recent attempt to create the background
        /// layer failed (returning <c>true</c>) or was successful (returning <c>false</c>).
        /// </summary>
        internal bool PreviousBackgroundLayerCreationFailed { get; private set; }

        /// <summary>
        /// Gets a value for the initialized background layer.
        /// </summary>
        internal BruTileLayer BackgroundLayer { get; private set; }

        /// <summary>
        /// Mark that a (new) background layer has successfully been initialized.
        /// </summary>
        /// <param name="backgroundLayer">The constructed layer.</param>
        /// <param name="dataSource">The data used to construct <paramref name="backgroundLayer"/>.</param>
        internal void SuccessfullyInitializedLayer(BruTileLayer backgroundLayer, WmtsMapData dataSource)
        {
            if (backgroundLayer == null)
            {
                throw new ArgumentNullException(nameof(backgroundLayer));
            }
            if (dataSource == null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            SourceCapabilitiesUrl = dataSource.SourceCapabilitiesUrl;
            SelectedCapabilityId = dataSource.SelectedCapabilityIdentifier;
            PreferredFormat = dataSource.PreferredFormat;

            BackgroundLayer = backgroundLayer;
            PreviousBackgroundLayerCreationFailed = false;
        }

        /// <summary>
        /// Mark that the attempt to create a new background layer failed.
        /// </summary>
        internal void LayerInitializationFailed()
        {
            ClearConfiguration();
            PreviousBackgroundLayerCreationFailed = true;
        }

        /// <summary>
        /// Clears the status information for the background layer and disposes <see cref="BackgroundLayer"/>
        /// as well.
        /// </summary>
        /// <param name="expectRecreationOfSameBackgroundLayer">Optional: A flag to indicate 
        /// if recreation of <see cref="BackgroundLayer"/> with the same parameters is expected
        /// (<c>true</c>) or is expected to be replaced (<c>false</c>)</param>
        internal void ClearConfiguration(bool expectRecreationOfSameBackgroundLayer = false)
        {
            SourceCapabilitiesUrl = null;
            SelectedCapabilityId = null;
            PreferredFormat = null;

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

        /// <summary>
        /// Indicates if a <see cref="WmtsMapData"/> corresponds with the <see cref="BackgroundLayer"/>.
        /// </summary>
        /// <param name="mapData">The map data.</param>
        /// <returns>Returns <c>true</c> if <paramref name="mapData"/> corresponds with
        /// <see cref="BackgroundLayer"/>, or <c>false</c> when this is not the case.</returns>
        internal bool HasSameConfiguration(WmtsMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            return Equals(mapData.SourceCapabilitiesUrl, SourceCapabilitiesUrl)
                   && Equals(mapData.SelectedCapabilityIdentifier, SelectedCapabilityId)
                   && Equals(mapData.PreferredFormat, PreferredFormat);
        }

        private string SourceCapabilitiesUrl { get; set; }
        private string SelectedCapabilityId { get; set; }
        private string PreferredFormat { get; set; }
    }
}