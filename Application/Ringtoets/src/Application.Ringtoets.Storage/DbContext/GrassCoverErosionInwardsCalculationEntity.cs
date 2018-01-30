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
    public class GrassCoverErosionInwardsCalculationEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GrassCoverErosionInwardsCalculationEntity()
        {
            GrassCoverErosionInwardsOutputEntities = new HashSet<GrassCoverErosionInwardsOutputEntity>();
            GrassCoverErosionInwardsSectionResultEntities = new HashSet<GrassCoverErosionInwardsSectionResultEntity>();
        }

        public long GrassCoverErosionInwardsCalculationEntityId { get; set; }
        public long CalculationGroupEntityId { get; set; }
        public long? HydraulicLocationEntityId { get; set; }
        public long? DikeProfileEntityId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public double? Orientation { get; set; }
        public double? CriticalFlowRateMean { get; set; }
        public double? CriticalFlowRateStandardDeviation { get; set; }
        public byte UseForeshore { get; set; }
        public byte DikeHeightCalculationType { get; set; }
        public double? DikeHeight { get; set; }
        public byte UseBreakWater { get; set; }
        public byte BreakWaterType { get; set; }
        public double? BreakWaterHeight { get; set; }
        public byte OvertoppingRateCalculationType { get; set; }
        public byte ShouldDikeHeightIllustrationPointsBeCalculated { get; set; }
        public byte ShouldOvertoppingRateIllustrationPointsBeCalculated { get; set; }
        public byte ShouldOvertoppingOutputIllustrationPointsBeCalculated { get; set; }

        public virtual CalculationGroupEntity CalculationGroupEntity { get; set; }
        public virtual DikeProfileEntity DikeProfileEntity { get; set; }
        public virtual HydraulicLocationEntity HydraulicLocationEntity { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsOutputEntity> GrassCoverErosionInwardsOutputEntities { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GrassCoverErosionInwardsSectionResultEntity> GrassCoverErosionInwardsSectionResultEntities { get; set; }
    }
}