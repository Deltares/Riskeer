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
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateSurfaceLineFeatures_SurfaceLinesNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateSurfaceLineFeatures_NoSurfaceLines_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(Enumerable.Empty<RingtoetsPipingSurfaceLine>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateSurfaceLineFeatures_GivenSurfaceLines_ReturnsSurfaceLineFeaturesArray()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.0, 6.0)
            };
            var pointsTwo = new[]
            {
                new Point3D(3.2, 23.3, 34.2),
                new Point3D(7.7, 12.6, 1.2)
            };
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine()
            };
            surfaceLines[0].SetGeometry(pointsOne);
            surfaceLines[1].SetGeometry(pointsTwo);

            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(surfaceLines);

            // Assert
            Assert.AreEqual(1, features.Length);
            Assert.AreEqual(2, features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, features.ElementAt(0).MapGeometries.ElementAt(0));
            AssertEqualPointCollections(pointsTwo, features.ElementAt(0).MapGeometries.ElementAt(1));
        }

        [Test]
        public void CreateStochasticSoilModelFeatures_StochasticSoilModelsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateStochasticSoilModelFeatures_NoStochasticSoilModels_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(Enumerable.Empty<StochasticSoilModel>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateStochasticSoilModelFeatures_GivenStochasticSoilModels_ReturnsStochasticSoilModelFeaturesArray()
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
            var stochasticSoilModels = new[]
            {
                new StochasticSoilModel(1, "StochasticSoilModelName1", "StochasticSoilModelSegmentName1"),
                new StochasticSoilModel(2, "StochasticSoilModelName2", "StochasticSoilModelSegmentName2")
            };
            stochasticSoilModels[0].Geometry.AddRange(pointsOne);
            stochasticSoilModels[1].Geometry.AddRange(pointsTwo);

            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(stochasticSoilModels);

            // Assert
            Assert.AreEqual(1, features.Length);
            Assert.AreEqual(2, features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, features.ElementAt(0).MapGeometries.ElementAt(0));
            AssertEqualPointCollections(pointsTwo, features.ElementAt(0).MapGeometries.ElementAt(1));
        }

        [Test]
        public void CreateFailureMechanismSectionFeatures_SectionsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionFeatures_NoSections_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(Enumerable.Empty<FailureMechanismSection>());

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
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionFeatures(sections);

            // Assert
            Assert.AreEqual(1, features.Length);
            Assert.AreEqual(2, features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, features.ElementAt(0).MapGeometries.ElementAt(0));
            AssertEqualPointCollections(pointsTwo, features.ElementAt(0).MapGeometries.ElementAt(1));
        }

        [Test]
        public void CreateFailureMechanismSectionStartPointFeatures_SectionsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionStartPointFeatures_NoSections_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(Enumerable.Empty<FailureMechanismSection>());

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
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionStartPointFeatures(sections);

            // Assert
            Assert.AreEqual(1, features.Length);
            Assert.AreEqual(1, features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                pointsOne[0],
                pointsTwo[0]
            }, features.ElementAt(0).MapGeometries.ElementAt(0));
        }

        [Test]
        public void CreateFailureMechanismSectionEndPointFeatures_SectionsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateFailureMechanismSectionEndPointFeatures_NoSections_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(Enumerable.Empty<FailureMechanismSection>());

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
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateFailureMechanismSectionEndPointFeatures(sections);

            // Assert
            Assert.AreEqual(1, features.Length);
            Assert.AreEqual(1, features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                pointsOne[1],
                pointsTwo[1]
            }, features.ElementAt(0).MapGeometries.ElementAt(0));
        }

        private static void AssertEqualPointCollections(IEnumerable<Point3D> points, MapGeometry geometry)
        {
            AssertEqualPointCollections(points.Select(p => new Point2D(p.X, p.Y)), geometry);
        }

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points.Select(p => new Point2D(p.X, p.Y)), geometry.PointCollections.First());
        }
    }
}