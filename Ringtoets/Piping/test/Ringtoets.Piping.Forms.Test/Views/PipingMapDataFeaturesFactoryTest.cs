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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
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
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(new RingtoetsPipingSurfaceLine[0]);

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
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Surfaceline 1"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Surfaceline 2"
                }
            };
            surfaceLines[0].SetGeometry(pointsOne);
            surfaceLines[1].SetGeometry(pointsTwo);

            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(surfaceLines);

            // Assert
            Assert.AreEqual(surfaceLines.Length, features.Length);

            for (var i = 0; i < features.Length; i++)
            {
                Assert.AreEqual(1, features[i].MapGeometries.Count());
                AssertEqualPointCollections(surfaceLines[i].Points, features[i].MapGeometries.First());
                Assert.AreEqual(1, features[i].MetaData.Keys.Count);
                Assert.AreEqual(surfaceLines[i].Name, features[i].MetaData["Naam"]);
            }
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
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(new StochasticSoilModel[0]);

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
            Assert.AreEqual(stochasticSoilModels.Length, features.Length);

            for (var i = 0; i < features.Length; i++)
            {
                Assert.AreEqual(1, features[i].MapGeometries.Count());
                AssertEqualPointCollections(stochasticSoilModels[i].Geometry, features[i].MapGeometries.First());
                Assert.AreEqual(1, features[i].MetaData.Keys.Count);
                Assert.AreEqual(stochasticSoilModels[i].Name, features[i].MetaData["Naam"]);
            }
        }

        [Test]
        public void CreateCalculationFeatures_CalculationsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_NoCalculations_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(Enumerable.Empty<PipingCalculationScenario>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_GivenCalculations_ReturnsCalculationFeaturesArray()
        {
            // Setup
            PipingCalculationScenario calculationA = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();
            PipingCalculationScenario calculationB = PipingCalculationScenarioFactory.CreatePipingCalculationScenarioWithValidInput();

            calculationA.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(1.0, 3.0);
            calculationB.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(1.0, 4.0);

            calculationA.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 5.0, 4.0);
            calculationB.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 2.2, 3.8);

            // Call
            MapFeature[] features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(new[]
            {
                calculationA,
                calculationB
            });

            // Assert
            Assert.AreEqual(2, features.Length);
            Assert.AreEqual(1, features[0].MapGeometries.Count());
            Assert.AreEqual(1, features[1].MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                new Point2D(1.0, 3.0),
                new Point2D(5.0, 4.0)
            }, features[0].MapGeometries.ElementAt(0));
            AssertEqualPointCollections(new[]
            {
                new Point2D(1.0, 4.0),
                new Point2D(2.2, 3.8)
            }, features[1].MapGeometries.ElementAt(0));
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