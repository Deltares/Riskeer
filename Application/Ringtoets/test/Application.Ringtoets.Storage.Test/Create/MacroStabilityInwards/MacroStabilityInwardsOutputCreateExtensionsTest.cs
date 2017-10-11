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
using System.Collections.Generic;
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.TestUtil.MacroStabilityInwards;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsOutputCreateExtensionsTest
    {
        [Test]
        public void Create_OutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((MacroStabilityInwardsOutput) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void Create_WithValidValues_ReturnsPropertyWithExpectedPropertiesSet()
        {
            // Setup
            var random = new Random(21);

            MacroStabilityInwardsSlidingCircle leftCircle = CreateSlidingCircle(13);
            MacroStabilityInwardsSlidingCircle rightCircle = CreateSlidingCircle(34);
            IEnumerable<MacroStabilityInwardsSlice> slices = CreateMacroStabilityInwardsSlices();
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(leftCircle,
                                                                     rightCircle,
                                                                     slices,
                                                                     random.NextDouble(),
                                                                     random.NextDouble());

            MacroStabilityInwardsGrid leftGrid = CreateGrid(13);
            MacroStabilityInwardsGrid rightGrid = CreateGrid(34);
            var tangentLines = new[]
            {
                random.NextDouble()
            };
            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(leftGrid, rightGrid, tangentLines);

            var output = new MacroStabilityInwardsOutput(slidingCurve, slipPlane, new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = random.NextDouble(),
                ForbiddenZonesXEntryMax = random.NextDouble(),
                ForbiddenZonesXEntryMin = random.NextDouble(),
                ZValue = random.NextDouble(),
            });

            // Call
            MacroStabilityInwardsCalculationOutputEntity entity = output.Create();

            // Assert
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(output, entity);
        }

        [Test]
        public void Create_WithNaNValues_ReturnsPropertyWithExpectedPropertiesSet()
        {
            // Setup
            MacroStabilityInwardsSlidingCircle leftCircle = CreateSlidingCircleWithNaNValues();
            MacroStabilityInwardsSlidingCircle rightCircle = CreateSlidingCircleWithNaNValues();
            IEnumerable<MacroStabilityInwardsSlice> slices = CreateMacroStabilityInwardsSlices();
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(leftCircle,
                                                                     rightCircle,
                                                                     slices,
                                                                     double.NaN,
                                                                     double.NaN);

            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(new MacroStabilityInwardsGrid(),
                                                                        new MacroStabilityInwardsGrid(),
                                                                        new[]
                                                                        {
                                                                            double.NaN
                                                                        });

            var output = new MacroStabilityInwardsOutput(slidingCurve, slipPlane, new MacroStabilityInwardsOutput.ConstructionProperties());

            // Call
            MacroStabilityInwardsCalculationOutputEntity entity = output.Create();

            // Assert
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(output, entity);
        }

        #region Slip Plane Helpers

        private static MacroStabilityInwardsGrid CreateGrid(int seed)
        {
            var random = new Random(seed);
            return new MacroStabilityInwardsGrid
            {
                XLeft = random.NextRoundedDouble(),
                XRight = random.NextRoundedDouble(),
                ZTop = random.NextRoundedDouble(),
                ZBottom = random.NextRoundedDouble(),
                NumberOfHorizontalPoints = random.Next(),
                NumberOfVerticalPoints = random.Next()
            };
        }

        #endregion

        #region Sliding Curve Helpers

        private static MacroStabilityInwardsSlidingCircle CreateSlidingCircle(int seed)
        {
            var random = new Random(seed);
            return new MacroStabilityInwardsSlidingCircle(new Point2D(random.NextDouble(), random.NextDouble()),
                                                          random.NextDouble(),
                                                          random.NextBoolean(),
                                                          random.NextDouble(),
                                                          random.NextDouble(),
                                                          random.NextDouble(),
                                                          random.NextDouble());
        }

        private static MacroStabilityInwardsSlidingCircle CreateSlidingCircleWithNaNValues()
        {
            var random = new Random(21);
            return new MacroStabilityInwardsSlidingCircle(new Point2D(double.NaN, double.NaN),
                                                          double.NaN,
                                                          random.NextBoolean(),
                                                          double.NaN,
                                                          double.NaN,
                                                          double.NaN,
                                                          double.NaN);
        }

        private static IEnumerable<MacroStabilityInwardsSlice> CreateMacroStabilityInwardsSlices()
        {
            var random = new Random(21);
            return new[]
            {
                new MacroStabilityInwardsSlice(new Point2D(random.NextDouble(), random.NextDouble()),
                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                               new Point2D(random.NextDouble(), random.NextDouble()),
                                               new MacroStabilityInwardsSlice.ConstructionProperties
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
                                                   TotalStress = random.NextDouble()
                                               })
            };
        }

        #endregion
    }
}