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
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;

namespace Riskeer.StabilityPointStructures.Service.Test
{
    [TestFixture]
    public class StabilityPointStructuresDataSynchronizationServiceTest
    {
        [Test]
        public void RemoveAllStructures_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StabilityPointStructuresDataSynchronizationService.RemoveAllStructures(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void RemoveAllStructures_FullyConfiguredFailureMechanism_RemoveAllStructuresAndClearDependentData()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            var structureA = new TestStabilityPointStructure(new Point2D(0, 0), "A");
            var structureB = new TestStabilityPointStructure(new Point2D(2, 2), "B");

            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                structureA,
                structureB
            }, "path/to/structures");

            var calculationWithOutput = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureA = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = structureA
                }
            };
            var calculationWithStructureBAndOutput = new StructuresCalculation<StabilityPointStructuresInput>
            {
                InputParameters =
                {
                    Structure = structureB
                },
                Output = new TestStructuresOutput()
            };
            var calculationWithStructureAAndOutput = new StructuresCalculation<StabilityPointStructuresInput>
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
            IEnumerable<IObservable> affectedObjects = StabilityPointStructuresDataSynchronizationService.RemoveAllStructures(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.StabilityPointStructures, structureA);
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
                failureMechanism.StabilityPointStructures
            };
            CollectionAssert.AreEquivalent(expectedAffectedObjects, affectedObjects);
        }

        [Test]
        public void ClearAllCalculationOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutput_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedCalculations()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            ICalculation[] expectedAffectedCalculations = failureMechanism.Calculations
                                                                          .Where(c => c.HasOutput)
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems = StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutput(failureMechanism);

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
            void Call() => StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void ClearAllCalculationOutputAndHydraulicBoundaryLocations_WithVariousCalculations_ClearsCalculationsOutputAndReturnsAffectedObjects()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            StructuresCalculation<StabilityPointStructuresInput>[] calculations = failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>().ToArray();
            IObservable[] expectedAffectedCalculations = calculations.Where(c => c.HasOutput)
                                                                     .Cast<IObservable>()
                                                                     .ToArray();
            IObservable[] expectedAffectedCalculationInputs = calculations.Select(c => c.InputParameters)
                                                                          .Where(i => i.HydraulicBoundaryLocation != null)
                                                                          .Cast<IObservable>()
                                                                          .ToArray();

            // Call
            IEnumerable<IObservable> affectedItems =
                StabilityPointStructuresDataSynchronizationService.ClearAllCalculationOutputAndHydraulicBoundaryLocations(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            Assert.IsTrue(failureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                          .All(c => c.InputParameters.HydraulicBoundaryLocation == null && !c.HasOutput));

            CollectionAssert.AreEquivalent(expectedAffectedCalculations.Concat(expectedAffectedCalculationInputs),
                                           affectedItems);
        }

        [Test]
        public void ClearReferenceLineDependentData_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => StabilityPointStructuresDataSynchronizationService.ClearReferenceLineDependentData(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(Call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void ClearReferenceLineDependentData_FullyConfiguredFailureMechanism_RemoveFailureMechanismDependentData()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();

            object[] expectedRemovedObjects = failureMechanism.Sections.OfType<object>()
                                                              .Concat(failureMechanism.SectionResults)
                                                              .Concat(failureMechanism.CalculationsGroup.GetAllChildrenRecursive())
                                                              .Concat(failureMechanism.ForeshoreProfiles)
                                                              .Concat(failureMechanism.StabilityPointStructures)
                                                              .ToArray();

            // Call
            ClearResults results = StabilityPointStructuresDataSynchronizationService.ClearReferenceLineDependentData(failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SectionResults);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.ForeshoreProfiles);
            CollectionAssert.IsEmpty(failureMechanism.StabilityPointStructures);

            IObservable[] array = results.ChangedObjects.ToArray();
            Assert.AreEqual(5, array.Length);
            CollectionAssert.Contains(array, failureMechanism);
            CollectionAssert.Contains(array, failureMechanism.SectionResults);
            CollectionAssert.Contains(array, failureMechanism.CalculationsGroup);
            CollectionAssert.Contains(array, failureMechanism.ForeshoreProfiles);
            CollectionAssert.Contains(array, failureMechanism.StabilityPointStructures);

            CollectionAssert.AreEquivalent(expectedRemovedObjects, results.RemovedObjects);
        }

        [Test]
        public void RemoveStructure_StructureNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StabilityPointStructuresDataSynchronizationService.RemoveStructure(null, new StabilityPointStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("structure", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => StabilityPointStructuresDataSynchronizationService.RemoveStructure(new TestStabilityPointStructure(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void RemoveStructure_FullyConfiguredFailureMechanism_RemovesStructureAndClearsDependentData()
        {
            // Setup
            StabilityPointStructuresFailureMechanism failureMechanism = CreateFullyConfiguredFailureMechanism();
            StabilityPointStructure structure = failureMechanism.StabilityPointStructures[0];
            StructuresCalculation<StabilityPointStructuresInput>[] calculationsWithStructure = failureMechanism.Calculations
                                                                                                               .Cast<StructuresCalculation<StabilityPointStructuresInput>>()
                                                                                                               .Where(c => ReferenceEquals(c.InputParameters.Structure, structure))
                                                                                                               .ToArray();
            StructuresCalculation<StabilityPointStructuresInput>[] calculationsWithOutput = calculationsWithStructure.Where(c => c.HasOutput)
                                                                                                                     .ToArray();

            // Precondition
            CollectionAssert.IsNotEmpty(calculationsWithOutput);
            CollectionAssert.IsNotEmpty(calculationsWithStructure);

            // Call
            IEnumerable<IObservable> affectedObjects = StabilityPointStructuresDataSynchronizationService.RemoveStructure(structure, failureMechanism);

            // Assert
            // Note: To make sure the clear is performed regardless of what is done with
            // the return result, no ToArray() should be called before these assertions:
            CollectionAssert.DoesNotContain(failureMechanism.StabilityPointStructures, structure);
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculationsWithStructure)
            {
                Assert.IsNull(calculation.InputParameters.Structure);
            }

            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculationsWithOutput)
            {
                Assert.IsFalse(calculation.HasOutput);
            }

            IObservable[] array = affectedObjects.ToArray();
            Assert.AreEqual(1 + calculationsWithOutput.Length + calculationsWithStructure.Length, array.Length);
            CollectionAssert.Contains(array, failureMechanism.StabilityPointStructures);
            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculationsWithStructure)
            {
                CollectionAssert.Contains(array, calculation.InputParameters);
            }

            foreach (StructuresCalculation<StabilityPointStructuresInput> calculation in calculationsWithOutput)
            {
                CollectionAssert.Contains(array, calculation);
            }
        }

        private StabilityPointStructuresFailureMechanism CreateFullyConfiguredFailureMechanism()
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
            var structure1 = new TestStabilityPointStructure(new Point2D(1, 0), "id structure1");
            var structure2 = new TestStabilityPointStructure(new Point2D(3, 0), "id structure2");
            var profile = new TestForeshoreProfile();
            StructuresCalculation<StabilityPointStructuresInput> calculation1 = new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure1
                },
                Output = new TestStructuresOutput()
            };
            StructuresCalculation<StabilityPointStructuresInput> calculation2 = new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure2
                }
            };
            StructuresCalculation<StabilityPointStructuresInput> calculation3 = new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    ForeshoreProfile = profile,
                    Structure = structure1
                }
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism
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
            failureMechanism.StabilityPointStructures.AddRange(new[]
            {
                structure1,
                structure2
            }, "path");

            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                profile
            }, "path");

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                section1,
                section2
            });

            return failureMechanism;
        }
    }
}