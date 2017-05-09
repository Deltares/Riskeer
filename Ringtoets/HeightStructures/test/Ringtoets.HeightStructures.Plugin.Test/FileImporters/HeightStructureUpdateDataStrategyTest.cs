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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.Structures;
using Ringtoets.Common.Utils;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Plugin.FileImporters;

namespace Ringtoets.HeightStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class HeightStructureUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path";

        private static IEnumerable<TestCaseData> DifferentHeightStructureWithSameId
        {
            get
            {
                var random = new Random(532);
                const string defaultId = "id";
                const string defaultName = "name";
                var defaultLocation = new Point2D(0, 0);

                yield return new TestCaseData(new TestHeightStructure(defaultId, "Different name"))
                    .SetName("Different name");
                yield return new TestCaseData(new TestHeightStructure(new Point2D(1, 1), defaultId))
                    .SetName("Different Location");
                yield return new TestCaseData(new TestHeightStructure
                {
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) random.Next(),
                        Shift = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    }
                }).SetName("Different AllowedLevelIncreaseStorage");
                yield return new TestCaseData(new TestHeightStructure
                {
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) random.Next(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    }
                }).SetName("Different CriticalOvertoppingDischarge");
                yield return new TestCaseData(new TestHeightStructure
                {
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) random.Next(),
                        Shift = random.NextRoundedDouble(),
                        StandardDeviation = random.NextRoundedDouble()
                    }
                }).SetName("Different FlowWidthAtBottomProtection");
                yield return new TestCaseData(new TestHeightStructure
                {
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) random.Next(),
                        StandardDeviation = random.NextRoundedDouble()
                    }
                }).SetName("Different LevelCrestStructure");
                yield return new TestCaseData(new TestHeightStructure
                {
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) random.Next(),
                        CoefficientOfVariation = random.NextRoundedDouble()
                    }
                }).SetName("Different StorageStructureArea");
                yield return new TestCaseData(new TestHeightStructure
                {
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) random.Next(),
                        StandardDeviation = random.NextRoundedDouble()
                    }
                }).SetName("Different WidthFlowApertures");
                yield return new TestCaseData(new HeightStructure(new HeightStructure.ConstructionProperties
                {
                    Name = defaultName,
                    Id = defaultId,
                    Location = defaultLocation,
                    FailureProbabilityStructureWithErosion = random.NextDouble()
                })).SetName("Different FailureProbabilityStructureWithErosion");
                yield return new TestCaseData(new HeightStructure(new HeightStructure.ConstructionProperties
                {
                    Name = defaultName,
                    Id = defaultId,
                    Location = defaultLocation,
                    StructureNormalOrientation = random.NextRoundedDouble()
                })).SetName("Different StructureNormalOrientation");
            }
        }

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
        public void UpdateStructuresWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(null, Enumerable.Empty<HeightStructure>(),
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(new StructureCollection<HeightStructure>(),
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
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(new StructureCollection<HeightStructure>(),
                                                                                Enumerable.Empty<HeightStructure>(),
                                                                                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CurrentCollectionAndImportedCollectionEmpty_DoesNothing()
        {
            // Setup
            var targetCollection = new StructureCollection<HeightStructure>();
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                                 Enumerable.Empty<HeightStructure>(),
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
                new TestHeightStructure(duplicateId, "Structure"),
                new TestHeightStructure(duplicateId, "Other structure")
            };

            var targetCollection = new StructureCollection<HeightStructure>();
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                readStructures,
                                                                                sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);

            string expectedMessage = "Kunstwerken moeten een unieke id hebben. " +
                                     $"Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructureAndImportedMultipleStructuresWithSameId_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            var expectedStructure = new TestHeightStructure(duplicateId, "expectedStructure");

            var expectedCollection = new[]
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            StructureCollection<HeightStructure> structures = failureMechanism.HeightStructures;
            structures.AddRange(new[]
            {
                new TestHeightStructure("id", "name"),
                new TestHeightStructure("other id", "other name")
            }, sourceFilePath);

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(
                structures, Enumerable.Empty<HeightStructure>(), sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(structures);
            CollectionAssert.AreEqual(new[]
            {
                structures
            }, affectedObjects);
        }

        [Test]
        [TestCaseSource(nameof(DifferentHeightStructureWithSameId))]
        public void UpdateStructuresWithImportedData_SingleChange_UpdatesOnlySingleChange(HeightStructure readStructure)
        {
            // Setup
            HeightStructure structure = new TestHeightStructure();

            var targetCollection = new StructureCollection<HeightStructure>();
            targetCollection.AddRange(new[]
            {
                structure
            }, sourceFilePath);
            var strategy = new HeightStructureUpdateDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetCollection,
                                                                                                 new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            AssertHeightStructures(readStructure, structure);
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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(failureMechanism.HeightStructures,
                                                                                                 new[]
                                                                                                 {
                                                                                                     readStructure
                                                                                                 },
                                                                                                 sourceFilePath);

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            AssertHeightStructures(readStructure, structure);
            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.HeightStructures,
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
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(targetDataCollection,
                                                                                                 new[]
                                                                                                 {
                                                                                                     readAffectedStructure,
                                                                                                     readUnaffectedStructure
                                                                                                 }, sourceFilePath);
            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            HeightStructure inputParametersAffectedStructure = affectedCalculation.InputParameters.Structure;
            Assert.AreSame(affectedStructure, inputParametersAffectedStructure);
            AssertHeightStructures(affectedStructure, inputParametersAffectedStructure);

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
        public void UpdateStructuresWithImportedData_CalculationWithSameReference_OnlyReturnsDistinctCalculation()
        {
            // Setup
            const string affectedId = "affectedId";
            var affectedStructure = new TestHeightStructure(affectedId, "Old name");
            var affectedCalculation = new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    Structure = affectedStructure
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
                        affectedCalculation
                    }
                }
            };

            StructureCollection<HeightStructure> structures = failureMechanism.HeightStructures;
            structures.AddRange(new[]
            {
                affectedStructure
            }, sourceFilePath);

            var structureToUpdateFrom = new TestHeightStructure(affectedId, "New name");

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

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
                                                                         failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>());

            HeightStructuresFailureMechanismSectionResult[] sectionResults = failureMechanism.SectionResults.ToArray();

            // Precondition
            Assert.AreSame(calculation, sectionResults[0].Calculation);
            Assert.IsNull(sectionResults[1].Calculation);

            var strategy = new HeightStructureUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(
                failureMechanism.HeightStructures,
                new[]
                {
                    readStructure
                },
                sourceFilePath);

            // Assert
            AssertHeightStructures(readStructure, structure);

            CollectionAssert.AreEqual(new IObservable[]
            {
                failureMechanism.HeightStructures,
                calculation.InputParameters,
                sectionResults[0],
                sectionResults[1]
            }, affectedObjects);

            sectionResults = failureMechanism.SectionResults.ToArray();
            Assert.AreEqual(2, sectionResults.Length);
            Assert.IsNull(sectionResults[0].Calculation);
            Assert.AreSame(calculation, sectionResults[1].Calculation);
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