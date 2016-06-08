using System;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;

using NUnit.Framework;

using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PipingOutputCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryIsNull_ThrowArgumentNullException()
        {
            // Setup
            var pipingOutput = new PipingOutput(1.1, 2.2, 3.3, 4.4, 5.5, 6.6);

            // Call
            TestDelegate call = () => pipingOutput.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_AllOutputValuesSet_ReturnEntity()
        {
            // Setup
            var pipingOutput = new PipingOutput(1.1, 2.2, 3.3, 4.4, 5.5, 6.6);

            var registry = new PersistenceRegistry();

            // Call
            PipingCalculationOutputEntity entity = pipingOutput.Create(registry);

            // Assert
            Assert.AreEqual(pipingOutput.HeaveFactorOfSafety, entity.HeaveFactorOfSafety);
            Assert.AreEqual(pipingOutput.HeaveZValue, entity.HeaveZValue);
            Assert.AreEqual(pipingOutput.SellmeijerFactorOfSafety, entity.SellmeijerFactorOfSafety);
            Assert.AreEqual(pipingOutput.SellmeijerZValue, entity.SellmeijerZValue);
            Assert.AreEqual(pipingOutput.UpliftFactorOfSafety, entity.UpliftFactorOfSafety);
            Assert.AreEqual(pipingOutput.UpliftZValue, entity.UpliftZValue);

            Assert.AreEqual(0, entity.PipingCalculationOutputEntityId);
            Assert.IsNull(entity.PipingCalculationEntity);
        }

        [Test]
        public void Create_AllOutputValuesNaN_ReturnEntityWithNullValues()
        {
            // Setup
            var pipingOutput = new PipingOutput(double.NaN, double.NaN, double.NaN,
                                                double.NaN, double.NaN, double.NaN);

            var registry = new PersistenceRegistry();

            // Call
            PipingCalculationOutputEntity entity = pipingOutput.Create(registry);

            // Assert
            Assert.IsNull(entity.HeaveFactorOfSafety);
            Assert.IsNull(entity.HeaveZValue);
            Assert.IsNull(entity.SellmeijerFactorOfSafety);
            Assert.IsNull(entity.SellmeijerZValue);
            Assert.IsNull(entity.UpliftFactorOfSafety);
            Assert.IsNull(entity.UpliftZValue);

            Assert.AreEqual(0, entity.PipingCalculationOutputEntityId);
            Assert.IsNull(entity.PipingCalculationEntity);
        }
    }
}