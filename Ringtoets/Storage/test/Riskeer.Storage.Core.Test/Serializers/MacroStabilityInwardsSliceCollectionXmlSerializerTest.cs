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

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Riskeer.Storage.Core.Serializers;
using Riskeer.Storage.Core.TestUtil.Serializers;

namespace Riskeer.Storage.Core.Test.Serializers
{
    [TestFixture]
    public class MacroStabilityInwardsSliceCollectionXmlSerializerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var serializer = new MacroStabilityInwardsSliceCollectionXmlSerializer();

            // Assert
            Assert.IsInstanceOf<DataCollectionSerializer<MacroStabilityInwardsSlice,
                MacroStabilityInwardsSliceCollectionXmlSerializer.SerializableMacroStabilityInwardsSlice>>(serializer);
            SerializerTestHelper.AssertSerializedData(typeof(MacroStabilityInwardsSliceCollectionXmlSerializer.SerializableMacroStabilityInwardsSlice));
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
            var serializer = new MacroStabilityInwardsSliceCollectionXmlSerializer();

            // When
            string xml = serializer.ToXml(original);
            MacroStabilityInwardsSlice[] roundtripResult = serializer.FromXml(xml);

            // Then
            TestHelper.AssertCollectionsAreEqual(original, roundtripResult, new MacroStabilityInwardsSliceComparer());
        }

        private class MacroStabilityInwardsSliceComparer : IEqualityComparer<MacroStabilityInwardsSlice>
        {
            public bool Equals(MacroStabilityInwardsSlice x, MacroStabilityInwardsSlice y)
            {
                return x.TopLeftPoint.Equals(y.TopLeftPoint)
                       && x.TopRightPoint.Equals(y.TopRightPoint)
                       && x.BottomLeftPoint.Equals(y.BottomLeftPoint)
                       && x.BottomRightPoint.Equals(y.BottomRightPoint)
                       && x.Cohesion.Equals(y.Cohesion)
                       && x.FrictionAngle.Equals(y.FrictionAngle)
                       && x.CriticalPressure.Equals(y.CriticalPressure)
                       && x.OverConsolidationRatio.Equals(y.OverConsolidationRatio)
                       && x.Pop.Equals(y.Pop)
                       && x.DegreeOfConsolidationPorePressureSoil.Equals(y.DegreeOfConsolidationPorePressureSoil)
                       && x.DegreeOfConsolidationPorePressureLoad.Equals(y.DegreeOfConsolidationPorePressureLoad)
                       && x.Dilatancy.Equals(y.Dilatancy)
                       && x.ExternalLoad.Equals(y.ExternalLoad)
                       && x.HydrostaticPorePressure.Equals(y.HydrostaticPorePressure)
                       && x.LeftForce.Equals(y.LeftForce)
                       && x.LeftForceAngle.Equals(y.LeftForceAngle)
                       && x.LeftForceY.Equals(y.LeftForceY)
                       && x.RightForce.Equals(y.RightForce)
                       && x.RightForceAngle.Equals(y.RightForceAngle)
                       && x.RightForceY.Equals(y.RightForceY)
                       && x.LoadStress.Equals(y.LoadStress)
                       && x.NormalStress.Equals(y.NormalStress)
                       && x.PorePressure.Equals(y.PorePressure)
                       && x.HorizontalPorePressure.Equals(y.HorizontalPorePressure)
                       && x.VerticalPorePressure.Equals(y.VerticalPorePressure)
                       && x.PiezometricPorePressure.Equals(y.PiezometricPorePressure)
                       && x.EffectiveStress.Equals(y.EffectiveStress)
                       && x.EffectiveStressDaily.Equals(y.EffectiveStressDaily)
                       && x.ExcessPorePressure.Equals(y.ExcessPorePressure)
                       && x.ShearStress.Equals(y.ShearStress)
                       && x.SoilStress.Equals(y.SoilStress)
                       && x.TotalPorePressure.Equals(y.TotalPorePressure)
                       && x.TotalStress.Equals(y.TotalStress)
                       && x.Weight.Equals(y.Weight);
            }

            public int GetHashCode(MacroStabilityInwardsSlice obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}