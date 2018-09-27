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

namespace Ringtoets.Storage.Core.DbContext
{
    public class HydraulicLocationCalculationCollectionEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HydraulicLocationCalculationCollectionEntity()
        {
            AssessmentSectionEntities = new HashSet<AssessmentSectionEntity>();
            AssessmentSectionEntities1 = new HashSet<AssessmentSectionEntity>();
            AssessmentSectionEntities2 = new HashSet<AssessmentSectionEntity>();
            AssessmentSectionEntities3 = new HashSet<AssessmentSectionEntity>();
            AssessmentSectionEntities4 = new HashSet<AssessmentSectionEntity>();
            AssessmentSectionEntities5 = new HashSet<AssessmentSectionEntity>();
            AssessmentSectionEntities6 = new HashSet<AssessmentSectionEntity>();
            AssessmentSectionEntities7 = new HashSet<AssessmentSectionEntity>();
            GrassCoverErosionOutwardsFailureMechanismMetaEntities = new HashSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity>();
            GrassCoverErosionOutwardsFailureMechanismMetaEntities1 = new HashSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity>();
            GrassCoverErosionOutwardsFailureMechanismMetaEntities2 = new HashSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity>();
            GrassCoverErosionOutwardsFailureMechanismMetaEntities3 = new HashSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity>();
            GrassCoverErosionOutwardsFailureMechanismMetaEntities4 = new HashSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity>();
            GrassCoverErosionOutwardsFailureMechanismMetaEntities5 = new HashSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity>();
            HydraulicLocationCalculationEntities = new HashSet<HydraulicLocationCalculationEntity>();
        }

        public long HydraulicLocationCalculationCollectionEntityId { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssessmentSectionEntity> AssessmentSectionEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssessmentSectionEntity> AssessmentSectionEntities1 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssessmentSectionEntity> AssessmentSectionEntities2 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssessmentSectionEntity> AssessmentSectionEntities3 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssessmentSectionEntity> AssessmentSectionEntities4 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssessmentSectionEntity> AssessmentSectionEntities5 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssessmentSectionEntity> AssessmentSectionEntities6 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssessmentSectionEntity> AssessmentSectionEntities7 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities1 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities2 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities3 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities4 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities5 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HydraulicLocationCalculationEntity> HydraulicLocationCalculationEntities { get; set; }
    }
}