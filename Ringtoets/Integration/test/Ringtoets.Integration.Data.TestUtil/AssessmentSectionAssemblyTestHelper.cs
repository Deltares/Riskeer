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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;

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
        /// <item>The aforementioned failure mechanism will have one section which contains manual assembly data.</item>
        /// </list>
        /// </summary>
        /// <returns>A collection of <see cref="AssessmentSection"/> configurations.</returns>
        public static IEnumerable<TestCaseData> GetConfiguredAssessmentSectionWithGroup1And2FailureMechanisms()
        {
            foreach (FailureMechanismConfiguration assessmentSectionConfiguration in GetGroup1And2FailureMechanismConfigurations())
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