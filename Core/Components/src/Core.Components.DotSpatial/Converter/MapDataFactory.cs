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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Components.DotSpatial.Data;
using DotSpatial.Data;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// A factory to create <see cref="FeatureSet"/> data from <see cref="MapData"/> which can be used on the map.
    /// </summary>
    public class MapDataFactory
    {
        private readonly IEnumerable<IMapDataConverter> converters = new Collection<IMapDataConverter>
        {
            new MapPointDataConverter()
        };
        /// <summary>
        /// Creates a new <see cref="FeatureSet"/> from the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="MapData"/> to base the creation of <see cref="FeatureSet"/> upon.</param>
        /// <returns>A new <see cref="FeatureSet"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the given <paramref name="data"/> type is not supported.</exception>
        public FeatureSet Create(MapData data)
        {
            foreach (var converter in converters)
            {
                if (converter.CanConvertMapData(data))
                {
                    return converter.Convert(data);
                }
            }

            throw new NotSupportedException(string.Format("MapData of type {0} is not supported.", data.GetType().Name));
        }
    }
}
