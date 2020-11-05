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

using System;
using System.Linq;
using Deltares.MacroStability.CSharpWrapper;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Output;
using CSharpWrapperPoint2D = Deltares.MacroStability.CSharpWrapper.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Output
{
    [TestFixture]
    public class WaternetCalculatorResultCreatorTest
    {
        [Test]
        public void Create_WaternetNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => WaternetCalculatorResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
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
                    new CSharpWrapperPoint2D(0, 0),
                    new CSharpWrapperPoint2D(1, 1)
                }
            };
            var phreaticLine = new HeadLine
            {
                Name = "line 2",
                Points =
                {
                    new CSharpWrapperPoint2D(2, 2),
                    new CSharpWrapperPoint2D(3, 3)
                }
            };
            var referenceLine = new ReferenceLine
            {
                Name = "line 3",
                Points =
                {
                    new CSharpWrapperPoint2D(4, 4),
                    new CSharpWrapperPoint2D(5, 5)
                },
                AssociatedHeadLine = headLine
            };

            var waternet = new Waternet
            {
                HeadLines =
                {
                    headLine
                },
                PhreaticLine = phreaticLine,
                ReferenceLines =
                {
                    referenceLine
                }
            };

            // Call
            WaternetCalculatorResult result = WaternetCalculatorResultCreator.Create(waternet);

            // Assert
            WaternetCalculatorOutputAssert.AssertPhreaticLines(new[]
            {
                phreaticLine,
                headLine
            }, result.PhreaticLines.ToArray());

            WaternetCalculatorOutputAssert.AssertReferenceLines(new[]
            {
                referenceLine
            }, result.WaternetLines.ToArray());
        }
    }
}