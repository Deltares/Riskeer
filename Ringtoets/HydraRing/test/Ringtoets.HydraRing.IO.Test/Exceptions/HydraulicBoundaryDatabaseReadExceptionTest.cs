using System;
using NUnit.Framework;
using Ringtoets.HydraRing.IO.Exceptions;

namespace Ringtoets.HydraRing.IO.Test.Exceptions
{
    public class HydraulicBoundaryDatabaseReadExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            string expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(HydraulicBoundaryDatabaseReadException).FullName);

            // Call
            HydraulicBoundaryDatabaseReadException exception = new HydraulicBoundaryDatabaseReadException();

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
            HydraulicBoundaryDatabaseReadException exception = new HydraulicBoundaryDatabaseReadException(expectedMessage);

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
            HydraulicBoundaryDatabaseReadException exception = new HydraulicBoundaryDatabaseReadException(expectedMessage, expectedInnerException);

            // Assert
            Assert.IsInstanceOf<Exception>(exception);
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}