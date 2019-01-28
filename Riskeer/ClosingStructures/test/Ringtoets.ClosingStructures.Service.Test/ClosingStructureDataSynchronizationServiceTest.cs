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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service;

namespace Ringtoets.ClosingStructures.Service.Test
{
    [TestFixture]
    public class ClosingStructuresDataSynchronizationServiceTest
    {
        [Test]
        public void RemoveStructure_StructureNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ClosingStructuresDataSynchronizationService.RemoveStructure(
                null,
                new ClosingStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structure", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ClosingStructuresDataSynchronizationService.RemoveStructure(
                new TestClosingStructure(),
                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FullyConfiguredFailureMechanism_RemovesStructureAndClearsDependentData()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var locationStructureToRemove = new Point2D(0, 0);
            var structureToRemove = new TestClosingStructure(locationStructureToRemove, "id1");

            var locationStructureToKeep = new Point2D(2, 2);
            var structureToKeep = new TestClosingStructure(locationStructureToKeep, "id2");

            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structureToRemove,
                structureToKeep
            }, "path/to/structures");

            var calculationWithOutput = new TestClosingStructuresCalculation
            {
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureToRemove = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    Structure = structureToRemove
                }
            };
            var calculationWithStructureToKeepAndOutput = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    Structure = structureToKeep
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureToRemoveAndOutput = new TestClosingStructuresCalculation
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

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    locationStructureToRemove,
                    new Point2D(1, 1)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(1, 1),
                    locationStructureToKeep
                })
            });

            ClosingStructuresFailureMechanismSectionResult sectionWithCalculationAtStructureToRemove = failureMechanism.SectionResults.ElementAt(0);
            sectionWithCalculationAtStructureToRemove.Calculation = calculationWithStructureToRemove;

            ClosingStructuresFailureMechanismSectionResult sectionWithCalculationAtStructureToKeep = failureMechanism.SectionResults.ElementAt(1);
            sectionWithCalculationAtStructureToKeep.Calculation = calculationWithStructureToKeepAndOutput;

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
            Assert.IsNull(sectionWithCalculationAtStructureToRemove.Calculation);
            Assert.IsNotNull(calculationWithOutput.Output);
            Assert.IsNotNull(calculationWithStructureToKeepAndOutput.Output);
            Assert.IsNotNull(calculationWithStructureToKeepAndOutput.InputParameters.Structure);
            Assert.AreSame(sectionWithCalculationAtStructureToKeep.Calculation, calculationWithStructureToKeepAndOutput);

            IObservable[] expectedAffectedObjects =
            {
                calculationWithStructureToRemove.InputParameters,
                calculationWithStructureToRemoveAndOutput,
                calculationWithStructureToRemoveAndOutput.InputParameters,
                sectionWithCalculationAtStructureToRemove,
                failureMechanism.ClosingStructures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void RemoveAllStructures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ClosingStructuresDataSynchronizationService.RemoveAllStructures(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveAllStructures_FullyConfiguredFailureMechanism_RemoveAllStructuresAndClearDependentData()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var locationStructureA = new Point2D(0, 0);
            var structureA = new TestClosingStructure(locationStructureA, "A");

            var locationStructureB = new Point2D(2, 2);
            var structureB = new TestClosingStructure(locationStructureB, "B");

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

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    locationStructureA,
                    new Point2D(1, 1)
                }),
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    new Point2D(1, 1),
                    locationStructureB
                })
            });

            ClosingStructuresFailureMechanismSectionResult sectionWithCalculationAtStructureA = failureMechanism.SectionResults.ElementAt(0);
            sectionWithCalculationAtStructureA.Calculation = calculationWithStructureA;

            ClosingStructuresFailureMechanismSectionResult sectionWithCalculationAtStructureB = failureMechanism.SectionResults.ElementAt(1);
            sectionWithCalculationAtStructureB.Calculation = calculationWithStructureBAndOutput;

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
            Assert.IsNull(sectionWithCalculationAtStructureA.Calculation);
            Assert.IsNull(sectionWithCalculationAtStructureB.Calculation);
            Assert.IsNotNull(calculationWithOutput.Output);

            IObservable[] expectedAffectedObjects =
            {
                calculationWithStructureA.InputParameters,
                calculationWithStructureAAndOutput,
                calculationWithStructureAAndOutput.InputParameters,
                calculationWithStructureBAndOutput,
                calculationWithStructureBAndOutput.InputParameters,
                sectionWithCalculationAtStructureA,
                sectionWithCalculationAtStructureB,
                failureMechanism.ClosingStructures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ClosingStructuresDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
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
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ClosingStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_CalculationsWithHydraulicBoundaryLocationAndOutput_ClearsHydraulicBoundaryLocationAndCalculationsAndReturnsAffectedObjects()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestStructuresOutput()
            };

            var calculation2 = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                },
                Output = new TestStructuresOutput()
            };

            var calculation3 = new StructuresCalculation<ClosingStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = ClosingStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in failureMechanism.CalculationsGroup.Children.Cast<StructuresCalculation<ClosingStructuresInput>>())
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
            var failureMechanism = new ClosingStructuresFailureMechanism();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0, 0);

            var calculation1 = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation2 = new StructuresCalculation<ClosingStructuresInput>
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculation3 = new StructuresCalculation<ClosingStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = ClosingStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in failureMechanism.CalculationsGroup.Children.Cast<StructuresCalculation<ClosingStructuresInput>>())
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
            IEnumerable<IObservable> affectedItems = ClosingStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in failureMechanism.CalculationsGroup.Children.Cast<StructuresCalculation<ClosingStructuresInput>>())
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
            var failureMechanism = new ClosingStructuresFailureMechanism();

            var calculation1 = new StructuresCalculation<ClosingStructuresInput>();
            var calculation2 = new StructuresCalculation<ClosingStructuresInput>();
            var calculation3 = new StructuresCalculation<ClosingStructuresInput>();

            failureMechanism.CalculationsGroup.Children.Add(calculation1);
            failureMechanism.CalculationsGroup.Children.Add(calculation2);
            failureMechanism.CalculationsGroup.Children.Add(calculation3);

            // Call
            IEnumerable<IObservable> affectedItems = ClosingStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ClosingStructuresDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
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

        private ClosingStructuresFailureMechanism CreateFullyConfiguredFailureMechanism()
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
            var structure1 = new TestClosingStructure(new Point2D(1, 0), "structure1");
            var structure2 = new TestClosingStructure(new Point2D(3, 0), "structure2");
            var profile = new TestForeshoreProfile();
            StructuresCalculation<ClosingStructuresInput> calculation1 = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure1
                },
                Output = new TestStructuresOutput()
            };
            StructuresCalculation<ClosingStructuresInput> calculation2 = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure2
                }
            };
            StructuresCalculation<ClosingStructuresInput> calculation3 = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure1
                }
            };
            var failureMechanism = new ClosingStructuresFailureMechanism
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
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structure1,
                structure2
            }, "some path");
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile
            }, "path");

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section1,
                section2
            });
            ClosingStructuresFailureMechanismSectionResult result1 = failureMechanism.SectionResults
                                                                                     .First(sr => ReferenceEquals(sr.Section, section1));
            ClosingStructuresFailureMechanismSectionResult result2 = failureMechanism.SectionResults
                                                                                     .First(sr => ReferenceEquals(sr.Section, section2));
            result1.Calculation = calculation1;
            result2.Calculation = calculation2;

            return failureMechanism;
        }
    }
}