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
using Ringtoets.Common.Data.FailureMechanism;
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
        /// failure mechanisms having an assembly result with probability, such that:
        /// <list type="bullet">
        /// <item>All other failure mechanisms are marked as irrelevant, except one failure mechanism.</item>
        /// <item>The relevant failure mechanism will have one section which contains manual assembly data.</item>
        /// </list>
        /// </summary>
        /// <returns>A collection of <see cref="TestCaseData"/> with <see cref="AssessmentSection"/> configurations
        /// and the affected <see cref="IFailureMechanism"/>.</returns>
        public static IEnumerable<TestCaseData> GetAssessmentSectionWithConfiguredFailureMechanismsWithProbability()
        {
            return GenerateTestCaseDataWithAffectedFailureMechanism(GetFailureMechanismWithProbabilityTestConfigurations());
        }

        /// <summary>
        /// Gets a collection of <see cref="AssessmentSection"/> configurations of
        /// failure mechanisms having an assembly result without probability, such that:
        /// <list type="bullet">
        /// <item>All other failure mechanisms are marked as irrelevant, except one failure mechanism.</item>
        /// <item>The relevant failure mechanism will have one section which contains manual assembly data.</item>
        /// </list>
        /// </summary>
        /// <returns>A collection of <see cref="TestCaseData"/> with <see cref="AssessmentSection"/> configurations
        /// and the affected <see cref="IFailureMechanism"/>.</returns>
        public static IEnumerable<TestCaseData> GetAssessmentSectionWithConfiguredFailureMechanismsWithoutProbability()
        {
            return GenerateTestCaseDataWithAffectedFailureMechanism(GetFailureMechanismsWithoutProbabilityTestConfigurations());
        }

        /// <summary>
        /// Gets a collection of <see cref="AssessmentSection"/> configurations of
        /// failure mechanisms having an assembly result with probability, such that:
        /// <list type="bullet">
        /// <item>All other failure mechanisms are marked as irrelevant, except one failure mechanism.</item>
        /// <item>The relevant failure mechanism will have one section which contains manual assembly data.</item>
        /// </list>
        /// </summary>
        /// <returns>A collection of <see cref="TestCaseData"/> with <see cref="AssessmentSection"/> configurations.</returns>
        public static IEnumerable<TestCaseData> GetAssessmentSectionWithoutConfiguredFailureMechanismWithProbability()
        {
            return GenerateTestCaseDataWithoutAffectedFailureMechanism(GetFailureMechanismWithProbabilityTestConfigurations());
        }

        /// <summary>
        /// Gets a collection of <see cref="AssessmentSection"/> configurations of
        /// failure mechanisms having an assembly result without probability, such that:
        /// <list type="bullet">
        /// <item>All other failure mechanisms are marked as irrelevant, except one failure mechanism.</item>
        /// <item>The relevant failure mechanism will have one section which contains manual assembly data.</item>
        /// </list>
        /// </summary>
        /// <returns>A collection of <see cref="TestCaseData"/> with <see cref="AssessmentSection"/> configurations.</returns>
        public static IEnumerable<TestCaseData> GetAssessmentSectionWithoutConfiguredFailureMechanismWithoutProbability()
        {
            return GenerateTestCaseDataWithoutAffectedFailureMechanism(GetFailureMechanismsWithoutProbabilityTestConfigurations());
        }

        private static IEnumerable<TestCaseData> GenerateTestCaseDataWithAffectedFailureMechanism(IEnumerable<AssessmentSectionConfiguration> configurations)
        {
            foreach (AssessmentSectionConfiguration configuration in configurations)
            {
                AssessmentSection assessmentSection = CreateAssessmentSectionWithIrrelevantFailureMechanisms();
                configuration.ConfigureAssessmentSectionAction(assessmentSection);

                IFailureMechanism configuredFailureMechanism = configuration.GetFailureMechanismFunc(assessmentSection);
                yield return new TestCaseData(assessmentSection, configuredFailureMechanism).SetName(configuredFailureMechanism.Name);
            }
        }

        private static IEnumerable<TestCaseData> GenerateTestCaseDataWithoutAffectedFailureMechanism(IEnumerable<AssessmentSectionConfiguration> configurations)
        {
            foreach (AssessmentSectionConfiguration configuration in configurations)
            {
                AssessmentSection assessmentSection = CreateAssessmentSectionWithIrrelevantFailureMechanisms();
                configuration.ConfigureAssessmentSectionAction(assessmentSection);

                IFailureMechanism configuredFailureMechanism = configuration.GetFailureMechanismFunc(assessmentSection);
                yield return new TestCaseData(assessmentSection).SetName(configuredFailureMechanism.Name);
            }
        }

        private static AssessmentSection CreateAssessmentSectionWithIrrelevantFailureMechanisms()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.DikeAndDune);
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.IsRelevant = false);
            return assessmentSection;
        }

        private static IEnumerable<AssessmentSectionConfiguration> GetFailureMechanismWithProbabilityTestConfigurations()
        {
            const double manualAssemblyProbability = 0.5;

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                PipingFailureMechanism failureMechanism = assessmentSection.Piping;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                PipingFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.Piping);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MacroStabilityInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.MacroStabilityInwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.GrassCoverErosionInwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                ClosingStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.ClosingStructures);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                HeightStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.HeightStructures);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StabilityPointStructuresFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.StabilityPointStructures);
        }

        private static IEnumerable<AssessmentSectionConfiguration> GetFailureMechanismsWithoutProbabilityTestConfigurations()
        {
            const FailureMechanismSectionAssemblyCategoryGroup sectionAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.Vv;
            const ManualFailureMechanismSectionAssemblyCategoryGroup manualSectionAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.Vv;

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                DuneErosionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.DuneErosion);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.GrassCoverErosionOutwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StabilityStoneCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.StabilityStoneCover);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                WaveImpactAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.WaveImpactAsphaltCover);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverSlipOffInwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.GrassCoverSlipOffInwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.GrassCoverSlipOffOutwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                PipingStructureFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.PipingStructure);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.StrengthStabilityLengthwiseConstruction);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                TechnicalInnovationFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.TechnicalInnovation);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MicrostabilityFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.Microstability);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MacroStabilityOutwardsFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.MacroStabilityOutwards);

            yield return new AssessmentSectionConfiguration(section =>
            {
                WaterPressureAsphaltCoverFailureMechanism failureMechanism = section.WaterPressureAsphaltCover;
                failureMechanism.IsRelevant = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                WaterPressureAsphaltCoverFailureMechanismSectionResult sectionResult = failureMechanism.SectionResults.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.WaterPressureAsphaltCover);
        }

        /// <summary>
        /// Class which holds the information to configure a failure mechanism in an <see cref="AssessmentSection"/>.
        /// </summary>
        private class AssessmentSectionConfiguration
        {
            /// <summary>
            /// Creates a new instance of <see cref="AssessmentSectionConfiguration"/>.
            /// </summary>
            /// <param name="configureAssessmentSectionAction">The <see cref="Action{T}"/> to configure a failure mechanism
            /// that belongs in a <see cref="AssessmentSection"/>.</param>
            /// <param name="getFailureMechanismFunc">The <see cref="Func{TResult}"/> to retrieve the affected failure mechanism.</param>
            public AssessmentSectionConfiguration(Action<AssessmentSection> configureAssessmentSectionAction,
                                                  Func<AssessmentSection, IFailureMechanism> getFailureMechanismFunc)
            {
                ConfigureAssessmentSectionAction = configureAssessmentSectionAction;
                GetFailureMechanismFunc = getFailureMechanismFunc;
            }

            /// <summary>
            /// Gets the <see cref="Action{T}"/> to configure a failure mechanism in an <see cref="AssessmentSection"/>.
            /// </summary>
            public Action<AssessmentSection> ConfigureAssessmentSectionAction { get; }

            /// <summary>
            /// Gets the <see cref="Func{TResult}"/> to retrieve the configured <see cref="IFailureMechanism"/>
            /// from the <see cref="AssessmentSection"/>.
            /// </summary>
            public Func<AssessmentSection, IFailureMechanism> GetFailureMechanismFunc { get; }
        }
    }
}