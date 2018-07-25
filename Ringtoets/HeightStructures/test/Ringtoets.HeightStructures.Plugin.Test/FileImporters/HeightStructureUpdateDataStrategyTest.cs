// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Structures;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Plugin.FileImporters;
using Ringtoets.HeightStructures.Util;

namespace Ringtoets.HeightStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class HeightStructureUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructureUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_CreatesNewInstance()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<UpdateDataStrategyBase<HeightStructure, HeightStructuresFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IStructureUpdateStrategy<HeightStructure>>(strategy);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(null,
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
                                                                                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithoutCurrentStructuresAndReadStructuresHaveDuplicateIds_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            var readStructures = new[]
            {
                new TestHeightStructure(duplicateId, "Structure"),
                new TestHeightStructure(duplicateId, "Other structure")
            };

            var targetCollection = new StructureCollection<HeightStructure>();
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(readStructures,
                                                                                sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);

            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructureAndImportedMultipleStructuresWithSameId_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            var expectedStructure = new TestHeightStructure(duplicateId, "expectedStructure");

            TestHeightStructure[] expectedCollection =
            {
                expectedStructure
            };

            var targetCollection = new StructureCollection<HeightStructure>();
            targetCollection.AddRange(expectedCollection, sourceFilePath);

            var readStructures = new[]
            {
                new TestHeightStructure(duplicateId, "Structure"),
                new TestHeightStructure(duplicateId, "Other structure")
            };

            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(readStructures,
                                                                                sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.AreEqual(expectedCollection, targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructuresAndImportedHasNoOverlap_UpdatesTargetCollection()
        {
            // Setup
            var targetStructure = new TestHeightStructure("target id");

            var failureMechanism = new HeightStructuresFailureMechanism();
            StructureCollection<HeightStructure> structures = failureMechanism.HeightStructures;
            structures.AddRange(new[]
            {
                targetStructure
            }, sourceFilePath);

            var readStructure = new TestHeightStructure("read id");
            TestHeightStructure[] importedStructures =
            {
                readStructure
            };

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

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
            var targetStructure = new TestHeightStructure(commonId, "old name");

            var failureMechanism = new HeightStructuresFailureMechanism();
            StructureCollection<HeightStructure> structures = failureMechanism.HeightStructures;
            structures.AddRange(new[]
            {
                targetStructure
            }, sourceFilePath);

            var readStructure = new TestHeightStructure(commonId, "new name");
            TestHeightStructure[] importedStructures =
            {
                readStructure
            };

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures, sourceFilePath);

            // Assert
            Assert.AreEqual(1, structures.Count);
            Assert.AreSame(targetStructure, structures[0]);
            AssertHeightStructures(readStructure, targetStructure);

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
            var updatedStructure = new TestHeightStructure(commonId, "old name");
            var removedStructure = new TestHeightStructure("removed id");

            var failureMechanism = new HeightStructuresFailureMechanism();
            StructureCollection<HeightStructure> structures = failureMechanism.HeightStructures;
            structures.AddRange(new[]
            {
                removedStructure,
                updatedStructure
            }, sourceFilePath);

            var structureToUpdateFrom = new TestHeightStructure(commonId, "new name");
            var addedStructure = new TestHeightStructure("added id");
            TestHeightStructure[] importedStructures =
            {
                structureToUpdateFrom,
                addedStructure
            };

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures, sourceFilePath);

            // Assert
            Assert.AreEqual(2, structures.Count);
            Assert.AreSame(updatedStructure, structures[0]);
            AssertHeightStructures(structureToUpdateFrom, updatedStructure);

            Assert.AreSame(addedStructure, structures[1]);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                updatedStructure,
                structures
            }, affectedObjects);
        }

        [Test]
        [TestCaseSource(typeof(HeightStructurePermutationHelper),
            nameof(HeightStructurePermutationHelper.DifferentHeightStructuresWithSameId),
            new object[]
            {
                "UpdateStructuresWithImportedData",
                "UpdatesOnlySingleChange"
            })]
        public void UpdateStructuresWithImportedData_SingleChange_UpdatesOnlySingleChange(HeightStructure readStructure)
        {
            // Setup
            HeightStructure structure = new TestHeightStructure();

            var failureMechanism = new HeightStructuresFailureMechanism();
            StructureCollection<HeightStructure> targetCollection = failureMechanism.HeightStructures;
            targetCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);
            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            AssertHeightStructures(readStructure, structure);
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
            HeightStructure readStructure = new TestHeightStructure(sameId, "new structure");
            HeightStructure structure = new TestHeightStructure(sameId, "original structure");

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
            failureMechanism.HeightStructures.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(calculation.HasOutput);
            AssertHeightStructures(readStructure, structure);
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.HeightStructures,
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
            var affectedStructure = new TestHeightStructure(affectedId, "Old name");
            var unaffectedStructure = new TestHeightStructure(unaffectedId, unaffectedStructureName);

            var affectedCalculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = affectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var unaffectedCalculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = unaffectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new HeightStructuresFailureMechanism
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
            StructureCollection<HeightStructure> targetDataCollection = failureMechanism.HeightStructures;
            targetDataCollection.AddRange(new[]
            {
                affectedStructure,
                unaffectedStructure
            }, sourceFilePath);

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            HeightStructure readAffectedStructure = new TestHeightStructure(affectedId, "New name");
            HeightStructure readUnaffectedStructure = new TestHeightStructure(unaffectedId, unaffectedStructureName);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                readAffectedStructure,
                readUnaffectedStructure
            }, sourceFilePath);
            // Assert
            Assert.IsTrue(affectedCalculation.HasOutput);
            HeightStructure inputParametersAffectedStructure = affectedCalculation.InputParameters.Structure;
            Assert.AreSame(affectedStructure, inputParametersAffectedStructure);
            AssertHeightStructures(affectedStructure, inputParametersAffectedStructure);

            Assert.IsTrue(unaffectedCalculation.HasOutput);
            HeightStructure inputParametersUnaffectedStructure = unaffectedCalculation.InputParameters.Structure;
            Assert.AreSame(unaffectedStructure, inputParametersUnaffectedStructure);
            AssertHeightStructures(readUnaffectedStructure, inputParametersUnaffectedStructure);

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
            const string removedId = "sameId";
            HeightStructure structure = new TestHeightStructure(removedId, "original structure");

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };

            StructureCollection<HeightStructure> targetDataCollection =
                failureMechanism.HeightStructures;
            targetDataCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
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
        public void UpdateStructuresWithImportedData_MultipleCalculationWithStructuresOneWithRemovedStructure_OnlyUpdatesCalculationWithRemovedStructure()
        {
            // Setup
            const string removedId = "affectedId";
            const string unaffectedId = "unaffectedId";
            const string unaffectedStructureName = "unaffectedStructure";
            var removedStructure = new TestHeightStructure(removedId, "Old name");
            var unaffectedStructure = new TestHeightStructure(unaffectedId, unaffectedStructureName);

            var affectedCalculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = removedStructure
                },
                Output = new TestStructuresOutput()
            };

            var unaffectedCalculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = unaffectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new HeightStructuresFailureMechanism
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
            StructureCollection<HeightStructure> targetDataCollection = failureMechanism.HeightStructures;
            targetDataCollection.AddRange(new[]
            {
                removedStructure,
                unaffectedStructure
            }, sourceFilePath);

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            HeightStructure readUnaffectedStructure = new TestHeightStructure(unaffectedId, unaffectedStructureName);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                readUnaffectedStructure
            }, sourceFilePath);
            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            Assert.IsNull(affectedCalculation.InputParameters.Structure);

            Assert.IsTrue(unaffectedCalculation.HasOutput);
            HeightStructure inputParametersUnaffectedStructure = unaffectedCalculation.InputParameters.Structure;
            Assert.AreSame(unaffectedStructure, inputParametersUnaffectedStructure);
            AssertHeightStructures(readUnaffectedStructure, inputParametersUnaffectedStructure);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedCalculation.InputParameters,
                targetDataCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SectionResultWithStructureImportedStructureWithSameId_UpdatesCalculation()
        {
            // Setup
            const string sameId = "sameId";
            var originalMatchingPoint = new Point2D(0, 0);
            var updatedMatchingPoint = new Point2D(20, 20);
            HeightStructure readStructure = new TestHeightStructure(updatedMatchingPoint, sameId);
            HeightStructure structure = new TestHeightStructure(originalMatchingPoint, sameId);

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
            failureMechanism.HeightStructures.AddRange(new[]
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

            HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);
            HeightStructuresFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults.ToArray();

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Precondition
            Assert.AreSame(calculation, sectionResults[0].Calculation);
            Assert.IsNull(sectionResults[1].Calculation);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            AssertHeightStructures(readStructure, structure);

            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.HeightStructures,
                structure,
                calculation.InputParameters,
                sectionResults[0],
                sectionResults[1]
            }, affectedObjects);

            sectionResults = failureMechanism.SectionResults.ToArray();
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation, sectionResults[1].Calculation);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SectionResultWithStructureImportedStructureWithSameIdRemoved_UpdatesCalculationInput()
        {
            // Setup
            const string sameId = "id";
            var originalMatchingPoint = new Point2D(0, 0);
            HeightStructure removedStructure = new TestHeightStructure(originalMatchingPoint, sameId);

            var calculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = removedStructure
                }
            };
            var failureMechanism = new HeightStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
            failureMechanism.HeightStructures.AddRange(new[]
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

            HeightStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);
            HeightStructuresFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults.ToArray();

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Precondition
            Assert.AreEqual(1, sectionResults.Length);
            Assert.AreSame(calculation, sectionResults[0].Calculation);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
                                                                                                 sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.HeightStructures,
                calculation.InputParameters,
                sectionResults[0]
            }, affectedObjects);

            sectionResults = failureMechanism.SectionResults.ToArray();
            Assert.AreEqual(1, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
        }

        private static void AssertHeightStructures(HeightStructure readStructure, HeightStructure structure)
        {
            Assert.AreEqual(readStructure.Name, structure.Name);
            Assert.AreEqual(readStructure.Location, structure.Location);
            Assert.AreEqual(readStructure.StructureNormalOrientation, structure.StructureNormalOrientation);

            Assert.AreEqual(readStructure.AllowedLevelIncreaseStorage.Mean, structure.AllowedLevelIncreaseStorage.Mean);
            Assert.AreEqual(readStructure.AllowedLevelIncreaseStorage.StandardDeviation, structure.AllowedLevelIncreaseStorage.StandardDeviation);
            Assert.AreEqual(readStructure.AllowedLevelIncreaseStorage.Shift, structure.AllowedLevelIncreaseStorage.Shift);

            Assert.AreEqual(readStructure.CriticalOvertoppingDischarge.Mean, structure.CriticalOvertoppingDischarge.Mean);
            Assert.AreEqual(readStructure.CriticalOvertoppingDischarge.CoefficientOfVariation, structure.CriticalOvertoppingDischarge.CoefficientOfVariation);

            Assert.AreEqual(readStructure.FailureProbabilityStructureWithErosion, structure.FailureProbabilityStructureWithErosion);

            Assert.AreEqual(readStructure.FlowWidthAtBottomProtection.Mean, structure.FlowWidthAtBottomProtection.Mean);
            Assert.AreEqual(readStructure.FlowWidthAtBottomProtection.StandardDeviation, structure.FlowWidthAtBottomProtection.StandardDeviation);
            Assert.AreEqual(readStructure.FlowWidthAtBottomProtection.Shift, structure.FlowWidthAtBottomProtection.Shift);

            Assert.AreEqual(readStructure.LevelCrestStructure.Mean, structure.LevelCrestStructure.Mean);
            Assert.AreEqual(readStructure.LevelCrestStructure.StandardDeviation, structure.LevelCrestStructure.StandardDeviation);

            Assert.AreEqual(readStructure.StorageStructureArea.Mean, structure.StorageStructureArea.Mean);
            Assert.AreEqual(readStructure.StorageStructureArea.CoefficientOfVariation, structure.StorageStructureArea.CoefficientOfVariation);

            Assert.AreEqual(readStructure.WidthFlowApertures.Mean, structure.WidthFlowApertures.Mean);
            Assert.AreEqual(readStructure.WidthFlowApertures.StandardDeviation, structure.WidthFlowApertures.StandardDeviation);
        }
    }
}