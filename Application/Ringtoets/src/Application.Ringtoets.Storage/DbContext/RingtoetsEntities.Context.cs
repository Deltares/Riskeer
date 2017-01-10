﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Application.Ringtoets.Storage.DbContext
{
    public partial class RingtoetsEntities : System.Data.Entity.DbContext
    {
        public virtual DbSet<AssessmentSectionEntity> AssessmentSectionEntities { get; set; }
        public virtual DbSet<CalculationGroupEntity> CalculationGroupEntities { get; set; }
        public virtual DbSet<CharacteristicPointEntity> CharacteristicPointEntities { get; set; }
        public virtual DbSet<ClosingStructureEntity> ClosingStructureEntities { get; set; }
        public virtual DbSet<ClosingStructuresCalculationEntity> ClosingStructuresCalculationEntities { get; set; }
        public virtual DbSet<ClosingStructuresFailureMechanismMetaEntity> ClosingStructuresFailureMechanismMetaEntities { get; set; }
        public virtual DbSet<ClosingStructuresOutputEntity> ClosingStructuresOutputEntities { get; set; }
        public virtual DbSet<ClosingStructuresSectionResultEntity> ClosingStructuresSectionResultEntities { get; set; }
        public virtual DbSet<DikeProfileEntity> DikeProfileEntities { get; set; }
        public virtual DbSet<DuneErosionFailureMechanismMetaEntity> DuneErosionFailureMechanismMetaEntities { get; set; }
        public virtual DbSet<DuneErosionSectionResultEntity> DuneErosionSectionResultEntities { get; set; }
        public virtual DbSet<DuneLocationEntity> DuneLocationEntities { get; set; }
        public virtual DbSet<DuneLocationOutputEntity> DuneLocationOutputEntities { get; set; }
        public virtual DbSet<FailureMechanismEntity> FailureMechanismEntities { get; set; }
        public virtual DbSet<FailureMechanismSectionEntity> FailureMechanismSectionEntities { get; set; }
        public virtual DbSet<ForeshoreProfileEntity> ForeshoreProfileEntities { get; set; }
        public virtual DbSet<GrassCoverErosionInwardsCalculationEntity> GrassCoverErosionInwardsCalculationEntities { get; set; }
        public virtual DbSet<GrassCoverErosionInwardsDikeHeightOutputEntity> GrassCoverErosionInwardsDikeHeightOutputEntities { get; set; }
        public virtual DbSet<GrassCoverErosionInwardsFailureMechanismMetaEntity> GrassCoverErosionInwardsFailureMechanismMetaEntities { get; set; }
        public virtual DbSet<GrassCoverErosionInwardsOutputEntity> GrassCoverErosionInwardsOutputEntities { get; set; }
        public virtual DbSet<GrassCoverErosionInwardsSectionResultEntity> GrassCoverErosionInwardsSectionResultEntities { get; set; }
        public virtual DbSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities { get; set; }
        public virtual DbSet<GrassCoverErosionOutwardsHydraulicLocationEntity> GrassCoverErosionOutwardsHydraulicLocationEntities { get; set; }
        public virtual DbSet<GrassCoverErosionOutwardsHydraulicLocationOutputEntity> GrassCoverErosionOutwardsHydraulicLocationOutputEntities { get; set; }
        public virtual DbSet<GrassCoverErosionOutwardsSectionResultEntity> GrassCoverErosionOutwardsSectionResultEntities { get; set; }
        public virtual DbSet<GrassCoverErosionOutwardsWaveConditionsCalculationEntity> GrassCoverErosionOutwardsWaveConditionsCalculationEntities { get; set; }
        public virtual DbSet<GrassCoverErosionOutwardsWaveConditionsOutputEntity> GrassCoverErosionOutwardsWaveConditionsOutputEntities { get; set; }
        public virtual DbSet<GrassCoverSlipOffInwardsSectionResultEntity> GrassCoverSlipOffInwardsSectionResultEntities { get; set; }
        public virtual DbSet<GrassCoverSlipOffOutwardsSectionResultEntity> GrassCoverSlipOffOutwardsSectionResultEntities { get; set; }
        public virtual DbSet<HeightStructureEntity> HeightStructureEntities { get; set; }
        public virtual DbSet<HeightStructuresCalculationEntity> HeightStructuresCalculationEntities { get; set; }
        public virtual DbSet<HeightStructuresFailureMechanismMetaEntity> HeightStructuresFailureMechanismMetaEntities { get; set; }
        public virtual DbSet<HeightStructuresOutputEntity> HeightStructuresOutputEntities { get; set; }
        public virtual DbSet<HeightStructuresSectionResultEntity> HeightStructuresSectionResultEntities { get; set; }
        public virtual DbSet<HydraulicLocationEntity> HydraulicLocationEntities { get; set; }
        public virtual DbSet<HydraulicLocationOutputEntity> HydraulicLocationOutputEntities { get; set; }
        public virtual DbSet<MacrostabilityInwardsSectionResultEntity> MacrostabilityInwardsSectionResultEntities { get; set; }
        public virtual DbSet<MacrostabilityOutwardsSectionResultEntity> MacrostabilityOutwardsSectionResultEntities { get; set; }
        public virtual DbSet<MicrostabilitySectionResultEntity> MicrostabilitySectionResultEntities { get; set; }
        public virtual DbSet<PipingCalculationEntity> PipingCalculationEntities { get; set; }
        public virtual DbSet<PipingCalculationOutputEntity> PipingCalculationOutputEntities { get; set; }
        public virtual DbSet<PipingFailureMechanismMetaEntity> PipingFailureMechanismMetaEntities { get; set; }
        public virtual DbSet<PipingSectionResultEntity> PipingSectionResultEntities { get; set; }
        public virtual DbSet<PipingSemiProbabilisticOutputEntity> PipingSemiProbabilisticOutputEntities { get; set; }
        public virtual DbSet<PipingStructureSectionResultEntity> PipingStructureSectionResultEntities { get; set; }
        public virtual DbSet<ProjectEntity> ProjectEntities { get; set; }
        public virtual DbSet<SoilLayerEntity> SoilLayerEntities { get; set; }
        public virtual DbSet<SoilProfileEntity> SoilProfileEntities { get; set; }
        public virtual DbSet<StabilityPointStructureEntity> StabilityPointStructureEntities { get; set; }
        public virtual DbSet<StabilityPointStructuresCalculationEntity> StabilityPointStructuresCalculationEntities { get; set; }
        public virtual DbSet<StabilityPointStructuresFailureMechanismMetaEntity> StabilityPointStructuresFailureMechanismMetaEntities { get; set; }
        public virtual DbSet<StabilityPointStructuresOutputEntity> StabilityPointStructuresOutputEntities { get; set; }
        public virtual DbSet<StabilityPointStructuresSectionResultEntity> StabilityPointStructuresSectionResultEntities { get; set; }
        public virtual DbSet<StabilityStoneCoverSectionResultEntity> StabilityStoneCoverSectionResultEntities { get; set; }
        public virtual DbSet<StabilityStoneCoverWaveConditionsCalculationEntity> StabilityStoneCoverWaveConditionsCalculationEntities { get; set; }
        public virtual DbSet<StabilityStoneCoverWaveConditionsOutputEntity> StabilityStoneCoverWaveConditionsOutputEntities { get; set; }
        public virtual DbSet<StochasticSoilModelEntity> StochasticSoilModelEntities { get; set; }
        public virtual DbSet<StochasticSoilProfileEntity> StochasticSoilProfileEntities { get; set; }
        public virtual DbSet<StrengthStabilityLengthwiseConstructionSectionResultEntity> StrengthStabilityLengthwiseConstructionSectionResultEntities { get; set; }
        public virtual DbSet<SurfaceLineEntity> SurfaceLineEntities { get; set; }
        public virtual DbSet<TechnicalInnovationSectionResultEntity> TechnicalInnovationSectionResultEntities { get; set; }
        public virtual DbSet<VersionEntity> VersionEntities { get; set; }
        public virtual DbSet<WaterPressureAsphaltCoverSectionResultEntity> WaterPressureAsphaltCoverSectionResultEntities { get; set; }
        public virtual DbSet<WaveImpactAsphaltCoverSectionResultEntity> WaveImpactAsphaltCoverSectionResultEntities { get; set; }
        public virtual DbSet<WaveImpactAsphaltCoverWaveConditionsCalculationEntity> WaveImpactAsphaltCoverWaveConditionsCalculationEntities { get; set; }
        public virtual DbSet<WaveImpactAsphaltCoverWaveConditionsOutputEntity> WaveImpactAsphaltCoverWaveConditionsOutputEntities { get; set; }
    
        /// <summary>
        /// This method is called in a 'code first' approach when the model for a derived <see cref="DbContext"/> has been initialized,
        /// but before the model has been locked down and used to initialize the <see cref="DbContext"/>.
        /// </summary>
        /// <param name="modelBuilder">The <see cref="DbModelBuilder"/> that defines the model for the context being created.</param>
        /// <exception cref="UnintentionalCodeFirstException">Thrown because the <see cref="DbContext"/> is created in a 'code first' approach.</exception>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    }
}