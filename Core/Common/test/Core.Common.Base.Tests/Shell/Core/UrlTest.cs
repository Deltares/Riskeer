using Core.Common.Utils;
using NUnit.Framework;

namespace Core.Common.Base.Tests.Shell.Core
{
    [TestFixture]
    public class UrlTest
    {
        [Test]
        public void Initialization()
        {
            Url url = new Url("Deltares", "http://www.deltares.com");

            Assert.AreEqual("Deltares", url.Name);
            Assert.AreEqual("http://www.deltares.com", url.Path);
        }

        [Test]
        public void Cloning()
        {
            Url url = new Url("Deltares", "http://www.deltares.com");
            Url urlClone = (Url) url.Clone();
            Assert.AreEqual(urlClone.Name, url.Name);
            Assert.AreEqual(urlClone.Path, url.Path);
        }
    }
}