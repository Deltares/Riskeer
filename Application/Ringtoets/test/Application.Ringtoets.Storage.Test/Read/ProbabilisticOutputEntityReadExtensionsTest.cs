using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;

using NUnit.Framework;

using Ringtoets.Common.Data.Probability;

namespace Application.Ringtoets.Storage.Test.Read
{
    [TestFixture]
    public class ProbabilisticOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_ValidEntity_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var entity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 567567,
                Probability = 0.1,
                RequiredProbability = 0.2,
                RequiredReliability = 0.3,
                Reliability = 0.4,
                FactorOfSafety = 0.5
            };

            // Call
            ProbabilityAssessmentOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.ProbabilisticOutputEntityId, output.StorageId);
            Assert.AreEqual(entity.FactorOfSafety, output.FactorOfSafety.Value);
            Assert.AreEqual(entity.Probability, output.Probability);
            Assert.AreEqual(entity.RequiredProbability, output.RequiredProbability);
            Assert.AreEqual(entity.Reliability, output.Reliability.Value);
            Assert.AreEqual(entity.RequiredReliability, output.RequiredReliability.Value);
        }

        [Test]
        public void Read_ValidEntityWithNullValues_ReturnGrassCoverErosionInwardsOutput()
        {
            // Setup
            var entity = new ProbabilisticOutputEntity
            {
                ProbabilisticOutputEntityId = 87345,
                Probability = null,
                RequiredProbability = null,
                RequiredReliability = null,
                Reliability = null,
                FactorOfSafety = null
            };

            // Call
            ProbabilityAssessmentOutput output = entity.Read();

            // Assert
            Assert.AreEqual(entity.ProbabilisticOutputEntityId, output.StorageId);
            Assert.IsNaN(output.FactorOfSafety);
            Assert.IsNaN(output.Probability);
            Assert.IsNaN(output.RequiredProbability);
            Assert.IsNaN(output.Reliability);
            Assert.IsNaN(output.RequiredReliability);
        }
    }
}