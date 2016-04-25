using System;

using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;

using Core.Common.TestUtil;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Test.Converters
{
    [TestFixture]
    public class FailureMechanismConverterBaseTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>();

            // Assert
            Assert.IsInstanceOf<IEntityConverter<IFailureMechanism, FailureMechanismEntity>>(converter);
        }

        [Test]
        public void ConvertEntityToModel_EntityIsNull_ThrowArgumentNullException()
        {
            // Setup
            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>();

            // Call
            TestDelegate call = () => converter.ConvertEntityToModel(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void ConvertEntityToModel_EntityOfIncorrectType_ThrowArgumentException()
        {
            // Setup
            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>
            {
                FailureMechanismType = FailureMechanismType.StrengthAndStabilityParallelConstruction
            };

            var entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short)FailureMechanismType.Piping
            };

            // Call
            TestDelegate call = () => converter.ConvertEntityToModel(entity);

            // Assert
            const string expectedMessage = "Incorrect modelType";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        [TestCase(0, FailureMechanismType.Piping, 1)]
        [TestCase(1, FailureMechanismType.DuneErosion, 5)]
        public void ConvertEntityToModel_ValidEntity_ReturnInitializedFailureMechanism(
            byte isRelevant, FailureMechanismType type, long id)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>
            {
                ConstructedFailureMechanismInstance = failureMechanism,
                FailureMechanismType = type
            };

            var entity = new FailureMechanismEntity
            {
                IsRelevant = isRelevant,
                FailureMechanismType = (short)type,
                FailureMechanismEntityId = id
            };

            // Call
            IFailureMechanism result = converter.ConvertEntityToModel(entity);

            // Assert
            bool expectedIsRelevantValue = isRelevant == 1;
            Assert.AreEqual(expectedIsRelevantValue, result.IsRelevant);
            Assert.AreEqual(id, result.StorageId);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true, FailureMechanismType.MacrostabilityInwards, 123456789)]
        [TestCase(false, FailureMechanismType.GrassRevetmentSlidingOutwards, 986532)]
        public void ConvertModelToEntity_ValidFailureMechanism_ProperlyInitializeEntity(
            bool isRelevant, FailureMechanismType type, long id)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.IsRelevant = isRelevant;
            failureMechanism.StorageId = id;
            mocks.ReplayAll();

            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>
            {
                FailureMechanismType = type
            };

            var entity = new FailureMechanismEntity();

            // Call
            converter.ConvertModelToEntity(failureMechanism, entity);

            // Assert
            byte expectedIsRelevantValue = isRelevant ? (byte)1 : (byte)0;
            Assert.AreEqual(expectedIsRelevantValue, entity.IsRelevant);
            Assert.AreEqual(id, entity.FailureMechanismEntityId);
            Assert.AreEqual((short)type, entity.FailureMechanismType);

            mocks.VerifyAll();
        }

        [Test]
        public void ConvertModelToEntity_FailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>();

            var entity = new FailureMechanismEntity();

            // Call
            TestDelegate call = () => converter.ConvertModelToEntity(null, entity);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void ConvertModelToEntity_EntityIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            mocks.ReplayAll();

            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>();

            // Call
            TestDelegate call = () => converter.ConvertModelToEntity(failureMechanism, null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
            
            mocks.VerifyAll();
        }

        [Test]
        public void CorrespondsToFailureMechanismType_TypeMatches_ReturnTrue()
        {
            // Setup
            var type = FailureMechanismType.DuneErosion;
            var entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short)type
            };

            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>
            {
                FailureMechanismType = type
            };

            // Call
            bool isCorresponding = converter.CorrespondsToFailureMechanismType(entity);

            // Assert
            Assert.IsTrue(isCorresponding);
        }

        [Test]
        public void CorrespondsToFailureMechanismType_TypeDoesNotMatch_ReturnFalse()
        {
            // Setup
            var type1 = FailureMechanismType.GrassRevetmentSlidingInwards;
            var type2 = FailureMechanismType.PipingAtStructure;
            var entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short)type1
            };

            var converter = new SimpleFailureMechanismConverter<IFailureMechanism>
            {
                FailureMechanismType = type2
            };

            // Call
            bool isCorresponding = converter.CorrespondsToFailureMechanismType(entity);

            // Assert
            Assert.IsFalse(isCorresponding);
        }

        private class SimpleFailureMechanismConverter<T> : FailureMechanismConverterBase<T> where T : IFailureMechanism
        {
            protected override T ConstructFailureMechanism()
            {
                return ConstructedFailureMechanismInstance;
            }

            protected override FailureMechanismType GetFailureMechanismType()
            {
                return FailureMechanismType;
            }

            public T ConstructedFailureMechanismInstance { get; set; }
            public FailureMechanismType FailureMechanismType { get; set; }
        }
    }
}