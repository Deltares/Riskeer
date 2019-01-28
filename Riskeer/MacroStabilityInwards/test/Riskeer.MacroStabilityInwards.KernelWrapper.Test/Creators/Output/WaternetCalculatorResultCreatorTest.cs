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
using System.Linq;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators.Output
{
    [TestFixture]
    public class WaternetCalculatorResultCreatorTest
    {
        [Test]
        public void Create_WaternetNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => WaternetCalculatorResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waternet", exception.ParamName);
        }

        [Test]
        public void Create_WaternetWithPhreaticLineAndHeadLinesAndWaternetLines_ReturnWaternetCalculatorResult()
        {
            // Setup
            var headLine = new HeadLine
            {
                Name = "line 1",
                Points =
                {
                    new GeometryPoint(0, 0),
                    new GeometryPoint(1, 1)
                }
            };
            var phreaticLine = new PhreaticLine
            {
                Name = "line 2",
                Points =
                {
                    new GeometryPoint(2, 2),
                    new GeometryPoint(3, 3)
                }
            };
            var waternetLine = new WaternetLine
            {
                Name = "line 3",
                Points =
                {
                    new GeometryPoint(4, 4),
                    new GeometryPoint(5, 5)
                },
                HeadLine = headLine
            };

            var waternet = new Waternet
            {
                HeadLineList =
                {
                    headLine
                },
                PhreaticLine = phreaticLine,
                WaternetLineList =
                {
                    waternetLine
                }
            };

            // Call
            WaternetCalculatorResult result = WaternetCalculatorResultCreator.Create(waternet);

            // Assert
            WaternetCalculatorOutputAssert.AssertPhreaticLines(new GeometryPointString[]
            {
                phreaticLine,
                headLine
            }, result.PhreaticLines.ToArray());

            WaternetCalculatorOutputAssert.AssertWaternetLines(new[]
            {
                waternetLine
            }, result.WaternetLines.ToArray());
        }
    }
}