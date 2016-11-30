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
using Core.Components.Gis.Data;
using DotSpatial.Controls;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The interface for a converter which converts <see cref="MapData"/> into <see cref="IMapFeatureLayer"/>.
    /// </summary>
    public interface IMapDataConverter
    {
        /// <summary>
        /// Checks whether the <see cref="IMapDataConverter"/> can convert the <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="MapData"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref name="data"/> can be converted by the
        /// <see cref="IMapDataConverter"/>, <c>false</c> otherwise.</returns>
        bool CanConvertMapData(MapData data);

        /// <summary>
        /// Creates a <see cref="IMapFeatureLayer"/> based on the <paramref name="data"/> that was given.
        /// </summary>
        /// <param name="data">The data to transform into a <see cref="IMapFeatureLayer"/>.</param>
        /// <returns>A new <see cref="IMapFeatureLayer"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <see cref="CanConvertMapData"/> returns <c>false</c>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        IMapFeatureLayer Convert(MapData data);

        /// <summary>
        /// Converts all feature related data from <paramref name="data"/> to <paramref name="layer"/>.
        /// </summary>
        /// <param name="data">The data to convert the feature related data from.</param>
        /// <param name="layer">The layer to convert the feature related data to.</param>
        void ConvertLayerFeatures(MapData data, IMapFeatureLayer layer);

        /// <summary>
        /// Converts all general properties (like <see cref="MapData.Name"/> and <see cref="MapData.IsVisible"/>) 
        /// from <paramref name="data"/> to <paramref name="layer"/>.
        /// </summary>
        /// <param name="data">The data to convert the general properties from.</param>
        /// <param name="layer">The layer to convert the general properties to.</param>
        void ConvertLayerProperties(MapData data, IMapFeatureLayer layer);
    }
}