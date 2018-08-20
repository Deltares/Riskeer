using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

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

            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportablePiping = exportableFailureMechanisms.First();
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, exportablePiping.FailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, exportablePiping.FailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableFailureMechanismType.STPH, exportablePiping.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group2, exportablePiping.Group);
            PipingFailureMechanism pipingFailureMechanism = assessmentSection.Piping;
            Assert.AreEqual(pipingFailureMechanism.Sections.Count(), exportablePiping.Sections.Count());
            Assert.AreEqual(pipingFailureMechanism.SectionResults.Count(), exportablePiping.SectionAssemblyResults.Count());

            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableMacroStabilityInwards = exportableFailureMechanisms.ElementAt(1);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, exportableMacroStabilityInwards.FailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, exportableMacroStabilityInwards.FailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableFailureMechanismType.STBI, exportableMacroStabilityInwards.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group2, exportableMacroStabilityInwards.Group);
            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism = assessmentSection.MacroStabilityInwards;
            Assert.AreEqual(macroStabilityInwardsFailureMechanism.Sections.Count(), exportableMacroStabilityInwards.Sections.Count());
            Assert.AreEqual(macroStabilityInwardsFailureMechanism.SectionResults.Count(), exportableMacroStabilityInwards.SectionAssemblyResults.Count());

            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableGrassCoverErosionInwards = exportableFailureMechanisms.ElementAt(2);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, exportableGrassCoverErosionInwards.FailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, exportableGrassCoverErosionInwards.FailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableFailureMechanismType.GEKB, exportableGrassCoverErosionInwards.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group1, exportableGrassCoverErosionInwards.Group);
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwardsFailureMechanism = assessmentSection.GrassCoverErosionInwards;
            Assert.AreEqual(grassCoverErosionInwardsFailureMechanism.Sections.Count(), exportableGrassCoverErosionInwards.Sections.Count());
            Assert.AreEqual(grassCoverErosionInwardsFailureMechanism.SectionResults.Count(), exportableGrassCoverErosionInwards.SectionAssemblyResults.Count());

            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableHeightStructures = exportableFailureMechanisms.ElementAt(3);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, exportableHeightStructures.FailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, exportableHeightStructures.FailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableFailureMechanismType.HTKW, exportableHeightStructures.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group1, exportableHeightStructures.Group);
            HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
            Assert.AreEqual(heightStructures.Sections.Count(), heightStructures.Sections.Count());
            Assert.AreEqual(heightStructures.SectionResults.Count(), exportableHeightStructures.SectionAssemblyResults.Count());

            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableClosingStructures = exportableFailureMechanisms.ElementAt(4);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, exportableClosingStructures.FailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, exportableClosingStructures.FailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableFailureMechanismType.BSKW, exportableClosingStructures.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group1, exportableClosingStructures.Group);
            ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
            Assert.AreEqual(closingStructures.Sections.Count(), closingStructures.Sections.Count());
            Assert.AreEqual(closingStructures.SectionResults.Count(), exportableClosingStructures.SectionAssemblyResults.Count());

            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableStabilityPointStructures = exportableFailureMechanisms.ElementAt(5);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Group, exportableStabilityPointStructures.FailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyOutput.Probability, exportableStabilityPointStructures.FailureMechanismAssembly.Probability);
            Assert.AreEqual(ExportableFailureMechanismType.STKWp, exportableStabilityPointStructures.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group1, exportableStabilityPointStructures.Group);
            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
            Assert.AreEqual(stabilityPointStructures.Sections.Count(), stabilityPointStructures.Sections.Count());
            Assert.AreEqual(stabilityPointStructures.SectionResults.Count(), exportableStabilityPointStructures.SectionAssemblyResults.Count());
        }

        private static void AssertExportableFailureMechanismsWithoutProbability(
            IEnumerable<ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult>> exportableFailureMechanisms,
            FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator,
            AssessmentSection assessmentSection)
        {
            Assert.AreEqual(2, exportableFailureMechanisms.Count());

            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableStabilityStoneCover = exportableFailureMechanisms.First();
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput, exportableStabilityStoneCover.FailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.ZST, exportableStabilityStoneCover.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group3, exportableStabilityStoneCover.Group);
            StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
            Assert.AreEqual(stabilityStoneCover.Sections.Count(), exportableStabilityStoneCover.Sections.Count());
            Assert.AreEqual(stabilityStoneCover.SectionResults.Count(), exportableStabilityStoneCover.SectionAssemblyResults.Count());

            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableWaveImpactAsphaltCover = exportableFailureMechanisms.ElementAt(1);
            Assert.AreEqual(failureMechanismAssemblyCalculator.FailureMechanismAssemblyCategoryGroupOutput, exportableWaveImpactAsphaltCover.FailureMechanismAssembly.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.AGK, exportableWaveImpactAsphaltCover.Code);
            Assert.AreEqual(ExportableFailureMechanismGroup.Group3, exportableWaveImpactAsphaltCover.Group);
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
            Assert.AreEqual(waveImpactAsphaltCover.Sections.Count(), exportableWaveImpactAsphaltCover.Sections.Count());
            Assert.AreEqual(waveImpactAsphaltCover.SectionResults.Count(), exportableWaveImpactAsphaltCover.SectionAssemblyResults.Count());
        }
    }
}