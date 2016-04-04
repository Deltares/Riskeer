using System;

using Core.Common.TestUtil;

using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class SemiProbabilisticPipingInputTest
    {
        [Test]
        public void Constructor_DefaultPropertiesSet()
        {
            // Call
            var inputParameters = new SemiProbabilisticPipingInput();

            // Assert
            Assert.AreEqual(1.0, inputParameters.A);
            Assert.AreEqual(350.0, inputParameters.B);

            Assert.IsNaN(inputParameters.SectionLength);
            Assert.AreEqual(0, inputParameters.Norm);
            Assert.IsNaN(inputParameters.Contribution);
        }

        [Test]
        [TestCase(0)]
        [TestCase(45.67)]
        [TestCase(100)]
        public void Contribution_SetNewValidValue_GetNewValue(double newContributionValue)
        {
            // Setup
            var inputParameters = new SemiProbabilisticPipingInput();

            // Call
            inputParameters.Contribution = newContributionValue;

            // Assert
            Assert.AreEqual(newContributionValue, inputParameters.Contribution);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-123.545)]
        [TestCase(100+1e-6)]
        [TestCase(5678.9)]
        public void Contribution_SetNewInvalidValue_ThrowArgumentOutOfRangeException(double newContributionValue)
        {
            // Setup
            var inputParameters = new SemiProbabilisticPipingInput();

            // Call
            TestDelegate call = () => inputParameters.Contribution = newContributionValue;

            // Assert
            const string expectedMessage = "De waarde voor de toegestane bijdrage aan faalkans moet in interval [0,100] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }
    }
}