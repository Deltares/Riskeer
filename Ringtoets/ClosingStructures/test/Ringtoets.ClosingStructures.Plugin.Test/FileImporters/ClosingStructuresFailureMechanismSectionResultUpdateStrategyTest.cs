﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Plugin.FileImporters;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Plugin.TestUtil.FileImporters;
using Ringtoets.Common.Primitives;

namespace Ringtoets.ClosingStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class ClosingStructuresFailureMechanismSectionResultUpdateStrategyTest : FailureMechanismSectionResultUpdateStrategyTestFixture<
        ClosingStructuresFailureMechanismSectionResultUpdateStrategy, ClosingStructuresFailureMechanismSectionResult>
    {
        protected override ClosingStructuresFailureMechanismSectionResult CreateEmptySectionResult()
        {
            return new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());
        }

        protected override ClosingStructuresFailureMechanismSectionResult CreateConfiguredSectionResult()
        {
            var random = new Random(39);
            return new ClosingStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<ClosingStructuresInput>(),
                SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther,
                DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.NotAssessed,
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                TailorMadeAssessmentProbability = random.NextDouble(),
                UseManualAssembly = true,
                ManualAssemblyProbability = random.NextDouble()
            };
        }

        protected override void AssertSectionResult(ClosingStructuresFailureMechanismSectionResult originResult, 
                                                    ClosingStructuresFailureMechanismSectionResult targetResult)
        {
            Assert.AreSame(originResult.Calculation, targetResult.Calculation);
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentProbability, targetResult.TailorMadeAssessmentProbability);
            Assert.AreEqual(originResult.UseManualAssembly, targetResult.UseManualAssembly);
            Assert.AreEqual(originResult.ManualAssemblyProbability, targetResult.ManualAssemblyProbability);
        }
    }
}