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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
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
            void Call() => AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(null, new Dictionary<IObservableEnumerable<DuneLocationCalculation>, double>());

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
                new DuneLocation(1, "location1", new Point2D(1, 1), new DuneLocation.ConstructionProperties()),
                new DuneLocation(2, "location2", new Point2D(2, 2), new DuneLocation.ConstructionProperties())
            };

            var calculations = new Dictionary<IObservableEnumerable<DuneLocationCalculation>, double>
            {
                {
                    new ObservableList<DuneLocationCalculation>
                    {
                        new DuneLocationCalculation(duneLocations[0])
                        {
                            Output = new TestDuneLocationCalculationOutput(random.NextDouble(), random.NextDouble(), random.NextDouble())
                        },
                        new DuneLocationCalculation(duneLocations[1])
                        {
                            Output = new TestDuneLocationCalculationOutput(random.NextDouble(), random.NextDouble(), random.NextDouble())
                        }
                    },
                    0.1
                },
                {
                    new ObservableList<DuneLocationCalculation>
                    {
                        new DuneLocationCalculation(duneLocations[0])
                        {
                            Output = new TestDuneLocationCalculationOutput(random.NextDouble(), random.NextDouble(), random.NextDouble())
                        },
                        new DuneLocationCalculation(duneLocations[1])
                    },
                    0.001
                }
            };

            // Call
            IEnumerable<AggregatedDuneLocation> aggregatedLocations = AggregatedDuneLocationFactory.CreateAggregatedDuneLocations(duneLocations, calculations);

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
                Assert.AreEqual(duneLocation.D50, aggregatedLocation.D50);

                for (var j = 0; j < calculations.Count; j++)
                {
                    Assert.AreEqual(calculations.ElementAt(j).Value, aggregatedLocation.WaterLevelCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(calculations.ElementAt(j).Value, aggregatedLocation.WaveHeightCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(calculations.ElementAt(j).Value, aggregatedLocation.WavePeriodCalculationsForTargetProbabilities.ElementAt(j).Item1);

                    DuneLocationCalculationOutput output = GetOutput(calculations.ElementAt(j).Key, duneLocations[i]);
                    Assert.AreEqual(output?.WaterLevel ?? RoundedDouble.NaN, aggregatedLocation.WaterLevelCalculationsForTargetProbabilities.ElementAt(j).Item2);
                    Assert.AreEqual(output?.WaveHeight ?? RoundedDouble.NaN, aggregatedLocation.WaveHeightCalculationsForTargetProbabilities.ElementAt(j).Item2);
                    Assert.AreEqual(output?.WavePeriod ?? RoundedDouble.NaN, aggregatedLocation.WavePeriodCalculationsForTargetProbabilities.ElementAt(j).Item2);
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