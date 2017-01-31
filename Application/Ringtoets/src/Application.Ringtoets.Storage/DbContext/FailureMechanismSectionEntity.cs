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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class FailureMechanismSectionEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FailureMechanismSectionEntity()
        {
            ClosingStructuresSectionResultEntities = new HashSet<ClosingStructuresSectionResultEntity>();
            DuneErosionSectionResultEntities = new HashSet<DuneErosionSectionResultEntity>();
            GrassCoverErosionInwardsSectionResultEntities = new HashSet<GrassCoverErosionInwardsSectionResultEntity>();
            GrassCoverErosionOutwardsSectionResultEntities = new HashSet<GrassCoverErosionOutwardsSectionResultEntity>();
            GrassCoverSlipOffInwardsSectionResultEntities = new HashSet<GrassCoverSlipOffInwardsSectionResultEntity>();
            GrassCoverSlipOffOutwardsSectionResultEntities = new HashSet<GrassCoverSlipOffOutwardsSectionResultEntity>();
            HeightStructuresSectionResultEntities = new HashSet<HeightStructuresSectionResultEntity>();
            MacrostabilityInwardsSectionResultEntities = new HashSet<MacrostabilityInwardsSectionResultEntity>();
            MacrostabilityOutwardsSectionResultEntities = new HashSet<MacrostabilityOutwardsSectionResultEntity>();
            MicrostabilitySectionResultEntities = new HashSet<MicrostabilitySectionResultEntity>();
            PipingSectionResultEntities = new HashSet<PipingSectionResultEntity>();
            PipingStructureSectionResultEntities = new HashSet<PipingStructureSectionResultEntity>();
            StabilityPointStructuresSectionResultEntities = new HashSet<StabilityPointStructuresSectionResultEntity>();
            StabilityStoneCoverSectionResultEntities = new HashSet<StabilityStoneCoverSectionResultEntity>();
            StrengthStabilityLengthwiseConstructionSectionResultEntities = new HashSet<StrengthStabilityLengthwiseConstructionSectionResultEntity>();
            TechnicalInnovationSectionResultEntities = new HashSet<TechnicalInnovationSectionResultEntity>();
            WaterPressureAsphaltCoverSectionResultEntities = new HashSet<WaterPressureAsphaltCoverSectionResultEntity>();
            WaveImpactAsphaltCoverSectionResultEntities = new HashSet<WaveImpactAsphaltCoverSectionResultEntity>();
        }

        public long FailureMechanismSectionEntityId { get; set; }
        public long FailureMechanismEntityId { get; set; }
        public string Name { get; set; }
        public string FailureMechanismSectionPointXml { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructuresSectionResultEntity> ClosingStructuresSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DuneErosionSectionResultEntity> DuneErosionSectionResultEntities { get; set; }

        public virtual FailureMechanismEntity FailureMechanismEntity { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsSectionResultEntity> GrassCoverErosionInwardsSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsSectionResultEntity> GrassCoverErosionOutwardsSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverSlipOffInwardsSectionResultEntity> GrassCoverSlipOffInwardsSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverSlipOffOutwardsSectionResultEntity> GrassCoverSlipOffOutwardsSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructuresSectionResultEntity> HeightStructuresSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacrostabilityInwardsSectionResultEntity> MacrostabilityInwardsSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacrostabilityOutwardsSectionResultEntity> MacrostabilityOutwardsSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MicrostabilitySectionResultEntity> MicrostabilitySectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PipingSectionResultEntity> PipingSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PipingStructureSectionResultEntity> PipingStructureSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityPointStructuresSectionResultEntity> StabilityPointStructuresSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityStoneCoverSectionResultEntity> StabilityStoneCoverSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StrengthStabilityLengthwiseConstructionSectionResultEntity> StrengthStabilityLengthwiseConstructionSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TechnicalInnovationSectionResultEntity> TechnicalInnovationSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WaterPressureAsphaltCoverSectionResultEntity> WaterPressureAsphaltCoverSectionResultEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WaveImpactAsphaltCoverSectionResultEntity> WaveImpactAsphaltCoverSectionResultEntities { get; set; }
    }
}