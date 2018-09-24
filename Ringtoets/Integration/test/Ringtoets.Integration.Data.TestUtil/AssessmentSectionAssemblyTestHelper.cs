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
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
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

namespace Ringtoets.Integration.Data.TestUtil
{
    /// <summary>
    /// Helper which creates configurations of <see cref="AssessmentSection"/>
    /// which can be used for testing assembly results.
    /// </summary>
    public static class AssessmentSectionAssemblyTestHelper
    {
        /// <summary>
        /// Gets a collection of <see cref="AssessmentSection"/> configurations of
        /// failure mechanisms belonging in groups 1 and 2, such that:
        /// <list type="bullet">
        /// <item>All other failure mechanisms are marked as irrelevant, except one failure mechanism.</item>
        /// <item>The relevant failure mechanism will have one section which contains manual assembly data.</item>
        /// </list>
        /// </summary>
        /// <returns>A collection of <see cref="AssessmentSection"/> configurations.</returns>
        public static IEnumerable<TestCaseData> GetConfiguredAssessmentSectionWithGroup1And2FailureMechanisms()
        {
            return GenerateTestCaseData(GetGroup1And2FailureMechanismConfigurations());
        }

        /// <summary>
        /// Gets a collection of <see cref="AssessmentSection"/> configurations of
        /// failure mechanisms belonging in group 3, such that:
        /// <list type="bullet">
        /// <item>All other failure mechanisms are marked as irrelevant, except one failure mechanism.</item>
        /// <item>The relevant failure mechanism will have one section which contains manual assembly data.</item>
        /// </list>
        /// </summary>
        /// <returns>A collection of <see cref="AssessmentSection"/> configurations.</returns>
        public static IEnumerable<TestCaseData> GetConfiguredAssessmentSectionWithGroup3FailureMechanisms()
        {
            return GenerateTestCaseData(GetGroup3FailureMechanismConfigurations());
        }

        private static IEnumerable<TestCaseData> GenerateTestCaseData(IEnumerable<FailureMechanismConfiguration> configurations)
        {
            foreach (FailureMechanismConfiguration assessmentSectionConfiguration in configurations)
            {
                AssessmentSection assessmentSection = CreateAssessmentSectionWithIrrelevantFailureMechanisms();
                assessmentSectionConfiguration.ConfigureAssessmentSection(assessmentSection);

                yield return new TestCaseData(assessmentSection).SetName(assessmentSectionConfiguration.FailureMechanismName);
            }
        }

        private static AssessmentSection CreateAssessmentSectionWithIrrelevantFailureMechanisms()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.DikeAndDune);
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.IsRelevant = false);
            return assessmentSection;
        }

        private static IEnumerable<FailureMechanismConfiguration> GetGroup1And2FailureMechanismConfigurations()
        {
            const double manualAssemblyProbability = 0.5;

            yield return new FailureMechanismConfiguration(section =>
            {
                PipingFailureMechanism failureMechanism = section.Piping;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, "Piping");

            yield return new FailureMechanismConfiguration(section =>
            {
                MacroStabilityInwardsFailureMechanism failureMechanism = section.MacroStabilityInwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MacroStabilityInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, "MacroStabilityInwards");

            yield return new FailureMechanismConfiguration(section =>
            {
                GrassCoverErosionInwardsFailureMechanism failureMechanism = section.GrassCoverErosionInwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, "GrassCoverErosionInwards");

            yield return new FailureMechanismConfiguration(section =>
            {
                ClosingStructuresFailureMechanism failureMechanism = section.ClosingStructures;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, "ClosingStructures");

            yield return new FailureMechanismConfiguration(section =>
            {
                HeightStructuresFailureMechanism failureMechanism = section.HeightStructures;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, "HeightStructures");

            yield return new FailureMechanismConfiguration(section =>
            {
                StabilityPointStructuresFailureMechanism failureMechanism = section.StabilityPointStructures;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, "StabilityPointStructures");
        }

        private static IEnumerable<FailureMechanismConfiguration> GetGroup3FailureMechanismConfigurations()
        {
            const FailureMechanismSectionAssemblyCategoryGroup sectionAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;
            const ManualFailureMechanismSectionAssemblyCategoryGroup manualSectionAssemblyCategoryGrouo = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            yield return new FailureMechanismConfiguration(section =>
            {
                DuneErosionFailureMechanism failureMechanism = section.DuneErosion;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, "DuneErosion");

            yield return new FailureMechanismConfiguration(section =>
            {
                GrassCoverErosionOutwardsFailureMechanism failureMechanism = section.GrassCoverErosionOutwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, "GrassCoverErosionOutwards");

            yield return new FailureMechanismConfiguration(section =>
            {
                StabilityStoneCoverFailureMechanism failureMechanism = section.StabilityStoneCover;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StabilityStoneCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, "StabilityStoneCover");

            yield return new FailureMechanismConfiguration(section =>
            {
                WaveImpactAsphaltCoverFailureMechanism failureMechanism = section.WaveImpactAsphaltCover;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, "WaveImpactAsphaltCover");

            yield return new FailureMechanismConfiguration(section =>
            {
                GrassCoverSlipOffInwardsFailureMechanism failureMechanism = section.GrassCoverSlipOffInwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGrouo;
            }, "GrassCoverSlipOffInwards");

            yield return new FailureMechanismConfiguration(section =>
            {
                GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = section.GrassCoverSlipOffOutwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGrouo;
            }, "GrassCoverSlipOffOutwards");

            yield return new FailureMechanismConfiguration(section =>
            {
                PipingStructureFailureMechanism failureMechanism = section.PipingStructure;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGrouo;
            }, "PipingStructure");

            yield return new FailureMechanismConfiguration(section =>
            {
                StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = section.StrengthStabilityLengthwiseConstruction;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGrouo;
            }, "StrengthStabilityLengthwiseConstruction");

            yield return new FailureMechanismConfiguration(section =>
            {
                TechnicalInnovationFailureMechanism failureMechanism = section.TechnicalInnovation;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGrouo;
            }, "TechnicalInnovation");

            yield return new FailureMechanismConfiguration(section =>
            {
                MicrostabilityFailureMechanism failureMechanism = section.Microstability;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGrouo;
            }, "Microstability");

            yield return new FailureMechanismConfiguration(section =>
            {
                MacroStabilityOutwardsFailureMechanism failureMechanism = section.MacroStabilityOutwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGrouo;
            }, "MacroStabilityOutwards");
        }

        private class FailureMechanismConfiguration
        {
            public FailureMechanismConfiguration(Action<AssessmentSection> configureAssessmentSection,
                                                 string failureMechanismName)
            {
                ConfigureAssessmentSection = configureAssessmentSection;
                FailureMechanismName = failureMechanismName;
            }

            public Action<AssessmentSection> ConfigureAssessmentSection { get; }
            public string FailureMechanismName { get; }
        }
    }
}