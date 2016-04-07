using System;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            // Assert
            Assert.AreSame(assessmentSection, context.Parent);
            Assert.AreSame(failureMechanism, context.WrappedData);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new SimpleFailureMechanismContext(null, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("wrappedFailureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new SimpleFailureMechanismContext(failureMechanism, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parent", exception.ParamName);
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

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
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

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
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);
            var otherContext = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            Assert.IsTrue(isEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToInequalOtherFailureMechanismInstance_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            var otherFailureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);
            var otherContext = new SimpleFailureMechanismContext(otherFailureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsFalse(isEqual);
            Assert.IsFalse(isEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherTypeInstance_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);
            var otherObject = new object();

            // Call
            bool isEqual = context.Equals(otherObject);
            bool isEqual2 = otherObject.Equals(context);

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
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            var otherContext = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            // Precondition
            Assert.True(context.Equals(otherContext));

            // Call
            int contextHashCode = context.GetHashCode();
            int otherContextHashCode = otherContext.GetHashCode();

            // Assert
            Assert.AreEqual(contextHashCode, otherContextHashCode);
            mocks.VerifyAll();
        }
				

        private class SimpleFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public SimpleFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) : base(wrappedFailureMechanism, parent) {}
        }
    }
}