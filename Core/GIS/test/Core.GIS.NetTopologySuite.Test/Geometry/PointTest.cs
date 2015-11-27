using Core.GIS.NetTopologySuite.Geometries;
using NUnit.Framework;

namespace Core.GIS.NetTopologySuite.Test.Geometry
{
    [TestFixture]
    public class PointTest
    {
        [Test]
        public void GetHashCodeMustBeComputed()
        {
            var first = new Point(1.0, 2.0);

            Assert.AreEqual(712319917, first.GetHashCode());
        }
    }
}