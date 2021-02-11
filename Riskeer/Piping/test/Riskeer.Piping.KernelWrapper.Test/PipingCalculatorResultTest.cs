// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

namespace Riskeer.Piping.KernelWrapper.Test
{
    [TestFixture]
    public class PipingCalculatorResultTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperties_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new PipingCalculatorResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("constructionProperties", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithoutValuesSet_PropertiesAreDefault()
        {
            // Call
            var pipingCalculatorResult = new PipingCalculatorResult(new PipingCalculatorResult.ConstructionProperties());

            // Assert
            Assert.IsNaN(pipingCalculatorResult.UpliftFactorOfSafety);
            Assert.IsNaN(pipingCalculatorResult.HeaveFactorOfSafety);
            Assert.IsNaN(pipingCalculatorResult.SellmeijerFactorOfSafety);
            Assert.IsNaN(pipingCalculatorResult.UpliftEffectiveStress);
            Assert.IsNaN(pipingCalculatorResult.HeaveGradient);
            Assert.IsNaN(pipingCalculatorResult.SellmeijerCreepCoefficient);
            Assert.IsNaN(pipingCalculatorResult.SellmeijerCriticalFall);
            Assert.IsNaN(pipingCalculatorResult.SellmeijerReducedFall);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValuesSet_PropertiesAreSet()
        {
            // Setup
            var random = new Random(22);
            double foSuValue = random.NextDouble();
            double foShValue = random.NextDouble();
            double foSsValue = random.NextDouble();
            double heaveGradient = random.NextDouble();
            double upliftEffectiveStress = random.NextDouble();
            double sellmeijerCreepCoefficient = random.NextDouble();
            double sellmeijerCriticalFall = random.NextDouble();
            double sellmeijerReducedFall = random.NextDouble();

            var constructionProperties = new PipingCalculatorResult.ConstructionProperties
            {
                UpliftFactorOfSafety = foSuValue,
                HeaveFactorOfSafety = foShValue,
                SellmeijerFactorOfSafety = foSsValue,
                UpliftEffectiveStress = upliftEffectiveStress,
                HeaveGradient = heaveGradient,
                SellmeijerCreepCoefficient = sellmeijerCreepCoefficient,
                SellmeijerCriticalFall = sellmeijerCriticalFall,
                SellmeijerReducedFall = sellmeijerReducedFall
            };

            // Call
            var pipingCalculatorResult = new PipingCalculatorResult(constructionProperties);

            // Assert
            Assert.AreEqual(foSuValue, pipingCalculatorResult.UpliftFactorOfSafety);
            Assert.AreEqual(foShValue, pipingCalculatorResult.HeaveFactorOfSafety);
            Assert.AreEqual(foSsValue, pipingCalculatorResult.SellmeijerFactorOfSafety);
            Assert.AreEqual(upliftEffectiveStress, pipingCalculatorResult.UpliftEffectiveStress);
            Assert.AreEqual(heaveGradient, pipingCalculatorResult.HeaveGradient);
            Assert.AreEqual(sellmeijerCreepCoefficient, pipingCalculatorResult.SellmeijerCreepCoefficient);
            Assert.AreEqual(sellmeijerCriticalFall, pipingCalculatorResult.SellmeijerCriticalFall);
            Assert.AreEqual(sellmeijerReducedFall, pipingCalculatorResult.SellmeijerReducedFall);
        }
    }
}