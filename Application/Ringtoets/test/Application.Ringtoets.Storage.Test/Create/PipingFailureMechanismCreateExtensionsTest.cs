using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class PipingFailureMechanismCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate test = () => failureMechanism.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithCollector_ReturnsFailureMechanismEntityWithPropertiesSet(bool isRelevant)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism
            {
                IsRelevant = isRelevant
            };
            var collector = new CreateConversionCollector();

            // Call
            var entity = failureMechanism.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short)FailureMechanismType.Piping, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.IsEmpty(entity.StochasticSoilModelEntities);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithStochasticSoilModels_ReturnsFailureMechanismEntityWithStochasticSoilModelEntities(bool isRelevant)
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.StochasticSoilModels.Add(new StochasticSoilModel(-1, "name", "segmentName"));
            failureMechanism.StochasticSoilModels.Add(new StochasticSoilModel(-1, "name2", "segmentName2"));
            var collector = new CreateConversionCollector();

            // Call
            var entity = failureMechanism.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(2, entity.StochasticSoilModelEntities.Count);
        }
    }
}