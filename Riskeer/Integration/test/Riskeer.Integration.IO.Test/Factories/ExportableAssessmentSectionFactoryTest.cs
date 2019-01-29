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
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Factories;
using Riskeer.Piping.Data;

namespace Riskeer.Integration.IO.Test.Factories
{
    [TestFixture]
    public class ExportableAssessmentSectionFactoryTest
    {
        [Test]
        public void CreateExportableAssessmentSection_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateExportableAssessmentSection_WithAssessmentSectionWithReferenceLine_ReturnsExpectedValues()
        {
            // Setup
            const string name = "assessmentSectionName";
            const string id = "assessmentSectionId";

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Name = name,
                Id = id
            };
            ReferenceLineTestFactory.SetReferenceLineGeometry(assessmentSection.ReferenceLine);

            FailureMechanismTestHelper.AddSections(assessmentSection.Piping, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.HeightStructures, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.ClosingStructures, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityPointStructures, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityStoneCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.WaveImpactAsphaltCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.DuneErosion, random.Next(1, 10));

            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.Microstability, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffOutwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffInwards, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.PipingStructure, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.WaterPressureAsphaltCover, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.StrengthStabilityLengthwiseConstruction, random.Next(1, 10));
            FailureMechanismTestHelper.AddSections(assessmentSection.TechnicalInnovation, random.Next(1, 10));

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorFactory.LastCreatedAssessmentSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                Assert.AreEqual(id, exportableAssessmentSection.Id);
                CollectionAssert.AreEqual(assessmentSection.ReferenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyWithProbability = exportableAssessmentSection.FailureMechanismAssemblyWithProbability;
                Assert.AreEqual(ExportableAssemblyMethod.WBI2B1, failureMechanismAssemblyWithProbability.AssemblyMethod);
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyOutput.Group, failureMechanismAssemblyWithProbability.AssemblyCategory);
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyOutput.Probability, failureMechanismAssemblyWithProbability.Probability);

                ExportableFailureMechanismAssemblyResult failureMechanismAssemblyWithoutProbability = exportableAssessmentSection.FailureMechanismAssemblyWithoutProbability;
                Assert.AreEqual(ExportableAssemblyMethod.WBI2A1, failureMechanismAssemblyWithoutProbability.AssemblyMethod);
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput, failureMechanismAssemblyWithoutProbability.AssemblyCategory);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleAssessmentSectionCategoryGroupOutput, exportableAssessmentSectionAssemblyResult.AssemblyCategory);
                Assert.AreEqual(ExportableAssemblyMethod.WBI2C1, exportableAssessmentSectionAssemblyResult.AssemblyMethod);

                AssertExportableFailureMechanismsWithProbability(exportableAssessmentSection.FailureMechanismsWithProbability,
                                                                 failureMechanismAssemblyCalculator,
                                                                 assessmentSection);

                AssertExportableFailureMechanismsWithoutProbability(exportableAssessmentSection.FailureMechanismsWithoutProbability,
                                                                    failureMechanismAssemblyCalculator,
                                                                    assessmentSection);

                int expectedNrOfSections = AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection, false).Count();
                Assert.AreEqual(expectedNrOfSections, exportableAssessmentSection.CombinedSectionAssemblies.Count());
            }
        }

        [Test]
        public void CreateExportableAssessmentSection_AssessmentSectionWithManualSectionAssemblyWithProbability_ManualAssemblyNotExecuted()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Id = "1"
            };
            ReferenceLineTestFactory.SetReferenceLineGeometry(assessmentSection.ReferenceLine);

            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            failureMechanism.IsRelevant = true;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyProbability = random.NextDouble();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub failureMechanismSectionAssemblyCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.IsNull(failureMechanismSectionAssemblyCalculator.ManualAssemblyCategoriesInput);
                Assert.AreEqual(0, failureMechanismSectionAssemblyCalculator.ManualAssemblyNInput);
                Assert.AreEqual(0, failureMechanismSectionAssemblyCalculator.ManualAssemblyProbabilityInput);
            }
        }

        #region TestHelpers FailureMechanismsWithProbability

        private static void AssertExportableFailureMechanismsWithProbability(
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> exportableFailureMechanisms,
            FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator,
            AssessmentSection assessmentSection)
        {
            Assert.AreEqual(6, exportableFailureMechanisms.Count());

            FailureMechanismAssembly expectedFailureMechanismAssemblyOutput = failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput;
            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableFailureMechanismType.STPH,
                                                            ExportableFailureMechanismGroup.Group2,
                                                            assessmentSection.ClosingStructures,
                                                            exportableFailureMechanisms.First());

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableFailureMechanismType.STBI,
                                                            ExportableFailureMechanismGroup.Group2,
                                                            assessmentSection.MacroStabilityInwards,
                                                            exportableFailureMechanisms.ElementAt(1));

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableFailureMechanismType.GEKB,
                                                            ExportableFailureMechanismGroup.Group1,
                                                            assessmentSection.GrassCoverErosionInwards,
                                                            exportableFailureMechanisms.ElementAt(2));

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableFailureMechanismType.HTKW,
                                                            ExportableFailureMechanismGroup.Group1,
                                                            assessmentSection.HeightStructures,
                                                            exportableFailureMechanisms.ElementAt(3));

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableFailureMechanismType.BSKW,
                                                            ExportableFailureMechanismGroup.Group1,
                                                            assessmentSection.ClosingStructures,
                                                            exportableFailureMechanisms.ElementAt(4));

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableFailureMechanismType.STKWp,
                                                            ExportableFailureMechanismGroup.Group1,
                                                            assessmentSection.StabilityPointStructures,
                                                            exportableFailureMechanisms.ElementAt(5));
        }

        private static void AssertExportableFailureMechanismWithProbability(FailureMechanismAssembly expectedAssemblyOutput,
                                                                            ExportableFailureMechanismType expectedFailureMechanismCode,
                                                                            ExportableFailureMechanismGroup expectedFailureMechanismGroup,
                                                                            IHasSectionResults<FailureMechanismSectionResult> failureMechanism,
                                                                            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> actualExportableFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanismCode, actualExportableFailureMechanism.Code);
            Assert.AreEqual(expectedFailureMechanismGroup, actualExportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResultWithProbability exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(expectedAssemblyOutput.Group, exportableFailureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(expectedAssemblyOutput.Probability, exportableFailureMechanismAssemblyResult.Probability);
            Assert.AreEqual(ExportableAssemblyMethod.WBI1B1, exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }

        #endregion

        #region TestHelpers FailureMechanismsWithoutProbability

        private static void AssertExportableFailureMechanismsWithoutProbability(
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> exportableFailureMechanisms,
            FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator,
            AssessmentSection assessmentSection)
        {
            Assert.AreEqual(12, exportableFailureMechanisms.Count());

            FailureMechanismAssemblyCategoryGroup expectedFailureMechanismAssemblyOutput = failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput.Value;
            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.ZST,
                                                               ExportableFailureMechanismGroup.Group3,
                                                               assessmentSection.StabilityStoneCover,
                                                               exportableFailureMechanisms.First());

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.AGK,
                                                               ExportableFailureMechanismGroup.Group3,
                                                               assessmentSection.WaveImpactAsphaltCover,
                                                               exportableFailureMechanisms.ElementAt(1));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.GEBU,
                                                               ExportableFailureMechanismGroup.Group3,
                                                               assessmentSection.GrassCoverErosionOutwards,
                                                               exportableFailureMechanisms.ElementAt(2));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.DA,
                                                               ExportableFailureMechanismGroup.Group3,
                                                               assessmentSection.DuneErosion,
                                                               exportableFailureMechanisms.ElementAt(3));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.STBU,
                                                               ExportableFailureMechanismGroup.Group4,
                                                               assessmentSection.MacroStabilityOutwards,
                                                               exportableFailureMechanisms.ElementAt(4));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.STMI,
                                                               ExportableFailureMechanismGroup.Group4,
                                                               assessmentSection.Microstability,
                                                               exportableFailureMechanisms.ElementAt(5));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.GABU,
                                                               ExportableFailureMechanismGroup.Group4,
                                                               assessmentSection.GrassCoverSlipOffOutwards,
                                                               exportableFailureMechanisms.ElementAt(6));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.GABI,
                                                               ExportableFailureMechanismGroup.Group4,
                                                               assessmentSection.GrassCoverSlipOffInwards,
                                                               exportableFailureMechanisms.ElementAt(7));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.PKW,
                                                               ExportableFailureMechanismGroup.Group4,
                                                               assessmentSection.PipingStructure,
                                                               exportableFailureMechanisms.ElementAt(8));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.AWO,
                                                               ExportableFailureMechanismGroup.Group4,
                                                               assessmentSection.WaterPressureAsphaltCover,
                                                               exportableFailureMechanisms.ElementAt(9));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.STKWl,
                                                               ExportableFailureMechanismGroup.Group4,
                                                               assessmentSection.StrengthStabilityLengthwiseConstruction,
                                                               exportableFailureMechanisms.ElementAt(10));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableFailureMechanismType.INN,
                                                               ExportableFailureMechanismGroup.Group4,
                                                               assessmentSection.TechnicalInnovation,
                                                               exportableFailureMechanisms.ElementAt(11));
        }

        private static void AssertExportableFailureMechanismWithoutProbability(FailureMechanismAssemblyCategoryGroup expectedAssemblyOutput,
                                                                               ExportableFailureMechanismType expectedFailureMechanismCode,
                                                                               ExportableFailureMechanismGroup expectedFailureMechanismGroup,
                                                                               IHasSectionResults<FailureMechanismSectionResult> failureMechanism,
                                                                               ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> actualExportableFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanismCode, actualExportableFailureMechanism.Code);
            Assert.AreEqual(expectedFailureMechanismGroup, actualExportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(expectedAssemblyOutput, exportableFailureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableAssemblyMethod.WBI1A1, exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }

        #endregion
    }
}