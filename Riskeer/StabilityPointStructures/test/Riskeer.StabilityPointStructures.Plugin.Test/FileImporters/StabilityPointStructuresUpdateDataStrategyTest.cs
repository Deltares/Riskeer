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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Structures;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Plugin.FileImporters;
using Ringtoets.StabilityPointStructures.Util;

namespace Riskeer.StabilityPointStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class StabilityPointStructuresUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path/to/Structures";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StabilityPointStructureUpdateDataStrategy(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_WithFailureMechanism_CreatesNewInstance()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();

            // Call
            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<UpdateDataStrategyBase<StabilityPointStructure,
                StabilityPointStructuresFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IStructureUpdateStrategy<StabilityPointStructure>>(strategy);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StabilityPointStructureUpdateDataStrategy(new StabilityPointStructuresFailureMechanism());

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
            var strategy = new StabilityPointStructureUpdateDataStrategy(new StabilityPointStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithoutCurrentStructuresAndReadStructuresHaveDuplicateNames_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            var readStructures = new[]
            {
                new TestStabilityPointStructure(duplicateId, "First Name"),
                new TestStabilityPointStructure(duplicateId, "Second Name")
            };

            var targetCollection = new StructureCollection<StabilityPointStructure>();
            var strategy = new StabilityPointStructureUpdateDataStrategy(new StabilityPointStructuresFailureMechanism());

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
        public void UpdateStructuresWithImportedData_WithCurrentStructuresAndImportedHasNoOverlap_UpdatesTargetCollection()
        {
            // Setup
            var targetStructure = new TestStabilityPointStructure("target id");

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> structures = failureMechanism.StabilityPointStructures;
            structures.AddRange(new[]
            {
                targetStructure
            }, sourceFilePath);

            var readStructure = new TestStabilityPointStructure("read id");
            TestStabilityPointStructure[] importedStructures =
            {
                readStructure
            };

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

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
            var targetStructure = new TestStabilityPointStructure(commonId, "old name");

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> structures = failureMechanism.StabilityPointStructures;
            structures.AddRange(new[]
            {
                targetStructure
            }, sourceFilePath);

            var readStructure = new TestStabilityPointStructure(commonId, "new name");
            TestStabilityPointStructure[] importedStructures =
            {
                readStructure
            };

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures, sourceFilePath);

            // Assert
            Assert.AreEqual(1, structures.Count);
            Assert.AreSame(targetStructure, structures[0]);
            AssertStabilityPointStructure(readStructure, targetStructure);

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
            var updatedStructure = new TestStabilityPointStructure(commonId, "old name");
            var removedStructure = new TestStabilityPointStructure("removed id");

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> structures = failureMechanism.StabilityPointStructures;
            structures.AddRange(new[]
            {
                removedStructure,
                updatedStructure
            }, sourceFilePath);

            var structureToUpdateFrom = new TestStabilityPointStructure(commonId, "new name");
            var addedStructure = new TestStabilityPointStructure("added id");
            TestStabilityPointStructure[] importedStructures =
            {
                structureToUpdateFrom,
                addedStructure
            };

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures, sourceFilePath);

            // Assert
            Assert.AreEqual(2, structures.Count);
            Assert.AreSame(updatedStructure, structures[0]);
            AssertStabilityPointStructure(structureToUpdateFrom, updatedStructure);

            Assert.AreSame(addedStructure, structures[1]);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                updatedStructure,
                structures
            }, affectedObjects);
        }

        [Test]
        [TestCaseSource(typeof(StabilityPointStructurePermutationHelper),
            nameof(StabilityPointStructurePermutationHelper.DifferentStabilityPointStructuresWithSameId),
            new object[]
            {
                "UpdateStructuresWithImportedData",
                "UpdatesOnlySingleChange"
            })]
        public void UpdateStructuresWithImportedData_SingleChange_UpdatesOnlySingleChange(StabilityPointStructure readStructure)
        {
            // Setup
            StabilityPointStructure structure = new TestStabilityPointStructure();

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;
            targetCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);
            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            AssertStabilityPointStructure(readStructure, structure);
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
            StabilityPointStructure readStructure = new TestStabilityPointStructure(sameId, "new structure");
            StabilityPointStructure structure = new TestStabilityPointStructure(sameId, "original structure");

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };

            StructureCollection<StabilityPointStructure> targetDataCollection =
                failureMechanism.StabilityPointStructures;
            targetDataCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            Assert.IsTrue(calculation.HasOutput);
            AssertStabilityPointStructure(readStructure, structure);
            CollectionAssert.AreEqual(new IObservable[]
            {
                targetDataCollection,
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
            var affectedStructure = new TestStabilityPointStructure(affectedId, "Old name");
            var unaffectedStructure = new TestStabilityPointStructure(unaffectedId, unaffectedStructureName);

            var affectedCalculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = affectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var unaffectedCalculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = unaffectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism
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

            StructureCollection<StabilityPointStructure> targetDataCollection = failureMechanism.StabilityPointStructures;
            targetDataCollection.AddRange(new[]
            {
                affectedStructure,
                unaffectedStructure
            }, sourceFilePath);

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            StabilityPointStructure readAffectedStructure = new TestStabilityPointStructure(affectedId, "New name");
            StabilityPointStructure readUnaffectedStructure = new TestStabilityPointStructure(unaffectedId, unaffectedStructureName);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                readAffectedStructure,
                readUnaffectedStructure
            }, sourceFilePath);
            // Assert
            Assert.IsTrue(affectedCalculation.HasOutput);
            StabilityPointStructure inputParametersAffectedStructure = affectedCalculation.InputParameters.Structure;
            Assert.AreSame(affectedStructure, inputParametersAffectedStructure);
            AssertStabilityPointStructure(affectedStructure, inputParametersAffectedStructure);

            Assert.IsTrue(unaffectedCalculation.HasOutput);
            StabilityPointStructure inputParametersUnaffectedStructure = unaffectedCalculation.InputParameters.Structure;
            Assert.AreSame(unaffectedStructure, inputParametersUnaffectedStructure);
            AssertStabilityPointStructure(readUnaffectedStructure, inputParametersUnaffectedStructure);

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
            StabilityPointStructure structure = new TestStabilityPointStructure(removedId, "original structure");

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };

            StructureCollection<StabilityPointStructure> targetDataCollection =
                failureMechanism.StabilityPointStructures;
            targetDataCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
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
            const string removedId = "removedId";
            const string unaffectedId = "unaffectedId";
            const string unaffectedStructureName = "unaffectedStructure";
            var removedStructure = new TestStabilityPointStructure(removedId, "Old name");
            var unaffectedStructure = new TestStabilityPointStructure(unaffectedId, unaffectedStructureName);

            var affectedCalculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = removedStructure
                },
                Output = new TestStructuresOutput()
            };

            var unaffectedCalculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = unaffectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism
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

            StructureCollection<StabilityPointStructure> targetDataCollection = failureMechanism.StabilityPointStructures;
            targetDataCollection.AddRange(new[]
            {
                removedStructure,
                unaffectedStructure
            }, sourceFilePath);

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            StabilityPointStructure readUnaffectedStructure = new TestStabilityPointStructure(unaffectedId, unaffectedStructureName);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                readUnaffectedStructure
            }, sourceFilePath);
            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            Assert.IsNull(affectedCalculation.InputParameters.Structure);

            Assert.IsTrue(unaffectedCalculation.HasOutput);
            StabilityPointStructure inputParametersUnaffectedStructure = unaffectedCalculation.InputParameters.Structure;
            Assert.AreSame(unaffectedStructure, inputParametersUnaffectedStructure);
            AssertStabilityPointStructure(readUnaffectedStructure, inputParametersUnaffectedStructure);

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
            var affectedStructure = new TestStabilityPointStructure(affectedId, "Old name");
            var affectedCalculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = affectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism
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

            StructureCollection<StabilityPointStructure> structures = failureMechanism.StabilityPointStructures;
            structures.AddRange(new[]
            {
                affectedStructure
            }, sourceFilePath);

            var structureToUpdateFrom = new TestStabilityPointStructure(affectedId, "New name");

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

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
            StabilityPointStructure readStructure = new TestStabilityPointStructure(updatedMatchingPoint, sameId);
            StabilityPointStructure structure = new TestStabilityPointStructure(originalMatchingPoint, sameId);

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = structure
                }
            };
            var failureMechanism = new TestStabilityPointStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
            failureMechanism.StabilityPointStructures.AddRange(new[]
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

            StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);
            StabilityPointStructuresFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults.ToArray();

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

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
            AssertStabilityPointStructure(readStructure, structure);

            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.StabilityPointStructures,
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
            StabilityPointStructure removedStructure = new TestStabilityPointStructure(originalMatchingPoint, sameId);

            var calculation = new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    Structure = removedStructure
                }
            };
            var failureMechanism = new StabilityPointStructuresFailureMechanism
            {
                CalculationsGroup =
                {
                    Children =
                    {
                        calculation
                    }
                }
            };
            failureMechanism.StabilityPointStructures.AddRange(new[]
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

            StabilityPointStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);
            StabilityPointStructuresFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults.ToArray();

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Precondition
            Assert.AreEqual(1, sectionResults.Length);
            Assert.AreSame(calculation, sectionResults[0].Calculation);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                                 sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.StabilityPointStructures,
                calculation.InputParameters,
                sectionResults[0]
            }, affectedObjects);

            sectionResults = failureMechanism.SectionResults.ToArray();
            Assert.AreEqual(1, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
        }

        private static void AssertStabilityPointStructure(StabilityPointStructure expectedStabilityPointStructure,
                                                          StabilityPointStructure actualStabilityPointStructure)
        {
            Assert.AreEqual(expectedStabilityPointStructure.Name, actualStabilityPointStructure.Name);
            Assert.AreEqual(expectedStabilityPointStructure.Id, actualStabilityPointStructure.Id);
            Assert.AreEqual(expectedStabilityPointStructure.Location, actualStabilityPointStructure.Location);
            Assert.AreEqual(expectedStabilityPointStructure.StructureNormalOrientation, actualStabilityPointStructure.StructureNormalOrientation);

            DistributionAssert.AreEqual(expectedStabilityPointStructure.StorageStructureArea, actualStabilityPointStructure.StorageStructureArea);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.AllowedLevelIncreaseStorage, actualStabilityPointStructure.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.WidthFlowApertures, actualStabilityPointStructure.WidthFlowApertures);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.InsideWaterLevel, actualStabilityPointStructure.InsideWaterLevel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ThresholdHeightOpenWeir, actualStabilityPointStructure.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.CriticalOvertoppingDischarge, actualStabilityPointStructure.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.FlowWidthAtBottomProtection, actualStabilityPointStructure.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ConstructiveStrengthLinearLoadModel, actualStabilityPointStructure.ConstructiveStrengthLinearLoadModel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ConstructiveStrengthQuadraticLoadModel, actualStabilityPointStructure.ConstructiveStrengthQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.BankWidth, actualStabilityPointStructure.BankWidth);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.InsideWaterLevelFailureConstruction, actualStabilityPointStructure.InsideWaterLevelFailureConstruction);
            Assert.AreEqual(expectedStabilityPointStructure.EvaluationLevel, actualStabilityPointStructure.EvaluationLevel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.LevelCrestStructure, actualStabilityPointStructure.LevelCrestStructure);
            Assert.AreEqual(expectedStabilityPointStructure.VerticalDistance, actualStabilityPointStructure.VerticalDistance);
            Assert.AreEqual(expectedStabilityPointStructure.FailureProbabilityRepairClosure, actualStabilityPointStructure.FailureProbabilityRepairClosure);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.FailureCollisionEnergy, actualStabilityPointStructure.FailureCollisionEnergy);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ShipMass, actualStabilityPointStructure.ShipMass);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.ShipVelocity, actualStabilityPointStructure.ShipVelocity);
            Assert.AreEqual(expectedStabilityPointStructure.LevellingCount, actualStabilityPointStructure.LevellingCount);
            Assert.AreEqual(expectedStabilityPointStructure.ProbabilityCollisionSecondaryStructure, actualStabilityPointStructure.ProbabilityCollisionSecondaryStructure);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.FlowVelocityStructureClosable, actualStabilityPointStructure.FlowVelocityStructureClosable);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.StabilityLinearLoadModel, actualStabilityPointStructure.StabilityLinearLoadModel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.StabilityQuadraticLoadModel, actualStabilityPointStructure.StabilityQuadraticLoadModel);
            DistributionAssert.AreEqual(expectedStabilityPointStructure.AreaFlowApertures, actualStabilityPointStructure.AreaFlowApertures);
            Assert.AreEqual(expectedStabilityPointStructure.InflowModelType, actualStabilityPointStructure.InflowModelType);
        }
    }
}