using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.Tests.Geometry
{
    [TestFixture]
    public class MultiPointTest
    {
        [Test]
        public void GetHashCodeShoulBeComputed()
        {
            var multiPoint = new MultiPoint(new[] { new Point(1.0, 2.0), new Point(3.0, 4.0) });

            Assert.AreEqual(2024009049, multiPoint.GetHashCode());
        }
    }
}