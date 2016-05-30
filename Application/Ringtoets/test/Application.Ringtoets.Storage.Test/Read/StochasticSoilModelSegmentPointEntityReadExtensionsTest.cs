using System;

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;

using Core.Common.Base.Geometry;

using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class StochasticSoilModelSegmentPointEntityReadExtensionsTest
    {
        [Test]
        [TestCase(1.1, -2.2)]
        [TestCase(-3.3, 4.4)]
        public void Read_ValidEntity_ReturnPoint2D(double x, double y)
        {
            // Setup
            var entity = new StochasticSoilModelSegmentPointEntity
            {
                X = Convert.ToDecimal(x),
                Y = Convert.ToDecimal(y)
            };

            // Call
            Point2D point = entity.Read();

            // Assert
            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
        }
    }
}