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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Components.DotSpatial.Data;
using DotSpatial.Data;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The converter that converts <see cref="MapPolygonData"/> into <see cref="FeatureSet"/> containing <see cref="Polygon"/>.
    /// </summary>
    public class MapPolygonDataConverter : MapDataConverter<MapPolygonData>
    {
        protected override IList<FeatureSet> Convert(MapPolygonData data)
        {
            var featureSet = new FeatureSet(FeatureType.Polygon);

            var points = data.Points.ToArray();

            var coordinates = new Collection<Coordinate>();
            for (int i = 0; i < points.Length; i++)
            {
                coordinates.Add(new Coordinate(points[i].Item1, points[i].Item2));
            }
            var polygon = new Polygon(coordinates);
            featureSet.Features.Add(polygon);

            return new List<FeatureSet> { featureSet };
        }
    }
}
