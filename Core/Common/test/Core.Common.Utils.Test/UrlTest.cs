using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    [TestFixture]
    public class UrlTest
    {
        [Test]
        public void Initialization()
        {
            var url = new Url("Deltares", "http://www.deltares.com");

            Assert.AreEqual("Deltares", url.Name);
            Assert.AreEqual("http://www.deltares.com", url.Path);
        }

        [Test]
        public void SipleProperties_SetAndGetValue_ReturnNewlySetValue()
        {
            // Setup
            var url = new Url("Deltares", "http://www.deltares.com");

            const string newName = "Google";
            const string newPath = "http://www.google.com";

            // Call
            url.Name = newName;
            url.Path = newPath;

            // Assert
            Assert.AreEqual(newName, url.Name);
            Assert.AreEqual(newPath, url.Path);
        }
    }
}