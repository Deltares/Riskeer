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
    public partial class HeightStructuresCalculationEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HeightStructuresCalculationEntity()
        {
            HeightStructuresOutputEntities = new HashSet<HeightStructuresOutputEntity>();
            HeightStructuresSectionResultEntities = new HashSet<HeightStructuresSectionResultEntity>();
        }

        public long HeightStructuresCalculationEntityId { get; set; }
        public long CalculationGroupEntityId { get; set; }
        public long? HydraulicLocationEntityId { get; set; }
        public long? HeightStructureEntityId { get; set; }
        public long? ForeshoreProfileEntityId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public double? ModelFactorSuperCriticalFlowMean { get; set; }
        public double? LevelCrestStructureMean { get; set; }
        public double? LevelCrestStructureStandardDeviation { get; set; }
        public double? DeviationWaveDirection { get; set; }

        public virtual CalculationGroupEntity CalculationGroupEntity { get; set; }
        public virtual HeightStructureEntity HeightStructureEntity { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructuresOutputEntity> HeightStructuresOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructuresSectionResultEntity> HeightStructuresSectionResultEntities { get; set; }

        public double? StructureNormalOrientation { get; set; }
        public double? AllowedLevelIncreaseStorageMean { get; set; }
        public double? AllowedLevelIncreaseStorageStandardDeviation { get; set; }
        public double? StorageStructureAreaMean { get; set; }
        public double? StorageStructureAreaCoefficientOfVariation { get; set; }
        public double? FlowWidthAtBottomProtectionMean { get; set; }
        public double? FlowWidthAtBottomProtectionStandardDeviation { get; set; }
        public double? CriticalOvertoppingDischargeMean { get; set; }
        public double? CriticalOvertoppingDischargeCoefficientOfVariation { get; set; }
        public double FailureProbabilityStructureWithErosion { get; set; }
        public double? WidthFlowAperturesMean { get; set; }
        public double? WidthFlowAperturesStandardDeviation { get; set; }
        public double? StormDurationMean { get; set; }
        public byte UseBreakWater { get; set; }
        public byte UseForeshore { get; set; }
        public byte BreakWaterType { get; set; }
        public double? BreakWaterHeight { get; set; }
        public byte ShouldIllustrationPointsBeCalculated { get; set; }
        public virtual ForeshoreProfileEntity ForeshoreProfileEntity { get; set; }
        public virtual HydraulicLocationEntity HydraulicLocationEntity { get; set; }
    }
}