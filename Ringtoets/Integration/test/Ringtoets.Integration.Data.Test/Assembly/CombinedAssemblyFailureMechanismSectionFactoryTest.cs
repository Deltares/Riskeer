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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Primitives;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Data.TestUtil;
using Ringtoets.Integration.TestUtil;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class CombinedAssemblyFailureMechanismSectionFactoryTest
    {
        [Test]
        public void CreateInput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(null,
                                                                                                 Enumerable.Empty<IFailureMechanism>(),
                                                                                                 new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateInput_FailureMechanismsNull_ThrowsArgumentNullException()
        {
            // Setup
            var assessmentSection = new AssessmentSection(new Random(21).NextEnumValue<AssessmentSectionComposition>());

            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection,
                                                                                                 null,
                                                                                                 new Random(39).NextBoolean());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanisms", exception.ParamName);
        }

        [Test]
        public void CreateInput_WithAllFailureMechanisms_ReturnsInputCollection()
        {
            // Setup
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms(), new Random(39).NextBoolean());

                // Assert
                Assert.AreEqual(18, inputs.Count());
                AssertSectionsWithResult(assessmentSection.Piping.SectionResults, inputs.ElementAt(0));
                AssertSectionsWithResult(assessmentSection.GrassCoverErosionInwards.SectionResults, inputs.ElementAt(1));
                AssertSectionsWithResult(assessmentSection.MacroStabilityInwards.SectionResults, inputs.ElementAt(2));
                AssertSectionsWithResult(assessmentSection.MacroStabilityOutwards.SectionResults, inputs.ElementAt(3));
                AssertSectionsWithResult(assessmentSection.Microstability.SectionResults, inputs.ElementAt(4));
                AssertSectionsWithResult(assessmentSection.StabilityStoneCover.SectionResults, inputs.ElementAt(5));
                AssertSectionsWithResult(assessmentSection.WaveImpactAsphaltCover.SectionResults, inputs.ElementAt(6));
                AssertSectionsWithResult(assessmentSection.WaterPressureAsphaltCover.SectionResults, inputs.ElementAt(7));
                AssertSectionsWithResult(assessmentSection.GrassCoverErosionOutwards.SectionResults, inputs.ElementAt(8));
                AssertSectionsWithResult(assessmentSection.GrassCoverSlipOffOutwards.SectionResults, inputs.ElementAt(9));
                AssertSectionsWithResult(assessmentSection.GrassCoverSlipOffInwards.SectionResults, inputs.ElementAt(10));
                AssertSectionsWithResult(assessmentSection.HeightStructures.SectionResults, inputs.ElementAt(11));
                AssertSectionsWithResult(assessmentSection.ClosingStructures.SectionResults, inputs.ElementAt(12));
                AssertSectionsWithResult(assessmentSection.PipingStructure.SectionResults, inputs.ElementAt(13));
                AssertSectionsWithResult(assessmentSection.StabilityPointStructures.SectionResults, inputs.ElementAt(14));
                AssertSectionsWithResult(assessmentSection.StrengthStabilityLengthwiseConstruction.SectionResults, inputs.ElementAt(15));
                AssertSectionsWithResult(assessmentSection.DuneErosion.SectionResults, inputs.ElementAt(16));
                AssertSectionsWithResult(assessmentSection.TechnicalInnovation.SectionResults, inputs.ElementAt(17));
            }
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismTestCaseData))]
        public void CreateInput_WithOneFailureMechanism_ReturnsInputCollection(AssessmentSection assessmentSection, IFailureMechanism relevantFailureMechanism)
        {
            // Setup
            assessmentSection.GetFailureMechanisms().ForEachElementDo(failureMechanism => failureMechanism.IsRelevant = failureMechanism == relevantFailureMechanism);

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, new[]
                    {
                        relevantFailureMechanism
                    }, new Random(39).NextBoolean());

                // Assert
                AssertSections(((IHasSectionResults<FailureMechanismSectionResult>) relevantFailureMechanism).SectionResults, inputs.Single());
            }
        }

        private static IEnumerable<TestCaseData> GetFailureMechanismTestCaseData()
        {
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                new Random(21).NextEnumValue<AssessmentSectionComposition>());

            yield return new TestCaseData(assessmentSection, assessmentSection.Piping);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverErosionInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.MacroStabilityInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.MacroStabilityOutwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.Microstability);
            yield return new TestCaseData(assessmentSection, assessmentSection.StabilityStoneCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.WaveImpactAsphaltCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.WaterPressureAsphaltCover);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverErosionOutwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverSlipOffOutwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.GrassCoverSlipOffInwards);
            yield return new TestCaseData(assessmentSection, assessmentSection.HeightStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.ClosingStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.PipingStructure);
            yield return new TestCaseData(assessmentSection, assessmentSection.StabilityPointStructures);
            yield return new TestCaseData(assessmentSection, assessmentSection.StrengthStabilityLengthwiseConstruction);
            yield return new TestCaseData(assessmentSection, assessmentSection.DuneErosion);
            yield return new TestCaseData(assessmentSection, assessmentSection.TechnicalInnovation);
        }

        private static void AssertSections<T>(IEnumerable<T> originalSectionResults, IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
            where T : FailureMechanismSectionResult
        {
            Assert.AreEqual(originalSectionResults.Count(), inputSections.Count());

            double expectedSectionStart = 0;

            for (var i = 0; i < originalSectionResults.Count(); i++)
            {
                double expectedSectionEnd = expectedSectionStart + originalSectionResults.ElementAt(i).Section.Length;

                Assert.AreEqual(expectedSectionStart, inputSections.ElementAt(i).SectionStart);
                Assert.AreEqual(expectedSectionEnd, inputSections.ElementAt(i).SectionEnd);

                expectedSectionStart = expectedSectionEnd;
            }
        }

        private static void AssertSectionsWithResult<T>(IEnumerable<T> originalSectionResults, IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
            where T : FailureMechanismSectionResult
        {
            AssertSections(originalSectionResults, inputSections);

            for (var i = 0; i < originalSectionResults.Count(); i++)
            {
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, inputSections.ElementAt(i).CategoryGroup);
            }
        }

        #region Manual Assembly

        #region Failure mechanisms with probability

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetConfiguredAssessmentSectionWithFailureMechanismsWithProbability))]
        public void CreateInput_AssessmentSectionWithFailureMechanismWithProbabilityWithManualAssemblyAndUseManualTrue_ReturnsInputWithExpectedValue(AssessmentSection assessmentSection)
        {
            // Setup
            IFailureMechanism relevantFailureMechanism = assessmentSection.GetFailureMechanisms().Single(fm => fm.IsRelevant);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    relevantFailureMechanism
                }, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionCalculator.ManualAssemblyAssemblyOutput.Group, input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetConfiguredAssessmentSectionWithFailureMechanismsWithProbability))]
        public void CreateInput_AssessmentSectionWithFailureMechanismWithProbabilityWithManualAssemblyAndUseManualFalse_ReturnsInputWithExpectedValue(AssessmentSection assessmentSection)
        {
            // Setup
            IFailureMechanism relevantFailureMechanism = assessmentSection.GetFailureMechanisms().Single(fm => fm.IsRelevant);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    relevantFailureMechanism
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionCalculator.CombinedAssemblyOutput.Group, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region Failure mechanisms without probability

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetConfiguredAssessmentSectionWithFailureMechanismsWithoutProbability))]
        public void CreateInput_AssessmentSectionWithFailureMechanismWithoutProbabilityWithManualAssemblyAndUseManualTrue_ReturnsInputWithExpectedValue(AssessmentSection assessmentSection)
        {
            // Setup
            IFailureMechanism relevantFailureMechanism = assessmentSection.GetFailureMechanisms().Single(fm => fm.IsRelevant);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    relevantFailureMechanism
                }, true);

                // Then
                AssertInputForFailureMechanismsWithoutProbabilityWithManualAssembly(relevantFailureMechanism, input);
            }
        }

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetConfiguredAssessmentSectionWithFailureMechanismsWithoutProbability))]
        public void CreateInput_AssessmentSectionWithFailureMechanismWithoutProbabilityWithManualAssemblyAndUseManualFalse_ReturnsInputWithExpectedValue(AssessmentSection assessmentSection)
        {
            // Setup
            IFailureMechanism relevantFailureMechanism = assessmentSection.GetFailureMechanisms().Single(fm => fm.IsRelevant);
            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    relevantFailureMechanism
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                FailureMechanismSectionAssemblyCalculatorStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionCalculator.CombinedAssemblyCategoryOutput, input.Single().Single().CategoryGroup);
            }
        }

        private static void AssertInputForFailureMechanismsWithoutProbabilityWithManualAssembly(IFailureMechanism failureMechanism,
                                                                                                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> actualInput)
        {
            var duneErosion = failureMechanism as DuneErosionFailureMechanism;
            if (duneErosion != null)
            {
                Assert.AreEqual(duneErosion.SectionResults
                                           .Single()
                                           .ManualAssemblyCategoryGroup,
                                actualInput.Single().Single().CategoryGroup);
            }

            var grassCoverErosionOutwards = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
            if (grassCoverErosionOutwards != null)
            {
                Assert.AreEqual(grassCoverErosionOutwards.SectionResults
                                                         .Single()
                                                         .ManualAssemblyCategoryGroup,
                                actualInput.Single().Single().CategoryGroup);
            }

            var stabilityStoneCover = failureMechanism as StabilityStoneCoverFailureMechanism;
            if (stabilityStoneCover != null)
            {
                Assert.AreEqual(stabilityStoneCover.SectionResults
                                                   .Single()
                                                   .ManualAssemblyCategoryGroup,
                                actualInput.Single().Single().CategoryGroup);
            }

            var waveImpactAsphaltCover = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
            if (waveImpactAsphaltCover != null)
            {
                Assert.AreEqual(waveImpactAsphaltCover.SectionResults
                                                      .Single()
                                                      .ManualAssemblyCategoryGroup,
                                actualInput.Single().Single().CategoryGroup);
            }

            var grassCoverSlipOffInwards = failureMechanism as GrassCoverSlipOffInwardsFailureMechanism;
            if (grassCoverSlipOffInwards != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(grassCoverSlipOffInwards.SectionResults
                                                                                                                            .Single()
                                                                                                                            .ManualAssemblyCategoryGroup),
                                actualInput.Single().Single().CategoryGroup);
            }

            var grassCoverSlipOffOutwards = failureMechanism as GrassCoverSlipOffOutwardsFailureMechanism;
            if (grassCoverSlipOffOutwards != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(grassCoverSlipOffOutwards.SectionResults
                                                                                                                             .Single()
                                                                                                                             .ManualAssemblyCategoryGroup),
                                actualInput.Single().Single().CategoryGroup);
            }

            var pipingStructure = failureMechanism as PipingStructureFailureMechanism;
            if (pipingStructure != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(pipingStructure.SectionResults
                                                                                                                   .Single()
                                                                                                                   .ManualAssemblyCategoryGroup),
                                actualInput.Single().Single().CategoryGroup);
            }

            var strengthStabilityLengthwiseConstruction = failureMechanism as StrengthStabilityLengthwiseConstructionFailureMechanism;
            if (strengthStabilityLengthwiseConstruction != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(strengthStabilityLengthwiseConstruction.SectionResults
                                                                                                                                           .Single()
                                                                                                                                           .ManualAssemblyCategoryGroup),
                                actualInput.Single().Single().CategoryGroup);
            }

            var technicalInnovation = failureMechanism as TechnicalInnovationFailureMechanism;
            if (technicalInnovation != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(technicalInnovation.SectionResults
                                                                                                                       .Single()
                                                                                                                       .ManualAssemblyCategoryGroup),
                                actualInput.Single().Single().CategoryGroup);
            }

            var microStability = failureMechanism as MicrostabilityFailureMechanism;
            if (microStability != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(microStability.SectionResults
                                                                                                                  .Single()
                                                                                                                  .ManualAssemblyCategoryGroup),
                                actualInput.Single().Single().CategoryGroup);
            }

            var macroStabilityOutwards = failureMechanism as MacroStabilityOutwardsFailureMechanism;
            if (macroStabilityOutwards != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(macroStabilityOutwards.SectionResults
                                                                                                                          .Single()
                                                                                                                          .ManualAssemblyCategoryGroup),
                                actualInput.Single().Single().CategoryGroup);
            }

            var waterPressureAsphaltCover = failureMechanism as WaterPressureAsphaltCoverFailureMechanism;
            if (waterPressureAsphaltCover != null)
            {
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(waterPressureAsphaltCover.SectionResults
                                                                                                                             .Single()
                                                                                                                             .ManualAssemblyCategoryGroup),
                                actualInput.Single().Single().CategoryGroup);
            }

            throw new NotSupportedException();
        }

        #endregion

        #region GrassCoverSlipOffInwards

        [Test]
        public void GivenGrassCoverSlipOffInwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverSlipOffInwards
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenGrassCoverSlipOffInwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverSlipOffInwards
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region GrassCoverSlipOffOutwards

        [Test]
        public void GivenGrassCoverSlipOffOutwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverSlipOffOutwards
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenGrassCoverSlipOffOutwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.GrassCoverSlipOffOutwards
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region Microstability

        [Test]
        public void GivenMicrostabilityFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.Microstability
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenMicrostabilityFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.Microstability
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region MacroStabilityOutwards

        [Test]
        public void GivenMacroStabilityOutwardsFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.MacroStabilityOutwards
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenMacroStabilityOutwardsFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.MacroStabilityOutwards
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.VIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region PipingStructure

        [Test]
        public void GivenPipingStructureFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.PipingStructure
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenPipingStructureFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.PipingStructure
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region StrengthStabilityLengthwiseConstruction

        [Test]
        public void GivenStrengthStabilityLengthwiseConstructionFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.StrengthStabilityLengthwiseConstruction
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenStrengthStabilityLengthwiseConstructionFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.StrengthStabilityLengthwiseConstruction
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region TechnicalInnovation

        [Test]
        public void GivenTechnicalInnovationFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.TechnicalInnovation
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenTechnicalInnovationFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.TechnicalInnovation
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #region WaterPressureAsphaltCover

        [Test]
        public void GivenWaterPressureAsphaltCoverFailureMechanismAndManualAssemblyAndUseManualTrue_WhenCreatingInput_ThenManualAssemblyUsed()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaterPressureAsphaltCover;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = random.NextEnumValue<ManualFailureMechanismSectionAssemblyCategoryGroup>();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.WaterPressureAsphaltCover
                }, true);

                // Then
                Assert.AreEqual(ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(sectionResult.ManualAssemblyCategoryGroup),
                                input.Single().Single().CategoryGroup);
            }
        }

        [Test]
        public void GivenWaterPressureAsphaltCoverFailureMechanismAndManualAssemblyAndUseManualFalse_WhenCreatingInput_ThenManualAssemblyIgnored()
        {
            // Given
            var random = new Random(21);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaterPressureAsphaltCover;
            FailureMechanismTestHelper.AddSections(failureMechanism, 1);
            WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
            sectionResult.UseManualAssembly = true;
            sectionResult.ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Iv;

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    assessmentSection.WaterPressureAsphaltCover
                }, false);

                // Then
                Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.IIv, input.Single().Single().CategoryGroup);
            }
        }

        #endregion

        #endregion
    }
}