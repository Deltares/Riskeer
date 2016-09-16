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

using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Interface that describes the properties and methods that must be implemented on classes that extend from a database context.
    /// </summary>
    public interface IRingtoetsEntities
    {
        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="ProjectEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<ProjectEntity> ProjectEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="AssessmentSectionEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<AssessmentSectionEntity> AssessmentSectionEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="FailureMechanismEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<FailureMechanismEntity> FailureMechanismEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="FailureMechanismSectionEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<FailureMechanismSectionEntity> FailureMechanismSectionEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="PipingSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<PipingSectionResultEntity> PipingSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="DikeProfileEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<DikeProfileEntity> DikeProfileEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="GrassCoverErosionInwardsFailureMechanismMetaEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<GrassCoverErosionInwardsFailureMechanismMetaEntity> GrassCoverErosionInwardsFailureMechanismMetaEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="GrassCoverErosionInwardsCalculationEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<GrassCoverErosionInwardsCalculationEntity> GrassCoverErosionInwardsCalculationEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="GrassCoverErosionInwardsOutputEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<GrassCoverErosionInwardsOutputEntity> GrassCoverErosionInwardsOutputEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="GrassCoverErosionInwardsSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<GrassCoverErosionInwardsSectionResultEntity> GrassCoverErosionInwardsSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="HeightStructuresSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<HeightStructuresSectionResultEntity> HeightStructuresSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="StrengthStabilityLengthwiseConstructionSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<StrengthStabilityLengthwiseConstructionSectionResultEntity> StrengthStabilityLengthwiseConstructionSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="TechnicalInnovationSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<TechnicalInnovationSectionResultEntity> TechnicalInnovationSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="WaterPressureAsphaltCoverSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<WaterPressureAsphaltCoverSectionResultEntity> WaterPressureAsphaltCoverSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="ClosingStructureSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<ClosingStructureSectionResultEntity> ClosingStructureSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="MacrostabilityInwardsSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<MacrostabilityInwardsSectionResultEntity> MacrostabilityInwardsSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="MacrostabilityOutwardsSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<MacrostabilityOutwardsSectionResultEntity> MacrostabilityOutwardsSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="WaveImpactAsphaltCoverSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<WaveImpactAsphaltCoverSectionResultEntity> WaveImpactAsphaltCoverSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="GrassCoverErosionOutwardsFailureMechanismMetaEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<GrassCoverErosionOutwardsFailureMechanismMetaEntity> GrassCoverErosionOutwardsFailureMechanismMetaEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="GrassCoverErosionOutwardsSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<GrassCoverErosionOutwardsSectionResultEntity> GrassCoverErosionOutwardsSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="GrassCoverSlipOffInwardsSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<GrassCoverSlipOffInwardsSectionResultEntity> GrassCoverSlipOffInwardsSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="GrassCoverSlipOffOutwardsSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<GrassCoverSlipOffOutwardsSectionResultEntity> GrassCoverSlipOffOutwardsSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="MicrostabilitySectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<MicrostabilitySectionResultEntity> MicrostabilitySectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="PipingStructureSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<PipingStructureSectionResultEntity> PipingStructureSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="DuneErosionSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<DuneErosionSectionResultEntity> DuneErosionSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="StabilityStoneCoverSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<StabilityStoneCoverSectionResultEntity> StabilityStoneCoverSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="StrengthStabilityPointConstructionSectionResultEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<StrengthStabilityPointConstructionSectionResultEntity> StrengthStabilityPointConstructionSectionResultEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="PipingFailureMechanismMetaEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<PipingFailureMechanismMetaEntity> PipingFailureMechanismMetaEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="HydraulicLocationEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<HydraulicLocationEntity> HydraulicLocationEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="CalculationGroupEntity"/> containing
        /// every calculation group found in the database.
        /// </summary>
        DbSet<CalculationGroupEntity> CalculationGroupEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="PipingCalculationEntity"/> containing
        /// every calculation group found in the database.
        /// </summary>
        DbSet<PipingCalculationEntity> PipingCalculationEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="PipingCalculationOutputEntity"/>
        /// containing every calculation group found in the database.
        /// </summary>
        DbSet<PipingCalculationOutputEntity> PipingCalculationOutputEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="PipingSemiProbabilisticOutputEntity"/>
        /// containing every calculation group found in the database.
        /// </summary>
        DbSet<PipingSemiProbabilisticOutputEntity> PipingSemiProbabilisticOutputEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="StochasticSoilModelEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<StochasticSoilModelEntity> StochasticSoilModelEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="StochasticSoilProfileEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<StochasticSoilProfileEntity> StochasticSoilProfileEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="SoilProfileEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<SoilProfileEntity> SoilProfileEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="SoilLayerEntity"/> containing
        /// every entity found in the database.
        /// </summary>
        DbSet<SoilLayerEntity> SoilLayerEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="SurfaceLineEntity"/> containing
        /// every piping surface line entity in the database.
        /// </summary>
        DbSet<SurfaceLineEntity> SurfaceLineEntities { get; }

        /// <summary>
        /// Gets a <see cref="DbSet{TEntity}"/> of <see cref="CharacteristicPointEntity"/>
        /// containing every characteristic point of piping surface lines in the database.
        /// </summary>
        DbSet<CharacteristicPointEntity> CharacteristicPointEntities { get; }

        /// <summary> 
        /// Persists all updates to the database and resets change tracking in the object context, see <see cref="ObjectContext.SaveChanges()"/>.
        /// </summary>
        /// <returns>The number of state entries written to the underlying database. This can include state entries for entities and/or relationships. 
        /// Relationship state entries are created for many-to-many relationships and relationships where there is no foreign key property included in the entity class 
        /// (often referred to as independent associations).</returns>
        /// <exception cref="OptimisticConcurrencyException">An optimistic concurrency violation has occurred while saving changes.</exception>
        int SaveChanges();
    }
}