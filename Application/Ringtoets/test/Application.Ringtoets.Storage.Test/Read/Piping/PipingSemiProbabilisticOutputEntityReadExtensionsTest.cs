using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.Piping;

using Core.Common.Base.Data;

using NUnit.Framework;

using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read.Piping
{
    [TestFixture]
    public class PipingSemiProbabilisticOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityWithValues_ReturnPipingSemiProbabilisticOutput()
        {
            // Setup
            var entity = new PipingSemiProbabilisticOutputEntity
            {
                PipingSemiProbabilisticOutputEntityId = 5867,
                HeaveFactorOfSafety = 1.1,
                HeaveProbability = 0.2,
                HeaveReliability = 3.3,
                PipingFactorOfSafety = 4.4,
                PipingProbability = 0.5,
                PipingReliability = 6.6,
                UpliftFactorOfSafety = 7.7,
                UpliftProbability = 0.8,
                SellmeijerFactorOfSafety = 10.10,
                SellmeijerProbability = 0.11,
                SellmeijerReliability = 12.12,
                RequiredProbability = 0.13,
                RequiredReliability = 14.14
            };

            // Call
            PipingSemiProbabilisticOutput pipingSemiProbabilisticOutput = entity.Read();

            // Assert
            Assert.AreEqual(entity.PipingSemiProbabilisticOutputEntityId, pipingSemiProbabilisticOutput.StorageId);
            AssertAreEqual(entity.HeaveFactorOfSafety, pipingSemiProbabilisticOutput.HeaveFactorOfSafety);
            AssertAreEqual(entity.HeaveProbability, pipingSemiProbabilisticOutput.HeaveProbability);
            AssertAreEqual(entity.HeaveReliability, pipingSemiProbabilisticOutput.HeaveReliability);
            AssertAreEqual(entity.PipingFactorOfSafety, pipingSemiProbabilisticOutput.PipingFactorOfSafety);
            AssertAreEqual(entity.PipingProbability, pipingSemiProbabilisticOutput.PipingProbability);
            AssertAreEqual(entity.PipingReliability, pipingSemiProbabilisticOutput.PipingReliability);
            AssertAreEqual(entity.UpliftFactorOfSafety, pipingSemiProbabilisticOutput.UpliftFactorOfSafety);
            AssertAreEqual(entity.UpliftProbability, pipingSemiProbabilisticOutput.UpliftProbability);
            AssertAreEqual(entity.SellmeijerFactorOfSafety, pipingSemiProbabilisticOutput.SellmeijerFactorOfSafety);
            AssertAreEqual(entity.SellmeijerProbability, pipingSemiProbabilisticOutput.SellmeijerProbability);
            AssertAreEqual(entity.SellmeijerReliability, pipingSemiProbabilisticOutput.SellmeijerReliability);
            AssertAreEqual(entity.RequiredProbability, pipingSemiProbabilisticOutput.RequiredProbability);
            AssertAreEqual(entity.RequiredReliability, pipingSemiProbabilisticOutput.RequiredReliability);
        }

        [Test]
        public void Read_EntityWithNullValues_ReturnPipingSemiProbabilisticOutput()
        {
            // Setup
            var entity = new PipingSemiProbabilisticOutputEntity
            {
                PipingSemiProbabilisticOutputEntityId = 670,
                HeaveFactorOfSafety = null,
                HeaveProbability = null,
                HeaveReliability = null,
                PipingFactorOfSafety = null,
                PipingProbability = null,
                PipingReliability = null,
                UpliftFactorOfSafety = null,
                UpliftProbability = null,
                SellmeijerFactorOfSafety = null,
                SellmeijerProbability = null,
                SellmeijerReliability = null,
                RequiredProbability = null,
                RequiredReliability = null
            };

            // Call
            PipingSemiProbabilisticOutput pipingSemiProbabilisticOutput = entity.Read();

            // Assert
            Assert.AreEqual(entity.PipingSemiProbabilisticOutputEntityId, pipingSemiProbabilisticOutput.StorageId);
            Assert.IsNaN(pipingSemiProbabilisticOutput.HeaveFactorOfSafety);
            Assert.IsNaN(pipingSemiProbabilisticOutput.HeaveProbability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.HeaveReliability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.PipingFactorOfSafety);
            Assert.IsNaN(pipingSemiProbabilisticOutput.PipingProbability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.PipingReliability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.UpliftFactorOfSafety);
            Assert.IsNaN(pipingSemiProbabilisticOutput.UpliftProbability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.SellmeijerFactorOfSafety);
            Assert.IsNaN(pipingSemiProbabilisticOutput.SellmeijerProbability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.SellmeijerReliability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.RequiredProbability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.RequiredReliability);
        }

        private static void AssertAreEqual(double? expectedParamterValue, double actualParameterValue)
        {
            Assert.AreEqual(expectedParamterValue, actualParameterValue);
        }

        private static void AssertAreEqual(double? expectedParamterValue, RoundedDouble actualParameterValue)
        {
            Assert.IsTrue(expectedParamterValue.HasValue);
            Assert.AreEqual(expectedParamterValue.Value, actualParameterValue, actualParameterValue.GetAccuracy());
        }
    }
}