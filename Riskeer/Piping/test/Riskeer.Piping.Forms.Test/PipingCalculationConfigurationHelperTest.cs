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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.Test
{
    [TestFixture]
    public class PipingCalculationConfigurationHelperTest
    {
        private static void CompareGeneralInputToInput(GeneralPipingInput generalInput, PipingCalculationScenario calculationInput)
        {
            Assert.AreEqual(generalInput.BeddingAngle, calculationInput.InputParameters.BeddingAngle);
            Assert.AreEqual(generalInput.CriticalHeaveGradient, calculationInput.InputParameters.CriticalHeaveGradient);
            Assert.AreEqual(generalInput.Gravity, calculationInput.InputParameters.Gravity);
            Assert.AreEqual(generalInput.MeanDiameter70, calculationInput.InputParameters.MeanDiameter70);
            Assert.AreEqual(generalInput.SandParticlesVolumicWeight.Value, calculationInput.InputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(generalInput.SellmeijerModelFactor, calculationInput.InputParameters.SellmeijerModelFactor);
            Assert.AreEqual(generalInput.SellmeijerReductionFactor, calculationInput.InputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(generalInput.UpliftModelFactor, calculationInput.InputParameters.UpliftModelFactor);
            Assert.AreEqual(generalInput.WaterKinematicViscosity, calculationInput.InputParameters.WaterKinematicViscosity);
            Assert.AreEqual(generalInput.WaterVolumetricWeight.Value, calculationInput.InputParameters.WaterVolumetricWeight);
            Assert.AreEqual(generalInput.WhitesDragCoefficient, calculationInput.InputParameters.WhitesDragCoefficient);
        }

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
            TestDelegate test = () => PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                null,
                Enumerable.Empty<PipingStochasticSoilModel>(),
                new GeneralPipingInput());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("surfaceLines", parameter);
        }

        [Test]
        public void GenerateCalculationItemsStructure_WithoutSoilModels_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                Enumerable.Empty<PipingSurfaceLine>(),
                null,
                new GeneralPipingInput());

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("soilModels", parameter);
        }

        [Test]
        public void GenerateCalculationItemsStructure_WithoutGeneralInput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                Enumerable.Empty<PipingSurfaceLine>(),
                Enumerable.Empty<PipingStochasticSoilModel>(),
                null);

            // Assert
            string parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("generalInput", parameter);
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
            Action call = () =>
            {
                result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    pipingSurfaceLines,
                    Enumerable.Empty<PipingStochasticSoilModel>(),
                    new GeneralPipingInput()).ToArray();
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
                surfaceLines,
                availableSoilModels,
                new GeneralPipingInput()).ToArray();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as CalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.AreEqual(2, calculationGroup.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup.Children, typeof(PipingCalculationScenario));

            var pipingCalculationScenario1 = (PipingCalculationScenario) calculationGroup.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile1.Probability, pipingCalculationScenario1.Contribution);

            PipingInput calculationInput1 = pipingCalculationScenario1.InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, calculationInput1.SurfaceLine);

            var pipingCalculationScenario2 = (PipingCalculationScenario) calculationGroup.Children[1];
            Assert.AreEqual((RoundedDouble) soilProfile2.Probability, pipingCalculationScenario2.Contribution);

            PipingInput calculationInput2 = pipingCalculationScenario2.InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, calculationInput2.SurfaceLine);
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
            Action call = () =>
            {
                result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    surfaceLines,
                    availableSoilModels,
                    new GeneralPipingInput()).ToArray();
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
                surfaceLines,
                availableSoilModels,
                new GeneralPipingInput()).ToArray();

            // Assert
            Assert.AreEqual(1, result.Count());
            var calculationGroup = result.First() as CalculationGroup;
            Assert.NotNull(calculationGroup);

            Assert.AreEqual(2, calculationGroup.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup.Children, typeof(PipingCalculationScenario));

            var pipingCalculationScenario1 = (PipingCalculationScenario) calculationGroup.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile1.Probability, pipingCalculationScenario1.Contribution);

            PipingInput calculationInput1 = pipingCalculationScenario1.InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, calculationInput1.SurfaceLine);

            var pipingCalculationScenario2 = (PipingCalculationScenario) calculationGroup.Children[1];
            Assert.AreEqual((RoundedDouble) soilProfile2.Probability, pipingCalculationScenario2.Contribution);

            PipingInput calculationInput2 = pipingCalculationScenario2.InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.StochasticSoilProfile);
            Assert.AreSame(surfaceLine, calculationInput2.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SurfaceLinesEachIntersectingSoilModel_ReturnTwoGroupsWithProfilesFromIntersectingSoilModels()
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
                surfaceLines,
                availableSoilModels,
                new GeneralPipingInput()).ToArray();

            // Assert
            Assert.AreEqual(2, result.Count());
            var calculationGroup1 = result.First(g => g.Name == surfaceLineName1) as CalculationGroup;
            Assert.NotNull(calculationGroup1);

            Assert.AreEqual(2, calculationGroup1.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup1.Children, typeof(PipingCalculationScenario));

            var pipingCalculationScenario1 = (PipingCalculationScenario) calculationGroup1.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile1.Probability, pipingCalculationScenario1.Contribution);

            PipingInput calculationInput1 = pipingCalculationScenario1.InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.StochasticSoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput1.SurfaceLine);

            var pipingCalculationScenario2 = (PipingCalculationScenario) calculationGroup1.Children[1];
            Assert.AreEqual((RoundedDouble) soilProfile2.Probability, pipingCalculationScenario1.Contribution);

            PipingInput calculationInput2 = pipingCalculationScenario2.InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.StochasticSoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput2.SurfaceLine);

            var calculationGroup2 = result.First(g => g.Name == surfaceLineName2) as CalculationGroup;
            Assert.NotNull(calculationGroup2);

            Assert.AreEqual(1, calculationGroup2.Children.Count);
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup2.Children, typeof(PipingCalculationScenario));

            var pipingCalculationScenario3 = (PipingCalculationScenario) calculationGroup2.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile2.Probability, pipingCalculationScenario1.Contribution);

            PipingInput calculationInput3 = pipingCalculationScenario3.InputParameters;
            Assert.AreSame(soilProfile2, calculationInput3.StochasticSoilProfile);
            Assert.AreSame(surfaceLine2, calculationInput3.SurfaceLine);
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
            Action call = () =>
            {
                result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                    surfaceLines,
                    availableSoilModels,
                    new GeneralPipingInput()).ToArray();
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
            CollectionAssert.AllItemsAreInstancesOfType(calculationGroup1.Children, typeof(PipingCalculationScenario));

            var pipingCalculationScenario1 = (PipingCalculationScenario) calculationGroup1.Children[0];
            Assert.AreEqual((RoundedDouble) soilProfile1.Probability, pipingCalculationScenario1.Contribution);

            PipingInput calculationInput1 = pipingCalculationScenario1.InputParameters;
            Assert.AreSame(soilProfile1, calculationInput1.StochasticSoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput1.SurfaceLine);

            PipingInput calculationInput2 = ((PipingCalculationScenario) calculationGroup1.Children[1]).InputParameters;
            Assert.AreSame(soilProfile2, calculationInput2.StochasticSoilProfile);
            Assert.AreSame(surfaceLine1, calculationInput2.SurfaceLine);
        }

        [Test]
        public void GenerateCalculationItemsStructure_Always_CreateCalculationsWithSurfaceLineNameSoilProfileNameGeneralInputAndSemiProbabilisticInput()
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

            var generalInput = new GeneralPipingInput();

            // Call
            IEnumerable<ICalculationBase> result = PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                surfaceLines,
                availableSoilModels,
                generalInput).ToArray();

            // Assert
            var group = result.First(sl => sl.Name == surfaceLine.Name) as CalculationGroup;
            Assert.NotNull(group);
            var calculationInput1 = (PipingCalculationScenario) group.Children[0];
            var calculationInput2 = (PipingCalculationScenario) group.Children[1];

            Assert.AreEqual($"{surfaceLine.Name} {soilProfile1.Name}", calculationInput1.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile2.Name}", calculationInput2.Name);

            CompareGeneralInputToInput(generalInput, calculationInput1);
            CompareGeneralInputToInput(generalInput, calculationInput2);
        }

        [Test]
        public void GenerateCalculationItemsStructure_SoilProfileEqualNames_CalculationsGetUniqueName()
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
                surfaceLines,
                availableSoilModels,
                new GeneralPipingInput()).ToArray();

            // Assert
            var group = result.First(sl => sl.Name == surfaceLine.Name) as CalculationGroup;
            Assert.NotNull(group);
            var calculationInput1 = (PipingCalculationScenario) group.Children[0];
            var calculationInput2 = (PipingCalculationScenario) group.Children[1];
            var calculationInput3 = (PipingCalculationScenario) group.Children[2];

            Assert.AreEqual($"{surfaceLine.Name} {soilProfile1.Name}", calculationInput1.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile2.Name} (1)", calculationInput2.Name);
            Assert.AreEqual($"{surfaceLine.Name} {soilProfile3.Name} (2)", calculationInput3.Name);
        }

        #endregion
    }
}