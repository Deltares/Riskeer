// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.DuneErosion.Data.Test
{
    [TestFixture]
    public class DuneErosionFailureMechanismExtensionsTest
    {
        private const double failureMechanismSpecificNormFactor = 2.15;

        [Test]
        public void GetNorm_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            var mocks = new MockRepository();
            var assessementSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => DuneErosionFailureMechanismExtensions.GetNorm(null,
                                                                                    assessementSection,
                                                                                    random.NextEnumValue<FailureMechanismCategoryType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        public void GetNorm_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate call = () => failureMechanism.GetNorm(null, random.NextEnumValue<FailureMechanismCategoryType>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void GetNorm_InvalidFailureMechanismCategoryType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            const int invalidValue = 9999;

            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new DuneErosionFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.GetNorm(assessmentSection,
                                                               (FailureMechanismCategoryType) invalidValue);

            // Assert
            string expectedMessage = $"The value of argument 'categoryType' ({invalidValue}) is invalid for Enum type '{nameof(FailureMechanismCategoryType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
            Assert.AreEqual("categoryType", parameterName);
        }

        [Test]
        [TestCaseSource(nameof(GetNormConfigurationPerFailureMechanismCategoryType))]
        public void GetNorm_WithValidData_ReturnMechanismSpecificNorm(DuneErosionFailureMechanism failureMechanism,
                                                                      IAssessmentSection assessmentSection,
                                                                      FailureMechanismCategoryType categoryType,
                                                                      double expectedNorm)
        {
            // Call
            double mechanismSpecificNorm = failureMechanism.GetNorm(assessmentSection, categoryType);

            // Assert
            Assert.AreEqual(expectedNorm, mechanismSpecificNorm);
        }

        private static IEnumerable<TestCaseData> GetNormConfigurationPerFailureMechanismCategoryType()
        {
            const double signalingNorm = 0.002;
            const double lowerLimitNorm = 0.005;

            var assessmentSection = new AssessmentSectionStub
            {
                FailureMechanismContribution =
                {
                    LowerLimitNorm = lowerLimitNorm,
                    SignalingNorm = signalingNorm
                }
            };

            var failureMechanism = new DuneErosionFailureMechanism
            {
                Contribution = 25
            };

            yield return new TestCaseData(
                failureMechanism,
                assessmentSection,
                FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                GetMechanismSpecificNorm(failureMechanism, signalingNorm / 30)
            ).SetName("MechanismSpecificFactorizedSignalingNorm");

            yield return new TestCaseData(
                failureMechanism,
                assessmentSection,
                FailureMechanismCategoryType.MechanismSpecificSignalingNorm,
                GetMechanismSpecificNorm(failureMechanism, signalingNorm)
            ).SetName("MechanismSpecificSignalingNorm");

            yield return new TestCaseData(
                failureMechanism,
                assessmentSection,
                FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm,
                GetMechanismSpecificNorm(failureMechanism, lowerLimitNorm)
            ).SetName("MechanismSpecificLowerLimitNorm");

            yield return new TestCaseData(
                failureMechanism,
                assessmentSection,
                FailureMechanismCategoryType.LowerLimitNorm,
                failureMechanismSpecificNormFactor * lowerLimitNorm
            ).SetName("LowerLimitNorm");

            yield return new TestCaseData(
                failureMechanism,
                assessmentSection,
                FailureMechanismCategoryType.FactorizedLowerLimitNorm,
                failureMechanismSpecificNormFactor * lowerLimitNorm * 30
            ).SetName("FactorizedLowerLimitNorm");
        }

        private static double GetMechanismSpecificNorm(DuneErosionFailureMechanism failureMechanism,
                                                       double norm)
        {
            return failureMechanismSpecificNormFactor * norm * (failureMechanism.Contribution / 100) / failureMechanism.GeneralInput.N;
        }
    }
}