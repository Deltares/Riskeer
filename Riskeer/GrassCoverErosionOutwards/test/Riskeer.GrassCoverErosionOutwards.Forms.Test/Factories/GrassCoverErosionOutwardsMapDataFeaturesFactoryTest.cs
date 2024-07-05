﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms.Factories;

namespace Riskeer.GrassCoverErosionOutwards.Forms.Test.Factories
{
    [TestFixture]
    public class GrassCoverErosionOutwardsMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateCalculationFeatures_CalculationsNull_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(null);

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_NoCalculations_ReturnsEmptyFeaturesCollection()
        {
            // Call
            IEnumerable<MapFeature> features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(
                Enumerable.Empty<GrassCoverErosionOutwardsWaveConditionsCalculation>());

            // Assert
            CollectionAssert.IsEmpty(features);
        }

        [Test]
        public void CreateCalculationFeatures_GivenCalculations_ReturnsCalculationFeaturesCollection()
        {
            // Setup
            var calculationA = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var calculationB = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            calculationA.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.0, 3.0));
            calculationB.InputParameters.ForeshoreProfile = new TestForeshoreProfile(new Point2D(1.0, 4.0));

            calculationA.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 5.0, 4.0);
            calculationB.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 2.2, 3.8);

            // Call
            IEnumerable<MapFeature> features = GrassCoverErosionOutwardsMapDataFeaturesFactory.CreateCalculationFeatures(new[]
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

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points.Select(p => new Point2D(p)), geometry.PointCollections.First());
        }
    }
}