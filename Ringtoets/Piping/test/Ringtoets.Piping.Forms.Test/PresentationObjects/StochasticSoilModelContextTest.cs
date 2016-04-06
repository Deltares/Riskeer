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
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StochasticSoilModelContextTest
    {
        [Test]
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new Data.Piping();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new StochasticSoilModelContext(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new Data.Piping();

            // Call
            TestDelegate test = () => new StochasticSoilModelContext(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToStochasticSoilModels()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new Data.Piping();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            // Call
            context.Attach(observer);

            // Assert
            failureMechanism.StochasticSoilModels.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromStochasticSoilModels()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new Data.Piping();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            context.Attach(observer);

            // Call
            context.Detach(observer);

            // Assert
            failureMechanism.StochasticSoilModels.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToStochasticSoilModels_NotificationCorrectlyPropagated()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new Data.Piping();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            failureMechanism.StochasticSoilModels.Attach(observer); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new Data.Piping();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new Data.Piping();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToEqualOtherInstance_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new Data.Piping();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            var otherContext = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            Assert.IsTrue(isEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToInequalOtherInstance_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new Data.Piping();
            var otherFailureMechanism = new Data.Piping();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            var otherContext = new StochasticSoilModelContext(otherFailureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsFalse(isEqual);
            Assert.IsFalse(isEqual2);
            mocks.VerifyAll();
        }
        
        [Test]
        public void Equals_ToInequalOtherObject_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new Data.Piping();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            var otherContext = new Object();

            // Call
            bool isEqual = context.Equals(otherContext);

            // Assert
            Assert.IsFalse(isEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void GetHashCode_TwoContextInstancesEqualToEachOther_ReturnIdenticalHashes()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = new Data.Piping();
            mocks.ReplayAll();

            var context = new StochasticSoilModelContext(failureMechanism, assessmentSection);

            var otherContext = new StochasticSoilModelContext(failureMechanism, assessmentSection);
            // Precondition
            Assert.True(context.Equals(otherContext));

            // Call
            int contextHashCode = context.GetHashCode();
            int otherContextHashCode = otherContext.GetHashCode();

            // Assert
            Assert.AreEqual(contextHashCode, otherContextHashCode);
            mocks.VerifyAll();
        }
    }
}