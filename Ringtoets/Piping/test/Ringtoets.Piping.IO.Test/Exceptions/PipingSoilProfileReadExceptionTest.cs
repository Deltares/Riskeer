using System;
using NUnit.Framework;
using Ringtoets.Piping.IO.Exceptions;

namespace Ringtoets.Piping.IO.Test.Exceptions
{
    public class PipingSoilProfileReadExceptionTest
    {
        [Test]
        public void Constructor_WithProfileName_InnerExceptionNullMessageDefaultAndProfileNameSet()
        {
            // Setup
            var profileName = "name";
            var expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(PipingSoilProfileReadException).FullName);

            // Call
            var exception = new PipingSoilProfileReadException(profileName);

            // Assert
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessage_InnerExceptionNullAndMessageSetToCustom()
        {
            // Setup
            var profileName = "name";
            var expectedMessage = "Some exception message";

            // Call
            var exception = new PipingSoilProfileReadException(profileName, expectedMessage);

            // Assert
            Assert.IsNull(exception.InnerException);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithCustomMessageAndInnerException_InnerExceptionSetAndMessageSetToCustom()
        {
            // Setup
            var profileName = "name";
            var expectedMessage = "Some exception message";
            var expectedInnerException = new Exception();

            // Call
            var exception = new PipingSoilProfileReadException(profileName, expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(profileName, exception.ProfileName);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}