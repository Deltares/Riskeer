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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.TestUtil.MacroStabilityInwards;

namespace Riskeer.Storage.Core.Test.Create.MacroStabilityInwards
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
        public void Create_WithValidValues_ReturnsEntityWithExpectedPropertiesSet()
        {
            // Setup
            var random = new Random(21);

            MacroStabilityInwardsSlidingCircle leftCircle = CreateSlidingCircle(13);
            MacroStabilityInwardsSlidingCircle rightCircle = CreateSlidingCircle(34);
            IEnumerable<MacroStabilityInwardsSlice> slices = new[]
            {
                MacroStabilityInwardsSliceTestFactory.CreateSlice()
            };
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(leftCircle,
                                                                     rightCircle,
                                                                     slices,
                                                                     random.NextDouble(),
                                                                     random.NextDouble());

            MacroStabilityInwardsGrid leftGrid = MacroStabilityInwardsGridTestFactory.Create();
            MacroStabilityInwardsGrid rightGrid = MacroStabilityInwardsGridTestFactory.Create();
            RoundedDouble[] tangentLines =
            {
                random.NextRoundedDouble()
            };
            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(leftGrid, rightGrid, tangentLines);

            var output = new MacroStabilityInwardsOutput(slidingCurve, slipPlane, new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = random.NextDouble(),
                ForbiddenZonesXEntryMax = random.NextDouble(),
                ForbiddenZonesXEntryMin = random.NextDouble()
            });

            // Call
            MacroStabilityInwardsCalculationOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            MacroStabilityInwardsCalculationOutputEntityTestHelper.AssertOutputPropertyValues(output, entity);
        }

        [Test]
        public void Create_WithNaNValues_ReturnsPropertyWithExpectedPropertiesSet()
        {
            // Setup
            MacroStabilityInwardsSlidingCircle leftCircle = CreateSlidingCircleWithNaNValues();
            MacroStabilityInwardsSlidingCircle rightCircle = CreateSlidingCircleWithNaNValues();
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(leftCircle,
                                                                     rightCircle,
                                                                     new MacroStabilityInwardsSlice[0],
                                                                     double.NaN,
                                                                     double.NaN);

            var slipPlane = new MacroStabilityInwardsSlipPlaneUpliftVan(CreateGridWithNaNValues(),
                                                                        CreateGridWithNaNValues(),
                                                                        new RoundedDouble[0]);

            var output = new MacroStabilityInwardsOutput(slidingCurve, slipPlane, new MacroStabilityInwardsOutput.ConstructionProperties());

            // Call
            MacroStabilityInwardsCalculationOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);
            Assert.IsNull(entity.FactorOfStability);
            Assert.IsNull(entity.ForbiddenZonesXEntryMin);
            Assert.IsNull(entity.ForbiddenZonesXEntryMax);
            Assert.IsNull(entity.ZValue);

            Assert.IsNull(entity.SlipPlaneLeftGridXLeft);
            Assert.IsNull(entity.SlipPlaneLeftGridXRight);
            Assert.IsNull(entity.SlipPlaneLeftGridZTop);
            Assert.IsNull(entity.SlipPlaneLeftGridZBottom);

            Assert.IsNull(entity.SlipPlaneRightGridXLeft);
            Assert.IsNull(entity.SlipPlaneRightGridXRight);
            Assert.IsNull(entity.SlipPlaneRightGridZTop);
            Assert.IsNull(entity.SlipPlaneRightGridZBottom);

            Assert.IsNull(entity.SlidingCurveIteratedHorizontalForce);
            Assert.IsNull(entity.SlidingCurveNonIteratedHorizontalForce);

            Assert.IsNull(entity.SlidingCurveLeftSlidingCircleCenterX);
            Assert.IsNull(entity.SlidingCurveLeftSlidingCircleCenterY);
            Assert.IsNull(entity.SlidingCurveLeftSlidingCircleRadius);
            Assert.IsNull(entity.SlidingCurveLeftSlidingCircleIteratedForce);
            Assert.IsNull(entity.SlidingCurveLeftSlidingCircleNonIteratedForce);
            Assert.IsNull(entity.SlidingCurveLeftSlidingCircleDrivingMoment);
            Assert.IsNull(entity.SlidingCurveLeftSlidingCircleResistingMoment);

            Assert.IsNull(entity.SlidingCurveRightSlidingCircleCenterX);
            Assert.IsNull(entity.SlidingCurveRightSlidingCircleCenterY);
            Assert.IsNull(entity.SlidingCurveRightSlidingCircleRadius);
            Assert.IsNull(entity.SlidingCurveRightSlidingCircleIteratedForce);
            Assert.IsNull(entity.SlidingCurveRightSlidingCircleNonIteratedForce);
            Assert.IsNull(entity.SlidingCurveRightSlidingCircleDrivingMoment);
            Assert.IsNull(entity.SlidingCurveRightSlidingCircleResistingMoment);
        }

        #region Slip Plane Helpers

        private static MacroStabilityInwardsGrid CreateGridWithNaNValues()
        {
            return new MacroStabilityInwardsGrid(double.NaN, double.NaN, double.NaN, double.NaN);
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

        #endregion
    }
}