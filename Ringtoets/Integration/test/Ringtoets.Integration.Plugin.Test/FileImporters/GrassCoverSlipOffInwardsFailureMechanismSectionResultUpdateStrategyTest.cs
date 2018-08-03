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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Plugin.FileImporters;

namespace Ringtoets.Integration.Plugin.Test.FileImporters
{
    [TestFixture]
    public class GrassCoverSlipOffInwardsFailureMechanismSectionResultUpdateStrategyTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var strategy = new GrassCoverSlipOffInwardsFailureMechanismSectionResultUpdateStrategy();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultUpdateStrategy<GrassCoverSlipOffInwardsFailureMechanismSectionResult>>(strategy);
        }

        [Test]
        public void UpdateSectionResult_OriginNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new GrassCoverSlipOffInwardsFailureMechanismSectionResultUpdateStrategy();

            // Call
            TestDelegate test = () => strategy.UpdateSectionResult(
                null, new GrassCoverSlipOffInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("origin", paramName);
        }

        [Test]
        public void UpdateSectionResult_TargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new GrassCoverSlipOffInwardsFailureMechanismSectionResultUpdateStrategy();

            // Call
            TestDelegate test = () => strategy.UpdateSectionResult(
                new GrassCoverSlipOffInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("target", paramName);
        }

        [Test]
        public void UpdateSectionResult_WithData_UpdatesTargetSectionResult()
        {
            // Setup
            var random = new Random(39);
            var strategy = new GrassCoverSlipOffInwardsFailureMechanismSectionResultUpdateStrategy();
            var originResult = new GrassCoverSlipOffInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.AssessFurther,
                DetailedAssessmentResult = DetailedAssessmentResultType.Sufficient,
                TailorMadeAssessmentResult = TailorMadeAssessmentResultType.ProbabilityNegligible,
                UseManualAssemblyCategoryGroup = true,
                ManualAssemblyCategoryGroup = random.NextEnumValue<FailureMechanismSectionAssemblyCategoryGroup>()
            };
            var targetResult = new GrassCoverSlipOffInwardsFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            strategy.UpdateSectionResult(originResult, targetResult);

            // Assert
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResult, targetResult.DetailedAssessmentResult);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.UseManualAssemblyCategoryGroup, targetResult.UseManualAssemblyCategoryGroup);
            Assert.AreEqual(originResult.ManualAssemblyCategoryGroup, targetResult.ManualAssemblyCategoryGroup);
            Assert.AreNotSame(originResult.Section, targetResult.Section);
        }
    }
}