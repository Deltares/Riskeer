// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Forms.Factories;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Test.Factories
{
    [TestFixture]
    public class AggregatedHydraulicBoundaryLocationFactoryTest
    {
        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_AssessmentSectionWithLocationsWithOutput_ReturnAggregatedHydraulicBoundaryLocations()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    LowerLimitNorm = 0.001
                }
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "location1", 1, 1),
                new HydraulicBoundaryLocation(2, "location2", 2, 2)
            }, true);

            // Call
            IEnumerable<AggregatedHydraulicBoundaryLocation> aggregatedLocations = AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(assessmentSection);

            // Assert
            Tuple<double, IEnumerable<HydraulicBoundaryLocationCalculation>>[] expectedWaterLevelTargetProbabilities = assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities
                                                                                                                                        .Select(tp => new Tuple<double, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                                                                                                                                    tp.TargetProbability, tp.HydraulicBoundaryLocationCalculations))
                                                                                                                                        .Concat(new[]
                                                                                                                                        {
                                                                                                                                            new Tuple<double, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                                                                                                                                assessmentSection.FailureMechanismContribution.SignalingNorm,
                                                                                                                                                assessmentSection.WaterLevelCalculationsForSignalingNorm),
                                                                                                                                            new Tuple<double, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                                                                                                                                assessmentSection.FailureMechanismContribution.LowerLimitNorm,
                                                                                                                                                assessmentSection.WaterLevelCalculationsForLowerLimitNorm)
                                                                                                                                        })
                                                                                                                                        .OrderByDescending(tp => tp.Item1)
                                                                                                                                        .ToArray();

            Tuple<double, IEnumerable<HydraulicBoundaryLocationCalculation>>[] expectedWaveHeightTargetProbabilities = assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities
                                                                                                                                        .Select(tp => new Tuple<double, IEnumerable<HydraulicBoundaryLocationCalculation>>(
                                                                                                                                                    tp.TargetProbability, tp.HydraulicBoundaryLocationCalculations))
                                                                                                                                        .OrderByDescending(tp => tp.Item1)
                                                                                                                                        .ToArray();

            HydraulicBoundaryLocation[] expectedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(expectedLocations.Length, aggregatedLocations.Count());

            for (var i = 0; i < expectedLocations.Length; i++)
            {
                AggregatedHydraulicBoundaryLocation aggregatedLocation = aggregatedLocations.ElementAt(i);

                Assert.AreEqual(expectedLocations[i].Id, aggregatedLocation.Id);
                Assert.AreEqual(expectedLocations[i].Name, aggregatedLocation.Name);
                Assert.AreEqual(expectedLocations[i].Location, aggregatedLocation.Location);

                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, expectedLocations[i]),
                                aggregatedLocation.WaterLevelCalculationForFactorizedSignalingNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForSignalingNorm, expectedLocations[i]),
                                aggregatedLocation.WaterLevelCalculationForSignalingNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocation.WaterLevelCalculationForLowerLimitNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocation.WaterLevelCalculationForFactorizedLowerLimitNorm);

                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, expectedLocations[i]),
                                aggregatedLocation.WaveHeightCalculationForFactorizedSignalingNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForSignalingNorm, expectedLocations[i]),
                                aggregatedLocation.WaveHeightCalculationForSignalingNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocation.WaveHeightCalculationForLowerLimitNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocation.WaveHeightCalculationForFactorizedLowerLimitNorm);

                Assert.AreEqual(expectedWaterLevelTargetProbabilities.Length, aggregatedLocation.WaterLevelCalculationForTargetProbabilities.Count());
                Assert.AreEqual(expectedWaveHeightTargetProbabilities.Length, aggregatedLocation.WaveHeightCalculationForTargetProbabilities.Count());

                for (var j = 0; j < expectedWaterLevelTargetProbabilities.Length; j++)
                {
                    Assert.AreEqual(expectedWaterLevelTargetProbabilities[j].Item1, aggregatedLocation.WaterLevelCalculationForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(GetExpectedResult(expectedWaterLevelTargetProbabilities[j].Item2, expectedLocations[i]),
                                    aggregatedLocation.WaterLevelCalculationForTargetProbabilities.ElementAt(j).Item2);
                }

                for (var j = 0; j < expectedWaveHeightTargetProbabilities.Length; j++)
                {
                    Assert.AreEqual(expectedWaveHeightTargetProbabilities[j].Item1, aggregatedLocation.WaveHeightCalculationForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(GetExpectedResult(expectedWaveHeightTargetProbabilities[j].Item2, expectedLocations[i]),
                                    aggregatedLocation.WaveHeightCalculationForTargetProbabilities.ElementAt(j).Item2);
                }
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
            IEnumerable<AggregatedHydraulicBoundaryLocation> aggregatedLocations = AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(assessmentSection);

            // Assert
            HydraulicBoundaryLocation[] expectedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(expectedLocations.Length, aggregatedLocations.Count());

            for (var i = 0; i < expectedLocations.Length; i++)
            {
                AggregatedHydraulicBoundaryLocation aggregatedLocation = aggregatedLocations.ElementAt(i);

                Assert.AreEqual(expectedLocations[i].Id, aggregatedLocation.Id);
                Assert.AreEqual(expectedLocations[i].Name, aggregatedLocation.Name);
                Assert.AreEqual(expectedLocations[i].Location, aggregatedLocation.Location);

                Assert.IsNaN(aggregatedLocation.WaterLevelCalculationForFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedLocation.WaterLevelCalculationForSignalingNorm);
                Assert.IsNaN(aggregatedLocation.WaterLevelCalculationForLowerLimitNorm);
                Assert.IsNaN(aggregatedLocation.WaterLevelCalculationForFactorizedLowerLimitNorm);

                Assert.IsNaN(aggregatedLocation.WaveHeightCalculationForFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedLocation.WaveHeightCalculationForSignalingNorm);
                Assert.IsNaN(aggregatedLocation.WaveHeightCalculationForLowerLimitNorm);
                Assert.IsNaN(aggregatedLocation.WaveHeightCalculationForFactorizedLowerLimitNorm);

                Assert.IsTrue(aggregatedLocation.WaterLevelCalculationForTargetProbabilities.All(tp => double.IsNaN(tp.Item2)));
                Assert.IsTrue(aggregatedLocation.WaveHeightCalculationForTargetProbabilities.All(tp => double.IsNaN(tp.Item2)));
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