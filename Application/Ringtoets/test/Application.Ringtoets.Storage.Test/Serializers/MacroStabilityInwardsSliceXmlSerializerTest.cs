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
using System.Collections;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;

namespace Application.Ringtoets.Storage.Test.Serializers
{
    [TestFixture]
    public class MacroStabilityInwardsSliceXmlSerializerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var serializer = new MacroStabilityInwardsSliceXmlSerializer();

            // Assert
            Assert.IsInstanceOf<DataCollectionSerializer<MacroStabilityInwardsSlice,
                MacroStabilityInwardsSliceXmlSerializer.SerializableMacroStabilityInwardsSlice>>(serializer);
        }

        [Test]
        public void GivenArrayOfMacroStabilityInwardsSlices_WhenConvertingRoundTrip_ThenEqualMacroStabilityInwardsSlices()
        {
            // Given
            var random = new Random(31);
            var original = new[]
            {
                new MacroStabilityInwardsSlice(
                    new Point2D(double.NaN, double.NaN),
                    new Point2D(double.NaN, double.NaN),
                    new Point2D(double.NaN, double.NaN),
                    new Point2D(double.NaN, double.NaN),
                    new MacroStabilityInwardsSlice.ConstructionProperties()),
                new MacroStabilityInwardsSlice(
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new Point2D(random.NextDouble(), random.NextDouble()),
                    new MacroStabilityInwardsSlice.ConstructionProperties
                    {
                        Cohesion = random.NextDouble(),
                        FrictionAngle = random.NextDouble(),
                        CriticalPressure = random.NextDouble(),
                        OverConsolidationRatio = random.NextDouble(),
                        Pop = random.NextDouble(),
                        DegreeOfConsolidationPorePressureSoil = random.NextDouble(),
                        DegreeOfConsolidationPorePressureLoad = random.NextDouble(),
                        Dilatancy = random.NextDouble(),
                        ExternalLoad = random.NextDouble(),
                        HydrostaticPorePressure = random.NextDouble(),
                        LeftForce = random.NextDouble(),
                        LeftForceAngle = random.NextDouble(),
                        LeftForceY = random.NextDouble(),
                        RightForce = random.NextDouble(),
                        RightForceAngle = random.NextDouble(),
                        RightForceY = random.NextDouble(),
                        LoadStress = random.NextDouble(),
                        NormalStress = random.NextDouble(),
                        PorePressure = random.NextDouble(),
                        HorizontalPorePressure = random.NextDouble(),
                        VerticalPorePressure = random.NextDouble(),
                        PiezometricPorePressure = random.NextDouble(),
                        EffectiveStress = random.NextDouble(),
                        EffectiveStressDaily = random.NextDouble(),
                        ExcessPorePressure = random.NextDouble(),
                        ShearStress = random.NextDouble(),
                        SoilStress = random.NextDouble(),
                        TotalPorePressure = random.NextDouble(),
                        TotalStress = random.NextDouble(),
                        Weight = random.NextDouble()
                    })
            };
            var serializer = new MacroStabilityInwardsSliceXmlSerializer();

            // When
            string xml = serializer.ToXml(original);
            MacroStabilityInwardsSlice[] roundtripResult = serializer.FromXml(xml);

            // Then
            CollectionAssert.AreEqual(original, roundtripResult, new MacroStabilityInwardsSliceComparer());
        }

        private class MacroStabilityInwardsSliceComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                var x1 = (MacroStabilityInwardsSlice) x;
                var y1 = (MacroStabilityInwardsSlice) y;
                if (x1.TopLeftPoint.Equals(y1.TopLeftPoint)
                    && x1.TopRightPoint.Equals(y1.TopRightPoint)
                    && x1.BottomLeftPoint.Equals(y1.BottomLeftPoint)
                    && x1.BottomRightPoint.Equals(y1.BottomRightPoint)
                    && x1.Cohesion.Equals(y1.Cohesion)
                    && x1.FrictionAngle.Equals(y1.FrictionAngle)
                    && x1.CriticalPressure.Equals(y1.CriticalPressure)
                    && x1.OverConsolidationRatio.Equals(y1.OverConsolidationRatio)
                    && x1.Pop.Equals(y1.Pop)
                    && x1.DegreeOfConsolidationPorePressureSoil.Equals(y1.DegreeOfConsolidationPorePressureSoil)
                    && x1.DegreeOfConsolidationPorePressureLoad.Equals(y1.DegreeOfConsolidationPorePressureLoad)
                    && x1.Dilatancy.Equals(y1.Dilatancy)
                    && x1.ExternalLoad.Equals(y1.ExternalLoad)
                    && x1.HydrostaticPorePressure.Equals(y1.HydrostaticPorePressure)
                    && x1.LeftForce.Equals(y1.LeftForce)
                    && x1.LeftForceAngle.Equals(y1.LeftForceAngle)
                    && x1.LeftForceY.Equals(y1.LeftForceY)
                    && x1.RightForce.Equals(y1.RightForce)
                    && x1.RightForceAngle.Equals(y1.RightForceAngle)
                    && x1.RightForceY.Equals(y1.RightForceY)
                    && x1.LoadStress.Equals(y1.LoadStress)
                    && x1.NormalStress.Equals(y1.NormalStress)
                    && x1.PorePressure.Equals(y1.PorePressure)
                    && x1.HorizontalPorePressure.Equals(y1.HorizontalPorePressure)
                    && x1.VerticalPorePressure.Equals(y1.VerticalPorePressure)
                    && x1.PiezometricPorePressure.Equals(y1.PiezometricPorePressure)
                    && x1.EffectiveStress.Equals(y1.EffectiveStress)
                    && x1.EffectiveStressDaily.Equals(y1.EffectiveStressDaily)
                    && x1.ExcessPorePressure.Equals(y1.ExcessPorePressure)
                    && x1.ShearStress.Equals(y1.ShearStress)
                    && x1.SoilStress.Equals(y1.SoilStress)
                    && x1.TotalPorePressure.Equals(y1.TotalPorePressure)
                    && x1.TotalStress.Equals(y1.TotalStress)
                    && x1.Weight.Equals(y1.Weight)
                )
                {
                    return 0;
                }
                return 1;
            }
        }
    }
}