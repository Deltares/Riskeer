// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
        public static void AssertUpliftVanSlidingCurveResult(UpliftVanSlidingCurveResult expected, UpliftVanSlidingCurveResult actual)
        {
            Assert.AreEqual(expected.IteratedHorizontalForce, actual.IteratedHorizontalForce);
            Assert.AreEqual(expected.NonIteratedHorizontalForce, actual.NonIteratedHorizontalForce);
            AssertUpliftVanSlidingCircleResult(expected.LeftCircle, actual.LeftCircle);
            AssertUpliftVanSlidingCircleResult(expected.RightCircle, actual.RightCircle);
            AssertUpliftVanSliceResults(expected.Slices.ToArray(), actual.Slices.ToArray());
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="UpliftVanCalculationGridResult"/>.</param>
        /// <param name="actual">The actual <see cref="UpliftVanCalculationGridResult"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        public static void AssertUpliftVanCalculationGridResult(UpliftVanCalculationGridResult expected, UpliftVanCalculationGridResult actual)
        {
            CollectionAssert.AreEqual(expected.TangentLines, actual.TangentLines);
            AssertUpliftVanGrid(expected.LeftGrid, actual.LeftGrid);
            AssertUpliftVanGrid(expected.RightGrid, actual.RightGrid);
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="UpliftVanSlidingCircleResult"/>.</param>
        /// <param name="actual">The actual <see cref="UpliftVanSlidingCircleResult"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertUpliftVanSlidingCircleResult(UpliftVanSlidingCircleResult expected, UpliftVanSlidingCircleResult actual)
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
        /// <param name="expected">The expected collection of <see cref="UpliftVanSliceResult"/>.</param>
        /// <param name="actual">The actual collection of <see cref="UpliftVanSliceResult"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertUpliftVanSliceResults(ICollection<UpliftVanSliceResult> expected, ICollection<UpliftVanSliceResult> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                UpliftVanSliceResult expectedUpliftVanSliceResult = expected.ElementAt(i);
                UpliftVanSliceResult actualUpliftVanSliceResult = actual.ElementAt(i);

                Assert.AreEqual(expectedUpliftVanSliceResult.Cohesion, actualUpliftVanSliceResult.Cohesion);
                Assert.AreEqual(expectedUpliftVanSliceResult.FrictionAngle, actualUpliftVanSliceResult.FrictionAngle);
                Assert.AreEqual(expectedUpliftVanSliceResult.CriticalPressure, actualUpliftVanSliceResult.CriticalPressure);
                Assert.AreEqual(expectedUpliftVanSliceResult.OverConsolidationRatio, actualUpliftVanSliceResult.OverConsolidationRatio);
                Assert.AreEqual(expectedUpliftVanSliceResult.Pop, actualUpliftVanSliceResult.Pop);
                Assert.AreEqual(expectedUpliftVanSliceResult.DegreeOfConsolidationPorePressureSoil, actualUpliftVanSliceResult.DegreeOfConsolidationPorePressureSoil);
                Assert.AreEqual(expectedUpliftVanSliceResult.DegreeOfConsolidationPorePressureLoad, actualUpliftVanSliceResult.DegreeOfConsolidationPorePressureLoad);
                Assert.AreEqual(expectedUpliftVanSliceResult.Dilatancy, actualUpliftVanSliceResult.Dilatancy);
                Assert.AreEqual(expectedUpliftVanSliceResult.ExternalLoad, actualUpliftVanSliceResult.ExternalLoad);
                Assert.AreEqual(expectedUpliftVanSliceResult.HydrostaticPorePressure, actualUpliftVanSliceResult.HydrostaticPorePressure);
                Assert.AreEqual(expectedUpliftVanSliceResult.LeftForce, actualUpliftVanSliceResult.LeftForce);
                Assert.AreEqual(expectedUpliftVanSliceResult.LeftForceAngle, actualUpliftVanSliceResult.LeftForceAngle);
                Assert.AreEqual(expectedUpliftVanSliceResult.LeftForceY, actualUpliftVanSliceResult.LeftForceY);
                Assert.AreEqual(expectedUpliftVanSliceResult.RightForce, actualUpliftVanSliceResult.RightForce);
                Assert.AreEqual(expectedUpliftVanSliceResult.RightForceAngle, actualUpliftVanSliceResult.RightForceAngle);
                Assert.AreEqual(expectedUpliftVanSliceResult.RightForceY, actualUpliftVanSliceResult.RightForceY);
                Assert.AreEqual(expectedUpliftVanSliceResult.LoadStress, actualUpliftVanSliceResult.LoadStress);
                Assert.AreEqual(expectedUpliftVanSliceResult.NormalStress, actualUpliftVanSliceResult.NormalStress);
                Assert.AreEqual(expectedUpliftVanSliceResult.PorePressure, actualUpliftVanSliceResult.PorePressure);
                Assert.AreEqual(expectedUpliftVanSliceResult.HorizontalPorePressure, actualUpliftVanSliceResult.HorizontalPorePressure);
                Assert.AreEqual(expectedUpliftVanSliceResult.VerticalPorePressure, actualUpliftVanSliceResult.VerticalPorePressure);
                Assert.AreEqual(expectedUpliftVanSliceResult.PiezometricPorePressure, actualUpliftVanSliceResult.PiezometricPorePressure);
                Assert.AreEqual(expectedUpliftVanSliceResult.EffectiveStress, actualUpliftVanSliceResult.EffectiveStress);
                Assert.AreEqual(expectedUpliftVanSliceResult.ExcessPorePressure, actualUpliftVanSliceResult.ExcessPorePressure);
                Assert.AreEqual(expectedUpliftVanSliceResult.ShearStress, actualUpliftVanSliceResult.ShearStress);
                Assert.AreEqual(expectedUpliftVanSliceResult.SoilStress, actualUpliftVanSliceResult.SoilStress);
                Assert.AreEqual(expectedUpliftVanSliceResult.TotalPorePressure, actualUpliftVanSliceResult.TotalPorePressure);
                Assert.AreEqual(expectedUpliftVanSliceResult.TotalStress, actualUpliftVanSliceResult.TotalStress);
                Assert.AreEqual(expectedUpliftVanSliceResult.Weight, actualUpliftVanSliceResult.Weight);
            }
        }

        /// <summary>
        /// Asserts whether <paramref name="actual"/> is equal to <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="UpliftVanGrid"/>.</param>
        /// <param name="actual">The actual <see cref="UpliftVanGrid"/>.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="actual"/>
        /// is not equal to <paramref name="expected"/>.</exception>
        private static void AssertUpliftVanGrid(UpliftVanGrid expected, UpliftVanGrid actual)
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