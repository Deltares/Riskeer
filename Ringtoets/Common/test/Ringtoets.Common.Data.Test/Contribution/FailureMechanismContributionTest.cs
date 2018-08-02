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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Test.Contribution
{
    [TestFixture]
    public class FailureMechanismContributionTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues),
            new object[]
            {
                "Constructor_InvalidLowerLimitNorm_ThrowsArgumentOutOfRangeException"
            })]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidLowerLimitNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);

            // Call
            TestDelegate test = () => new FailureMechanismContribution(invalidNorm,
                                                                       0.000001);

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            var exception = Assert.Throws<ArgumentOutOfRangeException>(test);
            StringAssert.StartsWith(expectedMessage, exception.Message);
            Assert.AreEqual(invalidNorm, exception.ActualValue);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues),
            new object[]
            {
                "Constructor_InvalidSignalingNorm_ThrowsArgumentOutOfRangeException"
            })]
        [SetCulture("nl-NL")]
        public void Constructor_InvalidSignalingNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);

            // Call
            TestDelegate test = () => new FailureMechanismContribution(0.1,
                                                                       invalidNorm);

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
            var random = new Random(21);
            int contribution = random.Next(1, 100);
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
        public void Constructor_ValidData_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double signalingNorm = random.NextDouble();
            double lowerLimitNorm = random.NextDouble(0.1, signalingNorm);

            // Call
            var result = new FailureMechanismContribution(lowerLimitNorm, signalingNorm);

            // Assert
            Assert.AreEqual(lowerLimitNorm, result.Norm);
            Assert.AreEqual(signalingNorm, result.SignalingNorm);
            Assert.AreEqual(lowerLimitNorm, result.LowerLimitNorm);
            Assert.AreEqual(NormType.LowerLimit, result.NormativeNorm);

        }

        [Test]
        public void LowerLimitNorm_WhenUpdatedAndNormativeNormLowerLimit_NormUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            const double norm = 1.0 / 30000;
            const double newNorm = 0.1;

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var random = new Random(21);
            int otherContribution = random.Next(1, 100);

            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm);

            // Call
            failureMechanismContribution.LowerLimitNorm = newNorm;

            // Assert
            CollectionAssert.AreEqual(Enumerable.Repeat(newNorm, 2),
                                      failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        public void LowerLimitNorm_WhenUpdatedAndNormativeNormNotLowerLimit_NormNotUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var random = new Random(21);
            int otherContribution = random.Next(1, 100);
            const double norm = 1.0 / 30000;

            var failureMechanismContribution = new FailureMechanismContribution(norm, norm)
            {
                NormativeNorm = NormType.Signaling
            };

            // Call
            failureMechanismContribution.LowerLimitNorm = 0.1;

            // Assert
            CollectionAssert.AreEqual(Enumerable.Repeat(norm, 2),
                                      failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        public void SignalingNorm_WhenUpdatedAndNormativeNormSignaling_NormUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            const double norm = 1.0 / 30000;
            const double newNorm = 0.000001;
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var random = new Random(21);
            int otherContribution = random.Next(1, 100);

            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm)
            {
                NormativeNorm = NormType.Signaling
            };

            // Call
            failureMechanismContribution.SignalingNorm = newNorm;

            // Assert
            CollectionAssert.AreEqual(Enumerable.Repeat(newNorm, 2),
                                      failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        public void SignalingNorm_WhenUpdatedAndNormativeNormNotSignaling_NormNotUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var random = new Random(21);
            int otherContribution = random.Next(1, 100);
            const double norm = 1.0 / 30000;

            var failureMechanismContribution = new FailureMechanismContribution(norm,
                                                                                norm);

            // Call
            failureMechanismContribution.SignalingNorm = 0.000001;

            // Assert
            CollectionAssert.AreEqual(Enumerable.Repeat(norm, 2),
                                      failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        public void NormativeNorm_WhenUpdated_NormUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var random = new Random(21);
            int otherContribution = random.Next(1, 100);

            var failureMechanismContribution = new FailureMechanismContribution(0.1, 0.001);

            // Precondition
            CollectionAssert.AreEqual(Enumerable.Repeat(0.1, 2),
                                      failureMechanismContribution.Distribution.Select(d => d.Norm));

            // Call
            failureMechanismContribution.NormativeNorm = NormType.Signaling;

            // Assert
            CollectionAssert.AreEqual(Enumerable.Repeat(0.001, 2),
                                      failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues),
            new object[]
            {
                "Norm_WhenUpdated_NormUpdatedForEachFailureMechanismContributionItem"
            })]
        [SetCulture("nl-NL")]
        public void LowerLimitNorm_InvalidNewNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);
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
        [TestCaseSource(nameof(GetInvalidNormValues),
            new object[]
            {
                "SignalingNorm_InvalidNewNorm_ThrowsArgumentOutOfRangeException"
            })]
        [SetCulture("nl-NL")]
        public void SignalingNorm_InvalidNewNorm_ThrowsArgumentOutOfRangeException(double invalidNorm)
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);
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
        [TestCaseSource(nameof(GetValidNormEdgeValues),
            new object[]
            {
                "Norm_SettingBothNormsToEdgeNorms_ThenPropertiesSet"
            })]
        public void GivenFailureMechanismContribution_WhenSettingBothNormsToEdgeNorms_ThenPropertiesSet(double newNorm)
        {
            // Given
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var random = new Random(21);

            // When
            var failureMechanismContribution = new FailureMechanismContribution(newNorm, newNorm);

            // Then
            CollectionAssert.AreEqual(Enumerable.Repeat(newNorm, 2),
                                      failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        public void SignalingNorm_SignalingNormBiggerThanLowerLimitNorm_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);
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
            var random = new Random(21);
            int contribution = random.Next(1, 100);
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
            var random = new Random(21);
            int contribution = random.Next(1, 100);
            var failureMechanismContribution = new FailureMechanismContribution(0.1,
                                                                                0.01)
            {
                NormativeNorm = normType
            };

            // Call
            double norm = failureMechanismContribution.Norm;

            // Assert
            Assert.AreEqual(expectedNorm, norm);
        }

        private static void AssertFailureProbabilitySpace(double newOtherContribution, double norm, double probabilitySpace)
        {
            double expectedProbabilitySpace = 100.0 / (norm * newOtherContribution);
            Assert.AreEqual(expectedProbabilitySpace, probabilitySpace);
        }

        private static IEnumerable<TestCaseData> GetValidNormEdgeValues(string name)
        {
            yield return new TestCaseData(1.0 / 10)
                .SetName($"{name} Minimum valid norm");
            yield return new TestCaseData(1.0 / 1000000)
                .SetName($"{name} Maximum valid norm");
        }

        private static IEnumerable<TestCaseData> GetInvalidNormValues(string name)
        {
            yield return new TestCaseData(double.MaxValue)
                .SetName($"{name} maxValue");
            yield return new TestCaseData(double.MinValue)
                .SetName($"{name} minValue");
            yield return new TestCaseData(double.NaN)
                .SetName($"{name} NaN");
            yield return new TestCaseData(0.1 + 1e-6)
                .SetName($"{name} maximum boundary");
            yield return new TestCaseData(0.000001 - 1e-6)
                .SetName($"{name} minimum boundary");
        }
    }
}