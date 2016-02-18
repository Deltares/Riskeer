using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class ReferenceLineContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var referenceLine = new ReferenceLine();

            // Call
            var referenceLineContext = new ReferenceLineContext(referenceLine, assessmentSection);

            // Assert
            Assert.AreSame(referenceLine, referenceLineContext.WrappedData);
            Assert.AreSame(assessmentSection, referenceLineContext.Parent);
            mocks.VerifyAll();
        }
    }
}