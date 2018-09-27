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

using System;
using Core.Components.DotSpatial.Layer.BruTile;
using Core.Components.Gis.Data;

namespace Core.Components.DotSpatial.Forms
{
    /// <summary>
    /// Abstract class for keeping track of various status information related to the
    /// <see cref="ImageBasedMapData"/> used to create a background layer in a map view.
    /// </summary>
    internal abstract class BackgroundLayerStatus : IDisposable
    {
        /// <summary>
        /// Gets a value indicating that the most recent attempt to create the background
        /// layer failed (returning <c>true</c>) or was successful (returning <c>false</c>).
        /// </summary>
        public virtual bool PreviousBackgroundLayerCreationFailed { get; protected set; }

        /// <summary>
        /// Gets the initialized background layer.
        /// </summary>
        public virtual BruTileLayer BackgroundLayer { get; protected set; }

        /// <summary>
        /// Marks that a (new) background layer has successfully been initialized.
        /// </summary>
        /// <param name="backgroundLayer">The constructed layer.</param>
        /// <param name="dataSource">The data used to construct <paramref name="backgroundLayer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
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

            OnLayerInitializationSuccessful(backgroundLayer, dataSource);
        }

        /// <summary>
        /// Mark that the attempt to create a new background layer failed.
        /// </summary>
        public void LayerInitializationFailed()
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
        /// (<c>true</c>) or is expected to be replaced (<c>false</c>).</param>
        public abstract void ClearConfiguration(bool expectRecreationOfSameBackgroundLayer = false);

        /// <summary>
        /// Indicates if a <see cref="ImageBasedMapData"/> corresponds with the <see cref="BackgroundLayer"/>.
        /// </summary>
        /// <param name="mapData">The map data.</param>
        /// <returns>Returns <c>true</c> if <paramref name="mapData"/> corresponds with
        /// <see cref="BackgroundLayer"/>, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapData"/> is <c>null</c>.</exception>
        public bool HasSameConfiguration(ImageBasedMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException(nameof(mapData));
            }

            return OnHasSameConfiguration(mapData);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                BackgroundLayer?.Dispose();
            }
        }

        /// <summary>
        /// Marks that a (new) background layer has successfully been initialized.
        /// </summary>
        /// <param name="backgroundLayer">The constructed layer.</param>
        /// <param name="dataSource">The data used to construct <paramref name="backgroundLayer"/>.</param>
        protected abstract void OnLayerInitializationSuccessful(BruTileLayer backgroundLayer, ImageBasedMapData dataSource);

        /// <summary>
        /// Indicates if a <see cref="ImageBasedMapData"/> corresponds with the <see cref="BackgroundLayer"/>.
        /// </summary>
        /// <param name="mapData">The map data.</param>
        /// <returns>Returns <c>true</c> if <paramref name="mapData"/> corresponds with
        /// <see cref="BackgroundLayer"/>, <c>false</c> otherwise.</returns>
        protected abstract bool OnHasSameConfiguration(ImageBasedMapData mapData);
    }
}