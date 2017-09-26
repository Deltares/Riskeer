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

using System;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service.Converters;

namespace Ringtoets.MacroStabilityInwards.Service.Test.Converters
{
    [TestFixture]
    public class MacroStabilityInwardsSlipPlaneUpliftVanConverterTest
    {
        [Test]
        public void Convert_ResultNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => MacroStabilityInwardsSlipPlaneUpliftVanConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("result", exception.ParamName);
        }

        [Test]
        public void Convert_WithResult_ReturnConvertedSlipPlaneUpliftVan()
        {
            // Setup
            UpliftVanGridResult leftGrid = UpliftVanGridResultTestFactory.Create();
            UpliftVanGridResult rightGrid = UpliftVanGridResultTestFactory.Create();
            var tangentLines = new[]
            {
                3,
                2,
                1.5
            };

            var result = new UpliftVanCalculationGridResult(leftGrid, rightGrid, tangentLines);

            // Call
            MacroStabilityInwardsSlipPlaneUpliftVan output = MacroStabilityInwardsSlipPlaneUpliftVanConverter.Convert(result);

            // Assert
            CollectionAssert.AreEqual(tangentLines, output.TangentLines);
            AssertGrid(leftGrid, output.LeftGrid);
            AssertGrid(rightGrid, output.RightGrid);
        }

        private static void AssertGrid(UpliftVanGridResult expectedGrid, MacroStabilityInwardsGrid actualGrid)
        {
            Assert.AreEqual(expectedGrid.XLeft, actualGrid.XLeft);
            Assert.AreEqual(expectedGrid.XRight, actualGrid.XRight);
            Assert.AreEqual(expectedGrid.ZTop, actualGrid.ZTop);
            Assert.AreEqual(expectedGrid.ZBottom, actualGrid.ZBottom);
            Assert.AreEqual(expectedGrid.NumberOfHorizontalPoints, actualGrid.NumberOfHorizontalPoints);
            Assert.AreEqual(expectedGrid.NumberOfVerticalPoints, actualGrid.NumberOfVerticalPoints);
        }
    }
}