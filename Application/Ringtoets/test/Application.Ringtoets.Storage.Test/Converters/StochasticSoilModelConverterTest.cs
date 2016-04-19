using System;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class StochasticSoilModelConverterTest
    {

        [Test]
        public void Constructor_Always_NewInstance()
        {
            // Call
            var converter = new StochasticSoilModelConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<StochasticSoilModel, StochasticSoilModelEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();

            // Call
            TestDelegate test = () => converter.ConvertEntityToModel(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ConvertEntityToModel_Always_ReturnsTheEntityAsModelWithId()
        {
            // Setup
            var storageId = new Random(21).Next();
            var segmentName = "SomeSegmentName";
            var name = "SomeName";
            var entity = new StochasticSoilModelEntity
            {
                StochasticSoilModelEntityId = storageId,
                Name = name,
                SegmentName = segmentName
            };
            var converter = new StochasticSoilModelConverter();

            // Call
            StochasticSoilModel location = converter.ConvertEntityToModel(entity);

            // Assert
            Assert.AreEqual(storageId, location.StorageId);
            Assert.AreEqual(name, location.Name);
            Assert.AreEqual(segmentName, location.SegmentName);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, new StochasticSoilModelEntity());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("modelObject", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(new StochasticSoilModel(-1, string.Empty, string.Empty), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_ValidModelValidEntity_ReturnsModelAsEntity()
        {
            // Setup
            var converter = new StochasticSoilModelConverter();
            var random = new Random(21);
            var entity = new StochasticSoilModelEntity();

            string segmentName = "someSegmentName";
            string name = "someName";
            long id = random.Next();
            long storageId = random.Next();
            var model = new StochasticSoilModel(id, name, segmentName)
            {
                StorageId = storageId
            };

            // Call
            converter.ConvertModelToEntity(model, entity);

            // Assert
            Assert.AreEqual(storageId, entity.StochasticSoilModelEntityId);
            Assert.AreEqual(name, entity.Name);
            Assert.AreEqual(segmentName, entity.SegmentName);
        }
    }
}