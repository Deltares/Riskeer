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

using System.Linq;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output
{
    /// <summary>
    /// Class for asserting Uplift Van calculator output.
    /// </summary>
    public static class UpliftVanCalculatorOutputAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="UpliftVanSlidingCurveResult"/>.</param>
        /// <param name="actual">The actual <see cref="UpliftVanSlidingCurveResult"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSlidingCurve(UpliftVanSlidingCurveResult expected, UpliftVanSlidingCurveResult actual)
        {
            Assert.AreEqual(expected.IteratedHorizontalForce, actual.IteratedHorizontalForce);
            Assert.AreEqual(expected.NonIteratedHorizontalForce, actual.NonIteratedHorizontalForce);
            AssertCircle(expected.LeftCircle, actual.LeftCircle);
            AssertCircle(expected.RightCircle, actual.RightCircle);
            AssertSlices(expected.Slices.ToArray(), actual.Slices.ToArray());
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="UpliftVanCalculationGridResult"/>.</param>
        /// <param name="actual">The actual <see cref="UpliftVanCalculationGridResult"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertSlipPlaneGrid(UpliftVanCalculationGridResult expected, UpliftVanCalculationGridResult actual)
        {
            CollectionAssert.AreEqual(expected.TangentLines, actual.TangentLines);
            AssertGrid(expected.LeftGrid, actual.LeftGrid);
            AssertGrid(expected.RightGrid, actual.RightGrid);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="UpliftVanSlidingCircleResult"/>.</param>
        /// <param name="actual">The actual <see cref="UpliftVanSlidingCircleResult"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertCircle(UpliftVanSlidingCircleResult expected, UpliftVanSlidingCircleResult actual)
        {
            Assert.AreEqual(expected.Center, actual.Center);
            Assert.AreEqual(expected.Radius, actual.Radius);
            Assert.AreEqual(expected.IsActive, actual.IsActive);
            Assert.AreEqual(expected.NonIteratedForce, actual.NonIteratedForce);
            Assert.AreEqual(expected.IteratedForce, actual.IteratedForce);
            Assert.AreEqual(expected.DrivingMoment, actual.DrivingMoment);
            Assert.AreEqual(expected.ResistingMoment, actual.ResistingMoment);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="UpliftVanSliceResult"/> array.</param>
        /// <param name="actual">The actual <see cref="UpliftVanSliceResult"/> array.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertSlices(UpliftVanSliceResult[] expected, UpliftVanSliceResult[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Cohesion, actual[i].Cohesion);
                Assert.AreEqual(expected[i].FrictionAngle, actual[i].FrictionAngle);
                Assert.AreEqual(expected[i].CriticalPressure, actual[i].CriticalPressure);
                Assert.AreEqual(expected[i].OverConsolidationRatio, actual[i].OverConsolidationRatio);
                Assert.AreEqual(expected[i].Pop, actual[i].Pop);
                Assert.AreEqual(expected[i].DegreeOfConsolidationPorePressureSoil, actual[i].DegreeOfConsolidationPorePressureSoil);
                Assert.AreEqual(expected[i].DegreeOfConsolidationPorePressureLoad, actual[i].DegreeOfConsolidationPorePressureLoad);
                Assert.AreEqual(expected[i].Dilatancy, actual[i].Dilatancy);
                Assert.AreEqual(expected[i].ExternalLoad, actual[i].ExternalLoad);
                Assert.AreEqual(expected[i].HydrostaticPorePressure, actual[i].HydrostaticPorePressure);
                Assert.AreEqual(expected[i].LeftForce, actual[i].LeftForce);
                Assert.AreEqual(expected[i].LeftForceAngle, actual[i].LeftForceAngle);
                Assert.AreEqual(expected[i].LeftForceY, actual[i].LeftForceY);
                Assert.AreEqual(expected[i].RightForce, actual[i].RightForce);
                Assert.AreEqual(expected[i].RightForceAngle, actual[i].RightForceAngle);
                Assert.AreEqual(expected[i].RightForceY, actual[i].RightForceY);
                Assert.AreEqual(expected[i].LoadStress, actual[i].LoadStress);
                Assert.AreEqual(expected[i].NormalStress, actual[i].NormalStress);
                Assert.AreEqual(expected[i].PorePressure, actual[i].PorePressure);
                Assert.AreEqual(expected[i].HorizontalPorePressure, actual[i].HorizontalPorePressure);
                Assert.AreEqual(expected[i].VerticalPorePressure, actual[i].VerticalPorePressure);
                Assert.AreEqual(expected[i].PiezometricPorePressure, actual[i].PiezometricPorePressure);
                Assert.AreEqual(expected[i].EffectiveStress, actual[i].EffectiveStress);
                Assert.AreEqual(expected[i].ExcessPorePressure, actual[i].ExcessPorePressure);
                Assert.AreEqual(expected[i].ShearStress, actual[i].ShearStress);
                Assert.AreEqual(expected[i].SoilStress, actual[i].SoilStress);
                Assert.AreEqual(expected[i].TotalPorePressure, actual[i].TotalPorePressure);
                Assert.AreEqual(expected[i].TotalStress, actual[i].TotalStress);
                Assert.AreEqual(expected[i].Weight, actual[i].Weight);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="UpliftVanGrid"/>.</param>
        /// <param name="actual">The actual <see cref="UpliftVanGrid"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertGrid(UpliftVanGrid expected, UpliftVanGrid actual)
        {
            Assert.AreEqual(expected.XLeft, actual.XLeft);
            Assert.AreEqual(expected.XRight, actual.XRight);
            Assert.AreEqual(expected.ZTop, actual.ZTop);
            Assert.AreEqual(expected.ZBottom, actual.ZBottom);
            Assert.AreEqual(expected.NumberOfHorizontalPoints, actual.NumberOfHorizontalPoints);
            Assert.AreEqual(expected.NumberOfVerticalPoints, actual.NumberOfVerticalPoints);
        }
    }
}