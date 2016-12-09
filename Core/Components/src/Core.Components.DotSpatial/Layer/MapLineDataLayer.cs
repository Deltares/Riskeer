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
using Core.Components.DotSpatial.Converter;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Controls;

namespace Core.Components.DotSpatial.Layer
{
    /// <summary>
    /// A <see cref="MapLineLayer"/> based on and updated according to the wrapped <see cref="MapLineData"/>.
    /// </summary>
    public class MapLineDataLayer : MapLineLayer, IFeatureBasedMapDataLayer
    {
        private readonly MapLineData mapLineData;
        private readonly MapLineDataConverter converter = new MapLineDataConverter();

        private MapFeature[] drawnFeatures;

        /// <summary>
        /// Creates a new instance of <see cref="MapLineDataLayer"/>.
        /// </summary>
        /// <param name="mapLineData">The <see cref="MapLineData"/> which the map line data layer is based upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapLineData"/> is <c>null</c>.</exception>
        public MapLineDataLayer(MapLineData mapLineData)
        {
            if (mapLineData == null)
            {
                throw new ArgumentNullException("mapLineData");
            }

            this.mapLineData = mapLineData;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(mapLineData.Features, drawnFeatures))
            {
                converter.ConvertLayerFeatures(mapLineData, this);

                DataSet.InitializeVertices();
                DataSet.UpdateExtent();
                AssignFastDrawnStates();

                drawnFeatures = mapLineData.Features;
            }

            converter.ConvertLayerProperties(mapLineData, this);
        }
    }
}