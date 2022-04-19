﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test
{
    [TestFixture]
    public class PipingCalculationConfigurationHelperTest
    {
        #region GetPipingSoilProfilesForSurfaceLine

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_SurfaceLineIntersectingSoilModel_ReturnSoilModel()
        {
            // Setup
            var soilProfile1 = new PipingStochasticSoilProfile(
                0.3, new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D));
            var soilProfile2 = new PipingStochasticSoilProfile(
                0.7, new PipingSoilProfile("Profile 2", -8.0, new[]
                {
                    new PipingSoilLayer(-4.0),
                    new PipingSoilLayer(0.0),
                    new PipingSoilLayer(4.0)
                }, SoilProfileType.SoilProfile1D));

            var soilModel = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new PipingSurfaceLine("surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            // Call
            IEnumerable<PipingStochasticSoilModel> result = PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                surfaceLine,
                availableSoilModels);

            // Assert
            PipingStochasticSoilModel[] expected =
            {
                soilModel
            };
            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_NoSurfaceLine_ReturnEmpty()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D);

            var soilModel = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new PipingStochasticSoilProfile(0.3, soilProfile1),
                new PipingStochasticSoilProfile(0.7, soilProfile2)
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            // Call
            IEnumerable<PipingStochasticSoilModel> result = PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                null,
                availableSoilModels);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_NoSoilModels_ReturnEmpty()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine("surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            // Call
            IEnumerable<PipingStochasticSoilModel> result = PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                surfaceLine,
                Enumerable.Empty<PipingStochasticSoilModel>());

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_SoilModelGeometryNotIntersecting_ReturnEmpty()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D);

            var soilModel = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new PipingStochasticSoilProfile(0.3, soilProfile1),
                new PipingStochasticSoilProfile(0.7, soilProfile2)
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new PipingSurfaceLine("surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });

            // Call
            IEnumerable<PipingStochasticSoilModel> result = PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                surfaceLine, availableSoilModels);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_SurfaceLineOverlappingSoilModels_ReturnSoilModels()
        {
            // Setup
            const double y = 1.1;
            var soilProfile1 = new PipingStochasticSoilProfile(
                1.0, new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D)
            );
            var soilModel1 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            }, new[]
            {
                soilProfile1
            });

            var soilProfile2 = new PipingStochasticSoilProfile(
                1.0, new PipingSoilProfile("Profile 2", -8.0, new[]
                {
                    new PipingSoilLayer(-4.0),
                    new PipingSoilLayer(0.0),
                    new PipingSoilLayer(4.0)
                }, SoilProfileType.SoilProfile1D)
            );
            var soilModel2 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            }, new[]
            {
                soilProfile2
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel1,
                soilModel2
            };

            var surfaceLine = new PipingSurfaceLine("surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(5.0, y, 0.0),
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });

            // Call
            IEnumerable<PipingStochasticSoilModel> result = PipingCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                surfaceLine,
                availableSoilModels);

            // Assert
            PipingStochasticSoilModel[] expected =
            {
                soilModel1,
                soilModel2
            };
            CollectionAssert.AreEquivalent(expected, result);
        }

        #endregion

        #region GenerateCalculationItemsStructure

        [Test]
        public void GenerateCalculationItemsStructure_WithoutSurfaceLines_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                null, true, true, Enumerable.Empty<PipingStochasticSoilModel>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("surfaceLines", exception.ParamName);
        }

        [Test]
        public void GenerateCalculationItemsStructure_WithoutSoilModels_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                Enumerable.Empty<PipingSurfaceLine>(), true, true, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilModels", exception.ParamName);
        }

        [Test]
        public void GenerateCalculationItemsStructure_WithSurfaceLinesWithEmptySoilModels_LogFourWarnings()
        {
            // Setup
            const string testName1 = "group1";
            const string testName2 = "group2";
            const string testName3 = "group3";
            const string testName4 = "group4";

            var pipingSurfaceLines = new List<PipingSurfaceLine>
            {
                new PipingSurfaceLine(testName1),
                new PipingSurfaceLine(testName2),
                new PipingSurfaceLine(testName3),
                new PipingSurfaceLine(testName4)
            };
            IEnumerable<ICalculationBase> result = null;

            // Call
            void Call()
            {
                result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    pipingSurfaceLines, true, true, Enumerable.Empty<PipingStochasticSoilModel>()).ToArray();
            }

            // Assert
            const string format = "Geen ondergrondschematisaties gevonden voor profielschematisatie '{0}'. De profielschematisatie is overgeslagen.";
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create(string.Format(format, testName1), LogLevelConstant.Warn),
                Tuple.Create(string.Format(format, testName2), LogLevelConstant.Warn),
                Tuple.Create(string.Format(format, testName3), LogLevelConstant.Warn),
                Tuple.Create(string.Format(format, testName4), LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, expectedMessages);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SurfaceLineIntersectingSoilModelAndGenerateSemiProbabilisticAndProbabilisticFalse_ReturnEmptyCollection()
        {
            // Setup
            var soilProfile1 = new PipingStochasticSoilProfile(
                0.3, new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D)
            );
            var soilProfile2 = new PipingStochasticSoilProfile(0.7, new PipingSoilProfile("Profile 2", -8.0, new[]
                {
                    new PipingSoilLayer(-4.0),
                    new PipingSoilLayer(0.0),
                    new PipingSoilLayer(4.0)
                }, SoilProfileType.SoilProfile1D)
            );

            var soilModel = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new PipingSurfaceLine("surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            PipingSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines, false, false,
                availableSoilModels).ToArray();

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        [TestCaseSource(nameof(GenerateForCalculationTypes))]
        public void GenerateCalculationItemsStructure_SurfaceLineIntersectingSoilModel_ReturnOneGroupWithTwoCalculations(bool generateSemiProbabilistic, bool generateProbabilistic)
        {
            // Setup
            var soilProfile1 = new PipingStochasticSoilProfile(
                0.3, new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D)
            );
            var soilProfile2 = new PipingStochasticSoilProfile(0.7, new PipingSoilProfile("Profile 2", -8.0, new[]
                {
                    new PipingSoilLayer(-4.0),
                    new PipingSoilLayer(0.0),
                    new PipingSoilLayer(4.0)
                }, SoilProfileType.SoilProfile1D)
            );

            var soilModel = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new PipingSurfaceLine("surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            PipingSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines, generateSemiProbabilistic,
                generateProbabilistic, availableSoilModels).ToArray();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = (CalculationGroup) result.First();

            if (generateSemiProbabilistic)
            {
                SemiProbabilisticPipingCalculationScenario[] semiProbabilisticCalculationScenarios =
                    calculationGroup.Children.OfType<SemiProbabilisticPipingCalculationScenario>().ToArray();
                Assert.AreEqual(2, semiProbabilisticCalculationScenarios.Length);

                AssertCalculationScenario(semiProbabilisticCalculationScenarios[0], soilProfile1, surfaceLine);
                AssertCalculationScenario(semiProbabilisticCalculationScenarios[1], soilProfile2, surfaceLine);
            }

            if (generateProbabilistic)
            {
                ProbabilisticPipingCalculationScenario[] probabilisticCalculationScenarios =
                    calculationGroup.Children.OfType<ProbabilisticPipingCalculationScenario>().ToArray();
                Assert.AreEqual(2, probabilisticCalculationScenarios.Length);

                AssertCalculationScenario(probabilisticCalculationScenarios[0], soilProfile1, surfaceLine);
                AssertCalculationScenario(probabilisticCalculationScenarios[1], soilProfile2, surfaceLine);
            }
        }

        [Test]
        public void GenerateCalculationItemsStructure_SoilModelGeometryNotIntersecting_LogWarning()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D);

            var soilModel = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new PipingStochasticSoilProfile(0.3, soilProfile1),
                new PipingStochasticSoilProfile(0.7, soilProfile2)
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            const string testName = "testName";
            var surfaceLine = new PipingSurfaceLine(testName);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });

            PipingSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            IEnumerable<ICalculationBase> result = null;

            // Call
            void Call()
            {
                result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    surfaceLines, true, true, availableSoilModels).ToArray();
            }

            // Assert
            var expectedMessage = Tuple.Create(
                $"Geen ondergrondschematisaties gevonden voor profielschematisatie '{testName}'. De profielschematisatie is overgeslagen.",
                LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, expectedMessage);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        [TestCaseSource(nameof(GenerateForCalculationTypes))]
        public void GenerateCalculationItemsStructure_SurfaceLineOverlappingSoilModel_ReturnOneGroupWithProfilesFromBothSoilModels(
            bool generateSemiProbabilistic, bool generateProbabilistic)
        {
            // Setup
            const double y = 1.1;
            var soilProfile1 = new PipingStochasticSoilProfile(
                1.0, new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D)
            );
            var soilModel1 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            }, new[]
            {
                soilProfile1
            });

            var soilProfile2 = new PipingStochasticSoilProfile(
                1.0, new PipingSoilProfile("Profile 2", -8.0, new[]
                {
                    new PipingSoilLayer(-4.0),
                    new PipingSoilLayer(0.0),
                    new PipingSoilLayer(4.0)
                }, SoilProfileType.SoilProfile1D)
            );
            var soilModel2 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            }, new[]
            {
                soilProfile2
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel1,
                soilModel2
            };

            var surfaceLine = new PipingSurfaceLine("surface line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(5.0, y, 0.0),
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });

            PipingSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines, generateSemiProbabilistic, generateProbabilistic, availableSoilModels).ToArray();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = (CalculationGroup) result.First();

            if (generateSemiProbabilistic)
            {
                SemiProbabilisticPipingCalculationScenario[] semiProbabilisticCalculationScenarios =
                    calculationGroup.Children.OfType<SemiProbabilisticPipingCalculationScenario>().ToArray();
                Assert.AreEqual(2, semiProbabilisticCalculationScenarios.Length);

                AssertCalculationScenario(semiProbabilisticCalculationScenarios[0], soilProfile1, surfaceLine);
                AssertCalculationScenario(semiProbabilisticCalculationScenarios[1], soilProfile2, surfaceLine);
            }

            if (generateProbabilistic)
            {
                ProbabilisticPipingCalculationScenario[] probabilisticCalculationScenarios =
                    calculationGroup.Children.OfType<ProbabilisticPipingCalculationScenario>().ToArray();
                Assert.AreEqual(2, probabilisticCalculationScenarios.Length);

                AssertCalculationScenario(probabilisticCalculationScenarios[0], soilProfile1, surfaceLine);
                AssertCalculationScenario(probabilisticCalculationScenarios[1], soilProfile2, surfaceLine);
            }
        }

        [Test]
        [TestCaseSource(nameof(GenerateForCalculationTypes))]
        public void GenerateCalculationItemsStructure_SurfaceLinesEachIntersectingSoilModel_ReturnTwoGroupsWithProfilesFromIntersectingSoilModels(
            bool generateSemiProbabilistic, bool generateProbabilistic)
        {
            // Setup
            var soilProfile1 = new PipingStochasticSoilProfile(
                1.0, new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D)
            );
            var soilProfile2 = new PipingStochasticSoilProfile(
                1.0, new PipingSoilProfile("Profile 2", -8.0, new[]
                {
                    new PipingSoilLayer(-4.0),
                    new PipingSoilLayer(0.0),
                    new PipingSoilLayer(4.0)
                }, SoilProfileType.SoilProfile1D)
            );

            const double y = 1.1;
            var soilModel1 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            var soilModel2 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            }, new[]
            {
                soilProfile2
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel1,
                soilModel2
            };

            const string surfaceLineName1 = "surface line 1";
            const string surfaceLineName2 = "surface line 2";
            var surfaceLine1 = new PipingSurfaceLine(surfaceLineName1);
            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });
            var surfaceLine2 = new PipingSurfaceLine(surfaceLineName2);
            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(2.5, y, 1.0),
                new Point3D(5.0, y, 0.0)
            });

            PipingSurfaceLine[] surfaceLines =
            {
                surfaceLine1,
                surfaceLine2
            };

            // Call
            IEnumerable<ICalculationBase> result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines, generateSemiProbabilistic, generateProbabilistic, availableSoilModels).ToArray();

            // Assert
            Assert.AreEqual(2, result.Count());
            var calculationGroup1 = (CalculationGroup) result.First(g => g.Name == surfaceLineName1);
            var calculationGroup2 = (CalculationGroup) result.First(g => g.Name == surfaceLineName2);

            if (generateSemiProbabilistic)
            {
                SemiProbabilisticPipingCalculationScenario[] semiProbabilisticCalculationScenariosGroup1 =
                    calculationGroup1.Children.OfType<SemiProbabilisticPipingCalculationScenario>().ToArray();

                Assert.AreEqual(2, semiProbabilisticCalculationScenariosGroup1.Length);
                AssertCalculationScenario(semiProbabilisticCalculationScenariosGroup1[0], soilProfile1, surfaceLine1);
                AssertCalculationScenario(semiProbabilisticCalculationScenariosGroup1[1], soilProfile2, surfaceLine1);

                SemiProbabilisticPipingCalculationScenario[] semiProbabilisticCalculationScenariosGroup2 =
                    calculationGroup2.Children.OfType<SemiProbabilisticPipingCalculationScenario>().ToArray();

                Assert.AreEqual(1, semiProbabilisticCalculationScenariosGroup2.Length);
                AssertCalculationScenario(semiProbabilisticCalculationScenariosGroup2[0], soilProfile2, surfaceLine2);
            }

            if (generateProbabilistic)
            {
                ProbabilisticPipingCalculationScenario[] probabilisticCalculationScenariosGroup1 =
                    calculationGroup1.Children.OfType<ProbabilisticPipingCalculationScenario>().ToArray();

                Assert.AreEqual(2, probabilisticCalculationScenariosGroup1.Length);
                AssertCalculationScenario(probabilisticCalculationScenariosGroup1[0], soilProfile1, surfaceLine1);
                AssertCalculationScenario(probabilisticCalculationScenariosGroup1[1], soilProfile2, surfaceLine1);

                ProbabilisticPipingCalculationScenario[] probabilisticCalculationScenariosGroup2 =
                    calculationGroup2.Children.OfType<ProbabilisticPipingCalculationScenario>().ToArray();

                Assert.AreEqual(1, probabilisticCalculationScenariosGroup2.Length);
                AssertCalculationScenario(probabilisticCalculationScenariosGroup2[0], soilProfile2, surfaceLine2);
            }
        }

        [Test]
        public void GenerateCalculationItemsStructure_OneSurfaceLineIntersectingSoilModelOneSurfaceLineNoIntersection_ReturnOneGroupsWithProfilesAndLogOneWarning()
        {
            // Setup
            var soilProfile1 = new PipingStochasticSoilProfile(
                1.0, new PipingSoilProfile("Profile 1", -10.0, new[]
                {
                    new PipingSoilLayer(-5.0),
                    new PipingSoilLayer(-2.0),
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D)
            );
            var soilProfile2 = new PipingStochasticSoilProfile(
                1.0, new PipingSoilProfile("Profile 2", -8.0, new[]
                {
                    new PipingSoilLayer(-4.0),
                    new PipingSoilLayer(0.0),
                    new PipingSoilLayer(4.0)
                }, SoilProfileType.SoilProfile1D)
            );

            const double y = 1.1;
            var soilModel1 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            var soilModel2 = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            }, new[]
            {
                soilProfile2
            });
            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel1,
                soilModel2
            };

            const string surfaceLineName1 = "surface line 1";
            const string surfaceLineName2 = "surface line 2";
            var surfaceLine1 = new PipingSurfaceLine(surfaceLineName1);
            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });
            var surfaceLine2 = new PipingSurfaceLine(surfaceLineName2);
            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, y, 1.0),
                new Point3D(6.4, y, 0.0)
            });

            PipingSurfaceLine[] surfaceLines =
            {
                surfaceLine1,
                surfaceLine2
            };

            ICalculationBase[] result = null;

            // Call
            void Call()
            {
                result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    surfaceLines, true, true, availableSoilModels).ToArray();
            }

            // Assert
            var expectedMessage = Tuple.Create(
                $"Geen ondergrondschematisaties gevonden voor profielschematisatie '{surfaceLineName2}'. De profielschematisatie is overgeslagen.",
                LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, expectedMessage, 1);

            Assert.AreEqual(1, result.Length);
            var calculationGroup1 = (CalculationGroup) result.First(g => g.Name == surfaceLineName1);

            Assert.AreEqual(4, calculationGroup1.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup1.Children, typeof(IPipingCalculationScenario<PipingInput>));

            AssertCalculationScenario((SemiProbabilisticPipingCalculationScenario) calculationGroup1.Children[0], soilProfile1, surfaceLine1);
            AssertCalculationScenario((ProbabilisticPipingCalculationScenario) calculationGroup1.Children[1], soilProfile1, surfaceLine1);
            AssertCalculationScenario((SemiProbabilisticPipingCalculationScenario) calculationGroup1.Children[2], soilProfile2, surfaceLine1);
            AssertCalculationScenario((ProbabilisticPipingCalculationScenario) calculationGroup1.Children[3], soilProfile2, surfaceLine1);
        }

        [Test]
        public void GenerateCalculationItemsStructure_Always_CreateCalculationsWithSurfaceLineNameSoilProfileNameAndSemiProbabilisticInput()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D);
            var soilProfile2 = new PipingSoilProfile("Profile 2", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D);

            var soilModel = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new PipingStochasticSoilProfile(0.3, soilProfile1),
                new PipingStochasticSoilProfile(0.7, soilProfile2)
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new PipingSurfaceLine("Surface Line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            PipingSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines, true, true,
                availableSoilModels).ToArray();

            // Assert
            var group = (CalculationGroup) result.First(sl => sl.Name == surfaceLine.Name);

            var semiProbabilisticCalculationScenario1 = (SemiProbabilisticPipingCalculationScenario) group.Children[0];
            var probabilisticCalculationScenario1 = (ProbabilisticPipingCalculationScenario) group.Children[1];
            var semiProbabilisticCalculationScenario2 = (SemiProbabilisticPipingCalculationScenario) group.Children[2];
            var probabilisticCalculationScenario2 = (ProbabilisticPipingCalculationScenario) group.Children[3];

            Assert.AreEqual($"{surfaceLine.Name} {soilProfile1.Name}", semiProbabilisticCalculationScenario1.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile1.Name}", probabilisticCalculationScenario1.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile2.Name}", semiProbabilisticCalculationScenario2.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile2.Name}", probabilisticCalculationScenario2.Name);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SoilProfileEqualNames_CalculationScenariosGetUniqueName()
        {
            // Setup
            var soilProfile1 = new PipingSoilProfile("Profile 1", -10.0, new[]
            {
                new PipingSoilLayer(-5.0),
                new PipingSoilLayer(-2.0),
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D);
            var soilProfile2 = new PipingSoilProfile("Profile 1", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D);
            var soilProfile3 = new PipingSoilProfile("Profile 1", -8.0, new[]
            {
                new PipingSoilLayer(-4.0),
                new PipingSoilLayer(0.0),
                new PipingSoilLayer(4.0)
            }, SoilProfileType.SoilProfile1D);

            var soilModel = new PipingStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new PipingStochasticSoilProfile(0.3, soilProfile1),
                new PipingStochasticSoilProfile(0.2, soilProfile2),
                new PipingStochasticSoilProfile(0.5, soilProfile3)
            });

            PipingStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new PipingSurfaceLine("Surface Line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            PipingSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines, true, true, availableSoilModels).ToArray();

            // Assert
            var group = (CalculationGroup) result.First(sl => sl.Name == surfaceLine.Name);

            var semiProbabilisticPipingCalculationScenario1 = (SemiProbabilisticPipingCalculationScenario) group.Children[0];
            var probabilisticPipingCalculationScenario1 = (ProbabilisticPipingCalculationScenario) group.Children[1];
            var semiProbabilisticPipingCalculationScenario2 = (SemiProbabilisticPipingCalculationScenario) group.Children[2];
            var probabilisticPipingCalculationScenario2 = (ProbabilisticPipingCalculationScenario) group.Children[3];
            var semiProbabilisticPipingCalculationScenario3 = (SemiProbabilisticPipingCalculationScenario) group.Children[4];
            var probabilisticPipingCalculationScenario3 = (ProbabilisticPipingCalculationScenario) group.Children[5];

            Assert.AreEqual($"{surfaceLine.Name} {soilProfile1.Name}", semiProbabilisticPipingCalculationScenario1.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile1.Name}", probabilisticPipingCalculationScenario1.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile2.Name} (1)", semiProbabilisticPipingCalculationScenario2.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile2.Name} (1)", probabilisticPipingCalculationScenario2.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile3.Name} (2)", semiProbabilisticPipingCalculationScenario3.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile3.Name} (2)", probabilisticPipingCalculationScenario3.Name);
        }

        private static IEnumerable<TestCaseData> GenerateForCalculationTypes
        {
            get
            {
                yield return new TestCaseData(true, true);
                yield return new TestCaseData(true, false);
                yield return new TestCaseData(false, true);
            }
        }

        private static void AssertCalculationScenario(IPipingCalculationScenario<PipingInput> calculationScenario,
                                                      PipingStochasticSoilProfile stochasticSoilProfile, PipingSurfaceLine surfaceLine)
        {
            PipingInput input = calculationScenario.InputParameters;
            Assert.AreSame(stochasticSoilProfile, input.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, input.SurfaceLine);
            Assert.AreEqual((RoundedDouble) stochasticSoilProfile.Probability, calculationScenario.Contribution);
        }

        #endregion
    }
}