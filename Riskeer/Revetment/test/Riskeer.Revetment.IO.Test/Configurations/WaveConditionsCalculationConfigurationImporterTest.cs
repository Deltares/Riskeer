// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Xml.Linq;
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
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.IO.Configurations;

namespace Riskeer.Revetment.IO.Test.Configurations
{
    [TestFixture]
    public class WaveConditionsCalculationConfigurationImporterTest
    {
        private readonly string path = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Revetment.IO, "WaveConditionsCalculationConfigurationImporter");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var importer = new TestWaveConditionsCalculationConfigurationImporter(
                "",
                new CalculationGroup(),
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Assert
            Assert.IsInstanceOf<CalculationConfigurationImporter<TestWaveConditionsCalculationConfigurationReader, WaveConditionsCalculationConfiguration>>(importer);
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new TestWaveConditionsCalculationConfigurationImporter(
                "", new CalculationGroup(), null, Enumerable.Empty<ForeshoreProfile>(),
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_ForeshoreProfilesNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => new TestWaveConditionsCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(), null,
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("foreshoreProfiles", exception.ParamName);
        }

        [Test]
        public void Constructor_FailureMechanismContributionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestWaveConditionsCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(), null,
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanismContribution", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationsForTargetProbabilitiesNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new TestWaveConditionsCalculationConfigurationImporter(
                "", new CalculationGroup(), Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(), new FailureMechanismContribution(0.1, 0.1), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationsForTargetProbabilities", exception.ParamName);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase("validConfigurationUpperRevetmentBoundBelowLowerRevetmentBound.xml",
                  "Een waarde van '2,2' als ondergrens bekledingen is ongeldig. De bovengrens van de bekleding moet boven de ondergrens liggen.")]
        [TestCase("validConfigurationUpperWaterLevelBoundBelowLowerWaterLevelBound.xml",
                  "Een waarde van '2,2' als ondergrens van de waterstanden is ongeldig. De bovengrens van de waterstanden moet boven de ondergrens liggen.")]
        [TestCase("validConfigurationOrientationOutOfRange.xml",
                  "Een waarde van '380' als oriëntatie is ongeldig. De waarde voor de oriëntatie moet in het bereik [0,00, 360,00] liggen.")]
        public void Import_ValidConfigurationInvalidData_LogMessageAndContinueImport(string file, string expectedErrorMessage)
        {
            // Setup
            string filePath = Path.Combine(path, file);

            var calculationGroup = new CalculationGroup();
            var importer = new TestWaveConditionsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            string expectedMessage = $"{expectedErrorMessage} Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_HydraulicBoundaryLocationUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationContainingUnknownHydraulicBoundaryLocation.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new TestWaveConditionsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De hydraulische belastingenlocatie 'Locatie' bestaat niet. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_ForeshoreProfileUnknown_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationContainingUnknownForeshoreProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new TestWaveConditionsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het voorlandprofiel met ID 'Voorlandprofiel' bestaat niet. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_WaveReductionWithoutForeshoreProfile_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationContainingWaveReductionWithoutForeshoreProfile.xml");

            var calculationGroup = new CalculationGroup();
            var importer = new TestWaveConditionsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                Enumerable.Empty<ForeshoreProfile>(),
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Er is geen voorlandprofiel opgegeven om golfreductie parameters aan toe te voegen. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_UseForeshoreTrueButProfileWithoutForeshoreGeometry_LogMessageAndContinueImport()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationUseForeshoreTrueForeshoreProfileWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var foreshoreProfile = new TestForeshoreProfile("Voorlandprofiel");
            var importer = new TestWaveConditionsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    foreshoreProfile
                },
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "Het opgegeven voorlandprofiel 'Voorlandprofiel' heeft geen voorlandgeometrie en kan daarom niet gebruikt worden. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        public void Import_UseForeshoreFalseButProfileWithoutForeshoreGeometry_DataAddedToModel()
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationCalculationUseForeshoreFalseForeshoreProfileWithoutGeometry.xml");

            var calculationGroup = new CalculationGroup();
            var foreshoreProfile = new TestForeshoreProfile("Voorlandprofiel");
            var importer = new TestWaveConditionsCalculationConfigurationImporter(
                filePath,
                calculationGroup,
                Enumerable.Empty<HydraulicBoundaryLocation>(),
                new[]
                {
                    foreshoreProfile
                },
                new FailureMechanismContribution(0.1, 0.1),
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Call
            bool successful = importer.Import();

            // Assert
            Assert.IsTrue(successful);

            var expectedCalculation = new TestTargetTestWaveConditionsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    UseForeshore = false,
                    Orientation = (RoundedDouble) 0,
                    ForeshoreProfile = foreshoreProfile,
                    WaterLevelType = WaveConditionsInputWaterLevelType.LowerLimit
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertWaveConditionsCalculation(expectedCalculation, (ICalculation<WaveConditionsInput>) calculationGroup.Children[0]);
        }

        [Test]
        [TestCase(NormType.LowerLimit, WaveConditionsInputWaterLevelType.LowerLimit)]
        [TestCase(NormType.Signaling, WaveConditionsInputWaterLevelType.Signaling)]
        public void Import_TargetProbabilityNull_DataAddedToModel(NormType normType, WaveConditionsInputWaterLevelType expectedWaterLevelType)
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationWithoutTargetProbability.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Locatie", 10, 20);
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, new BreakWater(BreakWaterType.Caisson, 0), new ForeshoreProfile.ConstructionProperties
            {
                Id = "Voorlandprofiel",
                Name = "VoorlandProfielName"
            });
            var failureMechanismContribution = new FailureMechanismContribution(0.1, 0.1)
            {
                NormativeNorm = normType
            };

            var importer = new TestWaveConditionsCalculationConfigurationImporter(
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
                failureMechanismContribution,
                Enumerable.Empty<HydraulicBoundaryLocationCalculationsForTargetProbability>());

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
            Assert.IsTrue(successful);

            var expectedCalculation = new TestTargetTestWaveConditionsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    WaterLevelType = expectedWaterLevelType,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    LowerBoundaryRevetment = (RoundedDouble) 2,
                    UpperBoundaryWaterLevels = (RoundedDouble) 9,
                    LowerBoundaryWaterLevels = (RoundedDouble) 4,
                    StepSize = WaveConditionsInputStepSize.Half,
                    ForeshoreProfile = foreshoreProfile,
                    Orientation = (RoundedDouble) 5.5,
                    UseForeshore = false,
                    UseBreakWater = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 6.6,
                        Type = BreakWaterType.Caisson
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertWaveConditionsCalculation(expectedCalculation, (ICalculation<WaveConditionsInput>) calculationGroup.Children[0]);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidTargetProbabilityData))]
        public void GivenDataModelWithUnmatchableTargetProbabilities_WhenImporting_ThenLogMessageAndContinueImport(
            FailureMechanismContribution failureMechanismContribution,
            IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> calculationsForTargetProbabilities)
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculation.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Locatie", 10, 20);
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, new BreakWater(BreakWaterType.Caisson, 0), new ForeshoreProfile.ConstructionProperties
            {
                Id = "Voorlandprofiel",
                Name = "VoorlandProfielName"
            });

            var importer = new TestWaveConditionsCalculationConfigurationImporter(
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
                failureMechanismContribution,
                calculationsForTargetProbabilities);

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            const string expectedMessage = "De doelkans '0.01' kan niet worden gevonden en kan daarom niet gebruikt worden. Berekening 'Berekening 1' is overgeslagen.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Error), 2);
            Assert.IsTrue(successful);
            CollectionAssert.IsEmpty(calculationGroup.Children);
        }

        [Test]
        [TestCaseSource(nameof(GetValidTargetProbabilityData))]
        public void Import_ValidConfigurationWithValidData_DataAddedToModel(FailureMechanismContribution failureMechanismContribution,
                                                                            HydraulicBoundaryLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                                                            WaveConditionsInputWaterLevelType expectedWaterLevelType)
        {
            // Setup
            string filePath = Path.Combine(path, "validConfigurationFullCalculation.xml");

            var calculationGroup = new CalculationGroup();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "Locatie", 10, 20);
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0), new[]
            {
                new Point2D(0, 0),
                new Point2D(1, 1),
                new Point2D(2, 2)
            }, new BreakWater(BreakWaterType.Caisson, 0), new ForeshoreProfile.ConstructionProperties
            {
                Id = "Voorlandprofiel",
                Name = "VoorlandProfielName"
            });

            var importer = new TestWaveConditionsCalculationConfigurationImporter(
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
                failureMechanismContribution,
                new[]
                {
                    calculationsForTargetProbability
                });

            // Call
            var successful = false;
            Action call = () => successful = importer.Import();

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, $"Gegevens zijn geïmporteerd vanuit bestand '{filePath}'.", 1);
            Assert.IsTrue(successful);

            var expectedCalculation = new TestTargetTestWaveConditionsCalculation
            {
                Name = "Berekening 1",
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CalculationsTargetProbability = expectedWaterLevelType == WaveConditionsInputWaterLevelType.UserDefinedTargetProbability
                                                        ? calculationsForTargetProbability
                                                        : null,
                    WaterLevelType = expectedWaterLevelType,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    LowerBoundaryRevetment = (RoundedDouble) 2,
                    UpperBoundaryWaterLevels = (RoundedDouble) 9,
                    LowerBoundaryWaterLevels = (RoundedDouble) 4,
                    StepSize = WaveConditionsInputStepSize.Half,
                    ForeshoreProfile = foreshoreProfile,
                    Orientation = (RoundedDouble) 5.5,
                    UseForeshore = false,
                    UseBreakWater = true,
                    BreakWater =
                    {
                        Height = (RoundedDouble) 6.6,
                        Type = BreakWaterType.Caisson
                    }
                }
            };

            Assert.AreEqual(1, calculationGroup.Children.Count);
            AssertWaveConditionsCalculation(expectedCalculation, (ICalculation<WaveConditionsInput>) calculationGroup.Children[0]);
        }

        private static IEnumerable<TestCaseData> GetValidTargetProbabilityData()
        {
            yield return new TestCaseData(
                new FailureMechanismContribution(0.01, 0.005),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.05),
                WaveConditionsInputWaterLevelType.LowerLimit);
            yield return new TestCaseData(
                new FailureMechanismContribution(0.02, 0.01),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.05),
                WaveConditionsInputWaterLevelType.Signaling);
            yield return new TestCaseData(
                new FailureMechanismContribution(0.02, 0.005),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01),
                WaveConditionsInputWaterLevelType.UserDefinedTargetProbability);
        }

        private static IEnumerable<TestCaseData> GetInvalidTargetProbabilityData()
        {
            yield return new TestCaseData(new FailureMechanismContribution(0.02, 0.005), new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.05)
            });
            yield return new TestCaseData(new FailureMechanismContribution(0.01, 0.01), new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.05)
            });
            yield return new TestCaseData(new FailureMechanismContribution(0.01, 0.005), new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
            });
            yield return new TestCaseData(new FailureMechanismContribution(0.02, 0.01), new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
            });
            yield return new TestCaseData(new FailureMechanismContribution(0.02, 0.005), new[]
            {
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01),
                new HydraulicBoundaryLocationCalculationsForTargetProbability(0.01)
            });
        }

        private static void AssertWaveConditionsCalculation(ICalculation<WaveConditionsInput> expectedCalculation, ICalculation<WaveConditionsInput> actualCalculation)
        {
            Assert.AreEqual(expectedCalculation.Name, actualCalculation.Name);
            Assert.AreSame(expectedCalculation.InputParameters.HydraulicBoundaryLocation, actualCalculation.InputParameters.HydraulicBoundaryLocation);
            Assert.AreSame(expectedCalculation.InputParameters.CalculationsTargetProbability, actualCalculation.InputParameters.CalculationsTargetProbability);
            Assert.AreEqual(expectedCalculation.InputParameters.WaterLevelType, actualCalculation.InputParameters.WaterLevelType);
            Assert.AreEqual(expectedCalculation.InputParameters.UpperBoundaryRevetment, actualCalculation.InputParameters.UpperBoundaryRevetment);
            Assert.AreEqual(expectedCalculation.InputParameters.LowerBoundaryRevetment, actualCalculation.InputParameters.LowerBoundaryRevetment);
            Assert.AreEqual(expectedCalculation.InputParameters.UpperBoundaryWaterLevels, actualCalculation.InputParameters.UpperBoundaryWaterLevels);
            Assert.AreEqual(expectedCalculation.InputParameters.LowerBoundaryWaterLevels, actualCalculation.InputParameters.LowerBoundaryWaterLevels);
            Assert.AreEqual(expectedCalculation.InputParameters.StepSize, actualCalculation.InputParameters.StepSize);
            Assert.AreEqual(expectedCalculation.InputParameters.Orientation, actualCalculation.InputParameters.Orientation);
            Assert.AreEqual(expectedCalculation.InputParameters.ForeshoreProfile, actualCalculation.InputParameters.ForeshoreProfile);
            Assert.AreEqual(expectedCalculation.InputParameters.UseForeshore, actualCalculation.InputParameters.UseForeshore);
            Assert.AreEqual(expectedCalculation.InputParameters.UseBreakWater, actualCalculation.InputParameters.UseBreakWater);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Height, actualCalculation.InputParameters.BreakWater.Height);
            Assert.AreEqual(expectedCalculation.InputParameters.BreakWater.Type, actualCalculation.InputParameters.BreakWater.Type);
        }

        private class TestWaveConditionsCalculationConfigurationImporter
            : WaveConditionsCalculationConfigurationImporter<TestTargetTestWaveConditionsCalculation, TestWaveConditionsCalculationConfigurationReader, WaveConditionsCalculationConfiguration>
        {
            public TestWaveConditionsCalculationConfigurationImporter(string xmlFilePath,
                                                                      CalculationGroup importTarget,
                                                                      IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                      IEnumerable<ForeshoreProfile> foreshoreProfiles,
                                                                      FailureMechanismContribution failureMechanismContribution,
                                                                      IEnumerable<HydraulicBoundaryLocationCalculationsForTargetProbability> calculationsForTargetProbabilities)
                : base(xmlFilePath, importTarget, hydraulicBoundaryLocations, foreshoreProfiles, failureMechanismContribution, calculationsForTargetProbabilities) {}

            protected override TestWaveConditionsCalculationConfigurationReader CreateCalculationConfigurationReader(
                string xmlFilePath)
            {
                return new TestWaveConditionsCalculationConfigurationReader(xmlFilePath);
            }
        }

        private class TestTargetTestWaveConditionsCalculation : TestWaveConditionsCalculation<TestWaveConditionsInput>
        {
            public TestTargetTestWaveConditionsCalculation() : base(new TestWaveConditionsInput()) {}
        }

        private class TestWaveConditionsCalculationConfigurationReader : WaveConditionsCalculationConfigurationReader<WaveConditionsCalculationConfiguration>
        {
            private static readonly string testDirectoryPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Revetment.IO,
                                                                                          nameof(WaveConditionsCalculationConfigurationImporter<TestTargetTestWaveConditionsCalculation, WaveConditionsCalculationConfigurationReader<WaveConditionsCalculationConfiguration>, WaveConditionsCalculationConfiguration>));

            public TestWaveConditionsCalculationConfigurationReader(string xmlFilePath)
                : base(xmlFilePath, new[]
                {
                    File.ReadAllText(Path.Combine(testDirectoryPath, "validConfigurationSchema_0.xsd")),
                    File.ReadAllText(Path.Combine(testDirectoryPath, "validConfigurationSchema_1.xsd")),
                    File.ReadAllText(Path.Combine(testDirectoryPath, "validConfigurationSchema.xsd"))
                }) {}

            protected override WaveConditionsCalculationConfiguration ParseCalculationElement(XElement calculationElement)
            {
                var configuration = new WaveConditionsCalculationConfiguration("Berekening 1");
                ParseCalculationElementData(calculationElement, configuration);
                return configuration;
            }
        }
    }
}