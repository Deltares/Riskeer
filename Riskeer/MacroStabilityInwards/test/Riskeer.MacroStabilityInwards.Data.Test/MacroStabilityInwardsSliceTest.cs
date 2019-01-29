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
using Core.Common.Base.Geometry;
using Core.Common.Data.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.TestUtil;

namespace Riskeer.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSliceTest
    {
        [Test]
        public void Constructor_TopLeftPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(null, new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0),
                                                                     new MacroStabilityInwardsSlice.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLeftPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_TopRightPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(new Point2D(0, 0), null, new Point2D(0, 0), new Point2D(0, 0),
                                                                     new MacroStabilityInwardsSlice.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topRightPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_BottomLeftPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(new Point2D(0, 0), new Point2D(0, 0), null, new Point2D(0, 0),
                                                                     new MacroStabilityInwardsSlice.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("bottomLeftPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_BottomRightPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), null,
                                                                     new MacroStabilityInwardsSlice.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("bottomRightPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSlice(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithCoordinates_ExpectedValues()
        {
            // Setup
            var topLeftPoint = new Point2D(0, 5);
            var topRightPoint = new Point2D(5, 4);
            var bottomLeftPoint = new Point2D(0, 0);
            var bottomRightPoint = new Point2D(5, 1);

            // Call
            var slice = new MacroStabilityInwardsSlice(topLeftPoint, topRightPoint, bottomLeftPoint, bottomRightPoint,
                                                       new MacroStabilityInwardsSlice.ConstructionProperties());

            // Assert
            Assert.IsInstanceOf<ICloneable>(slice);

            Assert.AreSame(topLeftPoint, slice.TopLeftPoint);
            Assert.AreSame(topRightPoint, slice.TopRightPoint);
            Assert.AreSame(bottomLeftPoint, slice.BottomLeftPoint);
            Assert.AreSame(bottomRightPoint, slice.BottomRightPoint);

            Assert.AreEqual(2, slice.XCenter.NumberOfDecimalPlaces);
            Assert.AreEqual(2.5, slice.XCenter, slice.XCenter.GetAccuracy());

            Assert.AreEqual(2, slice.ZCenterBottom.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, slice.ZCenterBottom, slice.ZCenterBottom.GetAccuracy());

            Assert.AreEqual(2, slice.Width.NumberOfDecimalPlaces);
            Assert.AreEqual(5.0, slice.Width, slice.Width.GetAccuracy());

            Assert.AreEqual(2, slice.ArcLength.NumberOfDecimalPlaces);
            Assert.AreEqual(5.1, slice.ArcLength, slice.ArcLength.GetAccuracy());

            Assert.AreEqual(2, slice.BottomAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(11.31, slice.BottomAngle, slice.BottomAngle.GetAccuracy());

            Assert.AreEqual(2, slice.TopAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(-11.31, slice.TopAngle, slice.TopAngle.GetAccuracy());
        }

        [Test]
        public void Constructor_ConstructionPropertiesEmpty_ExpectedValues()
        {
            // Call
            var slice = new MacroStabilityInwardsSlice(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0),
                                                       new MacroStabilityInwardsSlice.ConstructionProperties());

            // Assert
            Assert.IsNaN(slice.Cohesion);
            Assert.IsNaN(slice.FrictionAngle);
            Assert.IsNaN(slice.CriticalPressure);
            Assert.IsNaN(slice.OverConsolidationRatio);
            Assert.IsNaN(slice.Pop);
            Assert.IsNaN(slice.DegreeOfConsolidationPorePressureSoil);
            Assert.IsNaN(slice.DegreeOfConsolidationPorePressureLoad);
            Assert.IsNaN(slice.Dilatancy);
            Assert.IsNaN(slice.ExternalLoad);
            Assert.IsNaN(slice.HydrostaticPorePressure);
            Assert.IsNaN(slice.LeftForce);
            Assert.IsNaN(slice.LeftForceAngle);
            Assert.IsNaN(slice.LeftForceY);
            Assert.IsNaN(slice.RightForce);
            Assert.IsNaN(slice.RightForceAngle);
            Assert.IsNaN(slice.RightForceY);
            Assert.IsNaN(slice.LoadStress);
            Assert.IsNaN(slice.NormalStress);
            Assert.IsNaN(slice.PorePressure);
            Assert.IsNaN(slice.HorizontalPorePressure);
            Assert.IsNaN(slice.VerticalPorePressure);
            Assert.IsNaN(slice.PiezometricPorePressure);
            Assert.IsNaN(slice.EffectiveStress);
            Assert.IsNaN(slice.EffectiveStressDaily);
            Assert.IsNaN(slice.ExcessPorePressure);
            Assert.IsNaN(slice.ShearStress);
            Assert.IsNaN(slice.SoilStress);
            Assert.IsNaN(slice.TotalPorePressure);
            Assert.IsNaN(slice.TotalStress);
            Assert.IsNaN(slice.Weight);
        }

        [Test]
        public void Constructor_ConstructionPropertiesSet_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double cohesion = random.NextDouble();
            double frictionAngle = random.NextDouble();
            double criticalPressure = random.NextDouble();
            double overConsolidationRatio = random.NextDouble();
            double pop = random.NextDouble();
            double degreeOfConsolidationPorePressureSoil = random.NextDouble();
            double degreeOfConsolidationPorePressureLoad = random.NextDouble();
            double dilatancy = random.NextDouble();
            double externalLoad = random.NextDouble();
            double hydrostaticPorePressure = random.NextDouble();
            double leftForce = random.NextDouble();
            double leftForceAngle = random.NextDouble();
            double leftForceY = random.NextDouble();
            double rightForce = random.NextDouble();
            double rightForceAngle = random.NextDouble();
            double rightForceY = random.NextDouble();
            double loadStress = random.NextDouble();
            double normalStress = random.NextDouble();
            double porePressure = random.NextDouble();
            double horizontalPorePressure = random.NextDouble();
            double verticalPorePressure = random.NextDouble();
            double piezometricPorePressure = random.NextDouble();
            double effectiveStress = random.NextDouble();
            double effectiveStressDaily = random.NextDouble();
            double excessPorePressure = random.NextDouble();
            double shearStress = random.NextDouble();
            double soilStress = random.NextDouble();
            double totalPorePressure = random.NextDouble();
            double totalStress = random.NextDouble();
            double weight = random.NextDouble();

            var properties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Cohesion = cohesion,
                FrictionAngle = frictionAngle,
                CriticalPressure = criticalPressure,
                OverConsolidationRatio = overConsolidationRatio,
                DegreeOfConsolidationPorePressureSoil = degreeOfConsolidationPorePressureSoil,
                DegreeOfConsolidationPorePressureLoad = degreeOfConsolidationPorePressureLoad,
                Pop = pop,
                Dilatancy = dilatancy,
                ExternalLoad = externalLoad,
                HydrostaticPorePressure = hydrostaticPorePressure,
                LeftForce = leftForce,
                LeftForceAngle = leftForceAngle,
                LeftForceY = leftForceY,
                RightForce = rightForce,
                RightForceAngle = rightForceAngle,
                RightForceY = rightForceY,
                LoadStress = loadStress,
                NormalStress = normalStress,
                PorePressure = porePressure,
                HorizontalPorePressure = horizontalPorePressure,
                VerticalPorePressure = verticalPorePressure,
                PiezometricPorePressure = piezometricPorePressure,
                EffectiveStress = effectiveStress,
                EffectiveStressDaily = effectiveStressDaily,
                ExcessPorePressure = excessPorePressure,
                ShearStress = shearStress,
                SoilStress = soilStress,
                TotalPorePressure = totalPorePressure,
                TotalStress = totalStress,
                Weight = weight
            };

            // Call
            var slice = new MacroStabilityInwardsSlice(new Point2D(0, 0), new Point2D(0, 0),
                                                       new Point2D(0, 0), new Point2D(0, 0),
                                                       properties);

            // Assert
            Assert.AreEqual(3, slice.Cohesion.NumberOfDecimalPlaces);
            Assert.AreEqual(cohesion, slice.Cohesion, slice.Cohesion.GetAccuracy());

            Assert.AreEqual(3, slice.FrictionAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(frictionAngle, slice.FrictionAngle, slice.FrictionAngle.GetAccuracy());

            Assert.AreEqual(3, slice.CriticalPressure.NumberOfDecimalPlaces);
            Assert.AreEqual(criticalPressure, slice.CriticalPressure, slice.CriticalPressure.GetAccuracy());

            Assert.AreEqual(3, slice.OverConsolidationRatio.NumberOfDecimalPlaces);
            Assert.AreEqual(overConsolidationRatio, slice.OverConsolidationRatio, slice.OverConsolidationRatio.GetAccuracy());

            Assert.AreEqual(3, slice.Pop.NumberOfDecimalPlaces);
            Assert.AreEqual(pop, slice.Pop, slice.Pop.GetAccuracy());

            Assert.AreEqual(3, slice.DegreeOfConsolidationPorePressureSoil.NumberOfDecimalPlaces);
            Assert.AreEqual(degreeOfConsolidationPorePressureSoil, slice.DegreeOfConsolidationPorePressureSoil, slice.DegreeOfConsolidationPorePressureSoil.GetAccuracy());

            Assert.AreEqual(3, slice.DegreeOfConsolidationPorePressureLoad.NumberOfDecimalPlaces);
            Assert.AreEqual(degreeOfConsolidationPorePressureLoad, slice.DegreeOfConsolidationPorePressureLoad, slice.DegreeOfConsolidationPorePressureLoad.GetAccuracy());

            Assert.AreEqual(3, slice.Dilatancy.NumberOfDecimalPlaces);
            Assert.AreEqual(dilatancy, slice.Dilatancy, slice.Dilatancy.GetAccuracy());

            Assert.AreEqual(3, slice.ExternalLoad.NumberOfDecimalPlaces);
            Assert.AreEqual(externalLoad, slice.ExternalLoad, slice.ExternalLoad.GetAccuracy());

            Assert.AreEqual(3, slice.HydrostaticPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(hydrostaticPorePressure, slice.HydrostaticPorePressure, slice.HydrostaticPorePressure.GetAccuracy());

            Assert.AreEqual(3, slice.LeftForce.NumberOfDecimalPlaces);
            Assert.AreEqual(leftForce, slice.LeftForce, slice.LeftForce.GetAccuracy());

            Assert.AreEqual(3, slice.LeftForceAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(leftForceAngle, slice.LeftForceAngle, slice.LeftForceAngle.GetAccuracy());

            Assert.AreEqual(3, slice.LeftForceY.NumberOfDecimalPlaces);
            Assert.AreEqual(leftForceY, slice.LeftForceY, slice.LeftForceY.GetAccuracy());

            Assert.AreEqual(3, slice.RightForce.NumberOfDecimalPlaces);
            Assert.AreEqual(rightForce, slice.RightForce, slice.RightForce.GetAccuracy());

            Assert.AreEqual(3, slice.RightForceAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(rightForceAngle, slice.RightForceAngle, slice.RightForceAngle.GetAccuracy());

            Assert.AreEqual(3, slice.RightForceY.NumberOfDecimalPlaces);
            Assert.AreEqual(rightForceY, slice.RightForceY, slice.RightForceY.GetAccuracy());

            Assert.AreEqual(3, slice.LoadStress.NumberOfDecimalPlaces);
            Assert.AreEqual(loadStress, slice.LoadStress, slice.LoadStress.GetAccuracy());

            Assert.AreEqual(3, slice.NormalStress.NumberOfDecimalPlaces);
            Assert.AreEqual(normalStress, slice.NormalStress, slice.NormalStress.GetAccuracy());

            Assert.AreEqual(3, slice.PorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(porePressure, slice.PorePressure, slice.PorePressure.GetAccuracy());

            Assert.AreEqual(3, slice.HorizontalPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(horizontalPorePressure, slice.HorizontalPorePressure, slice.HorizontalPorePressure.GetAccuracy());

            Assert.AreEqual(3, slice.VerticalPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(verticalPorePressure, slice.VerticalPorePressure, slice.VerticalPorePressure.GetAccuracy());

            Assert.AreEqual(3, slice.PiezometricPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(piezometricPorePressure, slice.PiezometricPorePressure, slice.PiezometricPorePressure.GetAccuracy());

            Assert.AreEqual(3, slice.EffectiveStress.NumberOfDecimalPlaces);
            Assert.AreEqual(effectiveStress, slice.EffectiveStress, slice.EffectiveStress.GetAccuracy());

            Assert.AreEqual(3, slice.EffectiveStressDaily.NumberOfDecimalPlaces);
            Assert.AreEqual(effectiveStressDaily, slice.EffectiveStressDaily, slice.EffectiveStressDaily.GetAccuracy());

            Assert.AreEqual(3, slice.ExcessPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(excessPorePressure, slice.ExcessPorePressure, slice.ExcessPorePressure.GetAccuracy());

            Assert.AreEqual(3, slice.ShearStress.NumberOfDecimalPlaces);
            Assert.AreEqual(shearStress, slice.ShearStress, slice.ShearStress.GetAccuracy());

            Assert.AreEqual(3, slice.SoilStress.NumberOfDecimalPlaces);
            Assert.AreEqual(soilStress, slice.SoilStress, slice.SoilStress.GetAccuracy());

            Assert.AreEqual(3, slice.TotalPorePressure.NumberOfDecimalPlaces);
            Assert.AreEqual(totalPorePressure, slice.TotalPorePressure, slice.TotalPorePressure.GetAccuracy());

            Assert.AreEqual(3, slice.TotalStress.NumberOfDecimalPlaces);
            Assert.AreEqual(totalStress, slice.TotalStress, slice.TotalStress.GetAccuracy());

            Assert.AreEqual(3, slice.Weight.NumberOfDecimalPlaces);
            Assert.AreEqual(weight, slice.Weight, slice.Weight.GetAccuracy());
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var properties = new MacroStabilityInwardsSlice.ConstructionProperties
            {
                Cohesion = random.NextDouble(),
                FrictionAngle = random.NextDouble(),
                CriticalPressure = random.NextDouble(),
                OverConsolidationRatio = random.NextDouble(),
                DegreeOfConsolidationPorePressureSoil = random.NextDouble(),
                DegreeOfConsolidationPorePressureLoad = random.NextDouble(),
                Pop = random.NextDouble(),
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
            };

            var original = new MacroStabilityInwardsSlice(CreateRandomPoint(random),
                                                          CreateRandomPoint(random),
                                                          CreateRandomPoint(random),
                                                          CreateRandomPoint(random),
                                                          properties);

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        private static Point2D CreateRandomPoint(Random random)
        {
            return new Point2D(random.NextDouble(), random.NextDouble());
        }
    }
}