// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.Factories;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Forms.Test.Factories
{
    [TestFixture]
    public class AggregatedDuneLocationFactoryTest
    {
        [Test]
        public void CreateAggregatedDuneLocations_DuneLocationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(null, Enumerable.Empty<DuneLocationCalculationsForTargetProbability>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("duneLocations", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedDuneLocations_CalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(Enumerable.Empty<DuneLocation>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedDuneLocations_WithAllData_ReturnAggregatedDuneLocations()
        {
            // Setup
            var random = new Random(21);
            var duneLocations = new[]
            {
                new DuneLocation("location1", new HydraulicBoundaryLocation(1, string.Empty, 1, 1), new DuneLocation.ConstructionProperties()),
                new DuneLocation("location2", new HydraulicBoundaryLocation(2, string.Empty, 2, 2), new DuneLocation.ConstructionProperties())
            };

            var targetProbabilities = new[]
            {
                new DuneLocationCalculationsForTargetProbability(0.1)
                {
                    DuneLocationCalculations =
                    {
                        new DuneLocationCalculation(duneLocations[0])
                        {
                            Output = new TestDuneLocationCalculationOutput(random.NextDouble(), random.NextDouble(), random.NextDouble(), 
                                                                           random.NextDouble(), random.NextDouble(), random.NextDouble())
                        },
                        new DuneLocationCalculation(duneLocations[1])
                        {
                            Output = new TestDuneLocationCalculationOutput(random.NextDouble(), random.NextDouble(), random.NextDouble(), 
                                                                           random.NextDouble(), random.NextDouble(), random.NextDouble())
                        }
                    }
                },
                new DuneLocationCalculationsForTargetProbability(0.001)
                {
                    DuneLocationCalculations =
                    {
                        new DuneLocationCalculation(duneLocations[0])
                        {
                            Output = new TestDuneLocationCalculationOutput(random.NextDouble(), random.NextDouble(), random.NextDouble(),
                                                                           random.NextDouble(), random.NextDouble(), random.NextDouble())
                        },
                        new DuneLocationCalculation(duneLocations[1])
                    }
                }
            };

            // Call
            IEnumerable<AggregatedDuneLocation> aggregatedLocations = AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(duneLocations, targetProbabilities);

            // Assert
            Assert.AreEqual(duneLocations.Length, aggregatedLocations.Count());

            for (var i = 0; i < duneLocations.Length; i++)
            {
                DuneLocation duneLocation = duneLocations[i];
                AggregatedDuneLocation aggregatedLocation = aggregatedLocations.ElementAt(i);

                Assert.AreEqual(duneLocation.Id, aggregatedLocation.Id);
                Assert.AreEqual(duneLocation.Name, aggregatedLocation.Name);
                Assert.AreEqual(duneLocation.Location, aggregatedLocation.Location);
                Assert.AreEqual(duneLocation.CoastalAreaId, aggregatedLocation.CoastalAreaId);
                Assert.AreEqual(duneLocation.Offset, aggregatedLocation.Offset);

                for (var j = 0; j < targetProbabilities.Length; j++)
                {
                    Assert.AreEqual(targetProbabilities[j].TargetProbability, aggregatedLocation.WaterLevelCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(targetProbabilities[j].TargetProbability, aggregatedLocation.WaveHeightCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(targetProbabilities[j].TargetProbability, aggregatedLocation.WavePeriodCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(targetProbabilities[j].TargetProbability, aggregatedLocation.MeanTidalAmplitudeCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(targetProbabilities[j].TargetProbability, aggregatedLocation.WaveDirectionalSpreadCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(targetProbabilities[j].TargetProbability, aggregatedLocation.TideSurgePhaseDifferenceCalculationsForTargetProbabilities.ElementAt(j).Item1);

                    DuneLocationCalculationOutput output = GetOutput(targetProbabilities[j].DuneLocationCalculations, duneLocations[i]);
                    Assert.AreEqual(output?.WaterLevel ?? RoundedDouble.NaN, aggregatedLocation.WaterLevelCalculationsForTargetProbabilities.ElementAt(j).Item2);
                    Assert.AreEqual(output?.WaveHeight ?? RoundedDouble.NaN, aggregatedLocation.WaveHeightCalculationsForTargetProbabilities.ElementAt(j).Item2);
                    Assert.AreEqual(output?.WavePeriod ?? RoundedDouble.NaN, aggregatedLocation.WavePeriodCalculationsForTargetProbabilities.ElementAt(j).Item2);
                    Assert.AreEqual(output?.MeanTidalAmplitude ?? RoundedDouble.NaN, aggregatedLocation.MeanTidalAmplitudeCalculationsForTargetProbabilities.ElementAt(j).Item2);
                    Assert.AreEqual(output?.WaveDirectionalSpread ?? RoundedDouble.NaN, aggregatedLocation.WaveDirectionalSpreadCalculationsForTargetProbabilities.ElementAt(j).Item2);
                    Assert.AreEqual(output?.TideSurgePhaseDifference ?? RoundedDouble.NaN, aggregatedLocation.TideSurgePhaseDifferenceCalculationsForTargetProbabilities.ElementAt(j).Item2);
                }
            }
        }

        private static DuneLocationCalculationOutput GetOutput(IEnumerable<DuneLocationCalculation> calculations,
                                                               DuneLocation location)
        {
            return calculations.Single(c => c.DuneLocation.Equals(location))
                               .Output;
        }
    }
}