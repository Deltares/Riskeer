using NUnit.Framework;

namespace Core.Common.Utils.Tests
{
    [TestFixture]
    public class MathUtilsTest
    {
        [Test]
        public void ClipValueTest()
        {
            Assert.AreEqual(0, MathUtils.ClipValue(int.MinValue, 0, 10));
            Assert.AreEqual(0, MathUtils.ClipValue(-1, 0, 10));
            Assert.AreEqual(0, MathUtils.ClipValue(0, 0, 10));

            Assert.AreEqual(3, MathUtils.ClipValue(3, 0, 10));

            Assert.AreEqual(10, MathUtils.ClipValue(10, 0, 10));
            Assert.AreEqual(10, MathUtils.ClipValue(11, 0, 10));
            Assert.AreEqual(10, MathUtils.ClipValue(int.MaxValue, 0, 10));
        }
    }
}