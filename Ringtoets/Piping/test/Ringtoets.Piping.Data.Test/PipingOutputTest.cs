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
using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingOutputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
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

            var output = new PipingOutput(zuValue,
                                          foSuValue,
                                          zhValue,
                                          foShValue,
                                          zsValue,
                                          foSsValue,
                                          heaveGradient,
                                          sellmeijerCreepCoefficient,
                                          sellmeijerCriticalFall,
                                          sellmeijerReducedFall);

            Assert.IsInstanceOf<Observable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);

            Assert.AreEqual(zuValue, output.UpliftZValue);
            Assert.AreEqual(foSuValue, output.UpliftFactorOfSafety);
            Assert.AreEqual(zhValue, output.HeaveZValue);
            Assert.AreEqual(foShValue, output.HeaveFactorOfSafety);
            Assert.AreEqual(zsValue, output.SellmeijerZValue);
            Assert.AreEqual(foSsValue, output.SellmeijerFactorOfSafety);
            Assert.AreEqual(heaveGradient, output.HeaveGradient);
            Assert.AreEqual(sellmeijerCreepCoefficient, output.SellmeijerCreepCoefficient);
            Assert.AreEqual(sellmeijerCriticalFall, output.SellmeijerCriticalFall);
            Assert.AreEqual(sellmeijerReducedFall, output.SellmeijerReducedFall);
        }
    }
}