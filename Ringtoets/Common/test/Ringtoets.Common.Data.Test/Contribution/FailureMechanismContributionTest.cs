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
using System.Collections.Generic;
using NUnit.Framework;
using Ringtoets.Common.Data.Contribution;

namespace Ringtoets.Common.Data.Test.Contribution
{
    [TestFixture]
    public class FailureMechanismContributionTest
    {
        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues))]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidLowerLimitNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Call
            TestDelegate test = () => new FailureMechanismContribution(invalidNorm, 0.000001);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidNorm, exception.ActualValue);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues))]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidSignalingNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Call
            TestDelegate test = () => new FailureMechanismContribution(0.1, invalidNorm);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidNorm, exception.ActualValue);
        }

        [Test]
        public void Constructor_SignalingNormLargerThanLowerLimitNorm_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double signalingNorm = 0.1;

            // Call
            TestDelegate test = () => new FailureMechanismContribution(0.01,
                                                                       signalingNorm);

            // Assert
            const string expectedMessage = "De signaleringswaarde moet gelijk zijn aan of kleiner zijn dan de ondergrens.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(signalingNorm, exception.ActualValue);
        }

        [Test]
        [TestCaseSource(nameof(GetValidNormEdgeValues))]
        public void Constructor_ValidData_ExpectedValues(double norm)
        {
            // Call
            var result = new FailureMechanismContribution(norm, norm);

            // Assert
            Assert.AreEqual(norm, result.Norm);
            Assert.AreEqual(norm, result.SignalingNorm);
            Assert.AreEqual(norm, result.LowerLimitNorm);
            Assert.AreEqual(NormType.LowerLimit, result.NormativeNorm);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues))]
        [SetCulture("nl-NL")]
        public void LowerLimitNorm_InvalidNewNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Setup
            const double norm = 1.0 / 30000;
            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm);

            // Call
            TestDelegate test = () => failureMechanismContribution.LowerLimitNorm = invalidNorm;

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidNorm, exception.ActualValue);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues))]
        [SetCulture("nl-NL")]
        public void SignalingNorm_InvalidNewNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Setup
            const double norm = 1.0 / 30000;
            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm);

            // Call
            TestDelegate test = () => failureMechanismContribution.SignalingNorm = invalidNorm;

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidNorm, exception.ActualValue);
        }

        [Test]
        public void SignalingNorm_SignalingNormBiggerThanLowerLimitNorm_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double norm = 1.0 / 30000;
            const double newNorm = 1.0 / 10;
            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm);

            // Call
            TestDelegate test = () => failureMechanismContribution.SignalingNorm = newNorm;

            // Assert
            const string expectedMessage = "De signaleringswaarde moet gelijk zijn aan of kleiner zijn dan de ondergrens.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(newNorm, exception.ActualValue);
        }

        [Test]
        public void LowerLimitNorm_SignalingNormBiggerThanLowerLimitNorm_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double norm = 1.0 / 30000;
            const double newNorm = 1.0 / 1000000;
            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm);

            // Call
            TestDelegate test = () => failureMechanismContribution.LowerLimitNorm = newNorm;

            // Assert
            const string expectedMessage = "De ondergrens moet gelijk zijn aan of groter zijn dan de signaleringswaarde.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(newNorm, exception.ActualValue);
        }

        [Test]
        [TestCase(NormType.Signaling, 0.01)]
        [TestCase(NormType.LowerLimit, 0.1)]
        public void Norm_DifferentNormativeNormTypes_ReturnNorm(NormType normType, double expectedNorm)
        {
            // Setup
            var failureMechanismContribution = new FailureMechanismContribution(0.1, 0.01)
            {
                NormativeNorm = normType
            };

            // Call
            double norm = failureMechanismContribution.Norm;

            // Assert
            Assert.AreEqual(expectedNorm, norm);
        }

        private static IEnumerable<TestCaseData> GetValidNormEdgeValues()
        {
            yield return new TestCaseData(1.0 / 10);
            yield return new TestCaseData(1.0 / 1000000);
        }

        private static IEnumerable<TestCaseData> GetInvalidNormValues()
        {
            yield return new TestCaseData(double.MaxValue);
            yield return new TestCaseData(double.MinValue);
            yield return new TestCaseData(double.NaN);
            yield return new TestCaseData(0.1 + 1e-6);
            yield return new TestCaseData(0.000001 - 1e-6);
        }
    }
}