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
        public void GetAssessmentSectionWithConfiguredFailureMechanismsWithProbability_Always_ReturnsExpectedTestCases()
        {
            // Call
            TestCaseData[] testCases = AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithConfiguredFailureMechanismsWithProbability()
                                                                          .ToArray();

            // Assert
            Assert.AreEqual(6, testCases.Length);
            Assert.IsTrue(testCases.All(tc => tc.Arguments.Length == 2));

            const double expectedManualProbability = 0.5;

            TestCaseData pipingTestCase = testCases[0];
            var assessmentSection = (AssessmentSection) pipingTestCase.Arguments[0];
            PipingFailureMechanism piping = assessmentSection.Piping;
            Assert.AreSame(piping, pipingTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, piping);
            PipingFailureMechanismSectionResult pipingSectionResult = piping.SectionResults.Single();
            Assert.IsTrue(pipingSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, pipingSectionResult.ManualAssemblyProbability);

            TestCaseData macroStabilityInwardsTestCase = testCases[1];
            assessmentSection = (AssessmentSection) macroStabilityInwardsTestCase.Arguments[0];
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSection.MacroStabilityInwards;
            Assert.AreSame(macroStabilityInwards, macroStabilityInwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, macroStabilityInwards);
            MacroStabilityInwardsFailureMechanismSectionResult macroStabilityInwardsSectionResult = macroStabilityInwards.SectionResults.Single();
            Assert.IsTrue(macroStabilityInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, macroStabilityInwardsSectionResult.ManualAssemblyProbability);

            TestCaseData grassCoverErosionInwardsTestCase = testCases[2];
            assessmentSection = (AssessmentSection) grassCoverErosionInwardsTestCase.Arguments[0];
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
            Assert.AreSame(grassCoverErosionInwards, grassCoverErosionInwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, grassCoverErosionInwards);
            GrassCoverErosionInwardsFailureMechanismSectionResult grassCoverErosionInwardsSectionResult = grassCoverErosionInwards.SectionResults.Single();
            Assert.IsTrue(grassCoverErosionInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, grassCoverErosionInwardsSectionResult.ManualAssemblyProbability);

            TestCaseData closingStructuresTestCase = testCases[3];
            assessmentSection = (AssessmentSection) closingStructuresTestCase.Arguments[0];
            ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
            Assert.AreSame(closingStructures, closingStructuresTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, closingStructures);
            ClosingStructuresFailureMechanismSectionResult closingStructuresSectionResult = closingStructures.SectionResults.Single();
            Assert.IsTrue(closingStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, closingStructuresSectionResult.ManualAssemblyProbability);

            TestCaseData heightStructuresTestCase = testCases[4];
            assessmentSection = (AssessmentSection) heightStructuresTestCase.Arguments[0];
            HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
            Assert.AreSame(heightStructures, heightStructuresTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, heightStructures);
            HeightStructuresFailureMechanismSectionResult heightStructuresSectionResult = heightStructures.SectionResults.Single();
            Assert.IsTrue(heightStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, heightStructuresSectionResult.ManualAssemblyProbability);

            TestCaseData stabilityPointStructuresTestCase = testCases[5];
            assessmentSection = (AssessmentSection) stabilityPointStructuresTestCase.Arguments[0];
            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
            Assert.AreSame(stabilityPointStructures, stabilityPointStructuresTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, stabilityPointStructures);
            StabilityPointStructuresFailureMechanismSectionResult stabilityPointStructuresSectionResult = stabilityPointStructures.SectionResults.Single();
            Assert.IsTrue(stabilityPointStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, stabilityPointStructuresSectionResult.ManualAssemblyProbability);
        }

        [Test]
        public void GetAssessmentSectionWithConfiguredFailureMechanismsWithoutProbability_Always_ReturnsExpectedTestCases()
        {
            // Call
            TestCaseData[] testCases = AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithConfiguredFailureMechanismsWithoutProbability()
                                                                          .ToArray();

            // Assert
            Assert.AreEqual(12, testCases.Length);
            Assert.IsTrue(testCases.All(tc => tc.Arguments.Length == 2));

            const FailureMechanismSectionAssemblyCategoryGroup expectedSectionAssemblyGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;
            const ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            TestCaseData duneErosionTestCase = testCases[0];
            var assessmentSection = (AssessmentSection) duneErosionTestCase.Arguments[0];
            DuneErosionFailureMechanism duneErosion = assessmentSection.DuneErosion;
            Assert.AreSame(duneErosion, duneErosionTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, duneErosion);
            DuneErosionFailureMechanismSectionResult duneErosionSectionResult = duneErosion.SectionResults.Single();
            Assert.IsTrue(duneErosionSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, duneErosionSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData grassCoverErosionOutwardsTestCase = testCases[1];
            assessmentSection = (AssessmentSection) grassCoverErosionOutwardsTestCase.Arguments[0];
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = assessmentSection.GrassCoverErosionOutwards;
            Assert.AreSame(grassCoverErosionOutwards, grassCoverErosionOutwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, grassCoverErosionOutwards);
            GrassCoverErosionOutwardsFailureMechanismSectionResult grassCoverErosionOutwardsSectionResult = grassCoverErosionOutwards.SectionResults.Single();
            Assert.IsTrue(grassCoverErosionOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, grassCoverErosionOutwardsSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData stabilityStoneCoverTestCase = testCases[2];
            assessmentSection = (AssessmentSection) stabilityStoneCoverTestCase.Arguments[0];
            StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
            Assert.AreSame(stabilityStoneCover, stabilityStoneCoverTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, stabilityStoneCover);
            StabilityStoneCoverFailureMechanismSectionResult stabilityStoneCoverSectionResult = stabilityStoneCover.SectionResults.Single();
            Assert.IsTrue(stabilityStoneCoverSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, stabilityStoneCoverSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData waveImpactAsphaltCoverTestCase = testCases[3];
            assessmentSection = (AssessmentSection) waveImpactAsphaltCoverTestCase.Arguments[0];
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
            Assert.AreSame(waveImpactAsphaltCover, waveImpactAsphaltCoverTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, waveImpactAsphaltCover);
            WaveImpactAsphaltCoverFailureMechanismSectionResult waveImpactAsphaltCoverSectionResult = waveImpactAsphaltCover.SectionResults.Single();
            Assert.IsTrue(waveImpactAsphaltCoverSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, waveImpactAsphaltCoverSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData grassCoverSlipOffInwardsTestCase = testCases[4];
            assessmentSection = (AssessmentSection) grassCoverSlipOffInwardsTestCase.Arguments[0];
            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = assessmentSection.GrassCoverSlipOffInwards;
            Assert.AreSame(grassCoverSlipOffInwards, grassCoverSlipOffInwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, grassCoverSlipOffInwards);
            GrassCoverSlipOffInwardsFailureMechanismSectionResult grassCoverSlipOffInwardsSectionResult = grassCoverSlipOffInwards.SectionResults.Single();
            Assert.IsTrue(grassCoverSlipOffInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, grassCoverSlipOffInwardsSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData grassCoverSlipOffOutwardsTestCase = testCases[5];
            assessmentSection = (AssessmentSection) grassCoverSlipOffOutwardsTestCase.Arguments[0];
            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = assessmentSection.GrassCoverSlipOffOutwards;
            Assert.AreSame(grassCoverSlipOffOutwards, grassCoverSlipOffOutwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, grassCoverSlipOffOutwards);
            GrassCoverSlipOffOutwardsFailureMechanismSectionResult grassCoverSlipOffOutwardsSectionResult = grassCoverSlipOffOutwards.SectionResults.Single();
            Assert.IsTrue(grassCoverSlipOffOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, grassCoverSlipOffOutwardsSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData pipingStructureTestCase = testCases[6];
            assessmentSection = (AssessmentSection) pipingStructureTestCase.Arguments[0];
            PipingStructureFailureMechanism pipingStructure = assessmentSection.PipingStructure;
            Assert.AreSame(pipingStructure, pipingStructureTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, pipingStructure);
            PipingStructureFailureMechanismSectionResult pipingStructureSectionResult = pipingStructure.SectionResults.Single();
            Assert.IsTrue(pipingStructureSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, pipingStructureSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData strengthStabilityLengthWiseConstructionTestCase = testCases[7];
            assessmentSection = (AssessmentSection) strengthStabilityLengthWiseConstructionTestCase.Arguments[0];
            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = assessmentSection.StrengthStabilityLengthwiseConstruction;
            Assert.AreSame(strengthStabilityLengthwiseConstruction, strengthStabilityLengthWiseConstructionTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, strengthStabilityLengthwiseConstruction);
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult strengthStabilityLengthwiseConstructionSectionResult =
                strengthStabilityLengthwiseConstruction.SectionResults.Single();
            Assert.IsTrue(strengthStabilityLengthwiseConstructionSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, strengthStabilityLengthwiseConstructionSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData technicalInnovationTestCase = testCases[8];
            assessmentSection = (AssessmentSection) technicalInnovationTestCase.Arguments[0];
            TechnicalInnovationFailureMechanism technicalInnovation = assessmentSection.TechnicalInnovation;
            Assert.AreSame(technicalInnovation, technicalInnovationTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, technicalInnovation);
            TechnicalInnovationFailureMechanismSectionResult technicalInnovationSectionResult = technicalInnovation.SectionResults.Single();
            Assert.IsTrue(technicalInnovationSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, technicalInnovationSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData microstabilityTestCase = testCases[9];
            assessmentSection = (AssessmentSection) microstabilityTestCase.Arguments[0];
            MicrostabilityFailureMechanism microstability = assessmentSection.Microstability;
            Assert.AreSame(microstability, microstabilityTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, microstability);
            MicrostabilityFailureMechanismSectionResult microstabilitySectionResult = microstability.SectionResults.Single();
            Assert.IsTrue(microstabilitySectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, microstabilitySectionResult.ManualAssemblyCategoryGroup);

            TestCaseData macroStabilityOutwardsTestCase = testCases[10];
            assessmentSection = (AssessmentSection) macroStabilityOutwardsTestCase.Arguments[0];
            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = assessmentSection.MacroStabilityOutwards;
            Assert.AreSame(macroStabilityOutwards, macroStabilityOutwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, macroStabilityOutwards);
            MacroStabilityOutwardsFailureMechanismSectionResult macroStabilityOutwardsSectionResult = macroStabilityOutwards.SectionResults.Single();
            Assert.IsTrue(macroStabilityOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, macroStabilityOutwardsSectionResult.ManualAssemblyCategoryGroup);

            TestCaseData waterPressureAsphaltCoverTestCase = testCases[11];
            assessmentSection = (AssessmentSection) waterPressureAsphaltCoverTestCase.Arguments[0];
            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = assessmentSection.WaterPressureAsphaltCover;
            Assert.AreSame(waterPressureAsphaltCover, waterPressureAsphaltCoverTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, waterPressureAsphaltCover);
            WaterPressureAsphaltCoverFailureMechanismSectionResult waterPressureAsphaltCoverSectionResult = waterPressureAsphaltCover.SectionResults.Single();
            Assert.IsTrue(waterPressureAsphaltCoverSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, waterPressureAsphaltCoverSectionResult.ManualAssemblyCategoryGroup);
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