using System;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class SoilLayerEntityTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_Always_NewPoint(bool isAquifer)
        {
            // Setup
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            double top = random.NextDouble();
            var entity = new SoilLayerEntity
            {
                SoilLayerEntityId = entityId,
                Top = Convert.ToDecimal(top),
                IsAquifer = Convert.ToByte(isAquifer)
            };

            // Call
            var layer = entity.Read();

            // Assert
            Assert.IsNotNull(layer);
            Assert.AreEqual(entityId, layer.StorageId);
            Assert.AreEqual(top, layer.Top, 1e-6);
            Assert.AreEqual(isAquifer, layer.IsAquifer);
        }     
    }
}