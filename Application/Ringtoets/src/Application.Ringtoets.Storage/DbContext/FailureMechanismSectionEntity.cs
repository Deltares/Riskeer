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
    using System.Collections.Generic;
    
    public partial class FailureMechanismSectionEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FailureMechanismSectionEntity()
        {
            this.ClosingStructureSectionResultEntities = new HashSet<ClosingStructureSectionResultEntity>();
            this.DuneErosionSectionResultEntities = new HashSet<DuneErosionSectionResultEntity>();
            this.GrassCoverErosionInwardsSectionResultEntities = new HashSet<GrassCoverErosionInwardsSectionResultEntity>();
            this.GrassCoverErosionOutwardsSectionResultEntities = new HashSet<GrassCoverErosionOutwardsSectionResultEntity>();
            this.GrassCoverSlipOffInwardsSectionResultEntities = new HashSet<GrassCoverSlipOffInwardsSectionResultEntity>();
            this.GrassCoverSlipOffOutwardsSectionResultEntities = new HashSet<GrassCoverSlipOffOutwardsSectionResultEntity>();
            this.HeightStructuresSectionResultEntities = new HashSet<HeightStructuresSectionResultEntity>();
            this.MacrostabilityInwardsSectionResultEntities = new HashSet<MacrostabilityInwardsSectionResultEntity>();
            this.MacrostabilityOutwardsSectionResultEntities = new HashSet<MacrostabilityOutwardsSectionResultEntity>();
            this.MicrostabilitySectionResultEntities = new HashSet<MicrostabilitySectionResultEntity>();
            this.PipingSectionResultEntities = new HashSet<PipingSectionResultEntity>();
            this.PipingStructureSectionResultEntities = new HashSet<PipingStructureSectionResultEntity>();
            this.StabilityStoneCoverSectionResultEntities = new HashSet<StabilityStoneCoverSectionResultEntity>();
            this.StrengthStabilityLengthwiseConstructionSectionResultEntities = new HashSet<StrengthStabilityLengthwiseConstructionSectionResultEntity>();
            this.StrengthStabilityPointConstructionSectionResultEntities = new HashSet<StrengthStabilityPointConstructionSectionResultEntity>();
            this.TechnicalInnovationSectionResultEntities = new HashSet<TechnicalInnovationSectionResultEntity>();
            this.WaterPressureAsphaltCoverSectionResultEntities = new HashSet<WaterPressureAsphaltCoverSectionResultEntity>();
            this.WaveImpactAsphaltCoverSectionResultEntities = new HashSet<WaveImpactAsphaltCoverSectionResultEntity>();
        }
    
        public long FailureMechanismSectionEntityId { get; set; }
        public long FailureMechanismEntityId { get; set; }
        public string Name { get; set; }
        public string FailureMechanismSectionPointXml { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructureSectionResultEntity> ClosingStructureSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DuneErosionSectionResultEntity> DuneErosionSectionResultEntities { get; set; }
        public virtual FailureMechanismEntity FailureMechanismEntity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsSectionResultEntity> GrassCoverErosionInwardsSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsSectionResultEntity> GrassCoverErosionOutwardsSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverSlipOffInwardsSectionResultEntity> GrassCoverSlipOffInwardsSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverSlipOffOutwardsSectionResultEntity> GrassCoverSlipOffOutwardsSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructuresSectionResultEntity> HeightStructuresSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacrostabilityInwardsSectionResultEntity> MacrostabilityInwardsSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MacrostabilityOutwardsSectionResultEntity> MacrostabilityOutwardsSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MicrostabilitySectionResultEntity> MicrostabilitySectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PipingSectionResultEntity> PipingSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PipingStructureSectionResultEntity> PipingStructureSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityStoneCoverSectionResultEntity> StabilityStoneCoverSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StrengthStabilityLengthwiseConstructionSectionResultEntity> StrengthStabilityLengthwiseConstructionSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StrengthStabilityPointConstructionSectionResultEntity> StrengthStabilityPointConstructionSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TechnicalInnovationSectionResultEntity> TechnicalInnovationSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WaterPressureAsphaltCoverSectionResultEntity> WaterPressureAsphaltCoverSectionResultEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WaveImpactAsphaltCoverSectionResultEntity> WaveImpactAsphaltCoverSectionResultEntities { get; set; }
    }
}
