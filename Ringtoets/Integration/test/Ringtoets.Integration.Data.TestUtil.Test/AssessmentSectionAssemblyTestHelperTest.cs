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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;

namespace Ringtoets.Integration.Data.TestUtil.Test
{
    [TestFixture]
    public class AssessmentSectionAssemblyTestHelperTest
    {
        [Test]
        public void GetConfiguredAssessmentSectionWithGroup1And2FailureMechanisms_Always_ReturnsExpectedTestCases()
        {
            // Call
            TestCaseData[] testCases = AssessmentSectionAssemblyTestHelper.GetConfiguredAssessmentSectionWithGroup1And2FailureMechanisms()
                                                                          .ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "Piping",
                "MacroStabilityInwards",
                "GrassCoverErosionInwards",
                "ClosingStructures",
                "HeightStructures",
                "StabilityPointStructures"
            }, testCases.Select(tc => tc.TestName));
            Assert.IsTrue(testCases.All(tc => tc.Arguments.Length == 1));

            var assessmentSectionWithPiping = (AssessmentSection) testCases[0].Arguments[0];
            PipingFailureMechanism piping = assessmentSectionWithPiping.Piping;
            AssertAssessmentSection(assessmentSectionWithPiping, piping);
            PipingFailureMechanismSectionResult pipingSectionResult = piping.SectionResults.Single();
            Assert.IsTrue(pipingSectionResult.UseManualAssembly);
            Assert.AreEqual(0.5, pipingSectionResult.ManualAssemblyProbability);

            var assessmentSectionWithMacroStabilityInwards = (AssessmentSection) testCases[1].Arguments[0];
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSectionWithMacroStabilityInwards.MacroStabilityInwards;
            AssertAssessmentSection(assessmentSectionWithMacroStabilityInwards, macroStabilityInwards);
            MacroStabilityInwardsFailureMechanismSectionResult macroStabilityInwardsSectionResult = macroStabilityInwards.SectionResults.Single();
            Assert.IsTrue(macroStabilityInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(0.5, macroStabilityInwardsSectionResult.ManualAssemblyProbability);

            var assessmentSectionWithGrassCoverErosionInwards = (AssessmentSection) testCases[2].Arguments[0];
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSectionWithGrassCoverErosionInwards.GrassCoverErosionInwards;
            AssertAssessmentSection(assessmentSectionWithGrassCoverErosionInwards, grassCoverErosionInwards);
            GrassCoverErosionInwardsFailureMechanismSectionResult grassCoverErosionInwardsSectionResult = grassCoverErosionInwards.SectionResults.Single();
            Assert.IsTrue(grassCoverErosionInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(0.5, grassCoverErosionInwardsSectionResult.ManualAssemblyProbability);

            var assessmentSectionWithClosingStructures = (AssessmentSection) testCases[3].Arguments[0];
            ClosingStructuresFailureMechanism closingStructures = assessmentSectionWithClosingStructures.ClosingStructures;
            AssertAssessmentSection(assessmentSectionWithClosingStructures, closingStructures);
            ClosingStructuresFailureMechanismSectionResult closingStructuresSectionResult = closingStructures.SectionResults.Single();
            Assert.IsTrue(closingStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(0.5, closingStructuresSectionResult.ManualAssemblyProbability);

            var assessmentSectionWithHeightStructures = (AssessmentSection) testCases[4].Arguments[0];
            HeightStructuresFailureMechanism heightStructures = assessmentSectionWithHeightStructures.HeightStructures;
            AssertAssessmentSection(assessmentSectionWithHeightStructures, heightStructures);
            HeightStructuresFailureMechanismSectionResult heightStructuresSectionResult = heightStructures.SectionResults.Single();
            Assert.IsTrue(heightStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(0.5, heightStructuresSectionResult.ManualAssemblyProbability);

            var assessmentSectionWithStabilityPointStructures = (AssessmentSection) testCases[5].Arguments[0];
            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSectionWithStabilityPointStructures.StabilityPointStructures;
            AssertAssessmentSection(assessmentSectionWithStabilityPointStructures, stabilityPointStructures);
            StabilityPointStructuresFailureMechanismSectionResult stabilityPointStructuresSectionResult = stabilityPointStructures.SectionResults.Single();
            Assert.IsTrue(stabilityPointStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(0.5, stabilityPointStructuresSectionResult.ManualAssemblyProbability);
        }

        private static void AssertAssessmentSection(AssessmentSection assessmentSection,
                                                    IFailureMechanism relevantFailureMechanism)
        {
            Assert.AreEqual(AssessmentSectionComposition.DikeAndDune, assessmentSection.Composition);

            IEnumerable<IFailureMechanism> irrelevantFailureMechanisms = assessmentSection.GetFailureMechanisms()
                                                                                          .Where(fm => !ReferenceEquals(relevantFailureMechanism, fm));
            AssertIrrelevantFailureMechanisms(irrelevantFailureMechanisms);
        }

        private static void AssertIrrelevantFailureMechanisms(IEnumerable<IFailureMechanism> failureMechanisms)
        {
            Assert.IsTrue(failureMechanisms.All(fm => !fm.IsRelevant));
            CollectionAssert.IsEmpty(failureMechanisms.SelectMany(fm => fm.Sections));
        }
    }
}