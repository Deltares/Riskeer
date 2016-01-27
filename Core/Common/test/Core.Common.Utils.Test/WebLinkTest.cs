using System;

using NUnit.Framework;

namespace Core.Common.Utils.Test
{
    [TestFixture]
    public class WebLinkTest
    {
        [Test]
        public void Initialization()
        {
            // Setup
            const string name = "Deltares";
            var path = new Uri("http://www.deltares.com");

            // Call
            var url = new WebLink(name, path);

            // Assert
            Assert.AreEqual(name, url.Name);
            Assert.AreEqual(path, url.Path);
        }

        [Test]
        public void SimpleProperties_SetAndGetValue_ReturnNewlySetValue()
        {
            // Setup
            var url = new WebLink("Deltares", new Uri("http://www.deltares.com"));

            const string newName = "Google";
            var newPath = new Uri("http://www.google.com");

            // Call
            url.Name = newName;
            url.Path = newPath;

            // Assert
            Assert.AreEqual(newName, url.Name);
            Assert.AreEqual(newPath, url.Path);
        }
    }
}