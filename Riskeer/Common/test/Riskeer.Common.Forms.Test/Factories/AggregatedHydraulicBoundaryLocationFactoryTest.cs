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
        public void CreateAggregatedHydraulicBoundaryLocations_LocationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                null, new Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string>(),
                new Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("locations", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_WaterLevelCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                Enumerable.Empty<HydraulicBoundaryLocation>(), null,
                new Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waterLevelCalculations", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_WaveHeightCalculationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string>(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveHeightCalculations", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_WithAllData_ReturnAggregatedHydraulicBoundaryLocations()
        {
            // Setup
            var random = new Random(21);
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "location1", 1, 1),
                new HydraulicBoundaryLocation(2, "location2", 2, 2)
            };

            var waterLevelCalculations = new Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string>
            {
                {
                    new ObservableList<HydraulicBoundaryLocationCalculation>
                    {
                        new HydraulicBoundaryLocationCalculation(locations[0])
                        {
                            Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
                        },
                        new HydraulicBoundaryLocationCalculation(locations[1])
                        {
                            Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
                        }
                    },
                    "1/10"
                },
                {
                    new ObservableList<HydraulicBoundaryLocationCalculation>
                    {
                        new HydraulicBoundaryLocationCalculation(locations[0])
                        {
                            Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
                        },
                        new HydraulicBoundaryLocationCalculation(locations[1])
                    },
                    "1/100"
                }
            };
            var waveHeightCalculations = new Dictionary<IObservableEnumerable<HydraulicBoundaryLocationCalculation>, string>
            {
                {
                    new ObservableList<HydraulicBoundaryLocationCalculation>
                    {
                        new HydraulicBoundaryLocationCalculation(locations[0]),
                        new HydraulicBoundaryLocationCalculation(locations[1])
                        {
                            Output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble())
                        }
                    },
                    "1/200"
                }
            };

            // Call
            IEnumerable<AggregatedHydraulicBoundaryLocation> aggregatedLocations = AggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                locations, waterLevelCalculations, waveHeightCalculations);

            // Assert
            Assert.AreEqual(locations.Length, aggregatedLocations.Count());

            for (var i = 0; i < locations.Length; i++)
            {
                AggregatedHydraulicBoundaryLocation aggregatedLocation = aggregatedLocations.ElementAt(i);

                Assert.AreEqual(locations[i].Id, aggregatedLocation.Id);
                Assert.AreEqual(locations[i].Name, aggregatedLocation.Name);
                Assert.AreEqual(locations[i].Location, aggregatedLocation.Location);

                Assert.AreEqual(waterLevelCalculations.Count, aggregatedLocation.WaterLevelCalculationsForTargetProbabilities.Count());
                Assert.AreEqual(waveHeightCalculations.Count, aggregatedLocation.WaveHeightCalculationsForTargetProbabilities.Count());

                for (var j = 0; j < waterLevelCalculations.Count; j++)
                {
                    Assert.AreEqual(waterLevelCalculations.ElementAt(j).Value, aggregatedLocation.WaterLevelCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(GetExpectedResult(waterLevelCalculations.ElementAt(j).Key, locations[i]),
                                    aggregatedLocation.WaterLevelCalculationsForTargetProbabilities.ElementAt(j).Item2);
                }

                for (var j = 0; j < waveHeightCalculations.Count; j++)
                {
                    Assert.AreEqual(waveHeightCalculations.ElementAt(j).Value, aggregatedLocation.WaveHeightCalculationsForTargetProbabilities.ElementAt(j).Item1);
                    Assert.AreEqual(GetExpectedResult(waveHeightCalculations.ElementAt(j).Key, locations[i]),
                                    aggregatedLocation.WaveHeightCalculationsForTargetProbabilities.ElementAt(j).Item2);
                }
            }
        }

        private static RoundedDouble GetExpectedResult(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                       HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return calculations
                   .Single(calculation => calculation.HydraulicBoundaryLocation.Equals(hydraulicBoundaryLocation))
                   .Output?.Result ?? RoundedDouble.NaN;
        }
    }
}