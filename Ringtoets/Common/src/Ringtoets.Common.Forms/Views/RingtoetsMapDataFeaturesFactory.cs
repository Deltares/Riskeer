// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Factory for creating arrays of <see cref="MapFeature"/> to use in <see cref="MapData"/>
    /// (created via <see cref="RingtoetsMapDataFactory"/>).
    /// </summary>
    public static class RingtoetsMapDataFeaturesFactory
    {
        /// <summary>
        /// Create reference line features based on the provided <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="referenceLine">The <see cref="ReferenceLine"/> to create the reference line features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="referenceLine"/> is <c>null</c>.</returns>
        public static MapFeature[] CreateReferenceLineFeatures(ReferenceLine referenceLine)
        {
            return referenceLine != null
                       ? GetMapFeatures(referenceLine.Points)
                       : new MapFeature[0];
        }

        /// <summary>
        /// Create hydraulic boundary database location features based on the provided <paramref name="hydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> to create the location features for.</param>
        /// <returns>An array of features or an empty array when <paramref name="hydraulicBoundaryDatabase"/> is <c>null</c>.</returns>
        public static MapFeature[] CreateHydraulicBoundaryDatabaseFeatures(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            return hydraulicBoundaryDatabase != null
                       ? GetMapFeatures(hydraulicBoundaryDatabase.Locations.Select(l => l.Location))
                       : new MapFeature[0];
        }

        private static MapFeature[] GetMapFeatures(IEnumerable<Point2D> points)
        {
            return new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        points
                    })
                })
            };
        }
    }
}