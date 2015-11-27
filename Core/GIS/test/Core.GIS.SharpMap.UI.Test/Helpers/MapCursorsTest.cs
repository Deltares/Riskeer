using Core.GIS.SharpMap.UI.Helpers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.UI.Test.Helpers
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