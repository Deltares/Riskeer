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

using Deltares.WTIStability;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input
{
    /// <summary>
    /// Class for asserting Uplift Van kernel input.
    /// </summary>
    public static class UpliftVanKernelInputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SlipPlaneUpliftVan"/>.</param>
        /// <param name="actual">The actual <see cref="SlipPlaneUpliftVan"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSlipPlanesUpliftVan(SlipPlaneUpliftVan expected, SlipPlaneUpliftVan actual)
        {
            AssertSlipCircleGrid(expected.SlipPlaneLeftGrid, actual.SlipPlaneLeftGrid);
            AssertSlipCircleGrid(expected.SlipPlaneRightGrid, actual.SlipPlaneRightGrid);
            AssertSlipCircleTangentLine(expected.SlipCircleTangentLine, actual.SlipCircleTangentLine);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SlipPlaneConstraints"/>.</param>
        /// <param name="actual">The actual <see cref="SlipPlaneConstraints"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSlipPlaneConstraints(SlipPlaneConstraints expected, SlipPlaneConstraints actual)
        {
            Assert.AreEqual(expected.CreateZones, actual.CreateZones);
            Assert.AreEqual(expected.AutomaticForbiddenZones, actual.AutomaticForbiddenZones);
            Assert.AreEqual(expected.SlipPlaneMinDepth, actual.SlipPlaneMinDepth);
            Assert.AreEqual(expected.SlipPlaneMinLength, actual.SlipPlaneMinLength);
            Assert.AreEqual(expected.XEntryMin, actual.XEntryMin);
            Assert.AreEqual(expected.XEntryMax, actual.XEntryMax);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SlipCircleGrid"/>.</param>
        /// <param name="actual">The actual <see cref="SlipCircleGrid"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertSlipCircleGrid(SlipCircleGrid expected, SlipCircleGrid actual)
        {
            Assert.AreEqual(expected.GridXLeft, actual.GridXLeft);
            Assert.AreEqual(expected.GridXRight, actual.GridXRight);
            Assert.AreEqual(expected.GridZTop, actual.GridZTop);
            Assert.AreEqual(expected.GridZBottom, actual.GridZBottom);
            Assert.AreEqual(expected.GridXNumber, actual.GridXNumber);
            Assert.AreEqual(expected.GridZNumber, actual.GridZNumber);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="SlipCircleTangentLine"/>.</param>
        /// <param name="actual">The actual <see cref="SlipCircleTangentLine"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertSlipCircleTangentLine(SlipCircleTangentLine expected, SlipCircleTangentLine actual)
        {
            Assert.AreEqual(expected.AutomaticAtBoundaries, actual.AutomaticAtBoundaries);
            Assert.AreEqual(expected.TangentLineZTop, actual.TangentLineZTop);
            Assert.AreEqual(expected.TangentLineZBottom, actual.TangentLineZBottom);
            Assert.AreEqual(expected.TangentLineNumber, actual.TangentLineNumber);
            Assert.AreEqual(expected.MaxSpacingBetweenBoundaries, actual.MaxSpacingBetweenBoundaries);
        }
    }
}