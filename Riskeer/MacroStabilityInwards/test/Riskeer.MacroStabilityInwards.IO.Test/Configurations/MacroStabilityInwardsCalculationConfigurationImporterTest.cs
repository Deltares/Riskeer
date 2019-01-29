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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Configurations;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Configurations
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationConfigurationImporterTest
    {
        private readonly string importerPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.MacroStabilityInwards.IO,
                                                                          nameof(MacroStabilityInwardsCalculationConfigurationImporter));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter("",
                                                                                     new CalculationGroup(),
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<MacroStabilityInwardsCalculationConfigurationReader, MacroStabilityInwardsCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculationConfigurationImporter("",
                                                                                                new CalculationGroup(),
                                                                                                null,
                                                                                                new MacroStabilityInwardsFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("availableHydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculationConfigurationImporter("",
                                                                                                new CalculationGroup(),
                                                                                                Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithUnknownHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De hydraulische belastingenlocatie 'Locatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_SurfaceLineUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithUnknownSurfaceLine.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De profielschematisatie 'Profielschematisatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilModelUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithUnknownSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     failureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilModelNotIntersectingWithSurfaceLine_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithNonIntersectingSurfaceLineAndSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("Ondergrondmodel", new[]
                {
                    new Point2D(1.0, 0.0),
                    new Point2D(5.0, 0.0)
                });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     failureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel'doorkruist de profielschematisatie 'Profielschematisatie' niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilProfileUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithUnknownSoilProfile.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            MacroStabilityInwardsStochasticSoilModel stochasticSoilModel =
                MacroStabilityInwardsStochasticSoilModelTestFactory.CreateValidStochasticSoilModel("Ondergrondmodel", new[]
                {
                    new Point2D(1.0, 0.0),
                    new Point2D(5.0, 0.0)
                });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     failureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De ondergrondschematisatie 'Ondergrondschematisatie' bestaat niet binnen het stochastische ondergrondmodel 'Ondergrondmodel'. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilProfileSpecifiedWithoutSoilModel_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithSoilProfileWithoutSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     failureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen stochastisch ondergrondmodel opgegeven bij ondergrondschematisatie 'Ondergrondschematisatie'. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ScenarioEmpty_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithEmptyScenario.xml");

            var calculationGroup = new CalculationGroup();

            var macroStabilityInwardsFailureMechanism = new MacroStabilityInwardsFailureMechanism();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     macroStabilityInwardsFailureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "In een berekening moet voor het scenario tenminste de relevantie of contributie worden opgegeven. " +
                                           "Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ScenarioWithContributionSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationScenarioContributionOnly.xml");

            var calculationGroup = new CalculationGroup();

            var macroStabilityInwardsFailureMechanism = new MacroStabilityInwardsFailureMechanism();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     macroStabilityInwardsFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = "Calculation",
                Contribution = (RoundedDouble) 0.8765
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertMacroStabilityInwardsCalculationScenario(expectedCalculation, (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ScenarioWithRevelantSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationScenarioRevelantOnly.xml");

            var calculationGroup = new CalculationGroup();

            var macroStabilityInwardsFailureMechanism = new MacroStabilityInwardsFailureMechanism();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     macroStabilityInwardsFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = "Calculation",
                IsRelevant = false
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertMacroStabilityInwardsCalculationScenario(expectedCalculation, (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ValidTangentLineZTopAndZBottom_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithValidTangentLineZTopAndZBottom.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculations = new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid tangent line Z top and Z bottom",
                    InputParameters =
                    {
                        TangentLineZTop = (RoundedDouble) 0,
                        TangentLineZBottom = (RoundedDouble) 0
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid tangent line Z top, tangent Z bottom NaN",
                    InputParameters =
                    {
                        TangentLineZTop = (RoundedDouble) 1,
                        TangentLineZBottom = RoundedDouble.NaN
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid tangent line Z top",
                    InputParameters =
                    {
                        TangentLineZTop = (RoundedDouble) 1
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Tangent line Z top NaN, valid tangent Z bottom",
                    InputParameters =
                    {
                        TangentLineZTop = RoundedDouble.NaN,
                        TangentLineZBottom = (RoundedDouble) 1
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid tangent line Z bottom",
                    InputParameters =
                    {
                        TangentLineZBottom = (RoundedDouble) 1
                    }
                }
            };

            ICalculation[] actualCalculations = calculationGroup.GetCalculations().ToArray();
            Assert.AreEqual(expectedCalculations.Length, actualCalculations.Length);
            for (var i = 0; i < expectedCalculations.Length; i++)
            {
                AssertMacroStabilityInwardsCalculationScenario(expectedCalculations[i],
                                                               (MacroStabilityInwardsCalculationScenario) actualCalculations[i]);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_InvalidTangentLineZTopAndZBottom_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithInvalidTangentLineZTopAndZBottom.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Een waarde van '0,00' als tangentlijn Z-boven en '10,00' als tangentlijn Z-onder is ongeldig. " +
                                           "Tangentlijn Z-onder moet kleiner zijn dan of gelijk zijn aan tangentlijn Z-boven, of NaN. " +
                                           "Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_InvalidTangentLineNumber_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithInvalidTangentLineNumber.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            TestHelper.AssertLogMessagesWithLevelAreGenerated(call, new[]
            {
                Tuple.Create("Een waarde van '0' als aantal tangentlijnen is ongeldig. " +
                             "De waarde voor het aantal raaklijnen moet in het bereik [1, 50] liggen. " +
                             "Berekening 'Calculation with tangent line too low' is overgeslagen.",
                             LogLevelConstant.Error),
                Tuple.Create("Een waarde van '51' als aantal tangentlijnen is ongeldig. " +
                             "De waarde voor het aantal raaklijnen moet in het bereik [1, 50] liggen. " +
                             "Berekening 'Calculation with tangent line too high' is overgeslagen.",
                             LogLevelConstant.Error)
            });

            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.GetCalculations());
        }

        [Test]
        public void Import_ValidLeftGrid_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithValidLeftGrid.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculations = new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid Z top and Z bottom",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            ZTop = (RoundedDouble) 0,
                            ZBottom = (RoundedDouble) 0
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid Z top, Z bottom NaN",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            ZTop = (RoundedDouble) 1,
                            ZBottom = RoundedDouble.NaN
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid Z top",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            ZTop = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Z top NaN, valid Z bottom",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            ZTop = RoundedDouble.NaN,
                            ZBottom = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid Z bottom",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            ZBottom = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid X rechts and X links",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            XLeft = (RoundedDouble) 0,
                            XRight = (RoundedDouble) 0
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid X rechts, X links NaN",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            XLeft = RoundedDouble.NaN,
                            XRight = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid X rechts",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            XRight = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "X rechts NaN, valid X links",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            XLeft = (RoundedDouble) 1,
                            XRight = RoundedDouble.NaN
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid X links",
                    InputParameters =
                    {
                        LeftGrid =
                        {
                            XLeft = (RoundedDouble) 1
                        }
                    }
                }
            };

            ICalculation[] actualCalculations = calculationGroup.GetCalculations().ToArray();
            Assert.AreEqual(expectedCalculations.Length, actualCalculations.Length);
            for (var i = 0; i < expectedCalculations.Length; i++)
            {
                AssertMacroStabilityInwardsCalculationScenario(expectedCalculations[i],
                                                               (MacroStabilityInwardsCalculationScenario) actualCalculations[i]);
            }
        }

        [Test]
        public void Import_ValidRightGrid_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWithValidRightGrid.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculations = new[]
            {
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid Z top and Z bottom",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            ZTop = (RoundedDouble) 0,
                            ZBottom = (RoundedDouble) 0
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid Z top, Z bottom NaN",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            ZTop = (RoundedDouble) 1,
                            ZBottom = RoundedDouble.NaN
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid Z top",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            ZTop = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Z top NaN, valid Z bottom",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            ZTop = RoundedDouble.NaN,
                            ZBottom = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid Z bottom",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            ZBottom = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid X rechts and X links",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            XLeft = (RoundedDouble) 0,
                            XRight = (RoundedDouble) 0
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid X rechts, X links NaN",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            XLeft = RoundedDouble.NaN,
                            XRight = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid X rechts",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            XRight = (RoundedDouble) 1
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "X rechts NaN, valid X links",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            XLeft = (RoundedDouble) 1,
                            XRight = RoundedDouble.NaN
                        }
                    }
                },
                new MacroStabilityInwardsCalculationScenario
                {
                    Name = "Valid X links",
                    InputParameters =
                    {
                        RightGrid =
                        {
                            XLeft = (RoundedDouble) 1
                        }
                    }
                }
            };

            ICalculation[] actualCalculations = calculationGroup.GetCalculations().ToArray();
            Assert.AreEqual(expectedCalculations.Length, actualCalculations.Length);
            for (var i = 0; i < expectedCalculations.Length; i++)
            {
                AssertMacroStabilityInwardsCalculationScenario(expectedCalculations[i],
                                                               (MacroStabilityInwardsCalculationScenario) actualCalculations[i]);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(GetInvalidGridCombinations))]
        public void Import_InvalidGrid_LogMessageAndContinueImport(string file, string expectedMessage)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new HydraulicBoundaryLocation[0],
                                                                                     new MacroStabilityInwardsFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [TestCase(false, "validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml")]
        [TestCase(true, "validConfigurationFullCalculationContainingWaterLevel.xml")]
        public void Import_ValidConfigurationWithValidHydraulicBoundaryData_DataAddedToModel(bool manualAssessmentLevel, string file)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(0, new MacroStabilityInwardsSoilProfile1D("Ondergrondschematisatie", 0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(0)
            }));

            var stochasticSoilModel = new MacroStabilityInwardsStochasticSoilModel("Ondergrondmodel", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                stochasticSoilProfile
            });

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Locatie", 10, 20);
            var importer = new MacroStabilityInwardsCalculationConfigurationImporter(filePath,
                                                                                     calculationGroup,
                                                                                     new[]
                                                                                     {
                                                                                         hydraulicBoundaryLocation
                                                                                     },
                                                                                     failureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = "Calculation",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    UseAssessmentLevelManualInput = manualAssessmentLevel,
                    SurfaceLine = surfaceLine,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile,
                    WaterLevelRiverAverage = (RoundedDouble) 10.5,
                    DrainageConstructionPresent = true,
                    XCoordinateDrainageConstruction = (RoundedDouble) 10.6,
                    ZCoordinateDrainageConstruction = (RoundedDouble) 10.7,
                    MinimumLevelPhreaticLineAtDikeTopRiver = (RoundedDouble) 10.9,
                    MinimumLevelPhreaticLineAtDikeTopPolder = (RoundedDouble) 10.8,
                    AdjustPhreaticLine3And4ForUplift = true,
                    SlipPlaneMinimumDepth = (RoundedDouble) 0.4,
                    SlipPlaneMinimumLength = (RoundedDouble) 0.5,
                    MaximumSliceWidth = (RoundedDouble) 0.6,
                    CreateZones = true,
                    ZoningBoundariesDeterminationType = MacroStabilityInwardsZoningBoundariesDeterminationType.Manual,
                    ZoneBoundaryLeft = (RoundedDouble) 10.0,
                    ZoneBoundaryRight = (RoundedDouble) 43.5,
                    MoveGrid = true,
                    DikeSoilScenario = MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay,
                    TangentLineZTop = (RoundedDouble) 10,
                    TangentLineZBottom = (RoundedDouble) 1,
                    TangentLineNumber = 5,
                    GridDeterminationType = MacroStabilityInwardsGridDeterminationType.Automatic,
                    TangentLineDeterminationType = MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated,
                    PiezometricHeadPhreaticLine2Inwards = (RoundedDouble) 20.1,
                    PiezometricHeadPhreaticLine2Outwards = (RoundedDouble) 20.2,
                    LeakageLengthInwardsPhreaticLine3 = (RoundedDouble) 10.1,
                    LeakageLengthOutwardsPhreaticLine3 = (RoundedDouble) 10.2,
                    LeakageLengthInwardsPhreaticLine4 = (RoundedDouble) 10.3,
                    LeakageLengthOutwardsPhreaticLine4 = (RoundedDouble) 10.4,
                    LeftGrid =
                    {
                        XLeft = RoundedDouble.NaN,
                        XRight = RoundedDouble.NaN,
                        ZTop = RoundedDouble.NaN,
                        ZBottom = RoundedDouble.NaN,
                        NumberOfVerticalPoints = 6,
                        NumberOfHorizontalPoints = 5
                    },
                    RightGrid =
                    {
                        XLeft = (RoundedDouble) 1,
                        XRight = (RoundedDouble) 2,
                        ZTop = (RoundedDouble) 4,
                        ZBottom = (RoundedDouble) 3,
                        NumberOfVerticalPoints = 5,
                        NumberOfHorizontalPoints = 6
                    },
                    LocationInputDaily =
                    {
                        WaterLevelPolder = (RoundedDouble) 2.2,
                        UseDefaultOffsets = true,
                        PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 2.21,
                        PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 2.24,
                        PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 2.22,
                        PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 2.23
                    },
                    LocationInputExtreme =
                    {
                        PenetrationLength = (RoundedDouble) 16.2,
                        WaterLevelPolder = (RoundedDouble) 15.2,
                        UseDefaultOffsets = false,
                        PhreaticLineOffsetBelowDikeTopAtRiver = (RoundedDouble) 15.21,
                        PhreaticLineOffsetBelowDikeToeAtPolder = (RoundedDouble) 15.24,
                        PhreaticLineOffsetBelowDikeTopAtPolder = (RoundedDouble) 15.22,
                        PhreaticLineOffsetBelowShoulderBaseInside = (RoundedDouble) 15.23
                    }
                },
                IsRelevant = false,
                Contribution = (RoundedDouble) 0.088
            };
            if (manualAssessmentLevel)
            {
                expectedCalculation.InputParameters.AssessmentLevel = (RoundedDouble) 1.1;
            }

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertMacroStabilityInwardsCalculationScenario(expectedCalculation, (MacroStabilityInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        private static void AssertMacroStabilityInwardsCalculationScenario(MacroStabilityInwardsCalculationScenario expectedCalculation,
                                                                           MacroStabilityInwardsCalculationScenario actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);

            MacroStabilityInwardsInput expectedInput = expectedCalculation.InputParameters;
            MacroStabilityInwardsInput actualInput = actualCalculation.InputParameters;
            Assert.AreEqual(expectedInput.UseAssessmentLevelManualInput, actualInput.UseAssessmentLevelManualInput);
            if (expectedInput.UseAssessmentLevelManualInput)
            {
                Assert.AreEqual(expectedInput.AssessmentLevel.Value, actualInput.AssessmentLevel.Value);
            }
            else
            {
                Assert.AreSame(expectedInput.HydraulicBoundaryLocation, actualInput.HydraulicBoundaryLocation);
            }

            Assert.AreSame(expectedInput.SurfaceLine, actualInput.SurfaceLine);
            Assert.AreSame(expectedInput.StochasticSoilModel, actualInput.StochasticSoilModel);
            Assert.AreSame(expectedInput.StochasticSoilProfile, actualInput.StochasticSoilProfile);

            Assert.AreEqual(expectedInput.WaterLevelRiverAverage, actualInput.WaterLevelRiverAverage);
            Assert.AreEqual(expectedInput.DrainageConstructionPresent, actualInput.DrainageConstructionPresent);
            Assert.AreEqual(expectedInput.XCoordinateDrainageConstruction, actualInput.XCoordinateDrainageConstruction);
            Assert.AreEqual(expectedInput.ZCoordinateDrainageConstruction, actualInput.ZCoordinateDrainageConstruction);
            Assert.AreEqual(expectedInput.MinimumLevelPhreaticLineAtDikeTopRiver, actualInput.MinimumLevelPhreaticLineAtDikeTopRiver);
            Assert.AreEqual(expectedInput.MinimumLevelPhreaticLineAtDikeTopPolder, actualInput.MinimumLevelPhreaticLineAtDikeTopPolder);
            Assert.AreEqual(expectedInput.AdjustPhreaticLine3And4ForUplift, actualInput.AdjustPhreaticLine3And4ForUplift);

            Assert.AreEqual(expectedInput.PiezometricHeadPhreaticLine2Inwards, actualInput.PiezometricHeadPhreaticLine2Inwards);
            Assert.AreEqual(expectedInput.PiezometricHeadPhreaticLine2Outwards, actualInput.PiezometricHeadPhreaticLine2Outwards);
            Assert.AreEqual(expectedInput.LeakageLengthInwardsPhreaticLine3, actualInput.LeakageLengthInwardsPhreaticLine3);
            Assert.AreEqual(expectedInput.LeakageLengthOutwardsPhreaticLine3, actualInput.LeakageLengthOutwardsPhreaticLine3);
            Assert.AreEqual(expectedInput.LeakageLengthInwardsPhreaticLine4, actualInput.LeakageLengthInwardsPhreaticLine4);
            Assert.AreEqual(expectedInput.LeakageLengthOutwardsPhreaticLine4, actualInput.LeakageLengthOutwardsPhreaticLine4);

            Assert.AreEqual(expectedInput.SlipPlaneMinimumDepth, actualInput.SlipPlaneMinimumDepth);
            Assert.AreEqual(expectedInput.SlipPlaneMinimumLength, actualInput.SlipPlaneMinimumLength);
            Assert.AreEqual(expectedInput.MaximumSliceWidth, actualInput.MaximumSliceWidth);

            Assert.AreEqual(expectedInput.ZoneBoundaryLeft, actualInput.ZoneBoundaryLeft);
            Assert.AreEqual(expectedInput.ZoneBoundaryRight, actualInput.ZoneBoundaryRight);

            Assert.AreEqual(expectedInput.CreateZones, actualInput.CreateZones);
            Assert.AreEqual(expectedInput.MoveGrid, actualInput.MoveGrid);

            Assert.AreEqual(expectedInput.DikeSoilScenario, actualInput.DikeSoilScenario);
            Assert.AreEqual(expectedInput.TangentLineZTop, actualInput.TangentLineZTop);
            Assert.AreEqual(expectedInput.TangentLineZBottom, actualInput.TangentLineZBottom);
            Assert.AreEqual(expectedInput.TangentLineNumber, actualInput.TangentLineNumber);

            Assert.AreEqual(expectedInput.GridDeterminationType, actualInput.GridDeterminationType);
            Assert.AreEqual(expectedInput.TangentLineDeterminationType, actualInput.TangentLineDeterminationType);
            Assert.AreEqual(expectedInput.ZoningBoundariesDeterminationType, actualInput.ZoningBoundariesDeterminationType);

            AssertMacroStabilityInwardsGrid(expectedInput.LeftGrid, actualInput.LeftGrid);
            AssertMacroStabilityInwardsGrid(expectedInput.RightGrid, actualInput.RightGrid);

            AssertMacroStabilityInwardsLocationInput(expectedInput.LocationInputDaily, actualInput.LocationInputDaily);
            AssertMacroStabilityInwardsLocationInput(expectedInput.LocationInputExtreme, actualInput.LocationInputExtreme);

            Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
            Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution);
        }

        private static void AssertMacroStabilityInwardsGrid(MacroStabilityInwardsGrid expected,
                                                            MacroStabilityInwardsGrid actual)
        {
            Assert.AreEqual(expected.ZTop, actual.ZTop);
            Assert.AreEqual(expected.ZBottom, actual.ZBottom);
            Assert.AreEqual(expected.XLeft, actual.XLeft);
            Assert.AreEqual(expected.XRight, actual.XRight);
            Assert.AreEqual(expected.NumberOfHorizontalPoints, actual.NumberOfHorizontalPoints);
            Assert.AreEqual(expected.NumberOfVerticalPoints, actual.NumberOfVerticalPoints);
        }

        private static void AssertMacroStabilityInwardsLocationInput(IMacroStabilityInwardsLocationInputDaily expected,
                                                                     IMacroStabilityInwardsLocationInputDaily actual)
        {
            Assert.AreEqual(expected.PenetrationLength, actual.PenetrationLength);
            Assert.AreEqual(expected.WaterLevelPolder, actual.WaterLevelPolder);
            Assert.AreEqual(expected.UseDefaultOffsets, actual.UseDefaultOffsets);
            Assert.AreEqual(expected.PhreaticLineOffsetBelowDikeTopAtRiver, actual.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(expected.PhreaticLineOffsetBelowDikeTopAtPolder, actual.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(expected.PhreaticLineOffsetBelowShoulderBaseInside, actual.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(expected.PhreaticLineOffsetBelowDikeToeAtPolder, actual.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        private static void AssertMacroStabilityInwardsLocationInput(IMacroStabilityInwardsLocationInputExtreme expected,
                                                                     IMacroStabilityInwardsLocationInputExtreme actual)
        {
            Assert.AreEqual(expected.PenetrationLength, actual.PenetrationLength);
            Assert.AreEqual(expected.WaterLevelPolder, actual.WaterLevelPolder);
            Assert.AreEqual(expected.UseDefaultOffsets, actual.UseDefaultOffsets);
            Assert.AreEqual(expected.PhreaticLineOffsetBelowDikeTopAtRiver, actual.PhreaticLineOffsetBelowDikeTopAtRiver);
            Assert.AreEqual(expected.PhreaticLineOffsetBelowDikeTopAtPolder, actual.PhreaticLineOffsetBelowDikeTopAtPolder);
            Assert.AreEqual(expected.PhreaticLineOffsetBelowShoulderBaseInside, actual.PhreaticLineOffsetBelowShoulderBaseInside);
            Assert.AreEqual(expected.PhreaticLineOffsetBelowDikeToeAtPolder, actual.PhreaticLineOffsetBelowDikeToeAtPolder);
        }

        private static IEnumerable<TestCaseData> GetInvalidGridCombinations()
        {
            const string expectedZTopZBottomMessage = "Een waarde van '0,00' als Z boven en '10,00' als Z onder is ongeldig. " +
                                                      "Z onder moet kleiner zijn dan of gelijk zijn aan Z boven, of NaN. " +
                                                      "Berekening 'Calculation' is overgeslagen.";
            yield return new TestCaseData("validConfigurationCalculationWithInvalidLeftGridZTopAndZBottom.xml",
                                          expectedZTopZBottomMessage)
                .SetName("Invalid left grid ZTop And ZBottom");
            yield return new TestCaseData("validConfigurationCalculationWithInvalidRightGridZTopAndZBottom.xml",
                                          expectedZTopZBottomMessage)
                .SetName("Invalid right grid ZTop And ZBottom");

            const string expectedXLeftXRightMessage = "Een waarde van '10,00' als X links en '0,00' als X rechts is ongeldig. " +
                                                      "X rechts moet groter zijn dan of gelijk zijn aan X links, of NaN. " +
                                                      "Berekening 'Calculation' is overgeslagen.";
            yield return new TestCaseData("validConfigurationCalculationWithInvalidLeftGridXLeftAndXRight.xml",
                                          expectedXLeftXRightMessage)
                .SetName("Invalid left grid XLeft And XRight");
            yield return new TestCaseData("validConfigurationCalculationWithInvalidRightGridXLeftAndXRight.xml",
                                          expectedXLeftXRightMessage)
                .SetName("Invalid left grid XLeft And XRight");

            const string expectedNumberOfHorizontalPointsTooLargeMessage = "Een waarde van '101' als aantal horizontale punten is ongeldig. " +
                                                                           "De waarde voor het aantal horizontale punten moet in het bereik [1, 100] liggen. " +
                                                                           "Berekening 'Calculation' is overgeslagen.";
            yield return new TestCaseData("validConfigurationCalculationWithInvalidLeftGridNumberOfHorizontalPointsTooLarge.xml",
                                          expectedNumberOfHorizontalPointsTooLargeMessage)
                .SetName("Invalid left grid NumberOfHorizontalPoints too large");
            yield return new TestCaseData("validConfigurationCalculationWithInvalidRightGridNumberOfHorizontalPointsTooLarge.xml",
                                          expectedNumberOfHorizontalPointsTooLargeMessage)
                .SetName("Invalid left grid NumberOfHorizontalPoints too large");

            const string expectedNumberOfHorizontalPointsTooLowMessage = "Een waarde van '0' als aantal horizontale punten is ongeldig. " +
                                                                         "De waarde voor het aantal horizontale punten moet in het bereik [1, 100] liggen. " +
                                                                         "Berekening 'Calculation' is overgeslagen.";
            yield return new TestCaseData("validConfigurationCalculationWithInvalidLeftGridNumberOfHorizontalPointsTooLow.xml",
                                          expectedNumberOfHorizontalPointsTooLowMessage)
                .SetName("Invalid left grid NumberOfHorizontalPoints too low");
            yield return new TestCaseData("validConfigurationCalculationWithInvalidRightGridNumberOfHorizontalPointsTooLow.xml",
                                          expectedNumberOfHorizontalPointsTooLowMessage)
                .SetName("Invalid left grid NumberOfHorizontalPoints too low");

            const string expectedNumberOfVerticalPointsTooLargeMessage = "Een waarde van '101' als aantal verticale punten is ongeldig. " +
                                                                         "De waarde voor het aantal verticale punten moet in het bereik [1, 100] liggen. " +
                                                                         "Berekening 'Calculation' is overgeslagen.";
            yield return new TestCaseData("validConfigurationCalculationWithInvalidLeftGridNumberOfVerticalPointsTooLarge.xml",
                                          expectedNumberOfVerticalPointsTooLargeMessage)
                .SetName("Invalid left grid NumberOfVerticalPoints too large");
            yield return new TestCaseData("validConfigurationCalculationWithInvalidRightGridNumberOfVerticalPointsTooLarge.xml",
                                          expectedNumberOfVerticalPointsTooLargeMessage)
                .SetName("Invalid left grid NumberOfVerticalPoints too large");

            const string expectedNumberOfVerticalPointsTooLowMessage = "Een waarde van '0' als aantal verticale punten is ongeldig. " +
                                                                       "De waarde voor het aantal verticale punten moet in het bereik [1, 100] liggen. " +
                                                                       "Berekening 'Calculation' is overgeslagen.";
            yield return new TestCaseData("validConfigurationCalculationWithInvalidLeftGridNumberOfVerticalPointsTooLow.xml",
                                          expectedNumberOfVerticalPointsTooLowMessage)
                .SetName("Invalid left grid NumberOfVerticalPoints too low");
            yield return new TestCaseData("validConfigurationCalculationWithInvalidRightGridNumberOfVerticalPointsTooLow.xml",
                                          expectedNumberOfVerticalPointsTooLowMessage)
                .SetName("Invalid left grid NumberOfVerticalPoints too low");
        }
    }
}