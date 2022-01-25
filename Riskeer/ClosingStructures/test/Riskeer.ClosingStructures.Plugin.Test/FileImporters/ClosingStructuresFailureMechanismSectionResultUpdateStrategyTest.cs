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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.ClosingStructures.Data;
using Riskeer.ClosingStructures.Plugin.FileImporters;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil.FileImporters;
using Riskeer.Common.Primitives;

namespace Riskeer.ClosingStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        ClosingStructuresFailureMechanismSectionResultUpdateStrategy, ClosingStructuresFailureMechanismSectionResultOld, AdoptableFailureMechanismSectionResult>
    {
        protected override ClosingStructuresFailureMechanismSectionResultOld CreateEmptySectionResultOld()
        {
            return new ClosingStructuresFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override ClosingStructuresFailureMechanismSectionResultOld CreateConfiguredSectionResultOld()
        {
            var random = new Random(39);
            return new ClosingStructuresFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther,
                DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.NotAssessed,
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                TailorMadeAssessmentProbability = random.NextDouble(),
                UseManualAssembly = true,
                ManualAssemblyProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(ClosingStructuresFailureMechanismSectionResultOld originResult,
                                                    ClosingStructuresFailureMechanismSectionResultOld targetResult)
        {
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentProbability, targetResult.TailorMadeAssessmentProbability);
            Assert.AreEqual(originResult.UseManualAssembly, targetResult.UseManualAssembly);
            Assert.AreEqual(originResult.ManualAssemblyProbability, targetResult.ManualAssemblyProbability);
        }

        protected override AdoptableFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override AdoptableFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            var random = new Random(39);
            return new AdoptableFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                IsRelevant = random.NextBoolean(),
                InitialFailureMechanismResultType = random.NextEnumValue<AdoptableInitialFailureMechanismResultType>(),
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                FurtherAnalysisNeeded = random.NextBoolean(),
                RefinedSectionProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(AdoptableFailureMechanismSectionResult originResult, AdoptableFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.IsRelevant, targetResult.IsRelevant);
            Assert.AreEqual(originResult.InitialFailureMechanismResultType, targetResult.InitialFailureMechanismResultType);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultSectionProbability, targetResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(originResult.FurtherAnalysisNeeded, targetResult.FurtherAnalysisNeeded);
            Assert.AreEqual(originResult.RefinedSectionProbability, targetResult.RefinedSectionProbability);
        }
    }
}