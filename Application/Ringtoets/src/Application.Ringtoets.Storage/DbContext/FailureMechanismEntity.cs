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
    
    public partial class FailureMechanismEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FailureMechanismEntity()
        {
            this.ClosingStructureFailureMechanismMetaEntities = new HashSet<ClosingStructureFailureMechanismMetaEntity>();
            this.DikeProfileEntities = new HashSet<DikeProfileEntity>();
            this.FailureMechanismSectionEntities = new HashSet<FailureMechanismSectionEntity>();
            this.ForeshoreProfileEntities = new HashSet<ForeshoreProfileEntity>();
            this.GrassCoverErosionInwardsFailureMechanismMetaEntities = new HashSet<GrassCoverErosionInwardsFailureMechanismMetaEntity>();
            this.GrassCoverErosionOutwardsFailureMechanismMetaEntities = new HashSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity>();
            this.GrassCoverErosionOutwardsHydraulicLocationEntities = new HashSet<GrassCoverErosionOutwardsHydraulicLocationEntity>();
            this.HeightStructureEntities = new HashSet<HeightStructureEntity>();
            this.HeightStructuresFailureMechanismMetaEntities = new HashSet<HeightStructuresFailureMechanismMetaEntity>();
            this.PipingFailureMechanismMetaEntities = new HashSet<PipingFailureMechanismMetaEntity>();
            this.StabilityPointStructuresFailureMechanismMetaEntities = new HashSet<StabilityPointStructuresFailureMechanismMetaEntity>();
            this.StochasticSoilModelEntities = new HashSet<StochasticSoilModelEntity>();
            this.SurfaceLineEntities = new HashSet<SurfaceLineEntity>();
        }
    
        public long FailureMechanismEntityId { get; set; }
        public long AssessmentSectionEntityId { get; set; }
        public Nullable<long> CalculationGroupEntityId { get; set; }
        public short FailureMechanismType { get; set; }
        public byte IsRelevant { get; set; }
        public string Comments { get; set; }
    
        public virtual AssessmentSectionEntity AssessmentSectionEntity { get; set; }
        public virtual CalculationGroupEntity CalculationGroupEntity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructureFailureMechanismMetaEntity> ClosingStructureFailureMechanismMetaEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DikeProfileEntity> DikeProfileEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FailureMechanismSectionEntity> FailureMechanismSectionEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ForeshoreProfileEntity> ForeshoreProfileEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsFailureMechanismMetaEntity> GrassCoverErosionInwardsFailureMechanismMetaEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsHydraulicLocationEntity> GrassCoverErosionOutwardsHydraulicLocationEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructureEntity> HeightStructureEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructuresFailureMechanismMetaEntity> HeightStructuresFailureMechanismMetaEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PipingFailureMechanismMetaEntity> PipingFailureMechanismMetaEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityPointStructuresFailureMechanismMetaEntity> StabilityPointStructuresFailureMechanismMetaEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StochasticSoilModelEntity> StochasticSoilModelEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SurfaceLineEntity> SurfaceLineEntities { get; set; }
    }
}
