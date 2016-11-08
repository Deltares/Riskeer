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
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.HydraRing.Data;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.StabilityPointStructures.Service.Test
{
    [TestFixture]
    public class StabilityPointStructuresDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            var expectedAffectedCalculations = failureMechanism.Calculations.Where(c => c.HasOutput)
                                                               .ToArray();

            // Call
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> affectedItems =
                StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            Assert.IsFalse(failureMechanism.Calculations.Any(c => c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            var expectedAffectedCalculations = failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                                               .Where(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput)
                                                               .ToArray();

            // Call
            IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> affectedItems =
                StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            Assert.IsFalse(failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                           .Any(c => c.InputParameters.HydraulicBoundaryLocation != null || c.HasOutput));
            CollectionAssert.AreEquivalent(expectedAffectedCalculations, affectedItems);
        }

        private static StabilityPointStructuresFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var calculationWithOutput = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var calculationWithHydraulicBoundaryLocation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var subCalculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var subCalculationWithOutput = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };
            var subCalculationWithHydraulicBoundaryLocation = new StructuresCalculation<StabilityPointStructuresInput>
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

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(subCalculation);
            calculationGroup.Children.Add(subCalculationWithOutput);
            calculationGroup.Children.Add(subCalculationWithOutputAndHydraulicBoundaryLocation);
            calculationGroup.Children.Add(subCalculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationGroup);

            return failureMechanism;
        }
    }
}