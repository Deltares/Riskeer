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
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.TestUtil;

namespace Ringtoets.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class CombinedAssemblyFailureMechanismSectionFactoryTest
    {
        [Test]
        public void CreateInput_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(null, Enumerable.Empty<IFailureMechanism>());

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
            TestDelegate call = () => CombinedAssemblyFailureMechanismSectionFactory.CreateInput(assessmentSection, null);

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
                    assessmentSection, assessmentSection.GetFailureMechanisms());

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
                    });

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
    }
}