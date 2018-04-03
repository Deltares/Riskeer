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
    public class HydraulicLocationEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HydraulicLocationEntity()
        {
            ClosingStructuresCalculationEntities = new HashSet<ClosingStructuresCalculationEntity>();
            GrassCoverErosionInwardsCalculationEntities = new HashSet<GrassCoverErosionInwardsCalculationEntity>();
            GrassCoverErosionOutwardsWaveConditionsCalculationEntities = new HashSet<GrassCoverErosionOutwardsWaveConditionsCalculationEntity>();
            HeightStructuresCalculationEntities = new HashSet<HeightStructuresCalculationEntity>();
            HydraulicLocationCalculationEntities = new HashSet<HydraulicLocationCalculationEntity>();
            MacroStabilityInwardsCalculationEntities = new HashSet<MacroStabilityInwardsCalculationEntity>();
            PipingCalculationEntities = new HashSet<PipingCalculationEntity>();
            StabilityPointStructuresCalculationEntities = new HashSet<StabilityPointStructuresCalculationEntity>();
            StabilityStoneCoverWaveConditionsCalculationEntities = new HashSet<StabilityStoneCoverWaveConditionsCalculationEntity>();
            WaveImpactAsphaltCoverWaveConditionsCalculationEntities = new HashSet<WaveImpactAsphaltCoverWaveConditionsCalculationEntity>();
        }

        public long HydraulicLocationEntityId { get; set; }
        public long AssessmentSectionEntityId { get; set; }
        public long LocationId { get; set; }
        public string Name { get; set; }
        public double? LocationX { get; set; }
        public double? LocationY { get; set; }
        public int Order { get; set; }

        public virtual AssessmentSectionEntity AssessmentSectionEntity { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructuresCalculationEntity> ClosingStructuresCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsCalculationEntity> GrassCoverErosionInwardsCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsWaveConditionsCalculationEntity> GrassCoverErosionOutwardsWaveConditionsCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructuresCalculationEntity> HeightStructuresCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HydraulicLocationCalculationEntity> HydraulicLocationCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacroStabilityInwardsCalculationEntity> MacroStabilityInwardsCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PipingCalculationEntity> PipingCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityPointStructuresCalculationEntity> StabilityPointStructuresCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityStoneCoverWaveConditionsCalculationEntity> StabilityStoneCoverWaveConditionsCalculationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WaveImpactAsphaltCoverWaveConditionsCalculationEntity> WaveImpactAsphaltCoverWaveConditionsCalculationEntities { get; set; }
    }
}