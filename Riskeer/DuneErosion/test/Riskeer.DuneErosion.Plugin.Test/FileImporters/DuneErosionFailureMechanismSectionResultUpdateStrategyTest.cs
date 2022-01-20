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

using System;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil.FileImporters;
using Riskeer.Common.Primitives;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Plugin.FileImporters;

namespace Riskeer.DuneErosion.Plugin.Test.FileImporters
{
    [TestFixture]
    public class DuneErosionFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        DuneErosionFailureMechanismSectionResultUpdateStrategy, DuneErosionFailureMechanismSectionResultOld, NonAdoptableFailureMechanismSectionResult>
    {
        protected override DuneErosionFailureMechanismSectionResultOld CreateEmptySectionResultOld()
        {
            return new DuneErosionFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override DuneErosionFailureMechanismSectionResultOld CreateConfiguredSectionResultOld()
        {
            return new DuneErosionFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.NotApplicable,
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

        protected override void AssertSectionResult(DuneErosionFailureMechanismSectionResultOld originResult,
                                                    DuneErosionFailureMechanismSectionResultOld targetResult)
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

        protected override NonAdoptableFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override NonAdoptableFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            var random = new Random(39);
            return new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = true,
                InitialFailureMechanismResultType = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability,
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                FurtherAnalysisNeeded = true,
                RefinedSectionProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(NonAdoptableFailureMechanismSectionResult originResult, NonAdoptableFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.IsRelevant, targetResult.IsRelevant);
            Assert.AreEqual(originResult.InitialFailureMechanismResultType, targetResult.InitialFailureMechanismResultType);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultSectionProbability, targetResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(originResult.FurtherAnalysisNeeded, targetResult.FurtherAnalysisNeeded);
            Assert.AreEqual(originResult.RefinedSectionProbability, targetResult.RefinedSectionProbability);
        }
    }
}