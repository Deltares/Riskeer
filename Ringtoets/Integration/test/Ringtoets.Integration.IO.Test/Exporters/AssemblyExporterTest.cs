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
using System.IO;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Exporters;

namespace Ringtoets.Integration.IO.Test.Exporters
{
    [TestFixture]
    public class AssemblyExporterTest
    {
        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.gml"));

            // Call
            TestDelegate call = () => new AssemblyExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentException()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            TestDelegate call = () => new AssemblyExporter(assessmentSection, null);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void Constructor_WithValidArguments_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.gml"));

            // Call
            var exporter = new AssemblyExporter(assessmentSection, filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Export_CalculatorThrowsAssemblyException_LogsErrorAndReturnsFalse()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_CalculatorThrowsAssemblyException_LogsErrorAndReturnsFalse));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionAssemblyCalculator.ThrowExceptionOnCalculate = true;

                // Call
                var isExported = true;
                Action call = () => isExported = exporter.Export();

                // Assert
                const string expectedMessage = "Het is alleen mogelijk een volledig assemblageresultaat te exporteren.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(isExported);
            }
        }

        [Test]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.None)]
        [TestCase(AssessmentSectionAssemblyCategoryGroup.NotApplicable)]
        public void Export_InvalidAssessmentSectionCategoryGroupResults_LogsErrorAndReturnsFalse(AssessmentSectionAssemblyCategoryGroup invalidAssessmentSectionAssemblyCategoryGroup)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_InvalidAssessmentSectionCategoryGroupResults_LogsErrorAndReturnsFalse));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionAssemblyCalculator.AssembleAssessmentSectionCategoryGroupOutput = invalidAssessmentSectionAssemblyCategoryGroup;

                // Call
                var isExported = true;
                Action call = () => isExported = exporter.Export();

                // Assert
                const string expectedMessage = "Het is alleen mogelijk een volledig assemblageresultaat te exporteren.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(isExported);
            }
        }

        [Test]
        public void Export_FullyConfiguredAssessmentSectionAndValidAssemblyResults_ReturnsTrueAndCreatesFile()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_InvalidAssessmentSectionCategoryGroupResults_LogsErrorAndReturnsFalse));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionAssemblyCalculator.AssembleAssessmentSectionCategoryGroupOutput = AssessmentSectionAssemblyCategoryGroup.A;

                // Call
                bool isExported = exporter.Export();

                // Assert
                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(isExported);

                string expectedGmlFilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.IO),
                                                          nameof(AssemblyExporter), "ExpectedGml.gml");
                string expectedGml = File.ReadAllText(expectedGmlFilePath);
                string actualGml = File.ReadAllText(filePath);
                Assert.AreEqual(expectedGml, actualGml);
            }
        }

        [Test]
        public void Export_InvalidDirectorRights_LogsErrorAndReturnsFalse()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectorRights_LogsErrorAndReturnsFalse));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (var fileDisposeHelper = new FileDisposeHelper(filePath))
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                fileDisposeHelper.LockFiles();

                // Call
                var isExported = true;
                Action call = () => isExported = exporter.Export();

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. " +
                                         "Er zijn geen assemblageresultaten geëxporteerd.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(isExported);
            }
        }

        private static AssessmentSection CreateConfiguredAssessmentSection()
        {
            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            });

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = "assessmentSectionName",
                Id = "assessmentSectionId",
                ReferenceLine = referenceLine
            };

            FailureMechanismTestHelper.AddSections(assessmentSection.Piping, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityInwards, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionInwards, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.HeightStructures, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.ClosingStructures, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityPointStructures, 2);

            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityStoneCover, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.WaveImpactAsphaltCover, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionOutwards, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.DuneErosion, 2);

            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityOutwards, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.Microstability, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffOutwards, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffInwards, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.PipingStructure, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.WaterPressureAsphaltCover, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.StrengthStabilityLengthwiseConstruction, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.TechnicalInnovation, 2);

            return assessmentSection;
        }
    }
}