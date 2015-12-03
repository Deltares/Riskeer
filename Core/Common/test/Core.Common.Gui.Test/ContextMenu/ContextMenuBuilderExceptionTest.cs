using System;
using Core.Common.Gui.ContextMenu;
using NUnit.Framework;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class ContextMenuBuilderExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            var expectedMessage = string.Format("Exception of type '{0}' was thrown.", typeof(ContextMenuBuilderException).FullName);

            // Call
            var exception = new ContextMenuBuilderException();

            // Assert
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessage_InnerExceptionNullAndMessageSetToCustom()
        {
            // Setup
            var expectedMessage = "Some exception message";

            // Call
            var exception = new ContextMenuBuilderException(expectedMessage);

            // Assert
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessageAndInnerException_InnerExceptionSetAndMessageSetToCustom()
        {
            // Setup
            var expectedMessage = "Some exception message";
            var expectedInnerException = new Exception();

            // Call
            var exception = new ContextMenuBuilderException(expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}