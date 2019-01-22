// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Riskeer.Storage.Core.DbContext
{
    public partial class ClosingStructuresCalculationEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClosingStructuresCalculationEntity()
        {
            ClosingStructuresOutputEntities = new HashSet<ClosingStructuresOutputEntity>();
            ClosingStructuresSectionResultEntities = new HashSet<ClosingStructuresSectionResultEntity>();
        }

        public long ClosingStructuresCalculationEntityId { get; set; }
        public long CalculationGroupEntityId { get; set; }
        public long? ForeshoreProfileEntityId { get; set; }
        public long? HydraulicLocationEntityId { get; set; }
        public long? ClosingStructureEntityId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public double? Orientation { get; set; }
        public double? LevelCrestStructureNotClosingMean { get; set; }
        public double? LevelCrestStructureNotClosingStandardDeviation { get; set; }
        public double? InsideWaterLevelMean { get; set; }
        public double? InsideWaterLevelStandardDeviation { get; set; }
        public double? ThresholdHeightOpenWeirMean { get; set; }
        public double? ThresholdHeightOpenWeirStandardDeviation { get; set; }
        public double? AreaFlowAperturesMean { get; set; }
        public double? AreaFlowAperturesStandardDeviation { get; set; }
        public double ProbabilityOpenStructureBeforeFlooding { get; set; }
        public double FailureProbabilityOpenStructure { get; set; }
        public int IdenticalApertures { get; set; }
        public double FailureProbabilityReparation { get; set; }
        public byte InflowModelType { get; set; }
        public double? DeviationWaveDirection { get; set; }
        public double? DrainCoefficientMean { get; set; }
        public double? ModelFactorSuperCriticalFlowMean { get; set; }
        public double? FactorStormDurationOpenStructure { get; set; }

        public virtual CalculationGroupEntity CalculationGroupEntity { get; set; }
        public virtual ClosingStructureEntity ClosingStructureEntity { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructuresOutputEntity> ClosingStructuresOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructuresSectionResultEntity> ClosingStructuresSectionResultEntities { get; set; }

        public byte UseBreakWater { get; set; }
        public byte BreakWaterType { get; set; }
        public double? BreakWaterHeight { get; set; }
        public byte UseForeshore { get; set; }
        public double? StructureNormalOrientation { get; set; }
        public double? StorageStructureAreaMean { get; set; }
        public double? StorageStructureAreaCoefficientOfVariation { get; set; }
        public double? AllowedLevelIncreaseStorageMean { get; set; }
        public double? AllowedLevelIncreaseStorageStandardDeviation { get; set; }
        public double? WidthFlowAperturesMean { get; set; }
        public double? WidthFlowAperturesStandardDeviation { get; set; }
        public double? CriticalOvertoppingDischargeMean { get; set; }
        public double? CriticalOvertoppingDischargeCoefficientOfVariation { get; set; }
        public double? FlowWidthAtBottomProtectionMean { get; set; }
        public double? FlowWidthAtBottomProtectionStandardDeviation { get; set; }
        public double FailureProbabilityStructureWithErosion { get; set; }
        public double? StormDurationMean { get; set; }
        public byte ShouldIllustrationPointsBeCalculated { get; set; }
        public virtual HydraulicLocationEntity HydraulicLocationEntity { get; set; }
        public virtual ForeshoreProfileEntity ForeshoreProfileEntity { get; set; }
    }
}