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

using System.Linq;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Result;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result
{
    /// <summary>
    /// Helper that can be used in tests.
    /// </summary>
    public static class UpliftVanCalculatorResultHelper
    {
        /// <summary>
        /// Assert whether the <paramref name="actual"/> is equal to the <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected values.</param>
        /// <param name="actual">The actual values.</param>
        /// <exception cref="AssertionException">Thrown when the <paramref name="actual"/>
        /// is not equal to the <paramref name="expected"/>.</exception>
        public static void AssertSlidingCurve(UpliftVanSlidingCurveResult expected, UpliftVanSlidingCurveResult actual)
        {
            Assert.AreEqual(expected.IteratedHorizontalForce, actual.IteratedHorizontalForce);
            Assert.AreEqual(expected.NonIteratedHorizontalForce, actual.NonIteratedHorizontalForce);
            AssertCircle(expected.LeftCircle, actual.LeftCircle);
            AssertCircle(expected.RightCircle, actual.RightCircle);

            AssertSlices(expected.Slices.ToArray(), actual.Slices.ToArray());
        }

        public static void AssertSlipPlaneGrid(UpliftVanCalculationGridResult expected, UpliftVanCalculationGridResult actual)
        {
            CollectionAssert.AreEqual(expected.TangentLines, actual.TangentLines);
            AssertGrid(expected.LeftGrid, actual.LeftGrid);
            AssertGrid(expected.RightGrid, actual.RightGrid);
        }

        private static void AssertGrid(UpliftVanGridResult expected, UpliftVanGridResult actual)
        {
            Assert.AreEqual(expected.XLeft, actual.XLeft);
            Assert.AreEqual(expected.XRight, actual.XRight);
            Assert.AreEqual(expected.ZTop, actual.ZTop);
            Assert.AreEqual(expected.ZBottom, actual.ZBottom);
            Assert.AreEqual(expected.NumberOfHorizontalPoints, actual.NumberOfHorizontalPoints);
            Assert.AreEqual(expected.NumberOfVerticalPoints, actual.NumberOfVerticalPoints);
        }

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

        private static void AssertSlices(UpliftVanSliceResult[] expectedSlices, UpliftVanSliceResult[] actualSlices)
        {
            Assert.AreEqual(expectedSlices.Length, actualSlices.Length);

            for (var i = 0; i < expectedSlices.Length; i++)
            {
                Assert.AreEqual(expectedSlices[i].Cohesion, actualSlices[i].Cohesion);
                Assert.AreEqual(expectedSlices[i].FrictionAngle, actualSlices[i].FrictionAngle);
                Assert.AreEqual(expectedSlices[i].CriticalPressure, actualSlices[i].CriticalPressure);
                Assert.AreEqual(expectedSlices[i].OverConsolidationRatio, actualSlices[i].OverConsolidationRatio);
                Assert.AreEqual(expectedSlices[i].Pop, actualSlices[i].Pop);
                Assert.AreEqual(expectedSlices[i].DegreeOfConsolidationPorePressureSoil, actualSlices[i].DegreeOfConsolidationPorePressureSoil);
                Assert.AreEqual(expectedSlices[i].DegreeOfConsolidationPorePressureLoad, actualSlices[i].DegreeOfConsolidationPorePressureLoad);
                Assert.AreEqual(expectedSlices[i].Dilatancy, actualSlices[i].Dilatancy);
                Assert.AreEqual(expectedSlices[i].ExternalLoad, actualSlices[i].ExternalLoad);
                Assert.AreEqual(expectedSlices[i].HydrostaticPorePressure, actualSlices[i].HydrostaticPorePressure);
                Assert.AreEqual(expectedSlices[i].LeftForce, actualSlices[i].LeftForce);
                Assert.AreEqual(expectedSlices[i].LeftForceAngle, actualSlices[i].LeftForceAngle);
                Assert.AreEqual(expectedSlices[i].LeftForceY, actualSlices[i].LeftForceY);
                Assert.AreEqual(expectedSlices[i].RightForce, actualSlices[i].RightForce);
                Assert.AreEqual(expectedSlices[i].RightForceAngle, actualSlices[i].RightForceAngle);
                Assert.AreEqual(expectedSlices[i].RightForceY, actualSlices[i].RightForceY);
                Assert.AreEqual(expectedSlices[i].LoadStress, actualSlices[i].LoadStress);
                Assert.AreEqual(expectedSlices[i].NormalStress, actualSlices[i].NormalStress);
                Assert.AreEqual(expectedSlices[i].PorePressure, actualSlices[i].PorePressure);
                Assert.AreEqual(expectedSlices[i].HorizontalPorePressure, actualSlices[i].HorizontalPorePressure);
                Assert.AreEqual(expectedSlices[i].VerticalPorePressure, actualSlices[i].VerticalPorePressure);
                Assert.AreEqual(expectedSlices[i].PiezometricPorePressure, actualSlices[i].PiezometricPorePressure);
                Assert.AreEqual(expectedSlices[i].EffectiveStress, actualSlices[i].EffectiveStress);
                Assert.AreEqual(expectedSlices[i].EffectiveStressDaily, actualSlices[i].EffectiveStressDaily);
                Assert.AreEqual(expectedSlices[i].ExcessPorePressure, actualSlices[i].ExcessPorePressure);
                Assert.AreEqual(expectedSlices[i].ShearStress, actualSlices[i].ShearStress);
                Assert.AreEqual(expectedSlices[i].SoilStress, actualSlices[i].SoilStress);
                Assert.AreEqual(expectedSlices[i].TotalPorePressure, actualSlices[i].TotalPorePressure);
                Assert.AreEqual(expectedSlices[i].TotalStress, actualSlices[i].TotalStress);
                Assert.AreEqual(expectedSlices[i].Weight, actualSlices[i].Weight);
            }
        }
    }
}