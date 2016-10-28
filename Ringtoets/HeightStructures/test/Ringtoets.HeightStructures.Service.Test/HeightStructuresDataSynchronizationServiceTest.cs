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
using Ringtoets.Common.Data.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresDataSynchronizationServiceTest
    {
        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithOutput_ClearsCalculationsOutput()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StructuresCalculation<HeightStructuresInput>> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.CalculationsGroup.Children.Cast<StructuresCalculation<HeightStructuresInput>>())
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
        public void ClearCalculationOutput_WithCalculation_ClearsOutput()
        {
            // Setup
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void ClearHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresDataSynchronizationService.ClearHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearHydraulicBoundaryLocations_WithHydraulicBoundaryLocation_ClearsHydraulicBoundaryLocationAndReturnsAffectedCalculations()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StructuresCalculation<HeightStructuresInput>> affectedItems = HeightStructuresDataSynchronizationService.ClearHydraulicBoundaryLocations(failureMechanism);

            // Assert
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.CalculationsGroup.Children.Cast<StructuresCalculation<HeightStructuresInput>>())
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
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationAndOutput_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedCalculations()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StructuresCalculation<HeightStructuresInput>> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.CalculationsGroup.Children.Cast<StructuresCalculation<HeightStructuresInput>>())
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StructuresCalculation<HeightStructuresInput>> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.CalculationsGroup.Children.Cast<StructuresCalculation<HeightStructuresInput>>())
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
            var failureMechanism = new HeightStructuresFailureMechanism();

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new ProbabilityAssessmentOutput(0, 0, 0, 0, 0)
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StructuresCalculation<HeightStructuresInput>> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.CalculationsGroup.Children.Cast<StructuresCalculation<HeightStructuresInput>>())
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
            var failureMechanism = new HeightStructuresFailureMechanism();

            var calculation1 = new StructuresCalculation<HeightStructuresInput>();
            var calculation2 = new StructuresCalculation<HeightStructuresInput>();
            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<StructuresCalculation<HeightStructuresInput>> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(affectedItems);
        }
    }
}