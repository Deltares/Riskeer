using Core.Common.Utils.Builders;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Builders
{
    [TestFixture]
    public class FileReaderErrorMessageBuilderTest
    {
        [Test]
        public void Build_BasedOnPathAndMessage_ReturnBuiltErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";

            // Call
            var message = new FileReaderErrorMessageBuilder(filePath).Build(errorMessage);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': {1}",
                                                filePath, errorMessage);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Build_BasedOnPathAndMessageWithLocation_ReturnBuiltErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";
            const string location = "<location description>";

            // Call
            var message = new FileReaderErrorMessageBuilder(filePath).WithLocation(location).Build(errorMessage);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' {1}: {2}",
                                                filePath, location, errorMessage);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Build_BasedOnPathAndMessageWithSubject_ReturnBuiltErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";
            const string subject = "<subject description>";

            // Call
            var message = new FileReaderErrorMessageBuilder(filePath).WithSubject(subject).Build(errorMessage);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' ({1}): {2}",
                                                filePath, subject, errorMessage);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Build_BasedOnPathAndMessageWithSubjectAndLocation_ReturnBuiltErrorMessage()
        {
            // Setup
            const string filePath = "<file path>";
            const string errorMessage = "test test 1,2,3";
            const string subject = "<subject description>";
            const string location = "<location description>";

            // Call
            var message = new FileReaderErrorMessageBuilder(filePath).WithSubject(subject).WithLocation(location).Build(errorMessage);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}' {1} ({2}): {3}",
                                                filePath, location, subject, errorMessage);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}