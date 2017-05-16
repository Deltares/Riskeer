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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Plugin.FileImporters;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Structures;
using Ringtoets.Common.Utils;

namespace Ringtoets.ClosingStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class ClosingStructureUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ClosingStructureUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
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
        public void UpdateStructuresWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(null, Enumerable.Empty<ClosingStructure>(),
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(new StructureCollection<ClosingStructure>(),
                                                                                null,
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SourcePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(new StructureCollection<ClosingStructure>(),
                                                                                Enumerable.Empty<ClosingStructure>(),
                                                                                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CurrentCollectionAndImportedCollectionEmpty_DoesNothing()
        {
            // Setup
            var targetCollection = new StructureCollection<ClosingStructure>();
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                                 Enumerable.Empty<ClosingStructure>(),
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
                new TestClosingStructure(duplicateId, "Structure"),
                new TestClosingStructure(duplicateId, "Other structure")
            };

            var targetCollection = new StructureCollection<ClosingStructure>();
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

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
        public void UpdateStructuresWithImportedData_WithCurrentStructureAndImportedMultipleStructuresWithSameId_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            var expectedStructure = new TestClosingStructure(duplicateId, "expectedStructure");

            var expectedCollection = new[]
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
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                readStructures,
                                                                                sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.AreEqual(expectedCollection, targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructuresAndImportedDataEmpty_StructuresRemoved()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            StructureCollection<ClosingStructure> structures = failureMechanism.ClosingStructures;
            structures.AddRange(new[]
            {
                new TestClosingStructure("id", "name"),
                new TestClosingStructure("other id", "other name")
            }, sourceFilePath);

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(
                structures, Enumerable.Empty<ClosingStructure>(), sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(structures);
            CollectionAssert.AreEqual(new[]
            {
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

            var targetCollection = new StructureCollection<ClosingStructure>();
            targetCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);
            var strategy = new ClosingStructureUpdateDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                                 new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            AssertClosingStructures(readStructure, structure);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithStructureImportedStructureWithSameId_UpdatesCalculation()
        {
            // Setup
            const string sameId = "sameId";
            ClosingStructure readStructure = new TestClosingStructure(sameId, "new structure");
            ClosingStructure structure = new TestClosingStructure(sameId, "original structure");

            var calculation = new TestClosingStructuresCalculation
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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(failureMechanism.ClosingStructures,
                                                                                                 new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            AssertClosingStructures(readStructure, structure);
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.ClosingStructures,
                calculation.InputParameters,
                calculation
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_MultipleCalculationWithAssignedStructure_OnlyUpdatesCalculationWithUpdatedStructure()
        {
            // Setup
            const string affectedId = "affectedId";
            const string unaffectedId = "unaffectedId";
            const string unaffectedStructureName = "unaffectedStructure";
            var affectedStructure = new TestClosingStructure(affectedId, "Old name");
            var unaffectedStructure = new TestClosingStructure(unaffectedId, unaffectedStructureName);

            var affectedCalculation = new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    Structure = affectedStructure
                },
                Output = new TestStructuresOutput()
            };

            var unaffectedCalculation = new TestClosingStructuresCalculation
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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetDataCollection,
                                                                                                 new[]
                                                                                                 {
                                                                                                     readAffectedStructure,
                                                                                                     readUnaffectedStructure
                                                                                                 }, sourceFilePath);
            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            ClosingStructure inputParametersAffectedStructure = affectedCalculation.InputParameters.Structure;
            Assert.AreSame(affectedStructure, inputParametersAffectedStructure);
            AssertClosingStructures(affectedStructure, inputParametersAffectedStructure);

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
        public void UpdateStructuresWithImportedData_CalculationWithSameReference_OnlyReturnsDistinctCalculation()
        {
            // Setup
            const string affectedId = "affectedId";
            var affectedStructure = new TestClosingStructure(affectedId, "Old name");
            var affectedCalculation = new TestClosingStructuresCalculation
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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(structures,
                                                                                                 new[]
                                                                                                 {
                                                                                                     structureToUpdateFrom
                                                                                                 }, sourceFilePath);
            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                structures,
                affectedCalculation,
                affectedCalculation.InputParameters
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SectionResultWithStructureImportedStructureWithSameId_UpdatesCalculation()
        {
            // Setup
            const string sameId = "sameId";
            var originalMatchingPoint = new Point2D(0, 0);
            var updatedMatchingPoint = new Point2D(20, 20);
            ClosingStructure readStructure = new TestClosingStructure(updatedMatchingPoint, sameId);
            ClosingStructure structure = new TestClosingStructure(originalMatchingPoint, sameId);

            var calculation = new TestClosingStructuresCalculation
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
            failureMechanism.AddSection(new FailureMechanismSection("OldSection", new[]
            {
                originalMatchingPoint,
                intersectionPoint
            }));
            failureMechanism.AddSection(new FailureMechanismSection("NewSection", new[]
            {
                intersectionPoint,
                updatedMatchingPoint
            }));

            StructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism.SectionResults,
                                                                         failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>());

            ClosingStructuresFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults.ToArray();

            var strategy = new ClosingStructureUpdateDataStrategy(failureMechanism);

            // Precondition
            Assert.AreSame(calculation, sectionResults[0].Calculation);
            Assert.IsNull(sectionResults[1].Calculation);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(
                failureMechanism.ClosingStructures,
                new[]
                {
                    readStructure
                },
                sourceFilePath);

            // Assert
            AssertClosingStructures(readStructure, structure);

            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.ClosingStructures,
                calculation.InputParameters,
                sectionResults[0],
                sectionResults[1]
            }, affectedObjects);

            sectionResults = failureMechanism.SectionResults.ToArray();
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation, sectionResults[1].Calculation);
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
            Assert.AreEqual(readStructure.ProbabilityOrFrequencyOpenStructureBeforeFlooding, structure.ProbabilityOrFrequencyOpenStructureBeforeFlooding);

            DistributionAssert.AreEqual(readStructure.FlowWidthAtBottomProtection, structure.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(readStructure.InsideWaterLevel, structure.InsideWaterLevel);
            DistributionAssert.AreEqual(readStructure.LevelCrestStructureNotClosing, structure.LevelCrestStructureNotClosing);
            DistributionAssert.AreEqual(readStructure.StorageStructureArea, structure.StorageStructureArea);
            DistributionAssert.AreEqual(readStructure.ThresholdHeightOpenWeir, structure.ThresholdHeightOpenWeir);
            DistributionAssert.AreEqual(readStructure.WidthFlowApertures, structure.WidthFlowApertures);
        }
    }
}