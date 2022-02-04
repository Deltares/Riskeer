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
using Riskeer.Common.Plugin.FileImporters;
using Riskeer.Common.Plugin.TestUtil.FileImporters;

namespace Riskeer.Common.Plugin.Test.FileImporters
{
    [TestFixture]
    public class AdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        AdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy, AdoptableWithProfileProbabilityFailureMechanismSectionResult>
    {
        protected override AdoptableWithProfileProbabilityFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override AdoptableWithProfileProbabilityFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            var random = new Random(39);
            return new AdoptableWithProfileProbabilityFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = random.NextBoolean(),
                InitialFailureMechanismResultType = random.NextEnumValue<AdoptableInitialFailureMechanismResultType>(),
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                ManualInitialFailureMechanismResultProfileProbability = random.NextDouble(),
                FurtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(),
                ProbabilityRefinementType = random.NextEnumValue<ProbabilityRefinementType>(),
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
            Assert.AreEqual(originResult.FurtherAnalysisType, targetResult.FurtherAnalysisType);
            Assert.AreEqual(originResult.ProbabilityRefinementType, targetResult.ProbabilityRefinementType);
            Assert.AreEqual(originResult.RefinedSectionProbability, targetResult.RefinedSectionProbability);
            Assert.AreEqual(originResult.RefinedProfileProbability, targetResult.RefinedProfileProbability);
        }
    }
}