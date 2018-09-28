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
using Core.Components.Gis.Data;

namespace Core.Components.Gis.Forms
{
    /// <summary>
    /// Interface describing general map interactions.
    /// </summary>
    public interface IMapControl
    {
        /// <summary>
        /// Gets a value indicating whether or not the map can be panned with the left mouse button.
        /// </summary>
        bool IsPanningEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether or not the map can be zoomed by rectangle with the left mouse button.
        /// </summary>
        bool IsRectangleZoomingEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether or not the map coordinates of the mouse should be shown.
        /// </summary>
        bool IsMouseCoordinatesVisible { get; }

        /// <summary>
        /// Gets the data to show in the <see cref="IMapControl"/>.
        /// </summary>
        MapDataCollection Data { get; }

        /// <summary>
        /// Gets the data to show in the background of the <see cref="IMapControl"/>.
        /// </summary>
        ImageBasedMapData BackgroundMapData { get; }

        /// <summary>
        /// Removes all the data from the map without redrawing any layers.
        /// </summary>
        void RemoveAllData();

        /// <summary>
        /// Zooms to a level so that all visible layers are in view.
        /// </summary>
        void ZoomToAllVisibleLayers();

        /// <summary>
        /// Zooms to a level such that the given map data is in view.
        /// </summary>
        /// <param name="layerData">The data to zoom to.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="layerData"/>
        /// is not part of <see cref="Data"/>.</exception>
        void ZoomToAllVisibleLayers(MapData layerData);

        /// <summary>
        /// Toggles panning of the <see cref="IMapControl"/>. Panning is invoked by clicking the left mouse-button.
        /// </summary>
        void TogglePanning();

        /// <summary>
        /// Toggles rectangle zooming of the <see cref="IMapControl"/>. Rectangle zooming is invoked by clicking the left mouse-button.
        /// </summary>
        void ToggleRectangleZooming();

        /// <summary>
        /// Toggles the visibility mouse coordinates of the <see cref="IMapControl"/>.
        /// </summary>
        void ToggleMouseCoordinatesVisibility();
    }
}