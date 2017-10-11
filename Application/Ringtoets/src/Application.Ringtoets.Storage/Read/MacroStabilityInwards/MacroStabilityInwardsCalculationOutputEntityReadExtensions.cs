﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.MacroStabilityInwards.Data;

namespace Application.Ringtoets.Storage.Read.MacroStabilityInwards
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
        internal static MacroStabilityInwardsOutput Read(this MacroStabilityInwardsCalculationOutputEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            MacroStabilityInwardsSlipPlaneUpliftVan slipPlane = CreateSlipPlane(entity);
            MacroStabilityInwardsSlidingCurve slidingCurve = CreateSlidingCurve(entity);

            return new MacroStabilityInwardsOutput(slidingCurve, slipPlane, new MacroStabilityInwardsOutput.ConstructionProperties
            {
                FactorOfStability = entity.FactorOfStability.ToNullAsNaN(),
                ForbiddenZonesXEntryMax = entity.ForbiddenZonesXEntryMax.ToNullAsNaN(),
                ForbiddenZonesXEntryMin = entity.ForbiddenZonesXEntryMin.ToNullAsNaN(),
                ZValue = entity.ZValue.ToNullAsNaN()
            });
        }

        private static MacroStabilityInwardsSlipPlaneUpliftVan CreateSlipPlane(MacroStabilityInwardsCalculationOutputEntity entity)
        {
            var leftGrid = new MacroStabilityInwardsGrid
            {
                XLeft = (RoundedDouble) entity.SlipPlaneLeftGridXLeft.ToNullAsNaN(),
                XRight = (RoundedDouble) entity.SlipPlaneLeftGridXRight.ToNullAsNaN(),
                NumberOfHorizontalPoints = entity.SlipPlaneLeftGridNrOfHorizontalPoints,
                ZTop = (RoundedDouble) entity.SlipPlaneLeftGridZTop.ToNullAsNaN(),
                ZBottom = (RoundedDouble) entity.SlipPlaneLeftGridZBottom.ToNullAsNaN(),
                NumberOfVerticalPoints = entity.SlipPlaneLeftGridNrOfVerticalPoints
            };
            var rightGrid = new MacroStabilityInwardsGrid
            {
                XLeft = (RoundedDouble) entity.SlipPlaneRightGridXLeft.ToNullAsNaN(),
                XRight = (RoundedDouble) entity.SlipPlaneRightGridXRight.ToNullAsNaN(),
                NumberOfHorizontalPoints = entity.SlipPlaneRightGridNrOfHorizontalPoints,
                ZTop = (RoundedDouble) entity.SlipPlaneRightGridZTop.ToNullAsNaN(),
                ZBottom = (RoundedDouble) entity.SlipPlaneRightGridZBottom.ToNullAsNaN(),
                NumberOfVerticalPoints = entity.SlipPlaneRightGridNrOfVerticalPoints
            };

            double[] tangentLines = new TangentLinesXmlSerializer().FromXml(entity.SlipPlaneTangentLinesXml);
            return new MacroStabilityInwardsSlipPlaneUpliftVan(leftGrid, rightGrid, tangentLines);
        }

        private static MacroStabilityInwardsSlidingCurve CreateSlidingCurve(MacroStabilityInwardsCalculationOutputEntity entity)
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

            MacroStabilityInwardsSlice[] slices = new MacroStabilityInwardsSliceXmlSerializer().FromXml(entity.SlidingCurveSliceXML);
            return new MacroStabilityInwardsSlidingCurve(leftCircle,
                                                         rightCircle,
                                                         slices,
                                                         entity.SlidingCurveNonIteratedHorizontalForce.ToNullAsNaN(),
                                                         entity.SlidingCurveIteratedHorizontalForce.ToNullAsNaN());
        }
    }
}