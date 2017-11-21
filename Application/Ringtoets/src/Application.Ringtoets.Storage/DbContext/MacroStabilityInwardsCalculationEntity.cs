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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Ringtoets.Storage.DbContext
{
    public class MacroStabilityInwardsCalculationEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MacroStabilityInwardsCalculationEntity()
        {
            MacroStabilityInwardsCalculationOutputEntities = new HashSet<MacroStabilityInwardsCalculationOutputEntity>();
            MacroStabilityInwardsSemiProbabilisticOutputEntities = new HashSet<MacroStabilityInwardsSemiProbabilisticOutputEntity>();
        }

        public long MacroStabilityInwardsCalculationEntityId { get; set; }
        public long CalculationGroupEntityId { get; set; }
        public long? SurfaceLineEntityId { get; set; }
        public long? MacroStabilityInwardsStochasticSoilProfileEntityId { get; set; }
        public long? HydraulicLocationEntityId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public byte RelevantForScenario { get; set; }
        public double? ScenarioContribution { get; set; }
        public double? AssessmentLevel { get; set; }
        public byte UseAssessmentLevelManualInput { get; set; }
        public double? SlipPlaneMinimumDepth { get; set; }
        public double? SlipPlaneMinimumLength { get; set; }
        public double? MaximumSliceWidth { get; set; }
        public byte MoveGrid { get; set; }
        public byte GridDeterminationType { get; set; }
        public byte TangentLineDeterminationType { get; set; }
        public double? TangentLineZTop { get; set; }
        public double? TangentLineZBottom { get; set; }
        public int TangentLineNumber { get; set; }
        public double? LeftGridXLeft { get; set; }
        public double? LeftGridXRight { get; set; }
        public int LeftGridNrOfHorizontalPoints { get; set; }
        public double? LeftGridZTop { get; set; }
        public double? LeftGridZBottom { get; set; }
        public int LeftGridNrOfVerticalPoints { get; set; }
        public double? RightGridXLeft { get; set; }
        public double? RightGridXRight { get; set; }
        public int RightGridNrOfHorizontalPoints { get; set; }
        public double? RightGridZTop { get; set; }
        public double? RightGridZBottom { get; set; }
        public int RightGridNrOfVerticalPoints { get; set; }
        public byte DikeSoilScenario { get; set; }
        public double? WaterLevelRiverAverage { get; set; }
        public byte DrainageConstructionPresent { get; set; }
        public double? DrainageConstructionCoordinateX { get; set; }
        public double? DrainageConstructionCoordinateZ { get; set; }
        public double? MinimumLevelPhreaticLineAtDikeTopRiver { get; set; }
        public double? MinimumLevelPhreaticLineAtDikeTopPolder { get; set; }
        public byte AdjustPhreaticLine3And4ForUplift { get; set; }
        public double? LeakageLengthOutwardsPhreaticLine3 { get; set; }
        public double? LeakageLengthInwardsPhreaticLine3 { get; set; }
        public double? LeakageLengthOutwardsPhreaticLine4 { get; set; }
        public double? LeakageLengthInwardsPhreaticLine4 { get; set; }
        public double? PiezometricHeadPhreaticLine2Outwards { get; set; }
        public double? PiezometricHeadPhreaticLine2Inwards { get; set; }
        public double? LocationInputExtremeWaterLevelPolder { get; set; }
        public byte LocationInputExtremeUseDefaultOffsets { get; set; }
        public double? LocationInputExtremePhreaticLineOffsetBelowDikeTopAtRiver { get; set; }
        public double? LocationInputExtremePhreaticLineOffsetBelowDikeTopAtPolder { get; set; }
        public double? LocationInputExtremePhreaticLineOffsetBelowShoulderBaseInside { get; set; }
        public double? LocationInputExtremePhreaticLineOffsetDikeToeAtPolder { get; set; }
        public double? LocationInputExtremePenetrationLength { get; set; }
        public double? LocationInputDailyWaterLevelPolder { get; set; }
        public byte LocationInputDailyUseDefaultOffsets { get; set; }
        public double? LocationInputDailyPhreaticLineOffsetBelowDikeTopAtRiver { get; set; }
        public double? LocationInputDailyPhreaticLineOffsetBelowDikeTopAtPolder { get; set; }
        public double? LocationInputDailyPhreaticLineOffsetBelowShoulderBaseInside { get; set; }
        public double? LocationInputDailyPhreaticLineOffsetDikeToeAtPolder { get; set; }
        public byte CreateZones { get; set; }
        public byte ZoningBoundariesDeterminationType { get; set; }
        public double? ZoneBoundaryLeft { get; set; }
        public double? ZoneBoundaryRight { get; set; }

        public virtual CalculationGroupEntity CalculationGroupEntity { get; set; }
        public virtual HydraulicLocationEntity HydraulicLocationEntity { get; set; }
        public virtual SurfaceLineEntity SurfaceLineEntity { get; set; }
        public virtual MacroStabilityInwardsStochasticSoilProfileEntity MacroStabilityInwardsStochasticSoilProfileEntity { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacroStabilityInwardsCalculationOutputEntity> MacroStabilityInwardsCalculationOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacroStabilityInwardsSemiProbabilisticOutputEntity> MacroStabilityInwardsSemiProbabilisticOutputEntities { get; set; }
    }
}