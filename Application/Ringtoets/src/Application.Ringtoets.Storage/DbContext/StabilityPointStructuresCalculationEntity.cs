// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Application.Ringtoets.Storage.DbContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class StabilityPointStructuresCalculationEntity
    {
        public long StabilityPointStructuresCalculationEntityId { get; set; }
        public long CalculationGroupEntityId { get; set; }
        public Nullable<long> ForeshoreProfileEntityId { get; set; }
        public Nullable<long> HydraulicLocationEntityId { get; set; }
        public Nullable<long> StabilityPointStructureEntityId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public Nullable<byte> UseBreakWater { get; set; }
        public short BreakWaterType { get; set; }
        public Nullable<double> BreakWaterHeight { get; set; }
        public byte UseForeshore { get; set; }
        public Nullable<double> StructureNormalOrientation { get; set; }
        public Nullable<double> StorageStructureAreaMean { get; set; }
        public Nullable<double> StorageStructureAreaCoefficientOfVariation { get; set; }
        public Nullable<double> AllowedLevelIncreaseStorageMean { get; set; }
        public Nullable<double> AllowedLevelIncreaseStorageStandardDeviation { get; set; }
        public Nullable<double> WidthFlowAperturesMean { get; set; }
        public Nullable<double> WidthFlowAperturesCoefficientOfVariation { get; set; }
        public Nullable<double> InsideWaterLevelMean { get; set; }
        public Nullable<double> InsideWaterLevelStandardDeviation { get; set; }
        public Nullable<double> ThresholdHeightOpenWeirMean { get; set; }
        public Nullable<double> ThresholdHeightOpenWeirStandardDeviation { get; set; }
        public Nullable<double> CriticalOvertoppingDischargeMean { get; set; }
        public Nullable<double> CriticalOvertoppingDischargeCoefficientOfVariation { get; set; }
        public Nullable<double> FlowWidthAtBottomProtectionMean { get; set; }
        public Nullable<double> FlowWidthAtBottomProtectionStandardDeviation { get; set; }
        public Nullable<double> ConstructiveStrengthLinearLoadModelMean { get; set; }
        public Nullable<double> ConstructiveStrengthLinearLoadModelCoefficientOfVariation { get; set; }
        public Nullable<double> ConstructiveStrengthQuadraticLoadModelMean { get; set; }
        public Nullable<double> ConstructiveStrengthQuadraticLoadModelCoefficientOfVariation { get; set; }
        public Nullable<double> BankWidthMean { get; set; }
        public Nullable<double> BankWidthStandardDeviation { get; set; }
        public Nullable<double> InsideWaterLevelFailureConstructionMean { get; set; }
        public Nullable<double> InsideWaterLevelFailureConstructionStandardDeviation { get; set; }
        public Nullable<double> EvaluationLevel { get; set; }
        public Nullable<double> LevelCrestStructureMean { get; set; }
        public Nullable<double> LevelCrestStructureStandardDeviation { get; set; }
        public Nullable<double> VerticalDistance { get; set; }
        public double FailureProbabilityRepairClosure { get; set; }
        public Nullable<double> FailureCollisionEnergyMean { get; set; }
        public Nullable<double> FailureCollisionEnergyCoefficientOfVariation { get; set; }
        public Nullable<double> ShipMassMean { get; set; }
        public Nullable<double> ShipMassCoefficientOfVariation { get; set; }
        public Nullable<double> ShipVelocityMean { get; set; }
        public Nullable<double> ShipVelocityCoefficientOfVariation { get; set; }
        public int LevellingCount { get; set; }
        public double ProbabilityCollisionSecondaryStructure { get; set; }
        public Nullable<double> FlowVelocityStructureClosableMean { get; set; }
        public Nullable<double> FlowVelocityStructureClosableStandardDeviation { get; set; }
        public Nullable<double> StabilityLinearLoadModelMean { get; set; }
        public Nullable<double> StabilityLinearLoadModelCoefficientOfVariation { get; set; }
        public Nullable<double> StabilityQuadraticLoadModelMean { get; set; }
        public Nullable<double> StabilityQuadraticLoadModelCoefficientOfVariation { get; set; }
        public Nullable<double> AreaFlowAperturesMean { get; set; }
        public Nullable<double> AreaFlowAperturesStandardDeviation { get; set; }
        public byte InflowModelType { get; set; }
        public byte LoadSchematizationType { get; set; }
        public Nullable<double> VolumicWeightWater { get; set; }
        public Nullable<double> StormDurationMean { get; set; }
        public Nullable<double> ModelFactorSuperCriticalFlowMean { get; set; }
        public Nullable<double> FactorStormDurationOpenStructure { get; set; }
        public Nullable<double> DrainCoefficientMean { get; set; }
    
        public virtual CalculationGroupEntity CalculationGroupEntity { get; set; }
        public virtual ForeshoreProfileEntity ForeshoreProfileEntity { get; set; }
        public virtual HydraulicLocationEntity HydraulicLocationEntity { get; set; }
        public virtual StabilityPointStructureEntity StabilityPointStructureEntity { get; set; }
    }
}
