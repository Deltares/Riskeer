using System;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test.Read
{
    public class FailureMechanismEntityTest
    {
        [Test]
        public void ReadAsPipingFailureMechanism_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate test = () => entity.ReadAsPipingFailureMechanism(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameter);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ReadAsPipingFailureMechanism_WithCollector_ReturnsNewPipingFailureMechanismWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var entityId = new Random(21).Next(1, 502);
            var entity = new FailureMechanismEntity
            {
                FailureMechanismEntityId = entityId,
                IsRelevant = Convert.ToByte(isRelevant),
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.IsNotNull(failureMechanism);
            Assert.AreEqual(entityId, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
        }

        [Test]
        public void ReadAsPipingFailureMechanism_WithStochasticSoilModelsSet_ReturnsNewPipingFailureMechanismWithStochasticSoilModels()
        {
            // Setup
            var entity = new FailureMechanismEntity
            {
                StochasticSoilModelEntities =
                {
                    new StochasticSoilModelEntity(),
                    new StochasticSoilModelEntity()
                }
            };
            var collector = new ReadConversionCollector();

            // Call
            var failureMechanism = entity.ReadAsPipingFailureMechanism(collector);

            // Assert
            Assert.AreEqual(2, failureMechanism.StochasticSoilModels.Count);
        }   
    }
}