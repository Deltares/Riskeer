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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators.Waternet.Output
{
    [TestFixture]
    public class WaternetCalculatorResultTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnWaternetCalculatorResult()
        {
            // Call
            WaternetCalculatorResult result = WaternetCalculatorResultTestFactory.Create();

            // Assert
            WaternetPhreaticLineResult[] phreaticLines = result.PhreaticLines.ToArray();
            WaternetLineResult[] waternetLines = result.WaternetLines.ToArray();
            Assert.AreEqual(1, phreaticLines.Length);
            Assert.AreEqual(1, waternetLines.Length);

            WaternetPhreaticLineResult phreaticLine = phreaticLines[0];

            Assert.AreEqual("Line 1", phreaticLine.Name);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(10, 0)
            }, phreaticLine.Geometry);

            WaternetLineResult waternetLine = waternetLines[0];
            Assert.AreEqual("Line 2", waternetLine.Name);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(2, 2),
                new Point2D(3, 3)
            }, waternetLine.Geometry);
            Assert.AreSame(phreaticLine, waternetLine.PhreaticLine);
        }

        [Test]
        public void CreateEmptyResult_Always_ReturnEmptyCalculatorResult()
        {
            // Call
            WaternetCalculatorResult result = WaternetCalculatorResultTestFactory.CreateEmptyResult();

            // Assert
            CollectionAssert.AreEqual(new List<WaternetPhreaticLineResult>(), result.PhreaticLines);
            CollectionAssert.AreEqual(new List<WaternetLineResult>(), result.WaternetLines);
        }
    }
}