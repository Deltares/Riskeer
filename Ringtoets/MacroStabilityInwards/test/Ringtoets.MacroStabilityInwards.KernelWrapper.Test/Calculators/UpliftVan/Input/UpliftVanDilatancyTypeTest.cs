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

using System;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Input
{
    [TestFixture]
    public class UpliftVanDilatancyTypeTest
    {
        [Test]
        public void Values_ExpectedValues()
        {
            // Assert
            Assert.AreEqual(3, Enum.GetValues(typeof(UpliftVanDilatancyType)).Length);
            Assert.AreEqual(1, (int) UpliftVanDilatancyType.Phi);
            Assert.AreEqual(2, (int) UpliftVanDilatancyType.Zero);
            Assert.AreEqual(3, (int) UpliftVanDilatancyType.MinusPhi);
        }
    }
}