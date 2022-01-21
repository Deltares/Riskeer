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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil.FileImporters;
using Riskeer.Common.Primitives;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Plugin.FileImporters;

namespace Riskeer.GrassCoverErosionInwards.Plugin.Test.FileImporters
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        GrassCoverErosionInwardsFailureMechanismSectionResultUpdateStrategy, GrassCoverErosionInwardsFailureMechanismSectionResultOld, AdoptableWithProfileProbabilityFailureMechanismSectionResult>
    {
        protected override GrassCoverErosionInwardsFailureMechanismSectionResultOld CreateEmptySectionResultOld()
        {
            return new GrassCoverErosionInwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override GrassCoverErosionInwardsFailureMechanismSectionResultOld CreateConfiguredSectionResultOld()
        {
            var random = new Random(39);
            return new GrassCoverErosionInwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.NotApplicable,
                DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.NotAssessed,
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                TailorMadeAssessmentProbability = random.NextDouble(),
                UseManualAssembly = true,
                ManualAssemblyProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(GrassCoverErosionInwardsFailureMechanismSectionResultOld originResult,
                                                    GrassCoverErosionInwardsFailureMechanismSectionResultOld targetResult)
        {
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentProbability, targetResult.TailorMadeAssessmentProbability);
            Assert.AreEqual(originResult.UseManualAssembly, targetResult.UseManualAssembly);
            Assert.AreEqual(originResult.ManualAssemblyProbability, targetResult.ManualAssemblyProbability);
        }

        protected override AdoptableWithProfileProbabilityFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override AdoptableWithProfileProbabilityFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            var random = new Random(39);
            return new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = true,
                InitialFailureMechanismResultType = AdoptableInitialFailureMechanismResultType.Manual,
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                ManualInitialFailureMechanismResultProfileProbability = random.NextDouble(),
                FurtherAnalysisNeeded = true,
                ProbabilityRefinementType = ProbabilityRefinementType.Both,
                RefinedSectionProbability = random.NextDouble(),
                RefinedProfileProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(AdoptableWithProfileProbabilityFailureMechanismSectionResult originResult, AdoptableWithProfileProbabilityFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.IsRelevant, targetResult.IsRelevant);
            Assert.AreEqual(originResult.InitialFailureMechanismResultType, targetResult.InitialFailureMechanismResultType);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultSectionProbability, targetResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultProfileProbability, targetResult.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(originResult.FurtherAnalysisNeeded, targetResult.FurtherAnalysisNeeded);
            Assert.AreEqual(originResult.ProbabilityRefinementType, targetResult.ProbabilityRefinementType);
            Assert.AreEqual(originResult.RefinedSectionProbability, targetResult.RefinedSectionProbability);
            Assert.AreEqual(originResult.RefinedProfileProbability, targetResult.RefinedProfileProbability);
        }
    }
}