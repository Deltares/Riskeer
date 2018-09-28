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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Output;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators.UpliftVan.Output
{
    [TestFixture]
    public class UpliftVanSlidingCurveResultTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnUpliftVanSlidingCurveResult()
        {
            // Call
            UpliftVanSlidingCurveResult curve = UpliftVanSlidingCurveResultTestFactory.Create();

            // Assert
            AssertCircle(curve.LeftCircle);
            AssertCircle(curve.RightCircle);
            Assert.AreEqual(0, curve.IteratedHorizontalForce);
            Assert.AreEqual(0, curve.NonIteratedHorizontalForce);
            CollectionAssert.IsEmpty(curve.Slices);
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
    }
}