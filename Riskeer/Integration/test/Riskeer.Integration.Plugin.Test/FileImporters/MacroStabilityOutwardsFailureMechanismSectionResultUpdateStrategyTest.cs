// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Plugin.TestUtil.FileImporters;
using Riskeer.Common.Primitives;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Plugin.FileImporters;

namespace Riskeer.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class MacroStabilityOutwardsFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        MacroStabilityOutwardsFailureMechanismSectionResultUpdateStrategy, MacroStabilityOutwardsFailureMechanismSectionResult>
    {
        protected override MacroStabilityOutwardsFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override MacroStabilityOutwardsFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            var random = new Random(39);
            return new MacroStabilityOutwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.NotAssessed,
                DetailedAssessmentProbability = random.NextDouble(),
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.ProbabilityNegligible,
                TailorMadeAssessmentProbability = random.NextDouble(),
                UseManualAssembly = true,
                ManualAssemblyCategoryGroup = ManualFailureMechanismSectionAssemblyCategoryGroup.VIIv
            };
        }

        protected override void AssertSectionResult(MacroStabilityOutwardsFailureMechanismSectionResult originResult,
                                                    MacroStabilityOutwardsFailureMechanismSectionResult targetResult)
        {
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentProbability, targetResult.DetailedAssessmentProbability);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentProbability, targetResult.TailorMadeAssessmentProbability);
            Assert.AreEqual(originResult.UseManualAssembly, targetResult.UseManualAssembly);
            Assert.AreEqual(originResult.ManualAssemblyCategoryGroup, targetResult.ManualAssemblyCategoryGroup);
        }
    }
}