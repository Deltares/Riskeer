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
using NUnit.Framework;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineUpdateDataStrategyTest
    {
        private const string sourceFilePath = "some/path";

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingSurfaceLineUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new PipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<ISurfaceLineUpdateDataStrategy<RingtoetsPipingSurfaceLine>>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<RingtoetsPipingSurfaceLine, PipingFailureMechanism>>(strategy);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new PipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(null,
                                                                                  string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("importedDataCollection", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_SourceFilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new PipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                  null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_OnlyGeometryChanged_UpdatesGeometryOnly()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();

            RingtoetsPipingSurfaceLine surfaceLineToUpdateFrom = CreateValidSurfaceLineForCalculations();
            var expectedGeometry = new List<Point3D>
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5),
                new Point3D(6, 7, 8)
            };
            expectedGeometry.AddRange(surfaceLine.Points);
            surfaceLineToUpdateFrom.SetGeometry(expectedGeometry);

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection targetCollection = failureMechanism.SurfaceLines;
            targetCollection.AddRange(new[]
            {
                surfaceLine
            }, sourceFilePath);
            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            strategy.UpdateSurfaceLinesWithImportedData(new[]
                                                        {
                                                            surfaceLineToUpdateFrom
                                                        },
                                                        sourceFilePath);

            // Assert
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint,
                            surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(surfaceLineToUpdateFrom.Points, surfaceLine.Points);
            AssertCharacteristicPoints(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_OnlyReferenceLineIntersectionPointChanged_UpdatesCharacteristicPointsOnly()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();

            RingtoetsPipingSurfaceLine surfaceLineToUpdateFrom = CreateValidSurfaceLineForCalculations();
            surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint = new Point2D(123, 456);

            var targetCollection = new RingtoetsPipingSurfaceLineCollection();
            targetCollection.AddRange(new[]
            {
                surfaceLine
            }, sourceFilePath);
            var strategy = new PipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            strategy.UpdateSurfaceLinesWithImportedData(new[]
                                                        {
                                                            surfaceLineToUpdateFrom
                                                        },
                                                        sourceFilePath);

            // Assert
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint,
                            surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(surfaceLineToUpdateFrom.Points, surfaceLine.Points);
            AssertCharacteristicPoints(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_OnlyCharacteristicPointsChanged_UpdatesCharacteristicPointsOnly()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var surfaceLineToUpdateFrom = new RingtoetsPipingSurfaceLine
            {
                Name = surfaceLine.Name
            };
            surfaceLineToUpdateFrom.SetGeometry(surfaceLine.Points);

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection targetCollection = failureMechanism.SurfaceLines;
            targetCollection.AddRange(new[]
            {
                surfaceLine
            }, sourceFilePath);
            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            strategy.UpdateSurfaceLinesWithImportedData(new[]
                                                        {
                                                            surfaceLineToUpdateFrom
                                                        },
                                                        sourceFilePath);

            // Assert
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint,
                            surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(surfaceLineToUpdateFrom.Points, surfaceLine.Points);
            AssertCharacteristicPoints(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_GeometryAndReferenceLineIntersectionPointAndCharacteristicPointsChanged_UpdatesRelevantProperties()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            RingtoetsPipingSurfaceLine surfaceLineToUpdateFrom = DeepCloneAndModifyPoints(surfaceLine);

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection targetCollection = failureMechanism.SurfaceLines;
            targetCollection.AddRange(new[]
            {
                surfaceLine
            }, sourceFilePath);
            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            strategy.UpdateSurfaceLinesWithImportedData(new[]
                                                        {
                                                            surfaceLineToUpdateFrom
                                                        },
                                                        sourceFilePath);

            // Assert
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint,
                            surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(surfaceLineToUpdateFrom.Points, surfaceLine.Points);
            AssertCharacteristicPoints(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithoutCurrentSurfaceLinesAndReadLinesHaveDuplicateNames_ThrowsUpdateDataException()
        {
            // Setup
            const string duplicateName = "Duplicate name it is";
            var lineOne = new RingtoetsPipingSurfaceLine
            {
                Name = duplicateName
            };
            lineOne.SetGeometry(new[]
            {
                new Point3D(1, 2, 3)
            });
            var lineTwo = new RingtoetsPipingSurfaceLine
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

            var targetCollection = new RingtoetsPipingSurfaceLineCollection();
            var strategy = new PipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(importedSurfaceLines,
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
            var expectedSurfaceLine = new RingtoetsPipingSurfaceLine
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

            var targetCollection = new RingtoetsPipingSurfaceLineCollection();
            targetCollection.AddRange(expectedCollection, sourceFilePath);

            RingtoetsPipingSurfaceLine[] importedSurfaceLines =
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateName
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = duplicateName
                }
            };

            var strategy = new PipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<UpdateDataException>(call);
            const string expectedMessage = "Geïmporteerde data moet unieke elementen bevatten.";
            Assert.AreEqual(expectedMessage, exception.Message);

            CollectionAssert.AreEqual(expectedCollection, targetCollection);
            RingtoetsPipingSurfaceLine actualSurfaceLine = targetCollection[0];
            Assert.AreEqual(expectedSurfaceLine.Name, actualSurfaceLine.Name);
            CollectionAssert.AreEqual(expectedGeometry, actualSurfaceLine.Points);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentCollectionNotEmptyAndImportedDataHasFullOverlap_UpdatesTargetCollection()
        {
            // Setup
            const string collectionSurfaceLineName = "Name A";
            var targetSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = collectionSurfaceLineName
            };
            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection targetCollection = failureMechanism.SurfaceLines;
            targetCollection.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);

            RingtoetsPipingSurfaceLine readSurfaceLine = DeepCloneAndModifyPoints(targetSurfaceLine);
            var readSurfaceLines = new[]
            {
                readSurfaceLine
            };

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(readSurfaceLines,
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

            var targetSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = collectionSurfaceLineName
            };

            var readSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = readSurfaceLineName
            };
            var readSurfaceLines = new[]
            {
                readSurfaceLine
            };

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection surfaceLineCollection = failureMechanism.SurfaceLines;
            surfaceLineCollection.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);
            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(readSurfaceLines,
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

            var surfaceLineOne = new RingtoetsPipingSurfaceLine
            {
                Name = updatedSurfaceLineName
            };

            var surfaceLineTwo = new RingtoetsPipingSurfaceLine
            {
                Name = removedSurfaceLineName
            };

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection surfaceLineCollection = failureMechanism.SurfaceLines;
            surfaceLineCollection.AddRange(new[]
            {
                surfaceLineOne,
                surfaceLineTwo
            }, sourceFilePath);

            RingtoetsPipingSurfaceLine readSurfaceLineOne = DeepCloneAndModifyPoints(surfaceLineOne);
            var readSurfaceLineTwo = new RingtoetsPipingSurfaceLine
            {
                Name = addedSurfaceLineName
            };
            var readSurfaceLines = new[]
            {
                readSurfaceLineOne,
                readSurfaceLineTwo
            };

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(readSurfaceLines,
                                                                                                   sourceFilePath);

            // Assert
            var expectedSurfaceLineCollection = new[]
            {
                surfaceLineOne,
                readSurfaceLineTwo
            };
            CollectionAssert.AreEqual(expectedSurfaceLineCollection, surfaceLineCollection);

            RingtoetsPipingSurfaceLine updatedSurfaceLine = surfaceLineCollection[0];
            Assert.AreSame(surfaceLineOne, updatedSurfaceLine);
            Assert.AreEqual(readSurfaceLineOne, updatedSurfaceLine);

            RingtoetsPipingSurfaceLine addedSurfaceLine = surfaceLineCollection[1];
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
            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            RingtoetsPipingSurfaceLineCollection surfaceLineCollection = failureMechanism.SurfaceLines;
            surfaceLineCollection.AddRange(new[]
            {
                surfaceLine
            }, sourceFilePath);

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
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

            var affectedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = updatedSurfaceLineName
            };
            affectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var affectedCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = affectedSurfaceLine
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            var unaffectedGeometry = new[]
            {
                new Point3D(10, 9, 8),
                new Point3D(7, 6, 5)
            };
            var unaffectedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = unaffectedSurfaceLineName
            };
            unaffectedSurfaceLine.SetGeometry(unaffectedGeometry);
            var unAffectedCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = unaffectedSurfaceLine
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection collection = failureMechanism.SurfaceLines;
            collection.AddRange(new[]
            {
                affectedSurfaceLine,
                unaffectedSurfaceLine
            }, sourceFilePath);

            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unAffectedCalculation);

            RingtoetsPipingSurfaceLine importedAffectedSurfaceLine = DeepCloneAndModifyPoints(affectedSurfaceLine);
            RingtoetsPipingSurfaceLine importedUnaffectedSurfaceLine = DeepCloneName(unaffectedSurfaceLine);
            importedUnaffectedSurfaceLine.SetGeometry(unaffectedGeometry);

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
            {
                importedAffectedSurfaceLine,
                importedUnaffectedSurfaceLine
            }, "path").ToArray();

            // Assert
            Assert.IsTrue(unAffectedCalculation.HasOutput);
            PipingInput unaffectedInput = unAffectedCalculation.InputParameters;
            Assert.AreSame(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);
            Assert.AreEqual(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);

            Assert.IsFalse(affectedCalculation.HasOutput);
            PipingInput affectedInput = affectedCalculation.InputParameters;
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

            var removedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = removedSurfaceLineName
            };
            removedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var affectedCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = removedSurfaceLine
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            var unaffectedGeometry = new[]
            {
                new Point3D(10, 9, 8),
                new Point3D(7, 6, 5)
            };
            var unaffectedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = unaffectedSurfaceLineName
            };
            unaffectedSurfaceLine.SetGeometry(unaffectedGeometry);
            var unAffectedCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = unaffectedSurfaceLine
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection collection = failureMechanism.SurfaceLines;
            collection.AddRange(new[]
            {
                removedSurfaceLine,
                unaffectedSurfaceLine
            }, sourceFilePath);
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unAffectedCalculation);

            RingtoetsPipingSurfaceLine importedUnaffectedSurfaceLine = DeepCloneName(unaffectedSurfaceLine);
            importedUnaffectedSurfaceLine.SetGeometry(unaffectedGeometry);

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
            {
                importedUnaffectedSurfaceLine
            }, "path").ToArray();

            // Assert
            Assert.IsTrue(unAffectedCalculation.HasOutput);
            PipingInput unaffectedInput = unAffectedCalculation.InputParameters;
            Assert.AreSame(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);
            Assert.AreEqual(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);

            Assert.IsFalse(affectedCalculation.HasOutput);
            PipingInput affectedInput = affectedCalculation.InputParameters;
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
            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine
                }
            };

            var soilModels = new[]
            {
                new StochasticSoilModel("A")
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
                new StochasticSoilModel("C")
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

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            RingtoetsPipingSurfaceLine importedSurfaceLine = DeepCloneName(surfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
            {
                importedSurfaceLine
            }, "path").ToArray();

            // Assert
            PipingInput calculationInput = calculation.InputParameters;
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
                new StochasticSoilModel("A")
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
                new StochasticSoilModel("C")
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
                new StochasticSoilModel("E")
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

            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = soilModels[1]
                }
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");

            RingtoetsPipingSurfaceLine importedSurfaceLine = DeepCloneName(surfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
            {
                importedSurfaceLine
            }, "path").ToArray();

            // Assert
            PipingInput calculationInput = calculation.InputParameters;
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
            var soilModel = new StochasticSoilModel("A")
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

            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = soilModel,
                    StochasticSoilProfile = soilModel.StochasticSoilProfiles[0]
                }
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                soilModel
            }, "path");

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                   "path").ToArray();

            // Assert
            PipingInput calculationInput = calculation.InputParameters;
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
            var soilModel = new StochasticSoilModel("A")
            {
                Geometry =
                {
                    new Point2D(2, -1),
                    new Point2D(2, 1)
                }
            };

            RingtoetsPipingSurfaceLine surfaceLine = CreateValidSurfaceLineForCalculations();
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = soilModel
                }
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                soilModel
            }, "path");

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                   "path").ToArray();

            // Assert
            PipingInput calculationInput = calculation.InputParameters;
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

            var affectedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = updatedSurfaceLineName
            };
            affectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var affectedCalculation = new PipingCalculation(new GeneralPipingInput())
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
            var unaffectedSurfaceLine = new RingtoetsPipingSurfaceLine();
            unaffectedSurfaceLine.SetGeometry(unaffectedGeometry);
            var unAffectedCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = unaffectedSurfaceLine
                }
            };

            var soilModels = new[]
            {
                new StochasticSoilModel("A")
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
                new StochasticSoilModel("C")
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

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unAffectedCalculation);

            failureMechanism.SurfaceLines.AddRange(new[]
            {
                affectedSurfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");

            RingtoetsPipingSurfaceLine importedSurfaceLine = DeepCloneName(affectedSurfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
            {
                importedSurfaceLine
            }, "path").ToArray();

            // Assert
            PipingInput affectedInput = affectedCalculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                failureMechanism.SurfaceLines,
                affectedInput,
                affectedSurfaceLine
            }, affectedObjects);
            Assert.AreSame(affectedSurfaceLine, affectedInput.SurfaceLine);
            CollectionAssert.AreEqual(importedSurfaceLine.Points, affectedSurfaceLine.Points);
            Assert.AreEqual(soilModels[0], affectedInput.StochasticSoilModel);

            PipingInput unaffectedInput = unAffectedCalculation.InputParameters;
            Assert.AreSame(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);
            CollectionAssert.AreEqual(unaffectedGeometry, unaffectedSurfaceLine.Points);
            Assert.IsNull(unaffectedInput.StochasticSoilModel);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_CalculationWithSameReferences_OnlyReturnsDistinctCalculation()
        {
            // Setup
            const string updatedSurfaceLineName = "Name A";

            var affectedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = updatedSurfaceLineName
            };
            affectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            var affectedCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = affectedSurfaceLine
                },
                Output = new TestPipingOutput()
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);

            failureMechanism.SurfaceLines.AddRange(new[]
            {
                affectedSurfaceLine
            }, "path");

            RingtoetsPipingSurfaceLine importedSurfaceLine = DeepCloneAndModifyPoints(affectedSurfaceLine);

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
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

        [Test]
        public void UpdateSurfaceLinesWithImportedData_EntryPointNotOnSurfaceLineAnymore_SetsEntryPointToNaN()
        {
            // Setup
            const string updatedSurfaceLineName = "Name A";
            var affectedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = updatedSurfaceLineName
            };
            affectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            var affectedCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = affectedSurfaceLine,
                    EntryPointL = (RoundedDouble) 2
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection collection = failureMechanism.SurfaceLines;
            collection.AddRange(new[]
            {
                affectedSurfaceLine
            }, sourceFilePath);

            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);

            RingtoetsPipingSurfaceLine importedAffectedSurfaceLine = DeepCloneName(affectedSurfaceLine);
            importedAffectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 1)
            });

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
            {
                importedAffectedSurfaceLine
            }, "path").ToArray();

            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            PipingInput affectedInput = affectedCalculation.InputParameters;
            Assert.AreSame(affectedSurfaceLine, affectedInput.SurfaceLine);
            Assert.IsNaN(affectedInput.EntryPointL);
            CollectionAssert.AreEqual(importedAffectedSurfaceLine.Points, affectedSurfaceLine.Points);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                collection,
                affectedCalculation,
                affectedInput,
                affectedSurfaceLine
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ExitPointNotOnSurfaceLineAnymore_SetsExitPointToNaN()
        {
            // Setup
            const string updatedSurfaceLineName = "Name A";
            var affectedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = updatedSurfaceLineName
            };
            affectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            });
            var affectedCalculation = new PipingCalculation(new GeneralPipingInput())
            {
                InputParameters =
                {
                    SurfaceLine = affectedSurfaceLine,
                    EntryPointL = (RoundedDouble) 0,
                    ExitPointL = (RoundedDouble) 3
                },
                Output = new TestPipingOutput(),
                SemiProbabilisticOutput = new TestPipingSemiProbabilisticOutput()
            };

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection collection = failureMechanism.SurfaceLines;
            collection.AddRange(new[]
            {
                affectedSurfaceLine
            }, sourceFilePath);

            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);

            RingtoetsPipingSurfaceLine importedAffectedSurfaceLine = DeepCloneName(affectedSurfaceLine);
            importedAffectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 1),
                new Point3D(1, 0, 2)
            });

            var strategy = new PipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(new[]
            {
                importedAffectedSurfaceLine
            }, "path").ToArray();

            // Assert
            Assert.IsFalse(affectedCalculation.HasOutput);
            PipingInput affectedInput = affectedCalculation.InputParameters;
            Assert.AreSame(affectedSurfaceLine, affectedInput.SurfaceLine);
            Assert.AreEqual(0, affectedInput.EntryPointL.Value);
            Assert.IsNaN(affectedInput.ExitPointL);
            CollectionAssert.AreEqual(importedAffectedSurfaceLine.Points, affectedSurfaceLine.Points);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                collection,
                affectedCalculation,
                affectedInput,
                affectedSurfaceLine
            }, affectedObjects);
        }

        private static RingtoetsPipingSurfaceLine CreateValidSurfaceLineForCalculations()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
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
            surfaceLine.SetBottomDitchDikeSideAt(geometry[0]);
            surfaceLine.SetBottomDitchPolderSideAt(geometry[1]);
            surfaceLine.SetDikeToeAtPolderAt(geometry[2]);
            surfaceLine.SetDikeToeAtRiverAt(geometry[3]);
            surfaceLine.SetDitchDikeSideAt(geometry[4]);
            surfaceLine.SetDitchPolderSideAt(geometry[5]);

            return surfaceLine;
        }

        /// <summary>
        /// Makes a deep clone of <paramref name="surfaceLine"/> 
        /// and only copies the name <see cref="RingtoetsPipingSurfaceLine.Name"/>
        /// property.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> 
        /// which needs to be deep cloned.</param>
        /// <returns>A deep clone of <paramref name="surfaceLine"/> with
        /// only the name property copied.</returns>
        private static RingtoetsPipingSurfaceLine DeepCloneName(RingtoetsPipingSurfaceLine surfaceLine)
        {
            return new RingtoetsPipingSurfaceLine
            {
                Name = surfaceLine.Name
            };
        }

        /// <summary>
        /// Makes a deep clone of the <paramref name="surfaceLine"/> and sets a 
        /// new geometry, reference line intersection world reference point and 
        /// characteristic points.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> 
        /// which needs to be deep cloned and modified.</param>
        /// <returns>A deep clone of <paramref name="surfaceLine"/> with modified 
        /// geometric and characteristic points.</returns>
        private static RingtoetsPipingSurfaceLine DeepCloneAndModifyPoints(RingtoetsPipingSurfaceLine surfaceLine)
        {
            var random = new Random(21);
            Point2D newIntersectionPoint = null;
            if (surfaceLine.ReferenceLineIntersectionWorldPoint != null)
            {
                Point2D oldIntersectionPoint = surfaceLine.ReferenceLineIntersectionWorldPoint;
                newIntersectionPoint = new Point2D(oldIntersectionPoint.X + random.NextDouble(),
                                                   oldIntersectionPoint.Y + random.NextDouble());
            }

            var copiedLine = new RingtoetsPipingSurfaceLine
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
            copiedLine.SetBottomDitchDikeSideAt(newGeometry[0]);
            copiedLine.SetBottomDitchPolderSideAt(newGeometry[1]);
            copiedLine.SetDikeToeAtPolderAt(newGeometry[2]);
            copiedLine.SetDikeToeAtRiverAt(newGeometry[3]);
            copiedLine.SetDitchDikeSideAt(newGeometry[4]);
            copiedLine.SetDitchPolderSideAt(newGeometry[5]);

            return copiedLine;
        }

        private static void AssertCharacteristicPoints(RingtoetsPipingSurfaceLine expectedSurfaceLine, RingtoetsPipingSurfaceLine actualSurfaceLine)
        {
            Assert.AreEqual(expectedSurfaceLine.BottomDitchDikeSide, actualSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(expectedSurfaceLine.BottomDitchPolderSide, actualSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(expectedSurfaceLine.DikeToeAtPolder, actualSurfaceLine.DikeToeAtPolder);
            Assert.AreEqual(expectedSurfaceLine.DikeToeAtRiver, actualSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(expectedSurfaceLine.DitchPolderSide, actualSurfaceLine.DitchPolderSide);
            Assert.AreEqual(expectedSurfaceLine.DitchDikeSide, actualSurfaceLine.DitchDikeSide);
        }
    }
}