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

namespace Ringtoets.Storage.Core.DbContext
{
    public class ClosingStructureEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ClosingStructureEntity()
        {
            ClosingStructuresCalculationEntities = new HashSet<ClosingStructuresCalculationEntity>();
        }

        public long ClosingStructureEntityId { get; set; }
        public long FailureMechanismEntityId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? StructureNormalOrientation { get; set; }
        public double? StorageStructureAreaMean { get; set; }
        public double? StorageStructureAreaCoefficientOfVariation { get; set; }
        public double? AllowedLevelIncreaseStorageMean { get; set; }
        public double? AllowedLevelIncreaseStorageStandardDeviation { get; set; }
        public double? WidthFlowAperturesMean { get; set; }
        public double? WidthFlowAperturesStandardDeviation { get; set; }
        public double? LevelCrestStructureNotClosingMean { get; set; }
        public double? LevelCrestStructureNotClosingStandardDeviation { get; set; }
        public double? InsideWaterLevelMean { get; set; }
        public double? InsideWaterLevelStandardDeviation { get; set; }
        public double? ThresholdHeightOpenWeirMean { get; set; }
        public double? ThresholdHeightOpenWeirStandardDeviation { get; set; }
        public double? AreaFlowAperturesMean { get; set; }
        public double? AreaFlowAperturesStandardDeviation { get; set; }
        public double? CriticalOvertoppingDischargeMean { get; set; }
        public double? CriticalOvertoppingDischargeCoefficientOfVariation { get; set; }
        public double? FlowWidthAtBottomProtectionMean { get; set; }
        public double? FlowWidthAtBottomProtectionStandardDeviation { get; set; }
        public double? ProbabilityOpenStructureBeforeFlooding { get; set; }
        public double? FailureProbabilityOpenStructure { get; set; }
        public int IdenticalApertures { get; set; }
        public double? FailureProbabilityReparation { get; set; }
        public byte InflowModelType { get; set; }

        public virtual FailureMechanismEntity FailureMechanismEntity { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructuresCalculationEntity> ClosingStructuresCalculationEntities { get; set; }
    }
}