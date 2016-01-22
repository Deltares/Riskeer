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
            string expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(EntityNotFoundException).FullName);

            // Call
            EntityNotFoundException exception = new EntityNotFoundException();

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessage_InnerExceptionNullAndMessageSetToCustom()
        {
            // Setup
            const string expectedMessage = "Some exception message";

            // Call
            EntityNotFoundException exception = new EntityNotFoundException(expectedMessage);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessageAndInnerException_InnerExceptionSetAndMessageSetToCustom()
        {
            // Setup
            const string expectedMessage = "Some exception message";
            Exception expectedInnerException = new Exception();

            // Call
            EntityNotFoundException exception = new EntityNotFoundException(expectedMessage, expectedInnerException);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}