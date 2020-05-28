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
using System.Globalization;
using System.IO;
using Components.Persistence.Stability;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Exporters;
using Riskeer.MacroStabilityInwards.IO.TestUtil;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;

namespace Riskeer.MacroStabilityInwards.IO.Test.Exporters
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            var exporter = new MacroStabilityInwardsCalculationExporter(new MacroStabilityInwardsCalculation(), persistenceFactory, "ValidFilePath", AssessmentSectionTestHelper.GetTestAssessmentLevel);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationExporter(null, persistenceFactory, string.Empty, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculation", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_PersistenceFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsCalculationExporter(new MacroStabilityInwardsCalculation(), null, string.Empty, AssessmentSectionTestHelper.GetTestAssessmentLevel);

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
            void Call() => new MacroStabilityInwardsCalculationExporter(new MacroStabilityInwardsCalculation(), persistenceFactory, string.Empty, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getNormativeAssessmentLevelFunc", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_FilePathInvalid_ThrowsArgumentException(string filePath)
        {
            // Setup
            var mocks = new MockRepository();
            var persistenceFactory = mocks.Stub<IPersistenceFactory>();
            mocks.ReplayAll();

            // Call
            void Call() => new MacroStabilityInwardsCalculationExporter(new MacroStabilityInwardsCalculation(), persistenceFactory, filePath, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            // Assert
            Assert.Throws<ArgumentException>(Call);
            mocks.VerifyAll();
        }

        [Test]
        public void Export_PersistenceFactoryThrowsException_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string filePath = "ValidFilePath";
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory
            {
                ThrowException = true
            };

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, persistenceFactory, filePath, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            // Call
            var exportResult = true;
            void Call() => exportResult = exporter.Export();

            // Assert
            string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. Er is geen D-GEO Suite Stability Project geëxporteerd.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
            Assert.IsFalse(exportResult);
        }

        [Test]
        public void Export_PersistenceFactoryThrowsException_NoFileWritten()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationExporterTest)}.{nameof(Export_PersistenceFactoryThrowsException_NoFileWritten)}.ValidFile.stix");
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory
            {
                ThrowException = true,
                WriteFile = true
            };

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, persistenceFactory, filePath, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            // Call
            exporter.Export();

            // Assert
            Assert.IsFalse(File.Exists(filePath));
        }

        [Test]
        public void Export_RunsSuccessful_SetsDataCorrectlyAndReturnsTrue()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationExporterTest)}.{nameof(Export_RunsSuccessful_SetsDataCorrectlyAndReturnsTrue)}.ValidFile.stix");
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            var persistenceFactory = new MacroStabilityInwardsTestPersistenceFactory
            {
                WriteFile = true
            };

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, persistenceFactory, filePath, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    PersistableDataModelTestHelper.AssertPersistableDataModel(calculation, filePath, persistenceFactory.PersistableDataModel);
                    Assert.AreEqual(filePath, persistenceFactory.FilePath);

                    var persister = (MacroStabilityInwardsTestPersister) persistenceFactory.CreatedPersister;
                    Assert.IsTrue(persister.PersistCalled);

                    Assert.IsTrue(exportResult);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Export_RunsSuccessful_WritesFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationExporterTest)}.{nameof(Export_RunsSuccessful_WritesFile)}.ValidFile.stix");
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, new PersistenceFactory(), filePath, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    bool exportResult = exporter.Export();

                    // Assert
                    Assert.IsTrue(exportResult);
                    Assert.IsTrue(File.Exists(filePath));
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        [TestCase(1.01)]
        [TestCase(0.99)]
        public void Export_MaximumSliceWidthNotOne_LogsWarningAndReturnsTrue(double maximumSliceWidth)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationExporterTest)}.{nameof(Export_MaximumSliceWidthNotOne_LogsWarningAndReturnsTrue)}.ValidFile.stix");

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.MaximumSliceWidth = (RoundedDouble) maximumSliceWidth;
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, new PersistenceFactory(), filePath, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    string expectedMessage = $"De berekening bevat een lamelbreedte van {calculation.InputParameters.MaximumSliceWidth.ToString(null, CultureInfo.CurrentCulture)} meter. D-GEO Suite Stability ondersteunt enkel een maximale lamelbreedte van 1 meter. Er wordt daarom een lamelbreedte van 1 meter geëxporteerd.";
                    TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Warn));
                    Assert.IsTrue(exportResult);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Export_SoilProfileWithMultipleAquiferLayers_LogsWarningAndReturnsTrue()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationExporterTest)}.{nameof(Export_MaximumSliceWidthNotOne_LogsWarningAndReturnsTrue)}.ValidFile.stix");

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.InputParameters.StochasticSoilProfile.SoilProfile.Layers.ForEachElementDo(layer => layer.Data.IsAquifer = true);
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, new PersistenceFactory(), filePath, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            try
            {
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    const string expectedMessage = "De schematisatie van de berekening bevat meerdere aquifer lagen. De volgorde van de aquifer lagen kan niet bepaald worden tijdens exporteren. Er worden daarom geen lagen als aquifer geëxporteerd.";
                    TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Warn));
                    Assert.IsTrue(exportResult);
                }
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        [Test]
        public void Export_ErrorDuringCalculationExport_LogsErrorAndReturnsFalse()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationExporterTest)}.{nameof(Export_ErrorDuringCalculationExport_LogsErrorAndReturnsFalse)}");
            Directory.CreateDirectory(folderPath);

            string filePath = Path.Combine(folderPath, "ValidFile.stix");

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

            var exporter = new MacroStabilityInwardsCalculationExporter(calculation, new PersistenceFactory(), filePath, AssessmentSectionTestHelper.GetTestAssessmentLevel);

            try
            {
                using (var fileDisposeHelper = new FileDisposeHelper(filePath))
                using (new MacroStabilityInwardsCalculatorFactoryConfig())
                {
                    fileDisposeHelper.LockFiles();

                    // Call
                    var exportResult = false;
                    void Call() => exportResult = exporter.Export();

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. Er is geen D-GEO Suite Stability Project geëxporteerd.";
                    TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                    Assert.IsFalse(exportResult);
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }
    }
}