// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;

namespace Riskeer.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresDataSynchronizationServiceTest
    {
        [Test]
        public void RemoveStructure_StructureNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ClosingStructuresDataSynchronizationService.RemoveStructure(null, new ClosingStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("structure", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ClosingStructuresDataSynchronizationService.RemoveStructure(new TestClosingStructure(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FullyConfiguredFailureMechanism_RemovesStructureAndClearsDependentData()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var structureToRemove = new TestClosingStructure(new Point2D(0, 0), "id1");
            var structureToKeep = new TestClosingStructure(new Point2D(2, 2), "id2");

            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structureToRemove,
                structureToKeep
            }, "path/to/structures");

            var calculationWithOutput = new TestClosingStructuresCalculationScenario
            {
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureToRemove = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structureToRemove
                }
            };
            var calculationWithStructureToKeepAndOutput = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structureToKeep
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureToRemoveAndOutput = new TestClosingStructuresCalculationScenario
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
            IEnumerable<IObservable> affectedObjects = ClosingStructuresDataSynchronizationService.RemoveStructure(
                structureToRemove, failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ClosingStructures, structureToRemove);
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
                failureMechanism.ClosingStructures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void RemoveAllStructures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ClosingStructuresDataSynchronizationService.RemoveAllStructures(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveAllStructures_FullyConfiguredFailureMechanism_RemoveAllStructuresAndClearDependentData()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var structureA = new TestClosingStructure(new Point2D(0, 0), "A");
            var structureB = new TestClosingStructure(new Point2D(2, 2), "B");

            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structureA,
                structureB
            }, "path/to/structures");

            var calculationWithOutput = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureA = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structureA
                }
            };
            var calculationWithStructureBAndOutput = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structureB
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureAAndOutput = new StructuresCalculation<ClosingStructuresInput>
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
            IEnumerable<IObservable> affectedObjects = ClosingStructuresDataSynchronizationService.RemoveAllStructures(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.ClosingStructures, structureA);
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
                failureMechanism.ClosingStructures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ClosingStructuresDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithOutput_ClearsCalculationsOutput()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var calculation1 = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var calculation2 = new StructuresCalculation<ClosingStructuresInput>
            {
                Output = new TestStructuresOutput()
            };

            var calculation3 = new StructuresCalculation<ClosingStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = ClosingStructuresDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>())
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
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ClosingStructuresDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(
                null, Enumerable.Empty<HydraulicBoundaryLocation>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ClosingStructuresDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(
                new ClosingStructuresFailureMechanism(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void ClearCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            var hydraulicBoundaryLocation1 = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocation2 = new TestHydraulicBoundaryLocation();

            ClosingStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism(hydraulicBoundaryLocation1);
            failureMechanism.CalculationsGroup.Children.AddRange(new[]
            {
                new StructuresCalculationScenario<ClosingStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation2
                    }
                },
                new StructuresCalculationScenario<ClosingStructuresInput>
                {
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation2
                    },
                    Output = new TestStructuresOutput()
                }
            });

            IEnumerable<StructuresCalculationScenario<ClosingStructuresInput>> calculations = failureMechanism.Calculations.Cast<StructuresCalculationScenario<ClosingStructuresInput>>()
                                                                                                              .ToArray();

            IEnumerable<StructuresCalculationScenario<ClosingStructuresInput>> expectedAffectedCalculations = calculations.Where(
                c => c.InputParameters.HydraulicBoundaryLocation == hydraulicBoundaryLocation1
                     && c.HasOutput).ToArray();

            var expectedAffectedItems = new List<IObservable>(expectedAffectedCalculations);
            expectedAffectedItems.AddRange(calculations.Select(c => c.InputParameters)
                                                       .Where(i => i.HydraulicBoundaryLocation == hydraulicBoundaryLocation1));

            // Call
            IEnumerable<IObservable> affectedItems = ClosingStructuresDataSynchronizationService.ClearCalculationOutputAndHydraulicBoundaryLocations(
                failureMechanism, new[]
                {
                    hydraulicBoundaryLocation1
                });

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(expectedAffectedCalculations.All(c => !c.HasOutput));
            Assert.IsTrue(calculations.All(c => c.InputParameters.HydraulicBoundaryLocation != hydraulicBoundaryLocation1));

            CollectionAssert.AreEquivalent(expectedAffectedItems, affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ClosingStructuresDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            ClosingStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.ForeshoreProfiles)
                                                              .Concat(failureMechanism.ClosingStructures)
                                                              .ToArray();

            // Call
            ClearResults results = ClosingStructuresDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.ClosingStructures);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(5, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(array, failureMechanism.ClosingStructures);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
        }

        private ClosingStructuresFailureMechanism CreateFullyConfiguredFailureMechanism(HydraulicBoundaryLocation hydraulicBoundaryLocation = null)
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
            var structure1 = new TestClosingStructure(new Point2D(1, 0), "Id 1,0");
            var structure2 = new TestClosingStructure(new Point2D(3, 0), "Id 3,0");
            var profile = new TestForeshoreProfile();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile
            }, "path");

            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structure1,
                structure2
            }, "someLocation");

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section1,
                section2
            });

            if (hydraulicBoundaryLocation == null)
            {
                hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            }

            var calculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure1
                }
            };
            var calculationWithOutput = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure1
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure2,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithHydraulicBoundaryLocation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure1,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var calculationWithHydraulicBoundaryLocationAndForeshoreProfile = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure2,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile
                }
            };
            var calculationWithForeshoreProfile = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure1,
                    ForeshoreProfile = profile
                }
            };
            var calculationWithOutputHydraulicBoundaryLocationAndForeshoreProfile = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure1,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile
                },
                Output = new TestStructuresOutput()
            };

            var subCalculation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure2
                }
            };
            var subCalculationWithOutput = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure2
                },
                Output = new TestStructuresOutput()
            };
            var subCalculationWithOutputAndHydraulicBoundaryLocation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure1,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestStructuresOutput()
            };
            var subCalculationWithHydraulicBoundaryLocation = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure1,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };
            var subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure1,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile
                }
            };
            var subCalculationWithForeshoreProfile = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure2,
                    ForeshoreProfile = profile
                }
            };
            var subCalculationWithOutputHydraulicBoundaryLocationAndForeshoreProfile = new StructuresCalculationScenario<ClosingStructuresInput>
            {
                InputParameters =
                {
                    Structure = structure2,
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    ForeshoreProfile = profile
                },
                Output = new TestStructuresOutput()
            };

            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutput);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputAndHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocation);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithForeshoreProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithHydraulicBoundaryLocationAndForeshoreProfile);
            failureMechanism.CalculationsGroup.Children.Add(calculationWithOutputHydraulicBoundaryLocationAndForeshoreProfile);
            failureMechanism.CalculationsGroup.Children.Add(new CalculationGroup
            {
                Children =
                {
                    subCalculation,
                    subCalculationWithOutput,
                    subCalculationWithOutputAndHydraulicBoundaryLocation,
                    subCalculationWithHydraulicBoundaryLocation,
                    subCalculationWithForeshoreProfile,
                    subCalculationWithHydraulicBoundaryLocationAndForeshoreProfile,
                    subCalculationWithOutputHydraulicBoundaryLocationAndForeshoreProfile
                }
            });

            return failureMechanism;
        }
    }
}