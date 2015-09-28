using NUnit.Framework;
using SharpMap.UI.Helpers;

namespace SharpMap.UI.Tests.Helpers
{
    [TestFixture]
    public class MapCursorsTest
    {
        [Test]
        public void CheckNoCopiesAreReturned()
        {
            var c1 = MapCursors.AddPoint;
            var c2 = MapCursors.AddPoint;
            Assert.AreSame(c1, c2);
        }
    }
}