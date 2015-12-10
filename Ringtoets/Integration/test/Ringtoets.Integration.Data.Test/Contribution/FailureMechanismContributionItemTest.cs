using System;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data.Test.Contribution
{
    [TestFixture]
    public class FailureMechanismContributionItemTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_WithoutFailureMechanism_ThrowsArgumentNullException()
        {
            // Setup

            // Call
            TestDelegate test = () => new FailureMechanismContributionItem(null, new Random(21).Next());

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.FailureMechanismContributionItem_Can_not_create_contribution_item_without_failure_mechanism, message);
            StringAssert.EndsWith("failureMechanism", message);
        }

        [Test]
        public void Constructor_WithFailureMechanism_SetProperties()
        {
            // Setup
            string name = "SomeName";
            double contribution = new Random(21).NextDouble();

            var failureMechanism = mockRepository.StrictMock<IFailureMechanism>();

            failureMechanism.Expect(fm => fm.Name).Return(name);
            failureMechanism.Expect(fm => fm.Contribution).Return(contribution);

            mockRepository.ReplayAll();

            // Call
            var result = new FailureMechanismContributionItem(failureMechanism, new Random(21).Next());

            // Assert
            Assert.AreEqual(name,result.Assessment);
            Assert.AreEqual(contribution,result.Contribution);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(20, 2000, 10000)]
        [TestCase(3, 100, 10000/3.0)]
        [TestCase(25.5, 2550, 10000)]
        public void ProbabilitySpace_DifferentContributionAndNorm_ReturnsExpectedValue(double contribution, int norm, double expectedResult)
        {
            // Setup
            string name = "SomeName";

            var failureMechanism = mockRepository.StrictMock<IFailureMechanism>();

            failureMechanism.Expect(fm => fm.Name).Return(name);
            failureMechanism.Expect(fm => fm.Contribution).Return(contribution);

            mockRepository.ReplayAll();

            var contributionItem = new FailureMechanismContributionItem(failureMechanism, norm);

            // Call
            var result = contributionItem.ProbabilitySpace;

            // Assert
            Assert.AreEqual(expectedResult, result, double.Epsilon);

            mockRepository.VerifyAll();
        }
    }
}