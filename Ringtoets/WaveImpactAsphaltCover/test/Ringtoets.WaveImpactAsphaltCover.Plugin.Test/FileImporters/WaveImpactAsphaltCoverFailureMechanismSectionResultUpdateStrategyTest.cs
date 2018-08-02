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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.FileImporters;
using Ringtoets.Common.Primitives;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Plugin.FileImporters;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test.FileImporters
{
    [TestFixture]
    public class WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategyTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var strategy = new WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategy();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultUpdateStrategy<WaveImpactAsphaltCoverFailureMechanismSectionResult>>(strategy);
        }

        [Test]
        public void UpdateSectionResult_OriginNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategy();

            // Call
            TestDelegate test = () => strategy.UpdateSectionResult(
                null, new WaveImpactAsphaltCoverFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("origin", paramName);
        }

        [Test]
        public void UpdateSectionResult_TargetNull_ThrowsArgumentNullException()
        {
            // Setup
            var strategy = new WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategy();

            // Call
            TestDelegate test = () => strategy.UpdateSectionResult(
                new WaveImpactAsphaltCoverFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection()), null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("target", paramName);
        }

        [Test]
        public void UpdateSectionResult_WithData_UpdatesTargetSectionResult()
        {
            // Setup
            var strategy = new WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategy();
            var originResult = new WaveImpactAsphaltCoverFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection())
            {
                SimpleAssessmentResult = SimpleAssessmentResultType.NotApplicable,
                DetailedAssessmentResultForFactorizedSignalingNorm = DetailedAssessmentResultType.Sufficient,
                DetailedAssessmentResultForSignalingNorm = DetailedAssessmentResultType.Sufficient,
                DetailedAssessmentResultForLowerLimitNorm = DetailedAssessmentResultType.Sufficient,
                DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = DetailedAssessmentResultType.Sufficient,
                DetailedAssessmentResultForFactorizedLowerLimitNorm = DetailedAssessmentResultType.Sufficient,
                TailorMadeAssessmentResult = TailorMadeAssessmentCategoryGroupResultType.IIIv,
                UseManualAssemblyCategoryGroup = true,
                ManualAssemblyCategoryGroup = FailureMechanismSectionAssemblyCategoryGroup.VIv
            };
            var targetResult = new WaveImpactAsphaltCoverFailureMechanismSectionResult(FailureMechanismSectionTestFactory.CreateFailureMechanismSection());

            // Call
            strategy.UpdateSectionResult(originResult, targetResult);

            // Assert
            Assert.AreEqual(originResult.SimpleAssessmentResult, targetResult.SimpleAssessmentResult);
            Assert.AreEqual(originResult.DetailedAssessmentResultForFactorizedSignalingNorm, targetResult.DetailedAssessmentResultForFactorizedSignalingNorm);
            Assert.AreEqual(originResult.DetailedAssessmentResultForSignalingNorm, targetResult.DetailedAssessmentResultForSignalingNorm);
            Assert.AreEqual(originResult.DetailedAssessmentResultForLowerLimitNorm, targetResult.DetailedAssessmentResultForLowerLimitNorm);
            Assert.AreEqual(originResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm, targetResult.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm);
            Assert.AreEqual(originResult.DetailedAssessmentResultForFactorizedLowerLimitNorm, targetResult.DetailedAssessmentResultForFactorizedLowerLimitNorm);
            Assert.AreEqual(originResult.TailorMadeAssessmentResult, targetResult.TailorMadeAssessmentResult);
            Assert.AreEqual(originResult.UseManualAssemblyCategoryGroup, targetResult.UseManualAssemblyCategoryGroup);
            Assert.AreEqual(originResult.ManualAssemblyCategoryGroup, targetResult.ManualAssemblyCategoryGroup);
            Assert.AreNotSame(originResult.Section, targetResult.Section);
        }
    }
}