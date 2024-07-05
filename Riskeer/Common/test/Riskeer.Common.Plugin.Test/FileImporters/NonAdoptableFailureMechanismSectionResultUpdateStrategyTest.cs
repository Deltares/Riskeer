// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Primitives;

namespace Riskeer.Common.Plugin.Test.FileImporters
{
    [TestFixture]
    public class NonAdoptableFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        NonAdoptableFailureMechanismSectionResultUpdateStrategy, NonAdoptableFailureMechanismSectionResult>
    {
        protected override NonAdoptableFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override NonAdoptableFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            var random = new Random(39);
            return new NonAdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = random.NextBoolean(),
                InitialFailureMechanismResultType = random.NextEnumValue<NonAdoptableInitialFailureMechanismResultType>(),
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                FurtherAnalysisType = random.NextEnumValue<FailureMechanismSectionResultFurtherAnalysisType>(),
                RefinedSectionProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(NonAdoptableFailureMechanismSectionResult originResult, NonAdoptableFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.IsRelevant, targetResult.IsRelevant);
            Assert.AreEqual(originResult.InitialFailureMechanismResultType, targetResult.InitialFailureMechanismResultType);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultSectionProbability, targetResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(originResult.FurtherAnalysisType, targetResult.FurtherAnalysisType);
            Assert.AreEqual(originResult.RefinedSectionProbability, targetResult.RefinedSectionProbability);
        }
    }
}