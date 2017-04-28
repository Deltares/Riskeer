// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

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
                HeaveFactorOfSafety = 1.1,
                HeaveProbability = 0.2,
                HeaveReliability = 3.3,
                PipingFactorOfSafety = 4.4,
                PipingProbability = 0.5,
                PipingReliability = 6.6,
                UpliftFactorOfSafety = 7.7,
                UpliftProbability = 0.8,
                UpliftReliability = 9.9,
                SellmeijerFactorOfSafety = 10.10,
                SellmeijerProbability = 0.11,
                SellmeijerReliability = 12.12,
                RequiredProbability = 0.13,
                RequiredReliability = 14.14
            };

            // Call
            PipingSemiProbabilisticOutput pipingSemiProbabilisticOutput = entity.Read();

            // Assert
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