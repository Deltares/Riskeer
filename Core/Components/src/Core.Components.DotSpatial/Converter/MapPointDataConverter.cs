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

using System.Collections.Generic;
using Core.Components.DotSpatial.Data;
using DotSpatial.Data;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapPointData"/> into a <see cref="FeatureSet"/> containing one or more <see cref="Coordinate"/>.
    /// </summary>
    public class MapPointDataConverter : MapDataConverter<MapPointData>
    {
        protected override IList<FeatureSet> Convert(MapPointData data)
        {
            var featureSet = new FeatureSet(FeatureType.Point);

            foreach (var point in data.Points)
            {
                featureSet.Features.Add(new Coordinate(point.Item1, point.Item2));
            }

            return new List<FeatureSet> { featureSet };
        }
    }
}