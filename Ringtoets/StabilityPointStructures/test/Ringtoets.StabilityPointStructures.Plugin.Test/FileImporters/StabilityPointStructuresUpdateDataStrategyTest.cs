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
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Structures;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Plugin.FileImporters;

namespace Ringtoets.StabilityPointStructures.Plugin.Test.FileImporters
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
        public void UpdateStructuresWithImportedData_CurrentCollectionAndImportedCollectionEmpty_DoesNothing()
        {
            // Setup
            var targetCollection = new StructureCollection<StabilityPointStructure>();
            var strategy = new StabilityPointStructureUpdateDataStrategy(new StabilityPointStructuresFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                                 Enumerable.Empty<StabilityPointStructure>(),
                                                                                                 sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithoutCurrentStructuresAndReadStructuresHaveDuplicateNames_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            var readStructures = new[]
            {
                new TestStabilityPointStructure("First Name", duplicateId),
                new TestStabilityPointStructure("Second Name", duplicateId),
            };

            var targetCollection = new StructureCollection<StabilityPointStructure>();
            var strategy = new StabilityPointStructureUpdateDataStrategy(new StabilityPointStructuresFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                readStructures,
                                                                                sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);

            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructuresAndImportedDataEmpty_StructuresRemoved()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> structures = failureMechanism.StabilityPointStructures;
            structures.AddRange(new[]
            {
                new TestStabilityPointStructure("name", "id"),
                new TestStabilityPointStructure("other name", "other id"),
            }, sourceFilePath);

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(
                structures, Enumerable.Empty<StabilityPointStructure>(), sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(structures);
            CollectionAssert.AreEqual(new[]
            {
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

            var targetCollection = new StructureCollection<StabilityPointStructure>();
            targetCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);
            var strategy = new StabilityPointStructureUpdateDataStrategy(new StabilityPointStructuresFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                                 new[]
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
            StabilityPointStructure readStructure = new TestStabilityPointStructure("new structure", sameId);
            StabilityPointStructure structure = new TestStabilityPointStructure("original structure", sameId);

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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(
                targetDataCollection,
                new[]
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
            var affectedStructure = new TestStabilityPointStructure("Old name", affectedId);
            var unaffectedStructure = new TestStabilityPointStructure(unaffectedStructureName, unaffectedId);

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

            StabilityPointStructure readAffectedStructure = new TestStabilityPointStructure("New name", affectedId);
            StabilityPointStructure readUnaffectedStructure = new TestStabilityPointStructure(unaffectedStructureName, unaffectedId);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetDataCollection,
                                                                                                 new[]
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
            StabilityPointStructure structure = new TestStabilityPointStructure("original structure", removedId);

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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(
                targetDataCollection,
                Enumerable.Empty<StabilityPointStructure>(),
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
            var removedStructure = new TestStabilityPointStructure("Old name", removedId);
            var unaffectedStructure = new TestStabilityPointStructure(unaffectedStructureName, unaffectedId);

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

            StabilityPointStructure readUnaffectedStructure = new TestStabilityPointStructure(unaffectedStructureName, unaffectedId);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetDataCollection,
                                                                                                 new[]
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
            var affectedStructure = new TestStabilityPointStructure("Old name", affectedId);
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

            var structureToUpdateFrom = new TestStabilityPointStructure("New name", affectedId);

            var strategy = new StabilityPointStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(structures,
                                                                                                 new[]
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