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

namespace Riskeer.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class PipingOutputTestFactoryTest
    {
        [Test]
        public void Create_WithoutParameters_ReturnOutput()
        {
            // Call
            PipingOutput output = PipingOutputTestFactory.Create();

            // Assert
            Assert.IsTrue(IsValidDouble(output.HeaveZValue));
            Assert.IsTrue(IsValidDouble(output.HeaveFactorOfSafety));
            Assert.IsTrue(IsValidDouble(output.UpliftEffectiveStress));
            Assert.IsTrue(IsValidDouble(output.UpliftZValue));
            Assert.IsTrue(IsValidDouble(output.UpliftFactorOfSafety));
            Assert.IsTrue(IsValidDouble(output.SellmeijerZValue));
            Assert.IsTrue(IsValidDouble(output.SellmeijerFactorOfSafety));
            Assert.IsTrue(IsValidDouble(output.HeaveGradient));
            Assert.IsTrue(IsValidDouble(output.SellmeijerCreepCoefficient));
            Assert.IsTrue(IsValidDouble(output.SellmeijerCriticalFall));
            Assert.IsTrue(IsValidDouble(output.SellmeijerReducedFall));
        }

        [Test]
        public void Create_WithParameters_ReturnOutput()
        {
            // Setup
            var random = new Random(39);
            double heaveFactorOfSafety = random.NextDouble();
            double upliftFactorOfSafety = random.NextDouble();
            double sellmeijerFactorOfSafety = random.NextDouble();

            // Call
            PipingOutput output = PipingOutputTestFactory.Create(heaveFactorOfSafety, upliftFactorOfSafety, sellmeijerFactorOfSafety);

            // Assert
            Assert.AreEqual(heaveFactorOfSafety, output.HeaveFactorOfSafety);
            Assert.AreEqual(upliftFactorOfSafety, output.UpliftFactorOfSafety);
            Assert.AreEqual(sellmeijerFactorOfSafety, output.SellmeijerFactorOfSafety);
        }

        private static bool IsValidDouble(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}