// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;

namespace Riskeer.HeightStructures.Service.Test
{
    [TestFixture]
    public class HeightStructuresDataSynchronizationServiceTest
    {
        [Test]
        public void RemoveStructure_StructureNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HeightStructuresDataSynchronizationService.RemoveStructure(null, new HeightStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("structure", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HeightStructuresDataSynchronizationService.RemoveStructure(new TestHeightStructure(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FullyConfiguredFailureMechanism_RemovesStructureAndClearsDependentData()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var structureToRemove = new TestHeightStructure(new Point2D(0, 0), "id1");
            var structureToKeep = new TestHeightStructure(new Point2D(2, 2), "id2");

            failureMechanism.HeightStructures.AddRange(new[]
            {
                structureToRemove,
                structureToKeep
            }, "path/to/structures");

            var calculationWithOutput = new TestHeightStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureToRemove = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structureToRemove
                }
            };
            var calculationWithStructureToKeepAndOutput = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structureToKeep
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureToRemoveAndOutput = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structureToRemove
                },
                Output = new TestStructuresOutput()
            };
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculationWithOutput,
                calculationWithStructureToRemove,
                calculationWithStructureToKeepAndOutput,
                calculationWithStructureToRemoveAndOutput
            });

            // Call
            IEnumerable<IObservable> affectedObjects = HeightStructuresDataSynchronizationService.RemoveStructure(
                structureToRemove, failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.HeightStructures, structureToRemove);
            Assert.IsNull(calculationWithStructureToRemove.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureToRemoveAndOutput.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureToRemoveAndOutput.Output);
            Assert.IsNotNull(calculationWithOutput.Output);
            Assert.IsNotNull(calculationWithStructureToKeepAndOutput.Output);
            Assert.IsNotNull(calculationWithStructureToKeepAndOutput.InputParameters.Structure);

            IObservable[] expectedAffectedObjects =
            {
                calculationWithStructureToRemove.InputParameters,
                calculationWithStructureToRemoveAndOutput,
                calculationWithStructureToRemoveAndOutput.InputParameters,
                failureMechanism.HeightStructures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void RemoveAllStructures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HeightStructuresDataSynchronizationService.RemoveAllStructures(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveAllStructures_FullyConfiguredFailureMechanism_RemoveAllStructuresAndClearDependentData()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var structureA = new TestHeightStructure(new Point2D(0, 0), "A");
            var structureB = new TestHeightStructure(new Point2D(2, 2), "B");

            failureMechanism.HeightStructures.AddRange(new[]
            {
                structureA,
                structureB
            }, "path/to/structures");

            var calculationWithOutput = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureA = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = structureA
                }
            };
            var calculationWithStructureBAndOutput = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = structureB
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureAAndOutput = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = structureA
                },
                Output = new TestStructuresOutput()
            };
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                calculationWithOutput,
                calculationWithStructureA,
                calculationWithStructureBAndOutput,
                calculationWithStructureAAndOutput
            });

            // Call
            IEnumerable<IObservable> affectedObjects = HeightStructuresDataSynchronizationService.RemoveAllStructures(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.HeightStructures, structureA);
            Assert.IsNull(calculationWithStructureA.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureAAndOutput.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureBAndOutput.InputParameters.Structure);
            Assert.IsNull(calculationWithStructureAAndOutput.Output);
            Assert.IsNull(calculationWithStructureBAndOutput.Output);
            Assert.IsNotNull(calculationWithOutput.Output);

            IObservable[] expectedAffectedObjects =
            {
                calculationWithStructureA.InputParameters,
                calculationWithStructureAAndOutput,
                calculationWithStructureAAndOutput.InputParameters,
                calculationWithStructureBAndOutput,
                calculationWithStructureBAndOutput.InputParameters,
                failureMechanism.HeightStructures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithOutput_ClearsCalculationsOutput()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (ICalculation calculation in failureMechanism.Calculations)
            {
                Assert.IsFalse(calculation.HasOutput);
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
            void Call() => HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationAndOutput_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
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
                Output = new TestStructuresOutput()
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestStructuresOutput()
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
                Assert.IsNull(calculation.Output);
            }

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation1,
                calculation1.InputParameters,
                calculation2,
                calculation2.InputParameters
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationNoOutput_ClearsHydraulicBoundaryLocationAndReturnsAffectedInputs()
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
            // the return result, no ToArray() should be called before these assertions:
            foreach (StructuresCalculation<HeightStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>())
            {
                Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
            }

            CollectionAssert.AreEqual(new[]
            {
                calculation1.InputParameters,
                calculation2.InputParameters
            }, affectedItems);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithOutputAndNoHydraulicBoundaryLocation_ClearsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            var calculation1 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var calculation2 = new StructuresCalculation<HeightStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var calculation3 = new StructuresCalculation<HeightStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = HeightStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (ICalculation calculation in failureMechanism.Calculations)
            {
                Assert.IsFalse(calculation.HasOutput);
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
            void Call() => HeightStructuresDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            HeightStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.ForeshoreProfiles)
                                                              .Concat(failureMechanism.HeightStructures)
                                                              .ToArray();

            // Call
            ClearResults results = HeightStructuresDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.HeightStructures);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(5, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(array, failureMechanism.HeightStructures);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
        }

        private HeightStructuresFailureMechanism CreateFullyConfiguredFailureMechanism()
        {
            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(-1, 0),
                new Point2D(2, 0)
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(2, 0),
                new Point2D(4, 0)
            });
            var structure1 = new TestHeightStructure(new Point2D(1, 0), "Id 1,0");
            var structure2 = new TestHeightStructure(new Point2D(3, 0), "Id 3,0");
            var profile = new TestForeshoreProfile();
            StructuresCalculation<HeightStructuresInput> calculation1 = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure1
                },
                Output = new TestStructuresOutput()
            };
            StructuresCalculation<HeightStructuresInput> calculation2 = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure2
                }
            };
            StructuresCalculation<HeightStructuresInput> calculation3 = new TestHeightStructuresCalculationScenario
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure1
                }
            };
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation1,
                        new CalculationGroup
                        {
                            Children =
                            {
                                calculation2
                            }
                        },
                        calculation3
                    }
                }
            };
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile
            }, "path");

            failureMechanism.HeightStructures.AddRange(new[]
            {
                structure1,
                structure2
            }, "someLocation");

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section1,
                section2
            });
            HeightStructuresFailureMechanismSectionResult result1 = failureMechanism.SectionResults
                                                                                    .First(sr => ReferenceEquals(sr.Section, section1));
            HeightStructuresFailureMechanismSectionResult result2 = failureMechanism.SectionResults
                                                                                    .First(sr => ReferenceEquals(sr.Section, section2));
            result1.Calculation = calculation1;
            result2.Calculation = calculation2;

            return failureMechanism;
        }
    }
}