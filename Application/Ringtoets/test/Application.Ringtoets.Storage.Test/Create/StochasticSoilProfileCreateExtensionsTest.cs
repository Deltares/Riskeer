using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    public class StochasticSoilProfileCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var stochasticSoilProfile = new StochasticSoilProfile(40, SoilProfileType.SoilProfile1D, -1);

            // Call
            TestDelegate test = () => stochasticSoilProfile.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsStochasticSoilProfileEntityWithPropertiesSet()
        {
            // Setup
            var probability = new Random(21).NextDouble();
            var stochasticSoilProfile = new StochasticSoilProfile(probability, SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = new TestPipingSoilProfile()
            };
            var collector = new CreateConversionCollector();

            // Call
            var entity = stochasticSoilProfile.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(probability, entity.Probability);
        }   

        [Test]
        public void Create_DifferentStochasticSoilProfilesWithSamePipingSoilProfile_ReturnsStochasticSoilProfileEntityWithSameSoilProfileEntitySet()
        {
            // Setup
            var testPipingSoilProfile = new TestPipingSoilProfile();
            var firstStochasticSoilProfile = new StochasticSoilProfile(new Random(21).NextDouble(), SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = testPipingSoilProfile
            };
            var secondStochasticSoilProfile = new StochasticSoilProfile(new Random(21).NextDouble(), SoilProfileType.SoilProfile1D, -1)
            {
                SoilProfile = testPipingSoilProfile
            };
            var collector = new CreateConversionCollector();

            // Call
            var firstEntity = firstStochasticSoilProfile.Create(collector);
            var secondEntity = secondStochasticSoilProfile.Create(collector);

            // Assert
            Assert.AreSame(firstEntity.SoilProfileEntity, secondEntity.SoilProfileEntity);
        }   
    }
}