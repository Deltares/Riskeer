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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.Factories;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.Factories
{
    [TestFixture]
    public class GrassCoverErosionOutwardsMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateCalculationFeatures_CalculationsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_NoCalculations_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(
                Enumerable.Empty<GrassCoverErosionOutwardsWaveConditionsCalculation>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_GivenCalculations_ReturnsCalculationFeaturesArray()
        {
            // Setup
            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var calculationB = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            calculationA.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.0, 3.0));
            calculationB.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.0, 4.0));

            calculationA.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 5.0, 4.0);
            calculationB.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 2.2, 3.8);

            // Call
            MapFeature[] features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(new[]
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

        [Test]
        public void CreateHydraulicBoundaryLocationsFeatures_HydraulicBoundaryLocationsNull_ReturnsEmptyFeaturesArray()
        {
            // Call
            MapFeature[] features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationFeatures_GivenHydraulicBoundaryLocations_ReturnsLocationFeaturesArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            HydraulicBoundaryLocation[] hydraulicBoundaryLocations = points.Select(p => new HydraulicBoundaryLocation(0, "", p.X, p.Y)).ToArray();

            // Call
            MapFeature[] features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeatures(hydraulicBoundaryLocations);

            // Assert
            Assert.AreEqual(hydraulicBoundaryLocations.Length, features.Length);
            for (var i = 0; i < hydraulicBoundaryLocations.Length; i++)
            {
                Assert.AreEqual(4, features[i].MetaData.Keys.Count);
                Assert.AreEqual(hydraulicBoundaryLocations[i].Id, features[i].MetaData["ID"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].Name, features[i].MetaData["Naam"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].DesignWaterLevel, features[i].MetaData["Waterstand bij doorsnede-eis"]);
                Assert.AreEqual(hydraulicBoundaryLocations[i].WaveHeight, features[i].MetaData["Golfhoogte bij doorsnede-eis"]);
            }

            AssertEqualFeatureCollections(points, features);
        }

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points.Select(p => new Point2D(p)), geometry.PointCollections.First());
        }

        private static void AssertEqualFeatureCollections(Point2D[] points, MapFeature[] features)
        {
            Assert.AreEqual(points.Length, features.Length);
            for (var i = 0; i < points.Length; i++)
            {
                CollectionAssert.AreEqual(new[]
                {
                    points[i]
                }, features[i].MapGeometries.First().PointCollections.First());
            }
        }
    }
}