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

using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Views;

namespace Riskeer.MacroStabilityInwards.Forms.Test.Views
{
    public class MacroStabilityInwardsSliceRowTest
    {
        [Test]
        public void Constructor_SliceNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsSliceRow(null, new Random(22).Next());

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("slice", paramName);
        }

        [Test]
        public void Constructor_WithSlice_ExpectedValues()
        {
            // Setup
            var slice = new MacroStabilityInwardsSlice(
                new Point2D(0, 10),
                new Point2D(10, 8),
                new Point2D(0, 2),
                new Point2D(10, 1),
                new MacroStabilityInwardsSlice.ConstructionProperties
                {
                    Cohesion = double.NaN,
                    FrictionAngle = 2.0,
                    EffectiveStress = 3.0,
                    TotalPorePressure = 4.0,
                    Weight = 5.0,
                    PiezometricPorePressure = 6.0,
                    DegreeOfConsolidationPorePressureSoil = 7.0,
                    DegreeOfConsolidationPorePressureLoad = 8.0,
                    PorePressure = 9.0,
                    VerticalPorePressure = 10.0,
                    HorizontalPorePressure = 11.0,
                    ExternalLoad = 12.0,
                    OverConsolidationRatio = 13.0,
                    Pop = 14.0,
                    NormalStress = 15.0,
                    ShearStress = 16.0,
                    LoadStress = 17.0,
                    EffectiveStressDaily = 18.0
                });

            // Call
            var sliceRow = new MacroStabilityInwardsSliceRow(slice, 3);

            // Assert
            Assert.AreEqual("Lamel 3", sliceRow.Name);

            Assert.AreEqual(5.0, sliceRow.XCenter, sliceRow.XCenter.GetAccuracy());
            Assert.AreEqual(1.5, sliceRow.ZCenterBottom, sliceRow.ZCenterBottom.GetAccuracy());
            Assert.AreEqual(10.0, sliceRow.Width, sliceRow.Width.GetAccuracy());
            Assert.AreEqual(10.05, sliceRow.ArcLength, sliceRow.ArcLength.GetAccuracy());
            Assert.AreEqual(-5.71, sliceRow.BottomAngle, sliceRow.BottomAngle.GetAccuracy());
            Assert.AreEqual(-11.31, sliceRow.TopAngle, sliceRow.TopAngle.GetAccuracy());
            Assert.AreEqual(2.0, sliceRow.FrictionAngle, sliceRow.FrictionAngle.GetAccuracy());
            Assert.AreEqual(double.NaN, sliceRow.Cohesion, sliceRow.Cohesion.GetAccuracy());
            Assert.AreEqual(3.0, sliceRow.EffectiveStress, sliceRow.EffectiveStress.GetAccuracy());
            Assert.AreEqual(4.0, sliceRow.TotalPorePressure, sliceRow.TotalPorePressure.GetAccuracy());
            Assert.AreEqual(5.0, sliceRow.Weight, sliceRow.Weight.GetAccuracy());
            Assert.AreEqual(6.0, sliceRow.PiezometricPorePressure, sliceRow.PiezometricPorePressure.GetAccuracy());
            Assert.AreEqual(9.0, sliceRow.PorePressure, sliceRow.PorePressure.GetAccuracy());
            Assert.AreEqual(10.0, sliceRow.VerticalPorePressure, sliceRow.VerticalPorePressure.GetAccuracy());
            Assert.AreEqual(11.0, sliceRow.HorizontalPorePressure, sliceRow.HorizontalPorePressure.GetAccuracy());
            Assert.AreEqual(13.0, sliceRow.OverConsolidationRatio, sliceRow.OverConsolidationRatio.GetAccuracy());
            Assert.AreEqual(14.0, sliceRow.Pop, sliceRow.Pop.GetAccuracy());
            Assert.AreEqual(15.0, sliceRow.NormalStress, sliceRow.NormalStress.GetAccuracy());
            Assert.AreEqual(16.0, sliceRow.ShearStress, sliceRow.ShearStress.GetAccuracy());
            Assert.AreEqual(17.0, sliceRow.LoadStress, sliceRow.LoadStress.GetAccuracy());
            Assert.AreEqual(18.0, sliceRow.EffectiveStressDaily, sliceRow.EffectiveStressDaily.GetAccuracy());
        }
    }
}