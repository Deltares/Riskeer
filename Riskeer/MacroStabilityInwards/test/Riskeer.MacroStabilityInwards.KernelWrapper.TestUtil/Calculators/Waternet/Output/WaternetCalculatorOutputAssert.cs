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
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Output
{
    /// <summary>
    /// Class for asserting Waternet calculator output.
    /// </summary>
    public static class WaternetCalculatorOutputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="WaternetLine"/> array.</param>
        /// <param name="actual">The actual <see cref="WaternetLineResult"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertWaternetLines(WaternetLine[] expected, WaternetLineResult[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Name, actual[i].Name);
                CollectionAssert.AreEqual(expected[i].Points.Select(p => new Point2D(p.X, p.Z)), actual[i].Geometry);
                AssertPhreaticLine(expected[i].HeadLine, actual[i].PhreaticLine);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="GeometryPointString"/> array.</param>
        /// <param name="actual">The actual <see cref="WaternetPhreaticLineResult"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertPhreaticLines(GeometryPointString[] expected, WaternetPhreaticLineResult[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                AssertPhreaticLine(expected[i], actual[i]);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="GeometryPointString"/>.</param>
        /// <param name="actual">The actual <see cref="WaternetPhreaticLineResult"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertPhreaticLine(GeometryPointString expected, WaternetPhreaticLineResult actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            CollectionAssert.AreEqual(expected.Points.Select(p => new Point2D(p.X, p.Z)), actual.Geometry);
        }
    }
}