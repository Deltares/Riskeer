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
using Deltares.WTIStability;
using NUnit.Framework;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class SlidingDualCircleTestFactoryTest
    {
        [Test]
        public void Create_Always_ReturnSlidingDualCircleWithDefaultValues()
        {
            // Call
            SlidingDualCircle slidingDualCircle = SlidingDualCircleTestFactory.Create();

            // Assert
            Assert.IsFalse(slidingDualCircle.LeftCircleIsActive);
            Assert.AreEqual(0.1, slidingDualCircle.ActiveCircle.X);
            Assert.AreEqual(0.2, slidingDualCircle.ActiveCircle.Z);
            Assert.AreEqual(0.3, slidingDualCircle.ActiveForce);
            Assert.AreEqual(0.4, slidingDualCircle.ActiveForce0);
            Assert.AreEqual(0.5, slidingDualCircle.ActiveRadius);
            Assert.AreEqual(0.6, slidingDualCircle.DrivingMomentActive);
            Assert.AreEqual(0.7, slidingDualCircle.ResistingMomentActive);
            Assert.AreEqual(0.8, slidingDualCircle.PassiveCircle.X);
            Assert.AreEqual(0.9, slidingDualCircle.PassiveCircle.Z);
            Assert.AreEqual(1, slidingDualCircle.PassiveForce);
            Assert.AreEqual(1.1, slidingDualCircle.PassiveForce0);
            Assert.AreEqual(1.2, slidingDualCircle.PassiveRadius);
            Assert.AreEqual(1.3, slidingDualCircle.DrivingMomentPassive);
            Assert.AreEqual(1.4, slidingDualCircle.ResistingMomentPassive);
            Assert.AreEqual(1.5, slidingDualCircle.HorizontalForce);
            Assert.AreEqual(1.6, slidingDualCircle.HorizontalForce0);
            Assert.AreEqual(1, slidingDualCircle.Slices.Count);
            Slice slice = slidingDualCircle.Slices.First();

            Assert.AreEqual(1.7, slice.TopLeftX);
            Assert.AreEqual(1.8, slice.TopLeftZ);
            Assert.AreEqual(1.9, slice.TopRightX);
            Assert.AreEqual(2, slice.TopRightZ);
            Assert.AreEqual(2.1, slice.BottomLeftX);
            Assert.AreEqual(2.2, slice.BottomLeftZ);
            Assert.AreEqual(2.3, slice.BottomRightX);
            Assert.AreEqual(2.4, slice.BottomRightZ);
            Assert.AreEqual(2.5, slice.Cohesion);
            Assert.AreEqual(2.6, slice.Phi);
            Assert.AreEqual(2.7, slice.PGrens);
            Assert.AreEqual(2.8, slice.OCR);
            Assert.AreEqual(2.9, slice.POP);
            Assert.AreEqual(3, slice.DegreeofConsolidationPorePressure);
            Assert.AreEqual(3.1, slice.PorePressureDueToDegreeOfConsolidationLoad);
            Assert.AreEqual(3.2, slice.Dilatancy);
            Assert.AreEqual(3.3, slice.ExternalLoad);
            Assert.AreEqual(3.4, slice.HydrostaticPorePressure);
            Assert.AreEqual(3.5, slice.LeftForce);
            Assert.AreEqual(3.6, slice.LeftForceAngle);
            Assert.AreEqual(3.7, slice.LeftForceY);
            Assert.AreEqual(3.8, slice.RightForce);
            Assert.AreEqual(3.9, slice.RightForceAngle);
            Assert.AreEqual(4, slice.RightForceY);
            Assert.AreEqual(4.1, slice.LoadStress);
            Assert.AreEqual(4.2, slice.NormalStress);
            Assert.AreEqual(4.3, slice.PoreOnSurface);
            Assert.AreEqual(4.4, slice.HPoreOnSurface);
            Assert.AreEqual(4.5, slice.VPoreOnSurface);
            Assert.AreEqual(4.6, slice.PiezometricPorePressure);
            Assert.AreEqual(4.7, slice.EffectiveStress);
            Assert.AreEqual(4.8, slice.EffectiveStressDaily);
            Assert.AreEqual(4.9, slice.ExcessPorePressure);
            Assert.AreEqual(5, slice.ShearStress);
            Assert.AreEqual(5.1, slice.SoilStress);
            Assert.AreEqual(5.2, slice.TotalPorePressure);
            Assert.AreEqual(5.3, slice.TotalStress);
            Assert.AreEqual(5.4, slice.Weight);
        }
    }
}