// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Deltares.WTIStability;
using NUnit.Framework;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class SlipPlaneUpliftVanTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnSlipPlaneUpliftVan()
        {
            // Call
            SlipPlaneUpliftVan slipPlaneUpliftVan = SlipPlaneUpliftVanTestFactory.Create();

            // Assert
            SlipCircleGrid leftGrid = slipPlaneUpliftVan.SlipPlaneLeftGrid;
            Assert.AreEqual(0.1, leftGrid.GridXLeft);
            Assert.AreEqual(0.2, leftGrid.GridXRight);
            Assert.AreEqual(0.3, leftGrid.GridZTop);
            Assert.AreEqual(0.4, leftGrid.GridZBottom);
            Assert.AreEqual(1, leftGrid.GridXNumber);
            Assert.AreEqual(2, leftGrid.GridZNumber);
            SlipCircleGrid rightGrid = slipPlaneUpliftVan.SlipPlaneRightGrid;
            Assert.AreEqual(0.5, rightGrid.GridXLeft);
            Assert.AreEqual(0.6, rightGrid.GridXRight);
            Assert.AreEqual(0.7, rightGrid.GridZTop);
            Assert.AreEqual(0.8, rightGrid.GridZBottom);
            Assert.AreEqual(3, rightGrid.GridXNumber);
            Assert.AreEqual(4, rightGrid.GridZNumber);
        }
    }
}