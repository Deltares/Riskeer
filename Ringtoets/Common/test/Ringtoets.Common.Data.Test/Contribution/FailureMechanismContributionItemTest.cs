// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
            TestDelegate test = () => new FailureMechanismContributionItem(null, norm);

            // Assert
            const string expectedMessage = "Kan geen bijdrage element maken zonder een toetsspoor.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WithFailureMechanism_SetProperties()
        {
            // Setup
            string name = "SomeName";
            string code = "SN";
            var random = new Random(21);
            double contribution = random.Next(1, 100);
            var norm = random.Next(1, int.MaxValue);
            const bool isRelevant = false;

            var failureMechanism = mockRepository.StrictMock<IFailureMechanism>();

            failureMechanism.Expect(fm => fm.Name).Return(name);
            failureMechanism.Expect(fm => fm.Code).Return(code);
            failureMechanism.Expect(fm => fm.Contribution).Return(contribution);
            failureMechanism.Expect(fm => fm.IsRelevant).Return(isRelevant);

            mockRepository.ReplayAll();

            // Call
            var result = new FailureMechanismContributionItem(failureMechanism, norm);

            // Assert
            Assert.AreEqual(name, result.Assessment);
            Assert.AreEqual(contribution, result.Contribution);
            Assert.AreEqual(code, result.AssessmentCode);
            Assert.AreEqual(norm, result.Norm);
            Assert.IsFalse(result.IsAlwaysRelevant);
            Assert.AreEqual(isRelevant, result.IsRelevant);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Constructor_WithFailureMechanismThatIsAlwaysRelevant_SetProperties()
        {
            // Setup
            string name = "SomeName";
            string code = "SN";
            var random = new Random(21);
            double contribution = random.Next(1, 100);
            var norm = random.Next(1, int.MaxValue);
            const bool isRelevant = false;

            var failureMechanism = mockRepository.StrictMock<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.Name).Return(name);
            failureMechanism.Expect(fm => fm.Code).Return(code);
            failureMechanism.Expect(fm => fm.Contribution).Return(contribution);
            failureMechanism.Stub(fm => fm.IsRelevant).Return(isRelevant);

            mockRepository.ReplayAll();

            // Call
            var result = new FailureMechanismContributionItem(failureMechanism, norm, true);

            // Assert
            Assert.AreEqual(name, result.Assessment);
            Assert.AreEqual(contribution, result.Contribution);
            Assert.AreEqual(code, result.AssessmentCode);
            Assert.AreEqual(norm, result.Norm);
            Assert.IsTrue(result.IsAlwaysRelevant);
            Assert.IsTrue(result.IsRelevant);

            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(20, 2000, 10000)]
        [TestCase(3, 100, 10000/3.0)]
        [TestCase(25.5, 2550, 10000)]
        public void ProbabilitySpace_DifferentContributionAndNorm_ReturnsExpectedValue(double contribution, int norm, double expectedResult)
        {
            // Setup
            var failureMechanism = mockRepository.StrictMock<IFailureMechanism>();
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
        public void IsRelevant_SetNewValueWhenAlwaysRelevant_NothingChanges()
        {
            // Setup
            var failureMechanism = mockRepository.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = false;
            mockRepository.ReplayAll();

            var contributionItem = new FailureMechanismContributionItem(failureMechanism, 30000, true);

            // Precondition:
            Assert.IsTrue(contributionItem.IsRelevant);

            // Call
            contributionItem.IsRelevant = false;

            // Assert
            Assert.IsTrue(contributionItem.IsRelevant);
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