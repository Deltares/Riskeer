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
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.IO.Importers;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.Plugin.FileImporter;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Plugin.Test.FileImporter
{
    [TestFixture]
    public class RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<ISurfaceLineUpdateDataStrategy>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<RingtoetsMacroStabilityInwardsSurfaceLine, MacroStabilityInwardsFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(null, Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new RingtoetsMacroStabilityInwardsSurfaceLineCollection(),
                                                                                  null,
                                                                                  string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new RingtoetsMacroStabilityInwardsSurfaceLineCollection(),
                                                                                  Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                  null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_OnlyGeometryChanged_UpdatesGeometryOnly()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();

            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineToUpdateFrom = CreateValidSurfaceLineForCalculations();
            var expectedGeometry = new List<Point3D>
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5),
                new Point3D(6, 7, 8)
            };
            expectedGeometry.AddRange(surfaceLine.Points);
            surfaceLineToUpdateFrom.SetGeometry(expectedGeometry);

            var targetCollection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
            targetCollection.AddRange(new[]
            {
                surfaceLine
            }, sourceFilePath);
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                        new[]
                                                        {
                                                            surfaceLineToUpdateFrom
                                                        },
                                                        sourceFilePath);

            // Assert
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint,
                            surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(surfaceLineToUpdateFrom.Points, surfaceLine.Points);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithoutCurrentSurfaceLinesAndReadLinesHaveDuplicateNames_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateName = "Duplicate name it is";
            var lineOne = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateName
            };
            lineOne.SetGeometry(new[]
            {
                new Point3D(1, 2, 3)
            });
            var lineTwo = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateName
            };
            lineTwo.SetGeometry(new[]
            {
                new Point3D(4, 5, 6)
            });
            var importedSurfaceLines = new[]
            {
                lineOne,
                lineTwo
            };

            var targetCollection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                  importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);

            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentLinesAndImportedMultipleLinesWithSameNames_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateName = "Duplicate name it is";
            var expectedSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = duplicateName
            };
            Point3D[] expectedGeometry =
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5),
                new Point3D(6, 7, 8)
            };
            expectedSurfaceLine.SetGeometry(expectedGeometry);

            var expectedCollection = new[]
            {
                expectedSurfaceLine
            };

            var targetCollection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
            targetCollection.AddRange(expectedCollection, sourceFilePath);

            RingtoetsMacroStabilityInwardsSurfaceLine[] importedSurfaceLines =
            {
                new RingtoetsMacroStabilityInwardsSurfaceLine
                {
                    Name = duplicateName
                },
                new RingtoetsMacroStabilityInwardsSurfaceLine
                {
                    Name = duplicateName
                }
            };

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                  importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.AreEqual(expectedCollection, targetCollection);
            RingtoetsMacroStabilityInwardsSurfaceLine actualSurfaceLine = targetCollection[0];
            Assert.AreEqual(expectedSurfaceLine.Name, actualSurfaceLine.Name);
            CollectionAssert.AreEqual(expectedGeometry, actualSurfaceLine.Points);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentCollectionNotEmptyAndImportedDataHasFullOverlap_UpdatesTargetCollection()
        {
            // Setup
            const string collectionSurfaceLineName = "Name A";
            var targetSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = collectionSurfaceLineName
            };
            var targetCollection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
            targetCollection.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);

            RingtoetsMacroStabilityInwardsSurfaceLine readSurfaceLine = DeepCloneAndModifyPoints(targetSurfaceLine);
            var readSurfacelines = new[]
            {
                readSurfaceLine
            };

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(new MacroStabilityInwardsFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   readSurfacelines,
                                                                                                   sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(targetSurfaceLine, targetCollection[0]);
            Assert.AreEqual(readSurfaceLine, targetSurfaceLine);

            CollectionAssert.AreEqual(new IObservable[]
            {
                targetCollection,
                targetSurfaceLine
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentCollectionNotEmptyAndImportedDataHasNoOverlap_UpdatesTargetCollection()
        {
            // Setup
            const string collectionSurfaceLineName = "Name A";
            const string readSurfaceLineName = "Name B";

            var targetSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = collectionSurfaceLineName
            };

            var readSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = readSurfaceLineName
            };
            var readSurfacelines = new[]
            {
                readSurfaceLine
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            RingtoetsMacroStabilityInwardsSurfaceLineCollection surfaceLineCollection = failureMechanism.SurfaceLines;
            surfaceLineCollection.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);
            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(surfaceLineCollection,
                                                                                                   readSurfacelines,
                                                                                                   sourceFilePath);

            // Assert
            Assert.AreEqual(1, surfaceLineCollection.Count);
            Assert.AreSame(readSurfaceLine, surfaceLineCollection[0]);

            CollectionAssert.AreEqual(new[]
            {
                surfaceLineCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentCollectionNotEmptyAndImportedDataHasPartialOverlap_UpdatesTargetCollection()
        {
            // Setup
            const string updatedSurfaceLineName = "Name A";
            const string removedSurfaceLineName = "Name B";
            const string addedSurfaceLineName = "Name C";

            var surfaceLineOne = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = updatedSurfaceLineName
            };

            var surfaceLineTwo = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = removedSurfaceLineName
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            RingtoetsMacroStabilityInwardsSurfaceLineCollection surfaceLineCollection = failureMechanism.SurfaceLines;
            surfaceLineCollection.AddRange(new[]
            {
                surfaceLineOne,
                surfaceLineTwo
            }, sourceFilePath);

            RingtoetsMacroStabilityInwardsSurfaceLine readSurfaceLineOne = DeepCloneAndModifyPoints(surfaceLineOne);
            var readSurfaceLineTwo = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = addedSurfaceLineName
            };
            var readSurfacelines = new[]
            {
                readSurfaceLineOne,
                readSurfaceLineTwo
            };

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(surfaceLineCollection,
                                                                                                   readSurfacelines,
                                                                                                   sourceFilePath);

            // Assert
            var expectedSurfaceLineCollection = new[]
            {
                surfaceLineOne,
                readSurfaceLineTwo
            };
            CollectionAssert.AreEqual(expectedSurfaceLineCollection, surfaceLineCollection);

            RingtoetsMacroStabilityInwardsSurfaceLine updatedSurfaceLine = surfaceLineCollection[0];
            Assert.AreSame(surfaceLineOne, updatedSurfaceLine);
            Assert.AreEqual(readSurfaceLineOne, updatedSurfaceLine);

            RingtoetsMacroStabilityInwardsSurfaceLine addedSurfaceLine = surfaceLineCollection[1];
            Assert.AreSame(readSurfaceLineTwo, addedSurfaceLine);
            Assert.AreEqual(readSurfaceLineTwo, addedSurfaceLine);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                readSurfaceLineOne,
                surfaceLineCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_CalculationWithOutputAndAssignedLineDeleted_ClearsCalculationOutput()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            RingtoetsMacroStabilityInwardsSurfaceLineCollection surfaceLineCollection = failureMechanism.SurfaceLines;
            surfaceLineCollection.AddRange(new[]
            {
                surfaceLine
            }, sourceFilePath);

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(surfaceLineCollection,
                                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                                   sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.SurfaceLine);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                surfaceLineCollection,
                calculation,
                calculation.InputParameters
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_MultipleCalculationsWithSurfaceLinesOneWithUpdatedLine_OnlyUpdatesCalculationWithUpdatedSurfaceLine()
        {
            // Setup
            const string updatedSurfaceLineName = "Name A";
            const string unaffectedSurfaceLineName = "Name B";

            var affectedSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = updatedSurfaceLineName
            };
            affectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var affectedCalculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = affectedSurfaceLine
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };

            var unaffectedGeometry = new[]
            {
                new Point3D(10, 9, 8),
                new Point3D(7, 6, 5)
            };
            var unaffectedSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = unaffectedSurfaceLineName
            };
            unaffectedSurfaceLine.SetGeometry(unaffectedGeometry);
            var unAffectedCalculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = unaffectedSurfaceLine
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };

            var collection = new RingtoetsMacroStabilityInwardsSurfaceLineCollection();
            collection.AddRange(new[]
            {
                affectedSurfaceLine,
                unaffectedSurfaceLine
            }, sourceFilePath);

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unAffectedCalculation);

            RingtoetsMacroStabilityInwardsSurfaceLine importedAffectedSurfaceLine = DeepCloneAndModifyPoints(affectedSurfaceLine);
            RingtoetsMacroStabilityInwardsSurfaceLine importedUnaffectedSurfaceLine = DeepCloneName(unaffectedSurfaceLine);
            importedUnaffectedSurfaceLine.SetGeometry(unaffectedGeometry);

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(collection,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedAffectedSurfaceLine,
                                                                                                       importedUnaffectedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            Assert.IsTrue(unAffectedCalculation.HasOutput);
            MacroStabilityInwardsInput unaffectedInput = unAffectedCalculation.InputParameters;
            Assert.AreSame(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);
            Assert.AreEqual(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);

            Assert.IsFalse(affectedCalculation.HasOutput);
            MacroStabilityInwardsInput affectedInput = affectedCalculation.InputParameters;
            Assert.AreSame(affectedSurfaceLine, affectedInput.SurfaceLine);
            Assert.AreEqual(affectedSurfaceLine, affectedInput.SurfaceLine);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                collection,
                affectedCalculation,
                affectedInput,
                affectedSurfaceLine
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_MultipleCalculationsWithSurfaceLinesOneWithRemovedLine_OnlyUpdatesCalculationWithRemovedSurfaceLine()
        {
            // Setup
            const string removedSurfaceLineName = "Name A";
            const string unaffectedSurfaceLineName = "Name B";

            var removedSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = removedSurfaceLineName
            };
            removedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var affectedCalculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = removedSurfaceLine
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };

            var unaffectedGeometry = new[]
            {
                new Point3D(10, 9, 8),
                new Point3D(7, 6, 5)
            };
            var unaffectedSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = unaffectedSurfaceLineName
            };
            unaffectedSurfaceLine.SetGeometry(unaffectedGeometry);
            var unAffectedCalculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = unaffectedSurfaceLine
                },
                Output = new TestMacroStabilityInwardsOutput(),
                SemiProbabilisticOutput = new TestMacroStabilityInwardsSemiProbabilisticOutput()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            RingtoetsMacroStabilityInwardsSurfaceLineCollection collection = failureMechanism.SurfaceLines;
            collection.AddRange(new[]
            {
                removedSurfaceLine,
                unaffectedSurfaceLine
            }, sourceFilePath);
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unAffectedCalculation);

            RingtoetsMacroStabilityInwardsSurfaceLine importedUnaffectedSurfaceLine = DeepCloneName(unaffectedSurfaceLine);
            importedUnaffectedSurfaceLine.SetGeometry(unaffectedGeometry);

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(collection,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedUnaffectedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            Assert.IsTrue(unAffectedCalculation.HasOutput);
            MacroStabilityInwardsInput unaffectedInput = unAffectedCalculation.InputParameters;
            Assert.AreSame(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);
            Assert.AreEqual(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);

            Assert.IsFalse(affectedCalculation.HasOutput);
            MacroStabilityInwardsInput affectedInput = affectedCalculation.InputParameters;
            Assert.IsNull(affectedInput.SurfaceLine);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                collection,
                affectedCalculation,
                affectedInput
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCalculationAssignedToUpdatedLine_UpdatesCalculationAndStochasticSoilModel()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            var soilModels = new[]
            {
                new StochasticSoilModel(1, "A", "B")
                {
                    Geometry =
                    {
                        new Point2D(2, -1),
                        new Point2D(2, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1)
                    }
                },
                new StochasticSoilModel(2, "C", "D")
                {
                    Geometry =
                    {
                        new Point2D(-2, -1),
                        new Point2D(-2, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 2)
                    }
                }
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            RingtoetsMacroStabilityInwardsSurfaceLine importedSurfaceLine = DeepCloneName(surfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            MacroStabilityInwardsInput calculationInput = calculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                failureMechanism.SurfaceLines,
                surfaceLine,
                calculationInput
            }, affectedObjects);
            Assert.AreSame(surfaceLine, calculationInput.SurfaceLine);
            CollectionAssert.AreEqual(importedSurfaceLine.Points, surfaceLine.Points);
            Assert.AreEqual(soilModels[0], calculationInput.StochasticSoilModel);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCalculationAssignedToUpdatedLineAndMultipleMatchingSoilModels_UpdatesCalculationAndStochasticSoilModelToNull()
        {
            // Setup
            var soilModels = new[]
            {
                new StochasticSoilModel(1, "A", "B")
                {
                    Geometry =
                    {
                        new Point2D(2, -1),
                        new Point2D(2, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1)
                    }
                },
                new StochasticSoilModel(2, "C", "D")
                {
                    Geometry =
                    {
                        new Point2D(-2, -1),
                        new Point2D(-2, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 2)
                    }
                },
                new StochasticSoilModel(3, "E", "F")
                {
                    Geometry =
                    {
                        new Point2D(6, -1),
                        new Point2D(6, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 3)
                    }
                }
            };

            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = soilModels[1]
                }
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");

            RingtoetsMacroStabilityInwardsSurfaceLine importedSurfaceLine = DeepCloneName(surfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            MacroStabilityInwardsInput calculationInput = calculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                failureMechanism.SurfaceLines,
                surfaceLine,
                calculationInput
            }, affectedObjects);
            Assert.AreSame(surfaceLine, calculationInput.SurfaceLine);
            CollectionAssert.AreEqual(importedSurfaceLine.Points, surfaceLine.Points);
            Assert.IsNull(calculationInput.StochasticSoilModel);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCalculationAssignedToRemovedLine_UpdatesCalculationAndDoesNotRemoveStochasticSoilInputs()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A", "B")
            {
                Geometry =
                {
                    new Point2D(2, -1),
                    new Point2D(2, 1)
                },
                StochasticSoilProfiles =
                {
                    new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1)
                }
            };

            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = soilModel,
                    StochasticSoilProfile = soilModel.StochasticSoilProfiles[0]
                }
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                soilModel
            }, "path");

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                                   "path").ToArray();

            // Assert
            MacroStabilityInwardsInput calculationInput = calculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                failureMechanism.SurfaceLines,
                calculationInput
            }, affectedObjects);
            Assert.IsNull(calculationInput.SurfaceLine);
            Assert.AreSame(soilModel, calculationInput.StochasticSoilModel);
            Assert.AreSame(soilModel.StochasticSoilProfiles[0], calculationInput.StochasticSoilProfile);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCalculationAssignedToRemovedLine_UpdatesCalculationAndDoesNotRemoveStochasticSoilModelInput()
        {
            // Setup
            var soilModel = new StochasticSoilModel(1, "A", "B")
            {
                Geometry =
                {
                    new Point2D(2, -1),
                    new Point2D(2, 1)
                }
            };

            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = soilModel
                }
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                soilModel
            }, "path");

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   Enumerable.Empty<RingtoetsMacroStabilityInwardsSurfaceLine>(),
                                                                                                   "path").ToArray();

            // Assert
            MacroStabilityInwardsInput calculationInput = calculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                failureMechanism.SurfaceLines,
                calculationInput
            }, affectedObjects);
            Assert.IsNull(calculationInput.SurfaceLine);
            Assert.AreSame(soilModel, calculationInput.StochasticSoilModel);
            Assert.IsNull(calculationInput.StochasticSoilProfile);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_MultipleCalculations_OnlyUpdatesCalculationWithUpdatedSurfaceLine()
        {
            // Setup
            const string updatedSurfaceLineName = "Name A";

            var affectedSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = updatedSurfaceLineName
            };
            affectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var affectedCalculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = affectedSurfaceLine
                }
            };

            var unaffectedGeometry = new[]
            {
                new Point3D(10, 9, 8),
                new Point3D(7, 6, 5)
            };
            var unaffectedSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            unaffectedSurfaceLine.SetGeometry(unaffectedGeometry);
            var unAffectedCalculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = unaffectedSurfaceLine
                }
            };

            var soilModels = new[]
            {
                new StochasticSoilModel(1, "A", "B")
                {
                    Geometry =
                    {
                        new Point2D(2, -1),
                        new Point2D(2, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.2, SoilProfileType.SoilProfile1D, 1)
                    }
                },
                new StochasticSoilModel(2, "C", "D")
                {
                    Geometry =
                    {
                        new Point2D(-2, -1),
                        new Point2D(-2, 1)
                    },
                    StochasticSoilProfiles =
                    {
                        new StochasticSoilProfile(0.3, SoilProfileType.SoilProfile1D, 2)
                    }
                }
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unAffectedCalculation);

            failureMechanism.SurfaceLines.AddRange(new[]
            {
                affectedSurfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");

            RingtoetsMacroStabilityInwardsSurfaceLine importedSurfaceLine = DeepCloneName(affectedSurfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            MacroStabilityInwardsInput affectedInput = affectedCalculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                failureMechanism.SurfaceLines,
                affectedInput,
                affectedSurfaceLine
            }, affectedObjects);
            Assert.AreSame(affectedSurfaceLine, affectedInput.SurfaceLine);
            CollectionAssert.AreEqual(importedSurfaceLine.Points, affectedSurfaceLine.Points);
            Assert.AreEqual(soilModels[0], affectedInput.StochasticSoilModel);

            MacroStabilityInwardsInput unaffectedInput = unAffectedCalculation.InputParameters;
            Assert.AreSame(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);
            CollectionAssert.AreEqual(unaffectedGeometry, unaffectedSurfaceLine.Points);
            Assert.IsNull(unaffectedInput.StochasticSoilModel);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_CalculationWithSameReferences_OnlyReturnsDistinctCalculation()
        {
            // Setup
            const string updatedSurfaceLineName = "Name A";

            var affectedSurfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = updatedSurfaceLineName
            };
            affectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var affectedCalculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput())
            {
                InputParameters =
                {
                    SurfaceLine = affectedSurfaceLine
                },
                Output = new TestMacroStabilityInwardsOutput()
            };

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);

            failureMechanism.SurfaceLines.AddRange(new[]
            {
                affectedSurfaceLine
            }, "path");

            RingtoetsMacroStabilityInwardsSurfaceLine importedSurfaceLine = DeepCloneAndModifyPoints(affectedSurfaceLine);

            var strategy = new RingtoetsMacroStabilityInwardsSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                failureMechanism.SurfaceLines,
                affectedSurfaceLine,
                affectedCalculation,
                affectedCalculation.InputParameters
            }, affectedObjects);
        }

        private static RingtoetsMacroStabilityInwardsSurfaceLine CreateValidSurfaceLineForCalculations()
        {
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = "Name A",
                ReferenceLineIntersectionWorldPoint = new Point2D(123, 456)
            };
            var geometry = new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            };
            surfaceLine.SetGeometry(geometry);

            return surfaceLine;
        }

        /// <summary>
        /// Makes a deep clone of <paramref name="surfaceLine"/> 
        /// and only copies the name <see cref="RingtoetsMacroStabilityInwardsSurfaceLine.Name"/>
        /// property.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> 
        /// which needs to be deep cloned.</param>
        /// <returns>A deep clone of <paramref name="surfaceLine"/> with
        /// only the name property copied.</returns>
        private static RingtoetsMacroStabilityInwardsSurfaceLine DeepCloneName(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = surfaceLine.Name
            };
        }

        /// <summary>
        /// Makes a deep clone of the <paramref name="surfaceLine"/> and sets a 
        /// new geometry, reference line intersection world reference point and 
        /// characteristic points.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> 
        /// which needs to be deep cloned and modified.</param>
        /// <returns>A deep clone of <paramref name="surfaceLine"/> with modified 
        /// geometric and characteristic points.</returns>
        private static RingtoetsMacroStabilityInwardsSurfaceLine DeepCloneAndModifyPoints(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            var random = new Random(21);
            Point2D newIntersectionPoint = null;
            if (surfaceLine.ReferenceLineIntersectionWorldPoint != null)
            {
                Point2D oldIntersectionPoint = surfaceLine.ReferenceLineIntersectionWorldPoint;
                newIntersectionPoint = new Point2D(oldIntersectionPoint.X + random.NextDouble(),
                                                   oldIntersectionPoint.Y + random.NextDouble());
            }

            var copiedLine = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = surfaceLine.Name,
                ReferenceLineIntersectionWorldPoint = newIntersectionPoint
            };

            var newGeometry = new[]
            {
                new Point3D(6, 0, 10),
                new Point3D(7, 0, 11),
                new Point3D(8, 0, 12),
                new Point3D(9, 0, 13),
                new Point3D(10, 0, 14),
                new Point3D(11, 0, 15)
            };
            copiedLine.SetGeometry(newGeometry);
            return copiedLine;
        }
    }
}