using System;
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
            Assert.AreEqual(contribution, result.Distribution.ElementAt(0).Contribution);
            Assert.AreEqual((norm/contribution)*100, result.Distribution.ElementAt(0).ProbabilitySpace);
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