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

using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Plugin.TestUtil.FileImporters;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class GrassCoverSlipOffInwardsFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        GrassCoverSlipOffInwardsFailureMechanismSectionResultUpdateStrategy, GrassCoverSlipOffInwardsFailureMechanismSectionResult>
    {
        protected override GrassCoverSlipOffInwardsFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new GrassCoverSlipOffInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override GrassCoverSlipOffInwardsFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            return new GrassCoverSlipOffInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                DetailedAssessmentResult = DetailedAssessmentResultType.Sufficient,
                TailorMadeAssessmentResult = TailorMadeAssessmentResultType.ProbabilityNegligible,
                UseManualAssemblyCategoryGroup = true,
                ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv
            };
        }

        protected override void AssertSectionResult(GrassCoverSlipOffInwardsFailureMechanismSectionResult originResult,
                                                    GrassCoverSlipOffInwardsFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.UseManualAssemblyCategoryGroup, targetResult.UseManualAssemblyCategoryGroup);
            Assert.AreEqual(originResult.ManualAssemblyCategoryGroup, targetResult.ManualAssemblyCategoryGroup);
        }
    }
}