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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.Factories;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Factories
{
    [TestFixture]
    public class PipingMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateSurfaceLineFeatures_SurfaceLinesNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateSurfaceLineFeatures_NoSurfaceLines_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(new PipingSurfaceLine[0]);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateSurfaceLineFeatures_GivenSurfaceLines_ReturnsSurfaceLineFeaturesCollection()
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
                new PipingSurfaceLine("Surface line 1"),
                new PipingSurfaceLine("Surface line 2")
            };
            surfaceLines[0].SetGeometry(pointsOne);
            surfaceLines[1].SetGeometry(pointsTwo);

            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateSurfaceLineFeatures(surfaceLines);

            // Assert
            Assert.AreEqual(surfaceLines.Length, features.Count());

            for (var i = 0; i < features.Count(); i++)
            {
                Assert.AreEqual(1, features.ElementAt(i).MapGeometries.Count());
                AssertEqualPointCollections(surfaceLines[i].Points, features.ElementAt(i).MapGeometries.First());
                Assert.AreEqual(1, features.ElementAt(i).MetaData.Keys.Count);
                Assert.AreEqual(surfaceLines[i].Name, features.ElementAt(i).MetaData["Naam"]);
            }
        }

        [Test]
        public void CreateStochasticSoilModelFeatures_StochasticSoilModelsNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateStochasticSoilModelFeatures_NoStochasticSoilModels_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(new PipingStochasticSoilModel[0]);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateStochasticSoilModelFeatures_GivenStochasticSoilModels_ReturnsStochasticSoilModelFeaturesCollection()
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
            PipingStochasticSoilModel[] stochasticSoilModels =
            {
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("StochasticSoilModelName1", pointsOne),
                PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("StochasticSoilModelName2", pointsTwo)
            };

            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateStochasticSoilModelFeatures(stochasticSoilModels);

            // Assert
            Assert.AreEqual(stochasticSoilModels.Length, features.Count());

            for (var i = 0; i < features.Count(); i++)
            {
                Assert.AreEqual(1, features.ElementAt(i).MapGeometries.Count());
                AssertEqualPointCollections(stochasticSoilModels[i].Geometry, features.ElementAt(i).MapGeometries.First());
                Assert.AreEqual(1, features.ElementAt(i).MetaData.Keys.Count);
                Assert.AreEqual(stochasticSoilModels[i].Name, features.ElementAt(i).MetaData["Naam"]);
            }
        }

        [Test]
        public void CreateCalculationFeatures_CalculationsNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_NoCalculations_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(Enumerable.Empty<PipingCalculationScenario>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_GivenCalculations_ReturnsCalculationFeaturesCollection()
        {
            // Setup
            PipingCalculationScenario calculationA = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(new HydraulicBoundaryLocation(1, string.Empty, 5.0, 4.0));
            PipingCalculationScenario calculationB = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(new HydraulicBoundaryLocation(1, string.Empty, 2.2, 3.8));

            calculationA.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(1.0, 3.0);
            calculationB.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint = new Point2D(1.0, 4.0);

            // Call
            IEnumerable<MapFeature> features = PipingMapDataFeaturesFactory.CreateCalculationFeatures(new[]
            {
                calculationA,
                calculationB
            });

            // Assert
            Assert.AreEqual(2, features.Count());
            Assert.AreEqual(1, features.ElementAt(0).MapGeometries.Count());
            Assert.AreEqual(1, features.ElementAt(1).MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                new Point2D(1.0, 3.0),
                new Point2D(5.0, 4.0)
            }, features.ElementAt(0).MapGeometries.ElementAt(0));
            AssertEqualPointCollections(new[]
            {
                new Point2D(1.0, 4.0),
                new Point2D(2.2, 3.8)
            }, features.ElementAt(1).MapGeometries.ElementAt(0));
        }

        private static void AssertEqualPointCollections(IEnumerable<Point3D> points, MapGeometry geometry)
        {
            AssertEqualPointCollections(points.Select(p => new Point2D(p.X, p.Y)), geometry);
        }

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points.Select(p => new Point2D(p)), geometry.PointCollections.First());
        }
    }
}