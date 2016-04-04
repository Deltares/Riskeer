using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.Contribution;

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
            var norm = random.Next(1, int.MaxValue);
            
            // Call
            TestDelegate test = () =>
            {
                new FailureMechanismContribution(null, contribution, norm);
            };

            // Assert
            const string expectedMessage = "Kan geen bijdrageoverzicht maken zonder faalmechanismen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WithNullFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            var contribution = random.Next(1, 100);
            var norm = random.Next(1, int.MaxValue);

            // Call
            TestDelegate test = () => new FailureMechanismContribution(new IFailureMechanism[]
            {
                null
            }, contribution, norm);

            // Assert
            const string expectedMessage = "Kan geen bijdrage element maken zonder een faalmechanisme.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-200)]
        public void Constructor_WithInvalidNorm_ThrowsArgumentException(int norm)
        {
            // Setup
            var random = new Random(21);
            var contribution = random.Next(1,100);

            // Call
            TestDelegate test = () =>
            {
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution, norm);
            };

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
            var norm = random.Next(1, int.MaxValue);

            // Call
            TestDelegate test = () =>
            {
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution, norm);
            };

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, Properties.Resources.Contribution_Value_should_be_in_interval_0_100);
        }

        [Test]
        [TestCase(50)]
        [TestCase(0)]
        [TestCase(100)]
        public void Constructor_EmptyFailureMechanisms_OnlyOtherFailureMechanismAddedWithContributionSet(double contribution)
        {
            // Setup
            var random = new Random(21);
            var norm = random.Next(1, int.MaxValue);

            // Call
            var result = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), contribution, norm);

            // Assert
            Assert.AreEqual(1, result.Distribution.Count());
            FailureMechanismContributionItem otherFailureMechanismItem = result.Distribution.ElementAt(0);
            Assert.AreEqual(contribution, otherFailureMechanismItem.Contribution);
            AssertFailureProbabilitySpace(contribution, norm, otherFailureMechanismItem.ProbabilitySpace);
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
            var norm = random.Next(1, int.MaxValue);

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
                failureMechanism.Expect(fm => fm.Contribution).Return(contribution);

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
            CollectionAssert.AreEqual(failureMechanismContributions.Select(c => (norm/c)*100), result.Distribution.Select(d => d.ProbabilitySpace));

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(34.6)]
        [TestCase(100)]
        public void UpdateContributions_NoFailureMechanismsAndValidOtherContribution_UpdateDistribution(double newOtherContribution)
        {
            // Setup
            IEnumerable<IFailureMechanism> failuireMechanisms = Enumerable.Empty<IFailureMechanism>();

            const int norm = 30000;
            var failureMechanismContribution = new FailureMechanismContribution(failuireMechanisms, 12.34, norm);

            // Call
            failureMechanismContribution.UpdateContributions(failuireMechanisms, newOtherContribution);

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

            List<IFailureMechanism> failuireMechanisms = new List<IFailureMechanism>
            {
                failureMechanism1,
                failureMechanism2
            };

            const int norm = 30000;
            const double otherContribution = 12.34;
            var failureMechanismContribution = new FailureMechanismContribution(failuireMechanisms, otherContribution, norm);

            // Change failureMechanisms after construction of FailureMechanismContribution:
            failuireMechanisms.RemoveAt(1);
            failuireMechanisms.Add(failureMechanism3);
            failuireMechanisms.Add(failureMechanism4);

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
            failureMechanismContribution.UpdateContributions(failuireMechanisms, newOtherContribution);

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

        private void AssertFailureProbabilitySpace(double newOtherContribution, int norm, double probabilitySpace)
        {
            double expectedProbabilitySpace = (norm / newOtherContribution) * 100.0;
            Assert.AreEqual(expectedProbabilitySpace, probabilitySpace);
        }

        [Test]
        public void Norm_WhenUpdated_NormUpdatedForEachFailureMechanismContributionItem()
        {
            // Setup
            var random = new Random(21);
            var otherContribution = random.Next(1, 100);
            var norm = 20000;
            var newNorm = 30000;

            var failureMechanism = mockRepository.Stub<IFailureMechanism>();

            mockRepository.ReplayAll();

            var failureMechanismContribution = new FailureMechanismContribution(new[] { failureMechanism }, otherContribution, norm);

            // Call
            failureMechanismContribution.Norm = newNorm;

            // Assert
            Assert.AreEqual(Enumerable.Repeat(newNorm,2) , failureMechanismContribution.Distribution.Select(d => d.Norm));
        }
    }
}