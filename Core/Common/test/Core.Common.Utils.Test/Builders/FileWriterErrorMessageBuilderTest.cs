using Core.Common.Utils.Builders;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Builders
{
    [TestFixture]
    public class FileWriterErrorMessageBuilderTest
    {
        [Test]
        public void Build_BasedOnPathAndMessage_ReturnBuildErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";

            // Call
            var message = new FileWriterErrorMessageBuilder(filePath).Build(errorMessage);

            // Assert
            var expectedMessage = string.Format("Fout bij het schrijven naar bestand '{0}': {1}",
                                                filePath, errorMessage);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}