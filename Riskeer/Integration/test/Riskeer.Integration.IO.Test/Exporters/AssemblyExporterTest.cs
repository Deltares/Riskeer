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
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.IO.Exporters;

namespace Riskeer.Integration.IO.Test.Exporters
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

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionAssemblyCalculator.ThrowExceptionOnCalculate = true;

                // Call
                var isExported = true;
                Action call = () => isExported = exporter.Export();

                // Assert
                const string expectedMessage = "Om een toetsoordeel te kunnen exporteren moet voor alle vakken een resultaat zijn gespecificeerd.";
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

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionAssemblyCalculator.AssembleAssessmentSectionCategoryGroupOutput = invalidAssessmentSectionAssemblyCategoryGroup;

                // Call
                var isExported = true;
                Action call = () => isExported = exporter.Export();

                // Assert
                const string expectedMessage = "Om een toetsoordeel te kunnen exporteren moet voor alle vakken een resultaat zijn gespecificeerd.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(isExported);
            }
        }

        [Test]
        public void Export_WithManualAssemblyResult_LogsWarning()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_WithManualAssemblyResult_LogsWarning));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();
            assessmentSection.Piping.SectionResultsOld.First().UseManualAssembly = true;

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (new FileDisposeHelper(filePath))
            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                // Call
                Action call = () => exporter.Export();

                // Assert
                const string expectedMessage = "Veiligheidsoordeel is (deels) gebaseerd op handmatig ingevoerde toetsoordelen. Tijdens het exporteren worden handmatig ingevoerde toetsoordelen genegeerd.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Warn));
            }
        }

        [Test]
        public void Export_AssemblyCreatorExceptionThrown_LogsErrorAndReturnsFalse()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_AssemblyCreatorExceptionThrown_LogsErrorAndReturnsFalse));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                failureMechanismSectionAssemblyCalculator.CombinedAssemblyCategoryOutput = FailureMechanismSectionAssemblyCategoryGroup.None;

                // Call
                var isExported = true;
                Action call = () => isExported = exporter.Export();

                // Assert
                const string expectedMessage = "Om een toetsoordeel te kunnen exporteren moet voor alle vakken een resultaat zijn gespecificeerd.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(isExported);
            }
        }

        // [Test]
        // public void Export_FullyConfiguredAssessmentSectionAndValidAssemblyResults_ReturnsTrueAndCreatesFile()
        // {
        //     // Setup
        //     string filePath = TestHelper.GetScratchPadPath(nameof(Export_FullyConfiguredAssessmentSectionAndValidAssemblyResults_ReturnsTrueAndCreatesFile));
        //     AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();
        //
        //     var exporter = new AssemblyExporter(assessmentSection, filePath);
        //
        //     using (new FileDisposeHelper(filePath))
        //     using (new AssemblyToolCalculatorFactoryConfigOld())
        //     {
        //         var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
        //         AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
        //         assessmentSectionAssemblyCalculator.AssembleAssessmentSectionCategoryGroupOutput = AssessmentSectionAssemblyCategoryGroup.A;
        //
        //         // Call
        //         bool isExported = exporter.Export();
        //
        //         // Assert
        //         Assert.IsTrue(File.Exists(filePath));
        //         Assert.IsTrue(isExported);
        //
        //         string expectedGmlFilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
        //                                                   nameof(AssemblyExporter), "ExpectedGml.gml");
        //         string expectedGml = File.ReadAllText(expectedGmlFilePath);
        //         string actualGml = File.ReadAllText(filePath);
        //         Assert.AreEqual(expectedGml, actualGml);
        //     }
        // }
        //
        // [Test]
        // public void Export_InvalidDirectoryRights_LogsErrorAndReturnsFalse()
        // {
        //     // Setup
        //     string filePath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogsErrorAndReturnsFalse));
        //     AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();
        //
        //     var exporter = new AssemblyExporter(assessmentSection, filePath);
        //
        //     using (var fileDisposeHelper = new FileDisposeHelper(filePath))
        //     using (new AssemblyToolCalculatorFactoryConfigOld())
        //     {
        //         fileDisposeHelper.LockFiles();
        //
        //         // Call
        //         var isExported = true;
        //         Action call = () => isExported = exporter.Export();
        //
        //         // Assert
        //         string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. " +
        //                                  "Er is geen toetsoordeel geëxporteerd.";
        //         TestHelper.AssertLogMessageWithLevelIsGenerated(call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
        //         Assert.IsFalse(isExported);
        //     }
        // }

        private static AssessmentSection CreateConfiguredAssessmentSection()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = "assessmentSectionName",
                Id = "assessmentSectionId"
            };
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            });

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

            FailureMechanismTestHelper.AddSections(assessmentSection.Microstability, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffOutwards, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffInwards, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.PipingStructure, 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.WaterPressureAsphaltCover, 2);

            return assessmentSection;
        }
    }
}