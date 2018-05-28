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
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.Views;

namespace Ringtoets.DuneErosion.Forms.Test.Factories
{
    [TestFixture]
    public class AggregatedDuneLocationFactoryTest
    {
        [Test]
        public void CreateAggregatedDuneLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedDuneLocations_DuneLocationWithOutput_ReturnsAggregatedDuneLocations()
        {
            // Setup
            var duneLocations = new[]
            {
                CreateDuneLocation(20),
                CreateDuneLocation(21)
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(duneLocations);
            SetDuneLocationCalculationOutput(failureMechanism);

            // Call
            IEnumerable<AggregatedDuneLocation> aggregatedLocations = AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(failureMechanism);

            // Assert
            int expectedNrOfDuneLocations = duneLocations.Length;
            Assert.AreEqual(expectedNrOfDuneLocations, aggregatedLocations.Count());
            for (var i = 0; i < expectedNrOfDuneLocations; i++)
            {
                DuneLocation expectedDuneLocation = duneLocations[i];
                AggregatedDuneLocation aggregatedDuneLocation = aggregatedLocations.ElementAt(i);
                Assert.AreEqual(expectedDuneLocation.Id, aggregatedDuneLocation.Id);
                Assert.AreEqual(expectedDuneLocation.Name, aggregatedDuneLocation.Name);
                Assert.AreEqual(expectedDuneLocation.Location, aggregatedDuneLocation.Location);
                Assert.AreEqual(expectedDuneLocation.CoastalAreaId, aggregatedDuneLocation.CoastalAreaId);
                Assert.AreEqual(expectedDuneLocation.Offset, aggregatedDuneLocation.Offset);
                Assert.AreEqual(expectedDuneLocation.D50, aggregatedDuneLocation.D50);

                AssertDuneLocationOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm,
                                         expectedDuneLocation,
                                         () => aggregatedDuneLocation.WaterLevelForMechanismSpecificFactorizedSignalingNorm,
                                         () => aggregatedDuneLocation.WaveHeightForMechanismSpecificFactorizedSignalingNorm,
                                         () => aggregatedDuneLocation.WavePeriodForMechanismSpecificFactorizedSignalingNorm);

                AssertDuneLocationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm,
                                         expectedDuneLocation,
                                         () => aggregatedDuneLocation.WaterLevelForMechanismSpecificSignalingNorm,
                                         () => aggregatedDuneLocation.WaveHeightForMechanismSpecificSignalingNorm,
                                         () => aggregatedDuneLocation.WavePeriodForMechanismSpecificSignalingNorm);

                AssertDuneLocationOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm,
                                         expectedDuneLocation,
                                         () => aggregatedDuneLocation.WaterLevelForMechanismSpecificLowerLimitNorm,
                                         () => aggregatedDuneLocation.WaveHeightForMechanismSpecificLowerLimitNorm,
                                         () => aggregatedDuneLocation.WavePeriodForMechanismSpecificLowerLimitNorm);

                AssertDuneLocationOutput(failureMechanism.CalculationsForLowerLimitNorm,
                                         expectedDuneLocation,
                                         () => aggregatedDuneLocation.WaterLevelForLowerLimitNorm,
                                         () => aggregatedDuneLocation.WaveHeightForLowerLimitNorm,
                                         () => aggregatedDuneLocation.WavePeriodForLowerLimitNorm);

                AssertDuneLocationOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm,
                                         expectedDuneLocation,
                                         () => aggregatedDuneLocation.WaterLevelForFactorizedLowerLimitNorm,
                                         () => aggregatedDuneLocation.WaveHeightForFactorizedLowerLimitNorm,
                                         () => aggregatedDuneLocation.WavePeriodForFactorizedLowerLimitNorm);
            }
        }

        [Test]
        public void CreateAggregatedDuneLocations_DuneLocationWithoutOutput_ReturnsAggregatedDuneLocations()
        {
            // Setup
            var duneLocations = new[]
            {
                CreateDuneLocation(20),
                CreateDuneLocation(21)
            };

            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(duneLocations);

            // Call
            IEnumerable<AggregatedDuneLocation> aggregatedLocations = AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(failureMechanism);

            // Assert
            int expectedNrOfDuneLocations = duneLocations.Length;
            Assert.AreEqual(expectedNrOfDuneLocations, aggregatedLocations.Count());
            for (var i = 0; i < expectedNrOfDuneLocations; i++)
            {
                DuneLocation expectedDuneLocation = duneLocations[i];
                AggregatedDuneLocation aggregatedDuneLocation = aggregatedLocations.ElementAt(i);
                Assert.AreEqual(expectedDuneLocation.Id, aggregatedDuneLocation.Id);
                Assert.AreEqual(expectedDuneLocation.Name, aggregatedDuneLocation.Name);
                Assert.AreEqual(expectedDuneLocation.Location, aggregatedDuneLocation.Location);
                Assert.AreEqual(expectedDuneLocation.CoastalAreaId, aggregatedDuneLocation.CoastalAreaId);
                Assert.AreEqual(expectedDuneLocation.Offset, aggregatedDuneLocation.Offset);
                Assert.AreEqual(expectedDuneLocation.D50, aggregatedDuneLocation.D50);

                Assert.IsNaN(aggregatedDuneLocation.WaterLevelForMechanismSpecificFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedDuneLocation.WaterLevelForMechanismSpecificSignalingNorm);
                Assert.IsNaN(aggregatedDuneLocation.WaterLevelForMechanismSpecificLowerLimitNorm);
                Assert.IsNaN(aggregatedDuneLocation.WaterLevelForLowerLimitNorm);
                Assert.IsNaN(aggregatedDuneLocation.WaterLevelForFactorizedLowerLimitNorm);

                Assert.IsNaN(aggregatedDuneLocation.WaveHeightForMechanismSpecificFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedDuneLocation.WaveHeightForMechanismSpecificSignalingNorm);
                Assert.IsNaN(aggregatedDuneLocation.WaveHeightForMechanismSpecificLowerLimitNorm);
                Assert.IsNaN(aggregatedDuneLocation.WaveHeightForLowerLimitNorm);
                Assert.IsNaN(aggregatedDuneLocation.WaveHeightForFactorizedLowerLimitNorm);

                Assert.IsNaN(aggregatedDuneLocation.WavePeriodForMechanismSpecificFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedDuneLocation.WavePeriodForMechanismSpecificSignalingNorm);
                Assert.IsNaN(aggregatedDuneLocation.WavePeriodForMechanismSpecificLowerLimitNorm);
                Assert.IsNaN(aggregatedDuneLocation.WavePeriodForLowerLimitNorm);
                Assert.IsNaN(aggregatedDuneLocation.WavePeriodForFactorizedLowerLimitNorm);
            }
        }

        private static DuneLocation CreateDuneLocation(int seed)
        {
            var random = new Random(seed);

            int id = random.Next();
            return new DuneLocation(id, $"Location_{id}", new Point2D(random.NextDouble(), random.NextDouble()), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = random.Next(),
                D50 = random.NextDouble(),
                Offset = random.NextDouble()
            });
        }

        private static void SetDuneLocationCalculationOutput(DuneErosionFailureMechanism failureMechanism)
        {
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificSignalingNorm);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForLowerLimitNorm);
            SetDuneLocationCalculationOutput(failureMechanism.CalculationsForFactorizedLowerLimitNorm);
        }

        private static void SetDuneLocationCalculationOutput(IEnumerable<DuneLocationCalculation> calculations)
        {
            var random = new Random(21);
            foreach (DuneLocationCalculation duneLocationCalculation in calculations)
            {
                duneLocationCalculation.Output = new TestDuneLocationCalculationOutput(random.NextDouble(),
                                                                                       random.NextDouble(),
                                                                                       random.NextDouble());
            }
        }

        private static void AssertDuneLocationOutput(IEnumerable<DuneLocationCalculation> calculations,
                                                     DuneLocation duneLocation,
                                                     Func<RoundedDouble> getWaterLevelFunc,
                                                     Func<RoundedDouble> getWaveHeightFunc,
                                                     Func<RoundedDouble> getWavePeriodFunc)
        {
            DuneLocationCalculationOutput expectedOutput =
                calculations.Single(calculation => ReferenceEquals(duneLocation, calculation.DuneLocation))
                            .Output;

            Assert.AreEqual(expectedOutput.WaterLevel, getWaterLevelFunc());
            Assert.AreEqual(expectedOutput.WaveHeight, getWaveHeightFunc());
            Assert.AreEqual(expectedOutput.WavePeriod, getWavePeriodFunc());
        }
    }
}