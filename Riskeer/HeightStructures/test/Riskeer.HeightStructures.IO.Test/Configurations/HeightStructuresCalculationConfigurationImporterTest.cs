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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Data.TestUtil;
using Riskeer.HeightStructures.IO.Configurations;

namespace Riskeer.HeightStructures.IO.Test.Configurations
{
    [TestFixture]
    public class HeightStructuresCalculationConfigurationImporterTest
    {
        private readonly string importerPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.HeightStructures.IO, nameof(HeightStructuresCalculationConfigurationImporter));

        private static IEnumerable<TestCaseData> ValidConfigurationInvalidData
        {
            get
            {
                const string testNameFormat = "Import_InvalidData({0:80})";

                yield return new TestCaseData("validConfigurationModelFactorSuperCriticalFlowStandardDeviation.xml",
                                              "Er kan geen spreiding voor stochast 'modelfactoroverloopdebiet' opgegeven worden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationModelFactorSuperCriticalFlowVariationCoefficient.xml",
                                              "Er kan geen spreiding voor stochast 'modelfactoroverloopdebiet' opgegeven worden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationStormDurationVariationCoefficient.xml",
                                              "Er kan geen spreiding voor stochast 'stormduur' opgegeven worden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationStormDurationStandardDeviation.xml",
                                              "Er kan geen spreiding voor stochast 'stormduur' opgegeven worden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationFailureProbabilityStructureErosionWithoutStructure.xml",
                                              "Er is geen kunstwerk opgegeven om faalkans gegeven erosie bodem aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationOrientationWithoutStructure.xml",
                                              "Er is geen kunstwerk opgegeven om oriëntatie aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidFailureProbabilityStructureErosion.xml",
                                              "Een waarde van '1,1' als faalkans gegeven erosie bodem is ongeldig. De waarde voor de faalkans moet in het bereik [0,0, 1,0] liggen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidOrientation.xml",
                                              "Een waarde van '-12' als oriëntatie is ongeldig. De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationWaveReductionWithoutForeshoreProfile.xml",
                                              "Er is geen voorlandprofiel opgegeven om golfreductie parameters aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidAllowedLevelIncreaseStorageMean.xml",
                                              "Een gemiddelde van '-0,2' is ongeldig voor stochast 'peilverhogingkomberging'. Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidAllowedLevelIncreaseStorageStandardDeviation.xml",
                                              "Een standaardafwijking van '-0,01' is ongeldig voor stochast 'peilverhogingkomberging'. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidCriticalOvertoppingDischargeMean.xml",
                                              "Een gemiddelde van '-2' is ongeldig voor stochast 'kritiekinstromenddebiet'. Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidCriticalOvertoppingDischargeVariationCoefficient.xml",
                                              "Een variatiecoëfficiënt van '-0,1' is ongeldig voor stochast 'kritiekinstromenddebiet'. Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidFlowWidthAtBottomProtectionMean.xml",
                                              "Een gemiddelde van '-15,2' is ongeldig voor stochast 'breedtebodembescherming'. Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidFlowWidthAtBottomProtectionStandardDeviation.xml",
                                              "Een standaardafwijking van '-0,1' is ongeldig voor stochast 'breedtebodembescherming'. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidLevelCrestStructureStandardDeviation.xml",
                                              "Een standaardafwijking van '-0,1' is ongeldig voor stochast 'kerendehoogte'. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidStorageStructureAreaMean.xml",
                                              "Een gemiddelde van '-15000' is ongeldig voor stochast 'kombergendoppervlak'. Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidStorageStructureAreaVariationCoefficient.xml",
                                              "Een variatiecoëfficiënt van '-0,01' is ongeldig voor stochast 'kombergendoppervlak'. Variatiecoëfficiënt (CV) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidStormDurationMean.xml",
                                              "Een gemiddelde van '-6' is ongeldig voor stochast 'stormduur'. Gemiddelde moet groter zijn dan 0.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationInvalidWidthFlowAperturesStandardDeviation.xml",
                                              "Een standaardafwijking van '-0,1' is ongeldig voor stochast 'breedtedoorstroomopening'. Standaardafwijking (σ) moet groter zijn dan of gelijk zijn aan 0.")
                    .SetName(testNameFormat);

                yield return new TestCaseData("validConfigurationAllowedLevelIncreaseStorageVariationCoefficient.xml",
                                              "Indien voor parameter 'peilverhogingkomberging' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationCriticalOvertoppingDischargeStandardDeviation.xml",
                                              "Indien voor parameter 'kritiekinstromenddebiet' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationFlowWidthAtBottomProtectionVariationCoefficient.xml",
                                              "Indien voor parameter 'breedtebodembescherming' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationLevelCrestStructureVariationCoefficient.xml",
                                              "Indien voor parameter 'kerendehoogte' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationStorageStructureAreaStandardDeviation.xml",
                                              "Indien voor parameter 'kombergendoppervlak' de spreiding wordt opgegeven, moet dit door middel van een variatiecoëfficiënt. Voor berekening 'Berekening 1' is een standaardafwijking gevonden.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationWidthFlowAperturesVariationCoefficient.xml",
                                              "Indien voor parameter 'breedtedoorstroomopening' de spreiding wordt opgegeven, moet dit door middel van een standaardafwijking. Voor berekening 'Berekening 1' is een variatiecoëfficiënt gevonden.")
                    .SetName(testNameFormat);

                yield return new TestCaseData("validConfigurationAllowedLevelIncreaseWithoutStructure.xml",
                                              "Er is geen kunstwerk opgegeven om de stochast 'peilverhogingkomberging' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationCriticalOvertoppingDischargeWithoutStructure.xml",
                                              "Er is geen kunstwerk opgegeven om de stochast 'kritiekinstromenddebiet' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationFlowWidthAtBottomProtectionWithoutStructure.xml",
                                              "Er is geen kunstwerk opgegeven om de stochast 'breedtebodembescherming' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationLevelCrestStructureWithoutStructure.xml",
                                              "Er is geen kunstwerk opgegeven om de stochast 'kerendehoogte' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationStorageStructureAreaWithoutStructure.xml",
                                              "Er is geen kunstwerk opgegeven om de stochast 'kombergendoppervlak' aan toe te voegen.")
                    .SetName(testNameFormat);
                yield return new TestCaseData("validConfigurationWidthFlowAperturesWithoutStructure.xml",
                                              "Er is geen kunstwerk opgegeven om de stochast 'breedtedoorstroomopening' aan toe te voegen.")
                    .SetName(testNameFormat);
            }
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new HeightStructuresCalculationConfigurationImporter("",
                                                                                new CalculationGroup(),
                                                                                Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                Enumerable.Empty<ForeshoreProfile>(),
                                                                                Enumerable.Empty<HeightStructure>(),
                                                                                new HeightStructuresFailureMechanism());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<HeightStructuresCalculationConfigurationReader, HeightStructuresCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HeightStructuresCalculationConfigurationImporter("",
                                                                                           new CalculationGroup(),
                                                                                           null,
                                                                                           Enumerable.Empty<ForeshoreProfile>(),
                                                                                           Enumerable.Empty<HeightStructure>(),
                                                                                           new HeightStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HeightStructuresCalculationConfigurationImporter("",
                                                                                           new CalculationGroup(),
                                                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                           null,
                                                                                           Enumerable.Empty<HeightStructure>(),
                                                                                           new HeightStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
        }

        [Test]
        public void Constructor_StructuresNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HeightStructuresCalculationConfigurationImporter("",
                                                                                           new CalculationGroup(),
                                                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                           Enumerable.Empty<ForeshoreProfile>(),
                                                                                           null,
                                                                                           new HeightStructuresFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("structures", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HeightStructuresCalculationConfigurationImporter("",
                                                                                           new CalculationGroup(),
                                                                                           Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                           Enumerable.Empty<ForeshoreProfile>(),
                                                                                           Enumerable.Empty<HeightStructure>(),
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
            var structure = new TestHeightStructure("kunstwerk1", "kunstwerk1");
            var foreshoreProfile = new TestForeshoreProfile("profiel 1");

            var importer = new HeightStructuresCalculationConfigurationImporter(filePath,
                                                                                calculationGroup,
                                                                                Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                new ForeshoreProfile[]
                                                                                {
                                                                                    foreshoreProfile
                                                                                },
                                                                                new HeightStructure[]
                                                                                {
                                                                                    structure
                                                                                },
                                                                                new HeightStructuresFailureMechanism());
            var successful = false;

            // Call
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_UseForeshoreButForeshoreProfileWithoutGeometry_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationCalculationUseForeshoreWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var foreshoreProfile = new TestForeshoreProfile("Voorlandprofiel");
            var importer = new HeightStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    foreshoreProfile
                },
                Enumerable.Empty<HeightStructure>(),
                new HeightStructuresFailureMechanism());

            var successful = false;

            // Call
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het opgegeven voorlandprofiel 'Voorlandprofiel' heeft geen voorlandgeometrie en kan daarom niet gebruikt worden. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_FullCalculationConfiguration_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validFullConfiguration.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation("Locatie1");
            var foreshoreProfile = new TestForeshoreProfile("profiel1", new List<Point2D>
            {
                new Point2D(0, 3)
            });
            var structure = new TestHeightStructure("kunstwerk1");
            var importer = new HeightStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                new[]
                {
                    hydraulicBoundaryLocation
                },
                new[]
                {
                    foreshoreProfile
                },
                new[]
                {
                    structure
                },
                new HeightStructuresFailureMechanism());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            var expectedCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    Structure = structure,
                    ForeshoreProfile = foreshoreProfile,
                    StructureNormalOrientation = (RoundedDouble) 67.1,
                    FailureProbabilityStructureWithErosion = 1e-6,
                    UseBreakWater = true,
                    UseForeshore = true,
                    ShouldIllustrationPointsBeCalculated = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 1.23,
                        Type = BreakWaterType.Dam
                    },
                    StormDuration =
                    {
                        Mean = (RoundedDouble) 6.0
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = (RoundedDouble) 1.10
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 15.2,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 15000,
                        CoefficientOfVariation = (RoundedDouble) 0.01
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 0.2,
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) 4.3,
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 2,
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculation<HeightStructuresInput>) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_StochastWithMeanOnly_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationStochastMeansOnly.xml");

