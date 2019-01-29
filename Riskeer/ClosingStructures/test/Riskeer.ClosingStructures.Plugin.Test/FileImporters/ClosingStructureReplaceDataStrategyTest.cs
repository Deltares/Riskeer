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
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Data.TestUtil;
using Riskeer.ClosingStructures.Plugin.FileImporters;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.Structures;

namespace Riskeer.ClosingStructures.Plugin.Test.FileImporters
{
    public class ClosingStructureReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructure_NullFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ClosingStructureReplaceDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
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
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(null,
                                                                                string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new ClosingStructureReplaceDataStrategy(new ClosingStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(),
                                                                                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
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

            var calculation = new StructuresCalculation<ClosingStructuresInput>
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

            var calculation = new StructuresCalculation<ClosingStructuresInput>
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
        public void UpdateStructuresWithImportedData_CalculationWithSectionResultAndStructure_DataUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var location = new Point2D(1, 1);
            var structure = new TestClosingStructure(location);

            var calculation = new StructuresCalculation<ClosingStructuresInput>
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

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    location
                })
            });
            ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.First();
            sectionResult.Calculation = calculation;

            failureMechanism.ClosingStructures.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var strategy = new ClosingStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<ClosingStructure>(),
                                                                                                 sourceFilePath).ToArray();

            // Assert
            Assert.IsNull(sectionResult.Calculation);
            Assert.IsNull(calculation.InputParameters.Structure);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation.InputParameters,
                sectionResult,
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
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(importedClosingStructures,
                                                                                sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = "Kunstwerken moeten een unieke id hebben. " +
                                     $"Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}