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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.Assembly;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data.Test.Assembly
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
        public void HasManualAssemblyResults_PipingHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            PipingFailureMechanism failureMechanism = assessmentSection.Piping;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyProbability = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_MacroStabilityInwardsHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            MacroStabilityInwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityInwards;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyProbability = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_GrassCoverErosionInwardsHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            GrassCoverErosionInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionInwards;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyProbability = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_ClosingStructuresHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            ClosingStructuresFailureMechanism failureMechanism = assessmentSection.ClosingStructures;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyProbability = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_HeightStructuresHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            HeightStructuresFailureMechanism failureMechanism = assessmentSection.HeightStructures;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyProbability = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_StabilityPointStructuresHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            StabilityPointStructuresFailureMechanism failureMechanism = assessmentSection.StabilityPointStructures;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyProbability = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_GrassCoverErosionOutwardsHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverErosionOutwards;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_DuneErosionHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            DuneErosionFailureMechanism failureMechanism = assessmentSection.DuneErosion;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_StabilityStoneCoverHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            StabilityStoneCoverFailureMechanism failureMechanism = assessmentSection.StabilityStoneCover;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_WaveImpactAsphaltCoverHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaveImpactAsphaltCover;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_GrassCoverSlipOffInwardsHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            GrassCoverSlipOffInwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffInwards;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_GrassCoverSlipOffOutwardsHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            GrassCoverSlipOffOutwardsFailureMechanism failureMechanism = assessmentSection.GrassCoverSlipOffOutwards;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_MacroStabilityOutwardsHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            MacroStabilityOutwardsFailureMechanism failureMechanism = assessmentSection.MacroStabilityOutwards;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_MicrostabilityHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            MicrostabilityFailureMechanism failureMechanism = assessmentSection.Microstability;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_PipingStructureHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            PipingStructureFailureMechanism failureMechanism = assessmentSection.PipingStructure;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_StrengthStabilityLengthwiseConstructionHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism = assessmentSection.StrengthStabilityLengthwiseConstruction;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_TechnicalInnovationHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            TechnicalInnovationFailureMechanism failureMechanism = assessmentSection.TechnicalInnovation;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        [Test]
        [TestCaseSource(nameof(GetHasManualAssemblyResultCases))]
        public void HasManualAssemblyResults_WaterPressureAsphaltCoverHasManualAssemblyResult_ReturnsExpectedValue(bool isRelevant, bool expectedResult)
        {
            // Setup
            AssessmentSection assessmentSection = CreateConfiguredAssessmentSection(false);
            WaterPressureAsphaltCoverFailureMechanism failureMechanism = assessmentSection.WaterPressureAsphaltCover;
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.SectionResults.Single().UseManualAssemblyCategoryGroup = true;

            // Call
            bool hasManualAssemblyResults = AssessmentSectionHelper.HasManualAssemblyResults(assessmentSection);

            // Assert
            Assert.AreEqual(expectedResult, hasManualAssemblyResults);
        }

        private static IEnumerable<TestCaseData> GetHasManualAssemblyResultCases()
        {
            yield return new TestCaseData(true, true);
            yield return new TestCaseData(false, false);
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