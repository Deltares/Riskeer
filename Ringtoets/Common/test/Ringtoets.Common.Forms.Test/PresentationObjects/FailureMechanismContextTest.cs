using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var context = new SimpleFailureMechanismContext(failureMechanism, assessmentSection);

            // Assert
            Assert.AreSame(assessmentSection, context.Parent);
            Assert.AreSame(failureMechanism, context.WrappedData);
            mocks.VerifyAll();
        }

        private class SimpleFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public SimpleFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, AssessmentSectionBase parent) : base(wrappedFailureMechanism, parent) {}
        }
    }
}