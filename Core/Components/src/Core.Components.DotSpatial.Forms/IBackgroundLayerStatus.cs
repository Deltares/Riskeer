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
    /// Interface for keeping track of various status information related to the
    /// <see cref="ImageBasedMapData"/> used to create a background layer in a map view.
    /// </summary>
    internal interface IBackgroundLayerStatus : IDisposable
    {
        /// <summary>
        /// Gets a value indicating that the most recent attempt to create the background
        /// layer failed (returning <c>true</c>) or was successful (returning <c>false</c>).
        /// </summary>
        bool PreviousBackgroundLayerCreationFailed { get; }

        /// <summary>
        /// Gets the initialized background layer.
        /// </summary>
        BruTileLayer BackgroundLayer { get; }

        /// <summary>
        /// Mark that a (new) background layer has successfully been initialized.
        /// </summary>
        /// <param name="backgroundLayer">The constructed layer.</param>
        /// <param name="dataSource">The data used to construct <paramref name="backgroundLayer"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        void LayerInitializationSuccessful(BruTileLayer backgroundLayer, ImageBasedMapData dataSource);

        /// <summary>
        /// Mark that the attempt to create a new background layer failed.
        /// </summary>
        void LayerInitializationFailed();

        /// <summary>
        /// Clears the status information for the background layer and disposes <see cref="BackgroundLayer"/>
        /// as well.
        /// </summary>
        /// <param name="expectRecreationOfSameBackgroundLayer">Optional: A flag to indicate 
        /// if recreation of <see cref="BackgroundLayer"/> with the same parameters is expected
        /// (<c>true</c>) or is expected to be replaced (<c>false</c>).</param>
        void ClearConfiguration(bool expectRecreationOfSameBackgroundLayer = false);

        /// <summary>
        /// Indicates if a <see cref="ImageBasedMapData"/> corresponds with the <see cref="BackgroundLayer"/>.
        /// </summary>
        /// <param name="mapData">The map data.</param>
        /// <returns>Returns <c>true</c> if <paramref name="mapData"/> corresponds with
        /// <see cref="BackgroundLayer"/>, or <c>false</c> when this is not the case.</returns>
        bool HasSameConfiguration(ImageBasedMapData mapData);
    }
}