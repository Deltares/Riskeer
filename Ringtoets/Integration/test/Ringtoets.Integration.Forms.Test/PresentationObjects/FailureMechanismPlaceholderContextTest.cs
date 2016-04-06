using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class FailureMechanismPlaceholderContextTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new Placeholder("A");

            // Call
            var context = new FailureMechanismPlaceholderContext(failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismContext<Placeholder>>(context);
            Assert.AreSame(failureMechanism, context.WrappedData);
            Assert.AreSame(assessmentSection, context.Parent);
            mocks.VerifyAll();
        }
    }
}