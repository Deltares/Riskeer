// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.UpdateDataStrategies;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Importers;
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
            TestDelegate call = () => new RingtoetsPipingSurfaceLineUpdateDataStrategy(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("failureMechanism", paramName);
        }

        [Test]
        public void Constructor_CreatesNewInstance()
        {
            // Call
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<ISurfaceLineUpdateDataStrategy>(strategy);
            Assert.IsInstanceOf<UpdateDataStrategyBase<RingtoetsPipingSurfaceLine, PipingFailureMechanism>>(strategy);
        }

        [Test]
        public void Constructor_TargetCollectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(null, Enumerable.Empty<RingtoetsPipingSurfaceLine>(), string.Empty);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("targetDataCollection", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new RingtoetsPipingSurfaceLineCollection(),
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
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate test = () => strategy.UpdateSurfaceLinesWithImportedData(new RingtoetsPipingSurfaceLineCollection(),
                                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                  null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sourceFilePath", paramName);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_CurrentCollectionAndImportedCollectionEmpty_DoesNothing()
        {
            // Setup
            var targetCollection = new RingtoetsPipingSurfaceLineCollection();
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.IsEmpty(affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithoutCurrentSurfaceLinesAndReadLinesHaveDuplicateNames_ThrowsUpdateException()
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
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                  importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<RingtoetsPipingSurfaceLineUpdateException>(call);
            string message = $"Profielschematisaties moeten een unieke naam hebben. Gevonden dubbele elementen: {duplicateName}.";
            Assert.AreEqual(message, exception.Message);
            Assert.IsInstanceOf<ArgumentException>(exception.InnerException);

            CollectionAssert.IsEmpty(targetCollection);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_CurrentCollectionEmptyAndImportedDataNotEmpty_NewSurfaceLinesAdded()
        {
            // Setup
            const string collectionSurfaceLineOneName = "Name A";
            const string collectionSurfaceLineTwoName = "Name B";

            var importedSurfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = collectionSurfaceLineOneName
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = collectionSurfaceLineTwoName
                }
            };

            var targetCollection = new RingtoetsPipingSurfaceLineCollection();
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   importedSurfaceLines,
                                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.AreEqual(importedSurfaceLines, targetCollection);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentLinesAndImportedMultipleLinesWithSameNames_ThrowsUpdateException()
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

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            TestDelegate call = () => strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                  importedSurfaceLines,
                                                                                  sourceFilePath);

            // Assert
            var exception = Assert.Throws<RingtoetsPipingSurfaceLineUpdateException>(call);
            Assert.AreEqual("Het bijwerken van de profielschematisaties is mislukt.", exception.Message);
            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);

            CollectionAssert.AreEqual(expectedCollection, targetCollection);
            RingtoetsPipingSurfaceLine actualSurfaceLine = targetCollection[0];
            Assert.AreEqual(expectedSurfaceLine.Name, actualSurfaceLine.Name);
            CollectionAssert.AreEqual(expectedGeometry, actualSurfaceLine.Points);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_WithCurrentLinesAndImportedDataEmpty_SurfaceLinesRemoved()
        {
            // Setup
            var targetCollection = new RingtoetsPipingSurfaceLineCollection();
            const string collectionSurfaceLineName = "Name A";
            targetCollection.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = collectionSurfaceLineName
                }
            }, sourceFilePath);

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                   sourceFilePath);

            // Assert
            CollectionAssert.IsEmpty(targetCollection);
            CollectionAssert.AreEqual(new[]
            {
                targetCollection
            }, affectedObjects);
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
            var targetCollection = new RingtoetsPipingSurfaceLineCollection();
            targetCollection.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);

            Point3D[] expectedGeometry =
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5),
                new Point3D(6, 7, 8)
            };
            RingtoetsPipingSurfaceLine readSurfaceLine = DeepCloneSurfaceLine(targetSurfaceLine);
            readSurfaceLine.SetGeometry(expectedGeometry);
            var readSurfacelines = new[]
            {
                readSurfaceLine
            };

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(new PipingFailureMechanism());

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(targetCollection,
                                                                                                   readSurfacelines,
                                                                                                   sourceFilePath);

            // Assert
            Assert.AreEqual(1, targetCollection.Count);
            Assert.AreSame(targetSurfaceLine, targetCollection[0]);
            CollectionAssert.AreEqual(expectedGeometry, targetSurfaceLine.Points);
            CollectionAssert.AreEqual(new[]
            {
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
            var readSurfacelines = new[]
            {
                readSurfaceLine
            };

            var failureMechanism = new PipingFailureMechanism();
            RingtoetsPipingSurfaceLineCollection surfaceLineCollection = failureMechanism.SurfaceLines;
            surfaceLineCollection.AddRange(new[]
            {
                targetSurfaceLine
            }, sourceFilePath);
            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(failureMechanism);

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

            RingtoetsPipingSurfaceLine readSurfaceLineOne = DeepCloneSurfaceLine(surfaceLineOne);
            var readSurfaceLineTwo = new RingtoetsPipingSurfaceLine
            {
                Name = addedSurfaceLineName
            };
            var readSurfacelines = new[]
            {
                readSurfaceLineOne,
                readSurfaceLineTwo
            };

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(failureMechanism);

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
            Assert.AreSame(surfaceLineOne, surfaceLineCollection[0]);
            Assert.AreSame(readSurfaceLineTwo, surfaceLineCollection[1]);
            CollectionAssert.AreEqual(new[]
            {
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

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(failureMechanism);

            var collection = new RingtoetsPipingSurfaceLineCollection();
            collection.AddRange(new[]
            {
                surfaceLine
            }, sourceFilePath);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(collection,
                                                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                   sourceFilePath).ToArray();

            // Assert
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.InputParameters.SurfaceLine);
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                collection,
                calculation,
                calculation.InputParameters
            }, affectedObjects);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_MultipleCalculationsWithSurfaceLines_OnlyUpdatesCalculationWithUpdatedSurfaceLine()
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

            var collection = new RingtoetsPipingSurfaceLineCollection();
            collection.AddRange(new[]
            {
                affectedSurfaceLine,
                unaffectedSurfaceLine
            }, sourceFilePath);

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unAffectedCalculation);

            RingtoetsPipingSurfaceLine importedAffectedSurfaceLine = DeepCloneSurfaceLine(affectedSurfaceLine);
            importedAffectedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });
            RingtoetsPipingSurfaceLine importedUnaffectedSurfaceLine = DeepCloneSurfaceLine(unaffectedSurfaceLine);
            importedUnaffectedSurfaceLine.SetGeometry(unaffectedGeometry);

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(collection,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedAffectedSurfaceLine,
                                                                                                       importedUnaffectedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            Assert.IsTrue(unAffectedCalculation.HasOutput);
            PipingInput unaffectedInput = unAffectedCalculation.InputParameters;
            Assert.AreSame(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);
            CollectionAssert.AreEqual(unaffectedGeometry, unaffectedSurfaceLine.Points);

            Assert.IsFalse(affectedCalculation.HasOutput);
            PipingInput affectedInput = affectedCalculation.InputParameters;
            Assert.AreSame(affectedSurfaceLine, affectedInput.SurfaceLine);
            CollectionAssert.AreEqual(importedAffectedSurfaceLine.Points, affectedSurfaceLine.Points);

            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedCalculation,
                affectedInput,
                affectedSurfaceLine
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

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            RingtoetsPipingSurfaceLine importedSurfaceLine = DeepCloneSurfaceLine(surfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            PipingInput calculationInput = calculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                surfaceLine,
                calculationInput
            }, affectedObjects);
            Assert.AreSame(surfaceLine, calculationInput.SurfaceLine);
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

            RingtoetsPipingSurfaceLine importedSurfaceLine = DeepCloneSurfaceLine(surfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            PipingInput calculationInput = calculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                surfaceLine,
                calculationInput
            }, affectedObjects);
            Assert.AreSame(surfaceLine, calculationInput.SurfaceLine);
            CollectionAssert.AreEqual(importedSurfaceLine.Points, surfaceLine.Points);
            Assert.IsNull(calculationInput.StochasticSoilModel);
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

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(unAffectedCalculation);

            failureMechanism.SurfaceLines.AddRange(new[]
            {
                affectedSurfaceLine
            }, "path");
            failureMechanism.StochasticSoilModels.AddRange(soilModels, "path");

            RingtoetsPipingSurfaceLine importedSurfaceLine = DeepCloneSurfaceLine(affectedSurfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            PipingInput affectedInput = affectedCalculation.InputParameters;
            CollectionAssert.AreEquivalent(new IObservable[]
            {
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
                }
            };

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);
            failureMechanism.CalculationsGroup.Children.Add(affectedCalculation);

            failureMechanism.SurfaceLines.AddRange(new[]
            {
                affectedSurfaceLine
            }, "path");

            var importedSurfaceLine = DeepCloneSurfaceLine(affectedSurfaceLine);
            importedSurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(10, 0, 0)
            });

            var strategy = new RingtoetsPipingSurfaceLineUpdateDataStrategy(failureMechanism);

            // Call
            IEnumerable<IObservable> affectedObjects = strategy.UpdateSurfaceLinesWithImportedData(failureMechanism.SurfaceLines,
                                                                                                   new[]
                                                                                                   {
                                                                                                       importedSurfaceLine
                                                                                                   }, "path").ToArray();

            // Assert
            CollectionAssert.AreEquivalent(new IObservable[]
            {
                affectedSurfaceLine,
                affectedCalculation.InputParameters
            }, affectedObjects);
        }

        private static RingtoetsPipingSurfaceLine CreateValidSurfaceLineForCalculations()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1, 2, 3),
                new Point3D(4, 5, 6)
            });
            return surfaceLine;
        }

        private static RingtoetsPipingSurfaceLine DeepCloneSurfaceLine(RingtoetsPipingSurfaceLine surfaceLine)
        {
            var copiedLine = new RingtoetsPipingSurfaceLine
            {
                Name = surfaceLine.Name
            };
            copiedLine.SetGeometry(surfaceLine.Points);

            return copiedLine;
        }
    }
}