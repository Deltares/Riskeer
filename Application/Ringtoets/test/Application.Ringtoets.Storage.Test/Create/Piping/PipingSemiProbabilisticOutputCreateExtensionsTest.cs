// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Test.Create.Piping
{
    [TestFixture]
    public class PipingSemiProbabilisticOutputCreateExtensionsTest
    {
        [Test]
        public void Create_WithValidValues_ReturnEntity()
        {
            // Setup
            var output = new PipingSemiProbabilisticOutput(1.1, 2.2, 0.3, 4.4, 5.5, 0.6, 7.7,
                                                           8.8, 0.9, 0.10, 11.11, 0.12,
                                                           13.13, 14.14);
            // Call
            PipingSemiProbabilisticOutputEntity entity = output.Create();

            // Assert
            AssertAreEqual(output.UpliftFactorOfSafety, entity.UpliftFactorOfSafety);
            AssertAreEqual(output.UpliftReliability, entity.UpliftReliability);
            AssertAreEqual(output.UpliftProbability, entity.UpliftProbability);
            AssertAreEqual(output.HeaveFactorOfSafety, entity.HeaveFactorOfSafety);
            AssertAreEqual(output.HeaveReliability, entity.HeaveReliability);
            AssertAreEqual(output.HeaveProbability, entity.HeaveProbability);
            AssertAreEqual(output.SellmeijerFactorOfSafety, entity.SellmeijerFactorOfSafety);
            AssertAreEqual(output.SellmeijerReliability, entity.SellmeijerReliability);
            AssertAreEqual(output.SellmeijerProbability, entity.SellmeijerProbability);
            AssertAreEqual(output.RequiredProbability, entity.RequiredProbability);
            AssertAreEqual(output.RequiredReliability, entity.RequiredReliability);
            AssertAreEqual(output.PipingProbability, entity.PipingProbability);
            AssertAreEqual(output.PipingReliability, entity.PipingReliability);
            AssertAreEqual(output.PipingFactorOfSafety, entity.PipingFactorOfSafety);

            Assert.AreEqual(0, entity.PipingSemiProbabilisticOutputEntityId);
            Assert.AreEqual(0, entity.PipingCalculationEntityId);
        }

        [Test]
        public void Create_WithNaNValues_ReturnEntityWithNullPropertyValues()
        {
            // Setup
            var output = new PipingSemiProbabilisticOutput(double.NaN, double.NaN, double.NaN,
                                                           double.NaN, double.NaN, double.NaN,
                                                           double.NaN, double.NaN, double.NaN,
                                                           double.NaN, double.NaN, double.NaN,
                                                           double.NaN, double.NaN);
            // Call
            PipingSemiProbabilisticOutputEntity entity = output.Create();

            // Assert
            Assert.IsNull(entity.UpliftFactorOfSafety);
            Assert.IsNull(entity.UpliftReliability);
            Assert.IsNull(entity.UpliftProbability);
            Assert.IsNull(entity.HeaveFactorOfSafety);
            Assert.IsNull(entity.HeaveReliability);
            Assert.IsNull(entity.HeaveProbability);
            Assert.IsNull(entity.SellmeijerFactorOfSafety);
            Assert.IsNull(entity.SellmeijerReliability);
            Assert.IsNull(entity.SellmeijerProbability);
            Assert.IsNull(entity.RequiredProbability);
            Assert.IsNull(entity.RequiredReliability);
            Assert.IsNull(entity.PipingProbability);
            Assert.IsNull(entity.PipingReliability);
            Assert.IsNull(entity.PipingFactorOfSafety);

            Assert.AreEqual(0, entity.PipingSemiProbabilisticOutputEntityId);
            Assert.AreEqual(0, entity.PipingCalculationEntityId);
        }

        private static void AssertAreEqual(double expectedValue, double? actualValue)
        {
            if (double.IsNaN(expectedValue))
            {
                Assert.IsNull(actualValue);
            }
            Assert.AreEqual(expectedValue, Convert.ToDouble(actualValue));
        }

        private static void AssertAreEqual(RoundedDouble expectedValue, double? actualValue)
        {
            if (double.IsNaN(expectedValue))
            {
                Assert.IsNull(actualValue);
            }
            Assert.AreEqual(expectedValue, Convert.ToDouble(actualValue), expectedValue.GetAccuracy());
        }
    }
}