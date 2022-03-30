// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.Contribution;

namespace Riskeer.Common.Data.Test.Contribution
{
    [TestFixture]
    public class FailureMechanismContributionTest
    {
        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues))]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidMaximumAllowableFloodingProbability_ThrowsArgumentOutOfRangeException(double invalidNorm)
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
        public void Constructor_InvalidSignalFloodingProbability_ThrowsArgumentOutOfRangeException(double invalidNorm)
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
        public void Constructor_SignalFloodingProbabilityLargerThanMaximumAllowableFloodingProbability_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double signalFloodingProbability = 0.1;

            // Call
            TestDelegate test = () => new FailureMechanismContribution(0.01,
                                                                       signalFloodingProbability);

            // Assert
            const string expectedMessage = "De signaleringsparameter moet gelijk zijn aan of kleiner zijn dan de omgevingswaarde.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(signalFloodingProbability, exception.ActualValue);
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
            Assert.AreEqual(NormativeProbabilityType.MaximumAllowableFloodingProbability, result.NormativeNorm);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues))]
        [SetCulture("nl-NL")]
        public void MaximumAllowableFloodingProbability_InvalidNewProbability_ThrowsArgumentOutOfRangeException(double invalidNorm)
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
        public void SignalFloodingProbability_InvalidNewProbability_ThrowsArgumentOutOfRangeException(double invalidNorm)
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
        public void SignalFloodingProbability_SignalFloodingProbabilityBiggerThanMaximumAllowableFloodingProbability_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double norm = 1.0 / 30000;
            const double newNorm = 1.0 / 10;
            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm);

            // Call
            TestDelegate test = () => failureMechanismContribution.SignalingNorm = newNorm;

            // Assert
            const string expectedMessage = "De signaleringsparameter moet gelijk zijn aan of kleiner zijn dan de omgevingswaarde.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(newNorm, exception.ActualValue);
        }

        [Test]
        public void MaximumAllowableFloodingProbability_SignalFloodingProbabilityBiggerThanMaximumAllowableFloodingProbability_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double norm = 1.0 / 30000;
            const double newNorm = 1.0 / 1000000;
            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm);

            // Call
            TestDelegate test = () => failureMechanismContribution.LowerLimitNorm = newNorm;

            // Assert
            const string expectedMessage = "De omgevingswaarde moet gelijk zijn aan of groter zijn dan de signaleringsparameter.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(newNorm, exception.ActualValue);
        }

        [Test]
        public void MaximumAllowableFloodingProbability_ValidValue_SetsNewValue()
        {
            // Setup
            const double norm = 1.0 / 30000;
            const double newMaximumAllowableFloodingProbability = 1.0 / 20000;
            var failureMechanismContribution = new FailureMechanismContribution(norm, norm);

            // Call
            failureMechanismContribution.LowerLimitNorm = newMaximumAllowableFloodingProbability;

            // Assert
            Assert.AreEqual(newMaximumAllowableFloodingProbability, failureMechanismContribution.LowerLimitNorm);
        }

        [Test]
        public void SignalFloodingProbability_ValidValue_SetsNewValue()
        {
            // Setup
            const double norm = 1.0 / 30000;
            const double newSignalFloodingProbability = 1.0 / 40000;
            var failureMechanismContribution = new FailureMechanismContribution(norm, norm);

            // Call
            failureMechanismContribution.SignalingNorm = newSignalFloodingProbability;

            // Assert
            Assert.AreEqual(newSignalFloodingProbability, failureMechanismContribution.SignalingNorm);
        }

        [Test]
        [TestCase(NormativeProbabilityType.SignalFloodingProbability, 0.01)]
        [TestCase(NormativeProbabilityType.MaximumAllowableFloodingProbability, 0.1)]
        public void Norm_DifferentNormativeNormTypes_ReturnNorm(NormativeProbabilityType normativeProbabilityType, double expectedNorm)
        {
            // Setup
            var failureMechanismContribution = new FailureMechanismContribution(0.1, 0.01)
            {
                NormativeNorm = normativeProbabilityType
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