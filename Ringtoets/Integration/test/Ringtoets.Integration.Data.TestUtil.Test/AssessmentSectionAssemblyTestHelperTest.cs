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
            AssertPipingFailureMechanism(expectedManualProbability, piping);

            TestCaseData macroStabilityInwardsTestCase = testCases[1];
            assessmentSection = (AssessmentSection) macroStabilityInwardsTestCase.Arguments[0];
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSection.MacroStabilityInwards;
            Assert.AreSame(macroStabilityInwards, macroStabilityInwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, macroStabilityInwards);
            AssertMacroStabilityInwardsFailureMechanism(expectedManualProbability, macroStabilityInwards);

            TestCaseData grassCoverErosionInwardsTestCase = testCases[2];
            assessmentSection = (AssessmentSection) grassCoverErosionInwardsTestCase.Arguments[0];
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
            Assert.AreSame(grassCoverErosionInwards, grassCoverErosionInwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, grassCoverErosionInwards);
            AssertGrassCoverErosionInwardsFailureMechanism(expectedManualProbability, grassCoverErosionInwards);

            TestCaseData closingStructuresTestCase = testCases[3];
            assessmentSection = (AssessmentSection) closingStructuresTestCase.Arguments[0];
            ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
            Assert.AreSame(closingStructures, closingStructuresTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, closingStructures);
            AssertClosingStructuresFailureMechanism(expectedManualProbability, closingStructures);

            TestCaseData heightStructuresTestCase = testCases[4];
            assessmentSection = (AssessmentSection) heightStructuresTestCase.Arguments[0];
            HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
            Assert.AreSame(heightStructures, heightStructuresTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, heightStructures);
            AssertHeightStructuresFailureMechanism(expectedManualProbability, heightStructures);

            TestCaseData stabilityPointStructuresTestCase = testCases[5];
            assessmentSection = (AssessmentSection) stabilityPointStructuresTestCase.Arguments[0];
            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
            Assert.AreSame(stabilityPointStructures, stabilityPointStructuresTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, stabilityPointStructures);
            AssertStabilityPointStructuresFailureMechanism(expectedManualProbability, stabilityPointStructures);
        }

        [Test]
        public void GetAssessmentSectionWithoutConfiguredFailureMechanismWithProbability_Always_ReturnsExpectedTestCases()
        {
            // Call
            TestCaseData[] testCases = AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithoutConfiguredFailureMechanismWithProbability()
                                                                          .ToArray();

            // Assert
            Assert.AreEqual(6, testCases.Length);
            Assert.IsTrue(testCases.All(tc => tc.Arguments.Length == 1));

            const double expectedManualProbability = 0.5;

            TestCaseData pipingTestCase = testCases[0];
            var assessmentSection = (AssessmentSection) pipingTestCase.Arguments[0];
            PipingFailureMechanism piping = assessmentSection.Piping;
            AssertAssessmentSection(assessmentSection, piping);
            AssertPipingFailureMechanism(expectedManualProbability, piping);

            TestCaseData macroStabilityInwardsTestCase = testCases[1];
            assessmentSection = (AssessmentSection) macroStabilityInwardsTestCase.Arguments[0];
            MacroStabilityInwardsFailureMechanism macroStabilityInwards = assessmentSection.MacroStabilityInwards;
            AssertAssessmentSection(assessmentSection, macroStabilityInwards);
            AssertMacroStabilityInwardsFailureMechanism(expectedManualProbability, macroStabilityInwards);

            TestCaseData grassCoverErosionInwardsTestCase = testCases[2];
            assessmentSection = (AssessmentSection) grassCoverErosionInwardsTestCase.Arguments[0];
            GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
            AssertAssessmentSection(assessmentSection, grassCoverErosionInwards);
            AssertGrassCoverErosionInwardsFailureMechanism(expectedManualProbability, grassCoverErosionInwards);

            TestCaseData closingStructuresTestCase = testCases[3];
            assessmentSection = (AssessmentSection) closingStructuresTestCase.Arguments[0];
            ClosingStructuresFailureMechanism closingStructures = assessmentSection.ClosingStructures;
            AssertAssessmentSection(assessmentSection, closingStructures);
            AssertClosingStructuresFailureMechanism(expectedManualProbability, closingStructures);

            TestCaseData heightStructuresTestCase = testCases[4];
            assessmentSection = (AssessmentSection) heightStructuresTestCase.Arguments[0];
            HeightStructuresFailureMechanism heightStructures = assessmentSection.HeightStructures;
            AssertAssessmentSection(assessmentSection, heightStructures);
            AssertHeightStructuresFailureMechanism(expectedManualProbability, heightStructures);

            TestCaseData stabilityPointStructuresTestCase = testCases[5];
            assessmentSection = (AssessmentSection) stabilityPointStructuresTestCase.Arguments[0];
            StabilityPointStructuresFailureMechanism stabilityPointStructures = assessmentSection.StabilityPointStructures;
            AssertAssessmentSection(assessmentSection, stabilityPointStructures);
            AssertStabilityPointStructuresFailureMechanism(expectedManualProbability, stabilityPointStructures);
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
            AssertDuneErosionFailureMechanism(expectedSectionAssemblyGroup, duneErosion);

            TestCaseData grassCoverErosionOutwardsTestCase = testCases[1];
            assessmentSection = (AssessmentSection) grassCoverErosionOutwardsTestCase.Arguments[0];
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = assessmentSection.GrassCoverErosionOutwards;
            Assert.AreSame(grassCoverErosionOutwards, grassCoverErosionOutwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, grassCoverErosionOutwards);
            AssertGrassCoverErosionOutwardsFailureMechanism(expectedSectionAssemblyGroup, grassCoverErosionOutwards);

            TestCaseData stabilityStoneCoverTestCase = testCases[2];
            assessmentSection = (AssessmentSection) stabilityStoneCoverTestCase.Arguments[0];
            StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
            Assert.AreSame(stabilityStoneCover, stabilityStoneCoverTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, stabilityStoneCover);
            AssertStabilityStoneCoverFailureMechanism(expectedSectionAssemblyGroup, stabilityStoneCover);

            TestCaseData waveImpactAsphaltCoverTestCase = testCases[3];
            assessmentSection = (AssessmentSection) waveImpactAsphaltCoverTestCase.Arguments[0];
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
            Assert.AreSame(waveImpactAsphaltCover, waveImpactAsphaltCoverTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, waveImpactAsphaltCover);
            AssertWaveImpactAsphaltCoverFailureMechanism(expectedSectionAssemblyGroup, waveImpactAsphaltCover);

            TestCaseData grassCoverSlipOffInwardsTestCase = testCases[4];
            assessmentSection = (AssessmentSection) grassCoverSlipOffInwardsTestCase.Arguments[0];
            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = assessmentSection.GrassCoverSlipOffInwards;
            Assert.AreSame(grassCoverSlipOffInwards, grassCoverSlipOffInwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, grassCoverSlipOffInwards);
            AssertGrassCoverSlipOffInwardsFailureMechanism(expectedManualSectionAssemblyGroup, grassCoverSlipOffInwards);

            TestCaseData grassCoverSlipOffOutwardsTestCase = testCases[5];
            assessmentSection = (AssessmentSection) grassCoverSlipOffOutwardsTestCase.Arguments[0];
            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = assessmentSection.GrassCoverSlipOffOutwards;
            Assert.AreSame(grassCoverSlipOffOutwards, grassCoverSlipOffOutwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, grassCoverSlipOffOutwards);
            AssertGrassCoverSlipOffOutwardsFailureMechanism(expectedManualSectionAssemblyGroup, grassCoverSlipOffOutwards);

            TestCaseData pipingStructureTestCase = testCases[6];
            assessmentSection = (AssessmentSection) pipingStructureTestCase.Arguments[0];
            PipingStructureFailureMechanism pipingStructure = assessmentSection.PipingStructure;
            Assert.AreSame(pipingStructure, pipingStructureTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, pipingStructure);
            AssertPipingStructureFailureMechanism(expectedManualSectionAssemblyGroup, pipingStructure);

            TestCaseData strengthStabilityLengthWiseConstructionTestCase = testCases[7];
            assessmentSection = (AssessmentSection) strengthStabilityLengthWiseConstructionTestCase.Arguments[0];
            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = assessmentSection.StrengthStabilityLengthwiseConstruction;
            Assert.AreSame(strengthStabilityLengthwiseConstruction, strengthStabilityLengthWiseConstructionTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, strengthStabilityLengthwiseConstruction);
            AssertStrengthStabilityLengthWiseConstructionFailureMechanism(expectedManualSectionAssemblyGroup, strengthStabilityLengthwiseConstruction);

            TestCaseData technicalInnovationTestCase = testCases[8];
            assessmentSection = (AssessmentSection) technicalInnovationTestCase.Arguments[0];
            TechnicalInnovationFailureMechanism technicalInnovation = assessmentSection.TechnicalInnovation;
            Assert.AreSame(technicalInnovation, technicalInnovationTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, technicalInnovation);
            AssertTechnicalInnovationFailureMechanism(expectedManualSectionAssemblyGroup, technicalInnovation);

            TestCaseData microstabilityTestCase = testCases[9];
            assessmentSection = (AssessmentSection) microstabilityTestCase.Arguments[0];
            MicrostabilityFailureMechanism microstability = assessmentSection.Microstability;
            Assert.AreSame(microstability, microstabilityTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, microstability);
            AssertMicrostabilityFailureMechanism(expectedManualSectionAssemblyGroup, microstability);

            TestCaseData macroStabilityOutwardsTestCase = testCases[10];
            assessmentSection = (AssessmentSection) macroStabilityOutwardsTestCase.Arguments[0];
            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = assessmentSection.MacroStabilityOutwards;
            Assert.AreSame(macroStabilityOutwards, macroStabilityOutwardsTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, macroStabilityOutwards);
            AssertMacroStabilityOutwardsFailureMechanism(expectedManualSectionAssemblyGroup, macroStabilityOutwards);

            TestCaseData waterPressureAsphaltCoverTestCase = testCases[11];
            assessmentSection = (AssessmentSection) waterPressureAsphaltCoverTestCase.Arguments[0];
            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = assessmentSection.WaterPressureAsphaltCover;
            Assert.AreSame(waterPressureAsphaltCover, waterPressureAsphaltCoverTestCase.Arguments[1]);
            AssertAssessmentSection(assessmentSection, waterPressureAsphaltCover);
            AssertWaterPressureAsphaltCoverFailureMechanism(expectedManualSectionAssemblyGroup, waterPressureAsphaltCover);
        }

        [Test]
        public void GetAssessmentSectionWithoutConfiguredFailureMechanismWithoutProbability_Always_ReturnsExpectedTestCases()
        {
            // Call
            TestCaseData[] testCases = AssessmentSectionAssemblyTestHelper.GetAssessmentSectionWithoutConfiguredFailureMechanismWithoutProbability()
                                                                          .ToArray();

            // Assert
            Assert.AreEqual(12, testCases.Length);
            Assert.IsTrue(testCases.All(tc => tc.Arguments.Length == 1));

            const FailureMechanismSectionAssemblyCategoryGroup expectedSectionAssemblyGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;
            const ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            TestCaseData duneErosionTestCase = testCases[0];
            var assessmentSection = (AssessmentSection) duneErosionTestCase.Arguments[0];
            DuneErosionFailureMechanism duneErosion = assessmentSection.DuneErosion;
            AssertAssessmentSection(assessmentSection, duneErosion);
            AssertDuneErosionFailureMechanism(expectedSectionAssemblyGroup, duneErosion);

            TestCaseData grassCoverErosionOutwardsTestCase = testCases[1];
            assessmentSection = (AssessmentSection) grassCoverErosionOutwardsTestCase.Arguments[0];
            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards = assessmentSection.GrassCoverErosionOutwards;
            AssertAssessmentSection(assessmentSection, grassCoverErosionOutwards);
            AssertGrassCoverErosionOutwardsFailureMechanism(expectedSectionAssemblyGroup, grassCoverErosionOutwards);

            TestCaseData stabilityStoneCoverTestCase = testCases[2];
            assessmentSection = (AssessmentSection) stabilityStoneCoverTestCase.Arguments[0];
            StabilityStoneCoverFailureMechanism stabilityStoneCover = assessmentSection.StabilityStoneCover;
            AssertAssessmentSection(assessmentSection, stabilityStoneCover);
            AssertStabilityStoneCoverFailureMechanism(expectedSectionAssemblyGroup, stabilityStoneCover);

            TestCaseData waveImpactAsphaltCoverTestCase = testCases[3];
            assessmentSection = (AssessmentSection) waveImpactAsphaltCoverTestCase.Arguments[0];
            WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover = assessmentSection.WaveImpactAsphaltCover;
            AssertAssessmentSection(assessmentSection, waveImpactAsphaltCover);
            AssertWaveImpactAsphaltCoverFailureMechanism(expectedSectionAssemblyGroup, waveImpactAsphaltCover);

            TestCaseData grassCoverSlipOffInwardsTestCase = testCases[4];
            assessmentSection = (AssessmentSection) grassCoverSlipOffInwardsTestCase.Arguments[0];
            GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards = assessmentSection.GrassCoverSlipOffInwards;
            AssertAssessmentSection(assessmentSection, grassCoverSlipOffInwards);
            AssertGrassCoverSlipOffInwardsFailureMechanism(expectedManualSectionAssemblyGroup, grassCoverSlipOffInwards);

            TestCaseData grassCoverSlipOffOutwardsTestCase = testCases[5];
            assessmentSection = (AssessmentSection) grassCoverSlipOffOutwardsTestCase.Arguments[0];
            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards = assessmentSection.GrassCoverSlipOffOutwards;
            AssertAssessmentSection(assessmentSection, grassCoverSlipOffOutwards);
            AssertGrassCoverSlipOffOutwardsFailureMechanism(expectedManualSectionAssemblyGroup, grassCoverSlipOffOutwards);

            TestCaseData pipingStructureTestCase = testCases[6];
            assessmentSection = (AssessmentSection) pipingStructureTestCase.Arguments[0];
            PipingStructureFailureMechanism pipingStructure = assessmentSection.PipingStructure;
            AssertAssessmentSection(assessmentSection, pipingStructure);
            AssertPipingStructureFailureMechanism(expectedManualSectionAssemblyGroup, pipingStructure);

            TestCaseData strengthStabilityLengthWiseConstructionTestCase = testCases[7];
            assessmentSection = (AssessmentSection) strengthStabilityLengthWiseConstructionTestCase.Arguments[0];
            StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction = assessmentSection.StrengthStabilityLengthwiseConstruction;
            AssertAssessmentSection(assessmentSection, strengthStabilityLengthwiseConstruction);
            AssertStrengthStabilityLengthWiseConstructionFailureMechanism(expectedManualSectionAssemblyGroup, strengthStabilityLengthwiseConstruction);

            TestCaseData technicalInnovationTestCase = testCases[8];
            assessmentSection = (AssessmentSection) technicalInnovationTestCase.Arguments[0];
            TechnicalInnovationFailureMechanism technicalInnovation = assessmentSection.TechnicalInnovation;
            AssertAssessmentSection(assessmentSection, technicalInnovation);
            AssertTechnicalInnovationFailureMechanism(expectedManualSectionAssemblyGroup, technicalInnovation);

            TestCaseData microstabilityTestCase = testCases[9];
            assessmentSection = (AssessmentSection) microstabilityTestCase.Arguments[0];
            MicrostabilityFailureMechanism microstability = assessmentSection.Microstability;
            AssertAssessmentSection(assessmentSection, microstability);
            AssertMicrostabilityFailureMechanism(expectedManualSectionAssemblyGroup, microstability);

            TestCaseData macroStabilityOutwardsTestCase = testCases[10];
            assessmentSection = (AssessmentSection) macroStabilityOutwardsTestCase.Arguments[0];
            MacroStabilityOutwardsFailureMechanism macroStabilityOutwards = assessmentSection.MacroStabilityOutwards;
            AssertAssessmentSection(assessmentSection, macroStabilityOutwards);
            AssertMacroStabilityOutwardsFailureMechanism(expectedManualSectionAssemblyGroup, macroStabilityOutwards);

            TestCaseData waterPressureAsphaltCoverTestCase = testCases[11];
            assessmentSection = (AssessmentSection) waterPressureAsphaltCoverTestCase.Arguments[0];
            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover = assessmentSection.WaterPressureAsphaltCover;
            AssertAssessmentSection(assessmentSection, waterPressureAsphaltCover);
            AssertWaterPressureAsphaltCoverFailureMechanism(expectedManualSectionAssemblyGroup, waterPressureAsphaltCover);
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

        private static T GetSectionResult<T>(IHasSectionResults<T> failureMechanism) where T : FailureMechanismSectionResult
        {
            return failureMechanism.SectionResults.Single();
        }

        #region FailureMechanisms with probability

        private static void AssertPipingFailureMechanism(double expectedManualProbability, PipingFailureMechanism piping)
        {
            PipingFailureMechanismSectionResult pipingSectionResult = GetSectionResult(piping);
            Assert.IsTrue(pipingSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, pipingSectionResult.ManualAssemblyProbability);
        }

        private static void AssertMacroStabilityInwardsFailureMechanism(double expectedManualProbability, MacroStabilityInwardsFailureMechanism macroStabilityInwards)
        {
            MacroStabilityInwardsFailureMechanismSectionResult macroStabilityInwardsSectionResult = GetSectionResult(macroStabilityInwards);
            Assert.IsTrue(macroStabilityInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, macroStabilityInwardsSectionResult.ManualAssemblyProbability);
        }

        private static void AssertGrassCoverErosionInwardsFailureMechanism(double expectedManualProbability, GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards)
        {
            GrassCoverErosionInwardsFailureMechanismSectionResult grassCoverErosionInwardsSectionResult = GetSectionResult(grassCoverErosionInwards);
            Assert.IsTrue(grassCoverErosionInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, grassCoverErosionInwardsSectionResult.ManualAssemblyProbability);
        }

        private static void AssertClosingStructuresFailureMechanism(double expectedManualProbability, ClosingStructuresFailureMechanism closingStructures)
        {
            ClosingStructuresFailureMechanismSectionResult closingStructuresSectionResult = GetSectionResult(closingStructures);
            Assert.IsTrue(closingStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, closingStructuresSectionResult.ManualAssemblyProbability);
        }

        private static void AssertHeightStructuresFailureMechanism(double expectedManualProbability, HeightStructuresFailureMechanism heightStructures)
        {
            HeightStructuresFailureMechanismSectionResult heightStructuresSectionResult = GetSectionResult(heightStructures);
            Assert.IsTrue(heightStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, heightStructuresSectionResult.ManualAssemblyProbability);
        }

        private static void AssertStabilityPointStructuresFailureMechanism(double expectedManualProbability, StabilityPointStructuresFailureMechanism stabilityPointStructures)
        {
            StabilityPointStructuresFailureMechanismSectionResult stabilityPointStructuresSectionResult = GetSectionResult(stabilityPointStructures);
            Assert.IsTrue(stabilityPointStructuresSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualProbability, stabilityPointStructuresSectionResult.ManualAssemblyProbability);
        }

        #endregion

        #region FailureMechanisms without probability

        private static void AssertDuneErosionFailureMechanism(FailureMechanismSectionAssemblyCategoryGroup expectedSectionAssemblyGroup,
                                                              DuneErosionFailureMechanism duneErosion)
        {
            DuneErosionFailureMechanismSectionResult duneErosionSectionResult = GetSectionResult(duneErosion);
            Assert.IsTrue(duneErosionSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, duneErosionSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertGrassCoverErosionOutwardsFailureMechanism(FailureMechanismSectionAssemblyCategoryGroup expectedSectionAssemblyGroup,
                                                                            GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards)
        {
            GrassCoverErosionOutwardsFailureMechanismSectionResult grassCoverErosionOutwardsSectionResult = GetSectionResult(grassCoverErosionOutwards);
            Assert.IsTrue(grassCoverErosionOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, grassCoverErosionOutwardsSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertWaveImpactAsphaltCoverFailureMechanism(FailureMechanismSectionAssemblyCategoryGroup expectedSectionAssemblyGroup,
                                                                         WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover)
        {
            WaveImpactAsphaltCoverFailureMechanismSectionResult waveImpactAsphaltCoverSectionResult = GetSectionResult(waveImpactAsphaltCover);
            Assert.IsTrue(waveImpactAsphaltCoverSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, waveImpactAsphaltCoverSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertStabilityStoneCoverFailureMechanism(FailureMechanismSectionAssemblyCategoryGroup expectedSectionAssemblyGroup,
                                                                      StabilityStoneCoverFailureMechanism stabilityStoneCover)
        {
            StabilityStoneCoverFailureMechanismSectionResult stabilityStoneCoverSectionResult = GetSectionResult(stabilityStoneCover);
            Assert.IsTrue(stabilityStoneCoverSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedSectionAssemblyGroup, stabilityStoneCoverSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertGrassCoverSlipOffInwardsFailureMechanism(ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup,
                                                                           GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards)
        {
            GrassCoverSlipOffInwardsFailureMechanismSectionResult grassCoverSlipOffInwardsSectionResult = GetSectionResult(grassCoverSlipOffInwards);
            Assert.IsTrue(grassCoverSlipOffInwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, grassCoverSlipOffInwardsSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertGrassCoverSlipOffOutwardsFailureMechanism(ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup,
                                                                            GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards)
        {
            GrassCoverSlipOffOutwardsFailureMechanismSectionResult grassCoverSlipOffOutwardsSectionResult = GetSectionResult(grassCoverSlipOffOutwards);
            Assert.IsTrue(grassCoverSlipOffOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, grassCoverSlipOffOutwardsSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertPipingStructureFailureMechanism(ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup,
                                                                  PipingStructureFailureMechanism pipingStructure)
        {
            PipingStructureFailureMechanismSectionResult pipingStructureSectionResult = GetSectionResult(pipingStructure);
            Assert.IsTrue(pipingStructureSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, pipingStructureSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertStrengthStabilityLengthWiseConstructionFailureMechanism(ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup,
                                                                                          StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction)
        {
            StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult strengthStabilityLengthwiseConstructionSectionResult = GetSectionResult(strengthStabilityLengthwiseConstruction);
            Assert.IsTrue(strengthStabilityLengthwiseConstructionSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, strengthStabilityLengthwiseConstructionSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertTechnicalInnovationFailureMechanism(ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup,
                                                                      TechnicalInnovationFailureMechanism technicalInnovation)
        {
            TechnicalInnovationFailureMechanismSectionResult technicalInnovationSectionResult = GetSectionResult(technicalInnovation);
            Assert.IsTrue(technicalInnovationSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, technicalInnovationSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertMicrostabilityFailureMechanism(ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup,
                                                                 MicrostabilityFailureMechanism microstability)
        {
            MicrostabilityFailureMechanismSectionResult microstabilitySectionResult = GetSectionResult(microstability);
            Assert.IsTrue(microstabilitySectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, microstabilitySectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertMacroStabilityOutwardsFailureMechanism(ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup,
                                                                         MacroStabilityOutwardsFailureMechanism macroStabilityOutwards)
        {
            MacroStabilityOutwardsFailureMechanismSectionResult macroStabilityOutwardsSectionResult = GetSectionResult(macroStabilityOutwards);
            Assert.IsTrue(macroStabilityOutwardsSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, macroStabilityOutwardsSectionResult.ManualAssemblyCategoryGroup);
        }

        private static void AssertWaterPressureAsphaltCoverFailureMechanism(ManualFailureMechanismSectionAssemblyCategoryGroup expectedManualSectionAssemblyGroup,
                                                                            WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover)
        {
            WaterPressureAsphaltCoverFailureMechanismSectionResult waterPressureAsphaltCoverSectionResult = GetSectionResult(waterPressureAsphaltCover);
            Assert.IsTrue(waterPressureAsphaltCoverSectionResult.UseManualAssembly);
            Assert.AreEqual(expectedManualSectionAssemblyGroup, waterPressureAsphaltCoverSectionResult.ManualAssemblyCategoryGroup);
        }

        #endregion
    }
}