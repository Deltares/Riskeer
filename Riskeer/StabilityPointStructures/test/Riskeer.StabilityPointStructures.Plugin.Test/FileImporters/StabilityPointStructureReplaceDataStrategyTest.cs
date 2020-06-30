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
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.Structures;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.Data.TestUtil;
using Riskeer.StabilityPointStructures.Plugin.FileImporters;

namespace Riskeer.StabilityPointStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class StabilityPointStructureReplaceDataStrategyTest
    {
        private const string sourcePath = "some/path/to/structures";

        [Test]
        public void Constructor_WithFailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new StabilityPointStructureReplaceStrategy(null);

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
            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<ReplaceDataStrategyBase<StabilityPointStructure,
                StabilityPointStructuresFailureMechanism>>(strategy);
            Assert.IsInstanceOf<IStructureUpdateStrategy<StabilityPointStructure>>(strategy);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ImportedDataCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Call
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(null,
                                                                                sourcePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("importedDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Call
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_DifferentSourcePath_UpdatesSourcePath()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;

            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            var newSourcePath = "some/other/path/toStructures";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                                 newSourcePath);

            // Assert
            Assert.AreEqual(newSourcePath, targetCollection.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_NoCurrentStructures_SetsSourcePath()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;

            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                                 sourcePath);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
            Assert.AreEqual(sourcePath, targetCollection.SourcePath);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ImportedDataContainsDuplicateIds_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            StabilityPointStructure[] importedClosingStructures =
            {
                new TestStabilityPointStructure(duplicateId, "name"),
                new TestStabilityPointStructure(duplicateId, "Other name")
            };

            var strategy = new StabilityPointStructureReplaceStrategy(new StabilityPointStructuresFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(importedClosingStructures,
                                                                                sourcePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = "Kunstwerken moeten een unieke id hebben. " +
                                     $"Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void UpdateStructuresWithImportedData_NoCurrentStructuresWithImportedData_AddsNewStructure()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;

            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            var importedCollection = new[]
            {
                new TestStabilityPointStructure()
            };

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedCollection,
                                                                                                 sourcePath);

            // Assert
            Assert.AreEqual(sourcePath, targetCollection.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
            CollectionAssert.AreEqual(importedCollection, targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentStructuresAndImportedCollectionEmpty_ClearsCollection()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;
            targetCollection.AddRange(new[]
            {
                new TestStabilityPointStructure()
            }, sourcePath);

            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                                 sourcePath);

            // Assert
            Assert.AreEqual(sourcePath, targetCollection.SourcePath);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentAndImportedDataAreDifferent_ReplacesCurrentWithImportedData()
        {
            // Setup
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;
            targetCollection.AddRange(new[]
            {
                new TestStabilityPointStructure()
            }, sourcePath);

            var importedStructure = new TestStabilityPointStructure("a different id");

            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
                                                                                                 {
                                                                                                     importedStructure
                                                                                                 },
                                                                                                 sourcePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);

            TestStabilityPointStructure[] expectedCollection =
            {
                importedStructure
            };
            CollectionAssert.AreEqual(expectedCollection, targetCollection);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithoutOutputAndStructure_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var structure = new TestStabilityPointStructure();
            var calculation = new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                }
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;
            targetCollection.AddRange(new[]
            {
                structure
            }, sourcePath);

            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                                 sourcePath);

            // Assert
            Assert.IsNull(calculation.InputParameters.Structure);
            Assert.IsFalse(calculation.HasOutput);

            CollectionAssert.IsEmpty(targetCollection);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation.InputParameters,
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithOutputAndStructure_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var structure = new TestStabilityPointStructure();
            var calculation = new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                },
                Output = new TestStructuresOutput()
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;
            targetCollection.AddRange(new[]
            {
                structure
            }, sourcePath);

            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                                 sourcePath);

            // Assert
            Assert.IsNull(calculation.InputParameters.Structure);
            Assert.IsFalse(calculation.HasOutput);

            CollectionAssert.IsEmpty(targetCollection);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation,
                calculation.InputParameters,
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithSectionResultAndStructure_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var location = new Point2D(12, 34);
            var structure = new TestStabilityPointStructure(location);
            var calculation = new TestStabilityPointStructuresCalculationScenario
            {
                InputParameters =
                {
                    Structure = structure
                }
            };

            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    location
                })
            });
            StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.First();
            sectionResult.Calculation = calculation;

            StructureCollection<StabilityPointStructure> targetCollection = failureMechanism.StabilityPointStructures;
            targetCollection.AddRange(new[]
            {
                structure
            }, sourcePath);

            var strategy = new StabilityPointStructureReplaceStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<StabilityPointStructure>(),
                                                                                                 sourcePath);

            // Assert
            Assert.IsNull(calculation.InputParameters.Structure);
            Assert.IsNull(sectionResult.Calculation);

            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                sectionResult,
                calculation.InputParameters,
                targetCollection
            }, affectedObjects);
        }
    }
}