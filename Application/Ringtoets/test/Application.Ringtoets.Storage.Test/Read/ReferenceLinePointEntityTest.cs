using System;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class ReferenceLinePointEntityTest
    {
        [Test]
        public void Read_Always_NewPoint()
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            var entity = new ReferenceLinePointEntity
            {
                X = Convert.ToDecimal(x),
                Y = Convert.ToDecimal(y)
            };

            // Call
            var point = entity.Read();

            // Assert
            Assert.AreEqual(x, point.X, 1e-6);
            Assert.AreEqual(y, point.Y, 1e-6);
        }    
    }
}