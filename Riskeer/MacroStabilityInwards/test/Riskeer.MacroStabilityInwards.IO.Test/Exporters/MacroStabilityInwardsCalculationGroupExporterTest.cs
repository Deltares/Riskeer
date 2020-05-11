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
using System.IO;
using Components.Persistence.Stability;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Exporters;
using Riskeer.MacroStabilityInwards.IO.TestUtil;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;

namespace Riskeer.MacroStabilityInwards.IO.Test.Exporters
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationGroupExporterTest
    {
        private const string fileExtension = "stix";

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            var exporter = new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), persistenceFactory, "ValidFolderPath", "extension", c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(null, persistenceFactory, string.Empty, string.Empty, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PersistenceFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), null, string.Empty, string.Empty, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("persistenceFactory", exception.ParamName);
        }

        [Test]
        public void Constructor_GetAssessmentLevelFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), persistenceFactory, string.Empty, string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getNormativeAssessmentLevelFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("C:\\Not:Valid")]
        public void Constructor_InvalidFolderPath_ThrowsArgumentException(string folderPath)
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), persistenceFactory, folderPath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            // Assert
            Assert.Throws<ArgumentException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        public void Export_CalculationExporterReturnsFalse_ReturnsFalse()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationExporterReturnsFalse_ReturnsFalse)}");
            Directory.CreateDirectory(folderPath);

            MacroStabilityInwardsCalculationScenario calculation = CreateCalculation("calculation");

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation);

            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory
            {
                ThrowException = true
            };

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, persistenceFactory, folderPath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                // Call
                var exportResult = true;
                void Call() => exportResult = exporter.Export();

                // Assert
                string filePath = Path.Combine(folderPath, $"{calculation.Name}.stix");
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. Er is geen D-GEO Suite Stability Project geëxporteerd.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(exportResult);
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_CalculationGroupWithOnlyCalculations_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupWithOnlyCalculations_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation1);
            calculationGroup.Children.Add(calculation2);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, new PersistenceFactory(), folderPath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    AssertCalculationExists(Path.Combine(folderPath, $"{calculation1.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, $"{calculation2.Name}.{fileExtension}"));
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_CalculationsWithoutOutput_LogWarningsAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationsWithoutOutput_LogWarningsAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1", false);
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2", false);

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation1);
            calculationGroup.Children.Add(calculation2);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, new PersistenceFactory(), folderPath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    TestHelper.AssertLogMessagesWithLevelAreGenerated(Call, new[]
                    {
                        new Tuple<string, LogLevelConstant>($"Berekening '{calculation1.Name}' heeft geen uitvoer. Deze berekening wordt overgeslagen.", LogLevelConstant.Warn),
                        new Tuple<string, LogLevelConstant>($"Berekening '{calculation2.Name}' heeft geen uitvoer. Deze berekening wordt overgeslagen.", LogLevelConstant.Warn)
                    });
                    Assert.IsTrue(exportResult);
                    Assert.IsFalse(File.Exists(Path.Combine(folderPath, $"{calculation1.Name}.{fileExtension}")));
                    Assert.IsFalse(File.Exists(Path.Combine(folderPath, $"{calculation2.Name}.{fileExtension}")));
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_CalculationGroupWithNestedGroupsAndCalculations_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupWithNestedGroupsAndCalculations_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");
            MacroStabilityInwardsCalculationScenario calculation3 = CreateCalculation("calculation3");
            MacroStabilityInwardsCalculationScenario calculation4 = CreateCalculation("calculation4");

            var rootCalculationGroup = new CalculationGroup();
            var nestedGroup1 = new CalculationGroup
            {
                Name = "NestedGroup1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "NestedGroup2"
            };
            nestedGroup2.Children.Add(calculation4);
            nestedGroup1.Children.Add(calculation3);
            nestedGroup1.Children.Add(nestedGroup2);
            rootCalculationGroup.Children.Add(calculation1);
            rootCalculationGroup.Children.Add(calculation2);
            rootCalculationGroup.Children.Add(nestedGroup1);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootCalculationGroup, new PersistenceFactory(), folderPath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    AssertCalculationExists(Path.Combine(folderPath, $"{calculation1.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, $"{calculation2.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, nestedGroup1.Name, $"{calculation3.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, nestedGroup1.Name, nestedGroup2.Name, $"{calculation4.Name}.{fileExtension}"));
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_CalculationGroupWithCalculationsWithSameName_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupWithCalculationsWithSameName_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");
            MacroStabilityInwardsCalculationScenario calculation3 = CreateCalculation("calculation1");

            var calculationGroup = new CalculationGroup();
            calculationGroup.Children.Add(calculation1);
            calculationGroup.Children.Add(calculation2);
            calculationGroup.Children.Add(calculation3);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(calculationGroup, new PersistenceFactory(), folderPath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    AssertCalculationExists(Path.Combine(folderPath, $"{calculation1.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, $"{calculation2.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, $"{calculation3.Name} (1).{fileExtension}"));
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_CalculationGroupsWithSameName_WritesFilesAndReturnsTrue()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_CalculationGroupsWithSameName_WritesFilesAndReturnsTrue)}");
            Directory.CreateDirectory(folderPath);

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");
            MacroStabilityInwardsCalculationScenario calculation3 = CreateCalculation("calculation1");

            var rootGroup = new CalculationGroup
            {
                Name = "root"
            };
            var nestedGroup1 = new CalculationGroup
            {
                Name = "group1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "group1"
            };
            nestedGroup1.Children.Add(calculation1);
            nestedGroup1.Children.Add(calculation2);
            nestedGroup2.Children.Add(calculation3);
            rootGroup.Children.Add(nestedGroup1);
            rootGroup.Children.Add(nestedGroup2);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootGroup, new PersistenceFactory(), folderPath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    AssertCalculationExists(Path.Combine(folderPath, nestedGroup1.Name, $"{calculation1.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, nestedGroup1.Name, $"{calculation2.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, $"{nestedGroup2.Name} (1)", $"{calculation3.Name}.{fileExtension}"));
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        [Test]
        public void Export_ErrorDuringSingleCalculationExport_LogsErrorAndReturnsFalse()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_ErrorDuringSingleCalculationExport_LogsErrorAndReturnsFalse)}");
            Directory.CreateDirectory(folderPath);

            MacroStabilityInwardsCalculationScenario calculation1 = CreateCalculation("calculation1");
            MacroStabilityInwardsCalculationScenario calculation2 = CreateCalculation("calculation2");
            MacroStabilityInwardsCalculationScenario calculation3 = CreateCalculation("calculation3");
            MacroStabilityInwardsCalculationScenario calculation4 = CreateCalculation("calculation4");

            var rootGroup = new CalculationGroup
            {
                Name = "root"
            };
            var nestedGroup1 = new CalculationGroup
            {
                Name = "group1"
            };
            var nestedGroup2 = new CalculationGroup
            {
                Name = "group2"
            };
            nestedGroup1.Children.Add(calculation1);
            nestedGroup1.Children.Add(calculation2);
            nestedGroup2.Children.Add(calculation3);
            nestedGroup2.Children.Add(calculation4);
            rootGroup.Children.Add(nestedGroup1);
            rootGroup.Children.Add(nestedGroup2);

            var exporter = new MacroStabilityInwardsCalculationGroupExporter(rootGroup, new PersistenceFactory(), folderPath, fileExtension, c => AssessmentSectionTestHelper.GetTestAssessmentLevel());
            Directory.CreateDirectory(Path.Combine(folderPath, nestedGroup2.Name));
            string lockedCalculationFilePath = Path.Combine(folderPath, nestedGroup2.Name, $"{calculation3.Name}.{fileExtension}");

            try
            {
                using (var fileDisposeHelper = new FileDisposeHelper(lockedCalculationFilePath))
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    fileDisposeHelper.LockFiles();

                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{lockedCalculationFilePath}'. Er is geen D-GEO Suite Stability Project geëxporteerd.";
                    TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                    Assert.IsFalse(exportResult);
                    AssertCalculationExists(Path.Combine(folderPath, nestedGroup1.Name, $"{calculation1.Name}.{fileExtension}"));
                    AssertCalculationExists(Path.Combine(folderPath, nestedGroup1.Name, $"{calculation2.Name}.{fileExtension}"));
                    Assert.IsFalse(File.Exists(Path.Combine(folderPath, nestedGroup2.Name, $"{calculation4.Name}.{fileExtension}")));
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }

        private static void AssertCalculationExists(string calculationPath)
        {
            Assert.IsTrue(File.Exists(calculationPath));
            Assert.IsFalse(File.Exists($"{calculationPath}.temp"));
        }

        private static MacroStabilityInwardsCalculationScenario CreateCalculation(string calculationName, bool setOutput = true)
        {
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Name = calculationName;
            if (setOutput)
            {
                calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();
            }

            return calculation;
        }
    }
}