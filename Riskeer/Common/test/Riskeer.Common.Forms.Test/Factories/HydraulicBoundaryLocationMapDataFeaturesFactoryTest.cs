﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Features;
using NUnit.Framework;
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Util.TestUtil;

namespace Riskeer.Common.Forms.Test.Factories
{
    [TestFixture]
    public class HydraulicBoundaryLocationMapDataFeaturesFactoryTest
    {
        [Test]
        public void CreateHydraulicBoundaryLocationFeature_LocationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("location", exception.ParamName);
        }

        [Test]
        public void CreateHydraulicBoundaryLocationFeature_WithLocation_ReturnFeature()
        {
            // Setup
            var random = new Random(39);
            IEnumerable<Tuple<double, RoundedDouble>> waterLevelCalculationForTargetProbabilities = new[]
            {
                new Tuple<double, RoundedDouble>(0.1, random.NextRoundedDouble()),
                new Tuple<double, RoundedDouble>(0.1, random.NextRoundedDouble())
            };
            IEnumerable<Tuple<double, RoundedDouble>> waveHeightCalculationForTargetProbabilities = new[]
            {
                new Tuple<double, RoundedDouble>(0.001, random.NextRoundedDouble())
            };
            var location = new AggregatedHydraulicBoundaryLocation(1, "test", new Point2D(0, 0),
                                                                   waterLevelCalculationForTargetProbabilities,
                                                                   waveHeightCalculationForTargetProbabilities);

            // Call
            MapFeature feature = HydraulicBoundaryLocationMapDataFeaturesFactory.CreateHydraulicBoundaryLocationFeature(location);

            // Assert
            MapFeaturesMetaDataTestHelper.AssertMetaData(location.Id, feature, "ID");
            MapFeaturesMetaDataTestHelper.AssertMetaData(location.Name, feature, "Naam");
            
            MapFeaturesMetaDataTestHelper.AssertMetaData(location.WaterLevelCalculationForTargetProbabilities.First().Item2.ToString(), feature, "h - 1/10");
            MapFeaturesMetaDataTestHelper.AssertMetaData(location.WaterLevelCalculationForTargetProbabilities.Last().Item2.ToString(), feature, "h - 1/10 (1)");
            MapFeaturesMetaDataTestHelper.AssertMetaData(location.WaveHeightCalculationForTargetProbabilities.First().Item2.ToString(), feature, "Hs - 1/1.000");
        }
    }
}