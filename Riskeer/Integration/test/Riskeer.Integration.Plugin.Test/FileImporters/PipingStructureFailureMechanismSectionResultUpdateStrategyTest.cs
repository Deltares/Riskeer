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
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Plugin.FileImporters;

namespace Riskeer.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class PipingStructureFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        PipingStructureFailureMechanismSectionResultUpdateStrategy, PipingStructureFailureMechanismSectionResultOld, NonAdoptableFailureMechanismSectionResult>
    {
        protected override PipingStructureFailureMechanismSectionResultOld CreateEmptySectionResultOld()
        {
            return new PipingStructureFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override PipingStructureFailureMechanismSectionResultOld CreateConfiguredSectionResultOld()
        {
            return new PipingStructureFailureMechanismSectionResultOld(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                DetailedAssessmentResult = DetailedAssessmentResultType.Sufficient,
                TailorMadeAssessmentResult = TailorMadeAssessmentResultType.ProbabilityNegligible,
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv
            };
        }

        protected override void AssertSectionResult(PipingStructureFailureMechanismSectionResultOld originResult,
                                                    PipingStructureFailureMechanismSectionResultOld targetResult)
        {
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
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
                InitialFailureMechanismResult = NonAdoptableInitialFailureMechanismResultType.NoFailureProbability,
                ManualInitialFailureMechanismResultSectionProbability = random.NextDouble(),
                FurtherAnalysisNeeded = true,
                RefinedSectionProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(NonAdoptableFailureMechanismSectionResult originResult, NonAdoptableFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.IsRelevant, targetResult.IsRelevant);
            Assert.AreEqual(originResult.InitialFailureMechanismResult, targetResult.InitialFailureMechanismResult);
            Assert.AreEqual(originResult.ManualInitialFailureMechanismResultSectionProbability, targetResult.ManualInitialFailureMechanismResultSectionProbability);
            Assert.AreEqual(originResult.FurtherAnalysisNeeded, targetResult.FurtherAnalysisNeeded);
            Assert.AreEqual(originResult.RefinedSectionProbability, targetResult.RefinedSectionProbability);
        }
    }
}