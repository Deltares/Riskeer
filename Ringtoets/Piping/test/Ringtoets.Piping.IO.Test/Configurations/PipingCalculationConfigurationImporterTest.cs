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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.IO.Configurations;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationImporterTest
    {
        private readonly string importerPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Piping.IO, nameof(PipingCalculationConfigurationImporter));

        private static IEnumerable<TestCaseData> ValidConfigurationInvalidData
        {
            get
            {
                yield return new TestCaseData("validConfigurationInvalidEntryExitPoint.xml",
                                              "Een waarde van '2,2' als uittredepunt is ongeldig. Het uittredepunt moet landwaarts van het intredepunt liggen.");
                yield return new TestCaseData("validConfigurationExitPointNotOnSurfaceLine.xml",
                                              "Een waarde van '200,2' als uittredepunt is ongeldig. Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 10,0]).");
                yield return new TestCaseData("validConfigurationEntryPointNotOnSurfaceLine.xml",
                                              "Een waarde van '-10' als intredepunt is ongeldig. Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 10,0]).");
                yield return new TestCaseData("validConfigurationCalculationContainingInfinityEntryPoint.xml",
                                              $"Een waarde van '{double.NegativeInfinity}' als intredepunt is ongeldig. Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 10,0]).");
                yield return new TestCaseData("validConfigurationCalculationContainingInfinityExitPoint.xml",
                                              $"Een waarde van '{double.PositiveInfinity}' als uittredepunt is ongeldig. Het gespecificeerde punt moet op het profiel liggen (bereik [0,0, 10,0]).");
                yield return new TestCaseData("validConfigurationInvalidStandardDeviationPhreaticLevelExit.xml",
                                              "Een standaardafwijking van '-1' is ongeldig voor stochast 'polderpeil'. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.");
                yield return new TestCaseData("validConfigurationInvalidMeanDampingFactorExit.xml",
                                              "Een gemiddelde van '-1' is ongeldig voor stochast 'dempingsfactor'. Gemiddelde moet groter zijn dan 0.");
                yield return new TestCaseData("validConfigurationInvalidStandardDeviationDampingFactorExit.xml",
                                              "Een standaardafwijking van '-1' is ongeldig voor stochast 'dempingsfactor'. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.");
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new PipingCalculationConfigurationImporter("",
                                                                      new CalculationGroup(),
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<PipingCalculationConfigurationReader, PipingCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingCalculationConfigurationImporter("",
                                                                                 new CalculationGroup(),
                                                                                 null,
                                                                                 new PipingFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("availableHydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingCalculationConfigurationImporter("",
                                                                                 new CalculationGroup(),
                                                                                 Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCaseSource(nameof(ValidConfigurationInvalidData))]
        public void Import_ValidConfigurationInvalidData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new PipingSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      new HydraulicBoundaryLocation[0],
                                                                      pipingFailureMechanism);
            var successful = false;

            // Call
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      new PipingFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De hydraulische belastingenlocatie 'Locatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_SurfaceLineUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSurfaceLine.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      new HydraulicBoundaryLocation[0],
                                                                      new PipingFailureMechanism());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De profielschematisatie 'Profielschematisatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilModelUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var pipingFailureMechanism = new PipingFailureMechanism();
            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      new HydraulicBoundaryLocation[0],
                                                                      pipingFailureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilModelNotIntersectingWithSurfaceLine_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingNonIntersectingSurfaceLineAndSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new PipingSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 1.0, 0.0),
                new Point3D(2.5, 1.0, 1.0),
                new Point3D(5.0, 1.0, 0.0)
            });
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("Ondergrondmodel",
                                                                                                                                 new[]
                                                                                                                                 {
                                                                                                                                     new Point2D(1.0, 0.0),
                                                                                                                                     new Point2D(5.0, 0.0)
                                                                                                                                 });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      new HydraulicBoundaryLocation[0],
                                                                      pipingFailureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel'doorkruist de profielschematisatie 'Profielschematisatie' niet." +
                                           " Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilProfileUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSoilProfile.xml");

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new PipingSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel("Ondergrondmodel",
                                                                                                                                 new[]
                                                                                                                                 {
                                                                                                                                     new Point2D(1.0, 0.0),
                                                                                                                                     new Point2D(5.0, 0.0)
                                                                                                                                 });

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      new HydraulicBoundaryLocation[0],
                                                                      pipingFailureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De ondergrondschematisatie 'Ondergrondschematisatie' bestaat niet binnen het stochastische ondergrondmodel 'Ondergrondmodel'. " +
                                           "Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochasticSoilProfileSpecifiedWithoutSoilModel_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingSoilProfileWithoutSoilModel.xml");

            var calculationGroup = new CalculationGroup();
            var pipingFailureMechanism = new PipingFailureMechanism();
            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      new HydraulicBoundaryLocation[0],
                                                                      pipingFailureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen stochastisch ondergrondmodel opgegeven bij ondergrondschematisatie 'Ondergrondschematisatie'. " +
                                           "Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [TestCase("validConfigurationCalculationContainingEntryPointWithoutSurfaceLine.xml")]
        [TestCase("validConfigurationCalculationContainingExitPointWithoutSurfaceLine.xml")]
        [TestCase("validConfigurationCalculationContainingEntryPointAndExitPointWithoutSurfaceLine.xml")]
        [TestCase("validConfigurationCalculationContainingNaNs.xml")]
        public void Import_EntryAndOrExitPointDefinedWithoutSurfaceLine_LogMessageAndContinueImport(string file)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var pipingFailureMechanism = new PipingFailureMechanism();
            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      new HydraulicBoundaryLocation[0],
                                                                      pipingFailureMechanism);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen profielschematisatie, maar wel een intrede- of uittredepunt opgegeven. " +
                                           "Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_StochastsWithNoParametersSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationStochastsNoParameters.xml");

            var calculationGroup = new CalculationGroup();

            PipingFailureMechanism pipingFailureMechanism = CreatePipingFailureMechanism();

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      pipingFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
            {
                Name = "Calculation"
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertPipingCalculationScenario(expectedCalculation, (PipingCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_StochastsWithMeanSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationStochastsMeanOnly.xml");

            var calculationGroup = new CalculationGroup();

            PipingFailureMechanism pipingFailureMechanism = CreatePipingFailureMechanism();

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      pipingFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
            {
                Name = "Calculation",
                InputParameters =
                {
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble) 4.4
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble) 6.6
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertPipingCalculationScenario(expectedCalculation, (PipingCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_StochastsWithStandardDeviationSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationStochastsStandardDeviationOnly.xml");

            var calculationGroup = new CalculationGroup();

            PipingFailureMechanism pipingFailureMechanism = CreatePipingFailureMechanism();

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      pipingFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
            {
                Name = "Calculation",
                InputParameters =
                {
                    PhreaticLevelExit =
                    {
                        StandardDeviation = (RoundedDouble) 5.5
                    },
                    DampingFactorExit =
                    {
                        StandardDeviation = (RoundedDouble) 7.7
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertPipingCalculationScenario(expectedCalculation, (PipingCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ScenarioEmpty_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationContainingEmptyScenario.xml");

            var calculationGroup = new CalculationGroup();

            var pipingFailureMechanism = new PipingFailureMechanism();

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      pipingFailureMechanism);

            var successful = false;

            // Call
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "In een berekening moet voor het scenario tenminste de relevantie of contributie worden opgegeven. " +
                                           "Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(call, expectedMessage, 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ScenarioWithContributionSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationScenarioContributionOnly.xml");

            var calculationGroup = new CalculationGroup();

            PipingFailureMechanism pipingFailureMechanism = CreatePipingFailureMechanism();

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      pipingFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
            {
                Name = "Calculation",
                Contribution = (RoundedDouble) 0.8765
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertPipingCalculationScenario(expectedCalculation, (PipingCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ScenarioWithRevelantSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationScenarioRevelantOnly.xml");

            var calculationGroup = new CalculationGroup();

            PipingFailureMechanism pipingFailureMechanism = CreatePipingFailureMechanism();

            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      pipingFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
            {
                Name = "Calculation",
                IsRelevant = false
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertPipingCalculationScenario(expectedCalculation, (PipingCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        [TestCase("validConfigurationFullCalculationContainingHydraulicBoundaryLocation.xml", false)]
        [TestCase("validConfigurationFullCalculationContainingWaterLevel.xml", true)]
        public void Import_ValidConfigurationWithValidHydraulicBoundaryData_DataAddedToModel(string file, bool manualAssessmentLevel)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var surfaceLine = new PipingSurfaceLine("Profielschematisatie");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3.0, 5.0, 0.0),
                new Point3D(3.0, 0.0, 1.0),
                new Point3D(3.0, -5.0, 0.0)
            });
            var stochasticSoilProfile = new PipingStochasticSoilProfile(0, new PipingSoilProfile("Ondergrondschematisatie", 0, new[]
            {
                new PipingSoilLayer(0)
            }, SoilProfileType.SoilProfile1D));

            var stochasticSoilModel = new PipingStochasticSoilModel("Ondergrondmodel", new[]
            {
                new Point2D(1.0, 0.0),
                new Point2D(5.0, 0.0)
            }, new[]
            {
                stochasticSoilProfile
            });

            PipingFailureMechanism pipingFailureMechanism = CreatePipingFailureMechanism();
            pipingFailureMechanism.SurfaceLines.AddRange(new[]
            {
                surfaceLine
            }, "readerPath");
            pipingFailureMechanism.StochasticSoilModels.AddRange(new[]
            {
                stochasticSoilModel
            }, "readerPath");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Locatie", 10, 20);
            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      new[]
                                                                      {
                                                                          hydraulicBoundaryLocation
                                                                      },
                                                                      pipingFailureMechanism);

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new PipingCalculationScenario(pipingFailureMechanism.GeneralInput)
            {
                Name = "Calculation",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    UseAssessmentLevelManualInput = manualAssessmentLevel,
                    SurfaceLine = surfaceLine,
                    EntryPointL = (RoundedDouble) 2.2,
                    ExitPointL = (RoundedDouble) 3.3,
                    StochasticSoilModel = stochasticSoilModel,
                    StochasticSoilProfile = stochasticSoilProfile,
                    PhreaticLevelExit =
                    {
                        Mean = (RoundedDouble) 4.4,
                        StandardDeviation = (RoundedDouble) 5.5
                    },
                    DampingFactorExit =
                    {
                        Mean = (RoundedDouble) 6.6,
                        StandardDeviation = (RoundedDouble) 7.7
                    }
                },
                Contribution = (RoundedDouble) 0.088,
                IsRelevant = false
            };
            if (manualAssessmentLevel)
            {
                expectedCalculation.InputParameters.AssessmentLevel = (RoundedDouble) 1.1;
            }

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertPipingCalculationScenario(expectedCalculation, (PipingCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void GivenImportedCalculation_WhenPipingGeneralInputChanges_ThenImportedCalculationUpdated()
        {
            // Given
            string filePath = Path.Combine(importerPath, "validConfigurationStochastsNoParameters.xml");

            var calculationGroup = new CalculationGroup();

            PipingFailureMechanism pipingFailureMechanism = CreatePipingFailureMechanism();
            var importer = new PipingCalculationConfigurationImporter(filePath,
                                                                      calculationGroup,
                                                                      Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                      pipingFailureMechanism);
            importer.Import();

            // When
            var random = new Random(33);
            GeneralPipingInput generalInputParameters = pipingFailureMechanism.GeneralInput;
            generalInputParameters.WaterVolumetricWeight = random.NextRoundedDouble(0, 20);

            // Then
            var expectedCalculation = new PipingCalculationScenario(generalInputParameters)
            {
                Name = "Calculation"
            };

            AssertPipingCalculationScenario(expectedCalculation, (PipingCalculationScenario) calculationGroup.Children[0]);
        }

        private static PipingFailureMechanism CreatePipingFailureMechanism()
        {
            var random = new Random(21);
            var pipingFailureMechanism = new PipingFailureMechanism
            {
                GeneralInput =
                {
                    WaterVolumetricWeight = random.NextRoundedDouble(0, 20)
                }
            };
            return pipingFailureMechanism;
        }

        private static void AssertPipingCalculationScenario(PipingCalculationScenario expectedCalculation, PipingCalculationScenario actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
            Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution);

            PipingInput expectedInput = expectedCalculation.InputParameters;
            PipingInput actualInput = actualCalculation.InputParameters;
            Assert.AreEqual(expectedInput.UseAssessmentLevelManualInput, actualInput.UseAssessmentLevelManualInput);
            if (expectedInput.UseAssessmentLevelManualInput)
            {
                Assert.AreEqual(expectedInput.AssessmentLevel, actualInput.AssessmentLevel);
            }
            else
            {
                Assert.AreSame(expectedInput.HydraulicBoundaryLocation, actualInput.HydraulicBoundaryLocation);
            }

            Assert.AreEqual(expectedInput.WaterVolumetricWeight, actualInput.WaterVolumetricWeight);

            Assert.AreSame(expectedInput.StochasticSoilModel, actualInput.StochasticSoilModel);
            Assert.AreSame(expectedInput.StochasticSoilProfile, actualInput.StochasticSoilProfile);
            Assert.AreSame(expectedInput.SurfaceLine, actualInput.SurfaceLine);
            Assert.AreEqual(expectedInput.EntryPointL, actualInput.EntryPointL);
            Assert.AreEqual(expectedInput.ExitPointL, actualInput.ExitPointL);
            DistributionAssert.AreEqual(expectedInput.PhreaticLevelExit, actualInput.PhreaticLevelExit);
            DistributionAssert.AreEqual(expectedInput.DampingFactorExit, actualInput.DampingFactorExit);
        }
    }
}