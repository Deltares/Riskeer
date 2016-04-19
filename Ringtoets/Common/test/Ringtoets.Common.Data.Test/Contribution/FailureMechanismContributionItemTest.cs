﻿using System;

using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.Data.Test.Contribution
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
            var norm = new Random(21).Next(1, int.MaxValue);

            // Call
            TestDelegate test = () =>
            {
                new FailureMechanismContributionItem(null, norm);
            };

            // Assert
            const string expectedMessage = "Kan geen bijdrage element maken zonder een faalmechanisme.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WithFailureMechanism_SetProperties()
        {
            // Setup
            string name = "SomeName";
            var random = new Random(21);
            double contribution = random.Next(1, 100);
            var norm = random.Next(1, int.MaxValue);
            const bool isRelevant = false;

            var failureMechanism = mockRepository.StrictMock<IFailureMechanism>();

            failureMechanism.Expect(fm => fm.Name).Return(name);
            failureMechanism.Expect(fm => fm.Contribution).Return(contribution);
            failureMechanism.Expect(fm => fm.IsRelevant).Return(isRelevant);

            mockRepository.ReplayAll();

            // Call
            var result = new FailureMechanismContributionItem(failureMechanism, norm);

            // Assert
            Assert.AreEqual(name, result.Assessment);
            Assert.AreEqual(contribution, result.Contribution);
            Assert.AreEqual(norm, result.Norm);
            Assert.AreEqual(isRelevant, result.IsRelevant);

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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsRelevant_SetNewValue_FailureMechanismIsRelevantChanged(bool newIsRelevantValue)
        {
            // Setup
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            mockRepository.ReplayAll();

            var contributionItem = new FailureMechanismContributionItem(failureMechanism, 30000);

            // Call
            contributionItem.IsRelevant = newIsRelevantValue;

            // Assert
            Assert.AreEqual(newIsRelevantValue, failureMechanism.IsRelevant);
            mockRepository.VerifyAll();
        }

        [Test]
        public void NotifyFailureMechanismObservers_Always_CallsFailureMechanismUpdateObservers()
        {
            // Setup
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return("A");
            failureMechanism.Expect(fm => fm.NotifyObservers());
            mockRepository.ReplayAll();
            
            var contributionItem = new FailureMechanismContributionItem(failureMechanism, 30000);

            // Call
            contributionItem.NotifyFailureMechanismObservers();

            // Assert
            mockRepository.VerifyAll(); // Expect NotifyObservers on 'failureMechanism'
        }
    }
}