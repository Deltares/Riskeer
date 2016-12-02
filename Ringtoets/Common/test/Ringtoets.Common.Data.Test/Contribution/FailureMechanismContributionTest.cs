﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_WithNullFailureMechanisms_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var contribution = random.Next(1, 100);
            var norm = random.NextDouble();

            // Call
            TestDelegate test = () => new FailureMechanismContribution(null, contribution, norm);

            // Assert
            const string expectedMessage = "Kan geen bijdrageoverzicht maken zonder toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WithNullFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var contribution = random.Next(1, 100);
            var norm = random.NextDouble();

            // Call
            TestDelegate test = () => new FailureMechanismContribution(new IFailureMechanism[]
            {
                null
            }, contribution, norm);

            // Assert
            const string expectedMessage = "Kan geen bijdrage element maken zonder een toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WithInvalidNorm_ThrowsArgumentOutOfRangeException(
            [Values(-1e-6, -150)] double norm)
        {
            // Setup
            var random = new Random(21);
            var contribution = random.Next(1, 100);

            // Call
            TestDelegate test = () => new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution, norm);

            // Assert
            const string expectedMessage = "De faalkansbijdrage kan alleen bepaald worden als de norm van het traject groter is dan 0.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(-10)]
        [TestCase(-1e-6)]
        [TestCase(100 + 1e-6)]
        [TestCase(150)]
        public void Constructor_OtherContributionLessOrEqualTo0OrGreaterThan100_ArgumentException(double contribution)
        {
            // Setup
            var random = new Random(21);
            var norm = random.NextDouble();

            // Call
            TestDelegate test = () => new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution, norm);

            // Assert
            const string expectedMessage = "De waarde voor de toegestane bijdrage aan de faalkans moet in het bereik [0, 100] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        [TestCase(50)]
        [TestCase(0)]
        [TestCase(100)]
        public void Constructor_EmptyFailureMechanisms_OnlyOtherFailureMechanismAddedWithContributionSet(double contribution)
        {
            // Setup
            var random = new Random(21);
            var norm = random.NextDouble();

            // Call
            var result = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution, norm);

            // Assert
            Assert.AreEqual(1, result.Distribution.Count());
            FailureMechanismContributionItem otherFailureMechanismItem = result.Distribution.ElementAt(0);
            AssertFailureProbabilitySpace(contribution, norm, otherFailureMechanismItem.ProbabilitySpace);
            Assert.AreEqual(Resources.OtherFailureMechanism_DisplayName, otherFailureMechanismItem.Assessment);
            Assert.AreEqual(Resources.OtherFailureMechanism_Code, otherFailureMechanismItem.AssessmentCode);
            Assert.AreEqual(contribution, otherFailureMechanismItem.Contribution);
            Assert.IsTrue(otherFailureMechanismItem.IsAlwaysRelevant);
            Assert.IsTrue(otherFailureMechanismItem.IsRelevant);
            Assert.AreEqual(norm, result.Norm);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        public void Constructor_OneOrMoreFailureMechanisms_DistributionForFailureMechanismsWithOtherAtEnd(int failureMechanismCount)
        {
            // Setup
            var random = new Random(21);
            var otherContribution = random.Next(1, 100);
            var norm = random.NextDouble();

            var failureMechanismNames = new Collection<string>();
            var failureMechanismContributions = new Collection<double>();

            var failureMechanisms = new Collection<IFailureMechanism>();
            var namePrefixFormat = "mechanism_{0}";

            for (var i = 0; i < failureMechanismCount; i++)
            {
                var name = string.Format(namePrefixFormat, i);
                var contribution = random.Next(1, 100);
                var failureMechanism = mockRepository.StrictMock<IFailureMechanism>();
                failureMechanism.Expect(fm => fm.Name).Return(name);
                failureMechanism.Expect(fm => fm.Contribution).Return(contribution).Repeat.Twice();

                failureMechanisms.Add(failureMechanism);
                failureMechanismNames.Add(name);
                failureMechanismContributions.Add(contribution);
            }

            failureMechanismNames.Add("Overig");
            failureMechanismContributions.Add(otherContribution);

            mockRepository.ReplayAll();

            // Call
            var result = new FailureMechanismContribution(failureMechanisms, otherContribution, norm);

            // Assert
            Assert.AreEqual(failureMechanismCount + 1, result.Distribution.Count());

            CollectionAssert.AreEqual(failureMechanismNames, result.Distribution.Select(d => d.Assessment));
            CollectionAssert.AreEqual(failureMechanismContributions, result.Distribution.Select(d => d.Contribution));
            CollectionAssert.AreEqual(failureMechanismContributions.Select(c => (1.0/norm/c)*100), result.Distribution.Select(d => d.ProbabilitySpace));
            var expectedIsAlwaysRelevant = Enumerable.Repeat(false, failureMechanismCount)
                                                     .Concat(Enumerable.Repeat(true, 1));
            CollectionAssert.AreEqual(expectedIsAlwaysRelevant, result.Distribution.Select(d => d.IsAlwaysRelevant));
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(34.6)]
        [TestCase(100)]
        public void UpdateContributions_NoFailureMechanismsAndValidOtherContribution_UpdateDistribution(double newOtherContribution)
        {
            // Setup
            IEnumerable<IFailureMechanism> failureMechanisms = Enumerable.Empty<IFailureMechanism>();

            const double norm = 1.0/30000;
            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, 12.34, norm);

            // Call
            failureMechanismContribution.UpdateContributions(failureMechanisms, newOtherContribution);

            // Assert
            Assert.AreEqual(1, failureMechanismContribution.Distribution.Count());
            FailureMechanismContributionItem otherFailureMechanismContribution = failureMechanismContribution.Distribution.Last();
            Assert.AreEqual(newOtherContribution, otherFailureMechanismContribution.Contribution);
            Assert.AreEqual(norm, otherFailureMechanismContribution.Norm);
            AssertFailureProbabilitySpace(newOtherContribution, norm, otherFailureMechanismContribution.ProbabilitySpace);
        }

        [Test]
        [TestCase(0)]
        [TestCase(34.6)]
        [TestCase(100)]
        public void UpdateContributions_FailureMechanismsChangesAfterConstruction_UpdateDistribution(double newOtherContribution)
        {
            // Setup
            string name1 = "A", name2 = "B", name3 = "C", name4 = "D";
            double contribution1 = 1.1, contribution2 = 5.5, contribution3 = 23.45, contribution4 = 67.89;

            var mocks = new MockRepository();
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

            List<IFailureMechanism> failureMechanisms = new List<IFailureMechanism>
            {
                failureMechanism1,
                failureMechanism2
            };

            const double norm = 1.0/30000;
            const double otherContribution = 12.34;
            var failureMechanismContribution = new FailureMechanismContribution(failureMechanisms, otherContribution, norm);

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
            CollectionAssert.AreEqual(Enumerable.Repeat(norm, 4), failureMechanismContribution.Distribution.Select(d => d.Norm));
            mocks.VerifyAll();
        }

        [Test]
        public void Norm_WhenUpdated_NormUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            var random = new Random(21);
            var otherContribution = random.Next(1, 100);
            const double norm = 1.0/20000;
            const double newNorm = 1.0/30000;

            var failureMechanism = mockRepository.Stub<IFailureMechanism>();

            mockRepository.ReplayAll();

            var failureMechanismContribution = new FailureMechanismContribution(new[]
            {
                failureMechanism
            }, otherContribution, norm);

            // Call
            failureMechanismContribution.Norm = newNorm;

            // Assert
            Assert.AreEqual(Enumerable.Repeat(newNorm, 2), failureMechanismContribution.Distribution.Select(d => d.Norm));
            mockRepository.VerifyAll();
        }

        private static void AssertFailureProbabilitySpace(double newOtherContribution, double norm, double probabilitySpace)
        {
            double expectedProbabilitySpace = (1.0/norm/newOtherContribution)*100.0;
            Assert.AreEqual(expectedProbabilitySpace, probabilitySpace);
        }
    }
}