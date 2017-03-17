﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;

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
        /// <item>The name of the <see cref="MapData"/> is not <c>"Vakindeling"</c>.</item>
        /// <item>The number of sections and features in <see cref="MapData"/> are not the same.</item>
        /// <item>The points of a section and the geometry of a corresponding feature are not the same.</item>        
        /// </list></exception>
        public static void AssertFailureMechanismSectionsMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            Assert.AreEqual("Vakindeling", mapData.Name);

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
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations that contain the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapPointData"/>.</item>
        /// <item>The name of the <see cref="MapData"/> is not <c>"Hydraulische randvoorwaarden"</c>.</item>
        /// <item><paramref name="mapData"/> has features when <paramref name="hydraulicBoundaryLocations"/> is <c>null</c>.</item>
        /// <item>The number of hydraulic boundary locations and features in <see cref="MapData"/> are not the same.</item>
        /// <item>The point of a hydraulic boundary location and the geometry of a corresponding feature are not the same.</item>        
        /// </list></exception>
        public static void AssertHydraulicBoundaryLocationsMapData(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Hydraulische randvoorwaarden", mapData.Name);

            MapPointData hydraulicLocationsMapData = (MapPointData) mapData;
            if (hydraulicBoundaryLocations == null)
            {
                CollectionAssert.IsEmpty(hydraulicLocationsMapData.Features);
            }
            else
            {
                var hydraulicBoundaryLocationsArray = hydraulicBoundaryLocations.ToArray();

                Assert.AreEqual(hydraulicBoundaryLocationsArray.Length, hydraulicLocationsMapData.Features.Length);
                CollectionAssert.AreEqual(hydraulicBoundaryLocationsArray.Select(hrp => hrp.Location),
                                          hydraulicLocationsMapData.Features.SelectMany(f => f.MapGeometries.First().PointCollections.First()));
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line that contain the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapLineData"/>.</item>
        /// <item>The name of the <see cref="MapData"/> is not <c>"Referentielijn"</c>.</item>
        /// <item><paramref name="mapData"/> has features when <paramref name="referenceLine"/> is <c>null</c>.</item>
        /// <item><paramref name="mapData"/> has more than one feature.</item>
        /// <item>The points of the reference line and the geometry of the first feature are not the same.</item>
        /// </list></exception>
        public static void AssertReferenceLineMapData(ReferenceLine referenceLine, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            Assert.AreEqual("Referentielijn", mapData.Name);

            var referenceLineData = (MapLineData) mapData;
            if (referenceLine == null)
            {
                CollectionAssert.IsEmpty(referenceLineData.Features);
            }
            else
            {
                Assert.AreEqual(1, referenceLineData.Features.Length);
                CollectionAssert.AreEqual(referenceLine.Points, referenceLineData.Features.First().MapGeometries.First().PointCollections.First());
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the start points of the <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The sections that contains the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapPointData"/>.</item>
        /// <item>The name of the <see cref="MapData"/> is not <c>"Vakindeling (startpunten)"</c>.</item>
        /// <item><paramref name="mapData"/> has more than one feature.</item>
        /// <item>The start points of the sections and the geometry of the first feature are not the same.</item>
        /// </list></exception>
        public static void AssertFailureMechanismSectionsStartPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Vakindeling (startpunten)", mapData.Name);

            var sectionsStartPointData = (MapPointData) mapData;
            Assert.AreEqual(1, sectionsStartPointData.Features.Length);
            CollectionAssert.AreEqual(sections.Select(s => s.GetStart()), sectionsStartPointData.Features.First().MapGeometries.First().PointCollections.First());
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the end points of the <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The sections that contains the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapPointData"/>.</item>
        /// <item>The name of the <see cref="MapData"/> is not <c>"Vakindeling (eindpunten)"</c>.</item>
        /// <item><paramref name="mapData"/> has more than one feature.</item>
        /// <item>The end points of the sections and the geometry of the first feature are not the same.</item>
        /// </list></exception>
        public static void AssertFailureMechanismSectionsEndPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Vakindeling (eindpunten)", mapData.Name);

            var sectionsEndPointData = (MapPointData) mapData;
            Assert.AreEqual(1, sectionsEndPointData.Features.Length);
            CollectionAssert.AreEqual(sections.Select(s => s.GetLast()), sectionsEndPointData.Features.First().MapGeometries.First().PointCollections.First());
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the <paramref name="foreshoreProfiles"/>.
        /// </summary>
        /// <param name="foreshoreProfiles">The foreshore profiles that contains the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapLineData"/>.</item>
        /// <item>The name of the <see cref="MapData"/> is not <c>Voorlandprofielen</c>.</item>
        /// <item>The amount of features in <paramref name="mapData"/> is not equal to the length of the <paramref name="foreshoreProfiles"/>.</item>
        /// <item>The geometries of the features in <paramref name="mapData"/> are not equal to the expected geometry of the <paramref name="foreshoreProfiles"/>.</item>
        /// </list></exception>
        public static void AssertForeshoreProfilesMapData(IEnumerable<ForeshoreProfile> foreshoreProfiles, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            Assert.AreEqual("Voorlandprofielen", mapData.Name);

            MapLineData foreshoreProfilesData = (MapLineData) mapData;
            ForeshoreProfile[] foreshoreProfileArray = foreshoreProfiles.ToArray();

            Assert.AreEqual(foreshoreProfileArray.Length, foreshoreProfilesData.Features.Length);

            for (int i = 0; i < foreshoreProfileArray.Length; i++)
            {
                Point2D[] expectedGeometry = GetWorldPoints(foreshoreProfileArray[i]);
                MapGeometry profileDataA = foreshoreProfilesData.Features[i].MapGeometries.First();
                CollectionAssert.AreEquivalent(expectedGeometry, profileDataA.PointCollections.First());
            }
        }

        /// <summary>
        /// Asserts whether the <paramref name="mapData"/> is equivalent to <paramref name="wmtsMapData"/>
        /// </summary>
        /// <param name="wmtsMapData">The original WMTS map data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="WmtsMapData"/></item>
        /// <item>One of the properties of <paramref name="wmtsMapData"/> is not equal to <paramref name="mapData"/></item>
        /// </list></exception>
        private static void AssertWmtsMapData(WmtsMapData wmtsMapData, MapData mapData)
        {
            Assert.IsInstanceOf<WmtsMapData>(mapData);
            Assert.IsNotNull(wmtsMapData);
            Assert.AreEqual(wmtsMapData.Name, mapData.Name);
            Assert.AreEqual(wmtsMapData.IsVisible, mapData.IsVisible);

            var actualMapData = (WmtsMapData) mapData;
            Assert.AreEqual(wmtsMapData.PreferredFormat, actualMapData.PreferredFormat);
            Assert.AreEqual(wmtsMapData.SelectedCapabilityIdentifier, actualMapData.SelectedCapabilityIdentifier);
            Assert.AreEqual(wmtsMapData.SourceCapabilitiesUrl, actualMapData.SourceCapabilitiesUrl);
            Assert.AreEqual(wmtsMapData.IsConfigured, actualMapData.IsConfigured);
            Assert.AreEqual(wmtsMapData.Transparency, actualMapData.Transparency);
        }

        /// <summary>
        /// Asserts whether the <paramref name="mapData"/> is equivalent to <paramref name="imageBasedMapData"/>
        /// </summary>
        /// <param name="imageBasedMapData">The original image based map data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is no <see cref="ImageBasedMapData"/>;</item>
        /// <item>One of the properties of <paramref name="imageBasedMapData"/> is not equal to <paramref name="mapData"/>.</item>
        /// </list></exception>
        public static void AssertImageBasedMapData(ImageBasedMapData imageBasedMapData, MapData mapData)
        {
            var wmtsMapData = imageBasedMapData as WmtsMapData;
            if (wmtsMapData != null)
            {
                AssertWmtsMapData(wmtsMapData, mapData);
                return;
            }

            var wellKnownTileSourceMapData = imageBasedMapData as WellKnownTileSourceMapData;
            if (wellKnownTileSourceMapData != null)
            {
                AssertWellKnownTileSourceMapData(wellKnownTileSourceMapData, mapData);
                return;
            }
            Assert.IsInstanceOf<ImageBasedMapData>(imageBasedMapData);
            Assert.Fail($"unknown type of {nameof(ImageBasedMapData)}");
        }

        private static void AssertWellKnownTileSourceMapData(WellKnownTileSourceMapData wellKnownTileSourceMapData, MapData mapData)
        {
            Assert.IsInstanceOf<WellKnownTileSourceMapData>(mapData);
            Assert.IsNotNull(wellKnownTileSourceMapData);
            Assert.AreEqual(wellKnownTileSourceMapData.Name, mapData.Name);
            Assert.AreEqual(wellKnownTileSourceMapData.IsVisible, mapData.IsVisible);

            var actualMapData = (WellKnownTileSourceMapData) mapData;
            Assert.AreEqual(wellKnownTileSourceMapData.IsConfigured, actualMapData.IsConfigured);
            Assert.AreEqual(wellKnownTileSourceMapData.Transparency, actualMapData.Transparency);
            Assert.AreEqual(wellKnownTileSourceMapData.TileSource, actualMapData.TileSource);
        }

        private static Point2D[] GetWorldPoints(ForeshoreProfile foreshoreProfile)
        {
            return AdvancedMath2D.FromXToXY(
                foreshoreProfile.Geometry.Select(p => -p.X).ToArray(),
                foreshoreProfile.WorldReferencePoint,
                -foreshoreProfile.X0,
                foreshoreProfile.Orientation);
        }
    }
}