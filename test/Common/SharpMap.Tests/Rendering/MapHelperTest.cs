using System.Drawing;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.Rendering;

namespace SharpMap.Tests.Rendering
{
    [TestFixture]
    public class MapHelperTest
    {
        [Test]
        public void ImageToWorld()
        {
            var map = new Map(new Size(400, 300)) {Zoom = 50};

            var coordinate = MapHelper.ImageToWorld(map, 15, 15);

            Assert.AreEqual(1.875, coordinate.X);
            Assert.AreEqual(1.875, coordinate.Y);
        }

        [Test]
        public void GetEnvelope()
        {
            var envelope = MapHelper.GetEnvelope(new Coordinate(5, 5), 7, 6);

            Assert.AreEqual(5, envelope.Centre.X);
            Assert.AreEqual(5, envelope.Centre.Y);
            Assert.AreEqual(7, envelope.Width);
            Assert.AreEqual(6, envelope.Height);
        }

        [Test]
        public void GetEnvelopeForImage()
        {
            var map = new Map(new Size(400, 300)) { Zoom = 50 };

            var envelope = MapHelper.GetEnvelopeForImage(map, new Coordinate(5, 5), 16, 16);

            Assert.AreEqual(5, envelope.Centre.X);
            Assert.AreEqual(5, envelope.Centre.Y);
            Assert.AreEqual(2, envelope.Width);
            Assert.AreEqual(2, envelope.Height);
        }
    }
}
