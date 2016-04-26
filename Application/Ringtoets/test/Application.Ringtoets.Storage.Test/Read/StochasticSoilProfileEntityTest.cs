using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class StochasticSoilProfileEntityTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StochasticSoilProfileEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewStochasticSoilProfileWithPropertiesSet()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();
            var entityId = random.Next(1, 502);
            var entity = new StochasticSoilProfileEntity
            {
                StochasticSoilProfileEntityId = entityId,
                Probability = Convert.ToDecimal(probability),
                SoilProfileEntity = new SoilProfileEntity
                {
                    SoilLayerEntities =
                    {
                        new SoilLayerEntity()
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var profile = entity.Read(collector);

            // Assert
            Assert.IsNotNull(profile);
            Assert.AreEqual(entityId, profile.StorageId);
            Assert.AreEqual(probability, profile.Probability, 1e-6);
        }

        [Test]
        public void Read_WithCollectorDifferentStochasticSoilProfileEntitiesWithSameSoilProfileEntity_ReturnsStochasticSoilProfilesWithSamePipingSoilProfile()
        {
            // Setup
            double probability = new Random(21).NextDouble();
            var soilProfileEntity = new SoilProfileEntity
            {
                SoilLayerEntities =
                {
                    new SoilLayerEntity()
                }
            };
            var firstEntity = new StochasticSoilProfileEntity
            {
                Probability = Convert.ToDecimal(probability),
                SoilProfileEntity = soilProfileEntity
            };
            var secondEntity = new StochasticSoilProfileEntity
            {
                Probability = 1- Convert.ToDecimal(probability),
                SoilProfileEntity = soilProfileEntity
            };
            var collector = new ReadConversionCollector();

            var firstProfile = firstEntity.Read(collector);

            // Call
            var secondProfile = secondEntity.Read(collector);

            // Assert
            Assert.AreNotSame(firstProfile, secondProfile);
            Assert.AreSame(firstProfile.SoilProfile, secondProfile.SoilProfile);
        }
    }
}