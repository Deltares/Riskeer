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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsOutput"/> related to creating a 
    /// <see cref="MacroStabilityInwardsCalculationOutputEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsCalculationOutputEntity"/> based on the information
        /// of the <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for macro stability inwards failure mechanism to 
        /// create a database entity for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsCalculationOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsCalculationOutputEntity Create(this MacroStabilityInwardsOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var entity = new MacroStabilityInwardsCalculationOutputEntity
            {
                FactorOfStability = output.FactorOfStability.ToNaNAsNull(),
                ForbiddenZonesXEntryMin = output.ForbiddenZonesXEntryMin.ToNaNAsNull(),
                ForbiddenZonesXEntryMax = output.ForbiddenZonesXEntryMax.ToNaNAsNull(),
                ZValue = output.ZValue.ToNaNAsNull()
            };
            SetSlidingCurveParametersToEntity(entity, output.SlidingCurve);
            SetSlipPlaneParametersToEntity(entity, output.SlipPlane);

            return entity;
        }

        private static void SetSlidingCurveParametersToEntity(MacroStabilityInwardsCalculationOutputEntity entity,
                                                              MacroStabilityInwardsSlidingCurve slidingCurve)
        {
            entity.SlidingCurveSliceXML = new MacroStabilityInwardsSliceCollectionXmlSerializer().ToXml(slidingCurve.Slices);
            entity.SlidingCurveNonIteratedHorizontalForce = slidingCurve.NonIteratedHorizontalForce.ToNaNAsNull();
            entity.SlidingCurveIteratedHorizontalForce = slidingCurve.IteratedHorizontalForce.ToNaNAsNull();

            MacroStabilityInwardsSlidingCircle leftCircle = slidingCurve.LeftCircle;
            entity.SlidingCurveLeftSlidingCircleCenterX = leftCircle.Center.X.ToNaNAsNull();
            entity.SlidingCurveLeftSlidingCircleCenterY = leftCircle.Center.Y.ToNaNAsNull();
            entity.SlidingCurveLeftSlidingCircleRadius = leftCircle.Radius.ToNaNAsNull();
            entity.SlidingCurveLeftSlidingCircleIsActive = Convert.ToByte(leftCircle.IsActive);
            entity.SlidingCurveLeftSlidingCircleNonIteratedForce = leftCircle.NonIteratedForce.ToNaNAsNull();
            entity.SlidingCurveLeftSlidingCircleIteratedForce = leftCircle.IteratedForce.ToNaNAsNull();
            entity.SlidingCurveLeftSlidingCircleDrivingMoment = leftCircle.DrivingMoment.ToNaNAsNull();
            entity.SlidingCurveLeftSlidingCircleResistingMoment = leftCircle.ResistingMoment.ToNaNAsNull();

            MacroStabilityInwardsSlidingCircle rightCircle = slidingCurve.RightCircle;
            entity.SlidingCurveRightSlidingCircleCenterX = rightCircle.Center.X.ToNaNAsNull();
            entity.SlidingCurveRightSlidingCircleCenterY = rightCircle.Center.Y.ToNaNAsNull();
            entity.SlidingCurveRightSlidingCircleRadius = rightCircle.Radius.ToNaNAsNull();
            entity.SlidingCurveRightSlidingCircleIsActive = Convert.ToByte(rightCircle.IsActive);
            entity.SlidingCurveRightSlidingCircleNonIteratedForce = rightCircle.NonIteratedForce.ToNaNAsNull();
            entity.SlidingCurveRightSlidingCircleIteratedForce = rightCircle.IteratedForce.ToNaNAsNull();
            entity.SlidingCurveRightSlidingCircleDrivingMoment = rightCircle.DrivingMoment.ToNaNAsNull();
            entity.SlidingCurveRightSlidingCircleResistingMoment = rightCircle.ResistingMoment.ToNaNAsNull();
        }

        private static void SetSlipPlaneParametersToEntity(MacroStabilityInwardsCalculationOutputEntity entity,
                                                           MacroStabilityInwardsSlipPlaneUpliftVan slipPlane)
        {
            entity.SlipPlaneTangentLinesXml = new TangentLineCollectionXmlSerializer().ToXml(slipPlane.TangentLines);

            MacroStabilityInwardsGrid leftGrid = slipPlane.LeftGrid;
            entity.SlipPlaneLeftGridXLeft = leftGrid.XLeft.ToNaNAsNull();
            entity.SlipPlaneLeftGridXRight = leftGrid.XRight.ToNaNAsNull();
            entity.SlipPlaneLeftGridNrOfHorizontalPoints = leftGrid.NumberOfHorizontalPoints;
            entity.SlipPlaneLeftGridZTop = leftGrid.ZTop.ToNaNAsNull();
            entity.SlipPlaneLeftGridZBottom = leftGrid.ZBottom.ToNaNAsNull();
            entity.SlipPlaneLeftGridNrOfVerticalPoints = leftGrid.NumberOfVerticalPoints;

            MacroStabilityInwardsGrid rightGrid = slipPlane.RightGrid;
            entity.SlipPlaneRightGridXLeft = rightGrid.XLeft.ToNaNAsNull();
            entity.SlipPlaneRightGridXRight = rightGrid.XRight.ToNaNAsNull();
            entity.SlipPlaneRightGridNrOfHorizontalPoints = rightGrid.NumberOfHorizontalPoints;
            entity.SlipPlaneRightGridZTop = rightGrid.ZTop.ToNaNAsNull();
            entity.SlipPlaneRightGridZBottom = rightGrid.ZBottom.ToNaNAsNull();
            entity.SlipPlaneRightGridNrOfVerticalPoints = rightGrid.NumberOfVerticalPoints;
        }
    }
}