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
using Ringtoets.Common.Data.Calculation;
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
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
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
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsCalculationsAndReturnsAffectedCalculations()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            StabilityStoneCoverWaveConditionsCalculation[] expectedAffectedCalculations = failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                                                                                          .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput)
                                                                                                          .ToArray();

            // Call
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedItems =
                StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            Assert.IsFalse(failureMechanism.Calculations.Cast<StabilityStoneCoverWaveConditionsCalculation>()
                                           .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
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
        public void ClearAllWaveConditionsCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            StabilityStoneCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedItems =
                StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(failureMechanism);

            // Assert
            Assert.IsFalse(failureMechanism.Calculations.Any(c => c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_CalculationWithoutOutput_ReturnNoAffectedCalculations()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new StabilityStoneCoverWaveConditionsCalculation());

            // Call
            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> affectedItems =
                StabilityStoneCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(affectedItems);
        }

        private static StabilityStoneCoverFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var calculationWithOutput = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new StabilityStoneCoverWaveConditionsCalculation();
            var subCalculationWithOutput = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithHydraulicBoundaryLocation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutput);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutput,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation
                }
            });

            return failureMechanism;
        }
    }
}