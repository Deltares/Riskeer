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

using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Views;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Views
{
    public class MacroStabilityInwardsFormattedSliceRowTest
    {
        [Test]
        public void Constructor_SliceNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsFormattedSliceRow(null, 2);

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
                    LoadStress = 17.0
                });

            // Call
            var formattedSliceRow = new MacroStabilityInwardsFormattedSliceRow(slice, 3);

            // Assert
            Assert.AreEqual("Lamel 3", formattedSliceRow.Name);

            Assert.AreEqual(2, formattedSliceRow.XCenter.NumberOfDecimalPlaces);
            Assert.AreEqual(5.0, formattedSliceRow.XCenter, formattedSliceRow.XCenter.GetAccuracy());

            Assert.AreEqual(2, formattedSliceRow.ZCenterBottom.NumberOfDecimalPlaces);
            Assert.AreEqual(1.5, formattedSliceRow.ZCenterBottom, formattedSliceRow.ZCenterBottom.GetAccuracy());

            Assert.AreEqual(2, formattedSliceRow.Width.NumberOfDecimalPlaces);
            Assert.AreEqual(10.0, formattedSliceRow.Width, formattedSliceRow.Width.GetAccuracy());

            Assert.AreEqual(2, formattedSliceRow.ArcLength.NumberOfDecimalPlaces);
            Assert.AreEqual(10.05, formattedSliceRow.ArcLength, formattedSliceRow.ArcLength.GetAccuracy());

            Assert.AreEqual(2, formattedSliceRow.BottomAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(-5.71, formattedSliceRow.BottomAngle, formattedSliceRow.BottomAngle.GetAccuracy());

            Assert.AreEqual(2, formattedSliceRow.TopAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(-11.31, formattedSliceRow.TopAngle, formattedSliceRow.TopAngle.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.FrictionAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(2.0, formattedSliceRow.FrictionAngle, formattedSliceRow.FrictionAngle.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.Cohesion.NumberOfDecimalPlaces);
            Assert.AreEqual(double.NaN, formattedSliceRow.Cohesion, formattedSliceRow.Cohesion.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.EffectiveStress.NumberOfDecimalPlaces);
            Assert.AreEqual(3.0, formattedSliceRow.EffectiveStress, formattedSliceRow.EffectiveStress.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.TotalPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(4.0, formattedSliceRow.TotalPorePressure, formattedSliceRow.TotalPorePressure.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.Weight.NumberOfDecimalPlaces);
            Assert.AreEqual(5.0, formattedSliceRow.Weight, formattedSliceRow.Weight.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.PiezometricPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(6.0, formattedSliceRow.PiezometricPorePressure, formattedSliceRow.PiezometricPorePressure.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.DegreeOfConsolidationPorePressureSoil.NumberOfDecimalPlaces);
            Assert.AreEqual(7.0, formattedSliceRow.DegreeOfConsolidationPorePressureSoil, formattedSliceRow.DegreeOfConsolidationPorePressureSoil.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.DegreeOfConsolidationPorePressureLoad.NumberOfDecimalPlaces);
            Assert.AreEqual(8.0, formattedSliceRow.DegreeOfConsolidationPorePressureLoad, formattedSliceRow.DegreeOfConsolidationPorePressureLoad.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.PorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(9.0, formattedSliceRow.PorePressure, formattedSliceRow.PorePressure.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.VerticalPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(10.0, formattedSliceRow.VerticalPorePressure, formattedSliceRow.VerticalPorePressure.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.HorizontalPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(11.0, formattedSliceRow.HorizontalPorePressure, formattedSliceRow.HorizontalPorePressure.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.ExternalLoad.NumberOfDecimalPlaces);
            Assert.AreEqual(12.0, formattedSliceRow.ExternalLoad, formattedSliceRow.ExternalLoad.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.OverConsolidationRatio.NumberOfDecimalPlaces);
            Assert.AreEqual(13.0, formattedSliceRow.OverConsolidationRatio, formattedSliceRow.OverConsolidationRatio.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.Pop.NumberOfDecimalPlaces);
            Assert.AreEqual(14.0, formattedSliceRow.Pop, formattedSliceRow.Pop.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.NormalStress.NumberOfDecimalPlaces);
            Assert.AreEqual(15.0, formattedSliceRow.NormalStress, formattedSliceRow.NormalStress.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.ShearStress.NumberOfDecimalPlaces);
            Assert.AreEqual(16.0, formattedSliceRow.ShearStress, formattedSliceRow.ShearStress.GetAccuracy());

            Assert.AreEqual(3, formattedSliceRow.LoadStress.NumberOfDecimalPlaces);
            Assert.AreEqual(17.0, formattedSliceRow.LoadStress, formattedSliceRow.LoadStress.GetAccuracy());
        }
    }
}