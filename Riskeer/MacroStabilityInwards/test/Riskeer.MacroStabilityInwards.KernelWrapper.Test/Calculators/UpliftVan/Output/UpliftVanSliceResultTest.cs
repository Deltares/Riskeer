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
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan.Output
{
    [TestFixture]
    public class UpliftVanSliceResultTest
    {
        [Test]
        public void Constructor_TopLeftPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new UpliftVanSliceResult(null, new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0),
                                                               new UpliftVanSliceResult.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topLeftPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_TopRightPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new UpliftVanSliceResult(new Point2D(0, 0), null, new Point2D(0, 0), new Point2D(0, 0),
                                                               new UpliftVanSliceResult.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("topRightPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_BottomLeftPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new UpliftVanSliceResult(new Point2D(0, 0), new Point2D(0, 0), null, new Point2D(0, 0),
                                                               new UpliftVanSliceResult.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("bottomLeftPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_BottomRightPointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new UpliftVanSliceResult(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), null,
                                                               new UpliftVanSliceResult.ConstructionProperties());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("bottomRightPoint", exception.ParamName);
        }

        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new UpliftVanSliceResult(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithCoordinates_ExpectedValues()
        {
            // Setup
            var topLeftPoint = new Point2D(0, 0);
            var topRightPoint = new Point2D(1, 1);
            var bottomLeftPoint = new Point2D(2, 2);
            var bottomRightPoint = new Point2D(3, 3);

            // Call
            var slice = new UpliftVanSliceResult(topLeftPoint, topRightPoint, bottomLeftPoint, bottomRightPoint,
                                                 new UpliftVanSliceResult.ConstructionProperties());

            // Assert
            Assert.AreSame(topLeftPoint, slice.TopLeftPoint);
            Assert.AreSame(topRightPoint, slice.TopRightPoint);
            Assert.AreSame(bottomLeftPoint, slice.BottomLeftPoint);
            Assert.AreSame(bottomRightPoint, slice.BottomRightPoint);
        }

        [Test]
        public void Constructor_ConstructionPropertiesEmpty_ExpectedValues()
        {
            // Call
            var slice = new UpliftVanSliceResult(new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0), new Point2D(0, 0),
                                                 new UpliftVanSliceResult.ConstructionProperties());

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

            var properties = new UpliftVanSliceResult.ConstructionProperties
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
            var slice = new UpliftVanSliceResult(new Point2D(0, 0), new Point2D(0, 0),
                                                 new Point2D(0, 0), new Point2D(0, 0),
                                                 properties);

            // Assert
            Assert.AreEqual(cohesion, slice.Cohesion);
            Assert.AreEqual(frictionAngle, slice.FrictionAngle);
            Assert.AreEqual(criticalPressure, slice.CriticalPressure);
            Assert.AreEqual(overConsolidationRatio, slice.OverConsolidationRatio);
            Assert.AreEqual(pop, slice.Pop);
            Assert.AreEqual(degreeOfConsolidationPorePressureSoil, slice.DegreeOfConsolidationPorePressureSoil);
            Assert.AreEqual(degreeOfConsolidationPorePressureLoad, slice.DegreeOfConsolidationPorePressureLoad);
            Assert.AreEqual(dilatancy, slice.Dilatancy);
            Assert.AreEqual(externalLoad, slice.ExternalLoad);
            Assert.AreEqual(hydrostaticPorePressure, slice.HydrostaticPorePressure);
            Assert.AreEqual(leftForce, slice.LeftForce);
            Assert.AreEqual(leftForceAngle, slice.LeftForceAngle);
            Assert.AreEqual(leftForceY, slice.LeftForceY);
            Assert.AreEqual(rightForce, slice.RightForce);
            Assert.AreEqual(rightForceAngle, slice.RightForceAngle);
            Assert.AreEqual(rightForceY, slice.RightForceY);
            Assert.AreEqual(loadStress, slice.LoadStress);
            Assert.AreEqual(normalStress, slice.NormalStress);
            Assert.AreEqual(porePressure, slice.PorePressure);
            Assert.AreEqual(horizontalPorePressure, slice.HorizontalPorePressure);
            Assert.AreEqual(verticalPorePressure, slice.VerticalPorePressure);
            Assert.AreEqual(piezometricPorePressure, slice.PiezometricPorePressure);
            Assert.AreEqual(effectiveStress, slice.EffectiveStress);
            Assert.AreEqual(effectiveStressDaily, slice.EffectiveStressDaily);
            Assert.AreEqual(excessPorePressure, slice.ExcessPorePressure);
            Assert.AreEqual(shearStress, slice.ShearStress);
            Assert.AreEqual(soilStress, slice.SoilStress);
            Assert.AreEqual(totalPorePressure, slice.TotalPorePressure);
            Assert.AreEqual(totalStress, slice.TotalStress);
            Assert.AreEqual(weight, slice.Weight);
        }
    }
}