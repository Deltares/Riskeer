using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class StochasticSoilModelEntityTest
    {
        [Test]
        public void Read_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new StochasticSoilModelEntity();

            // Call
            TestDelegate test = () => entity.Read(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        public void Read_WithCollector_ReturnsNewStochasticSoilModelWithPropertiesSet()
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            string testName = "testName";
            string testSegmentName = "testSegmentName";
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = entityId,
                Name = testName,
                SegmentName = testSegmentName,
            };
            var collector = new ReadConversionCollector();

            // Call
            var model = entity.Read(collector);

            // Assert
            Assert.IsNotNull(model);
            Assert.AreEqual(entityId, model.StorageId);
            Assert.AreEqual(testName, model.Name);
            Assert.AreEqual(testSegmentName, model.SegmentName);
        } 

        [Test]
        public void Read_WithCollectorWithStochasticSoilProfiles_ReturnsNewStochasticSoilModelWithStochasticSoilProfiles()
        {
            // Setup
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilProfileEntities =
                {
                    new StochasticSoilProfileEntity
                    {
                        SoilProfileEntity = new SoilProfileEntity
                        {
                            SoilLayerEntities =
                            {
                                new SoilLayerEntity()
                            }
                        }
                    },
                    new StochasticSoilProfileEntity
                    {
                        SoilProfileEntity = new SoilProfileEntity
                        {
                            SoilLayerEntities =
                            {
                                new SoilLayerEntity()
                            }
                        }
                    }
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var model = entity.Read(collector);

            // Assert
            Assert.AreEqual(2, model.StochasticSoilProfiles.Count);
        } 
    }
}