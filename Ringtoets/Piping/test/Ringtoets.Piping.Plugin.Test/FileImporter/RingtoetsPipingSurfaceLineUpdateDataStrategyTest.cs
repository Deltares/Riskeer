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
using Ringtoets.Piping.IO.Importers;
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
            Assert.IsInstanceOf<UpdateDataStrategyBase<RingtoetsPipingSurfaceLine, string, PipingFailureMechanism>>(strategy);
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
        public void UpdateSurfaceLinesWithImportedData_ReadSurfaceLinesNotInTargetCollection_NewSurfaceLinesAdded()
        {
            // Setup
            var importedSurfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line A"
                },
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Line B"
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
            targetCollection.AddRange(new[]
            {
                new RingtoetsPipingSurfaceLine
                {
                    Name = "Name A"
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
        public void UpdateSurfaceLinesWithImportedData_WithCurrentLinesAndImportedDataHasSameName_UpdatesTargetCollection()
        {
            // Setup
            var targetSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
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
            var readSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
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

            var importedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
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
            CollectionAssert.Contains(affectedObjects, calculation);
            CollectionAssert.Contains(affectedObjects, calculationInput);
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

            var importedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
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
            CollectionAssert.Contains(affectedObjects, calculation);
            CollectionAssert.Contains(affectedObjects, calculationInput);
            Assert.AreSame(surfaceLine, calculationInput.SurfaceLine);
            CollectionAssert.AreEqual(importedSurfaceLine.Points, surfaceLine.Points);
            Assert.IsNull(calculationInput.StochasticSoilModel);
        }

        [Test]
        public void UpdateSurfaceLinesWithImportedData_MultipleCalculations_OnlyUpdatesCalculationWithUpdatedSurfaceLine()
        {
            // Setup
            var affectedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
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

            var importedSurfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
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
            PipingInput unaffectedInput = unAffectedCalculation.InputParameters;
            CollectionAssert.DoesNotContain(affectedObjects, unAffectedCalculation);
            CollectionAssert.DoesNotContain(affectedObjects, unaffectedInput);
            Assert.AreSame(unaffectedSurfaceLine, unaffectedInput.SurfaceLine);
            CollectionAssert.AreEqual(unaffectedGeometry, unaffectedSurfaceLine.Points);
            Assert.IsNull(unaffectedInput.StochasticSoilModel);

            PipingInput affectedInput = affectedCalculation.InputParameters;
            CollectionAssert.Contains(affectedObjects, affectedCalculation);
            CollectionAssert.Contains(affectedObjects, affectedInput);
            Assert.AreSame(affectedSurfaceLine, affectedInput.SurfaceLine);
            CollectionAssert.AreEqual(importedSurfaceLine.Points, affectedSurfaceLine.Points);
            Assert.AreEqual(soilModels[0], affectedInput.StochasticSoilModel);
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
    }
}