using System;
using Application.Ringtoets.Storage.Exceptions;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Exceptions
{
    [TestFixture]
    public class UpdateDatabaseExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            string expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(UpdateDatabaseException).FullName);

            // Call
            UpdateDatabaseException exception = new UpdateDatabaseException();

            // Assert
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessage_InnerExceptionNullAndMessageSetToCustom()
        {
            // Setup
            const string expectedMessage = "Some exception message";

            // Call
            UpdateDatabaseException exception = new UpdateDatabaseException(expectedMessage);

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
            UpdateDatabaseException exception = new UpdateDatabaseException(expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}