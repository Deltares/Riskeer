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

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSliceTestFactoryTest
    {
        [Test]
        public void CreateSlice_Always_ReturnsMacroStabilityInwardsSlice()
        {
            // Call
            MacroStabilityInwardsSlice slice = MacroStabilityInwardsSliceTestFactory.CreateSlice();

            // Assert
            AssertPointIsConcreteValue(slice.TopLeftPoint);
            AssertPointIsConcreteValue(slice.TopRightPoint);
            AssertPointIsConcreteValue(slice.BottomLeftPoint);
            AssertPointIsConcreteValue(slice.BottomRightPoint);

            AssertIsConcreteValue(slice.Cohesion);
            AssertIsConcreteValue(slice.FrictionAngle);
            AssertIsConcreteValue(slice.CriticalPressure);
            AssertIsConcreteValue(slice.OverConsolidationRatio);
            AssertIsConcreteValue(slice.Pop);
            AssertIsConcreteValue(slice.DegreeOfConsolidationPorePressureSoil);
            AssertIsConcreteValue(slice.DegreeOfConsolidationPorePressureLoad);
            AssertIsConcreteValue(slice.Dilatancy);
            AssertIsConcreteValue(slice.ExternalLoad);
            AssertIsConcreteValue(slice.HydrostaticPorePressure);
            AssertIsConcreteValue(slice.LeftForce);
            AssertIsConcreteValue(slice.LeftForceAngle);
            AssertIsConcreteValue(slice.RightForce);
            AssertIsConcreteValue(slice.RightForceAngle);
            AssertIsConcreteValue(slice.LoadStress);
            AssertIsConcreteValue(slice.NormalStress);
            AssertIsConcreteValue(slice.PorePressure);
            AssertIsConcreteValue(slice.HorizontalPorePressure);
            AssertIsConcreteValue(slice.VerticalPorePressure);
            AssertIsConcreteValue(slice.PiezometricPorePressure);
            AssertIsConcreteValue(slice.EffectiveStress);
            AssertIsConcreteValue(slice.EffectiveStressDaily);
            AssertIsConcreteValue(slice.ExcessPorePressure);
            AssertIsConcreteValue(slice.ShearStress);
            AssertIsConcreteValue(slice.SoilStress);
            AssertIsConcreteValue(slice.TotalPorePressure);
            AssertIsConcreteValue(slice.TotalStress);
            AssertIsConcreteValue(slice.Weight);
        }

        private static void AssertPointIsConcreteValue(Point2D actualPoint)
        {
            AssertIsConcreteValue(actualPoint.X);
            AssertIsConcreteValue(actualPoint.Y);
        }

        private static void AssertIsConcreteValue(double actualValue)
        {
            Assert.IsFalse(double.IsNaN(actualValue));
            Assert.IsFalse(double.IsInfinity(actualValue));
        }
    }
}