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

using Core.Common.Base.Data;
using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class BreakWaterTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.1;

            // Call
            BreakWater breakWater = new BreakWater(type, height);

            // Assert
            Assert.AreEqual(type, breakWater.Type);
            Assert.AreEqual(height, breakWater.Height, 1e-6);
            Assert.AreEqual(2, breakWater.Height.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(BreakWaterType.Dam)]
        [TestCase(BreakWaterType.Wall)]
        public void Properties_Type_ReturnsExpectedValue(BreakWaterType newType)
        {
            // Setup
            BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.1;
            BreakWater breakWater = new BreakWater(type, height);

            // Call
            breakWater.Type = newType;

            // Assert
            Assert.AreEqual(newType, breakWater.Type);
        }

        [Test]
        public void Properties_Height_ReturnsExpectedValue()
        {
            // Setup
            BreakWaterType type = BreakWaterType.Caisson;
            const double height = 100.10;
            BreakWater breakWater = new BreakWater(type, height);

            // Call
            breakWater.Height = (RoundedDouble) 10.00;

            // Assert
            Assert.AreEqual(10.0, breakWater.Height.Value);
        }
    }
}