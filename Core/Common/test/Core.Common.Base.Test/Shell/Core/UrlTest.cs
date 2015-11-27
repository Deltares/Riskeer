using Core.Common.Utils;
using NUnit.Framework;

namespace Core.Common.Base.Test.Shell.Core
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
    }
}