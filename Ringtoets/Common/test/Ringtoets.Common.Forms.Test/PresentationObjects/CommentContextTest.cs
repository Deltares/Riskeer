using System;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class CommentContextTest
    {
        [Test]
        public void Constuctor_DefaultValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            var context = new CommentContext<IAssessmentSection>(assessmentSectionMock);

            // Assert
            Assert.AreSame(assessmentSectionMock, context.CommentContainer);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new CommentContext<IAssessmentSection>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("commentContainer", exception.ParamName);
        }
    }
}