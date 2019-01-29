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
    public partial class StabilityPointStructuresCalculationEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StabilityPointStructuresCalculationEntity()
        {
            StabilityPointStructuresOutputEntities = new HashSet<StabilityPointStructuresOutputEntity>();
            StabilityPointStructuresSectionResultEntities = new HashSet<StabilityPointStructuresSectionResultEntity>();
        }

        public long StabilityPointStructuresCalculationEntityId { get; set; }
        public long CalculationGroupEntityId { get; set; }
        public long? ForeshoreProfileEntityId { get; set; }
        public long? HydraulicLocationEntityId { get; set; }
        public long? StabilityPointStructureEntityId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public double? InsideWaterLevelMean { get; set; }
        public double? InsideWaterLevelStandardDeviation { get; set; }
        public double? ThresholdHeightOpenWeirMean { get; set; }
        public double? ThresholdHeightOpenWeirStandardDeviation { get; set; }
        public double? ConstructiveStrengthLinearLoadModelMean { get; set; }
        public double? ConstructiveStrengthLinearLoadModelCoefficientOfVariation { get; set; }
        public double? ConstructiveStrengthQuadraticLoadModelMean { get; set; }
        public double? ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation { get; set; }
        public double? BankWidthMean { get; set; }
        public double? BankWidthStandardDeviation { get; set; }
        public double? InsideWaterLevelFailureConstructionMean { get; set; }
        public double? InsideWaterLevelFailureConstructionStandardDeviation { get; set; }
        public double? EvaluationLevel { get; set; }
        public double? LevelCrestStructureMean { get; set; }
        public double? LevelCrestStructureStandardDeviation { get; set; }
        public double? VerticalDistance { get; set; }
        public double FailureProbabilityRepairClosure { get; set; }
        public double? FailureCollisionEnergyMean { get; set; }
        public double? FailureCollisionEnergyCoefficientOfVariation { get; set; }
        public double? ShipMassMean { get; set; }
        public double? ShipMassCoefficientOfVariation { get; set; }
        public double? ShipVelocityMean { get; set; }
        public double? ShipVelocityCoefficientOfVariation { get; set; }
        public int LevellingCount { get; set; }
        public double ProbabilityCollisionSecondaryStructure { get; set; }
        public double? FlowVelocityStructureClosableMean { get; set; }
        public double? StabilityLinearLoadModelMean { get; set; }
        public double? StabilityLinearLoadModelCoefficientOfVariation { get; set; }
        public double? StabilityQuadraticLoadModelMean { get; set; }
        public double? StabilityQuadraticLoadModelCoefficientOfVariation { get; set; }
        public double? AreaFlowAperturesMean { get; set; }
        public double? AreaFlowAperturesStandardDeviation { get; set; }
        public byte InflowModelType { get; set; }
        public byte LoadSchematizationType { get; set; }
        public double? VolumicWeightWater { get; set; }
        public double? FactorStormDurationOpenStructure { get; set; }
        public double? DrainCoefficientMean { get; set; }

        public virtual CalculationGroupEntity CalculationGroupEntity { get; set; }
        public virtual StabilityPointStructureEntity StabilityPointStructureEntity { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityPointStructuresOutputEntity> StabilityPointStructuresOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityPointStructuresSectionResultEntity> StabilityPointStructuresSectionResultEntities { get; set; }

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
        public double? StormDurationMean { get; set; }
        public double FailureProbabilityStructureWithErosion { get; set; }
        public byte ShouldIllustrationPointsBeCalculated { get; set; }
        public virtual ForeshoreProfileEntity ForeshoreProfileEntity { get; set; }
        public virtual HydraulicLocationEntity HydraulicLocationEntity { get; set; }
    }
}