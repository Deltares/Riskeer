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
using Core.Common.Util.Extensions;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Primitives;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Data.TestUtil
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
        /// <item>All other failure mechanisms are marked as not part of the assembly, except one failure mechanism.</item>
        /// <item>The failure mechanism that are part of the assembly will have one section which contains manual assembly data.</item>
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
        /// <item>All other failure mechanisms are marked as not part of the assembly, except one failure mechanism.</item>
        /// <item>The failure mechanism that are part of the assembly will have one section which contains manual assembly data.</item>
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
        /// <item>All other failure mechanisms are marked as not part of the assembly, except one failure mechanism.</item>
        /// <item>The failure mechanism that are part of the assembly will have one section which contains manual assembly data.</item>
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
        /// <item>All other failure mechanisms are marked as not part of the assembly, except one failure mechanism.</item>
        /// <item>The failure mechanism that are part of the assembly will have one section which contains manual assembly data.</item>
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
                AssessmentSection assessmentSection = CreateAssessmentSectionWithFailureMechanismsNotPartOfAssembly();
                configuration.ConfigureAssessmentSectionAction(assessmentSection);

                IFailureMechanism configuredFailureMechanism = configuration.GetFailureMechanismFunc(assessmentSection);
                yield return new TestCaseData(assessmentSection, configuredFailureMechanism);
            }
        }

        private static IEnumerable<TestCaseData> GenerateTestCaseDataWithoutAffectedFailureMechanism(IEnumerable<AssessmentSectionConfiguration> configurations)
        {
            foreach (AssessmentSectionConfiguration configuration in configurations)
            {
                AssessmentSection assessmentSection = CreateAssessmentSectionWithFailureMechanismsNotPartOfAssembly();
                configuration.ConfigureAssessmentSectionAction(assessmentSection);

                yield return new TestCaseData(assessmentSection);
            }
        }

        private static AssessmentSection CreateAssessmentSectionWithFailureMechanismsNotPartOfAssembly()
        {
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.DikeAndDune);
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.InAssembly = false);
            return assessmentSection;
        }

        private static IEnumerable<AssessmentSectionConfiguration> GetFailureMechanismWithProbabilityTestConfigurations()
        {
            const double manualAssemblyProbability = 0.5;

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                PipingFailureMechanism failureMechanism = assessmentSection.Piping;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                PipingFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.Piping);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MacroStabilityInwardsFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.MacroStabilityInwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverErosionInwardsFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.GrassCoverErosionInwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                ClosingStructuresFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.ClosingStructures);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                HeightStructuresFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyProbability = manualAssemblyProbability;
            }, assessmentSection => assessmentSection.HeightStructures);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StabilityPointStructuresFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
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
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                DuneErosionFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.DuneErosion);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverErosionOutwardsFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.GrassCoverErosionOutwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StabilityStoneCoverFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.StabilityStoneCover);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                WaveImpactAsphaltCoverFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = sectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.WaveImpactAsphaltCover);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverSlipOffInwardsFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.GrassCoverSlipOffInwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.GrassCoverSlipOffOutwards);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                PipingStructureFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.PipingStructure);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.StrengthStabilityLengthwiseConstruction);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                TechnicalInnovationFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.TechnicalInnovation);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MicrostabilityFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.Microstability);

            yield return new AssessmentSectionConfiguration(assessmentSection =>
            {
                MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                MacroStabilityOutwardsFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
                sectionResult.UseManualAssembly = true;
                sectionResult.ManualAssemblyCategoryGroup = manualSectionAssemblyCategoryGroup;
            }, assessmentSection => assessmentSection.MacroStabilityOutwards);

            yield return new AssessmentSectionConfiguration(section =>
            {
                WaterPressureAsphaltCoverFailureMechanism failureMechanism = section.WaterPressureAsphaltCover;
                failureMechanism.InAssembly = true;
                FailureMechanismTestHelper.AddSections(failureMechanism, 1);

                WaterPressureAsphaltCoverFailureMechanismSectionResultOld sectionResult = failureMechanism.SectionResultsOld.Single();
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