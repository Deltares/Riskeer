using System;
using Application.Ringtoets.Storage.Exceptions;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Exceptions
{
    [TestFixture]
    public class InvalidFileExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            var expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(InvalidFileException).FullName);

            // Call
            var exception = new InvalidFileException();

            // Assert
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessage_InnerExceptionNullAndMessageSetToCustom()
        {
            // Setup
            var expectedMessage ="Some exception message";

            // Call
            var exception = new InvalidFileException(expectedMessage);

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
            var exception = new InvalidFileException(expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}