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
using System.Linq;
using Deltares.MacroStability.Data;
using Deltares.MacroStability.Geometry;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Output
{
    [TestFixture]
    public class UpliftVanSlidingCurveResultCreatorTest
    {
        [Test]
        public void Create_SlidingCurveNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanSlidingCurveResultCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("slidingCurve", exception.ParamName);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Create_WithSlidingCurve_ReturnSlidingCurveResult(bool leftCircleIsActive)
        {
            // Setup
            var random = new Random(21);
            double activeCircleX = random.Next();
            double activeCircleZ = random.Next();

            double activeCircleIteratedForce = random.Next();
            double activeCircleNonIteratedForce = random.Next();
            double activeCircleRadius = random.Next();
            double activeCircleDrivingMoment = random.Next();
            double activeCircleResistingMoment = random.Next();

            double passiveCircleX = random.Next();
            double passiveCircleZ = random.Next();
            double passiveCircleIteratedForce = random.Next();
            double passiveCircleNonIteratedForce = random.Next();
            double passiveCircleRadius = random.Next();
            double passiveCircleDrivingMoment = random.Next();
            double passiveCircleResistingMoment = random.Next();

            double iteratedHorizontalForce = random.Next();
            double nonIteratedHorizontalForce = random.Next();

            var slidingCurve = new SlidingDualCircle
            {
                ActiveCircle = new GeometryPoint(activeCircleX, activeCircleZ),
                ActiveForce = activeCircleIteratedForce,
                ActiveForce0 = activeCircleNonIteratedForce,
                ActiveRadius = activeCircleRadius,
                DrivingMomentActive = activeCircleDrivingMoment,
                ResistingMomentActive = activeCircleResistingMoment,
                PassiveCircle = new GeometryPoint(passiveCircleX, passiveCircleZ),
                PassiveForce = passiveCircleIteratedForce,
                PassiveForce0 = passiveCircleNonIteratedForce,
                PassiveRadius = passiveCircleRadius,
                DrivingMomentPassive = passiveCircleDrivingMoment,
                ResistingMomentPassive = passiveCircleResistingMoment,
                HorizontalForce = iteratedHorizontalForce,
                HorizontalForce0 = nonIteratedHorizontalForce
            };

            // Call
            UpliftVanSlidingCurveResult result = UpliftVanSlidingCurveResultCreator.Create(slidingCurve);

            // Assert
            AssertActiveCircle(leftCircleIsActive ? result.LeftCircle : result.RightCircle,
                               activeCircleX, activeCircleZ, activeCircleIteratedForce,
                               activeCircleNonIteratedForce, activeCircleRadius,
                               activeCircleDrivingMoment, activeCircleResistingMoment);

            AssertPassiveCircle(leftCircleIsActive ? result.RightCircle : result.LeftCircle,
                                passiveCircleX, passiveCircleZ, passiveCircleIteratedForce,
                                passiveCircleNonIteratedForce, passiveCircleRadius,
                                passiveCircleDrivingMoment, passiveCircleResistingMoment);

            Assert.AreEqual(iteratedHorizontalForce, result.IteratedHorizontalForce);
            Assert.AreEqual(nonIteratedHorizontalForce, result.NonIteratedHorizontalForce);
        }

        [Test]
        public void Create_SlidingCurveWithSlices_ReturnSlidingCurveResult()
        {
            // Setup
            var random = new Random(21);
            double topLeftX = random.NextDouble();
            double topLeftZ = random.NextDouble();
            double topRightX = random.NextDouble();
            double topRightZ = random.NextDouble();
            double bottomLeftX = random.NextDouble();
            double bottomLeftZ = random.NextDouble();
            double bottomRightX = random.NextDouble();
            double bottomRightZ = random.NextDouble();
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

            var slidingCurve = new SlidingDualCircle
            {
                Slices =
                {
                    new Slice
                    {
                        TopLeftX = topLeftX,
                        TopLeftZ = topLeftZ,
                        TopRightX = topRightX,
                        TopRightZ = topRightZ,
                        BottomLeftX = bottomLeftX,
                        BottomLeftZ = bottomLeftZ,
                        BottomRightX = bottomRightX,
                        BottomRightZ = bottomRightZ,
                        Cohesion = cohesion,
                        Phi = frictionAngle,
                        PGrens = criticalPressure,
                        OCR = overConsolidationRatio,
                        POP = pop,
                        DegreeofConsolidationPorePressure = degreeOfConsolidationPorePressureSoil,
                        PorePressureDueToDegreeOfConsolidationLoad = degreeOfConsolidationPorePressureLoad,
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
                        PoreOnSurface = porePressure,
                        HPoreOnSurface = horizontalPorePressure,
                        VPoreOnSurface = verticalPorePressure,
                        PiezometricPorePressure = piezometricPorePressure,
                        EffectiveStress = effectiveStress,
                        EffectiveStressDaily = effectiveStressDaily,
                        ExcessPorePressure = excessPorePressure,
                        ShearStress = shearStress,
                        SoilStress = soilStress,
                        TotalPorePressure = totalPorePressure,
                        TotalStress = totalStress,
                        Weight = weight
                    }
                }
            };

            // Call
            UpliftVanSlidingCurveResult result = UpliftVanSlidingCurveResultCreator.Create(slidingCurve);

            // Assert
            Assert.AreEqual(1, result.Slices.Count());

            UpliftVanSliceResult slice = result.Slices.First();
            Assert.AreEqual(new Point2D(topLeftX, topLeftZ), slice.TopLeftPoint);
            Assert.AreEqual(new Point2D(topRightX, topRightZ), slice.TopRightPoint);
            Assert.AreEqual(new Point2D(bottomLeftX, bottomLeftZ), slice.BottomLeftPoint);
            Assert.AreEqual(new Point2D(bottomRightX, bottomRightZ), slice.BottomRightPoint);

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

        private static void AssertActiveCircle(UpliftVanSlidingCircleResult circle, double x, double z, double iteratedForce,
                                               double nonIteratedForce, double radius, double drivingMoment, double resistingMoment)
        {
            Assert.IsTrue(circle.IsActive);
            AssertCircle(circle, x, z, iteratedForce, nonIteratedForce, radius, drivingMoment, resistingMoment);
        }

        private static void AssertPassiveCircle(UpliftVanSlidingCircleResult circle, double x, double z, double iteratedForce,
                                                double nonIteratedForce, double radius, double drivingMoment, double resistingMoment)
        {
            Assert.IsFalse(circle.IsActive);
            AssertCircle(circle, x, z, iteratedForce, nonIteratedForce, radius, drivingMoment, resistingMoment);
        }

        private static void AssertCircle(UpliftVanSlidingCircleResult circle, double x, double z, double iteratedForce,
                                         double nonIteratedForce, double radius, double drivingMoment, double resistingMoment)
        {
            Assert.AreEqual(x, circle.Center.X);
            Assert.AreEqual(z, circle.Center.Y);
            Assert.AreEqual(iteratedForce, circle.IteratedForce);
            Assert.AreEqual(nonIteratedForce, circle.NonIteratedForce);
            Assert.AreEqual(radius, circle.Radius);
            Assert.AreEqual(drivingMoment, circle.DrivingMoment);
            Assert.AreEqual(resistingMoment, circle.ResistingMoment);
        }
    }
}