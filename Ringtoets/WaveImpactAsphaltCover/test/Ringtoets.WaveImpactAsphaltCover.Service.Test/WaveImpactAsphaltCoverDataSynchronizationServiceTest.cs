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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.WaveImpactAsphaltCover.Service.Test
{
    [TestFixture]
    public class WaveImpactAsphaltCoverDataSynchronizationServiceTest
    {
        [Test]
        public void ClearWaveConditionsCalculation_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => WaveImpactAsphaltCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearWaveConditionsCalculation_WithCalculation_OutputNull()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            // Precondition
            Assert.IsNotNull(calculation.Output);

            // Call
            WaveImpactAsphaltCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                                WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            WaveImpactAsphaltCoverWaveConditionsCalculation[] expectedAffectedCalculations = failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                                                                                             .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput)
                                                                                                             .ToArray();

            // Call
            IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> affectedItems =
                WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            Assert.IsFalse(failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                           .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            CollectionAssert.AreEqual(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllWaveConditionsCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations.Where(c => c.HasOutput)
                                                                          .ToArray();
            // Call
            IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> affectedItems =
                WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(failureMechanism);

            // Assert
            Assert.IsFalse(failureMechanism.Calculations.Any(c => c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => WaveImpactAsphaltCoverDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            // Call
            IObservable[] observables = WaveImpactAsphaltCoverDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism).ToArray();

            // Assert
            Assert.AreEqual(3, observables.Length);

            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.Contains(observables, failureMechanism);

            CollectionAssert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.Contains(observables, failureMechanism.WaveConditionsCalculationGroup);

            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(observables, failureMechanism.ForeshoreProfiles);
        }

        private static WaveImpactAsphaltCoverFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var calculationWithOutput = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var calculationWithHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var subCalculationWithOutput = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var subCalculationWithHydraulicBoundaryLocation = new WaveImpactAsphaltCoverWaveConditionsCalculation
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