// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;

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
        /// <param name="sections">The sections that contain the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapLineData"/>;</item>
        /// <item>the name of the <see cref="MapData"/> is not <c>"Vakindeling"</c>;</item>
        /// <item>the number of sections and features in <see cref="MapData"/> are not the same;</item>
        /// <item>the points of a section and the geometry of a corresponding feature are not the same.</item>
        /// </list>
        /// </exception>
        public static void AssertFailureMechanismSectionsMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            Assert.AreEqual("Vakindeling", mapData.Name);

            var sectionsMapLinesData = (MapLineData) mapData;
            MapFeature[] sectionMapLinesFeatures = sectionsMapLinesData.Features.ToArray();
            FailureMechanismSection[] sectionsArray = sections.ToArray();
            Assert.AreEqual(sectionsArray.Length, sectionMapLinesFeatures.Length);

            for (var index = 0; index < sectionsArray.Length; index++)
            {
                MapGeometry geometry = sectionMapLinesFeatures[index].MapGeometries.First();
                FailureMechanismSection failureMechanismSection = sectionsArray[index];
                CollectionAssert.AreEquivalent(failureMechanismSection.Points, geometry.PointCollections.First());
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the data of
        /// hydraulic boundary locations and calculations in <paramref name="assessmentSection"/>.
        /// </summary>
        /// <param name="assessmentSection">The assessment section that contains the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapPointData"/>;</item>
        /// <item>the number of hydraulic boundary locations and features in <see cref="MapData"/> are not the same;</item>
        /// <item>the general properties (such as id, name and location) of hydraulic boundary locations and features in
        /// <see cref="MapData"/> are not the same;</item>
        /// <item>the wave height or the design water level calculation results of a hydraulic boundary location and the
        /// respective outputs of a corresponding feature are not the same.</item>
        /// </list>
        /// </exception>
        public static void AssertHydraulicBoundaryLocationsMapData(IAssessmentSection assessmentSection, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            var hydraulicLocationsMapData = (MapPointData) mapData;

            MapFeaturesTestHelper.AssertHydraulicBoundaryFeaturesData(assessmentSection, hydraulicLocationsMapData.Features);
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the <paramref name="referenceLine"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line that contains the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapLineData"/>;</item>
        /// <item>the name of the <see cref="MapData"/> is not <c>"Referentielijn"</c>;</item>
        /// <item><paramref name="mapData"/> has features when <paramref name="referenceLine"/> is <c>null</c>;</item>
        /// <item><paramref name="mapData"/> has more than one feature;</item>
        /// <item>the points of the reference line and the geometry of the first feature are not the same.</item>
        /// </list>
        /// </exception>
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
                Assert.AreEqual(1, referenceLineData.Features.Count());
                CollectionAssert.AreEqual(referenceLine.Points, referenceLineData.Features.First().MapGeometries.First().PointCollections.First());
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the start points of the <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The sections that contain the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapPointData"/>;</item>
        /// <item>the name of the <see cref="MapData"/> is not <c>"Vakindeling (startpunten)"</c>;</item>
        /// <item><paramref name="mapData"/> has more than one feature;</item>
        /// <item>the start points of the sections and the geometry of the first feature are not the same.</item>
        /// </list>
        /// </exception>
        public static void AssertFailureMechanismSectionsStartPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Vakindeling (startpunten)", mapData.Name);

            var sectionsStartPointData = (MapPointData) mapData;
            Assert.AreEqual(1, sectionsStartPointData.Features.Count());
            CollectionAssert.AreEqual(sections.Select(s => s.StartPoint), sectionsStartPointData.Features.First().MapGeometries.First().PointCollections.First());
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the end points of the <paramref name="sections"/>.
        /// </summary>
        /// <param name="sections">The sections that contain the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapPointData"/>;</item>
        /// <item>the name of the <see cref="MapData"/> is not <c>"Vakindeling (eindpunten)"</c>;</item>
        /// <item><paramref name="mapData"/> has more than one feature;</item>
        /// <item>the end points of the sections and the geometry of the first feature are not the same.</item>
        /// </list>
        /// </exception>
        public static void AssertFailureMechanismSectionsEndPointMapData(IEnumerable<FailureMechanismSection> sections, MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Vakindeling (eindpunten)", mapData.Name);

            var sectionsEndPointData = (MapPointData) mapData;
            Assert.AreEqual(1, sectionsEndPointData.Features.Count());
            CollectionAssert.AreEqual(sections.Select(s => s.EndPoint), sectionsEndPointData.Features.First().MapGeometries.First().PointCollections.First());
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative for the <paramref name="foreshoreProfiles"/>.
        /// </summary>
        /// <param name="foreshoreProfiles">The foreshore profiles that contain the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not <see cref="MapLineData"/>;</item>
        /// <item>the name of the <see cref="MapData"/> is not <c>Voorlandprofielen</c>;</item>
        /// <item>the amount of features in <paramref name="mapData"/> is not equal to the length of the <paramref name="foreshoreProfiles"/>;</item>
        /// <item>the geometries of the features in <paramref name="mapData"/> are not equal to the expected geometry of the <paramref name="foreshoreProfiles"/>.</item>
        /// </list>
        /// </exception>
        public static void AssertForeshoreProfilesMapData(IEnumerable<ForeshoreProfile> foreshoreProfiles, MapData mapData)
        {
            Assert.IsInstanceOf<MapLineData>(mapData);
            Assert.AreEqual("Voorlandprofielen", mapData.Name);

            var foreshoreProfilesData = (MapLineData) mapData;
            ForeshoreProfile[] foreshoreProfileArray = foreshoreProfiles.ToArray();

            Assert.AreEqual(foreshoreProfileArray.Length, foreshoreProfilesData.Features.Count());

            for (var i = 0; i < foreshoreProfileArray.Length; i++)
            {
                IEnumerable<Point2D> expectedGeometry = GetWorldPoints(foreshoreProfileArray[i]);
                MapGeometry profileDataA = foreshoreProfilesData.Features.ElementAt(i).MapGeometries.First();
                CollectionAssert.AreEquivalent(expectedGeometry, profileDataA.PointCollections.First());
            }
        }

        /// <summary>
        /// Asserts whether the <see cref="ImageBasedMapData"/> contains the data that is representative for the <paramref name="backgroundData"/>.
        /// </summary>
        /// <param name="backgroundData">The <see cref="BackgroundData"/> that contains the original data.</param>
        /// <param name="imageBasedMapData">The <see cref="ImageBasedMapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="imageBasedMapData"/> is not of the expected type;</item>
        /// <item>one of the properties of <paramref name="imageBasedMapData"/> is not equal to <paramref name="backgroundData"/>.</item>
        /// </list>
        /// </exception>
        public static void AssertImageBasedMapData(BackgroundData backgroundData, ImageBasedMapData imageBasedMapData)
        {
            Assert.AreEqual(backgroundData.Name, imageBasedMapData.Name);
            Assert.AreEqual(backgroundData.IsVisible, imageBasedMapData.IsVisible);
            Assert.AreEqual(backgroundData.Transparency, imageBasedMapData.Transparency);

            var wmtsBackgroundDataConfiguration = backgroundData.Configuration as WmtsBackgroundDataConfiguration;
            if (wmtsBackgroundDataConfiguration != null)
            {
                Assert.IsInstanceOf<WmtsMapData>(imageBasedMapData);

                var wmtsMapData = (WmtsMapData) imageBasedMapData;
                Assert.AreEqual(wmtsBackgroundDataConfiguration.PreferredFormat, wmtsMapData.PreferredFormat);
                Assert.AreEqual(wmtsBackgroundDataConfiguration.SelectedCapabilityIdentifier, wmtsMapData.SelectedCapabilityIdentifier);
                Assert.AreEqual(wmtsBackgroundDataConfiguration.SourceCapabilitiesUrl, wmtsMapData.SourceCapabilitiesUrl);
                Assert.AreEqual(wmtsBackgroundDataConfiguration.IsConfigured, wmtsMapData.IsConfigured);

                return;
            }

            var wellKnownBackgroundDataConfiguration = backgroundData.Configuration as WellKnownBackgroundDataConfiguration;
            if (wellKnownBackgroundDataConfiguration != null)
            {
                Assert.IsInstanceOf<WellKnownTileSourceMapData>(imageBasedMapData);

                var wellKnownTileSourceMapData = (WellKnownTileSourceMapData) imageBasedMapData;
                Assert.AreEqual(wellKnownBackgroundDataConfiguration.WellKnownTileSource, (RingtoetsWellKnownTileSource) wellKnownTileSourceMapData.TileSource);
                Assert.IsTrue(wellKnownTileSourceMapData.IsConfigured);

                return;
            }

            Assert.Fail("Unsupported background configuration.");
        }

        /// <summary>
        /// Asserts whether the <see cref="MapData"/> contains the data that is representative 
        /// for the <paramref name="structures"/>.
        /// </summary>
        /// <param name="structures">The structures that contain the original data.</param>
        /// <param name="mapData">The <see cref="MapData"/> that needs to be asserted.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mapData"/> is not an instance of <see cref="MapPointData"/>;</item>
        /// <item><paramref name="structures"/> is <c>null</c>.</item>
        /// <item>the name of the <see cref="MapData"/> is not <c>Kunstwerken</c>;</item>
        /// <item>the amount of features in <paramref name="mapData"/> is not equal to the 
        /// amount of the <paramref name="structures"/>;</item>
        /// <item>the geometries of the features in <paramref name="mapData"/> are not equal to 
        /// the expected geometry of the <paramref name="structures"/>.</item>
        /// </list>
        /// </exception>
        public static void AssertStructuresMapData(IEnumerable<StructureBase> structures, MapData mapData)
        {
            Assert.NotNull(structures);
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Kunstwerken", mapData.Name);

            var structuresData = (MapPointData) mapData;
            StructureBase[] structuresArray = structures.ToArray();

            Assert.AreEqual(structuresArray.Length, structuresData.Features.Count());
            CollectionAssert.AreEqual(structuresArray.Select(hrp => hrp.Location),
                                      structuresData.Features.SelectMany(f => f.MapGeometries.First().PointCollections.First()));
        }

        private static IEnumerable<Point2D> GetWorldPoints(ForeshoreProfile foreshoreProfile)
        {
            return AdvancedMath2D.FromXToXY(
                foreshoreProfile.Geometry.Select(p => -p.X).ToArray(),
                foreshoreProfile.WorldReferencePoint,
                -foreshoreProfile.X0,
                foreshoreProfile.Orientation);
        }
    }
}