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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Primitives;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

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

            const double expectedManualProbability = 0.5;

            var assessmentSection = (AssessmentSection) testCases[0].Arguments[0];
            PipingFailureMechanism piping = assessmentSection.Piping;
            AssertAssessmentSection(assessmentSection, piping);
            PipingFailureMechanismSectionResult pipingSectionResult = piping.SectionResults.Single();
            Assert.IsTrue(pipingSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, pipingSectionResult.ManualAssemblyProbability);

            assessmentSection = (AssessmentSection) testCases[1].Arguments[0];
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSection.MacroStabilityInwards;
            AssertAssessmentSection(assessmentSection, macroStabilityInwards);
            MacroStabilityInwardsFailureMechanismSectionResult macroStabilityInwardsSectionResult = macroStabilityInwards.SectionResults.Single();
            Assert.IsTrue(macroStabilityInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, macroStabilityInwardsSectionResult.ManualAssemblyProbability);

            assessmentSection = (AssessmentSection) testCases[2].Arguments[0];
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
            AssertAssessmentSection(assessmentSection, grassCoverErosionInwards);
            GrassCoverErosionInwardsFailureMechanismSectionResult grassCoverErosionInwardsSectionResult = grassCoverErosionInwards.SectionResults.Single();
            Assert.IsTrue(grassCoverErosionInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, grassCoverErosionInwardsSectionResult.ManualAssemblyProbability);

            assessmentSection = (AssessmentSection) testCases[3].Arguments[0];
            ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
            AssertAssessmentSection(assessmentSection, closingStructures);
            ClosingStructuresFailureMechanismSectionResult closingStructuresSectionResult = closingStructures.SectionResults.Single();
            Assert.IsTrue(closingStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, closingStructuresSectionResult.ManualAssemblyProbability);

            assessmentSection = (AssessmentSection) testCases[4].Arguments[0];
            HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
            AssertAssessmentSection(assessmentSection, heightStructures);
            HeightStructuresFailureMechanismSectionResult heightStructuresSectionResult = heightStructures.SectionResults.Single();
            Assert.IsTrue(heightStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, heightStructuresSectionResult.ManualAssemblyProbability);

            assessmentSection = (AssessmentSection) testCases[5].Arguments[0];
            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
            AssertAssessmentSection(assessmentSection, stabilityPointStructures);
            StabilityPointStructuresFailureMechanismSectionResult stabilityPointStructuresSectionResult = stabilityPointStructures.SectionResults.Single();
            Assert.IsTrue(stabilityPointStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, stabilityPointStructuresSectionResult.ManualAssemblyProbability);
        }

        [Test]
        public void GetConfiguredAssessmentSectionWithGroup3FailureMechanisms_Always_ReturnsExpectedTestCases()
        {
            // Call
            TestCaseData[] testCases = AssessmentSectionAssemblyTestHelper.GetConfiguredAssessmentSectionWithGroup3FailureMechanisms()
                                                                          .ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                "DuneErosion",
                "GrassCoverErosionOutwards",
                "StabilityStoneCover",
                "WaveImpactAsphaltCover",
                "GrassCoverSlipOffInwards",
                "GrassCoverSlipOffOutwards",
                "PipingStructure",
                "StrengthStabilityLengthwiseConstruction",
                "TechnicalInnovation",
                "Microstability",
                "MacroStabilityOutwards"
            }, testCases.Select(tc => tc.TestName));
            Assert.IsTrue(testCases.All(tc => tc.Arguments.Length == 1));

            const FailureMechanismSectionAssemblyCategoryGroup expectedSectionAssemblyGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;
            const ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            var assessmentSection = (AssessmentSection) testCases[0].Arguments[0];
            DuneErosionFailureMechanism duneErosion = assessmentSection.DuneErosion;
            AssertAssessmentSection(assessmentSection, duneErosion);
            DuneErosionFailureMechanismSectionResult duneErosionSectionResult = duneErosion.SectionResults.Single();
            Assert.IsTrue(duneErosionSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, duneErosionSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[1].Arguments[0];
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = assessmentSection.GrassCoverErosionOutwards;
            AssertAssessmentSection(assessmentSection, grassCoverErosionOutwards);
            GrassCoverErosionOutwardsFailureMechanismSectionResult grassCoverErosionOutwardsSectionResult = grassCoverErosionOutwards.SectionResults.Single();
            Assert.IsTrue(grassCoverErosionOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, grassCoverErosionOutwardsSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[2].Arguments[0];
            StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
            AssertAssessmentSection(assessmentSection, stabilityStoneCover);
            StabilityStoneCoverFailureMechanismSectionResult stabilityStoneCoverSectionResult = stabilityStoneCover.SectionResults.Single();
            Assert.IsTrue(stabilityStoneCoverSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, stabilityStoneCoverSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[3].Arguments[0];
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
            AssertAssessmentSection(assessmentSection, waveImpactAsphaltCover);
            WaveImpactAsphaltCoverFailureMechanismSectionResult waveImpactAsphaltCoverSectionResult = waveImpactAsphaltCover.SectionResults.Single();
            Assert.IsTrue(waveImpactAsphaltCoverSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, waveImpactAsphaltCoverSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[4].Arguments[0];
            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = assessmentSection.GrassCoverSlipOffInwards;
            AssertAssessmentSection(assessmentSection, grassCoverSlipOffInwards);
            GrassCoverSlipOffInwardsFailureMechanismSectionResult grassCoverSlipOffInwardsSectionResult = grassCoverSlipOffInwards.SectionResults.Single();
            Assert.IsTrue(grassCoverSlipOffInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, grassCoverSlipOffInwardsSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[5].Arguments[0];
            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = assessmentSection.GrassCoverSlipOffOutwards;
            AssertAssessmentSection(assessmentSection, grassCoverSlipOffOutwards);
            GrassCoverSlipOffOutwardsFailureMechanismSectionResult grassCoverSlipOffOutwardsSectionResult = grassCoverSlipOffOutwards.SectionResults.Single();
            Assert.IsTrue(grassCoverSlipOffOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, grassCoverSlipOffOutwardsSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[6].Arguments[0];
            PipingStructureFailureMechanism pipingStructure = assessmentSection.PipingStructure;
            AssertAssessmentSection(assessmentSection, pipingStructure);
            PipingStructureFailureMechanismSectionResult pipingStructureSectionResult = pipingStructure.SectionResults.Single();
            Assert.IsTrue(pipingStructureSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, pipingStructureSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[7].Arguments[0];
            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = assessmentSection.StrengthStabilityLengthwiseConstruction;
            AssertAssessmentSection(assessmentSection, strengthStabilityLengthwiseConstruction);
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult strengthStabilityLengthwiseConstructionSectionResult =
                strengthStabilityLengthwiseConstruction.SectionResults.Single();
            Assert.IsTrue(strengthStabilityLengthwiseConstructionSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, strengthStabilityLengthwiseConstructionSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[8].Arguments[0];
            TechnicalInnovationFailureMechanism technicalInnovation = assessmentSection.TechnicalInnovation;
            AssertAssessmentSection(assessmentSection, technicalInnovation);
            TechnicalInnovationFailureMechanismSectionResult technicalInnovationSectionResult = technicalInnovation.SectionResults.Single();
            Assert.IsTrue(technicalInnovationSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, technicalInnovationSectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[9].Arguments[0];
            MicrostabilityFailureMechanism microstability = assessmentSection.Microstability;
            AssertAssessmentSection(assessmentSection, microstability);
            MicrostabilityFailureMechanismSectionResult microstabilitySectionResult = microstability.SectionResults.Single();
            Assert.IsTrue(microstabilitySectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, microstabilitySectionResult.ManualAssemblyCategoryGroup);

            assessmentSection = (AssessmentSection) testCases[10].Arguments[0];
            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = assessmentSection.MacroStabilityOutwards;
            AssertAssessmentSection(assessmentSection, macroStabilityOutwards);
            MacroStabilityOutwardsFailureMechanismSectionResult macroStabilityOutwardsSectionResult = macroStabilityOutwards.SectionResults.Single();
            Assert.IsTrue(macroStabilityOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, macroStabilityOutwardsSectionResult.ManualAssemblyCategoryGroup);
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