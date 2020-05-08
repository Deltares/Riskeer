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

            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

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
        public void Export_RunsSuccessful_WritesFilesAndRemovesTempFile()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath($"{nameof(MacroStabilityInwardsCalculationGroupExporterTest)}.{nameof(Export_RunsSuccessful_WritesFilesAndRemovesTempFile)}");
            Directory.CreateDirectory(folderPath);

            MacroStabilityInwardsCalculationScenario calculation1 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation1.Name = "Calculation1";
            calculation1.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();
            MacroStabilityInwardsCalculationScenario calculation2 = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(new TestHydraulicBoundaryLocation());
            calculation2.Name = "Calculation2";
            calculation2.Output = MacroStabilityInwardsOutputTestFactory.CreateRandomOutput();

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
                    string filePath1 = Path.Combine(folderPath, $"{calculation1.Name}.{fileExtension}");
                    Assert.IsTrue(File.Exists(filePath1));
                    Assert.IsFalse(File.Exists($"{filePath1}.temp"));

                    string filePath2 = Path.Combine(folderPath, $"{calculation2.Name}.{fileExtension}");
                    Assert.IsTrue(File.Exists(filePath2));
                    Assert.IsFalse(File.Exists($"{filePath2}.temp"));
                }
            }
            finally
            {
                Directory.Delete(folderPath, true);
            }
        }
    }
}