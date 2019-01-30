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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Util.Test
{
    [TestFixture]
    public class FailureMechanismNormHelperTest
    {
        [Test]
        public void GetNorm_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate test = () => FailureMechanismNormHelper.GetNorm(null,
                                                                         FailureMechanismCategoryType.FactorizedLowerLimitNorm,
                                                                         random.NextDouble(0, 100),
                                                                         random.NextDouble(1, 20));

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("assessmentSection", paramName);
        }

        [Test]
        public void GetNorm_InvalidFailureMechanismCategoryType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            const int invalidValue = 9999;

            // Call
            TestDelegate test = () => FailureMechanismNormHelper.GetNorm(new AssessmentSectionStub(),
                                                                         (FailureMechanismCategoryType) invalidValue,
                                                                         random.NextDouble(0, 100),
                                                                         random.NextDouble(1, 20));

            // Assert
            string expectedMessage = $"The value of argument 'categoryType' ({invalidValue}) is invalid for Enum type '{nameof(FailureMechanismCategoryType)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, expectedMessage).ParamName;
            Assert.AreEqual("categoryType", parameterName);
        }

        [Test]
        [TestCase(-0.1)]
        [TestCase(100.1)]
        [TestCase(double.NaN)]
        public void GetNorm_ContributionOutOfRange_ThrowsArgumentOutOfRangeException(double contribution)
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => FailureMechanismNormHelper.GetNorm(new AssessmentSectionStub(),
                                                                         random.NextEnumValue<FailureMechanismCategoryType>(),
                                                                         contribution,
                                                                         random.NextDouble(1, 20));

            // Assert
            const string message = "The value for 'failureMechanismContribution' must be in the range of [0.0, 100.0].";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual("failureMechanismContribution", paramName);
        }

        [Test]
        [TestCase(0.99)]
        [TestCase(-1)]
        [TestCase(double.NaN)]
        public void GetNorm_NOutOfRange_ThrowsArgumentOutOfRangeException(double n)
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => FailureMechanismNormHelper.GetNorm(new AssessmentSectionStub(),
                                                                         random.NextEnumValue<FailureMechanismCategoryType>(),
                                                                         random.NextDouble(0, 100),
                                                                         n);

            // Assert
            const string message = "The value for 'n' must be 1.0 or larger.";
            string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, message).ParamName;
            Assert.AreEqual("n", paramName);
        }

        [Test]
        [TestCaseSource(nameof(GetNormConfigurationPerFailureMechanismCategoryType))]
        public void GetNorm_AssessmentSectionWithNormConfiguration_ReturnsCorrespondingNorm(
            double contribution,
            double n,
            IAssessmentSection assessmentSection,
            FailureMechanismCategoryType categoryType,
            double expectedNorm)
        {
            // Call
            double norm = FailureMechanismNormHelper.GetNorm(assessmentSection, categoryType, contribution, n);

            // Assert
            Assert.AreEqual(expectedNorm, norm, 1e-5);
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

            var random = new Random(21);
            double contribution = random.NextDouble(0, 100);
            double n = random.NextDouble(1, 20);

            yield return new TestCaseData(
                contribution,
                n,
                assessmentSection,
                FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                GetMechanismSpecificNorm(contribution, n, signalingNorm / 30)
            ).SetName("MechanismSpecificFactorizedSignalingNorm");

            yield return new TestCaseData(
                contribution,
                n,
                assessmentSection,
                FailureMechanismCategoryType.MechanismSpecificSignalingNorm,
                GetMechanismSpecificNorm(contribution, n, signalingNorm)
            ).SetName("MechanismSpecificSignalingNorm");

            yield return new TestCaseData(
                contribution,
                n,
                assessmentSection,
                FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm,
                GetMechanismSpecificNorm(contribution, n, lowerLimitNorm)
            ).SetName("MechanismSpecificLowerLimitNorm");

            yield return new TestCaseData(
                contribution,
                n,
                assessmentSection,
                FailureMechanismCategoryType.LowerLimitNorm,
                lowerLimitNorm
            ).SetName("LowerLimitNorm");

            yield return new TestCaseData(
                contribution,
                n,
                assessmentSection,
                FailureMechanismCategoryType.FactorizedLowerLimitNorm,
                lowerLimitNorm * 30
            ).SetName("FactorizedLowerLimitNorm");
        }

        private static double GetMechanismSpecificNorm(double failureMechanismContribution,
                                                       double n,
                                                       double norm)
        {
            return norm * (failureMechanismContribution / 100) / n;
        }
    }
}