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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil.FileImporters;
using Riskeer.Common.Primitives;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Plugin.FileImporters;

namespace Riskeer.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class GrassCoverSlipOffOutwardsFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        GrassCoverSlipOffOutwardsFailureMechanismSectionResultUpdateStrategy, GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>
    {
        protected override GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld CreateEmptySectionResultOld()
        {
            return new GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld CreateConfiguredSectionResultOld()
        {
            return new GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                DetailedAssessmentResult = DetailedAssessmentResultType.Sufficient,
                TailorMadeAssessmentResult = TailorMadeAssessmentResultType.ProbabilityNegligible,
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv
            };
        }

        protected override void AssertSectionResult(GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld originResult,
                                                    GrassCoverSlipOffOutwardsFailureMechanismSectionResultOld targetResult)
        {
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.UseManualAssembly, targetResult.UseManualAssembly);
            Assert.AreEqual(originResult.ManualAssemblyCategoryGroup, targetResult.ManualAssemblyCategoryGroup);
        }

        protected override NonAdoptableWithProfileProbabilityFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override NonAdoptableWithProfileProbabilityFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            var random = new Random(39);
            return new NonAdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = random.NextBoolean(),
                InitialFailureMechanismResultType = random.NextEnumValue<NonAdoptableInitialFailureMechanismResultType>(),
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                ManualInitialFailureMechanismResultProfileProbability = random.NextDouble(),
                FurtherAnalysisNeeded = random.NextBoolean(),
                RefinedSectionProbability = random.NextDouble(),
                RefinedProfileProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(NonAdoptableWithProfileProbabilityFailureMechanismSectionResult originResult, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.IsRelevant, targetResult.IsRelevant);
            Assert.AreEqual(originResult.InitialFailureMechanismResultType, targetResult.InitialFailureMechanismResultType);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultSectionProbability, targetResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultProfileProbability, targetResult.ManualInitialFailureMechanismResultProfileProbability);
            Assert.AreEqual(originResult.FurtherAnalysisNeeded, targetResult.FurtherAnalysisNeeded);
            Assert.AreEqual(originResult.RefinedSectionProbability, targetResult.RefinedSectionProbability);
            Assert.AreEqual(originResult.RefinedProfileProbability, targetResult.RefinedProfileProbability);
        }
    }
}