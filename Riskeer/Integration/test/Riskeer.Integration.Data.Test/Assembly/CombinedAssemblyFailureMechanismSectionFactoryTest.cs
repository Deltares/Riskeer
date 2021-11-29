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
using System.Linq;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly;
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.TestUtil;
using Riskeer.Integration.TestUtil;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Data.Test.Assembly
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
        public void CreateInput_WithAllFailureMechanismsAndUseManualFalse_ReturnsInputCollection()
        {
            // Setup
            var random = new Random(21);
            AssessmentSection assessmentSection = TestDataGenerator.GetAssessmentSectionWithAllFailureMechanismSectionsAndResults(
                random.NextEnumValue<AssessmentSectionComposition>());

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, assessmentSection.GetFailureMechanisms(), random.NextBoolean());

                // Assert
                Assert.AreEqual(18, inputs.Count());

                AssertSections(assessmentSection.Piping.SectionResultsOld, inputs.ElementAt(0));
                AssertSections(assessmentSection.GrassCoverErosionInwards.SectionResultsOld, inputs.ElementAt(1));
                AssertSections(assessmentSection.MacroStabilityInwards.SectionResultsOld, inputs.ElementAt(2));
                AssertSections(assessmentSection.MacroStabilityOutwards.SectionResultsOld, inputs.ElementAt(3));
                AssertSections(assessmentSection.Microstability.SectionResultsOld, inputs.ElementAt(4));
                AssertSections(assessmentSection.StabilityStoneCover.SectionResultsOld, inputs.ElementAt(5));
                AssertSections(assessmentSection.WaveImpactAsphaltCover.SectionResultsOld, inputs.ElementAt(6));
                AssertSections(assessmentSection.WaterPressureAsphaltCover.SectionResultsOld, inputs.ElementAt(7));
                AssertSections(assessmentSection.GrassCoverErosionOutwards.SectionResultsOld, inputs.ElementAt(8));
                AssertSections(assessmentSection.GrassCoverSlipOffOutwards.SectionResultsOld, inputs.ElementAt(9));
                AssertSections(assessmentSection.GrassCoverSlipOffInwards.SectionResultsOld, inputs.ElementAt(10));
                AssertSections(assessmentSection.HeightStructures.SectionResultsOld, inputs.ElementAt(11));
                AssertSections(assessmentSection.ClosingStructures.SectionResultsOld, inputs.ElementAt(12));
                AssertSections(assessmentSection.PipingStructure.SectionResultsOld, inputs.ElementAt(13));
                AssertSections(assessmentSection.StabilityPointStructures.SectionResultsOld, inputs.ElementAt(14));
                AssertSections(assessmentSection.StrengthStabilityLengthwiseConstruction.SectionResultsOld, inputs.ElementAt(15));
                AssertSections(assessmentSection.DuneErosion.SectionResultsOld, inputs.ElementAt(16));
                AssertSections(assessmentSection.TechnicalInnovation.SectionResultsOld, inputs.ElementAt(17));
            }
        }

        [Test]
        [TestCaseSource(nameof(GetFailureMechanismTestCaseData))]
        public void CreateInput_WithOneFailureMechanism_ReturnsInputCollection(AssessmentSection assessmentSection, IFailureMechanism failureMechanismInAssembly)
        {
            // Setup
            assessmentSection.GetFailureMechanisms().ForEachElementDo(failureMechanism => failureMechanism.InAssembly = failureMechanism == failureMechanismInAssembly);

            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> inputs = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(
                    assessmentSection, new[]
                    {
                        failureMechanismInAssembly
                    }, new Random(39).NextBoolean());

                // Assert
                AssertSections(((IHasSectionResults<FailureMechanismSectionResultOld>) failureMechanismInAssembly).SectionResultsOld, inputs.Single());
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

        private static void AssertSectionsWithResult<T>(IEnumerable<T> originalSectionResults,
                                                        FailureMechanismSectionAssemblyCategoryGroup expectedAssemblyCategoryGroupInput,
                                                        IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
            where T : FailureMechanismSectionResultOld
        {
            AssertSections(originalSectionResults, inputSections);

            for (var i = 0; i < originalSectionResults.Count(); i++)
            {
                Assert.AreEqual(expectedAssemblyCategoryGroupInput, inputSections.ElementAt(i).CategoryGroup);
            }
        }

        private static void AssertSections<T>(IEnumerable<T> originalSectionResults, IEnumerable<CombinedAssemblyFailureMechanismSection> inputSections)
            where T : FailureMechanismSectionResultOld
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

        #region Manual Assembly

        #region Failure mechanisms with probability

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithConfiguredFailureMechanismsWithProbability))]
        public void CreateInput_AssessmentSectionWithFailureMechanismWithProbabilityWithManualAssemblyAndUseManualTrue_ReturnsInputWithExpectedValue(AssessmentSection assessmentSection,
                                                                                                                                                     IFailureMechanism failureMechanismInAssembly)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    failureMechanismInAssembly
                }, true);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                AssertSectionsWithResult(((IHasSectionResults<FailureMechanismSectionResultOld>) failureMechanismInAssembly).SectionResultsOld,
                                         sectionCalculator.ManualAssemblyAssemblyOutput.Group,
                                         input.Single());
            }
        }

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithConfiguredFailureMechanismsWithProbability))]
        public void CreateInput_AssessmentSectionWithFailureMechanismWithProbabilityWithManualAssemblyAndUseManualFalse_ReturnsInputWithExpectedValue(AssessmentSection assessmentSection,
                                                                                                                                                      IFailureMechanism failureMechanismInAssembly)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    failureMechanismInAssembly
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                AssertSectionsWithResult(((IHasSectionResults<FailureMechanismSectionResultOld>) failureMechanismInAssembly).SectionResultsOld,
                                         sectionCalculator.CombinedAssemblyOutput.Group,
                                         input.Single());
            }
        }

        #endregion

        #region Failure mechanisms without probability

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithConfiguredFailureMechanismsWithoutProbability))]
        public void CreateInput_AssessmentSectionWithFailureMechanismWithoutProbabilityWithManualAssemblyAndUseManualTrue_ReturnsInputWithExpectedValue(AssessmentSection assessmentSection,
                                                                                                                                                        IFailureMechanism failureMechanismInAssembly)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                // Call
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    failureMechanismInAssembly
                }, true);

                // Then
                AssertInputForFailureMechanismsWithoutProbabilityWithManualAssembly(failureMechanismInAssembly, input);
            }
        }

        [Test]
        [TestCaseSource(typeof(AssessmentSectionAssemblyTestHelper), nameof(AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithConfiguredFailureMechanismsWithoutProbability))]
        public void CreateInput_AssessmentSectionWithFailureMechanismWithoutProbabilityWithManualAssemblyAndUseManualFalse_ReturnsInputWithExpectedValue(AssessmentSection assessmentSection,
                                                                                                                                                         IFailureMechanism failureMechanismInAssembly)
        {
            // Setup
            using (new AssemblyToolCalculatorFactoryConfigOld())
            {
                // When
                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> input = CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, new[]
                {
                    failureMechanismInAssembly
                }, false);

                // Then
                var calculatorFactory = (TestAssemblyToolCalculatorFactoryOld) AssemblyToolCalculatorFactoryOld.Instance;
                FailureMechanismSectionAssemblyCalculatorOldStub sectionCalculator = calculatorFactory.LastCreatedFailureMechanismSectionAssemblyCalculator;
                Assert.AreEqual(sectionCalculator.CombinedAssemblyCategoryOutput, input.Single().Single().CategoryGroup);
            }
        }

        private static void AssertInputForFailureMechanismsWithoutProbabilityWithManualAssembly(IFailureMechanism failureMechanism,
                                                                                                IEnumerable<IEnumerable<CombinedAssemblyFailureMechanismSection>> actualInput)
        {
            var duneErosion = failureMechanism as DuneErosionFailureMechanism;
            if (duneErosion != null)
            {
                AssertSectionsWithResult(duneErosion.SectionResultsOld,
                                         GetFailureMechanismSectionResult(duneErosion).ManualAssemblyCategoryGroup,
                                         actualInput.Single());
                return;
            }

            var grassCoverErosionOutwards = failureMechanism as GrassCoverErosionOutwardsFailureMechanism;
            if (grassCoverErosionOutwards != null)
            {
                AssertSectionsWithResult(grassCoverErosionOutwards.SectionResultsOld,
                                         GetFailureMechanismSectionResult(grassCoverErosionOutwards).ManualAssemblyCategoryGroup,
                                         actualInput.Single());
                return;
            }

            var stabilityStoneCover = failureMechanism as StabilityStoneCoverFailureMechanism;
            if (stabilityStoneCover != null)
            {
                AssertSectionsWithResult(stabilityStoneCover.SectionResultsOld,
                                         GetFailureMechanismSectionResult(stabilityStoneCover).ManualAssemblyCategoryGroup,
                                         actualInput.Single());
                return;
            }

            var waveImpactAsphaltCover = failureMechanism as WaveImpactAsphaltCoverFailureMechanism;
            if (waveImpactAsphaltCover != null)
            {
                AssertSectionsWithResult(waveImpactAsphaltCover.SectionResultsOld,
                                         GetFailureMechanismSectionResult(waveImpactAsphaltCover).ManualAssemblyCategoryGroup,
                                         actualInput.Single());
                return;
            }

            var grassCoverSlipOffInwards = failureMechanism as GrassCoverSlipOffInwardsFailureMechanism;
            if (grassCoverSlipOffInwards != null)
            {
                AssertSectionsWithResult(grassCoverSlipOffInwards.SectionResultsOld,
                                         ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(grassCoverSlipOffInwards).ManualAssemblyCategoryGroup),
                                         actualInput.Single());
                return;
            }

            var grassCoverSlipOffOutwards = failureMechanism as GrassCoverSlipOffOutwardsFailureMechanism;
            if (grassCoverSlipOffOutwards != null)
            {
                AssertSectionsWithResult(grassCoverSlipOffOutwards.SectionResultsOld,
                                         ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(grassCoverSlipOffOutwards).ManualAssemblyCategoryGroup),
                                         actualInput.Single());
                return;
            }

            var pipingStructure = failureMechanism as PipingStructureFailureMechanism;
            if (pipingStructure != null)
            {
                AssertSectionsWithResult(pipingStructure.SectionResultsOld,
                                         ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(pipingStructure).ManualAssemblyCategoryGroup),
                                         actualInput.Single());
                return;
            }

            var strengthStabilityLengthwiseConstruction = failureMechanism as StrengthStabilityLengthwiseConstructionFailureMechanism;
            if (strengthStabilityLengthwiseConstruction != null)
            {
                AssertSectionsWithResult(strengthStabilityLengthwiseConstruction.SectionResultsOld,
                                         ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(strengthStabilityLengthwiseConstruction).ManualAssemblyCategoryGroup),
                                         actualInput.Single());
                return;
            }

            var technicalInnovation = failureMechanism as TechnicalInnovationFailureMechanism;
            if (technicalInnovation != null)
            {
                AssertSectionsWithResult(technicalInnovation.SectionResultsOld,
                                         ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(technicalInnovation).ManualAssemblyCategoryGroup),
                                         actualInput.Single());
                return;
            }

            var microStability = failureMechanism as MicrostabilityFailureMechanism;
            if (microStability != null)
            {
                AssertSectionsWithResult(microStability.SectionResultsOld,
                                         ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(microStability).ManualAssemblyCategoryGroup),
                                         actualInput.Single());
                return;
            }

            var macroStabilityOutwards = failureMechanism as MacroStabilityOutwardsFailureMechanism;
            if (macroStabilityOutwards != null)
            {
                AssertSectionsWithResult(macroStabilityOutwards.SectionResultsOld,
                                         ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(macroStabilityOutwards).ManualAssemblyCategoryGroup),
                                         actualInput.Single());
                return;
            }

            var waterPressureAsphaltCover = failureMechanism as WaterPressureAsphaltCoverFailureMechanism;
            if (waterPressureAsphaltCover != null)
            {
                AssertSectionsWithResult(waterPressureAsphaltCover.SectionResultsOld,
                                         ManualFailureMechanismSectionAssemblyCategoryGroupConverter.Convert(GetFailureMechanismSectionResult(waterPressureAsphaltCover).ManualAssemblyCategoryGroup),
                                         actualInput.Single());
                return;
            }

            throw new NotSupportedException();
        }

        private static T GetFailureMechanismSectionResult<T>(IHasSectionResults<T> failureMechanism) where T : FailureMechanismSectionResultOld
        {
            return failureMechanism.SectionResultsOld.Single();
        }

        #endregion

        #endregion
    }
}