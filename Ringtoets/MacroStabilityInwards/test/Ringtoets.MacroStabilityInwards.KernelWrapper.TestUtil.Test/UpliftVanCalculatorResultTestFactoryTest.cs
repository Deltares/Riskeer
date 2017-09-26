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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class UpliftVanCalculatorResultTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnResultWithDefaultValues()
        {
            // Call
            UpliftVanCalculatorResult result = UpliftVanCalculatorResultTestFactory.Create();

            // Assert
            Assert.AreEqual(0.1, result.FactorOfStability);
            Assert.AreEqual(0.2, result.ZValue);
            Assert.AreEqual(0.3, result.ForbiddenZonesXEntryMin);
            Assert.AreEqual(0.4, result.ForbiddenZonesXEntryMax);
            Assert.IsTrue(result.GridAutomaticallyCalculated);
            Assert.IsTrue(result.ForbiddenZonesAutomaticallyCalculated);

            AssertSlidingCurve(result.SlidingCurveResult);
            AssertUpliftVanCalculationGrid(result.CalculationGridResult);
        }

        private static void AssertSlidingCurve(UpliftVanSlidingCurveResult slidingCurve)
        {
            AssertCircle(slidingCurve.LeftCircle);
            AssertCircle(slidingCurve.RightCircle);

            Assert.AreEqual(0, slidingCurve.IteratedHorizontalForce);
            Assert.AreEqual(0, slidingCurve.NonIteratedHorizontalForce);
            CollectionAssert.IsEmpty(slidingCurve.Slices);
        }

        private static void AssertCircle(UpliftVanSlidingCircleResult circle)
        {
            Assert.AreEqual(new Point2D(0, 0), circle.Center);
            Assert.AreEqual(0.1, circle.Radius);
            Assert.IsTrue(circle.IsActive);
            Assert.AreEqual(0.2, circle.NonIteratedForce);
            Assert.AreEqual(0.3, circle.IteratedForce);
            Assert.AreEqual(0.4, circle.DrivingMoment);
            Assert.AreEqual(0.5, circle.ResistingMoment);
        }

        private static void AssertUpliftVanCalculationGrid(UpliftVanCalculationGridResult upliftVanCalculationGrid)
        {
            AssertGrid(upliftVanCalculationGrid.LeftGrid);
            AssertGrid(upliftVanCalculationGrid.RightGrid);

            CollectionAssert.AreEqual(new[]
            {
                3,
                2,
                1.5
            }, upliftVanCalculationGrid.TangentLines);
        }

        private static void AssertGrid(UpliftVanGridResult grid)
        {
            Assert.AreEqual(0.1, grid.XLeft);
            Assert.AreEqual(0.2, grid.XRight);
            Assert.AreEqual(0.3, grid.ZTop);
            Assert.AreEqual(0.4, grid.ZBottom);
            Assert.AreEqual(1, grid.NumberOfHorizontalPoints);
            Assert.AreEqual(2, grid.NumberOfVerticalPoints);
        }
    }
}