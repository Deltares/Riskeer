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
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Plugin.FileImporters;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.Structures;

namespace Riskeer.ClosingStructures.Plugin.Test.FileImporters
{
    public class ClosingStructureReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new ClosingStructureReplaceDataStrategy(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidFailureMechanism_CreatesNewInstance()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();

            // Call
            var strategy = new ClosingStructureReplaceDataStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<IStructureUpdateStrategy<ClosingStructure>>(strategy);
            Assert.IsInstanceOf<ReplaceDataStrategyBase<ClosingStructure, ClosingStructuresFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadClosingStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ClosingStructureReplaceDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            void Call() => strategy.UpdateStructuresWithImportedData(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("importedDataCollection", exception.ParamName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ClosingStructureReplaceDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            void Call() => strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sourceFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_DifferentSourcePath_UpdatesSourcePathOfTargetCollection()
        {
            // Setup 
            var failureMechanism = new ClosingStructuresFailureMechanism();
            StructureCollection<ClosingStructure> targetCollection = failureMechanism.ClosingStructures;

            var strategy = new ClosingStructureReplaceDataStrategy(failureMechanism);
            const string newSourcePath = "some/other/path";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(),
                                                                                                 newSourcePath);

            // Assert
            CollectionAssert.AreEqual(new IObservable[]
            {
                targetCollection
            }, affectedObjects);
            CollectionAssert.IsEmpty(targetCollection);
            Assert.AreEqual(newSourcePath, targetCollection.SourcePath);
        }

        [Test]
        public void UpdateStructuresWithImportedData_NoCurrentStructures_SetsSourcePath()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            StructureCollection<ClosingStructure> targetCollection = failureMechanism.ClosingStructures;

            var strategy = new ClosingStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(),
                                                                                                 sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
            Assert.AreEqual(sourceFilePath, targetCollection.SourcePath);
        }

        [Test]
        public void UpdateStructuresWithImportedData_NoCurrentStructuresWithImportedData_AddsNewStructure()
        {
            // Setup
            var importedStructures = new[]
            {
                new TestClosingStructure()
            };

            var failureMechanism = new ClosingStructuresFailureMechanism();
            var strategy = new ClosingStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures,
                                                                                                 sourceFilePath);

            // Assert
            StructureCollection<ClosingStructure> actualCollection = failureMechanism.ClosingStructures;
            CollectionAssert.AreEqual(importedStructures, actualCollection);
            CollectionAssert.AreEqual(new[]
            {
                actualCollection
            }, affectedObjects);
            Assert.AreEqual(sourceFilePath, actualCollection.SourcePath);
        }

        [Test]
        public void UpdateStructuresWithImportedData_WithCurrentAndImportedDataAreDifferent_ReplacesCurrentWithImportedData()
        {
            // Setup
            var failureMechanism = new ClosingStructuresFailureMechanism();
            failureMechanism.ClosingStructures.AddRange(new[]
            {
                new TestClosingStructure("id", "Original")
            }, sourceFilePath);

            var importedStructure = new TestClosingStructure("Different id", "Imported");

            var strategy = new ClosingStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                importedStructure
            }, sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.ClosingStructures
            }, affectedObjects);

            TestClosingStructure[] expected =
            {
                importedStructure
            };
            CollectionAssert.AreEqual(expected, failureMechanism.ClosingStructures);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithoutOutputAndStructure_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var structure = new TestClosingStructure();

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

            var strategy = new ClosingStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(),
                                                                                                 sourceFilePath).ToArray();

            // Assert
            Assert.IsNull(calculation.InputParameters.Structure);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation.InputParameters,
                failureMechanism.ClosingStructures
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithOutputAndStructure_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var structure = new TestClosingStructure();

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

            var strategy = new ClosingStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(),
                                                                                                 sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.Structure);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation,
                calculation.InputParameters,
                failureMechanism.ClosingStructures
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ImportedDataContainsDuplicateIds_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            ClosingStructure[] importedClosingStructures =
            {
                new TestClosingStructure(duplicateId, "name"),
                new TestClosingStructure(duplicateId, "Other name")
            };

            var strategy = new ClosingStructureReplaceDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            void Call() => strategy.UpdateStructuresWithImportedData(importedClosingStructures, sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(Call);
            string expectedMessage = "Kunstwerken moeten een unieke id hebben. " +
                                     $"Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}