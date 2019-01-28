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

using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.CalculatedInput.TestUtil
{
    /// <summary>
    /// Class for asserting calculator output.
    /// </summary>
    public static class CalculatorOutputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> corresponds to <paramref name="original"/>.
        /// </summary>
        /// <param name="original">The original <see cref="WaternetCalculatorResult"/>.</param>
        /// <param name="actual">The actual <see cref="MacroStabilityInwardsWaternet"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// does not correspond to <paramref name="original"/>.</exception>
        public static void AssertWaternet(WaternetCalculatorResult original, MacroStabilityInwardsWaternet actual)
        {
            WaternetPhreaticLineResult[] originalPhreaticLines = original.PhreaticLines.ToArray();
            WaternetLineResult[] originalWaternetLines = original.WaternetLines.ToArray();
            MacroStabilityInwardsPhreaticLine[] actualPhreaticLines = actual.PhreaticLines.ToArray();
            MacroStabilityInwardsWaternetLine[] actualWaternetLines = actual.WaternetLines.ToArray();

            Assert.AreEqual(originalPhreaticLines.Length, actualPhreaticLines.Length);
            Assert.AreEqual(originalWaternetLines.Length, actualWaternetLines.Length);

            for (var i = 0; i < originalPhreaticLines.Length; i++)
            {
                Assert.AreEqual(originalPhreaticLines[i].Name, actualPhreaticLines[i].Name);
                CollectionAssert.AreEqual(originalPhreaticLines[i].Geometry, actualPhreaticLines[i].Geometry);
            }

            for (var i = 0; i < originalWaternetLines.Length; i++)
            {
                Assert.AreEqual(originalWaternetLines[i].Name, actualWaternetLines[i].Name);
                CollectionAssert.AreEqual(originalWaternetLines[i].Geometry, actualWaternetLines[i].Geometry);
                Assert.AreEqual(originalWaternetLines[i].PhreaticLine.Name, actualWaternetLines[i].PhreaticLine.Name);
                CollectionAssert.AreEqual(originalWaternetLines[i].PhreaticLine.Geometry, actualWaternetLines[i].PhreaticLine.Geometry);
            }
        }
    }
}