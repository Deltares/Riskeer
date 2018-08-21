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
                assessmentSectionAssemblyCalculator.CombinedFailureMechanismSectionAssemblyOutput = new[]
                {
                    CreateCombinedFailureMechanismSectionAssembly(assessmentSection, 20),
                    CreateCombinedFailureMechanismSectionAssembly(assessmentSection, 21)
                };

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

        private static CombinedFailureMechanismSectionAssembly CreateCombinedFailureMechanismSectionAssembly(AssessmentSection assessmentSection, int seed)
        {
            var random = new Random(seed);
            return new CombinedFailureMechanismSectionAssembly(new CombinedAssemblyFailureMechanismSection(random.NextDouble(),
                                                                                                           random.NextDouble(),
                                                                                                           random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()),
                                                               assessmentSection.GetFailureMechanisms()
                                                                                .Where(fm => fm.IsRelevant)
                                                                                .Select(fm => random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()).ToArray());
        }

        #region TestHelper CombinedFailureMechanismSection

        private static void AssertCombinedFailureMechanismSectionAssemblyResults(IEnumerable<CombinedFailureMechanismSectionAssemblyResult> expectedSections,
                                                                                 ExportableCombinedSectionAssemblyCollection collection)
        {
            int expectedNrOfSections = expectedSections.Count();
            Assert.AreEqual(expectedNrOfSections, collection.Sections.Count());
            Assert.AreEqual(expectedNrOfSections, collection.CombinedSectionAssemblyResults.Count());

            for (var i = 0; i < expectedNrOfSections; i++)
            {
                CombinedFailureMechanismSectionAssemblyResult expectedSection = expectedSections.ElementAt(i);
                ExportableCombinedFailureMechanismSection actualSection = collection.Sections.ElementAt(i);
                ExportableCombinedSectionAssembly actualSectionResult = collection.CombinedSectionAssemblyResults.ElementAt(i);

                AssertExportableCombinedFailureMechanismSection(expectedSection, actualSection);
                AssertExportableCombinedFailureMechanismSectionResult(actualSectionResult, actualSection, expectedSection);
            }
        }

        private static void AssertExportableCombinedFailureMechanismSection(CombinedFailureMechanismSectionAssemblyResult expectedSection,
                                                                            ExportableCombinedFailureMechanismSection actualSection)
        {
            Assert.AreEqual(expectedSection.SectionStart, actualSection.StartDistance);
            Assert.AreEqual(expectedSection.SectionEnd, actualSection.EndDistance);
            CollectionAssert.IsEmpty(actualSection.Geometry);
            Assert.AreEqual(ExportableAssemblyMethod.WBI3A1, actualSection.AssemblyMethod);
        }

        private static void AssertExportableCombinedFailureMechanismSectionResult(ExportableCombinedSectionAssembly expectedSectionResult,
                                                                                  ExportableCombinedFailureMechanismSection expectedSection,
                                                                                  CombinedFailureMechanismSectionAssemblyResult actualCombinedSectionAssemblyResult)
        {
            Assert.AreSame(expectedSection, expectedSectionResult.Section);
            Assert.AreEqual(actualCombinedSectionAssemblyResult.TotalResult, expectedSectionResult.CombinedSectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableAssemblyMethod.WBI3C1, expectedSectionResult.CombinedSectionAssemblyResult.AssemblyMethod);

            IEnumerable<ExportableFailureMechanismCombinedSectionAssemblyResult> failureMechanismCombinedSectionResults = expectedSectionResult.FailureMechanismResults;
            Assert.AreEqual(18, failureMechanismCombinedSectionResults.Count());
            Assert.IsTrue(failureMechanismCombinedSectionResults.All(result => result.SectionAssemblyResult.AssemblyMethod == ExportableAssemblyMethod.WBI3B1));

            Assert.AreEqual(actualCombinedSectionAssemblyResult.Piping, failureMechanismCombinedSectionResults.ElementAt(0).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STPH, failureMechanismCombinedSectionResults.ElementAt(0).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.GrassCoverErosionInwards, failureMechanismCombinedSectionResults.ElementAt(1).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.GEKB, failureMechanismCombinedSectionResults.ElementAt(1).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.MacroStabilityInwards, failureMechanismCombinedSectionResults.ElementAt(2).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STBI, failureMechanismCombinedSectionResults.ElementAt(2).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.MacroStabilityOutwards, failureMechanismCombinedSectionResults.ElementAt(3).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STBU, failureMechanismCombinedSectionResults.ElementAt(3).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.Microstability, failureMechanismCombinedSectionResults.ElementAt(4).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STMI, failureMechanismCombinedSectionResults.ElementAt(4).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.StabilityStoneCover, failureMechanismCombinedSectionResults.ElementAt(5).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.ZST, failureMechanismCombinedSectionResults.ElementAt(5).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.WaveImpactAsphaltCover, failureMechanismCombinedSectionResults.ElementAt(6).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.AGK, failureMechanismCombinedSectionResults.ElementAt(6).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.WaterPressureAsphaltCover, failureMechanismCombinedSectionResults.ElementAt(7).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.AWO, failureMechanismCombinedSectionResults.ElementAt(7).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.GrassCoverErosionOutwards, failureMechanismCombinedSectionResults.ElementAt(8).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.GEBU, failureMechanismCombinedSectionResults.ElementAt(8).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.GrassCoverSlipOffOutwards, failureMechanismCombinedSectionResults.ElementAt(9).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.GABU, failureMechanismCombinedSectionResults.ElementAt(9).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.GrassCoverSlipOffInwards, failureMechanismCombinedSectionResults.ElementAt(10).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.GABI, failureMechanismCombinedSectionResults.ElementAt(10).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.HeightStructures, failureMechanismCombinedSectionResults.ElementAt(11).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.HTKW, failureMechanismCombinedSectionResults.ElementAt(11).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.ClosingStructures, failureMechanismCombinedSectionResults.ElementAt(12).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.BSKW, failureMechanismCombinedSectionResults.ElementAt(12).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.PipingStructure, failureMechanismCombinedSectionResults.ElementAt(13).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.PKW, failureMechanismCombinedSectionResults.ElementAt(13).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.StabilityPointStructures, failureMechanismCombinedSectionResults.ElementAt(14).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STKWp, failureMechanismCombinedSectionResults.ElementAt(14).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.StrengthStabilityLengthwiseConstruction, failureMechanismCombinedSectionResults.ElementAt(15).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.STKWl, failureMechanismCombinedSectionResults.ElementAt(15).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.DuneErosion, failureMechanismCombinedSectionResults.ElementAt(16).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.DA, failureMechanismCombinedSectionResults.ElementAt(16).Code);

            Assert.AreEqual(actualCombinedSectionAssemblyResult.TechnicalInnovation, failureMechanismCombinedSectionResults.ElementAt(17).SectionAssemblyResult.AssemblyCategory);
            Assert.AreEqual(ExportableFailureMechanismType.INN, failureMechanismCombinedSectionResults.ElementAt(17).Code);
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