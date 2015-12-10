using System;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data.Test.Contribution
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

            // Call
            TestDelegate test = () => new FailureMechanismContribution(null, random.NextDouble(), random.Next());

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.FailureMechanismContribution_FailureMechanismContribution_Can_not_create_FailureMechanismContribution_without_FailureMechanism_collection, message);
            StringAssert.EndsWith("failureMechanisms", message);
        }

        [Test]
        public void Constructor_WithNullFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate test = () => new FailureMechanismContribution(new IFailureMechanism[]{null}, random.NextDouble(), random.Next());

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.FailureMechanismContributionItem_Can_not_create_contribution_item_without_failure_mechanism, message);
            StringAssert.EndsWith("failureMechanism", message);
        }

        [Test]
        [TestCase(-10)]
        [TestCase(-1e-6)]
        [TestCase(100+1e-6)]
        [TestCase(150)]
        public void Constructor_OtherContributionOutside0To100_ArgumentException(double contribution)
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate test = () => new FailureMechanismContribution(new IFailureMechanism[] { }, contribution, random.Next());

            // Assert
            var message = Assert.Throws<ArgumentException>(test).Message;
            StringAssert.StartsWith(Common.Data.Properties.Resources.FailureMechanism_Contribution_Value_should_be_in_interval_0_100, message);
            StringAssert.EndsWith("value", message);
        }

        [Test]
        public void Constructor_EmptyFailureMechanisms_OnlyOtherFailureMechanismAdded()
        {
            // Setup
            var random = new Random(21);
            var otherContribution = (double)random.Next(0, 100);
            var norm = random.Next();

            // Call
            var result =  new FailureMechanismContribution(new IFailureMechanism[] { }, otherContribution, norm);
            
            // Assert
            Assert.AreEqual(1, result.Distribution.Count());
            Assert.AreEqual(otherContribution, result.Distribution.ElementAt(0).Contribution);
            Assert.AreEqual((norm / otherContribution) * 100, result.Distribution.ElementAt(0).ProbabilitySpace);
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
            var otherContribution = (double)random.Next(0, 100);

            var failureMechanismNames = new Collection<string>();
            var failureMechanismContributions = new Collection<double>();

            var failureMechanisms = new Collection<IFailureMechanism>();
            var namePrefixFormat = "mechanism_{0}";

            for (var i = 0; i < failureMechanismCount; i++)
            {
                var name = string.Format(namePrefixFormat, i);
                var contribution = random.Next(0, 100);
                var failureMechanism = mockRepository.StrictMock<IFailureMechanism>();
                failureMechanism.Expect(fm => fm.Name).Return(name);
                failureMechanism.Expect(fm => fm.Contribution).Return(contribution);

                failureMechanisms.Add(failureMechanism);
                failureMechanismNames.Add(name);
                failureMechanismContributions.Add(contribution);
            }

            failureMechanismNames.Add(Resources.OtherFailureMechanism_DisplayName);
            failureMechanismContributions.Add(otherContribution);

            mockRepository.ReplayAll();
            var norm = random.Next();

            // Call
            var result = new FailureMechanismContribution(failureMechanisms, otherContribution, norm);

            // Assert
            Assert.AreEqual(failureMechanismCount + 1, result.Distribution.Count());

            CollectionAssert.AreEqual(failureMechanismNames, result.Distribution.Select(d => d.Assessment));
            CollectionAssert.AreEqual(failureMechanismContributions, result.Distribution.Select(d => d.Contribution));
            CollectionAssert.AreEqual(failureMechanismContributions.Select(c => (norm / c) * 100), result.Distribution.Select(d => d.ProbabilitySpace));

            mockRepository.VerifyAll();
        }
    }
}