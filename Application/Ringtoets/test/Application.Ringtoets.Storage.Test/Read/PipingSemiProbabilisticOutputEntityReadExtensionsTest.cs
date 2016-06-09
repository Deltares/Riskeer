using System;

using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read;

using Core.Common.Base.Data;

using NUnit.Framework;

using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Read
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
                HeaveFactorOfSafety = 1.1m,
                HeaveProbability = 2.2m,
                HeaveReliability = 3.3m,
                PipingFactorOfSafety = 4.4m,
                PipingProbability = 5.5m,
                PipingReliability = 6.6m,
                UpliftFactorOfSafety = 7.7m,
                UpliftProbability = 8.8m,
                UpliftReliability = 9.9m,
                SellmeijerFactorOfSafety = 10.10m,
                SellmeijerProbability = 11.11m,
                SellmeijerReliability = 12.12m,
                RequiredProbability = 13.13m,
                RequiredReliability = 14.14m
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
            AssertAreEqual(entity.UpliftReliability, pipingSemiProbabilisticOutput.UpliftReliability);
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
                UpliftReliability = null,
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
            Assert.IsNaN(pipingSemiProbabilisticOutput.UpliftReliability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.SellmeijerFactorOfSafety);
            Assert.IsNaN(pipingSemiProbabilisticOutput.SellmeijerProbability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.SellmeijerReliability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.RequiredProbability);
            Assert.IsNaN(pipingSemiProbabilisticOutput.RequiredReliability);
        }

        private static void AssertAreEqual(decimal? expectedParamterValue, RoundedDouble actualParameterValue)
        {
            Assert.AreEqual(Convert.ToDouble(expectedParamterValue), actualParameterValue, actualParameterValue.GetAccuracy());
        }
    }
}