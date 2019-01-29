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
using Riskeer.Common.Data;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.UpdateDataStrategies;
using Riskeer.Common.IO.Structures;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.Plugin.FileImporters;

namespace Riskeer.HeightStructures.Plugin.Test.FileImporters
{
    public class HeightStructureReplaceDataStrategyTest
    {
        private const string sourceFilePath = "path";

        [Test]
        public void Constructure_NullFailureMechanism_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HeightStructureReplaceDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_ValidFailureMechanism_CreatesNewInstance()
        {
            // Setup
            var failureMechanism = new HeightStructuresFailureMechanism();

            // Call
            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Assert
            Assert.IsInstanceOf<IStructureUpdateStrategy<HeightStructure>>(strategy);
            Assert.IsInstanceOf<ReplaceDataStrategyBase<HeightStructure, HeightStructuresFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ReadHeightStructuresNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new HeightStructureReplaceDataStrategy(new HeightStructuresFailureMechanism());

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
            var strategy = new HeightStructureReplaceDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
                                                                                null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateStructuresWithImportedData_DifferentSourcePath_UpdatesSourcePathOfTargetCollection()
        {
            // Setup 
            var failureMechanism = new HeightStructuresFailureMechanism();
            StructureCollection<HeightStructure> targetCollection = failureMechanism.HeightStructures;

            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);
            const string newSourcePath = "some/other/path";

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            StructureCollection<HeightStructure> targetCollection = failureMechanism.HeightStructures;

            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
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
                new TestHeightStructure()
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(importedStructures,
                                                                                                 sourceFilePath);

            // Assert
            StructureCollection<HeightStructure> actualCollection = failureMechanism.HeightStructures;
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
            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.HeightStructures.AddRange(new[]
            {
                new TestHeightStructure("id", "Original")
            }, sourceFilePath);

            var importedStructure = new TestHeightStructure("Different id", "Imported");

            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(new[]
            {
                importedStructure
            }, sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                failureMechanism.HeightStructures
            }, affectedObjects);

            TestHeightStructure[] expected =
            {
                importedStructure
            };
            CollectionAssert.AreEqual(expected, failureMechanism.HeightStructures);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithoutOutputAndStructure_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var structure = new TestHeightStructure();

            var calculation = new StructuresCalculation<HeightStructuresInput>
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

            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
                                                                                                 sourceFilePath).ToArray();

            // Assert
            Assert.IsNull(calculation.InputParameters.Structure);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation.InputParameters,
                failureMechanism.HeightStructures
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithOutputAndStructure_CalculationUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var structure = new TestHeightStructure();

            var calculation = new StructuresCalculation<HeightStructuresInput>
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

            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
                                                                                                 sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.Structure);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation,
                calculation.InputParameters,
                failureMechanism.HeightStructures
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_CalculationWithSectionResultAndStructure_DataUpdatedAndReturnsAffectedObject()
        {
            // Setup
            var location = new Point2D(1, 1);
            var structure = new TestHeightStructure(location);

            var calculation = new StructuresCalculation<HeightStructuresInput>
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

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
            {
                FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                {
                    location
                })
            });
            HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.First();
            sectionResult.Calculation = calculation;

            failureMechanism.HeightStructures.AddRange(new[]
            {
                structure
            }, sourceFilePath);

            var strategy = new HeightStructureReplaceDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateStructuresWithImportedData(Enumerable.Empty<HeightStructure>(),
                                                                                                 sourceFilePath).ToArray();

            // Assert
            Assert.IsNull(sectionResult.Calculation);
            Assert.IsNull(calculation.InputParameters.Structure);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                calculation.InputParameters,
                sectionResult,
                failureMechanism.HeightStructures
            }, affectedObjects);
        }

        [Test]
        public void UpdateStructuresWithImportedData_ImportedDataContainsDuplicateIds_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateId = "I am a duplicate id";
            HeightStructure[] importedHeightStructures =
            {
                new TestHeightStructure(duplicateId, "name"),
                new TestHeightStructure(duplicateId, "Other name")
            };

            var strategy = new HeightStructureReplaceDataStrategy(new HeightStructuresFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateStructuresWithImportedData(importedHeightStructures,
                                                                                sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            string expectedMessage = "Kunstwerken moeten een unieke id hebben. " +
                                     $"Gevonden dubbele elementen: {duplicateId}.";
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}