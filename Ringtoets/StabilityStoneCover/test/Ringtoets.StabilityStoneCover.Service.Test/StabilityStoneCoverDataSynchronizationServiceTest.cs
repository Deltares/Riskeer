// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.Probability;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Ringtoets.StabilityStoneCover.Service.Test
{
    [TestFixture]
    public class StabilityStoneCoverDataSynchronizationServiceTest
    {
        [Test]
        public void ClearWaveConditionsCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearWaveConditionsCalculationOutput_WithCalculation_OutputNull()
        {
            // Setup
            var calculation = new StabilityStoneCoverWaveConditionsCalculation()
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(new[]
                {
                    new WaveConditionsOutput(12.0, 4.3, 0.4, 49)
                }, new[]
                {
                    new WaveConditionsOutput(6.0, 3.7, 4.3, 29)
                })
            };

            // Precondition
            Assert.IsNotNull(calculation.Output);

            // Call
            StabilityStoneCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationAndOutput_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedCalculations()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>())
            };

            var calculation2 = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>())
            };

            var calculation3 = new StabilityStoneCoverWaveConditionsCalculation();

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation1);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation2);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedItems = StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in failureMechanism.WaveConditionsCalculationGroup.Children
                                                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>())
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
                Assert.IsNull(calculation.Output);
            }
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationNoOutput_ClearsHydraulicBoundaryLocationAndReturnsAffectedCalculations()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation2 = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation3 = new StabilityStoneCoverWaveConditionsCalculation();

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation1);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation2);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedItems = StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in failureMechanism.WaveConditionsCalculationGroup.Children
                                                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>())
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            }
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithOutputAndNoHydraulicBoundaryLocation_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var calculation1 = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>())
            };

            var calculation2 = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>())
            };

            var calculation3 = new StabilityStoneCoverWaveConditionsCalculation();

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation1);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation2);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedItems = StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in failureMechanism.WaveConditionsCalculationGroup.Children
                                                                                                 .Cast<StabilityStoneCoverWaveConditionsCalculation>())
            {
                Assert.IsNull(calculation.Output);
            }
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationWithoutOutputAndHydraulicBoundaryLocation_ReturnNoAffectedCalculations()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var calculation1 = new StabilityStoneCoverWaveConditionsCalculation();
            var calculation2 = new StabilityStoneCoverWaveConditionsCalculation();
            var calculation3 = new StabilityStoneCoverWaveConditionsCalculation();

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation1);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation2);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedItems = StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(affectedItems);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_WithOutput_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = new StabilityStoneCoverFailureMechanism();
            StabilityStoneCoverWaveConditionsCalculation calculation1 = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>())
            };

            StabilityStoneCoverWaveConditionsCalculation calculation2 = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>())
            };

            StabilityStoneCoverWaveConditionsCalculation calculation3 = new StabilityStoneCoverWaveConditionsCalculation();

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation1);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation2);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedItems = StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(failureMechanism);

            // Assert
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in failureMechanism.WaveConditionsCalculationGroup.Children.Cast<StabilityStoneCoverWaveConditionsCalculation>())
            {
                Assert.IsNull(calculation.Output);
            }
            CollectionAssert.AreEqual(new[]
            {
                calculation1,
                calculation2
            }, affectedItems);
        }
    }
}