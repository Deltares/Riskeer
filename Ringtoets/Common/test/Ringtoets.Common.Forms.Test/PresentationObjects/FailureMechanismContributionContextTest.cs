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
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismContributionContextTest
    {
        [Test]
        public void Contructor_ExpectedValues()
        {
            // Setup
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 30000);

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var context = new FailureMechanismContributionContext(contribution, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IEquatable<FailureMechanismContributionContext>>(context);
            Assert.IsInstanceOf<IObservable>(context);
            Assert.AreSame(contribution, context.WrappedData);
            Assert.AreSame(assessmentSection, context.Parent);
            mocks.VerifyAll();
        }

        [Test]
        public void Contructor_FailureMechanismContributionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismContributionContext(null, assessmentSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Failure mechanism contribution cannot be null.");
            mocks.VerifyAll();
        }

        [Test]
        public void Contructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 30000);

            // Call
            TestDelegate call = () => new FailureMechanismContributionContext(contribution, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Assessment section cannot be null.");
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 30000);

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToDifferentObjectType_ReturnFalse()
        {
            // Setup
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 30000);

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);
            object differentObject = new object();

            // Call
            bool isEqual1 = context.Equals(differentObject);
            bool isEqual2 = differentObject.Equals(context);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToEqualObject_ReturnTrue()
        {
            // Setup
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 30000);

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context1 = new FailureMechanismContributionContext(contribution, assessmentSection);
            object context2 = new FailureMechanismContributionContext(contribution, assessmentSection);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void Equals_ToInequalObject_ReturnFalse()
        {
            // Setup
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution1 = new FailureMechanismContribution(failureMechanisms, 1.1, 30000);
            var contribution2 = new FailureMechanismContribution(failureMechanisms, 2.2, 40000);

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context1 = new FailureMechanismContributionContext(contribution1, assessmentSection);
            var context2 = new FailureMechanismContributionContext(contribution2, assessmentSection);

            // Precondition
            Assert.IsFalse(contribution1.Equals(contribution2));

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void GetHashCode_ComparingEqualObject_ReturnEqualHashCodes()
        {
            // Setup
            var failureMechanisms = Enumerable.Empty<IFailureMechanism>();
            var contribution = new FailureMechanismContribution(failureMechanisms, 1.1, 30000);

            var mocks = new MockRepository();
            IAssessmentSection assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context1 = new FailureMechanismContributionContext(contribution, assessmentSection);
            var context2 = new FailureMechanismContributionContext(contribution, assessmentSection);

            // Call
            int hash1 = context1.GetHashCode();
            int hash2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void NotifyObservers_WithAttachedObserver_UpdateObserverCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 30000);

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);
            context.Attach(observer);

            // Call
            context.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect UpdateObserver called
        }

        [Test]
        public void NotifyObservers_WithDetachedObserver_NoUpdateObserverCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 30000);

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);
            context.Attach(observer);
            context.Detach(observer);

            // Call
            context.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_WithAttachedObserverToContribution_UpdateObserverCalled()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 30000);
            contribution.Attach(observer);

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);

            // Call
            context.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect UpdateObserver called
        }

        [Test]
        public void GivenObserverAttachedToContext_WhenWrappedDataNotifiesObservers_ThenUpdateObserverCalled()
        {
            // Given
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var contribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100.0, 30000);

            var context = new FailureMechanismContributionContext(contribution, assessmentSection);
            context.Attach(observer);

            // When
            contribution.NotifyObservers();

            // Then
            mocks.VerifyAll(); // Expect no UpdateObserver call
        }
    }
}