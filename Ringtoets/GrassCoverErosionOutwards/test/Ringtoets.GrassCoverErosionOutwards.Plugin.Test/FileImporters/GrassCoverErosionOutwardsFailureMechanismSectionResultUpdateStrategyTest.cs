﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Plugin.FileImporters;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.FileImporters
{
    [TestFixture]
    public class GrassCoverErosionOutwardsFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        GrassCoverErosionOutwardsFailureMechanismSectionResultUpdateStrategy, GrassCoverErosionOutwardsFailureMechanismSectionResult>
    {
        protected override GrassCoverErosionOutwardsFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new GrassCoverErosionOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override GrassCoverErosionOutwardsFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            return new GrassCoverErosionOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
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

        protected override void AssertSectionResult(GrassCoverErosionOutwardsFailureMechanismSectionResult originResult,
                                                    GrassCoverErosionOutwardsFailureMechanismSectionResult targetResult)
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