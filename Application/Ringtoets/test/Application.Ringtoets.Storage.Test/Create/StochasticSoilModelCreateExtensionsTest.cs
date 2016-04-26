using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class StochasticSoilModelCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilModel = new TestStochasticSoilModel();

            // Call
            TestDelegate test = () => stochasticSoilModel.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsStochasticSoilModelEntityWithPropertiesSet()
        {
            // Setup
            string testName = "testName";
            string testSegmentName = "testSegmentName";
            var stochasticSoilModel = new StochasticSoilModel(-1, testName, testSegmentName);
            var collector = new CreateConversionCollector();

            // Call
            var entity = stochasticSoilModel.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(testSegmentName, entity.SegmentName);
            Assert.IsEmpty(entity.StochasticSoilProfileEntities);
        }   

        [Test]
        public void Create_WithStochasticSoilProfiles_ReturnsStochasticSoilModelEntityWithPropertiesAndStochasticSoilProfileEntitiesSet()
        {
            // Setup
            var stochasticSoilModel = new StochasticSoilModel(-1, "testName", "testSegmentName");
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(50, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            });
            stochasticSoilModel.StochasticSoilProfiles.Add(new StochasticSoilProfile(50, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            });
            var collector = new CreateConversionCollector();

            // Call
            var entity = stochasticSoilModel.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.StochasticSoilProfileEntities.Count);
        }   
    }
}