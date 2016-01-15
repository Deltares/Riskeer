using System;
using Application.Ringtoets.Storage.Exceptions;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Exceptions
{
    [TestFixture]
    public class EntityNotFoundExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            var expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(EntityNotFoundException).FullName);

            // Call
            var exception = new EntityNotFoundException();

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
            var exception = new EntityNotFoundException(expectedMessage);

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
            var exception = new EntityNotFoundException(expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}