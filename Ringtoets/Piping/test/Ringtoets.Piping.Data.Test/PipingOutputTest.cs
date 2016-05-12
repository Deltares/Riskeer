﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

            var output = new PipingOutput(zuValue, foSuValue, zhValue, foShValue, zsValue, foSsValue);

            Assert.IsInstanceOf<Observable>(output);
            Assert.IsInstanceOf<ICalculationOutput>(output);
            Assert.AreEqual(output.UpliftZValue, zuValue);
            Assert.AreEqual(output.UpliftFactorOfSafety, foSuValue);
            Assert.AreEqual(output.HeaveZValue, zhValue);
            Assert.AreEqual(output.HeaveFactorOfSafety, foShValue);
            Assert.AreEqual(output.SellmeijerZValue, zsValue);
            Assert.AreEqual(output.SellmeijerFactorOfSafety, foSsValue);
        }
    }
}