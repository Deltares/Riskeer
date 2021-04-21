// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
        /// Gets the data to show in the <see cref="IMapControl"/>.
        /// </summary>
        MapDataCollection Data { get; }

        /// <summary>
        /// Gets the data to show in the background of the <see cref="IMapControl"/>.
        /// </summary>
        ImageBasedMapData BackgroundMapData { get; }

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
    }
}