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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Data.TestUtil;
using Riskeer.Piping.IO.Configurations;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.IO.Test.Configurations
{
    [TestFixture]
    public class PipingCalculationConfigurationImporterTest
    {
        private static readonly string importerPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Piping.IO, nameof(PipingCalculationConfigurationImporter));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new PipingCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(), new PipingFailureMechanism());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<PipingCalculationConfigurationReader, PipingCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new PipingCalculationConfigurationImporter(
                "", new CalculationGroup(), null, new PipingFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("availableHydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new PipingCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        /// <summary>
        /// Test fixture base class for running <see cref="PipingCalculationConfigurationImporter"/> tests
        /// that need to be run for both <see cref="SemiProbabilisticPipingCalculationScenario"/> and
        /// <see cref="ProbabilisticPipingCalculationScenario"/>. 
        /// </summary>
        /// <typeparam name="TCalculationScenario">The type of the calculation scenario.</typeparam>
        private abstract class PipingCalculationConfigurationImporterTestFixture<TCalculationScenario>
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>
        {
            [Test]
            [SetCulture("nl-NL")]
            [TestCaseSource(typeof(PipingCalculationConfigurationImporterTestFixture<IPipingCalculationScenario<PipingInput>>), nameof(ValidConfigurationInvalidData))]
            public void Import_ValidConfigurationInvalidData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_ValidConfigurationInvalidData_LogMessageAndContinueImport));
                SetCalculationType(Path.Combine(importerPath, file), filePath);

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

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, new HydraulicBoundaryLocation[0], pipingFailureMechanism);

                var successful = false;

                try
                {
                    // Call
                    void Call() => successful = importer.Import();

                    // Assert
                    var expectedMessage = $"{expectedErrorMessage} Berekening 'Calculation' is overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
                    Assert.IsTrue(successful);
                    CollectionAssert.IsEmpty(calculationGroup.Children);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownHydraulicBoundaryLocation.xml"), filePath);

                var calculationGroup = new CalculationGroup();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, Enumerable.Empty<HydraulicBoundaryLocation>(), new PipingFailureMechanism());

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    const string expectedMessage = "De hydraulische belastingenlocatie 'Locatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
                    Assert.IsTrue(successful);
                    CollectionAssert.IsEmpty(calculationGroup.Children);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_SurfaceLineUnknown_LogMessageAndContinueImport()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_SurfaceLineUnknown_LogMessageAndContinueImport));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSurfaceLine.xml"), filePath);

                var calculationGroup = new CalculationGroup();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, new HydraulicBoundaryLocation[0], new PipingFailureMechanism());

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    const string expectedMessage = "De profielschematisatie 'Profielschematisatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
                    Assert.IsTrue(successful);
                    CollectionAssert.IsEmpty(calculationGroup.Children);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_StochasticSoilModelUnknown_LogMessageAndContinueImport()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_StochasticSoilModelUnknown_LogMessageAndContinueImport));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSoilModel.xml"), filePath);

                var calculationGroup = new CalculationGroup();
                var pipingFailureMechanism = new PipingFailureMechanism();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, new HydraulicBoundaryLocation[0], pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel' bestaat niet. Berekening 'Calculation' is overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
                    Assert.IsTrue(successful);
                    CollectionAssert.IsEmpty(calculationGroup.Children);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_StochasticSoilModelNotIntersectingWithSurfaceLine_LogMessageAndContinueImport()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_StochasticSoilModelNotIntersectingWithSurfaceLine_LogMessageAndContinueImport));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationCalculationContainingNonIntersectingSurfaceLineAndSoilModel.xml"), filePath);

                var calculationGroup = new CalculationGroup();
                var surfaceLine = new PipingSurfaceLine("Profielschematisatie");
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(0.0, 1.0, 0.0),
                    new Point3D(2.5, 1.0, 1.0),
                    new Point3D(5.0, 1.0, 0.0)
                });
                PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(
                    "Ondergrondmodel",
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

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, new HydraulicBoundaryLocation[0], pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    const string expectedMessage = "Het stochastische ondergrondmodel 'Ondergrondmodel'doorkruist de profielschematisatie 'Profielschematisatie' niet." +
                                                   " Berekening 'Calculation' is overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
                    Assert.IsTrue(successful);
                    CollectionAssert.IsEmpty(calculationGroup.Children);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_StochasticSoilProfileUnknown_LogMessageAndContinueImport()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_StochasticSoilProfileUnknown_LogMessageAndContinueImport));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationCalculationContainingUnknownSoilProfile.xml"), filePath);

                var calculationGroup = new CalculationGroup();
                var surfaceLine = new PipingSurfaceLine("Profielschematisatie");
                surfaceLine.SetGeometry(new[]
                {
                    new Point3D(3.0, 5.0, 0.0),
                    new Point3D(3.0, 0.0, 1.0),
                    new Point3D(3.0, -5.0, 0.0)
                });
                PipingStochasticSoilModel stochasticSoilModel = PipingStochasticSoilModelTestFactory.CreatePipingStochasticSoilModel(
                    "Ondergrondmodel",
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

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, new HydraulicBoundaryLocation[0], pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    const string expectedMessage = "De ondergrondschematisatie 'Ondergrondschematisatie' bestaat niet binnen het stochastische ondergrondmodel 'Ondergrondmodel'. " +
                                                   "Berekening 'Calculation' is overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
                    Assert.IsTrue(successful);
                    CollectionAssert.IsEmpty(calculationGroup.Children);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_StochasticSoilProfileSpecifiedWithoutSoilModel_LogMessageAndContinueImport()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_StochasticSoilProfileSpecifiedWithoutSoilModel_LogMessageAndContinueImport));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationCalculationContainingSoilProfileWithoutSoilModel.xml"), filePath);

                var calculationGroup = new CalculationGroup();
                var pipingFailureMechanism = new PipingFailureMechanism();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, new HydraulicBoundaryLocation[0], pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    const string expectedMessage = "Er is geen stochastisch ondergrondmodel opgegeven bij ondergrondschematisatie 'Ondergrondschematisatie'. " +
                                                   "Berekening 'Calculation' is overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
                    Assert.IsTrue(successful);
                    CollectionAssert.IsEmpty(calculationGroup.Children);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            [TestCase("validConfigurationCalculationContainingEntryPointWithoutSurfaceLine.xml")]
            [TestCase("validConfigurationCalculationContainingExitPointWithoutSurfaceLine.xml")]
            [TestCase("validConfigurationCalculationContainingEntryPointAndExitPointWithoutSurfaceLine.xml")]
            [TestCase("validConfigurationCalculationContainingNaNs.xml")]
            public void Import_EntryAndOrExitPointDefinedWithoutSurfaceLine_LogMessageAndContinueImport(string file)
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_EntryAndOrExitPointDefinedWithoutSurfaceLine_LogMessageAndContinueImport));
                SetCalculationType(Path.Combine(importerPath, file), filePath);

                var calculationGroup = new CalculationGroup();
                var pipingFailureMechanism = new PipingFailureMechanism();
                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, new HydraulicBoundaryLocation[0], pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    const string expectedMessage = "Er is geen profielschematisatie, maar wel een intrede- of uittredepunt opgegeven. " +
                                                   "Berekening 'Calculation' is overgeslagen.";
                    TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
                    Assert.IsTrue(successful);
                    CollectionAssert.IsEmpty(calculationGroup.Children);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_StochastsWithNoParametersSet_DataAddedToModel()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_StochastsWithNoParametersSet_DataAddedToModel));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationStochastsNoParameters.xml"), filePath);

                var calculationGroup = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, Enumerable.Empty<HydraulicBoundaryLocation>(), pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
                    Assert.IsTrue(successful);

                    TCalculationScenario expectedCalculation = CreateCalculationScenario();
                    expectedCalculation.Name = "Calculation";

                    Assert.AreEqual(1, calculationGroup.Children.Count);
                    AssertPipingCalculationScenario(expectedCalculation, (TCalculationScenario) calculationGroup.Children[0]);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_StochastsWithMeanSet_DataAddedToModel()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_StochastsWithMeanSet_DataAddedToModel));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationStochastsMeanOnly.xml"), filePath);

                var calculationGroup = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, Enumerable.Empty<HydraulicBoundaryLocation>(), pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
                    Assert.IsTrue(successful);

                    TCalculationScenario expectedCalculation = CreateCalculationScenario();
                    expectedCalculation.Name = "Calculation";
                    expectedCalculation.InputParameters.PhreaticLevelExit.Mean = (RoundedDouble) 4.4;
                    expectedCalculation.InputParameters.DampingFactorExit.Mean = (RoundedDouble) 6.6;

                    Assert.AreEqual(1, calculationGroup.Children.Count);
                    AssertPipingCalculationScenario(expectedCalculation, (TCalculationScenario) calculationGroup.Children[0]);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_StochastsWithStandardDeviationSet_DataAddedToModel()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_StochastsWithStandardDeviationSet_DataAddedToModel));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationStochastsStandardDeviationOnly.xml"), filePath);

                var calculationGroup = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, Enumerable.Empty<HydraulicBoundaryLocation>(), pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
                    Assert.IsTrue(successful);

                    TCalculationScenario expectedCalculation = CreateCalculationScenario();
                    expectedCalculation.Name = "Calculation";
                    expectedCalculation.InputParameters.PhreaticLevelExit.StandardDeviation = (RoundedDouble) 5.5;
                    expectedCalculation.InputParameters.DampingFactorExit.StandardDeviation = (RoundedDouble) 7.7;

                    Assert.AreEqual(1, calculationGroup.Children.Count);
                    AssertPipingCalculationScenario(expectedCalculation, (TCalculationScenario) calculationGroup.Children[0]);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_ScenarioWithContributionSet_DataAddedToModel()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_ScenarioWithContributionSet_DataAddedToModel));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationScenarioContributionOnly.xml"), filePath);

                var calculationGroup = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, Enumerable.Empty<HydraulicBoundaryLocation>(), pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
                    Assert.IsTrue(successful);

                    TCalculationScenario expectedCalculation = CreateCalculationScenario();
                    expectedCalculation.Name = "Calculation";
                    expectedCalculation.Contribution = (RoundedDouble) 0.8765;

                    Assert.AreEqual(1, calculationGroup.Children.Count);
                    AssertPipingCalculationScenario(expectedCalculation, (TCalculationScenario) calculationGroup.Children[0]);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            [Test]
            public void Import_ScenarioWithRelevantSet_DataAddedToModel()
            {
                // Setup
                string filePath = TestHelper.GetScratchPadPath(nameof(Import_ScenarioWithRelevantSet_DataAddedToModel));
                SetCalculationType(Path.Combine(importerPath, "validConfigurationScenarioRelevantOnly.xml"), filePath);

                var calculationGroup = new CalculationGroup();

                var pipingFailureMechanism = new PipingFailureMechanism();

                var importer = new PipingCalculationConfigurationImporter(
                    filePath, calculationGroup, Enumerable.Empty<HydraulicBoundaryLocation>(), pipingFailureMechanism);

                try
                {
                    // Call
                    var successful = false;
                    void Call() => successful = importer.Import();

                    // Assert
                    TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
                    Assert.IsTrue(successful);

                    TCalculationScenario expectedCalculation = CreateCalculationScenario();
                    expectedCalculation.Name = "Calculation";
                    expectedCalculation.IsRelevant = false;

                    Assert.AreEqual(1, calculationGroup.Children.Count);
                    AssertPipingCalculationScenario(expectedCalculation, (TCalculationScenario) calculationGroup.Children[0]);
                }
                finally
                {
                    File.Delete(filePath);
                }
            }

            protected abstract string CalculationType { get; }

            protected abstract TCalculationScenario CreateCalculationScenario();

            protected abstract void AssertPipingCalculationScenario(TCalculationScenario expectedCalculation,
                                                                    TCalculationScenario actualCalculation);

            protected void AssertPipingCalculationScenarioGenericProperties(TCalculationScenario expectedCalculation,
                                                                            TCalculationScenario actualCalculation)
            {
                Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
                Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
                Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution);

                PipingInput expectedInput = expectedCalculation.InputParameters;
                PipingInput actualInput = actualCalculation.InputParameters;

                Assert.AreSame(expectedInput.StochasticSoilModel, actualInput.StochasticSoilModel);
                Assert.AreSame(expectedInput.StochasticSoilProfile, actualInput.StochasticSoilProfile);
                Assert.AreSame(expectedInput.SurfaceLine, actualInput.SurfaceLine);
                Assert.AreEqual(expectedInput.EntryPointL, actualInput.EntryPointL);
                Assert.AreEqual(expectedInput.ExitPointL, actualInput.ExitPointL);
                DistributionAssert.AreEqual(expectedInput.PhreaticLevelExit, actualInput.PhreaticLevelExit);
                DistributionAssert.AreEqual(expectedInput.DampingFactorExit, actualInput.DampingFactorExit);
            }

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
                                                  "Een standaardafwijking van '-1' is ongeldig voor stochast 'binnendijksewaterstand'. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.");
                    yield return new TestCaseData("validConfigurationInvalidMeanDampingFactorExit.xml",
                                                  "Een gemiddelde van '-1' is ongeldig voor stochast 'dempingsfactor'. Gemiddelde moet groter zijn dan 0.");
                    yield return new TestCaseData("validConfigurationInvalidStandardDeviationDampingFactorExit.xml",
                                                  "Een standaardafwijking van '-1' is ongeldig voor stochast 'dempingsfactor'. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.");
                }
            }

            private void SetCalculationType(string readFilePath, string writeFilePath)
            {
                string text = File.ReadAllText(readFilePath);
                text = text.Replace("toetstype", CalculationType);
                File.WriteAllText(writeFilePath, text);
            }
        }

        /// <summary>
        /// Test fixture for running <see cref="PipingCalculationConfigurationImporter"/> tests for
        /// <see cref="SemiProbabilisticPipingCalculationScenario"/>. 
        /// </summary>
        [TestFixture]
        private class SemiProbabilisticPipingCalculationConfigurationImporterTest : PipingCalculationConfigurationImporterTestFixture<SemiProbabilisticPipingCalculationScenario>
        {
            protected override string CalculationType => "semi-probabilistisch";

            [Test]
            [TestCase("validConfigurationFullSemiProbabilisticCalculationContainingHydraulicBoundaryLocation.xml", false)]
            [TestCase("validConfigurationFullSemiProbabilisticCalculationContainingWaterLevel.xml", true)]
            public void Import_ValidConfigurationFullSemiProbabilisticCalculationWithValidHydraulicBoundaryData_DataAddedToModel(
                string file, bool manualAssessmentLevel)
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

                var pipingFailureMechanism = new PipingFailureMechanism();
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
                var successful = false;
                void Call() => successful = importer.Import();

                // Assert
                TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
                Assert.IsTrue(successful);

                var expectedCalculation = new SemiProbabilisticPipingCalculationScenario
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
                AssertPipingCalculationScenario(expectedCalculation, (SemiProbabilisticPipingCalculationScenario) calculationGroup.Children[0]);
            }

            protected override SemiProbabilisticPipingCalculationScenario CreateCalculationScenario()
            {
                return new SemiProbabilisticPipingCalculationScenario();
            }

            protected override void AssertPipingCalculationScenario(SemiProbabilisticPipingCalculationScenario expectedCalculation,
                                                                    SemiProbabilisticPipingCalculationScenario actualCalculation)
            {
                AssertPipingCalculationScenarioGenericProperties(expectedCalculation, actualCalculation);

                SemiProbabilisticPipingInput expectedInput = expectedCalculation.InputParameters;
                SemiProbabilisticPipingInput actualInput = actualCalculation.InputParameters;
                Assert.AreEqual(expectedInput.UseAssessmentLevelManualInput, actualInput.UseAssessmentLevelManualInput);
                if (expectedInput.UseAssessmentLevelManualInput)
                {
                    Assert.AreEqual(expectedInput.AssessmentLevel, actualInput.AssessmentLevel);
                }
                else
                {
                    Assert.AreSame(expectedInput.HydraulicBoundaryLocation, actualInput.HydraulicBoundaryLocation);
                }
            }
        }

        /// <summary>
        /// Test fixture for running <see cref="PipingCalculationConfigurationImporter"/> tests for
        /// <see cref="ProbabilisticPipingCalculationScenario"/>. 
        /// </summary>
        [TestFixture]
        private class ProbabilisticPipingCalculationConfigurationImporterTest : PipingCalculationConfigurationImporterTestFixture<ProbabilisticPipingCalculationScenario>
        {
            protected override string CalculationType => "probabilistisch";

            [Test]
            public void Import_ValidConfigurationFullProbabilisticCalculationWithValidHydraulicBoundaryData_DataAddedToModel()
            {
                // Setup
                string filePath = Path.Combine(importerPath, "validConfigurationFullProbabilisticCalculation.xml");

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

                var pipingFailureMechanism = new PipingFailureMechanism();
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
                var successful = false;
                void Call() => successful = importer.Import();

                // Assert
                TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
                Assert.IsTrue(successful);

                var expectedCalculation = new ProbabilisticPipingCalculationScenario
                {
                    Name = "Calculation",
                    InputParameters =
                    {
                        HydraulicBoundaryLocation = hydraulicBoundaryLocation,
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
                        },
                        ShouldProfileSpecificIllustrationPointsBeCalculated = true,
                        ShouldSectionSpecificIllustrationPointsBeCalculated = true
                    },
                    Contribution = (RoundedDouble) 0.088,
                    IsRelevant = false
                };

                Assert.AreEqual(1, calculationGroup.Children.Count);
                AssertPipingCalculationScenario(expectedCalculation, (ProbabilisticPipingCalculationScenario) calculationGroup.Children[0]);
            }

            protected override ProbabilisticPipingCalculationScenario CreateCalculationScenario()
            {
                return new ProbabilisticPipingCalculationScenario();
            }

            protected override void AssertPipingCalculationScenario(ProbabilisticPipingCalculationScenario expectedCalculation,
                                                                    ProbabilisticPipingCalculationScenario actualCalculation)
            {
                AssertPipingCalculationScenarioGenericProperties(expectedCalculation, actualCalculation);

                ProbabilisticPipingInput expectedInput = expectedCalculation.InputParameters;
                ProbabilisticPipingInput actualInput = actualCalculation.InputParameters;
                Assert.AreSame(expectedInput.HydraulicBoundaryLocation, actualInput.HydraulicBoundaryLocation);
                Assert.AreEqual(expectedInput.ShouldProfileSpecificIllustrationPointsBeCalculated, actualInput.ShouldProfileSpecificIllustrationPointsBeCalculated);
                Assert.AreEqual(expectedInput.ShouldSectionSpecificIllustrationPointsBeCalculated, actualInput.ShouldSectionSpecificIllustrationPointsBeCalculated);
            }
        }
    }
}