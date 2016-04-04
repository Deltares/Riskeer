using NUnit.Framework;

namespace Ringtoets.Common.Data.Test
{
    [TestFixture]
    public class AssessmentSectionCommentTest
    {
        [Test]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            var comment = new AssessmentSectionComment();

            // Assert
            Assert.IsNullOrEmpty(comment.Text);
        }

        [Test]
        public void Text_SetNewValue_ReturnsNewValue()
        {
            // Setup
            var comment = new AssessmentSectionComment();
            var newValue = "Some new value";

            // Call
            comment.Text = newValue;

            // Assert
            Assert.AreEqual(newValue, comment.Text);
        }
    }
}