// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class AggregatedHydraulicBoundaryLocationFactoryTest
    {
        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_AssessmentSectionWithLocationsWithOutput_ReturnAggregatedHydraulicBoundaryLocations()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "location1", 1, 1),
                new HydraulicBoundaryLocation(2, "location2", 2, 2)
            }, true);

            // Call
            AggregatedHydraulicBoundaryLocation[] aggregatedLocations = AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(assessmentSection).ToArray();

            // Assert
            HydraulicBoundaryLocation[] expectedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(expectedLocations.Length, aggregatedLocations.Length);

            for (var i = 0; i < expectedLocations.Length; i++)
            {
                Assert.AreEqual(expectedLocations[i].Id, aggregatedLocations[i].Id);
                Assert.AreEqual(expectedLocations[i].Name, aggregatedLocations[i].Name);
                Assert.AreEqual(expectedLocations[i].Location, aggregatedLocations[i].Location);

                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForFactorizedSignalingNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForSignalingNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForSignalingNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForLowerLimitNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForFactorizedLowerLimitNorm);

                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForFactorizedSignalingNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForSignalingNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForSignalingNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForLowerLimitNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForFactorizedLowerLimitNorm);
            }
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_AssessmentSectionWithLocationsWithoutOutput_ReturnAggregatedHydraulicBoundaryLocations()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "location1", 1, 1),
                new HydraulicBoundaryLocation(2, "location2", 2, 2)
            });

            // Call
            AggregatedHydraulicBoundaryLocation[] aggregatedLocations = AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(assessmentSection).ToArray();

            // Assert
            HydraulicBoundaryLocation[] expectedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(expectedLocations.Length, aggregatedLocations.Length);

            for (var i = 0; i < expectedLocations.Length; i++)
            {
                Assert.AreEqual(expectedLocations[i].Id, aggregatedLocations[i].Id);
                Assert.AreEqual(expectedLocations[i].Name, aggregatedLocations[i].Name);
                Assert.AreEqual(expectedLocations[i].Location, aggregatedLocations[i].Location);

                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForSignalingNorm);
                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForLowerLimitNorm);
                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForFactorizedLowerLimitNorm);

                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForSignalingNorm);
                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForLowerLimitNorm);
                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForFactorizedLowerLimitNorm);
            }
        }

        private static RoundedDouble GetExpectedResult(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                       HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return calculations
                   .Single(calculation => calculation.HydraulicBoundaryLocation.Equals(hydraulicBoundaryLocation))
                   .Output.Result;
        }
    }
}