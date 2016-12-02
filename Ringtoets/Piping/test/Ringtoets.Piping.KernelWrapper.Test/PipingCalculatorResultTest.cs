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
using NUnit.Framework;

namespace Ringtoets.Piping.KernelWrapper.Test
{
    [TestFixture]
    public class PipingCalculatorResultTest
    {
        [Test]
        public void GivenSomeValues_WhenConstructedWithValues_ThenPropertiesAreSet()
        {
            var random = new Random(22);
            var zuValue = random.NextDouble();
            var foSuValue = random.NextDouble();
            var zhValue = random.NextDouble();
            var foShValue = random.NextDouble();
            var zsValue = random.NextDouble();
            var foSsValue = random.NextDouble();
            var heaveGradient = random.NextDouble();
            var sellmeijerCreepCoefficient = random.NextDouble();
            var sellmeijerCriticalFall = random.NextDouble();
            var sellmeijerReducedFall = random.NextDouble();

            var actual = new PipingCalculatorResult(
                zuValue,
                foSuValue,
                zhValue,
                foShValue,
                zsValue,
                foSsValue,
                heaveGradient,
                sellmeijerCreepCoefficient,
                sellmeijerCriticalFall,
                sellmeijerReducedFall);

            Assert.AreEqual(zuValue, actual.UpliftZValue);
            Assert.AreEqual(foSuValue, actual.UpliftFactorOfSafety);
            Assert.AreEqual(zhValue, actual.HeaveZValue);
            Assert.AreEqual(foShValue, actual.HeaveFactorOfSafety);
            Assert.AreEqual(zsValue, actual.SellmeijerZValue);
            Assert.AreEqual(foSsValue, actual.SellmeijerFactorOfSafety);
            Assert.AreEqual(heaveGradient, actual.HeaveGradient);
            Assert.AreEqual(sellmeijerCreepCoefficient, actual.SellmeijerCreepCoefficient);
            Assert.AreEqual(sellmeijerCriticalFall, actual.SellmeijerCriticalFall);
            Assert.AreEqual(sellmeijerReducedFall, actual.SellmeijerReducedFall);
        }
    }
}