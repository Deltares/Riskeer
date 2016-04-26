using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PipingSoilLayerCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var soilLayer = new PipingSoilLayer(new Random(21).NextDouble());

            // Call
            TestDelegate test = () => soilLayer.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollector_ReturnsFailureMechanismEntityWithPropertiesSet(bool isAquifer)
        {
            // Setup
            double top = new Random(21).NextDouble();
            var soilLayer = new PipingSoilLayer(top)
            {
                IsAquifer = isAquifer
            };
            var collector = new CreateConversionCollector();

            // Call
            var entity = soilLayer.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToDecimal(top), entity.Top);
            Assert.AreEqual(Convert.ToByte(isAquifer), entity.IsAquifer);
        } 
    }
}