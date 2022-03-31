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
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
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
            void Call() => new AssemblyExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Constructor_FilePathNull_ThrowsArgumentException()
        {
            // Setup
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());

            // Call
            void Call() => new AssemblyExporter(assessmentSection, null);

            // Assert
            Assert.Throws<ArgumentException>(Call);
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
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, true)]
        public void Export_SpecificFailureMechanismsWithSameName_ExpectedResultBasedOnInAssemblyState(
            bool firstSpecificFailureMechanismInAssembly,
            bool secondSpecificFailureMechanismInAssembly,
            bool isExportExpectedToBeSuccessful)
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_SpecificFailureMechanismsWithSameName_ExpectedResultBasedOnInAssemblyState));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            SpecificFailureMechanism specificFailureMechanism1 = assessmentSection.SpecificFailureMechanisms.ElementAt(0);
            SpecificFailureMechanism specificFailureMechanism2 = assessmentSection.SpecificFailureMechanisms.ElementAt(1);

            specificFailureMechanism1.InAssembly = firstSpecificFailureMechanismInAssembly;
            specificFailureMechanism2.InAssembly = secondSpecificFailureMechanismInAssembly;
            assessmentSection.SpecificFailureMechanisms.Last().Name = specificFailureMechanism1.Name;

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                SetCombinedFailureMechanismSectionAssemblyOutput(assessmentSection);

                // Call & Assert
                if (isExportExpectedToBeSuccessful)
                {
                    Assert.IsTrue(exporter.Export());
                }
                else
                {
                    var isExported = true;

                    const string expectedMessage = "Om assemblageresultaten te kunnen exporteren moeten de specifieke faalmechanismen unieke namen hebben.";
                    TestHelper.AssertLogMessageWithLevelIsGenerated(() => isExported = exporter.Export(),
                                                                    new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));

                    Assert.IsFalse(isExported);
                }
            }
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
                void Call() => isExported = exporter.Export();

                // Assert
                const string expectedMessage = "De assemblageresultaten kunnen niet worden geëxporteerd. Inspecteer de resultaten van de individuele faalmechanismen of de gecombineerde faalkans voor meer details.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(isExported);
            }
        }

        [Test]
        public void Export_AssemblyCreatorExceptionThrown_LogsErrorAndReturnsFalse()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_AssemblyCreatorExceptionThrown_LogsErrorAndReturnsFalse));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                failureMechanismSectionAssemblyCalculator.FailureMechanismSectionAssemblyResultOutput = new DefaultFailureMechanismSectionAssemblyResult();

                // Call
                var isExported = true;
                void Call() => isExported = exporter.Export();

                // Assert
                const string expectedMessage = "De assemblageresultaten kunnen niet worden geëxporteerd. Inspecteer de resultaten van de individuele faalmechanismen of de gecombineerde faalkans voor meer details.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(isExported);
            }
        }

        [Test]
        public void Export_FullyConfiguredAssessmentSectionAndValidAssemblyResults_ReturnsTrueAndCreatesFile()
        {
            // Setup
            string folderPath = TestHelper.GetScratchPadPath(nameof(Export_FullyConfiguredAssessmentSectionAndValidAssemblyResults_ReturnsTrueAndCreatesFile));
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "actualAssembly.gml");

            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (new FileDisposeHelper(filePath))
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                SetCombinedFailureMechanismSectionAssemblyOutput(assessmentSection);

                try
                {
                    // Call
                    bool isExported = exporter.Export();

                    // Assert
                    Assert.IsTrue(File.Exists(filePath));
                    Assert.IsTrue(isExported);

                    string expectedGmlFilePath = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Riskeer.Integration.IO),
                                                              nameof(AssemblyExporter), "ExpectedGml.gml");
                    string expectedGml = File.ReadAllText(expectedGmlFilePath);
                    string actualGml = File.ReadAllText(filePath);
                    Assert.AreEqual(expectedGml, actualGml);
                }
                finally
                {
                    Directory.Delete(folderPath, true);
                }
            }
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogsErrorAndReturnsFalse()
        {
            // Setup
            string filePath = TestHelper.GetScratchPadPath(nameof(Export_InvalidDirectoryRights_LogsErrorAndReturnsFalse));
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection();

            var exporter = new AssemblyExporter(assessmentSection, filePath);

            using (var fileDisposeHelper = new FileDisposeHelper(filePath))
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                assessmentSectionAssemblyCalculator.CombinedFailureMechanismSectionAssemblyOutput = Array.Empty<CombinedFailureMechanismSectionAssembly>();

                fileDisposeHelper.LockFiles();

                // Call
                var isExported = true;
                void Call() => isExported = exporter.Export();

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. " +
                                         "Er is geen oordeel geëxporteerd.";
                TestHelper.AssertLogMessageWithLevelIsGenerated(Call, new Tuple<string, LogLevelConstant>(expectedMessage, LogLevelConstant.Error));
                Assert.IsFalse(isExported);
            }
        }

        private static AssessmentSection CreateConfiguredAssessmentSection()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = "assessmentSectionName",
                Id = "assessmentSectionId"
            };
            assessmentSection.ReferenceLine.SetGeometry(new[]
            {
                new Point2D(0, 0),
                new Point2D(3, 4)
            });

            assessmentSection.SpecificFailureMechanisms.AddRange(new[]
            {
                new SpecificFailureMechanism
                {
                    Name = "Specific failure mechanism 1"
                },
                new SpecificFailureMechanism
                {
                    Name = "Specific failure mechanism 2"
                }
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

            FailureMechanismTestHelper.AddSections(assessmentSection.SpecificFailureMechanisms.First(), 2);
            FailureMechanismTestHelper.AddSections(assessmentSection.SpecificFailureMechanisms.Last(), 2);

            return assessmentSection;
        }

        private static void SetCombinedFailureMechanismSectionAssemblyOutput(IAssessmentSection assessmentSection)
        {
            IEnumerable<IFailureMechanism> failureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                .Concat(assessmentSection.SpecificFailureMechanisms);

            var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
            AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
            assessmentSectionAssemblyCalculator.CombinedFailureMechanismSectionAssemblyOutput = new[]
            {
                new CombinedFailureMechanismSectionAssembly(new CombinedAssemblyFailureMechanismSection(0, 2.5, FailureMechanismSectionAssemblyGroup.II),
                                                            failureMechanisms.Select(fm => FailureMechanismSectionAssemblyGroup.II)),
                new CombinedFailureMechanismSectionAssembly(new CombinedAssemblyFailureMechanismSection(2.5, 5, FailureMechanismSectionAssemblyGroup.III),
                                                            failureMechanisms.Select(fm => FailureMechanismSectionAssemblyGroup.III))
            };
        }
    }
}