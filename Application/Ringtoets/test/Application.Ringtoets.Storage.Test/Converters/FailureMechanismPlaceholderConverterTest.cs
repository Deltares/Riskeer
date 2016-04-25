using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;

using NUnit.Framework;

using Ringtoets.Integration.Data.Placeholders;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class FailureMechanismPlaceholderConverterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new FailureMechanismPlaceholderConverter(FailureMechanismType.StrengthAndStabilityParallelConstruction);

            // Assert
            Assert.IsInstanceOf<FailureMechanismConverterBase<FailureMechanismPlaceholder>>(converter);
        }

        [Test]
        [TestCase(984753874, true)]
        [TestCase(123, false)]
        public void ConvertEntityToModel_ProperEntity_ReturnNewlyCreatedFailureMechanism(
            long id, bool isRelevant)
        {
            // Setup
            var type = FailureMechanismType.StructureHeight;

            var entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short)type,
                FailureMechanismEntityId = id,
                IsRelevant = isRelevant ? (byte)1 : (byte)0
            };

            var converter = new FailureMechanismPlaceholderConverter(type);

            // Call
            FailureMechanismPlaceholder failureMechanism = converter.ConvertEntityToModel(entity);

            // Assert
            Assert.AreEqual(id, failureMechanism.StorageId);
            Assert.AreEqual(isRelevant, failureMechanism.IsRelevant);
        }

        [Test]
        [TestCase(98546309845, FailureMechanismType.GrassRevetmentSlidingOutwards, true)]
        [TestCase(1, FailureMechanismType.StabilityStoneRevetment, false)]
        public void ConvertModelToEntity_InitializedFailureMechanism_ChangeValuesForEntity(
            long id, FailureMechanismType type, bool isRelevant)
        {
            // Setup

            var converter = new FailureMechanismPlaceholderConverter(type);

            var failureMechanism = new FailureMechanismPlaceholder("A")
            {
                StorageId = id,
                IsRelevant = isRelevant
            };

            var entity = new FailureMechanismEntity();

            // Call
            converter.ConvertModelToEntity(failureMechanism, entity);

            // Assert
            Assert.AreEqual(id, entity.FailureMechanismEntityId);
            Assert.AreEqual((short)type, entity.FailureMechanismType);
            var expectedIsRelevantValue = isRelevant ?
                                              (byte)1 :
                                              (byte)0;
            Assert.AreEqual(expectedIsRelevantValue, entity.IsRelevant);
        }
    }
}