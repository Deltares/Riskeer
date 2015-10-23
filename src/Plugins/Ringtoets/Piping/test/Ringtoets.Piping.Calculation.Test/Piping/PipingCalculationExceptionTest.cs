using System;
using NUnit.Framework;
using Wti.Calculation.Piping;

namespace Wti.Calculation.Test.Piping
{
    public class PipingCalculationExceptionTest
    {
        [Test]
        public void DefaultConstructor_InnerExceptionNullAndMessageDefault()
        {
            // Setup
            var expectedMessage = String.Format("Exception of type '{0}' was thrown.", typeof(PipingCalculationException).FullName);

            // Call
            var exception = new PipingCalculationException();

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
            var exception = new PipingCalculationException(expectedMessage);

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
            var exception = new PipingCalculationException(expectedMessage, expectedInnerException);

            // Assert
            Assert.AreSame(expectedInnerException, exception.InnerException);
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}