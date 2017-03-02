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
    
    public partial class ForeshoreProfileEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ForeshoreProfileEntity()
        {
            ClosingStructuresCalculationEntities = new HashSet<ClosingStructuresCalculationEntity>();
            GrassCoverErosionOutwardsWaveConditionsCalculationEntities = new HashSet<GrassCoverErosionOutwardsWaveConditionsCalculationEntity>();
            HeightStructuresCalculationEntities = new HashSet<HeightStructuresCalculationEntity>();
            StabilityPointStructuresCalculationEntities = new HashSet<StabilityPointStructuresCalculationEntity>();
            StabilityStoneCoverWaveConditionsCalculationEntities = new HashSet<StabilityStoneCoverWaveConditionsCalculationEntity>();
            WaveImpactAsphaltCoverWaveConditionsCalculationEntities = new HashSet<WaveImpactAsphaltCoverWaveConditionsCalculationEntity>();
        }
    
        public long ForeshoreProfileEntityId { get; set; }
        public long FailureMechanismEntityId { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public Nullable<double> Orientation { get; set; }
        public Nullable<byte> BreakWaterType { get; set; }
        public Nullable<double> BreakWaterHeight { get; set; }
        public string GeometryXml { get; set; }
        public Nullable<double> X { get; set; }
        public Nullable<double> Y { get; set; }
        public Nullable<double> X0 { get; set; }
        public int Order { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructuresCalculationEntity> ClosingStructuresCalculationEntities { get; set; }
        public virtual FailureMechanismEntity FailureMechanismEntity { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionOutwardsWaveConditionsCalculationEntity> GrassCoverErosionOutwardsWaveConditionsCalculationEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructuresCalculationEntity> HeightStructuresCalculationEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityPointStructuresCalculationEntity> StabilityPointStructuresCalculationEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityStoneCoverWaveConditionsCalculationEntity> StabilityStoneCoverWaveConditionsCalculationEntities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WaveImpactAsphaltCoverWaveConditionsCalculationEntity> WaveImpactAsphaltCoverWaveConditionsCalculationEntities { get; set; }
    }
}
