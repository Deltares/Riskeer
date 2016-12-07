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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Probability;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service;

namespace Ringtoets.GrassCoverErosionInwards.Utils.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsDataSynchronizationServiceTest
    {
        [Test]
        public void ClearCalculationOutput_CalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculation", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutput_WithCalculation_ClearsOutput()
        {
            // Setup
            GrassCoverErosionInwardsCalculation calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };

            // Call
            IEnumerable<IObservable> changedObjects = GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsNull(calculation.Output);

            CollectionAssert.AreEqual(new[]
            {
                calculation
            }, changedObjects);
        }

        [Test]
        public void ClearCalculationOutput_CalculationWithoutOutput_DoNothing()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = null
            };

            // Call
            IEnumerable<IObservable> changedObjects = GrassCoverErosionInwardsDataSynchronizationService.ClearCalculationOutput(calculation);

            // Assert
            CollectionAssert.IsEmpty(changedObjects);
        }

        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.All(c => !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            IEnumerable<GrassCoverErosionInwardsCalculation> grassCoverErosionInwardsCalculations =
                failureMechanism.Calculations
                                .Cast<GrassCoverErosionInwardsCalculation>()
                                .ToArray();
            IObservable[] expectedAffectedCalculations = grassCoverErosionInwardsCalculations
                .Where(c => c.HasOutput)
                .Cast<IObservable>()
                .ToArray();
            IObservable[] expectedAffectedCalculationInputs = grassCoverErosionInwardsCalculations
                .Select(c => c.InputParameters)
                .Where(i => i.HydraulicBoundaryLocation != null)
                .Cast<IObservable>()
                .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems =
                GrassCoverErosionInwardsDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.Cast<GrassCoverErosionInwardsCalculation>()
                                          .All(c => c.InputParameters.HydraulicBoundaryLocation == null &&
                                                    !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations.Concat(expectedAffectedCalculationInputs),
                                           affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            GrassCoverErosionInwardsFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            // Call
            IEnumerable<IObservable> observables = GrassCoverErosionInwardsDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.DikeProfiles);

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(3, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.DikeProfiles);
        }

        private static GrassCoverErosionInwardsFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation = new GrassCoverErosionInwardsCalculation();
            var calculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var calculationWithHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new GrassCoverErosionInwardsCalculation();
            var subCalculationWithOutput = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new GrassCoverErosionInwardsOutput(0, false, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0)
            };
            var subCalculationWithHydraulicBoundaryLocation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
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