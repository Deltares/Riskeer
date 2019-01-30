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
using Core.Common.Base.Geometry;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="MacroStabilityInwardsOutput"/>
    /// based on the <see cref="MacroStabilityInwardsCalculationOutputEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsCalculationOutputEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="MacroStabilityInwardsCalculationOutputEntity"/> and use the information to
        /// construct a <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsCalculationOutputEntity"/> to create
        /// <see cref="MacroStabilityInwardsOutput"/> for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsOutput Read(this MacroStabilityInwardsCalculationOutputEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            MacroStabilityInwardsSlipPlaneUpliftVan slipPlane = ReadSlipPlane(entity);
            MacroStabilityInwardsSlidingCurve slidingCurve = ReadSlidingCurve(entity);

            return new MacroStabilityInwardsOutput(slidingCurve, slipPlane, new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = entity.FactorOfStability.ToNullAsNaN(),
                ForbiddenZonesXEntryMax = entity.ForbiddenZonesXEntryMax.ToNullAsNaN(),
                ForbiddenZonesXEntryMin = entity.ForbiddenZonesXEntryMin.ToNullAsNaN(),
                ZValue = entity.ZValue.ToNullAsNaN()
            });
        }

        private static MacroStabilityInwardsSlipPlaneUpliftVan ReadSlipPlane(MacroStabilityInwardsCalculationOutputEntity entity)
        {
            var leftGrid = new MacroStabilityInwardsGrid(entity.SlipPlaneLeftGridXLeft.ToNullAsNaN(),
                                                         entity.SlipPlaneLeftGridXRight.ToNullAsNaN(),
                                                         entity.SlipPlaneLeftGridZTop.ToNullAsNaN(),
                                                         entity.SlipPlaneLeftGridZBottom.ToNullAsNaN())
            {
                NumberOfHorizontalPoints = entity.SlipPlaneLeftGridNrOfHorizontalPoints,
                NumberOfVerticalPoints = entity.SlipPlaneLeftGridNrOfVerticalPoints
            };
            var rightGrid = new MacroStabilityInwardsGrid(entity.SlipPlaneRightGridXLeft.ToNullAsNaN(),
                                                          entity.SlipPlaneRightGridXRight.ToNullAsNaN(),
                                                          entity.SlipPlaneRightGridZTop.ToNullAsNaN(),
                                                          entity.SlipPlaneRightGridZBottom.ToNullAsNaN()
            )
            {
                NumberOfHorizontalPoints = entity.SlipPlaneRightGridNrOfHorizontalPoints,
                NumberOfVerticalPoints = entity.SlipPlaneRightGridNrOfVerticalPoints
            };

            RoundedDouble[] tangentLines = new TangentLineCollectionXmlSerializer().FromXml(entity.SlipPlaneTangentLinesXml);
            return new MacroStabilityInwardsSlipPlaneUpliftVan(leftGrid, rightGrid, tangentLines);
        }

        private static MacroStabilityInwardsSlidingCurve ReadSlidingCurve(MacroStabilityInwardsCalculationOutputEntity entity)
        {
            var leftCircle = new MacroStabilityInwardsSlidingCircle(new Point2D(entity.SlidingCurveLeftSlidingCircleCenterX.ToNullAsNaN(),
                                                                                entity.SlidingCurveLeftSlidingCircleCenterY.ToNullAsNaN()),
                                                                    entity.SlidingCurveLeftSlidingCircleRadius.ToNullAsNaN(),
                                                                    Convert.ToBoolean(entity.SlidingCurveLeftSlidingCircleIsActive),
                                                                    entity.SlidingCurveLeftSlidingCircleNonIteratedForce.ToNullAsNaN(),
                                                                    entity.SlidingCurveLeftSlidingCircleIteratedForce.ToNullAsNaN(),
                                                                    entity.SlidingCurveLeftSlidingCircleDrivingMoment.ToNullAsNaN(),
                                                                    entity.SlidingCurveLeftSlidingCircleResistingMoment.ToNullAsNaN());

            var rightCircle = new MacroStabilityInwardsSlidingCircle(new Point2D(entity.SlidingCurveRightSlidingCircleCenterX.ToNullAsNaN(),
                                                                                 entity.SlidingCurveRightSlidingCircleCenterY.ToNullAsNaN()),
                                                                     entity.SlidingCurveRightSlidingCircleRadius.ToNullAsNaN(),
                                                                     Convert.ToBoolean(entity.SlidingCurveRightSlidingCircleIsActive),
                                                                     entity.SlidingCurveRightSlidingCircleNonIteratedForce.ToNullAsNaN(),
                                                                     entity.SlidingCurveRightSlidingCircleIteratedForce.ToNullAsNaN(),
                                                                     entity.SlidingCurveRightSlidingCircleDrivingMoment.ToNullAsNaN(),
                                                                     entity.SlidingCurveRightSlidingCircleResistingMoment.ToNullAsNaN());

            MacroStabilityInwardsSlice[] slices = new MacroStabilityInwardsSliceCollectionXmlSerializer().FromXml(entity.SlidingCurveSliceXML);
            return new MacroStabilityInwardsSlidingCurve(leftCircle,
                                                         rightCircle,
                                                         slices,
                                                         entity.SlidingCurveNonIteratedHorizontalForce.ToNullAsNaN(),
                                                         entity.SlidingCurveIteratedHorizontalForce.ToNullAsNaN());
        }
    }
}