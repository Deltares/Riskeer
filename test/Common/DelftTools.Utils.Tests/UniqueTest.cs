using System.Linq;
using DelftTools.TestUtils;
using DelftTools.Utils.Data;
using NUnit.Framework;

namespace DelftTools.Utils.Tests
{
    [TestFixture]
    public class UniqueTest
    {
        [Test]
        public void GetHashCodeValue()
        {
            var unique = new Unique<long>();
            Assert.AreNotEqual(0, unique.GetHashCode()); //may fail on rare occasions..
            Assert.AreNotEqual(-1, unique.GetHashCode()); //may fail on rare occasions..
            Assert.AreEqual(unique.GetHashCode(), unique.GetHashCode());
        }

        [Test]
        public void GetHashCodeShouldBeFast()
        {
            var uniques = Enumerable.Range(1, 100000).Select(i => new Unique<long>()).ToList();

            TestHelper.AssertIsFasterThan(110, //between around 45
                                          () =>
                                              {
                                                  foreach (var unique in uniques)
                                                  {
                                                      for (int i = 0; i < 100; i++)
                                                      {
                                                          int h = unique.GetHashCode();
                                                      }
                                                  }
                                              }, false, true);
        }
    }
}