            var calculationGroup = new CalculationGroup();
            var structure = new TestHeightStructure("kunstwerk1", "kunstwerk1");
            var importer = new HeightStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new[]
                {
                    structure
                },
                new HeightStructuresFailureMechanism());

            var expectedCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    Structure = structure,
                    StormDuration =
                    {
                        Mean = (RoundedDouble) 6.0
                    },
                    ModelFactorSuperCriticalFlow =
                    {
                        Mean = (RoundedDouble) 1.10
                    },
                    FlowWidthAtBottomProtection =
                    {
                        Mean = (RoundedDouble) 15.2
                    },
                    WidthFlowApertures =
                    {
                        Mean = (RoundedDouble) 15.2
                    },
                    StorageStructureArea =
                    {
                        Mean = (RoundedDouble) 15000
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        Mean = (RoundedDouble) 0.2
                    },
                    LevelCrestStructure =
                    {
                        Mean = (RoundedDouble) 4.3
                    },
                    CriticalOvertoppingDischarge =
                    {
                        Mean = (RoundedDouble) 2
                    }
                }
            };

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculation<HeightStructuresInput>) calculationGroup.Children[0]);
        }

        [Test]
        public void Import_StochastWithStandardDeviationOrVariationCoefficientOnly_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationStochastStandardDeviationVariationCoefficientOnly.xml");

            var calculationGroup = new CalculationGroup();
            var structure = new TestHeightStructure("kunstwerk1", "kunstwerk1");
            var importer = new HeightStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new[]
                {
                    structure
                },
                new HeightStructuresFailureMechanism());

            var expectedCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    Structure = structure,
                    FlowWidthAtBottomProtection =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    WidthFlowApertures =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    StorageStructureArea =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.01
                    },
                    AllowedLevelIncreaseStorage =
                    {
                        StandardDeviation = (RoundedDouble) 0.01
                    },
                    LevelCrestStructure =
                    {
                        StandardDeviation = (RoundedDouble) 0.1
                    },
                    CriticalOvertoppingDischarge =
                    {
                        CoefficientOfVariation = (RoundedDouble) 0.1
                    }
                }
            };

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculation<HeightStructuresInput>) calculationGroup.Children[0]);
        }

        [Test]
        [TestCase("validConfigurationEmptyCalculation.xml")]
        [TestCase("validConfigurationEmptyStochasts.xml")]
        [TestCase("validConfigurationEmptyStochastsElement.xml")]
        [TestCase("validConfigurationEmptyWaveReduction.xml")]
        public void Import_EmptyConfigurations_DataAddedToModel(string file)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();
            var structure = new TestHeightStructure("kunstwerk1", "kunstwerk1");
            var importer = new HeightStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new[]
                {
                    structure
                },
                new HeightStructuresFailureMechanism());

            var expectedCalculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = "Berekening 1"
            };

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);
            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertCalculation(expectedCalculation, (StructuresCalculation<HeightStructuresInput>) calculationGroup.Children[0]);
        }

        [Test]
        public void DoPostImport_CalculationWithStructureInSection_AssignsCalculationToSectionResult()
        {
            // Setup
            string filePath = Path.Combine(importerPath, "validConfigurationFullCalculation.xml");
            var calculationGroup = new CalculationGroup();

            var failureMechanism = new HeightStructuresFailureMechanism();

            FailureMechanismTestHelper.SetSections(failureMechanism, new[]
                {
                    FailureMechanismSectionTestFactory.CreateFailureMechanismSection(new[]
                    {
                        new Point2D(0, 0),
                        new Point2D(10, 10)
                    })
                }
            );

            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                InputParameters =
                {
                    Structure = new TestHeightStructure(new Point2D(5, 5))
                }
            };
            failureMechanism.CalculationsGroup.Children.Add(
                calculation);

            var importer = new HeightStructuresCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                Enumerable.Empty<HeightStructure>(),
                failureMechanism);

            // Preconditions
            Assert.AreEqual(1, failureMechanism.SectionResults.Count());
            Assert.IsNull(failureMechanism.SectionResults.ElementAt(0).Calculation);

            // Call
            importer.DoPostImport();

            // Assert
            Assert.AreSame(calculation, failureMechanism.SectionResults.ElementAt(0).Calculation);
        }

        [TestCase("validConfigurationUnknownForeshoreProfile.xml",
            "Het voorlandprofiel met ID 'unknown' bestaat niet.")]
        [TestCase("validConfigurationUnknownHydraulicBoundaryLocation.xml",
            "De hydraulische belastingenlocatie 'unknown' bestaat niet.")]
        [TestCase("validConfigurationUnknownStructure.xml",
            "Het kunstwerk met ID 'unknown' bestaat niet.")]
        public void Import_ValidConfigurationUnknownData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
        {
            // Setup
            string filePath = Path.Combine(importerPath, file);

            var calculationGroup = new CalculationGroup();

            var importer = new HeightStructuresCalculationConfigurationImporter(filePath,
                                                                                calculationGroup,
                                                                                Enumerable.Empty<HydraulicBoundaryLocation>(),
                                                                                Enumerable.Empty<ForeshoreProfile>(),
                                                                                Enumerable.Empty<HeightStructure>(),
                                                                                new HeightStructuresFailureMechanism());
            var successful = false;

            // Call
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 1);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        private static void AssertCalculation(StructuresCalculation<HeightStructuresInput> expectedCalculation, StructuresCalculation<HeightStructuresInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreSame(expectedCalculation.InputParameters.HydraulicBoundaryLocation, actualCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.AreEqual(expectedCalculation.InputParameters.StructureNormalOrientation, actualCalculation.InputParameters.StructureNormalOrientation);
            Assert.AreSame(expectedCalculation.InputParameters.ForeshoreProfile, actualCalculation.InputParameters.ForeshoreProfile);
            Assert.AreSame(expectedCalculation.InputParameters.Structure, actualCalculation.InputParameters.Structure);
            Assert.AreEqual(expectedCalculation.InputParameters.UseForeshore, actualCalculation.InputParameters.UseForeshore);
            Assert.AreEqual(expectedCalculation.InputParameters.UseBreakWater, actualCalculation.InputParameters.UseBreakWater);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Height, actualCalculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Type, actualCalculation.InputParameters.BreakWater.Type);
            Assert.AreEqual(expectedCalculation.InputParameters.ShouldIllustrationPointsBeCalculated, actualCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.LevelCrestStructure, actualCalculation.InputParameters.LevelCrestStructure);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.AllowedLevelIncreaseStorage, actualCalculation.InputParameters.AllowedLevelIncreaseStorage);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.FlowWidthAtBottomProtection, actualCalculation.InputParameters.FlowWidthAtBottomProtection);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.ModelFactorSuperCriticalFlow, actualCalculation.InputParameters.ModelFactorSuperCriticalFlow);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.CriticalOvertoppingDischarge, actualCalculation.InputParameters.CriticalOvertoppingDischarge);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StorageStructureArea, actualCalculation.InputParameters.StorageStructureArea);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.StormDuration, actualCalculation.InputParameters.StormDuration);
            DistributionAssert.AreEqual(expectedCalculation.InputParameters.WidthFlowApertures, actualCalculation.InputParameters.WidthFlowApertures);
        }
    }
}