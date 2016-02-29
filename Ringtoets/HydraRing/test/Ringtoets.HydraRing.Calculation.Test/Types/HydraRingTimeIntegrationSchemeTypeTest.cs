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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.HydraRing.Calculation.Test.Types
{
    [TestFixture]
    public class HydraRingTimeIntegrationSchemeTypeTest
    {
        [Test]
        public void Values_HasThree()
        {
            Assert.AreEqual(3, Enum.GetValues(typeof(HydraRingTimeIntegrationSchemeType)).Length);
        }

        [Test]
        public void ConvertToInteger_ForAllValues_ReturnsExpectedInteger()
        {
            Assert.AreEqual(1, (int) HydraRingTimeIntegrationSchemeType.FBC);
            Assert.AreEqual(2, (int) HydraRingTimeIntegrationSchemeType.APT);
            Assert.AreEqual(3, (int) HydraRingTimeIntegrationSchemeType.NTI);
        }
    }
}
