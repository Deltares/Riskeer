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
    public class AssessmentSectionEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AssessmentSectionEntity()
        {
            BackgroundDataEntities = new HashSet<BackgroundDataEntity>();
            FailureMechanismEntities = new HashSet<FailureMechanismEntity>();
            HydraulicBoundaryDatabaseEntities = new HashSet<HydraulicBoundaryDatabaseEntity>();
            HydraulicLocationCalculationForTargetProbabilityCollectionEntities = new HashSet<HydraulicLocationCalculationForTargetProbabilityCollectionEntity>();
            HydraulicLocationEntities = new HashSet<HydraulicLocationEntity>();
            SpecificFailurePathEntities = new HashSet<SpecificFailurePathEntity>();
        }

        public long AssessmentSectionEntityId { get; set; }
        public long ProjectEntityId { get; set; }
        public long HydraulicLocationCalculationCollectionEntity1Id { get; set; }
        public long HydraulicLocationCalculationCollectionEntity2Id { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public double LowerLimitNorm { get; set; }
        public double SignalingNorm { get; set; }
        public byte NormativeNormType { get; set; }
        public byte Composition { get; set; }
        public string ReferenceLinePointXml { get; set; }
        public int Order { get; set; }

        public virtual ProjectEntity ProjectEntity { get; set; }
        public virtual HydraulicLocationCalculationCollectionEntity HydraulicLocationCalculationCollectionEntity { get; set; }
        public virtual HydraulicLocationCalculationCollectionEntity HydraulicLocationCalculationCollectionEntity1 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BackgroundDataEntity> BackgroundDataEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FailureMechanismEntity> FailureMechanismEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HydraulicBoundaryDatabaseEntity> HydraulicBoundaryDatabaseEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HydraulicLocationCalculationForTargetProbabilityCollectionEntity> HydraulicLocationCalculationForTargetProbabilityCollectionEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HydraulicLocationEntity> HydraulicLocationEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SpecificFailurePathEntity> SpecificFailurePathEntities { get; set; }
    }
}