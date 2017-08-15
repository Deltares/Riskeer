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
        public void Constructor_WithNullFailureMechanisms_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);

            // Call
            TestDelegate test = () => new FailureMechanismContribution(null, contribution);

            // Assert
            const string expectedMessage = "Kan geen bijdrageoverzicht maken zonder toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WithNullFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);

            // Call
            TestDelegate test = () => new FailureMechanismContribution(new IFailureMechanism[]
            {
                null
            }, contribution);

            // Assert
            const string expectedMessage = "Kan geen bijdrage element maken zonder een toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-10)]
        [TestCase(-1e-6)]
        [TestCase(100 + 1e-6)]
        [TestCase(150)]
        [TestCase(double.NaN)]
        public void Constructor_OtherContributionLessOrEqualTo0OrGreaterThan100_ArgumentException(double contribution)
        {
            // Call
            TestDelegate test = () => new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution);

            // Assert
            const string expectedMessage = "De waarde voor de toegestane bijdrage aan de faalkans moet in het bereik [0,0, 100,0] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(50)]
        [TestCase(0)]
        [TestCase(100)]
        public void Constructor_EmptyFailureMechanisms_OnlyOtherFailureMechanismAddedWithContributionSet(double contribution)
        {
            // Call
            var result = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution);

            // Assert
            const double norm = 1.0 / 30000;
            Assert.AreEqual(1, result.Distribution.Count());
            FailureMechanismContributionItem otherFailureMechanismItem = result.Distribution.ElementAt(0);
            AssertFailureProbabilitySpace(contribution, norm, otherFailureMechanismItem.ProbabilitySpace);
            Assert.AreEqual(Resources.OtherFailureMechanism_DisplayName, otherFailureMechanismItem.Assessment);
            Assert.AreEqual(Resources.OtherFailureMechanism_Code, otherFailureMechanismItem.AssessmentCode);
            Assert.AreEqual(contribution, otherFailureMechanismItem.Contribution);
            Assert.IsTrue(otherFailureMechanismItem.IsAlwaysRelevant);
            Assert.IsTrue(otherFailureMechanismItem.IsRelevant);
            Assert.AreEqual(norm, result.Norm);
            Assert.AreEqual(norm, result.SignalingNorm);
            Assert.AreEqual(norm, result.LowerLimitNorm);
            Assert.AreEqual(NormType.LowerLimit, result.NormativeNorm);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        public void Constructor_OneOrMoreFailureMechanisms_DistributionForFailureMechanismsWithOtherAtEnd(int failureMechanismCount)
        {
            // Setup
            var random = new Random(21);
            int otherContribution = random.Next(1, 100);

            var failureMechanismNames = new Collection<string>();
            var failureMechanismContributions = new Collection<double>();

            var failureMechanisms = new Collection<IFailureMechanism>();
            const string namePrefixFormat = "mechanism_{0}";

            for (var i = 0; i < failureMechanismCount; i++)
            {
                string name = string.Format(namePrefixFormat, i);
                int contribution = random.Next(1, 100);
                var failureMechanism = mocks.StrictMock<IFailureMechanism>();
                failureMechanism.Expect(fm => fm.Name).Return(name);
                failureMechanism.Expect(fm => fm.Contribution).Return(contribution).Repeat.Twice();

                failureMechanisms.Add(failureMechanism);
                failureMechanismNames.Add(name);
                failureMechanismContributions.Add(contribution);
            }

            failureMechanismNames.Add("Overig");
            failureMechanismContributions.Add(otherContribution);

            mocks.ReplayAll();

            // Call
            var result = new FailureMechanismContribution(failureMechanisms, otherContribution);

            // Assert
            Assert.AreEqual(failureMechanismCount + 1, result.Distribution.Count());

            CollectionAssert.AreEqual(failureMechanismNames, result.Distribution.Select(d => d.Assessment));
            CollectionAssert.AreEqual(failureMechanismContributions, result.Distribution.Select(d => d.Contribution));
            CollectionAssert.AreEqual(failureMechanismContributions.Select(c => 100.0 / (1.0 / 30000 * c)), result.Distribution.Select(d => d.ProbabilitySpace));
            IEnumerable<bool> expectedIsAlwaysRelevant = Enumerable.Repeat(false, failureMechanismCount)
                                                                   .Concat(Enumerable.Repeat(true, 1));
            CollectionAssert.AreEqual(expectedIsAlwaysRelevant, result.Distribution.Select(d => d.IsAlwaysRelevant));
            mocks.VerifyAll();
        }

        [Test]
        public void UpdateContribution_FailureMechanismsIsNull_ThrowsArgumentNullException()
        {
            // Setup
            IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, 12.34);

            // Call
            TestDelegate call = () => failureMechanismContribution.UpdateContributions(null, 0);

            // Assert
            const string message = "Kan geen bijdrageoverzicht maken zonder toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(34.6)]
        [TestCase(100)]
        public void UpdateContributions_NoFailureMechanismsAndValidOtherContribution_UpdateDistribution(double newOtherContribution)
        {
            // Setup
            IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();

            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, 12.34);

            // Call
            failureMechanismContribution.UpdateContributions(failureMechanisms, newOtherContribution);

            // Assert
            const double norm = 1.0 / 30000;
            Assert.AreEqual(1, failureMechanismContribution.Distribution.Count());
            FailureMechanismContributionItem otherFailureMechanismContribution = failureMechanismContribution.Distribution.Last();
            Assert.AreEqual(newOtherContribution, otherFailureMechanismContribution.Contribution);
            Assert.AreEqual(norm, otherFailureMechanismContribution.Norm);
            AssertFailureProbabilitySpace(newOtherContribution, norm, otherFailureMechanismContribution.ProbabilitySpace);
        }

        [Test]
        public void UpdateContributions_MultipleChanges_AllFailureMechanismContributionItemsHaveLatestContribution()
        {
            // Given
            IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, 12.34);

            const double latestContribution = 2.3;

            // When
            failureMechanismContribution.UpdateContributions(failureMechanisms, 1);
            FailureMechanismContributionItem item1 = failureMechanismContribution.Distribution.Single();
            failureMechanismContribution.UpdateContributions(failureMechanisms, latestContribution);
            FailureMechanismContributionItem item2 = failureMechanismContribution.Distribution.Single();

            // Then
            Assert.AreEqual(latestContribution, item1.Contribution);
            Assert.AreEqual(latestContribution, item2.Contribution);
            Assert.AreEqual(item1.Assessment, item2.Assessment);
        }

        [Test]
        [TestCase(0)]
        [TestCase(34.6)]
        [TestCase(100)]
        public void UpdateContributions_FailureMechanismsChangesAfterConstruction_UpdateDistribution(double newOtherContribution)
        {
            // Setup
            const string name1 = "A";
            const string name2 = "B";
            const string name3 = "C";
            const string name4 = "D";
            const double contribution1 = 1.1;
            const double contribution2 = 5.5;
            const double contribution3 = 23.45;
            const double contribution4 = 67.89;

            var failureMechanism1 = mocks.Stub<IFailureMechanism>();
            failureMechanism1.Contribution = contribution1;
            failureMechanism1.Stub(fm => fm.Name).Return(name1);
            var failureMechanism2 = mocks.Stub<IFailureMechanism>();
            failureMechanism2.Contribution = contribution2;
            failureMechanism2.Stub(fm => fm.Name).Return(name2);
            var failureMechanism3 = mocks.Stub<IFailureMechanism>();
            failureMechanism3.Contribution = contribution3;
            failureMechanism3.Stub(fm => fm.Name).Return(name3);
            var failureMechanism4 = mocks.Stub<IFailureMechanism>();
            failureMechanism4.Contribution = contribution4;
            failureMechanism4.Stub(fm => fm.Name).Return(name4);
            mocks.ReplayAll();

            var failureMechanisms = new List<IFailureMechanism>
            {
                failureMechanism1,
                failureMechanism2
            };

            const double otherContribution = 12.34;
            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, otherContribution);

            // Change failureMechanisms after construction of FailureMechanismContribution:
            failureMechanisms.RemoveAt(1);
            failureMechanisms.Add(failureMechanism3);
            failureMechanisms.Add(failureMechanism4);

            // Precondition
            Assert.AreEqual(3, failureMechanismContribution.Distribution.Count());
            var originalNames = new[]
            {
                name1,
                name2,
                "Overig"
            };
            CollectionAssert.AreEqual(originalNames, failureMechanismContribution.Distribution.Select(d => d.Assessment));
            var originalContributionValues = new[]
            {
                contribution1,
                contribution2,
                otherContribution
            };
            CollectionAssert.AreEqual(originalContributionValues, failureMechanismContribution.Distribution.Select(d => d.Contribution));

            // Call
            failureMechanismContribution.UpdateContributions(failureMechanisms, newOtherContribution);

            // Assert
            Assert.AreEqual(4, failureMechanismContribution.Distribution.Count());

            var expectedNames = new[]
            {
                name1,
                name3,
                name4,
                "Overig"
            };
            CollectionAssert.AreEqual(expectedNames, failureMechanismContribution.Distribution.Select(d => d.Assessment));
            var contributionValues = new[]
            {
                contribution1,
                contribution3,
                contribution4,
                newOtherContribution
            };
            CollectionAssert.AreEqual(contributionValues, failureMechanismContribution.Distribution.Select(d => d.Contribution));
            CollectionAssert.AreEqual(Enumerable.Repeat(1.0 / 30000, 4), failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        public void LowerLimitNorm_WhenUpdatedAndNormativeNormLowerLimit_NormUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            const double newNorm = 0.1;

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var random = new Random(21);
            int otherContribution = random.Next(1, 100);

            var failureMechanismContribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution);

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

            var failureMechanismContribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution)
            {
                NormativeNorm = NormType.Signaling
            };

            double originalNorm = failureMechanismContribution.Norm;

            // Call
            failureMechanismContribution.LowerLimitNorm = 0.1;

            // Assert
            CollectionAssert.AreEqual(Enumerable.Repeat(originalNorm, 2),
                                      failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        public void SignalingNorm_WhenUpdatedAndNormativeNormSignaling_NormUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            const double newNorm = 0.000001;
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var random = new Random(21);
            int otherContribution = random.Next(1, 100);

            var failureMechanismContribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution)
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

            var failureMechanismContribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution);

            double originalNorm = failureMechanismContribution.Norm;

            // Call
            failureMechanismContribution.SignalingNorm = 0.000001;

            // Assert
            CollectionAssert.AreEqual(Enumerable.Repeat(originalNorm, 2),
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

            var failureMechanismContribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution)
            {
                LowerLimitNorm = 0.1,
                SignalingNorm = 0.001
            };

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
        public void LowerLimitNorm_InvalidNewNorm_ThrowsArgumentOutOfRangeException(double newNorm)
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);
            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution);

            // Call
            TestDelegate test = () => failureMechanismContribution.LowerLimitNorm = newNorm;

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCaseSource(nameof(GetInvalidNormValues),
            new object[]
            {
                "SignalingNorm_InvalidNewNorm_ThrowsArgumentOutOfRangeException"
            })]
        [SetCulture("nl-NL")]
        public void SignalingNorm_InvalidNewNorm_ThrowsArgumentOutOfRangeException(double newNorm)
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);
            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution);

            // Call
            TestDelegate test = () => failureMechanismContribution.SignalingNorm = newNorm;

            // Assert
            const string expectedMessage = "De waarde van de norm moet in het bereik [0,000001, 0,1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        public void SignalingNorm_SignalingNormBiggerThanLowerLimitNorm_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);
            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution);

            // Call
            TestDelegate test = () => failureMechanismContribution.SignalingNorm = 0.1;

            // Assert
            const string expectedMessage = "De signaleringswaarde moet gelijk of kleiner zijn dan de ondergrens.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        public void LowerLimitNorm_SignalingNormBiggerThanLowerLimitNorm_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);
            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution);

            // Call
            TestDelegate test = () => failureMechanismContribution.LowerLimitNorm = 0.000001;

            // Assert
            const string expectedMessage = "De ondergrens moet gelijk of groter zijn dan de signaleringswaarde.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(NormType.Signaling, 0.01)]
        [TestCase(NormType.LowerLimit, 0.1)]
        public void Norm_DifferentNormativeNormTypes_ReturnNorm(NormType normType, double expectedNorm)
        {
            // Setup
            var random = new Random(21);
            int contribution = random.Next(1, 100);
            var failureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution)
            {
                LowerLimitNorm = 0.1,
                SignalingNorm = 0.01,
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