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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationHelperTest
    {
        #region GetMacroStabilityInwardsSoilProfilesForSurfaceLine

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_SurfaceLineIntersectingSoilModel_ReturnSoilModel()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsStochasticSoilProfile(0.3, new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            }));
            var soilProfile2 = new MacroStabilityInwardsStochasticSoilProfile(0.7, new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            }));

            var soilModel = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> result = MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                surfaceLine,
                availableSoilModels);

            // Assert
            MacroStabilityInwardsStochasticSoilModel[] expected =
            {
                soilModel
            };
            CollectionAssert.AreEquivalent(expected, result);
        }

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_NoSurfaceLine_ReturnEmpty()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            });
            var soilProfile2 = new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            });

            var soilModel = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.3, soilProfile1),
                new MacroStabilityInwardsStochasticSoilProfile(0.7, soilProfile2)
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> result = MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                null,
                availableSoilModels);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_NoSoilModels_ReturnEmpty()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> result = MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                surfaceLine,
                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>());

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_SoilModelGeometryNotIntersecting_ReturnEmpty()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            });
            var soilProfile2 = new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            });

            var soilModel = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.3, soilProfile1),
                new MacroStabilityInwardsStochasticSoilProfile(0.7, soilProfile2)
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> result = MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(surfaceLine, availableSoilModels);

            // Assert
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetStochasticSoilModelsForSurfaceLine_SurfaceLineOverlappingSoilModels_ReturnSoilModels()
        {
            // Setup
            const double y = 1.1;

            var soilProfile1 = new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            }));
            var soilModel1 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            }, new[]
            {
                soilProfile1
            });

            var soilProfile2 = new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            }));
            var soilModel2 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            }, new[]
            {
                soilProfile2
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel1,
                soilModel2
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(5.0, y, 0.0),
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });

            // Call
            IEnumerable<MacroStabilityInwardsStochasticSoilModel> result = MacroStabilityInwardsCalculationConfigurationHelper.GetStochasticSoilModelsForSurfaceLine(
                surfaceLine,
                availableSoilModels);

            // Assert
            MacroStabilityInwardsStochasticSoilModel[] expected =
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
            TestDelegate test = () => MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                null,
                Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("surfaceLines", parameter);
        }

        [Test]
        public void GenerateCalculationItemsStructure_WithoutSoilModels_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                Enumerable.Empty<MacroStabilityInwardsSurfaceLine>(),
                null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("soilModels", parameter);
        }

        [Test]
        public void GenerateCalculationItemsStructure_WithSurfaceLinesWithEmptySoilModels_LogFourWarnings()
        {
            // Setup
            const string testName1 = "group1";
            const string testName2 = "group2";
            const string testName3 = "group3";
            const string testName4 = "group4";

            var surfaceLines = new List<MacroStabilityInwardsSurfaceLine>
            {
                new MacroStabilityInwardsSurfaceLine(testName1),
                new MacroStabilityInwardsSurfaceLine(testName2),
                new MacroStabilityInwardsSurfaceLine(testName3),
                new MacroStabilityInwardsSurfaceLine(testName4)
            };
            IEnumerable<ICalculationBase> result = null;

            // Call
            Action call = () =>
            {
                result = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    surfaceLines,
                    Enumerable.Empty<MacroStabilityInwardsStochasticSoilModel>()).ToArray();
            };

            // Assert
            const string format = "Geen ondergrondschematisaties gevonden voor profielschematisatie '{0}'. De profielschematisatie is overgeslagen.";
            Tuple<string, LogLevelConstant>[] expectedMessages =
            {
                Tuple.Create(string.Format(format, testName1), LogLevelConstant.Warn),
                Tuple.Create(string.Format(format, testName2), LogLevelConstant.Warn),
                Tuple.Create(string.Format(format, testName3), LogLevelConstant.Warn),
                Tuple.Create(string.Format(format, testName4), LogLevelConstant.Warn)
            };
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, expectedMessages);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SurfaceLineIntersectingSoilModel_ReturnOneGroupWithTwoCalculations()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsStochasticSoilProfile(0.3, new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            }));
            var soilProfile2 = new MacroStabilityInwardsStochasticSoilProfile(0.7, new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            }));

            var soilModel = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            MacroStabilityInwardsSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines,
                availableSoilModels).ToArray();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as CalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.AreEqual(2, calculationGroup.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup.Children, typeof(MacroStabilityInwardsCalculationScenario));

            var calculationScenario1 = (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile1.Probability, calculationScenario1.Contribution);

            MacroStabilityInwardsInput calculationInput1 = calculationScenario1.InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, calculationInput1.SurfaceLine);

            var calculationScenario2 = (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[1];
            Assert.AreEqual((RoundedDouble) soilProfile2.Probability, calculationScenario2.Contribution);

            MacroStabilityInwardsInput calculationInput2 = calculationScenario2.InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, calculationInput2.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SoilModelGeometryNotIntersecting_LogWarning()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            });
            var soilProfile2 = new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            });

            var soilModel = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.3, soilProfile1),
                new MacroStabilityInwardsStochasticSoilProfile(0.7, soilProfile2)
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            const string testName = "testName";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(testName);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });

            MacroStabilityInwardsSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            IEnumerable<ICalculationBase> result = null;

            // Call
            Action call = () =>
            {
                result = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    surfaceLines,
                    availableSoilModels).ToArray();
            };

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Geen ondergrondschematisaties gevonden voor profielschematisatie '{testName}'. De profielschematisatie is overgeslagen.",
                LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage);
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SurfaceLineOverlappingSoilModel_ReturnOneGroupWithProfilesFromBothSoilModels()
        {
            // Setup
            const double y = 1.1;

            var soilProfile1 = new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            }));
            var soilModel1 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            }, new[]
            {
                soilProfile1
            });

            var soilProfile2 = new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            }));
            var soilModel2 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            }, new[]
            {
                soilProfile2
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel1,
                soilModel2
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(5.0, y, 0.0),
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });

            MacroStabilityInwardsSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines,
                availableSoilModels).ToArray();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as CalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.AreEqual(2, calculationGroup.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup.Children, typeof(MacroStabilityInwardsCalculationScenario));

            var calculationScenario1 = (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile1.Probability, calculationScenario1.Contribution);

            MacroStabilityInwardsInput calculationInput1 = calculationScenario1.InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, calculationInput1.SurfaceLine);

            var calculationScenario2 = (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[1];
            Assert.AreEqual((RoundedDouble) soilProfile2.Probability, calculationScenario2.Contribution);

            MacroStabilityInwardsInput calculationInput2 = calculationScenario2.InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, calculationInput2.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SurfaceLinesEachIntersectingSoilModel_ReturnTwoGroupsWithProfilesFromIntersectingSoilModels()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            }));
            var soilProfile2 = new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            }));

            const double y = 1.1;
            var soilModel1 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            var soilModel2 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            }, new[]
            {
                soilProfile2
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel1,
                soilModel2
            };

            const string surfaceLineName1 = "surface line 1";
            const string surfaceLineName2 = "surface line 2";
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine(surfaceLineName1);
            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });
            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine(surfaceLineName2);
            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(2.5, y, 1.0),
                new Point3D(5.0, y, 0.0)
            });

            MacroStabilityInwardsSurfaceLine[] surfaceLines =
            {
                surfaceLine1,
                surfaceLine2
            };

            // Call
            IEnumerable<ICalculationBase> result = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines,
                availableSoilModels).ToArray();

            // Assert
            Assert.AreEqual(2, result.Count());
            var calculationGroup1 = result.First(g => g.Name == surfaceLineName1) as CalculationGroup;
            Assert.NotNull(calculationGroup1);

            Assert.AreEqual(2, calculationGroup1.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup1.Children, typeof(MacroStabilityInwardsCalculationScenario));

            var calculationScenario1 = (MacroStabilityInwardsCalculationScenario) calculationGroup1.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile1.Probability, calculationScenario1.Contribution);

            MacroStabilityInwardsInput calculationInput1 = calculationScenario1.InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.StochasticSoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput1.SurfaceLine);

            var calculationScenario2 = (MacroStabilityInwardsCalculationScenario) calculationGroup1.Children[1];
            Assert.AreEqual((RoundedDouble) soilProfile2.Probability, calculationScenario1.Contribution);

            MacroStabilityInwardsInput calculationInput2 = calculationScenario2.InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.StochasticSoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput2.SurfaceLine);

            var calculationGroup2 = result.First(g => g.Name == surfaceLineName2) as CalculationGroup;
            Assert.NotNull(calculationGroup2);

            Assert.AreEqual(1, calculationGroup2.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup2.Children, typeof(MacroStabilityInwardsCalculationScenario));

            var calculationScenario3 = (MacroStabilityInwardsCalculationScenario) calculationGroup2.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile2.Probability, calculationScenario1.Contribution);

            MacroStabilityInwardsInput calculationInput3 = calculationScenario3.InputParameters;
            Assert.AreSame(soilProfile2, calculationInput3.StochasticSoilProfile);
            Assert.AreSame(surfaceLine2, calculationInput3.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationItemsStructure_OneSurfaceLineIntersectingSoilModelOneSurfaceLineNoIntersection_ReturnOneGroupsWithProfilesAndLogOneWarning()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            }));
            var soilProfile2 = new MacroStabilityInwardsStochasticSoilProfile(1.0, new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            }));

            const double y = 1.1;
            var soilModel1 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, y),
                new Point2D(2.0, y)
            }, new[]
            {
                soilProfile1,
                soilProfile2
            });

            var soilModel2 = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(3.0, y),
                new Point2D(4.0, y)
            }, new[]
            {
                soilProfile2
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel1,
                soilModel2
            };

            const string surfaceLineName1 = "surface line 1";
            const string surfaceLineName2 = "surface line 2";
            var surfaceLine1 = new MacroStabilityInwardsSurfaceLine(surfaceLineName1);
            surfaceLine1.SetGeometry(new[]
            {
                new Point3D(2.5, y, 1.0),
                new Point3D(0.0, y, 0.0)
            });
            var surfaceLine2 = new MacroStabilityInwardsSurfaceLine(surfaceLineName2);
            surfaceLine2.SetGeometry(new[]
            {
                new Point3D(5.0, y, 1.0),
                new Point3D(6.4, y, 0.0)
            });

            MacroStabilityInwardsSurfaceLine[] surfaceLines =
            {
                surfaceLine1,
                surfaceLine2
            };

            ICalculationBase[] result = null;

            // Call
            Action call = () =>
            {
                result = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    surfaceLines,
                    availableSoilModels).ToArray();
            };

            // Assert
            Tuple<string, LogLevelConstant> expectedMessage = Tuple.Create(
                $"Geen ondergrondschematisaties gevonden voor profielschematisatie '{surfaceLineName2}'. De profielschematisatie is overgeslagen.",
                LogLevelConstant.Warn);
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, expectedMessage, 1);

            Assert.AreEqual(1, result.Length);
            var calculationGroup1 = result.First(g => g.Name == surfaceLineName1) as CalculationGroup;
            Assert.NotNull(calculationGroup1);

            Assert.AreEqual(2, calculationGroup1.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup1.Children, typeof(MacroStabilityInwardsCalculationScenario));

            var calculationScenario1 = (MacroStabilityInwardsCalculationScenario) calculationGroup1.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile1.Probability, calculationScenario1.Contribution);

            MacroStabilityInwardsInput calculationInput1 = calculationScenario1.InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.StochasticSoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput1.SurfaceLine);

            MacroStabilityInwardsInput calculationInput2 = ((MacroStabilityInwardsCalculationScenario) calculationGroup1.Children[1]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.StochasticSoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput2.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationItemsStructure_Always_CreateCalculationsWithSurfaceLineNameSoilProfileNameGeneralInputAndSemiProbabilisticInput()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            });
            var soilProfile2 = new MacroStabilityInwardsSoilProfile1D("Profile 2", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            });

            var soilModel = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.3, soilProfile1),
                new MacroStabilityInwardsStochasticSoilProfile(0.7, soilProfile2)
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Surface Line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            MacroStabilityInwardsSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines,
                availableSoilModels).ToArray();

            // Assert
            var group = result.First(sl => sl.Name == surfaceLine.Name) as CalculationGroup;
            Assert.NotNull(group);
            var calculationInput1 = (MacroStabilityInwardsCalculationScenario) group.Children[0];
            var calculationInput2 = (MacroStabilityInwardsCalculationScenario) group.Children[1];

            Assert.AreEqual($"{surfaceLine.Name} {soilProfile1.Name}", calculationInput1.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile2.Name}", calculationInput2.Name);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SoilProfileEqualNames_CalculationsGetUniqueName()
        {
            // Setup
            var soilProfile1 = new MacroStabilityInwardsSoilProfile1D("Profile 1", -10.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-5.0),
                new MacroStabilityInwardsSoilLayer1D(-2.0),
                new MacroStabilityInwardsSoilLayer1D(1.0)
            });
            var soilProfile2 = new MacroStabilityInwardsSoilProfile1D("Profile 1", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            });
            var soilProfile3 = new MacroStabilityInwardsSoilProfile1D("Profile 1", -8.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(-4.0),
                new MacroStabilityInwardsSoilLayer1D(0.0),
                new MacroStabilityInwardsSoilLayer1D(4.0)
            });

            var soilModel = new MacroStabilityInwardsStochasticSoilModel("A", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                new MacroStabilityInwardsStochasticSoilProfile(0.3, soilProfile1),
                new MacroStabilityInwardsStochasticSoilProfile(0.2, soilProfile2),
                new MacroStabilityInwardsStochasticSoilProfile(0.5, soilProfile3)
            });

            MacroStabilityInwardsStochasticSoilModel[] availableSoilModels =
            {
                soilModel
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Surface Line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            MacroStabilityInwardsSurfaceLine[] surfaceLines =
            {
                surfaceLine
            };

            // Call
            IEnumerable<ICalculationBase> result = MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines,
                availableSoilModels).ToArray();

            // Assert
            var group = result.First(sl => sl.Name == surfaceLine.Name) as CalculationGroup;
            Assert.NotNull(group);
            var calculationInput1 = (MacroStabilityInwardsCalculationScenario) group.Children[0];
            var calculationInput2 = (MacroStabilityInwardsCalculationScenario) group.Children[1];
            var calculationInput3 = (MacroStabilityInwardsCalculationScenario) group.Children[2];

            Assert.AreEqual($"{surfaceLine.Name} {soilProfile1.Name}", calculationInput1.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile2.Name} (1)", calculationInput2.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile3.Name} (2)", calculationInput3.Name);
        }

        #endregion
    }
}