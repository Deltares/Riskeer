using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class SoilProfileEntityTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new SoilProfileEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithCollector_ReturnsNewPipingSoilProfileWithPropertiesSet(bool isRelevant)
        {
            // Setup
            string testName = "testName";
            var random = new Random(21);
            var entityId = random.Next(1, 502);
            double bottom = random.NextDouble();
            var entity = new SoilProfileEntity
            {
                SoilProfileEntityId = entityId,
                Name = testName,
                Bottom = Convert.ToDecimal(bottom),
                SoilLayerEntities =
                {
                    new SoilLayerEntity{ Top = Convert.ToDecimal(bottom + 0.5) },
                    new SoilLayerEntity{ Top = Convert.ToDecimal(bottom + 1.2) }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.Read(collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(testName, failureMechanism.Name);
            Assert.AreEqual(bottom, failureMechanism.Bottom, 1e-6);
        } 

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Read_WithCollectorWithoutLayers_ThrowsArgumentException(bool isRelevant)
        {
            // Setup
            var entity = new SoilProfileEntity();
            var collector = new ReadConversionCollector();

            // Call
            TestDelegate test = () => entity.Read(collector);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Read_WithCollectorReadTwice_ReturnsSamePipingSoilProfile()
        {
            // Setup
            string testName = "testName";
            double bottom = new Random(21).NextDouble();
            var entity = new SoilProfileEntity
            {
                Name = testName,
                Bottom = Convert.ToDecimal(bottom),
                SoilLayerEntities =
                {
                    new SoilLayerEntity{ Top = Convert.ToDecimal(bottom + 0.5) },
                    new SoilLayerEntity{ Top = Convert.ToDecimal(bottom + 1.2) }
                }
            };
            var collector = new ReadConversionCollector();

            var firstFailureMechanism = entity.Read(collector);

            // Call
            var secondFailureMechanism = entity.Read(collector);

            // Assert
            Assert.AreSame(firstFailureMechanism, secondFailureMechanism);
        }
    }
}