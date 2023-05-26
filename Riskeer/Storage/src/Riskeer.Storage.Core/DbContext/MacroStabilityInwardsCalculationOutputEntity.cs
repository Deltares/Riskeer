// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Riskeer.Storage.Core.DbContext
{
    public class MacroStabilityInwardsCalculationOutputEntity
    {
        public long MacroStabilityInwardsCalculationOutputEntityId { get; set; }
        public long MacroStabilityInwardsCalculationEntityId { get; set; }
        public double? FactorOfStability { get; set; }
        public double? ForbiddenZonesXEntryMin { get; set; }
        public double? ForbiddenZonesXEntryMax { get; set; }
        public double? SlidingCurveLeftSlidingCircleCenterX { get; set; }
        public double? SlidingCurveLeftSlidingCircleCenterY { get; set; }
        public double? SlidingCurveLeftSlidingCircleRadius { get; set; }
        public byte SlidingCurveLeftSlidingCircleIsActive { get; set; }
        public double? SlidingCurveLeftSlidingCircleNonIteratedForce { get; set; }
        public double? SlidingCurveLeftSlidingCircleIteratedForce { get; set; }
        public double? SlidingCurveLeftSlidingCircleDrivingMoment { get; set; }
        public double? SlidingCurveLeftSlidingCircleResistingMoment { get; set; }
        public double? SlidingCurveRightSlidingCircleCenterX { get; set; }
        public double? SlidingCurveRightSlidingCircleCenterY { get; set; }
        public double? SlidingCurveRightSlidingCircleRadius { get; set; }
        public byte SlidingCurveRightSlidingCircleIsActive { get; set; }
        public double? SlidingCurveRightSlidingCircleNonIteratedForce { get; set; }
        public double? SlidingCurveRightSlidingCircleIteratedForce { get; set; }
        public double? SlidingCurveRightSlidingCircleDrivingMoment { get; set; }
        public double? SlidingCurveRightSlidingCircleResistingMoment { get; set; }
        public double? SlidingCurveNonIteratedHorizontalForce { get; set; }
        public double? SlidingCurveIteratedHorizontalForce { get; set; }
        public string SlidingCurveSliceXML { get; set; }
        public double? SlipPlaneLeftGridXLeft { get; set; }
        public double? SlipPlaneLeftGridXRight { get; set; }
        public int SlipPlaneLeftGridNrOfHorizontalPoints { get; set; }
        public double? SlipPlaneLeftGridZTop { get; set; }
        public double? SlipPlaneLeftGridZBottom { get; set; }
        public int SlipPlaneLeftGridNrOfVerticalPoints { get; set; }
        public double? SlipPlaneRightGridXLeft { get; set; }
        public double? SlipPlaneRightGridXRight { get; set; }
        public int SlipPlaneRightGridNrOfHorizontalPoints { get; set; }
        public double? SlipPlaneRightGridZTop { get; set; }
        public double? SlipPlaneRightGridZBottom { get; set; }
        public int SlipPlaneRightGridNrOfVerticalPoints { get; set; }
        public string SlipPlaneTangentLinesXml { get; set; }

        public virtual MacroStabilityInwardsCalculationEntity MacroStabilityInwardsCalculationEntity { get; set; }
    }
}