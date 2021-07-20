﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.IO;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.IO.Configurations;

namespace Riskeer.GrassCoverErosionInwards.IO.Test.Configurations
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationConfigurationImporterTest
    {
        private readonly string importerPath = TestHelper.GetTestDataPath(
            TestDataPath.Riskeer.GrassCoverErosionInwards.IO,
            nameof(GrassCoverErosionInwardsCalculationConfigurationImporter));

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                "", new CalculationGroup(), new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(), Enumerable.Empty<DikeProfile>());

            // Assert
            Assert.IsInstanceOf<
                CalculationConfigurationImporter<
                    GrassCoverErosionInwardsCalculationConfigurationReader,
                    GrassCoverErosionInwardsCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_FailureMechanismContributionNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsCalculationConfigurationImporter(
                "", new CalculationGroup(), null, Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());
            
            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismContribution", exception.ParamName);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsCalculationConfigurationImporter(
                "", new CalculationGroup(), new FailureMechanismContribution(0.01, 0.001),
                null, Enumerable.Empty<DikeProfile>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_DikeProfilesNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new GrassCoverErosionInwardsCalculationConfigurationImporter(
                "", new CalculationGroup(), new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("dikeProfiles", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_ValidConfigurationInvalidOrientation_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationOrientationOutOfRange.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    DikeProfileTestFactory.CreateDikeProfile("Dijkprofiel", "Dijkprofiel ID")
                });

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Een waarde van '380' als oriëntatie is ongeldig. De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen. " +
                                           "Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_ValidConfigurationInvalidCriticalWaveReductionMean_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationNegativeCriticalWaveReductionMean.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Een gemiddelde van '-1' is ongeldig voor stochast 'overslagdebiet'. " +
                                           "Gemiddelde moet groter zijn dan 0. " +
                                           "Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void Import_ValidConfigurationInvalidCriticalWaveReductionStandardDeviation_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationNegativeCriticalWaveReductionStandardDeviation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Een standaardafwijking van '-2,1' is ongeldig voor stochast 'overslagdebiet'. " +
                                           "Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0. " +
                                           "Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationUnknownHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "De hydraulische belastingenlocatie 'Locatie' bestaat niet. Berekening 'Calculation' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_DikeProfileUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationUnknownDikeProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het dijkprofiel met ID 'Dijkprofiel' bestaat niet. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_WaveReductionWithoutDikeProfile_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationWaveReductionWithoutDikeProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen dijkprofiel opgegeven om golfreductie parameters aan toe te voegen. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_DikeHeightWithoutDikeProfile_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationDikeHeightWithoutDikeProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen dijkprofiel opgegeven om de dijkhoogte aan toe te voegen. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_OrientationWithoutDikeProfile_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationOrientationWithoutDikeProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen dijkprofiel opgegeven om de oriëntatie aan toe te voegen. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_UseForeshoreButProfileWithoutGeometry_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationUseForeshoreWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile("Dijkprofiel", "Dijkprofiel ID");
            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    dikeProfile
                });

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het opgegeven dijkprofiel 'Dijkprofiel ID' heeft geen voorlandgeometrie en kan daarom niet gebruikt worden. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageIsGenerated(Call, expectedMessage, 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ValidConfigurationWithoutForeshoreProfileNotUsed_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationNotUseForeshoreWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "HBlocatie", 10, 20);
            DikeProfile dikeProfile = DikeProfileTestFactory.CreateDikeProfile("Dijkprofiel", "Dijkprofiel ID");

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                new[]
                {
                    hydraulicBoundaryLocation
                },
                new[]
                {
                    dikeProfile
                });

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculationScenario(expectedCalculation, (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ScenarioWithContributionSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationScenarioContributionOnly.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                Name = "Calculation",
                Contribution = (RoundedDouble) 0.8765
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculationScenario(expectedCalculation, (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ScenarioWithRelevantSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationScenarioRelevantOnly.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                Name = "Calculation",
                IsRelevant = false
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculationScenario(expectedCalculation, (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ValidConfigurationOnlyCriticalFlowRateMeanSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCriticalFlowRateMeanOnly.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    CriticalFlowRate =
                    {
                        Mean = (RoundedDouble) 2.0
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculationScenario(expectedCalculation, (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ValidConfigurationOnlyCriticalFlowRateNoParametersSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCriticalFlowRateNoParameters.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                Name = "Berekening 1"
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculationScenario(expectedCalculation, (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ValidConfigurationOnlyCriticalFlowRateStandardDeviationSet_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCriticalFlowRateStandardDeviationOnly.xml");

            var calculationGroup = new CalculationGroup();

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<DikeProfile>());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    CriticalFlowRate =
                    {
                        StandardDeviation = (RoundedDouble) 1.1
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculationScenario(expectedCalculation, (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_ValidConfigurationWithValidData_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationFullCalculation.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Locatie", 10, 20);
            var dikeProfile = new DikeProfile(new Point2D(0, 0), new[]
            {
                new RoughnessPoint(new Point2D(0, 0), 2.1),
                new RoughnessPoint(new Point2D(1, 1), 3.9),
                new RoughnessPoint(new Point2D(2, 2), 5.2)
            }, new[]
            {
                new Point2D(1, 0),
                new Point2D(3, 4),
                new Point2D(6, 5)
            }, new BreakWater(BreakWaterType.Caisson, 0), new DikeProfile.ConstructionProperties
            {
                Id = "Dijkprofiel ID",
                Name = "Dijkprofiel",
                DikeHeight = 3.45
            });

            var importer = new GrassCoverErosionInwardsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new FailureMechanismContribution(0.01, 0.001),
                new[]
                {
                    hydraulicBoundaryLocation
                },
                new[]
                {
                    dikeProfile
                });

            // Call
            var successful = false;
            void Call() => successful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(Call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);

            Assert.IsTrue(successful);

            var expectedCalculation = new GrassCoverErosionInwardsCalculationScenario(double.NaN)
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = dikeProfile,
                    DikeHeightCalculationType = DikeHeightCalculationType.CalculateByAssessmentSectionNorm,
                    OvertoppingRateCalculationType = OvertoppingRateCalculationType.CalculateByProfileSpecificRequiredProbability,
                    Orientation = (RoundedDouble) 5.5,
                    UseForeshore = true,
                    UseBreakWater = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 6.6,
                        Type = BreakWaterType.Caisson
                    },
                    CriticalFlowRate =
                    {
                        Mean = (RoundedDouble) 2.0,
                        StandardDeviation = (RoundedDouble) 1.1
                    },
                    ShouldOvertoppingOutputIllustrationPointsBeCalculated = true,
                    ShouldDikeHeightIllustrationPointsBeCalculated = false,
                    ShouldOvertoppingRateIllustrationPointsBeCalculated = true
                },
                IsRelevant = true,
                Contribution = (RoundedDouble) 0.5432
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculationScenario(expectedCalculation, (GrassCoverErosionInwardsCalculationScenario) calculationGroup.Children[0]);
        }

        private static void AssertCalculationScenario(GrassCoverErosionInwardsCalculationScenario expectedCalculation, GrassCoverErosionInwardsCalculationScenario actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreEqual(expectedCalculation.IsRelevant, actualCalculation.IsRelevant);
            Assert.AreEqual(expectedCalculation.Contribution, actualCalculation.Contribution);

            Assert.AreSame(expectedCalculation.InputParameters.HydraulicBoundaryLocation, actualCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedCalculation.InputParameters.Orientation, actualCalculation.InputParameters.Orientation);
            Assert.AreEqual(expectedCalculation.InputParameters.DikeProfile, actualCalculation.InputParameters.DikeProfile);
            Assert.AreEqual(expectedCalculation.InputParameters.DikeHeightCalculationType, actualCalculation.InputParameters.DikeHeightCalculationType);
            Assert.AreEqual(expectedCalculation.InputParameters.OvertoppingRateCalculationType, actualCalculation.InputParameters.OvertoppingRateCalculationType);
            Assert.AreEqual(expectedCalculation.InputParameters.DikeHeight, actualCalculation.InputParameters.DikeHeight);
            Assert.AreEqual(expectedCalculation.InputParameters.UseForeshore, actualCalculation.InputParameters.UseForeshore);
            Assert.AreEqual(expectedCalculation.InputParameters.UseBreakWater, actualCalculation.InputParameters.UseBreakWater);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Height, actualCalculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Type, actualCalculation.InputParameters.BreakWater.Type);
            Assert.AreEqual(expectedCalculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated,
                            actualCalculation.InputParameters.ShouldOvertoppingOutputIllustrationPointsBeCalculated);
            Assert.AreEqual(expectedCalculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated,
                            actualCalculation.InputParameters.ShouldDikeHeightIllustrationPointsBeCalculated);
            Assert.AreEqual(expectedCalculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated,
                            actualCalculation.InputParameters.ShouldOvertoppingRateIllustrationPointsBeCalculated);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.CriticalFlowRate, actualCalculation.InputParameters.CriticalFlowRate);
        }
    }
}