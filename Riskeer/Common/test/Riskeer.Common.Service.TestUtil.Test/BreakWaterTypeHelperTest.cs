// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;

namespace Riskeer.Common.Service.TestUtil.Test
{
    [TestFixture]
    public class BreakWaterTypeHelperTest
    {
        [Test]
        [TestCase((BreakWaterType) 0, 0)]
        [TestCase(BreakWaterType.Caisson, 1)]
        [TestCase(BreakWaterType.Wall, 2)]
        [TestCase(BreakWaterType.Dam, 3)]
        [TestCase((BreakWaterType) 99, 0)]
        public void GetHydraRingBreakWaterType(BreakWaterType breakWaterType, int expectedBreakWaterType)
        {
            // Call
            int actualBreakWaterType = BreakWaterTypeHelper.GetHydraRingBreakWaterType(breakWaterType);

            // Assert
            Assert.AreEqual(expectedBreakWaterType, actualBreakWaterType);
        }
    }
}