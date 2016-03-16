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
    public class RingtoetsPipingSurfaceLineContextTest
    {
        [Test]
        public void ParameteredConstructor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = new PipingFailureMechanism();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            var context = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);

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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new RingtoetsPipingSurfaceLineContext(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => new RingtoetsPipingSurfaceLineContext(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void NotifyObservers_ObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var context = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);

            context.Attach(observer);

            // Call
            context.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect attach and notify observers on failure mechanism
        }

        [Test]
        public void NotifyObservers_ObserverDetached_NoCallsOnObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var failureMechanism = new PipingFailureMechanism();

            var presentationObject = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);

            // Call
            presentationObject.Detach(observer);

            // Assert
            mocks.VerifyAll(); // Expect detach from failure mechanism
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);

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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);

            var otherContext = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);

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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var failureMechanism = new PipingFailureMechanism();
            var otherFailureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);

            var otherContext = new RingtoetsPipingSurfaceLineContext(otherFailureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsFalse(isEqual);
            Assert.IsFalse(isEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void GetHashCode_TwoContextInstancesEqualToEachOther_ReturnIdenticalHashes()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var failureMechanism = new PipingFailureMechanism();
            mocks.ReplayAll();

            var context = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);

            var otherContext = new RingtoetsPipingSurfaceLineContext(failureMechanism, assessmentSection);
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
