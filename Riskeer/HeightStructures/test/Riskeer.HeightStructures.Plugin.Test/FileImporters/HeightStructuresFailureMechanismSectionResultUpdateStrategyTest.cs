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
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Plugin.FileImporters;

namespace Riskeer.HeightStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class HeightStructuresFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        HeightStructuresFailureMechanismSectionResultUpdateStrategy, HeightStructuresFailureMechanismSectionResultOld, AdoptableFailureMechanismSectionResult>
    {
        protected override HeightStructuresFailureMechanismSectionResultOld CreateEmptySectionResultOld()
        {
            return new HeightStructuresFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override HeightStructuresFailureMechanismSectionResultOld CreateConfiguredSectionResultOld()
        {
            var random = new Random(39);
            return new HeightStructuresFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther,
                DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.NotAssessed,
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                TailorMadeAssessmentProbability = random.NextDouble(),
                UseManualAssembly = true,
                ManualAssemblyProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(HeightStructuresFailureMechanismSectionResultOld originResult,
                                                    HeightStructuresFailureMechanismSectionResultOld targetResult)
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
                IsRelevant = true,
                InitialFailureMechanismResult = InitialFailureMechanismResultType.Manual,
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                FurtherAnalysisNeeded = true,
                RefinedSectionProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(AdoptableFailureMechanismSectionResult originResult, AdoptableFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.IsRelevant, targetResult.IsRelevant);
            Assert.AreEqual(originResult.InitialFailureMechanismResult, targetResult.InitialFailureMechanismResult);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultSectionProbability, targetResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(originResult.FurtherAnalysisNeeded, targetResult.FurtherAnalysisNeeded);
            Assert.AreEqual(originResult.RefinedSectionProbability, targetResult.RefinedSectionProbability);
        }
    }
}