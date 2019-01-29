// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Data.Test.Assembly
{
    [TestFixture]
    public class AssessmentSectionHelperTest
    {
        [Test]
        public void HasManualAssemblyResults_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => AssessmentSectionHelper.HasManualAssemblyResults(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void HasManualAssemblyResults_AssessmentSectionWithoutManualAssemblyResults_ReturnsFalse()
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(true);

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.IsFalse(hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_FailureMechanismHasManualAssemblyResult_ReturnsExpectedValue(
            Func<AssessmentSection, IHasSectionResults<FailureMechanismSectionResult>> getFailureMechanismFunc,
            bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            IHasSectionResults<FailureMechanismSectionResult> failureMechanism = getFailureMechanismFunc(assessmentSection);
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssembly = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        private static IEnumerable<TestCaseData> GetHasManualAssemblyResultCases()
        {
            Func<AssessmentSection, IHasSectionResults<FailureMechanismSectionResult>>[] getFailureMechanismFuncs =
            {
                assessmentSection => assessmentSection.Piping,
                assessmentSection => assessmentSection.MacroStabilityInwards,
                assessmentSection => assessmentSection.GrassCoverErosionInwards,
                assessmentSection => assessmentSection.ClosingStructures,
                assessmentSection => assessmentSection.HeightStructures,
                assessmentSection => assessmentSection.StabilityPointStructures,
                assessmentSection => assessmentSection.GrassCoverErosionOutwards,
                assessmentSection => assessmentSection.DuneErosion,
                assessmentSection => assessmentSection.WaveImpactAsphaltCover,
                assessmentSection => assessmentSection.StabilityStoneCover,
                assessmentSection => assessmentSection.GrassCoverSlipOffInwards,
                assessmentSection => assessmentSection.GrassCoverSlipOffOutwards,
                assessmentSection => assessmentSection.MacroStabilityOutwards,
                assessmentSection => assessmentSection.Microstability,
                assessmentSection => assessmentSection.PipingStructure,
                assessmentSection => assessmentSection.StrengthStabilityLengthwiseConstruction,
                assessmentSection => assessmentSection.TechnicalInnovation,
                assessmentSection => assessmentSection.WaterPressureAsphaltCover
            };

            foreach (Func<AssessmentSection, IHasSectionResults<FailureMechanismSectionResult>> func in getFailureMechanismFuncs)
            {
                yield return new TestCaseData(func, true, true);
                yield return new TestCaseData(func, false, false);
            }
        }

        private static AssessmentSection CreateConfiguredAssessmentSection(bool failureMechanismsRelevant)
        {
            var random = new Random(39);
            var assessmentSection = new AssessmentSection(random.NextEnumValue<AssessmentSectionComposition>());
            assessmentSection.GetFailureMechanisms().ForEachElementDo(fm => fm.IsRelevant = failureMechanismsRelevant);
            const int nrOfSections = 1;
            FailureMechanismTestHelper.AddSections(assessmentSection.Piping, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityInwards, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionInwards, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.HeightStructures, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.ClosingStructures, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityPointStructures, nrOfSections);

            FailureMechanismTestHelper.AddSections(assessmentSection.StabilityStoneCover, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.WaveImpactAsphaltCover, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverErosionOutwards, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.DuneErosion, nrOfSections);

            FailureMechanismTestHelper.AddSections(assessmentSection.MacroStabilityOutwards, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.Microstability, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffOutwards, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.GrassCoverSlipOffInwards, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.PipingStructure, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.WaterPressureAsphaltCover, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.StrengthStabilityLengthwiseConstruction, nrOfSections);
            FailureMechanismTestHelper.AddSections(assessmentSection.TechnicalInnovation, nrOfSections);

            return assessmentSection;
        }
    }
}