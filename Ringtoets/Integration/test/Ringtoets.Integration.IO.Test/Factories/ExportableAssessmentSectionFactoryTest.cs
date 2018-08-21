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
using Ringtoets.Integration.Data.Assembly;
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
                var calculatorfactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssessmentSectionAssemblyCalculatorStub assessmentSectionAssemblyCalculator = calculatorfactory.LastCreatedAssessmentSectionAssemblyCalculator;

                FailureMechanismAssemblyCalculatorStub failureMechanismAssemblyCalculator = calculatorfactory.LastCreatedFailureMechanismAssemblyCalculator;

                // Call
                ExportableAssessmentSection exportableAssessmentSection = ExportableAssessmentSectionFactory.CreateExportableAssessmentSection(assessmentSection);

                // Assert
                Assert.AreEqual(name, exportableAssessmentSection.Name);
                CollectionAssert.AreEqual(referenceLine.Points, exportableAssessmentSection.Geometry);

                ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyWithProbablity = exportableAssessmentSection.FailureMechanismAssemblyWithProbability;
                Assert.AreEqual(ExportableAssemblyMethod.WBI2B1, failureMechanismAssemblyWithProbablity.AssemblyMethod);
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyOutput.Group, failureMechanismAssemblyWithProbablity.AssemblyCategory);
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyOutput.Probability, failureMechanismAssemblyWithProbablity.Probability);

                ExportableFailureMechanismAssemblyResult failureMechanismAssemblyWithoutProbablity = exportableAssessmentSection.FailureMechanismAssemblyWithoutProbability;
                Assert.AreEqual(ExportableAssemblyMethod.WBI2A1, failureMechanismAssemblyWithoutProbablity.AssemblyMethod);
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleFailureMechanismsAssemblyCategoryGroupOutput, failureMechanismAssemblyWithoutProbablity.AssemblyCategory);

                ExportableAssessmentSectionAssemblyResult exportableAssessmentSectionAssemblyResult = exportableAssessmentSection.AssessmentSectionAssembly;
                Assert.AreEqual(assessmentSectionAssemblyCalculator.AssembleAssessmentSectionCategoryGroupOutput, exportableAssessmentSectionAssemblyResult.AssemblyCategory);
                Assert.AreEqual(ExportableAssemblyMethod.WBI2C1, exportableAssessmentSectionAssemblyResult.AssemblyMethod);

                AssertExportableFailureMechanismsWithProbability(exportableAssessmentSection.FailureMechanismsWithProbability,
                                                                 failureMechanismAssemblyCalculator,
                                                                 assessmentSection);

                AssertExportableFailureMechanismsWithoutProbability(exportableAssessmentSection.FailureMechanismsWithoutProbability,
                                                                    failureMechanismAssemblyCalculator,
                                                                    assessmentSection);

                AssertCombinedFailureMechanismSectionAssemblyResults(AssessmentSectionAssemblyFactory.AssembleCombinedPerFailureMechanismSection(assessmentSection),
                                                                     exportableAssessmentSection.CombinedSectionAssemblyResults);
            }
        }

        #region TestHelper CombinedFailureMechanismSection

        private static void AssertCombinedFailureMechanismSectionAssemblyResults(IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedSections,
                                                                                 ExportableCombinedSectionAssemblyCollection collection)
        {
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, collection.Sections.Count());
            Assert.AreEqual(expectedNrOfSections, collection.CombinedSectionAssemblyResults.Count());
        }

        #endregion

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
                                                                            ExportableFailureMechanismGroup expecteFailureMechanismGroup,
                                                                            IHasSectionResults<FailureMechanismSectionResult> failureMechanism,
                                                                            ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> actualExportableFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanismCode, actualExportableFailureMechanism.Code);
            Assert.AreEqual(expecteFailureMechanismGroup, actualExportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResultWithProbability exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(expectedAssemblyOutput.Group, exportableFailureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(expectedAssemblyOutput.Probability, exportableFailureMechanismAssemblyResult.Probability);
            Assert.AreEqual(ExportableAssemblyMethod.WBI1B1, exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.Sections.Count(), actualExportableFailureMechanism.Sections.Count());
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
                                                                               ExportableFailureMechanismGroup expecteFailureMechanismGroup,
                                                                               IHasSectionResults<FailureMechanismSectionResult> failureMechanism,
                                                                               ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> actualExportableFailureMechanism)
        {
            Assert.AreEqual(expectedFailureMechanismCode, actualExportableFailureMechanism.Code);
            Assert.AreEqual(expecteFailureMechanismGroup, actualExportableFailureMechanism.Group);

            ExportableFailureMechanismAssemblyResult exportableFailureMechanismAssemblyResult = actualExportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(expectedAssemblyOutput, exportableFailureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableAssemblyMethod.WBI1A1, exportableFailureMechanismAssemblyResult.AssemblyMethod);

            Assert.AreEqual(failureMechanism.Sections.Count(), actualExportableFailureMechanism.Sections.Count());
            Assert.AreEqual(failureMechanism.SectionResults.Count(), actualExportableFailureMechanism.SectionAssemblyResults.Count());
        }

        #endregion
    }
}