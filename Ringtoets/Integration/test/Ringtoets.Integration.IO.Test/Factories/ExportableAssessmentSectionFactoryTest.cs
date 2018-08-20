using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Test.Factories
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
        public void CreateExportableAssessmentSection_WithAssessmentSection_ReturnsExpectedValues()
        {
            // Setup
            const string name = "assessmentSectionName";

            var referenceLine = new ReferenceLine();
            referenceLine.SetGeometry(new[]
            {
                new Point2D(1, 1),
                new Point2D(2, 2)
            });

            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>())
            {
                Name = name,
                ReferenceLine = referenceLine
            };

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

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;
                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                CollectionAssert.AreEqual(referenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleAssessmentSectionCategoryGroupOutput, exportableAssessmentSectionAssemblyResult.AssemblyCategory);
                Assert.AreEqual(ExportableAssemblyMethod.WBI2C1, exportableAssessmentSectionAssemblyResult.AssemblyMethod);

                AssertExportableFailureMechanismsWithProbability(exportableAssessmentSection.FailureMechanismsWithProbability,
                                                                 failureMechanismAssemblyCalculator,
                                                                 assessmentSection);

                AssertExportableFailureMechanismsWithoutProbability(exportableAssessmentSection.FailureMechanismsWithoutProbability,
                                                                    failureMechanismAssemblyCalculator,
                                                                    assessmentSection);

                Assert.IsNotNull(exportableAssessmentSection.CombinedSectionAssemblyResults);
            }
        }

        private static void AssertExportableFailureMechanismsWithProbability(
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability>> exportableFailureMechanisms,
            FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator,
            AssessmentSection assessmentSection)
        {
            Assert.AreEqual(6, exportableFailureMechanisms.Count());

            FailureMechanismAssembly expectedFailureMechanismAssemblyOutput = failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput;
            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableAssemblyMethod.WBI1B1,
                                                            ExportableFailureMechanismType.STPH,
                                                            ExportableFailureMechanismGroup.Group2,
                                                            assessmentSection.ClosingStructures,
                                                            exportableFailureMechanisms.First());

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableAssemblyMethod.WBI1B1,
                                                            ExportableFailureMechanismType.STBI,
                                                            ExportableFailureMechanismGroup.Group2,
                                                            assessmentSection.MacroStabilityInwards,
                                                            exportableFailureMechanisms.ElementAt(1));

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableAssemblyMethod.WBI1B1,
                                                            ExportableFailureMechanismType.GEKB,
                                                            ExportableFailureMechanismGroup.Group1,
                                                            assessmentSection.GrassCoverErosionInwards,
                                                            exportableFailureMechanisms.ElementAt(2));

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableAssemblyMethod.WBI1B1,
                                                            ExportableFailureMechanismType.HTKW,
                                                            ExportableFailureMechanismGroup.Group1,
                                                            assessmentSection.HeightStructures,
                                                            exportableFailureMechanisms.ElementAt(3));

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableAssemblyMethod.WBI1B1,
                                                            ExportableFailureMechanismType.BSKW,
                                                            ExportableFailureMechanismGroup.Group1,
                                                            assessmentSection.ClosingStructures,
                                                            exportableFailureMechanisms.ElementAt(4));

            AssertExportableFailureMechanismWithProbability(expectedFailureMechanismAssemblyOutput,
                                                            ExportableAssemblyMethod.WBI1B1,
                                                            ExportableFailureMechanismType.STKWp,
                                                            ExportableFailureMechanismGroup.Group1,
                                                            assessmentSection.StabilityPointStructures,
                                                            exportableFailureMechanisms.ElementAt(5));
        }

        private static void AssertExportableFailureMechanismWithProbability(FailureMechanismAssembly expectedAssemblyOutput,
                                                                            ExportableAssemblyMethod expectedAssemblyMethod,
                                                                            ExportableFailureMechanismType expectedFailureMechanismCode,
                                                                            ExportableFailureMechanismGroup expecteFailureMechanismGroup,
                                                                            IHasSectionResults<FailureMechanismSectionResult> failureMechanism,
                                                                            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> actualExportableFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanismCode, actualExportableFailureMechanism.Code);
            Assert.AreEqual(expecteFailureMechanismGroup, actualExportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResultWithProbability exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(expectedAssemblyOutput.Group, exportableFailureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(expectedAssemblyOutput.Probability, exportableFailureMechanismAssemblyResult.Probability);
            Assert.AreEqual(expectedAssemblyMethod, exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.Sections.Count(), actualExportableFailureMechanism.Sections.Count());
            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }

        private static void AssertExportableFailureMechanismsWithoutProbability(
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> exportableFailureMechanisms,
            FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator,
            AssessmentSection assessmentSection)
        {
            Assert.AreEqual(4, exportableFailureMechanisms.Count());

            FailureMechanismAssemblyCategoryGroup expectedFailureMechanismAssemblyOutput = failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput.Value;
            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableAssemblyMethod.WBI1A1,
                                                               ExportableFailureMechanismType.ZST,
                                                               ExportableFailureMechanismGroup.Group3,
                                                               assessmentSection.StabilityStoneCover,
                                                               exportableFailureMechanisms.First());

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableAssemblyMethod.WBI1A1,
                                                               ExportableFailureMechanismType.AGK,
                                                               ExportableFailureMechanismGroup.Group3,
                                                               assessmentSection.WaveImpactAsphaltCover,
                                                               exportableFailureMechanisms.ElementAt(1));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableAssemblyMethod.WBI1A1,
                                                               ExportableFailureMechanismType.GEBU,
                                                               ExportableFailureMechanismGroup.Group3,
                                                               assessmentSection.GrassCoverErosionOutwards,
                                                               exportableFailureMechanisms.ElementAt(2));

            AssertExportableFailureMechanismWithoutProbability(expectedFailureMechanismAssemblyOutput,
                                                               ExportableAssemblyMethod.WBI1A1,
                                                               ExportableFailureMechanismType.DA,
                                                               ExportableFailureMechanismGroup.Group3,
                                                               assessmentSection.DuneErosion,
                                                               exportableFailureMechanisms.ElementAt(3));
        }

        private static void AssertExportableFailureMechanismWithoutProbability(FailureMechanismAssemblyCategoryGroup expectedAssemblyOutput,
                                                                               ExportableAssemblyMethod expectedAssemblyMethod,
                                                                               ExportableFailureMechanismType expectedFailureMechanismCode,
                                                                               ExportableFailureMechanismGroup expecteFailureMechanismGroup,
                                                                               IHasSectionResults<FailureMechanismSectionResult> failureMechanism,
                                                                               ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> actualExportableFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanismCode, actualExportableFailureMechanism.Code);
            Assert.AreEqual(expecteFailureMechanismGroup, actualExportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(expectedAssemblyOutput, exportableFailureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(expectedAssemblyMethod, exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.Sections.Count(), actualExportableFailureMechanism.Sections.Count());
            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }
    }
}