// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Framework;

namespace Ringtoets.Piping.KernelWrapper.Test
{
    [TestFixture]
    public class PipingCalculatorResultTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperties_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingCalculatorResult(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("constructionProperties", paramName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValuesSet_PropertiesAreDefault()
        {
            // Call
            var actual = new PipingCalculatorResult(new PipingCalculatorResult.ConstructionProperties());

            // Assert
            Assert.IsNaN(actual.UpliftZValue);
            Assert.IsNaN(actual.UpliftFactorOfSafety);
            Assert.IsNaN(actual.HeaveZValue);
            Assert.IsNaN(actual.HeaveFactorOfSafety);
            Assert.IsNaN(actual.SellmeijerZValue);
            Assert.IsNaN(actual.SellmeijerFactorOfSafety);
            Assert.IsNaN(actual.UpliftEffectiveStress);
            Assert.IsNaN(actual.HeaveGradient);
            Assert.IsNaN(actual.SellmeijerCreepCoefficient);
            Assert.IsNaN(actual.SellmeijerCriticalFall);
            Assert.IsNaN(actual.SellmeijerReducedFall);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValuesSet_PropertiesAreSet()
        {
            // Setup
            var random = new Random(22);
            double zuValue = random.NextDouble();
            double foSuValue = random.NextDouble();
            double zhValue = random.NextDouble();
            double foShValue = random.NextDouble();
            double zsValue = random.NextDouble();
            double foSsValue = random.NextDouble();
            double heaveGradient = random.NextDouble();
            double upliftEffectiveStress = random.NextDouble();
            double sellmeijerCreepCoefficient = random.NextDouble();
            double sellmeijerCriticalFall = random.NextDouble();
            double sellmeijerReducedFall = random.NextDouble();

            var constructionProperties = new PipingCalculatorResult.ConstructionProperties
            {
                UpliftZValue = zuValue,
                UpliftFactorOfSafety = foSuValue,
                HeaveZValue = zhValue,
                HeaveFactorOfSafety = foShValue,
                SellmeijerZValue = zsValue,
                SellmeijerFactorOfSafety = foSsValue,
                UpliftEffectiveStress = upliftEffectiveStress,
                HeaveGradient = heaveGradient,
                SellmeijerCreepCoefficient = sellmeijerCreepCoefficient,
                SellmeijerCriticalFall = sellmeijerCriticalFall,
                SellmeijerReducedFall = sellmeijerReducedFall
            };

            // Call
            var actual = new PipingCalculatorResult(constructionProperties);

            // Assert
            Assert.AreEqual(zuValue, actual.UpliftZValue);
            Assert.AreEqual(foSuValue, actual.UpliftFactorOfSafety);
            Assert.AreEqual(zhValue, actual.HeaveZValue);
            Assert.AreEqual(foShValue, actual.HeaveFactorOfSafety);
            Assert.AreEqual(zsValue, actual.SellmeijerZValue);
            Assert.AreEqual(foSsValue, actual.SellmeijerFactorOfSafety);
            Assert.AreEqual(upliftEffectiveStress, actual.UpliftEffectiveStress);
            Assert.AreEqual(heaveGradient, actual.HeaveGradient);
            Assert.AreEqual(sellmeijerCreepCoefficient, actual.SellmeijerCreepCoefficient);
            Assert.AreEqual(sellmeijerCriticalFall, actual.SellmeijerCriticalFall);
            Assert.AreEqual(sellmeijerReducedFall, actual.SellmeijerReducedFall);
        }
    }
}