using System;
using Core.Common.Base.Storage;
using NUnit.Framework;

namespace Core.Common.Base.Test.Storage
{
    [TestFixture]
    public class StorageValidationExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            string expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(StorageValidationException).FullName);

            // Call
            StorageValidationException exception = new StorageValidationException();

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
            StorageValidationException exception = new StorageValidationException(expectedMessage);

            // Assert
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
            StorageValidationException exception = new StorageValidationException(expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}