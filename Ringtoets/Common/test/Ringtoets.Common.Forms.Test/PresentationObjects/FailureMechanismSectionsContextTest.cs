using System;
using System.Collections.Generic;

using Core.Common.Base;
using Core.Common.Base.Geometry;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismSectionsContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IEnumerable<FailureMechanismSection> sectionsSequence = new[]
            {
                new FailureMechanismSection("A", new[]
                {
                    new Point2D(1, 2),
                    new Point2D(3, 4)
                }),
                new FailureMechanismSection("B", new[]
                {
                    new Point2D(3, 4),
                    new Point2D(5, 6)
                })
            };

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Sections).Return(sectionsSequence);

            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            var context = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.AreSame(sectionsSequence, context.WrappedData);
            Assert.AreSame(failureMechanism, context.ParentFailureMechanism);
            Assert.AreSame(assessmentSection, context.ParentAssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsContext(null, assessmentSection);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new FailureMechanismSectionsContext(failureMechanism, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.Attach(observer));
            failureMechanism.Expect(fm => fm.NotifyObservers());
            mocks.ReplayAll();

            var presentationObject = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            presentationObject.Attach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect attach and notify observers on failure mechanism
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverDetached_NoCallsOnObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();

            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.Detach(observer));
            mocks.ReplayAll();

            var presentationObject = new FailureMechanismSectionsContext(failureMechanism, assessmentSection);

            // Call
            presentationObject.Detach(observer);

            // Assert
            mocks.VerifyAll(); // Expect detach from failure mechanism
        }
    }
}