﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.HydraRing.Calculation.Test.Data
{
    [TestFixture]
    public class HydraRingProfilePointTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var hydraRingProfilePoint = new HydraRingProfilePoint(1.1, 2.2);

            // Assert
            Assert.AreEqual(1.1, hydraRingProfilePoint.X);
            Assert.AreEqual(2.2, hydraRingProfilePoint.Z);
            Assert.AreEqual(1.0, hydraRingProfilePoint.Roughness);
        }
    }
}