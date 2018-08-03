// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Primitives;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Plugin.FileImporters;

namespace Ringtoets.StabilityPointStructures.Plugin.Test.FileImporters
{
    [TestFixture]
    public class StabilityPointStructuresFailureMechanismSectionResultUpdateStrategyTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var strategy = new StabilityPointStructuresFailureMechanismSectionResultUpdateStrategy();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultUpdateStrategy<StabilityPointStructuresFailureMechanismSectionResult>>(strategy);
        }

        [Test]
        public void UpdateSectionResult_OriginNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StabilityPointStructuresFailureMechanismSectionResultUpdateStrategy();

            // Call
            TestDelegate test = () => strategy.UpdateSectionResult(
                null, new StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("origin", paramName);
        }

        [Test]
        public void UpdateSectionResult_TargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new StabilityPointStructuresFailureMechanismSectionResultUpdateStrategy();

            // Call
            TestDelegate test = () => strategy.UpdateSectionResult(
                new StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("target", paramName);
        }

        [Test]
        public void UpdateSectionResult_WithData_UpdatesTargetSectionResult()
        {
            // Setup
            var random = new Random(39);
            var strategy = new StabilityPointStructuresFailureMechanismSectionResultUpdateStrategy();
            var originResult = new StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                Calculation = new StructuresCalculation<StabilityPointStructuresInput>(),
                SimpleAssessmentResult = SimpleAssessmentValidityOnlyResultType.NotApplicable,
                DetailedAssessmentResult = DetailedAssessmentProbabilityOnlyResultType.NotAssessed,
                TailorMadeAssessmentResult = TailorMadeAssessmentProbabilityCalculationResultType.Probability,
                TailorMadeAssessmentProbability = random.NextDouble(),
                UseManualAssemblyProbability = true,
                ManualAssemblyProbability = random.NextDouble()
            };
            var targetResult = new StabilityPointStructuresFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            strategy.UpdateSectionResult(originResult, targetResult);

            // Assert
            Assert.AreSame(originResult.Calculation, targetResult.Calculation);
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentProbability, targetResult.TailorMadeAssessmentProbability);
            Assert.AreEqual(originResult.UseManualAssemblyProbability, targetResult.UseManualAssemblyProbability);
            Assert.AreEqual(originResult.ManualAssemblyProbability, targetResult.ManualAssemblyProbability);
            Assert.AreNotSame(originResult.Section, targetResult.Section);
        }
    }
}