// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TestUtil;

namespace Ringtoets.Common.Forms.Test.Factories
{
    [TestFixture]
    public class RingtoetsMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateSingleLineMapFeature_PointsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsMapDataFeaturesFactory.CreateSingleLineMapFeature(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("points", paramName);
        }

        [Test]
        public void CreateSingleLineMapFeature_WithPoints_ReturnMapFeatureWithLineGeometry()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4)
            };

            // Call
            MapFeature feature = RingtoetsMapDataFeaturesFactory.CreateSingleLineMapFeature(points);

            // Assert
            Assert.AreEqual(1, feature.MapGeometries.Count());
            Assert.AreEqual(1, feature.MapGeometries.First().PointCollections.Count());
            CollectionAssert.AreEqual(points, feature.MapGeometries.First().PointCollections.First());

            CollectionAssert.IsEmpty(feature.MetaData);
        }

        [Test]
        public void CreateReferenceLineFeatures_ReferenceLineNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(null, string.Empty, string.Empty);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateReferenceLineFeatures_GivenReferenceLine_ReturnsReferenceLineFeature()
        {
            // Setup
            const string id = "1";
            const string name = "Traject 1";

            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(points);

            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateReferenceLineFeatures(referenceLine, id, name);

            // Assert
            MapFeature mapFeature = features.Single();
            Assert.AreEqual(3, mapFeature.MetaData.Keys.Count);
            Assert.AreEqual(id, mapFeature.MetaData["ID"]);
            Assert.AreEqual(name, mapFeature.MetaData["Naam"]);

            var expectedLength = new RoundedDouble(2, Math2D.Length(points));
            Assert.AreEqual(expectedLength, (RoundedDouble) mapFeature.MetaData["Lengte*"], expectedLength.GetAccuracy());
            AssertEqualPointCollections(points, mapFeature.MapGeometries.ElementAt(0));
        }

        [Test]
        public void CreateHydraulicBoundaryLocationFeatures_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationFeatures_NoLocations_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(
                new AssessmentSectionStub());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateHydraulicBoundaryLocationFeatures_GivenLocations_ReturnsLocationFeaturesCollection(bool setOutput)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "location1", 1, 1),
                new HydraulicBoundaryLocation(2, "location2", 2, 2)
            }, setOutput);

            // Call
            MapFeature[] features = RingtoetsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(assessmentSection).ToArray();

            // Assert
            MapFeaturesTestHelper.AssertHydraulicBoundaryFeaturesData(assessmentSection, features);
        }

        [Test]
        public void CreateFailureMechanismSectionFeatures_SectionsNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionFeatures_NoSections_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(
                Enumerable.Empty<FailureMechanismSection>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionFeatures_GivenSections_ReturnsSectionFeaturesCollection()
        {
            // Setup
            const string sectionName1 = "section 1";
            const string sectionName2 = "section 2";

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
                new FailureMechanismSection(sectionName1, pointsOne),
                new FailureMechanismSection(sectionName2, pointsTwo)
            };

            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(sections);

            // Assert
            Assert.AreEqual(2, features.Count());
            for (var i = 0; i < features.Count(); i++)
            {
                Assert.AreEqual(1, features.ElementAt(i).MapGeometries.Count());
                Assert.AreEqual(2, features.ElementAt(i).MetaData.Keys.Count);

                Assert.AreEqual(sections[i].Name, features.ElementAt(i).MetaData["Naam"]);
                var expectedLength = new RoundedDouble(2, Math2D.Length(sections[i].Points));
                Assert.AreEqual(expectedLength, (RoundedDouble) features.ElementAt(i).MetaData["Lengte*"], expectedLength.GetAccuracy());

                AssertEqualPointCollections(sections[i].Points, features.ElementAt(i).MapGeometries.First());
            }
        }

        [Test]
        public void CreateFailureMechanismSectionStartPointFeatures_SectionsNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionStartPointFeatures_NoSections_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(Enumerable.Empty<FailureMechanismSection>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionStartPointFeatures_GivenSections_ReturnsSectionBeginPointFeaturesCollection()
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
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(sections);

            // Assert
            Assert.AreEqual(1, features.Count());
            Assert.AreEqual(1, features.First().MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                pointsOne[0],
                pointsTwo[0]
            }, features.First().MapGeometries.ElementAt(0));
        }

        [Test]
        public void CreateFailureMechanismSectionEndPointFeatures_SectionsNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionEndPointFeatures_NoSections_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(Enumerable.Empty<FailureMechanismSection>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionEndPointFeatures_GivenSections_ReturnsSectionEndPointFeaturesCollection()
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
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(sections);

            // Assert
            Assert.AreEqual(1, features.Count());
            Assert.AreEqual(1, features.First().MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                pointsOne[1],
                pointsTwo[1]
            }, features.First().MapGeometries.ElementAt(0));
        }

        [Test]
        public void CreateStructuresFeatures_NullStructures_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateStructuresFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateStructuresFeatures_EmptyStructures_ReturnsEmptyCollection()
        {
            // Setup
            IEnumerable<StructureBase> structures = Enumerable.Empty<StructureBase>();

            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateStructuresFeatures(structures);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateStructuresFeatures_WithStructures_ReturnsCollectionWithFeatures()
        {
            // Setup
            var structure1 = new TestStructure("id", "A", new Point2D(1.1, 2.2));
            var structure2 = new TestStructure("id", "B", new Point2D(3.3, 4.4));

            TestStructure[] structures =
            {
                structure1,
                structure2
            };

            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateStructuresFeatures(structures);

            // Assert
            Assert.AreEqual(2, features.Count());
            Point2D locationOne = features.First().MapGeometries.Single().PointCollections.First().Single();
            Point2D locationTwo = features.ElementAt(1).MapGeometries.Single().PointCollections.First().Single();
            Assert.AreEqual(structure1.Location, locationOne);
            Assert.AreEqual(structure2.Location, locationTwo);

            const int expectedNumberOfMetaDataOptions = 1;
            Assert.AreEqual(expectedNumberOfMetaDataOptions, features.First().MetaData.Count);
            Assert.AreEqual(expectedNumberOfMetaDataOptions, features.ElementAt(1).MetaData.Count);
            Assert.AreEqual(structure1.Name, features.First().MetaData["Naam"]);
            Assert.AreEqual(structure2.Name, features.ElementAt(1).MetaData["Naam"]);
        }

        [Test]
        public void CreateStructureCalculationsFeatures_NullLocations_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateStructureCalculationsFeatures<
                SimpleStructuresInput, StructureBase>(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateStructureCalculationsFeatures_EmptyLocations_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateStructureCalculationsFeatures
                <SimpleStructuresInput, StructureBase>(Enumerable.Empty<SimpleStructuresCalculation>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateStructureCalculationsFeatures_WithCalculations_ReturnsCollectionWithCalculations()
        {
            // Setup
            var calculationLocationA = new Point2D(1.2, 2.3);
            var calculationLocationB = new Point2D(2.7, 2.0);

            var hydraulicBoundaryLocationA = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3);
            var hydraulicBoundaryLocationB = new HydraulicBoundaryLocation(1, string.Empty, 7.7, 12.6);

            var simpleStructuresCalculationA = new SimpleStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocationA,
                    Structure = new TestStructure(calculationLocationA)
                }
            };

            var simpleStructuresCalculationB = new SimpleStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocationB,
                    Structure = new TestStructure(calculationLocationB)
                }
            };

            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateStructureCalculationsFeatures
                <SimpleStructuresInput, StructureBase>(new[]
                {
                    simpleStructuresCalculationA,
                    simpleStructuresCalculationB
                });

            // Assert
            Assert.AreEqual(2, features.Count());
            Assert.AreEqual(1, features.First().MapGeometries.Count());
            Assert.AreEqual(1, features.ElementAt(1).MapGeometries.Count());
            Point2D[] mapDataGeometryOne = features.First().MapGeometries.ElementAt(0).PointCollections.First().ToArray();
            Point2D[] mapDataGeometryTwo = features.ElementAt(1).MapGeometries.ElementAt(0).PointCollections.First().ToArray();

            CollectionElementsAlmostEquals(new[]
            {
                calculationLocationA,
                hydraulicBoundaryLocationA.Location
            }, mapDataGeometryOne);
            CollectionElementsAlmostEquals(new[]
            {
                calculationLocationB,
                hydraulicBoundaryLocationB.Location
            }, mapDataGeometryTwo);
        }

        [Test]
        public void CreateCalculationsFeatures_NullLocations_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateCalculationFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationsFeatures_EmptyLocations_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateCalculationFeatures(
                new MapCalculationData[0]);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationsFeatures_WithCalculations_ReturnsCollectionWithCalculations()
        {
            // Setup
            var calculationLocationA = new Point2D(1.2, 2.3);
            var calculationLocationB = new Point2D(2.7, 2.0);

            var hydraulicBoundaryLocationA = new HydraulicBoundaryLocation(1, string.Empty, 1.3, 2.3);
            var hydraulicBoundaryLocationB = new HydraulicBoundaryLocation(1, string.Empty, 7.7, 12.6);

            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateCalculationFeatures(new[]
            {
                new MapCalculationData("calculationA", calculationLocationA, hydraulicBoundaryLocationA),
                new MapCalculationData("calculationB", calculationLocationB, hydraulicBoundaryLocationB)
            });

            // Assert
            Assert.AreEqual(2, features.Count());
            Assert.AreEqual(1, features.First().MapGeometries.Count());
            Assert.AreEqual(1, features.ElementAt(1).MapGeometries.Count());
            Point2D[] mapDataGeometryOne = features.First().MapGeometries.ElementAt(0).PointCollections.First().ToArray();
            Point2D[] mapDataGeometryTwo = features.ElementAt(1).MapGeometries.ElementAt(0).PointCollections.First().ToArray();

            CollectionElementsAlmostEquals(new[]
            {
                calculationLocationA,
                hydraulicBoundaryLocationA.Location
            }, mapDataGeometryOne);
            CollectionElementsAlmostEquals(new[]
            {
                calculationLocationB,
                hydraulicBoundaryLocationB.Location
            }, mapDataGeometryTwo);
        }

        [Test]
        public void CreateDikeProfilesFeatures_NullDikeProfiles_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateDikeProfilesFeatures_EmptyDikeProfiles_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(
                Enumerable.Empty<DikeProfile>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateDikeProfilesFeatures_WithDikeProfiles_ReturnsCollectionWithDikeProfiles()
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
            Point2D[] pointsOne =
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
                new DikeProfile(new Point2D(5, 4), roughnessPointsOne, pointsOne, null, new DikeProfile.ConstructionProperties
                {
                    Id = "A"
                }),
                new DikeProfile(new Point2D(2, 1), roughnessPointsTwo, Enumerable.Empty<Point2D>(), null, new DikeProfile.ConstructionProperties
                {
                    Id = "bid",
                    Name = "B"
                })
            };

            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateDikeProfilesFeatures(dikeProfiles);

            // Assert
            Assert.AreEqual(2, features.Count());
            Assert.AreEqual(1, features.First().MapGeometries.Count());
            Assert.AreEqual(1, features.ElementAt(1).MapGeometries.Count());
            Point2D[] mapDataDikeGeometryOne = features.First().MapGeometries.ElementAt(0).PointCollections.First().ToArray();
            Point2D[] mapDataDikeGeometryTwo = features.ElementAt(1).MapGeometries.ElementAt(0).PointCollections.First().ToArray();

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

            const int expectedNumberOfMetaDataOptions = 1;
            Assert.AreEqual(expectedNumberOfMetaDataOptions, features.First().MetaData.Count);
            Assert.AreEqual(expectedNumberOfMetaDataOptions, features.ElementAt(1).MetaData.Count);
            Assert.AreEqual(dikeProfiles[0].Name, features.First().MetaData["Naam"]);
            Assert.AreEqual(dikeProfiles[1].Name, features.ElementAt(1).MetaData["Naam"]);
        }

        [Test]
        public void CreateForeshoreProfilesFeatures_NullForeshoreProfiles_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateForeshoreProfilesFeatures_EmptyForeshoreProfiles_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(
                Enumerable.Empty<ForeshoreProfile>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateForeshoreProfilesFeatures_WithForeshoreProfiles_ReturnsCollectionForeshoreProfilesWithGeometry()
        {
            // Setup
            var pointA = new Point2D(1.2, 2.3);
            var pointB = new Point2D(2.7, 2.0);

            var pointC = new Point2D(1.3, 2.3);
            var pointD = new Point2D(4.6, 2.0);
            var pointE = new Point2D(3.2, 23.3);
            var pointF = new Point2D(7.7, 12.6);

            Point2D[] pointsOne =
            {
                pointA,
                pointB
            };
            Point2D[] pointsTwo =
            {
                pointC,
                pointD,
                pointE,
                pointF
            };
            var foreshoreProfiles = new[]
            {
                new ForeshoreProfile(new Point2D(5, 4), pointsOne, null, new ForeshoreProfile.ConstructionProperties
                {
                    Id = "A"
                }),
                new ForeshoreProfile(new Point2D(3, 3), Enumerable.Empty<Point2D>(), null, new ForeshoreProfile.ConstructionProperties
                {
                    Id = "bid",
                    Name = "B"
                }),
                new ForeshoreProfile(new Point2D(2, 1), pointsTwo, null, new ForeshoreProfile.ConstructionProperties
                {
                    Id = "cid",
                    Name = "C"
                })
            };

            // Call
            IEnumerable<MapFeature> features = RingtoetsMapDataFeaturesFactory.CreateForeshoreProfilesFeatures(foreshoreProfiles);

            // Assert
            Assert.AreEqual(2, features.Count());
            Assert.AreEqual(1, features.First().MapGeometries.Count());
            Assert.AreEqual(1, features.ElementAt(1).MapGeometries.Count());
            Point2D[] mapDataGeometryOne = features.First().MapGeometries.ElementAt(0).PointCollections.First().ToArray();
            Point2D[] mapDataGeometryTwo = features.ElementAt(1).MapGeometries.ElementAt(0).PointCollections.First().ToArray();

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

            const int expectedNumberOfMetaDataOptions = 1;
            Assert.AreEqual(expectedNumberOfMetaDataOptions, features.First().MetaData.Count);
            Assert.AreEqual(expectedNumberOfMetaDataOptions, features.ElementAt(1).MetaData.Count);
            Assert.AreEqual(foreshoreProfiles[0].Name, features.First().MetaData["Naam"]);
            Assert.AreEqual(foreshoreProfiles[2].Name, features.ElementAt(1).MetaData["Naam"]);
        }

        private static void CollectionElementsAlmostEquals(IEnumerable<Point2D> expected, Point2D[] actual)
        {
            Assert.AreEqual(expected.Count(), actual.Length);

            for (var index = 0; index < actual.Length; index++)
            {
                Point2D actualPoint = actual[index];
                Point2D expectedPoint = expected.ElementAt(index);

                const double delta = 1e-8;
                Assert.AreEqual(expectedPoint.X, actualPoint.X, delta);
                Assert.AreEqual(expectedPoint.Y, actualPoint.Y, delta);
            }
        }

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points, geometry.PointCollections.First());
        }

        private class SimpleStructuresInput : StructuresInputBase<StructureBase>
        {
            public override bool IsStructureInputSynchronized
            {
                get
                {
                    return false;
                }
            }

            public override void SynchronizeStructureInput() {}
        }

        private class SimpleStructuresCalculation : StructuresCalculation<SimpleStructuresInput> {}
    }
}