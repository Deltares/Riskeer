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
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.TestUtil.MacroStabilityInwards
{
    /// <summary>
    /// Class to assert the properties of macro stability inwards calculation output entities.
    /// </summary>
    public static class MacroStabilityInwardsCalculationOutputEntityTestHelper
    {
        /// <summary>
        /// Determines for each property of <paramref name="entity"/> whether the matching 
        /// property of <paramref name="output"/> has an equal value.
        /// </summary>
        /// <param name="output">The <see cref="MacroStabilityInwardsOutput"/> to compare.</param>
        /// <param name="entity">The <see cref="MacroStabilityInwardsCalculationOutputEntity"/>
        /// to compare.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the argument is <c>null</c>.</exception>
        /// <exception cref="AssertionException">Thrown when any of the values of the 
        /// <see cref="MacroStabilityInwardsOutput"/> and its nested elements do not match.
        /// </exception>
        public static void AssertOutputPropertyValues(MacroStabilityInwardsOutput output,
                                                      MacroStabilityInwardsCalculationOutputEntity entity)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            AssertAreEqual(output.FactorOfStability, entity.FactorOfStability);
            AssertAreEqual(output.ForbiddenZonesXEntryMin, entity.ForbiddenZonesXEntryMin);
            AssertAreEqual(output.ForbiddenZonesXEntryMax, entity.ForbiddenZonesXEntryMax);

            AssertSlidingCurveProperties(output.SlidingCurve, entity);
            AssertSlipPlaneProperties(output.SlipPlane, entity);
        }

        private static void AssertSlipPlaneProperties(MacroStabilityInwardsSlipPlaneUpliftVan slipPlane,
                                                      MacroStabilityInwardsCalculationOutputEntity entity)
        {
            string expectedTangentLinesXml = new TangentLineCollectionXmlSerializer().ToXml(slipPlane.TangentLines);
            Assert.AreEqual(expectedTangentLinesXml, entity.SlipPlaneTangentLinesXml);

            MacroStabilityInwardsGrid leftGrid = slipPlane.LeftGrid;
            AssertAreEqual(leftGrid.XLeft, entity.SlipPlaneLeftGridXLeft);
            AssertAreEqual(leftGrid.XRight, entity.SlipPlaneLeftGridXRight);
            Assert.AreEqual(leftGrid.NumberOfHorizontalPoints, entity.SlipPlaneLeftGridNrOfHorizontalPoints);
            AssertAreEqual(leftGrid.ZTop, entity.SlipPlaneLeftGridZTop);
            AssertAreEqual(leftGrid.ZBottom, entity.SlipPlaneLeftGridZBottom);
            Assert.AreEqual(leftGrid.NumberOfVerticalPoints, entity.SlipPlaneLeftGridNrOfVerticalPoints);

            MacroStabilityInwardsGrid rightGrid = slipPlane.RightGrid;
            AssertAreEqual(rightGrid.XLeft, entity.SlipPlaneRightGridXLeft);
            AssertAreEqual(rightGrid.XRight, entity.SlipPlaneRightGridXRight);
            Assert.AreEqual(rightGrid.NumberOfHorizontalPoints, entity.SlipPlaneRightGridNrOfHorizontalPoints);
            AssertAreEqual(rightGrid.ZTop, entity.SlipPlaneRightGridZTop);
            AssertAreEqual(rightGrid.ZBottom, entity.SlipPlaneRightGridZBottom);
            Assert.AreEqual(rightGrid.NumberOfVerticalPoints, entity.SlipPlaneRightGridNrOfVerticalPoints);
        }

        private static void AssertSlidingCurveProperties(MacroStabilityInwardsSlidingCurve slidingCurve,
                                                         MacroStabilityInwardsCalculationOutputEntity entity)
        {
            string expectedSlicesXml = new MacroStabilityInwardsSliceCollectionXmlSerializer().ToXml(slidingCurve.Slices);
            Assert.AreEqual(expectedSlicesXml, entity.SlidingCurveSliceXML);

            AssertAreEqual(slidingCurve.IteratedHorizontalForce, entity.SlidingCurveIteratedHorizontalForce);
            AssertAreEqual(slidingCurve.NonIteratedHorizontalForce, entity.SlidingCurveNonIteratedHorizontalForce);

            MacroStabilityInwardsSlidingCircle leftCircle = slidingCurve.LeftCircle;
            AssertAreEqual(leftCircle.Center.X, entity.SlidingCurveLeftSlidingCircleCenterX);
            AssertAreEqual(leftCircle.Center.Y, entity.SlidingCurveLeftSlidingCircleCenterY);
            AssertAreEqual(leftCircle.Radius, entity.SlidingCurveLeftSlidingCircleRadius);
            Assert.AreEqual(Convert.ToByte(leftCircle.IsActive), entity.SlidingCurveLeftSlidingCircleIsActive);
            AssertAreEqual(leftCircle.IteratedForce, entity.SlidingCurveLeftSlidingCircleIteratedForce);
            AssertAreEqual(leftCircle.NonIteratedForce, entity.SlidingCurveLeftSlidingCircleNonIteratedForce);
            AssertAreEqual(leftCircle.DrivingMoment, entity.SlidingCurveLeftSlidingCircleDrivingMoment);
            AssertAreEqual(leftCircle.ResistingMoment, entity.SlidingCurveLeftSlidingCircleResistingMoment);

            MacroStabilityInwardsSlidingCircle rightCircle = slidingCurve.RightCircle;
            AssertAreEqual(rightCircle.Center.X, entity.SlidingCurveRightSlidingCircleCenterX);
            AssertAreEqual(rightCircle.Center.Y, entity.SlidingCurveRightSlidingCircleCenterY);
            AssertAreEqual(rightCircle.Radius, entity.SlidingCurveRightSlidingCircleRadius);
            Assert.AreEqual(Convert.ToByte(rightCircle.IsActive), entity.SlidingCurveRightSlidingCircleIsActive);
            AssertAreEqual(rightCircle.IteratedForce, entity.SlidingCurveRightSlidingCircleIteratedForce);
            AssertAreEqual(rightCircle.NonIteratedForce, entity.SlidingCurveRightSlidingCircleNonIteratedForce);
            AssertAreEqual(rightCircle.DrivingMoment, entity.SlidingCurveRightSlidingCircleDrivingMoment);
            AssertAreEqual(rightCircle.ResistingMoment, entity.SlidingCurveRightSlidingCircleResistingMoment);
        }

        private static void AssertAreEqual(double expectedDouble, double? actualDouble)
        {
            if (double.IsNaN(expectedDouble))
            {
                Assert.IsNull(actualDouble);
            }
            else
            {
                Assert.AreEqual(expectedDouble, actualDouble);
            }
        }

        private static void AssertAreEqual(RoundedDouble expectedDouble, double? actualDouble)
        {
            if (double.IsNaN(expectedDouble))
            {
                Assert.IsNull(actualDouble);
            }
            else
            {
                Assert.AreEqual(expectedDouble, actualDouble, expectedDouble.GetAccuracy());
            }
        }
    }
}