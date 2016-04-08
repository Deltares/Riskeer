using System;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
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
            var commentMock = mocks.StrictMock<IComment>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            var context = new CommentContext<IComment>(commentMock, assessmentSectionMock);

            // Assert
            Assert.AreSame(commentMock, context.CommentContainer);
            Assert.AreSame(assessmentSectionMock, context.AssessmentSection);
        }

        [Test]
        public void Constructor_CommentContainerNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();
            // Call
            TestDelegate call = () => new CommentContext<IComment>(null, assessmentSectionMock);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("commentContainer", exception.ParamName);
        }

        [Test]
        public void Constructor_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var commentMock = mocks.StrictMock<IComment>();

            mocks.ReplayAll();
            // Call
            TestDelegate call = () => new CommentContext<IComment>(commentMock, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }
    }
}