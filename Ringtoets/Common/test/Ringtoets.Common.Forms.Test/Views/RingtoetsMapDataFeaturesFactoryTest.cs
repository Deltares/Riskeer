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
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.Views;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class RingtoetsMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateReferenceLineFeatures_ReferenceLineNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateReferenceLineFeatures_GivenReferenceLine_ReturnsReferenceLineFeaturesArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine);

            // Assert
            AssertEqualPointCollections(points, features[0].MapGeometries.ElementAt(0));
        }

        [Test]
        public void CreateHydraulicBoundaryDatabaseFeaturesWithDefaultLabels_HydraulicBoundaryDatabaseNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeaturesWithDefaultLabels(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateHydraulicBoundaryDatabaseFeaturesWithDefaultLabels_GivenHydraulicBoundaryDatabase_ReturnsLocationFeaturesArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.AddRange(points.Select(p => new HydraulicBoundaryLocation(0, "", p.X, p.Y)));

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeaturesWithDefaultLabels(hydraulicBoundaryDatabase);

            // Assert
            var hydraulicBoundaryLocations = hydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(hydraulicBoundaryLocations.Count, features.Length);
            for (int i = 0; i < hydraulicBoundaryLocations.Count; i++)
            {
                Assert.AreEqual(4, features[i].MetaData.Keys.Count);
                Assert.AreEqual(hydraulicBoundaryLocations[i].Id, features[i].MetaData["ID"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].Name, features[i].MetaData["Naam"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].DesignWaterLevel, features[i].MetaData["Toetspeil"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].WaveHeight, features[i].MetaData["Golfhoogte"]);
            }

            AssertEqualFeatureCollections(points, features);
        }

        [Test]
        public void CreateHydraulicBoundaryDatabaseFeaturesWithOptionalLabels_HydraulicBoundaryDatabaseNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeaturesWithOptionalLabels(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateHydraulicBoundaryDatabaseFeaturesWithOptionalLabels_GivenHydraulicBoundaryDatabase_ReturnsLocationFeaturesArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            hydraulicBoundaryDatabase.Locations.AddRange(points.Select(p => new HydraulicBoundaryLocation(0, "", p.X, p.Y)));

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryDatabaseFeaturesWithOptionalLabels(hydraulicBoundaryDatabase);

            // Assert
            var hydraulicBoundaryLocations = hydraulicBoundaryDatabase.Locations;
            Assert.AreEqual(hydraulicBoundaryLocations.Count, features.Length);
            for (int i = 0; i < hydraulicBoundaryLocations.Count; i++)
            {
                Assert.AreEqual(4, features[i].MetaData.Keys.Count);
                Assert.AreEqual(hydraulicBoundaryLocations[i].Id, features[i].MetaData["ID"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].Name, features[i].MetaData["Naam"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].DesignWaterLevel, features[i].MetaData["Waterstand bij doorsnede-eis"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].WaveHeight, features[i].MetaData["Golfhoogte bij doorsnede-eis"]);
            }

            AssertEqualFeatureCollections(points, features);
        }

        [Test]
        public void CreateFailureMechanismSectionFeatures_SectionsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionFeatures_NoSections_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(Enumerable.Empty<FailureMechanismSection>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionFeatures_GivenSections_ReturnsSectionFeaturesArray()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var pointsTwo = new[]
            {
                new Point2D(3.2, 23.3),
                new Point2D(7.7, 12.6)
            };
            var sections = new[]
            {
                new FailureMechanismSection(string.Empty, pointsOne),
                new FailureMechanismSection(string.Empty, pointsTwo)
            };

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(sections);

            // Assert
            Assert.AreEqual(1, features.Length);
            Assert.AreEqual(2, features[0].MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, features[0].MapGeometries.ElementAt(0));
            AssertEqualPointCollections(pointsTwo, features[0].MapGeometries.ElementAt(1));
        }

        [Test]
        public void CreateFailureMechanismSectionStartPointFeatures_SectionsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionStartPointFeatures_NoSections_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(Enumerable.Empty<FailureMechanismSection>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionStartPointFeatures_GivenSections_ReturnsSectionBeginPointFeaturesArray()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var pointsTwo = new[]
            {
                new Point2D(3.2, 23.3),
                new Point2D(7.7, 12.6)
            };
            var sections = new[]
            {
                new FailureMechanismSection(string.Empty, pointsOne),
                new FailureMechanismSection(string.Empty, pointsTwo)
            };

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(sections);

            // Assert
            Assert.AreEqual(1, features.Length);
            Assert.AreEqual(1, features[0].MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                pointsOne[0],
                pointsTwo[0]
            }, features[0].MapGeometries.ElementAt(0));
        }

        [Test]
        public void CreateFailureMechanismSectionEndPointFeatures_SectionsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionEndPointFeatures_NoSections_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(Enumerable.Empty<FailureMechanismSection>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionEndPointFeatures_GivenSections_ReturnsSectionEndPointFeaturesArray()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var pointsTwo = new[]
            {
                new Point2D(3.2, 23.3),
                new Point2D(7.7, 12.6)
            };
            var sections = new[]
            {
                new FailureMechanismSection(string.Empty, pointsOne),
                new FailureMechanismSection(string.Empty, pointsTwo)
            };

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(sections);

            // Assert
            Assert.AreEqual(1, features.Length);
            Assert.AreEqual(1, features[0].MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                pointsOne[1],
                pointsTwo[1]
            }, features[0].MapGeometries.ElementAt(0));
        }

        private static void AssertEqualFeatureCollections(Point2D[] points, MapFeature[] features)
        {
            Assert.AreEqual(points.Length, features.Length);
            for (int i = 0; i < points.Length; i++)
            {
                CollectionAssert.AreEqual(new[] { points[i] }, features[i].MapGeometries.First().PointCollections.First());
            }
        }

        [Test]
        public void CreateDikeProfilesFeatures_NullDikeProfiles_ReturnsEmptyCollection()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(null);

            // Assert
            Assert.IsEmpty(features);
        }

        [Test]
        public void CreateDikeProfilesFeatures_EmptyDikeProfiles_ReturnsEmptyCollection()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(
                Enumerable.Empty<DikeProfile>());

            // Assert
            Assert.IsEmpty(features);
        }

        [Test]
        public void CreateDikeProfilesFeatures_WithDikeProfiles_ReturnsCollectionWithForeshoreProfileAndDikeProfile()
        {
            // Setup
            var pointA = new Point2D(1.2, 2.3);
            var pointB = new Point2D(2.7, 2.0);
            var pointC = new Point2D(3.2, 23.3);
            var pointD = new Point2D(7.7, 12.6);

            var pointE = new Point2D(1.3, 2.3);
            var pointF = new Point2D(4.6, 2.0);
            var pointG = new Point2D(6.3, 23.3);
            var pointH = new Point2D(4.2, 12.6);

            var roughnessPointsOne = new[]
            {
                new RoughnessPoint(pointA, 1),
                new RoughnessPoint(pointB, 2)
            };
            var pointsOne = new[]
            {
                pointC,
                pointD
            };
            var roughnessPointsTwo = new[]
            {
                new RoughnessPoint(pointE, 1),
                new RoughnessPoint(pointF, 2),
                new RoughnessPoint(pointG, 1),
                new RoughnessPoint(pointH, 2)
            };
            var dikeProfiles = new[]
            {
                new DikeProfile(new Point2D(5,4), roughnessPointsOne, pointsOne, null, new DikeProfile.ConstructionProperties()), 
                new DikeProfile(new Point2D(2,1), roughnessPointsTwo, Enumerable.Empty<Point2D>(), null, new DikeProfile.ConstructionProperties())
            };

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(dikeProfiles);

            // Assert
            Assert.AreEqual(2, features.Length);
            Assert.AreEqual(1, features[0].MapGeometries.Count());
            Assert.AreEqual(1, features[1].MapGeometries.Count());
            var mapDataDikeGeometryOne = features[0].MapGeometries.ElementAt(0).PointCollections.First().ToArray();
            var mapDataDikeGeometryTwo = features[1].MapGeometries.ElementAt(0).PointCollections.First().ToArray();

            CollectionElementsAlmostEquals(new[]
            {
                new Point2D(5, 2.8),
                new Point2D(5, 1.3)
            }, mapDataDikeGeometryOne);
            CollectionElementsAlmostEquals(new[]
            {
                new Point2D(2, -0.3),
                new Point2D(2, -3.6),
                new Point2D(2, -5.3),
                new Point2D(2, -3.2)
            }, mapDataDikeGeometryTwo);
        }
        [Test]
        public void CreateForeshoreProfilesFeatures_NullForeshoreProfiles_ReturnsEmptyCollection()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(null);

            // Assert
            Assert.IsEmpty(features);
        }

        [Test]
        public void CreateForeshoreProfilesFeatures_EmptyForeshoreProfiles_ReturnsEmptyCollection()
        {
            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(
                Enumerable.Empty<ForeshoreProfile>());

            // Assert
            Assert.IsEmpty(features);
        }

        [Test]
        public void CreateForeshoreProfilesFeatures_WithForeshoreProfiles_ReturnsCollectionWithForeshoreProfileAndForeshoreProfile()
        {
            // Setup
            var pointA = new Point2D(1.2, 2.3);
            var pointB = new Point2D(2.7, 2.0);

            var pointC = new Point2D(1.3, 2.3);
            var pointD = new Point2D(4.6, 2.0);
            var pointE = new Point2D(3.2, 23.3);
            var pointF = new Point2D(7.7, 12.6);

            var pointsOne = new[]
            {
                pointA,
                pointB
            };
            var pointsTwo = new[]
            {
                pointC,
                pointD,
                pointE,
                pointF
            };
            var dikeProfiles = new[]
            {
                new ForeshoreProfile(new Point2D(5,4), pointsOne, null, new ForeshoreProfile.ConstructionProperties()), 
                new ForeshoreProfile(new Point2D(2,1), pointsTwo, null, new ForeshoreProfile.ConstructionProperties())
            };

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(dikeProfiles);

            // Assert
            Assert.AreEqual(2, features.Length);
            Assert.AreEqual(1, features[0].MapGeometries.Count());
            Assert.AreEqual(1, features[1].MapGeometries.Count());
            var mapDataGeometryOne = features[0].MapGeometries.ElementAt(0).PointCollections.First().ToArray();
            var mapDataGeometryTwo = features[1].MapGeometries.ElementAt(0).PointCollections.First().ToArray();

            CollectionElementsAlmostEquals(new[]
            {
                new Point2D(5, 2.8),
                new Point2D(5, 1.3)
            }, mapDataGeometryOne);
            CollectionElementsAlmostEquals(new[]
            {
                new Point2D(2, -0.3),
                new Point2D(2, -3.6),
                new Point2D(2, -2.2),
                new Point2D(2, -6.7)
            }, mapDataGeometryTwo);
        }

        private void CollectionElementsAlmostEquals(IEnumerable<Point2D> expected, Point2D[] actual)
        {
            Assert.AreEqual(expected.Count(), actual.Length);

            for (int index = 0; index < actual.Length; index++)
            {
                var actualPoint = actual[index];
                var expectedPoint = expected.ElementAt(index);

                var delta = 1e-8;
                Assert.AreEqual(expectedPoint.X, actualPoint.X, delta);
                Assert.AreEqual(expectedPoint.Y, actualPoint.Y, delta);
            }
        }

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points, geometry.PointCollections.First());
        }
    }
}