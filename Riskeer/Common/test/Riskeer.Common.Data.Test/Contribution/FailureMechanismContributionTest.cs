// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
        [TestCaseSource(nameof(GetInvalidProbabilityValues))]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidMaximumAllowableFloodingProbability_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Call
            void Call() => new FailureMechanismContribution(invalidNorm, 0.000001);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidNorm, exception.ActualValue);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidProbabilityValues))]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidSignalFloodingProbability_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Call
            void Call() => new FailureMechanismContribution(0.1, invalidNorm);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidNorm, exception.ActualValue);
        }

        [Test]
        public void Constructor_SignalFloodingProbabilityLargerThanMaximumAllowableFloodingProbability_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double signalFloodingProbability = 0.1;

            // Call
            void Call() => new FailureMechanismContribution(0.01, signalFloodingProbability);

            // Assert
            const string expectedMessage = "De signaleringsparameter moet gelijk zijn aan of kleiner zijn dan de omgevingswaarde.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(signalFloodingProbability, exception.ActualValue);
        }

        [Test]
        [TestCaseSource(nameof(GetValidProbabilityEdgeValues))]
        public void Constructor_ValidData_ExpectedValues(double probability)
        {
            // Call
            var result = new FailureMechanismContribution(probability, probability);

            // Assert
            Assert.AreEqual(probability, result.NormativeProbability);
            Assert.AreEqual(probability, result.SignalFloodingProbability);
            Assert.AreEqual(probability, result.MaximumAllowableFloodingProbability);
            Assert.AreEqual(NormativeProbabilityType.MaximumAllowableFloodingProbability, result.NormativeProbabilityType);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidProbabilityValues))]
        [SetCulture("nl-NL")]
        public void MaximumAllowableFloodingProbability_InvalidNewProbability_ThrowsArgumentOutOfRangeException(double invalidProbability)
        {
            // Setup
            const double probability = 1.0 / 30000;
            var failureMechanismContribution = new FailureMechanismContribution(probability, probability);

            // Call
            void Call() => failureMechanismContribution.MaximumAllowableFloodingProbability = invalidProbability;

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidProbability, exception.ActualValue);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidProbabilityValues))]
        [SetCulture("nl-NL")]
        public void SignalFloodingProbability_InvalidNewProbability_ThrowsArgumentOutOfRangeException(double invalidProbability)
        {
            // Setup
            const double probability = 1.0 / 30000;
            var failureMechanismContribution = new FailureMechanismContribution(probability, probability);

            // Call
            void Call() => failureMechanismContribution.SignalFloodingProbability = invalidProbability;

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidProbability, exception.ActualValue);
        }

        [Test]
        public void SignalFloodingProbability_SignalFloodingProbabilityBiggerThanMaximumAllowableFloodingProbability_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double probability = 1.0 / 30000;
            const double newSignalFloodingProbability = 1.0 / 10;
            var failureMechanismContribution = new FailureMechanismContribution(probability, probability);

            // Call
            void Call() => failureMechanismContribution.SignalFloodingProbability = newSignalFloodingProbability;

            // Assert
            const string expectedMessage = "De signaleringsparameter moet gelijk zijn aan of kleiner zijn dan de omgevingswaarde.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(newSignalFloodingProbability, exception.ActualValue);
        }

        [Test]
        public void MaximumAllowableFloodingProbability_SignalFloodingProbabilityBiggerThanMaximumAllowableFloodingProbability_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            const double probability = 1.0 / 30000;
            const double newMaximumAllowableFloodingProbability = 1.0 / 1000000;
            var failureMechanismContribution = new FailureMechanismContribution(probability, probability);

            // Call
            void Call() => failureMechanismContribution.MaximumAllowableFloodingProbability = newMaximumAllowableFloodingProbability;

            // Assert
            const string expectedMessage = "De omgevingswaarde moet gelijk zijn aan of groter zijn dan de signaleringsparameter.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(Call);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(newMaximumAllowableFloodingProbability, exception.ActualValue);
        }

        [Test]
        public void MaximumAllowableFloodingProbability_ValidValue_SetsNewValue()
        {
            // Setup
            const double probability = 1.0 / 30000;
            const double newMaximumAllowableFloodingProbability = 1.0 / 20000;
            var failureMechanismContribution = new FailureMechanismContribution(probability, probability);

            // Call
            failureMechanismContribution.MaximumAllowableFloodingProbability = newMaximumAllowableFloodingProbability;

            // Assert
            Assert.AreEqual(newMaximumAllowableFloodingProbability, failureMechanismContribution.MaximumAllowableFloodingProbability);
        }

        [Test]
        public void SignalFloodingProbability_ValidValue_SetsNewValue()
        {
            // Setup
            const double probability = 1.0 / 30000;
            const double newSignalFloodingProbability = 1.0 / 40000;
            var failureMechanismContribution = new FailureMechanismContribution(probability, probability);

            // Call
            failureMechanismContribution.SignalFloodingProbability = newSignalFloodingProbability;

            // Assert
            Assert.AreEqual(newSignalFloodingProbability, failureMechanismContribution.SignalFloodingProbability);
        }

        [Test]
        [TestCase(NormativeProbabilityType.SignalFloodingProbability, 0.01)]
        [TestCase(NormativeProbabilityType.MaximumAllowableFloodingProbability, 0.1)]
        public void NormativeProbability_DifferentNormativeProbabilityTypes_ReturnNorm(NormativeProbabilityType normativeProbabilityType, double expectedProbability)
        {
            // Setup
            var failureMechanismContribution = new FailureMechanismContribution(0.1, 0.01)
            {
                NormativeProbabilityType = normativeProbabilityType
            };

            // Call
            double normativeProbability = failureMechanismContribution.NormativeProbability;

            // Assert
            Assert.AreEqual(expectedProbability, normativeProbability);
        }

        private static IEnumerable<TestCaseData> GetValidProbabilityEdgeValues()
        {
            yield return new TestCaseData(1.0 / 10);
            yield return new TestCaseData(1.0 / 1000000);
        }

        private static IEnumerable<TestCaseData> GetInvalidProbabilityValues()
        {
            yield return new TestCaseData(double.MaxValue);
            yield return new TestCaseData(double.MinValue);
            yield return new TestCaseData(double.NaN);
            yield return new TestCaseData(0.1 + 1e-6);
            yield return new TestCaseData(0.000001 - 1e-6);
        }
    }
}