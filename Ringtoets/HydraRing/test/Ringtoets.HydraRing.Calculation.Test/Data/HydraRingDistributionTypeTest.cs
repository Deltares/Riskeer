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
using Riskeer.HydraRing.Calculation.Data;

namespace Riskeer.HydraRing.Calculation.Test.Data
{
    [TestFixture]
    public class HydraRingDistributionTypeTest
    {
        [Test]
        public void Values_HasFive()
        {
            Assert.AreEqual(5, Enum.GetValues(typeof(HydraRingDistributionType)).Length);
        }

        [Test]
        public void ConvertToInteger_ForAllValues_ReturnsExpectedInteger()
        {
            Assert.AreEqual(0, (int) HydraRingDistributionType.Deterministic);
            Assert.AreEqual(2, (int) HydraRingDistributionType.Normal);
            Assert.AreEqual(4, (int) HydraRingDistributionType.LogNormal);
            Assert.AreEqual(18, (int) HydraRingDistributionType.RayleighN);
            Assert.AreEqual(19, (int) HydraRingDistributionType.TruncatedNormal);
        }
    }
}