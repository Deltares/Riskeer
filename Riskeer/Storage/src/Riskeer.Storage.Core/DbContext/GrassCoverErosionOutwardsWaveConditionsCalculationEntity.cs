// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    public class GrassCoverErosionOutwardsWaveConditionsCalculationEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GrassCoverErosionOutwardsWaveConditionsCalculationEntity()
        {
            GrassCoverErosionOutwardsWaveConditionsOutputEntities = new HashSet<GrassCoverErosionOutwardsWaveConditionsOutputEntity>();
        }
    
        public long GrassCoverErosionOutwardsWaveConditionsCalculationEntityId { get; set; }
        public long CalculationGroupEntityId { get; set; }
        public long? ForeshoreProfileEntityId { get; set; }
        public long? HydraulicLocationEntityId { get; set; }
        public long? HydraulicLocationCalculationForTargetProbabilityCollectionEntityId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public byte UseBreakWater { get; set; }
        public byte BreakWaterType { get; set; }
        public double? BreakWaterHeight { get; set; }
        public byte UseForeshore { get; set; }
        public double? Orientation { get; set; }
        public double? UpperBoundaryRevetment { get; set; }
        public double? LowerBoundaryRevetment { get; set; }
        public double? UpperBoundaryWaterLevels { get; set; }
        public double? LowerBoundaryWaterLevels { get; set; }
        public byte StepSize { get; set; }
        public byte CalculationType { get; set; }
        public byte WaterLevelType { get; set; }
    
        public virtual CalculationGroupEntity CalculationGroupEntity { get; set; }
        public virtual ForeshoreProfileEntity ForeshoreProfileEntity { get; set; }
        public virtual HydraulicLocationEntity HydraulicLocationEntity { get; set; }
        public virtual HydraulicLocationCalculationForTargetProbabilityCollectionEntity HydraulicLocationCalculationForTargetProbabilityCollectionEntity { get; set; }
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsWaveConditionsOutputEntity> GrassCoverErosionOutwardsWaveConditionsOutputEntities { get; set; }
    }
}
