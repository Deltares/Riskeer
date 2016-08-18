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
    
    public partial class AssessmentSectionEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AssessmentSectionEntity()
        {
            this.FailureMechanismEntities = new HashSet<FailureMechanismEntity>();
            this.HydraulicLocationEntities = new HashSet<HydraulicLocationEntity>();
        }
    
        public long AssessmentSectionEntityId { get; set; }
        public string Id { get; set; }
        public long ProjectEntityId { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public int Norm { get; set; }
        public string HydraulicDatabaseVersion { get; set; }
        public string HydraulicDatabaseLocation { get; set; }
        public short Composition { get; set; }
        public string ReferenceLinePointXml { get; set; }
        public int Order { get; set; }
    
        public virtual ProjectEntity ProjectEntity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FailureMechanismEntity> FailureMechanismEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HydraulicLocationEntity> HydraulicLocationEntities { get; set; }
    }
}
