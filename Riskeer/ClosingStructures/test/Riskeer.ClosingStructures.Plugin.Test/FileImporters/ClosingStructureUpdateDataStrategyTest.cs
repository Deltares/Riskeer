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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Plugin.FileImporters;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.Structures;

namespace Riskeer.ClosingStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class ClosingStructureUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ClosingStructureUpdateDataStrategy(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_CreatesNewInstance()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<UpdateDataStrategyBase<ClosingStructure, ClosingStructuresFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IStructureUpdateStrategy<ClosingStructure>>(strategy);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            void Call() => strategy.UpdateStructuresWithImportedData(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("importedDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            void Call() => strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithoutCurrentStructuresAndReadStructuresHaveDuplicateNames_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            var readStructures = new[]
            {
                new TestClosingStructure(duplicateId, "Structure"),
                new TestClosingStructure(duplicateId, "Other structure")
            };

            var targetCollection = new StructureCollection<ClosingStructure>();
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            void Call() => strategy.UpdateStructuresWithImportedData(readStructures, sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(Call);

            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructureAndImportedMultipleStructuresWithSameId_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            var expectedStructure = new TestClosingStructure(duplicateId, "expectedStructure");

            TestClosingStructure[] expectedCollection =
            {
                expectedStructure
            };

            var targetCollection = new StructureCollection<ClosingStructure>();
            targetCollection.AddRange(expectedCollection, sourceFilePath);

            var readStructures = new[]
            {
                new TestClosingStructure(duplicateId, "Structure"),
                new TestClosingStructure(duplicateId, "Other structure")
            };

            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            void Call() => strategy.UpdateStructuresWithImportedData(readStructures, sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(Call);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.AreEqual(expectedCollection, targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructuresAndImportedHasNoOverlap_UpdatesTargetCollection()
        {
            // Setup
            var targetStructure = new TestClosingStructure("target id");

            var failureMechanism = new ClosingStructuresFailureMechanism();
            StructureCollection<ClosingStructure> structures = failureMechanism.ClosingStructures;
            structures.AddRange(new[]
            {
                targetStructure
            }, sourceFilePath);

            var readStructure = new TestClosingStructure("read id");
            TestClosingStructure[] importedStructures =
            {
                readStructure
            };

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures,
                                                                                                 sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedStructures, structures);
            Assert.AreSame(readStructure, structures[0]);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                structures
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructuresAndImportedHasFullOverlap_UpdatesTargetCollection()
        {
            // Setup
            const string commonId = "common id";
            var targetStructure = new TestClosingStructure(commonId, "old name");

            var failureMechanism = new ClosingStructuresFailureMechanism();
            StructureCollection<ClosingStructure> structures = failureMechanism.ClosingStructures;
            structures.AddRange(new[]
            {
                targetStructure
            }, sourceFilePath);

            var readStructure = new TestClosingStructure(commonId, "new name");
            TestClosingStructure[] importedStructures =
            {
                readStructure
            };

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures, sourceFilePath);

            // Assert
            Assert.AreEqual(1, structures.Count);
            Assert.AreSame(targetStructure, structures[0]);
            AssertClosingStructures(readStructure, targetStructure);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                targetStructure,
                structures
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructuresAndImportedHasPartialOverlap_UpdatesTargetCollection()
        {
            // Setup
            const string commonId = "common id";
            var updatedStructure = new TestClosingStructure(commonId, "old name");
            var removedStructure = new TestClosingStructure("removed id");

            var failureMechanism = new ClosingStructuresFailureMechanism();
            StructureCollection<ClosingStructure> structures = failureMechanism.ClosingStructures;
            structures.AddRange(new[]
            {
                removedStructure,
                updatedStructure
            }, sourceFilePath);

            var structureToUpdateFrom = new TestClosingStructure(commonId, "new name");
            var addedStructure = new TestClosingStructure("added id");
            TestClosingStructure[] importedStructures =
            {
                structureToUpdateFrom,
                addedStructure
            };

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures, sourceFilePath);

            // Assert
            Assert.AreEqual(2, structures.Count);
            Assert.AreSame(updatedStructure, structures[0]);
            AssertClosingStructures(structureToUpdateFrom, updatedStructure);

            Assert.AreSame(addedStructure, structures[1]);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                updatedStructure,
                structures
            }, affectedObjects);
        }

        [Test]
        [TestCaseSource(typeof(ClosingStructurePermutationHelper),
            nameof(ClosingStructurePermutationHelper.DifferentClosingStructuresWithSameId),
            new object[]
            {
                "UpdateStructuresWithImportedData",
                "UpdatesOnlySingleChange"
            })]
        public void UpdateStructuresWithImportedData_SingleChange_UpdatesOnlySingleChange(ClosingStructure readStructure)
        {
            // Setup
            ClosingStructure structure = new TestClosingStructure();

            var failureMechanism = new ClosingStructuresFailureMechanism();
            StructureCollection<ClosingStructure> targetCollection = failureMechanism.ClosingStructures;
            targetCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);
            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            AssertClosingStructures(readStructure, structure);
            CollectionAssert.AreEqual(new IObservable[]
            {
                targetCollection,
                structure
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithStructureImportedStructureWithSameId_UpdatesCalculationInput()
        {
            // Setup
            const string sameId = "sameId";
            ClosingStructure readStructure = new TestClosingStructure(sameId, "new structure");
            ClosingStructure structure = new TestClosingStructure(sameId, "original structure");

            var calculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(calculation.HasOutput);
            AssertClosingStructures(readStructure, structure);
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.ClosingStructures,
                structure,
                calculation.InputParameters
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_MultipleCalculationWithAssignedStructure_OnlyUpdatesCalculationInputWithUpdatedStructure()
        {
            // Setup
            const string affectedId = "affectedId";
            const string unaffectedId = "unaffectedId";
            const string unaffectedStructureName = "unaffectedStructure";
            var affectedStructure = new TestClosingStructure(affectedId, "Old name");
            var unaffectedStructure = new TestClosingStructure(unaffectedId, unaffectedStructureName);

            var affectedCalculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = affectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var unaffectedCalculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = unaffectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        affectedCalculation,
                        unaffectedCalculation
                    }
                }
            };
            StructureCollection<ClosingStructure> targetDataCollection = failureMechanism.ClosingStructures;
            targetDataCollection.AddRange(new[]
            {
                affectedStructure,
                unaffectedStructure
            }, sourceFilePath);

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            ClosingStructure readAffectedStructure = new TestClosingStructure(affectedId, "New name");
            ClosingStructure readUnaffectedStructure = new TestClosingStructure(unaffectedId, unaffectedStructureName);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                readAffectedStructure,
                readUnaffectedStructure
            }, sourceFilePath);
            // Assert
            Assert.IsTrue(affectedCalculation.HasOutput);
            ClosingStructure inputParametersAffectedStructure = affectedCalculation.InputParameters.Structure;
            Assert.AreSame(affectedStructure, inputParametersAffectedStructure);
            AssertClosingStructures(affectedStructure, inputParametersAffectedStructure);

            Assert.IsTrue(unaffectedCalculation.HasOutput);
            ClosingStructure inputParametersUnaffectedStructure = unaffectedCalculation.InputParameters.Structure;
            Assert.AreSame(unaffectedStructure, inputParametersUnaffectedStructure);
            AssertClosingStructures(readUnaffectedStructure, inputParametersUnaffectedStructure);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedStructure,
                affectedCalculation.InputParameters,
                targetDataCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithRemovedStructure_UpdatesCalculation()
        {
            // Setup
            const string sameId = "sameId";
            ClosingStructure structure = new TestClosingStructure(sameId, "original structure");

            var calculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };

            StructureCollection<ClosingStructure> targetDataCollection =
                failureMechanism.ClosingStructures;
            targetDataCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(),
                                                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.Structure);
            CollectionAssert.AreEqual(new IObservable[]
            {
                targetDataCollection,
                calculation,
                calculation.InputParameters
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_MultipleCalculationWithStructureOneWithRemovedStructure_OnlyUpdatesCalculationWithRemovedStructure()
        {
            // Setup
            const string removedId = "affectedId";
            const string unaffectedId = "unaffectedId";
            const string unaffectedStructureName = "unaffectedStructure";
            var removedStructure = new TestClosingStructure(removedId, "Old name");
            var unaffectedStructure = new TestClosingStructure(unaffectedId, unaffectedStructureName);

            var affectedCalculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = removedStructure
                },
                Output = new TestStructuresOutput()
            };

            var unaffectedCalculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = unaffectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        affectedCalculation,
                        unaffectedCalculation
                    }
                }
            };

            StructureCollection<ClosingStructure> targetDataCollection = failureMechanism.ClosingStructures;
            targetDataCollection.AddRange(new[]
            {
                removedStructure,
                unaffectedStructure
            }, sourceFilePath);

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            ClosingStructure readUnaffectedStructure = new TestClosingStructure(unaffectedId, unaffectedStructureName);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                readUnaffectedStructure
            }, sourceFilePath);
            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            Assert.IsNull(affectedCalculation.InputParameters.Structure);

            Assert.IsTrue(unaffectedCalculation.HasOutput);
            ClosingStructure inputParametersUnaffectedStructure = unaffectedCalculation.InputParameters.Structure;
            Assert.AreSame(unaffectedStructure, inputParametersUnaffectedStructure);
            AssertClosingStructures(readUnaffectedStructure, inputParametersUnaffectedStructure);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedCalculation.InputParameters,
                targetDataCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithSameReference_OnlyReturnsDistinctCalculationInput()
        {
            // Setup
            const string affectedId = "affectedId";
            var affectedStructure = new TestClosingStructure(affectedId, "Old name");
            var affectedCalculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = affectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        affectedCalculation,
                        affectedCalculation
                    }
                }
            };

            StructureCollection<ClosingStructure> structures = failureMechanism.ClosingStructures;
            structures.AddRange(new[]
            {
                affectedStructure
            }, sourceFilePath);

            var structureToUpdateFrom = new TestClosingStructure(affectedId, "New name");

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                structureToUpdateFrom
            }, sourceFilePath);
            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                structures,
                affectedStructure,
                affectedCalculation.InputParameters
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SectionResultWithStructureImportedStructureWithSameId_UpdatesCalculationInput()
        {
            // Setup
            const string sameId = "sameId";
            var originalMatchingPoint = new Point2D(0, 0);
            var updatedMatchingPoint = new Point2D(20, 20);
            ClosingStructure readStructure = new TestClosingStructure(updatedMatchingPoint, sameId);
            ClosingStructure structure = new TestClosingStructure(originalMatchingPoint, sameId);

            var calculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var intersectionPoint = new Point2D(10, 10);
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                new FailureMechanismSection("OldSection", new[]
                {
                    originalMatchingPoint,
                    intersectionPoint
                }),
                new FailureMechanismSection("NewSection", new[]
                {
                    intersectionPoint,
                    updatedMatchingPoint
                })
            });

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            AssertClosingStructures(readStructure, structure);

            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.ClosingStructures,
                structure,
                calculation.InputParameters,
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SectionResultWithStructureImportedStructureWithSameIdRemoved_UpdatesCalculationInput()
        {
            // Setup
            const string sameId = "id";
            var originalMatchingPoint = new Point2D(0, 0);
            ClosingStructure removedStructure = new TestClosingStructure(originalMatchingPoint, sameId);

            var calculation = new TestClosingStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = removedStructure
                }
            };
            var failureMechanism = new ClosingStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                removedStructure
            }, sourceFilePath);

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    originalMatchingPoint,
                    new Point2D(10, 10)
                })
            });

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(),
                                                                                                 sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.ClosingStructures,
                calculation.InputParameters,
            }, affectedObjects);
        }

        private static void AssertClosingStructures(ClosingStructure readStructure, ClosingStructure structure)
        {
            Assert.AreEqual(readStructure.Name, structure.Name);
            Assert.AreEqual(readStructure.Location, structure.Location);
            Assert.AreEqual(readStructure.StructureNormalOrientation, structure.StructureNormalOrientation);

            DistributionAssert.AreEqual(readStructure.AllowedLevelIncreaseStorage, structure.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(readStructure.AreaFlowApertures, structure.AreaFlowApertures);
            DistributionAssert.AreEqual(readStructure.CriticalOvertoppingDischarge, structure.CriticalOvertoppingDischarge);

            Assert.AreEqual(readStructure.FailureProbabilityOpenStructure, structure.FailureProbabilityOpenStructure);
            Assert.AreEqual(readStructure.FailureProbabilityReparation, structure.FailureProbabilityReparation);
            Assert.AreEqual(readStructure.IdenticalApertures, structure.IdenticalApertures);
            Assert.AreEqual(readStructure.InflowModelType, structure.InflowModelType);
            Assert.AreEqual(readStructure.ProbabilityOpenStructureBeforeFlooding, structure.ProbabilityOpenStructureBeforeFlooding);

            DistributionAssert.AreEqual(readStructure.FlowWidthAtBottomProtection, structure.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(readStructure.InsideWaterLevel, structure.InsideWaterLevel);
            DistributionAssert.AreEqual(readStructure.LevelCrestStructureNotClosing, structure.LevelCrestStructureNotClosing);
            DistributionAssert.AreEqual(readStructure.StorageStructureArea, structure.StorageStructureArea);
            DistributionAssert.AreEqual(readStructure.ThresholdHeightOpenWeir, structure.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(readStructure.WidthFlowApertures, structure.WidthFlowApertures);
        }
    }
}