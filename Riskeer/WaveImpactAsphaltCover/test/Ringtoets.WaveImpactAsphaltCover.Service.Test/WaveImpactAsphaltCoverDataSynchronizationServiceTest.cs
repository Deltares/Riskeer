// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
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
        public void ClearWaveConditionsCalculation_WithCalculation_OutputNullAndReturnAffectedCalculations()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            // Precondition
            Assert.IsNotNull(calculation.Output);

            // Call
            IEnumerable<IObservable> affectedObjects = WaveImpactAsphaltCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsNull(calculation.Output);

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, affectedObjects);
        }

        [Test]
        public void ClearWaveConditionsCalculation_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = null
            };

            // Call
            IEnumerable<IObservable> affectedObjects = WaveImpactAsphaltCoverDataSynchronizationService.ClearWaveConditionsCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(affectedObjects);
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
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsOutputAndReturnsAffectedObjects()
        {
            // Setup
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            WaveImpactAsphaltCoverWaveConditionsCalculation[] calculations = failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>().ToArray();
            IObservable[] expectedAffectedCalculations = calculations.Where(c => c.HasOutput)
                                                                     .Cast<IObservable>()
                                                                     .ToArray();
            IObservable[] expectedAffectedCalculationInputs = calculations.Select(c => c.InputParameters)
                                                                          .Where(i => i.HydraulicBoundaryLocation != null)
                                                                          .Cast<IObservable>()
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems =
                WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>()
                                          .All(c => c.InputParameters.HydraulicBoundaryLocation == null &&
                                                    !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations.Concat(expectedAffectedCalculationInputs),
                                           affectedItems);
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
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();
            // Call
            IEnumerable<IObservable> affectedItems =
                WaveImpactAsphaltCoverDataSynchronizationService.ClearAllWaveConditionsCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.All(c => !c.HasOutput));

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

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.WaveConditionsCalculationGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.ForeshoreProfiles)
                                                              .ToArray();

            // Call
            ClearResults results = WaveImpactAsphaltCoverDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.WaveConditionsCalculationGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(4, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
            CollectionAssert.Contains(array, failureMechanism.WaveConditionsCalculationGroup);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
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