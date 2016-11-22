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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
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
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
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
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
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
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
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
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
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
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => HeightStructuresDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            // Call
            IEnumerable<IObservable> observables = HeightStructuresDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should not be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.HeightStructures);

            IObservable[] array = observables.ToArray();
            Assert.AreEqual(4, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(array, failureMechanism.HeightStructures);
        }

        private HeightStructuresFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(-1, 0),
                new Point2D(2, 0)
            });
            var structure = new TestHeightStructure();
            var profile = new TestForeshoreProfile();
            var heightStructuresFailureMechanism = new HeightStructuresFailureMechanism
            {
                HeightStructures =
                {
                    structure
                },
                ForeshoreProfiles =
                {
                    profile
                },
                CalculationsGroup =
                {
                    Children =
                    {
                        new TestHeightStructuresCalculation
                        {
                            InputParameters =
                            {
                                ForeshoreProfile = profile,
                                Structure = structure
                            }
                        },
                        new CalculationGroup("B", true)
                    }
                }
            };
            heightStructuresFailureMechanism.AddSection(section);
            return heightStructuresFailureMechanism;
        }
    }
}