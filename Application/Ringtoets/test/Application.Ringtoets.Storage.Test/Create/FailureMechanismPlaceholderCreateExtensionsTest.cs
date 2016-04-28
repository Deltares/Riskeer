using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class FailureMechanismPlaceholderCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var failureMechanism = new FailureMechanismPlaceholder("name");

            // Call
            TestDelegate test = () => failureMechanism.Create(FailureMechanismType.DuneErosion, null);

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
            var failureMechanism = new FailureMechanismPlaceholder("name")
            {
                IsRelevant = isRelevant
            };
            var collector = new CreateConversionCollector();
            var failureMechanismType = FailureMechanismType.DuneErosion;

            // Call
            var entity = failureMechanism.Create(failureMechanismType, collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual((short)failureMechanismType, entity.FailureMechanismType);
            Assert.AreEqual(Convert.ToByte(isRelevant), entity.IsRelevant);
            Assert.IsEmpty(entity.StochasticSoilModelEntities);
        }
    }
}