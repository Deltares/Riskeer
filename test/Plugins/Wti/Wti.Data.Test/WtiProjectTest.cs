using DelftTools.Utils;

using NUnit.Framework;

namespace Wti.Data.Test
{
    [TestFixture]
    public class WtiProjectTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var project = new WtiProject();

            // assert
            Assert.IsInstanceOf<INameable>(project);
            Assert.AreEqual("WTI 2017 project", project.Name);
        }
    }
}