﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil.FileImporters;
using Riskeer.Common.Primitives;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Plugin.FileImporters;

namespace Riskeer.WaveImpactAsphaltCover.Plugin.Test.FileImporters
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategy, WaveImpactAsphaltCoverFailureMechanismSectionResultOld>
    {
        protected override WaveImpactAsphaltCoverFailureMechanismSectionResultOld CreateEmptySectionResultOld()
        {
            return new WaveImpactAsphaltCoverFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override WaveImpactAsphaltCoverFailureMechanismSectionResultOld CreateConfiguredSectionResultOld()
        {
            return new WaveImpactAsphaltCoverFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                DetailedAssessmentResultForFactorizedSignalingNorm = DetailedAssessmentResultType.Sufficient,
                DetailedAssessmentResultForSignalingNorm = DetailedAssessmentResultType.Sufficient,
                DetailedAssessmentResultForLowerLimitNorm = DetailedAssessmentResultType.Sufficient,
                DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = DetailedAssessmentResultType.Sufficient,
                DetailedAssessmentResultForFactorizedLowerLimitNorm = DetailedAssessmentResultType.Sufficient,
                TailorMadeAssessmentResult = TailorMadeAssessmentCategoryGroupResultType.IIIv,
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.VIv
            };
        }

        protected override void AssertSectionResult(WaveImpactAsphaltCoverFailureMechanismSectionResultOld originResult,
                                                    WaveImpactAsphaltCoverFailureMechanismSectionResultOld targetResult)
        {
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResultForFactorizedSignalingNorm, targetResult.DetailedAssessmentResultForFactorizedSignalingNorm);
            Assert.AreEqual(originResult.DetailedAssessmentResultForSignalingNorm, targetResult.DetailedAssessmentResultForSignalingNorm);
            Assert.AreEqual(originResult.DetailedAssessmentResultForLowerLimitNorm, targetResult.DetailedAssessmentResultForLowerLimitNorm);
            Assert.AreEqual(originResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, targetResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(originResult.DetailedAssessmentResultForFactorizedLowerLimitNorm, targetResult.DetailedAssessmentResultForFactorizedLowerLimitNorm);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.UseManualAssembly, targetResult.UseManualAssembly);
            Assert.AreEqual(originResult.ManualAssemblyCategoryGroup, targetResult.ManualAssemblyCategoryGroup);
        }
    }
}