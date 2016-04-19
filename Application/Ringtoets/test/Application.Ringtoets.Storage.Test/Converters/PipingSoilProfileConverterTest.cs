using System;
using System.Linq;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Converters
{
    public class PipingSoilProfileConverterTest
    {
        [Test]
        public void Constructor_Always_NewInstance()
        {
            // Call
            var converter = new PipingSoilProfileConverter();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<PipingSoilProfile, SoilProfileEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new PipingSoilProfileConverter();

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
            var name = "SomeName";
            var entity = new SoilProfileEntity()
            {
                SoilProfileEntityId = storageId,
                Name = name
            };
            entity.SoilLayerEntities.Add(new SoilLayerEntity());
            var converter = new PipingSoilProfileConverter();

            // Call
            var location = converter.ConvertEntityToModel(entity);

            // Assert
            Assert.AreEqual(storageId, location.StorageId);
            Assert.AreEqual(name, location.Name);
        }

        [Test]
        public void ConvertModelToEntity_NullModel_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new PipingSoilProfileConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(null, new SoilProfileEntity());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("modelObject", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_NullEntity_ThrowsArgumentNullException()
        {
            // Setup
            var converter = new PipingSoilProfileConverter();

            // Call
            TestDelegate test = () => converter.ConvertModelToEntity(new TestPipingSoilProfile(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void ConvertModelToEntity_ValidModelValidEntity_ReturnsModelAsEntity()
        {
            // Setup
            var converter = new PipingSoilProfileConverter();
            var random = new Random(21);
            var entity = new SoilProfileEntity();

            long storageId = random.Next();
            var model = new TestPipingSoilProfile
            {
                StorageId = storageId
            };

            // Call
            converter.ConvertModelToEntity(model, entity);

            // Assert
            Assert.AreEqual(storageId, entity.SoilProfileEntityId);
            Assert.AreEqual(model.Name, entity.Name);
            Assert.AreEqual(model.Bottom, entity.Bottom);
            Assert.AreEqual(1, entity.SoilLayerEntities.Count);

            var layer = entity.SoilLayerEntities.ElementAt(0);
            Assert.AreEqual(0, layer.Top);
            Assert.AreEqual(1, layer.IsAquifer);
        }
    }
}