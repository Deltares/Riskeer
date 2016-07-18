using System;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.DbContext;

using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.GrassCoverErosionInwards
{
    [TestFixture]
    public class GeneralGrassCoverErosionInwardsInputCreateExtensionsTest
    {
        [Test]
        public void Create_PersistenceRegistryNull_ThrowArgumentNullException()
        {
            // Setup
            var input = new GeneralGrassCoverErosionInwardsInput();

            // Call
            TestDelegate call = () => input.Create(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("registry", paramName);
        }

        [Test]
        public void Create_ValidGeneralInput_ReturnEntity()
        {
            // Setup
            var n = new Random(21).Next(0,20);
            var input = new GeneralGrassCoverErosionInwardsInput
            {
                N = n
            };
            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsFailureMechanismMetaEntity entity = input.Create(registry);

            // Assert
            Assert.AreEqual(n, entity.N);
        }

        [Test]
        public void Create_ValidGeneralInput_RegisterEntityToRegistry()
        {
            // Setup
            var input = new GeneralGrassCoverErosionInwardsInput
            {
                N = 1
            };
            var registry = new PersistenceRegistry();

            // Call
            GrassCoverErosionInwardsFailureMechanismMetaEntity entity = input.Create(registry);

            // Assert
            entity.GrassCoverErosionInwardsFailureMechanismMetaEntityId = 23456789;
            registry.TransferIds();
            Assert.AreEqual(entity.GrassCoverErosionInwardsFailureMechanismMetaEntityId, input.StorageId);
        }
    }
}