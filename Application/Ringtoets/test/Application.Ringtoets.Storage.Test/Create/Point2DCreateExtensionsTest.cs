using System;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class Point2DCreateExtensionsTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(100)]
        [TestCase(Int32.MaxValue)]
        [TestCase(Int32.MinValue)]
        public void CreateReferenceLinePoint_Always_NewReferenceLinePointEntityWithPropertiesSet(int order)
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            var soilProfile = new Point2D(x, y);

            // Call
            var entity = soilProfile.CreateReferenceLinePoint(order);

            // Assert
            Assert.AreEqual(Convert.ToDecimal(x), entity.X);
            Assert.AreEqual(Convert.ToDecimal(y), entity.Y);
            Assert.AreEqual(order, entity.Order);
        }
    }
}