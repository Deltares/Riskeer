using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PipingSoilProfileCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();

            // Call
            TestDelegate test = () => soilProfile.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollectorAndLayers_ReturnsSoilProfileEntityWithPropertiesAndSoilLayerEntitiesSet()
        {
            // Setup
            string testName = "testName";
            double bottom = new Random(21).NextDouble();
            var layers = new []
            {
                new PipingSoilLayer(bottom + 1), 
                new PipingSoilLayer(bottom + 2) 
            };
            var soilProfile = new PipingSoilProfile(testName, bottom, layers, SoilProfileType.SoilProfile1D, -1);
            var collector = new CreateConversionCollector();

            // Call
            var entity = soilProfile.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToDecimal(bottom), entity.Bottom);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(2, entity.SoilLayerEntities.Count);
        }  

        [Test]
        public void Create_ForTheSameEntityTwice_ReturnsSameSoilProfileEntityInstance()
        {
            // Setup
            var soilProfile = new TestPipingSoilProfile();
            var collector = new CreateConversionCollector();

            var firstEntity = soilProfile.Create(collector);

            // Call
            var secondEntity = soilProfile.Create(collector);

            // Assert
            Assert.AreSame(firstEntity, secondEntity);
        }  
    }
}