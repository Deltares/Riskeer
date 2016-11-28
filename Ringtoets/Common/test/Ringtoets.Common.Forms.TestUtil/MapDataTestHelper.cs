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
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to test properties of a <see cref="MapData"/>.
    /// </summary>
    public static class MapDataTestHelper
    {
        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The sections that contains the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapLineData"/>.</item>
        /// <item>The number of sections and features in <see cref="MapData"/> are not the same.</item>
        /// <item>The points of a section and the geometry of a feature are not the same.</item>
        /// <item>The name of the <see cref="MapData"/> is not <c>"Vakindeling"</c>.</item>
        /// </list></exception>
        public static void AssertFailureMechanismSectionsMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            MapLineData sectionsMapLinesData = (MapLineData) mapData;
            MapFeature[] sectionMapLinesFeatures = sectionsMapLinesData.Features.ToArray();
            FailureMechanismSection[] sectionsArray = sections.ToArray();
            Assert.AreEqual(sectionsArray.Length, sectionMapLinesFeatures.Length);

            for (int index = 0; index < sectionsArray.Length; index++)
            {
                MapGeometry geometry = sectionMapLinesFeatures[index].MapGeometries.First();
                FailureMechanismSection failureMechanismSection = sectionsArray[index];
                CollectionAssert.AreEquivalent(failureMechanismSection.Points, geometry.PointCollections.First());
            }
            Assert.AreEqual("Vakindeling", mapData.Name);
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the <see cref="HydraulicBoundaryDatabase"/>
        /// </summary>
        /// <param name="database">The <see cref="HydraulicBoundaryDatabase"/> that contains the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapPointData"/>.</item>
        /// <item>The number of <see cref="HydraulicBoundaryDatabase.Locations"/> and features 
        /// in <see cref="MapData"/> are not the same.</item>
        /// <item>The points of a location and the geometry of a feature are not the same.</item>
        /// <item>The name of the <see cref="MapData"/> is not <c>"Hydraulische randvoorwaarden"</c>.</item>
        /// </list></exception>
        public static void AssertHydraulicBoundaryLocationsMapData(HydraulicBoundaryDatabase database, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            MapPointData hydraulicLocationsMapData = (MapPointData)mapData;
            if (database == null)
            {
                CollectionAssert.IsEmpty(hydraulicLocationsMapData.Features);
            }
            else
            {
                Assert.AreEqual(database.Locations.Count, hydraulicLocationsMapData.Features.Length);
                CollectionAssert.AreEqual(database.Locations.Select(hrp => hrp.Location),
                    hydraulicLocationsMapData.Features.SelectMany(f => f.MapGeometries.First().PointCollections.First()));
            }
            Assert.AreEqual("Hydraulische randvoorwaarden", mapData.Name);
        }
    }
}