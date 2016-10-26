using NUnit.Framework;

namespace Ringtoets.Common.Service.Test
{
    [TestFixture]
    public class ParameterNameExtractorTest
    {
        [Test]
        public void GetFromDisplayName_EmptyString_ReturnEmptyString()
        {
            // Call
            string result = ParameterNameExtractor.GetFromDisplayName(string.Empty);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void GetFromDisplayName_ParameterWithoutUnits_ReturnParameterName()
        {
            // Setup
            const string parameterName = " A ";

            // Call
            string result = ParameterNameExtractor.GetFromDisplayName(parameterName);

            // Assert
            Assert.AreEqual("a", result);
        }

        [Test]
        public void GetFromDisplayName_ParameterWithUnits_ReturnParameterName()
        {
            // Setup
            const string parameterName = " A ";
            string parameterWithUnits = string.Format("{0} [m/s]", parameterName);

            // Call
            string result = ParameterNameExtractor.GetFromDisplayName(parameterWithUnits);

            // Assert
            Assert.AreEqual("a", result);
        }
    }
}