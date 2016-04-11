using System;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
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
            var commentMock = mocks.StrictMock<ICommentable>();
            mocks.ReplayAll();

            // Call
            var context = new CommentContext<ICommentable>(commentMock);

            // Assert
            Assert.AreSame(commentMock, context.CommentContainer);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_CommentContainerNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new CommentContext<ICommentable>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("commentContainer", exception.ParamName);
        }
    }
}