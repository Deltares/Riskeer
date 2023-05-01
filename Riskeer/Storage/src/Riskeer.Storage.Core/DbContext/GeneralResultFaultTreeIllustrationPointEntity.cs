// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
    public partial class GeneralResultFaultTreeIllustrationPointEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GeneralResultFaultTreeIllustrationPointEntity()
        {
            ClosingStructuresOutputEntities = new HashSet<ClosingStructuresOutputEntity>();
            GrassCoverErosionInwardsDikeHeightOutputEntities = new HashSet<GrassCoverErosionInwardsDikeHeightOutputEntity>();
            GrassCoverErosionInwardsOutputEntities = new HashSet<GrassCoverErosionInwardsOutputEntity>();
            GrassCoverErosionInwardsOvertoppingRateOutputEntities = new HashSet<GrassCoverErosionInwardsOvertoppingRateOutputEntity>();
            HeightStructuresOutputEntities = new HashSet<HeightStructuresOutputEntity>();
            ProbabilisticPipingCalculationOutputEntities = new HashSet<ProbabilisticPipingCalculationOutputEntity>();
            ProbabilisticPipingCalculationOutputEntities1 = new HashSet<ProbabilisticPipingCalculationOutputEntity>();
            StabilityPointStructuresOutputEntities = new HashSet<StabilityPointStructuresOutputEntity>();
            TopLevelFaultTreeIllustrationPointEntities = new HashSet<TopLevelFaultTreeIllustrationPointEntity>();
            StochastEntities = new HashSet<StochastEntity>();
        }

        public long GeneralResultFaultTreeIllustrationPointEntityId { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClosingStructuresOutputEntity> ClosingStructuresOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsDikeHeightOutputEntity> GrassCoverErosionInwardsDikeHeightOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsOutputEntity> GrassCoverErosionInwardsOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsOvertoppingRateOutputEntity> GrassCoverErosionInwardsOvertoppingRateOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HeightStructuresOutputEntity> HeightStructuresOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProbabilisticPipingCalculationOutputEntity> ProbabilisticPipingCalculationOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProbabilisticPipingCalculationOutputEntity> ProbabilisticPipingCalculationOutputEntities1 { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StabilityPointStructuresOutputEntity> StabilityPointStructuresOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TopLevelFaultTreeIllustrationPointEntity> TopLevelFaultTreeIllustrationPointEntities { get; set; }

        public string GoverningWindDirectionName { get; set; }
        public double GoverningWindDirectionAngle { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StochastEntity> StochastEntities { get; set; }
    }
}