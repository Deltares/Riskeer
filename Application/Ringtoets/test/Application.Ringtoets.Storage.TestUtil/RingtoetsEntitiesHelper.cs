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

using System.Collections.ObjectModel;
using System.Data.Entity;

using Application.Ringtoets.Storage.DbContext;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.TestUtil
{
    public static class RingtoetsEntitiesHelper
    {
        /// <summary>
        /// Creates a <see cref="IRingtoetsEntities"/> stub using the given mock repository.
        /// </summary>
        /// <param name="mockRepository">The mock repository.</param>
        /// <returns>A stubbed <see cref="IRingtoetsEntities"/> implementation.</returns>
        public static IRingtoetsEntities CreateStub(MockRepository mockRepository)
        {
            DbSet<ProjectEntity> projectsSet = CreateEmptyTestDbSet<ProjectEntity>();
            DbSet<HydraulicLocationEntity> hydraylicLocationsSet = CreateEmptyTestDbSet<HydraulicLocationEntity>();
            DbSet<FailureMechanismEntity> failureMechanismsSet = CreateEmptyTestDbSet<FailureMechanismEntity>();
            DbSet<FailureMechanismSectionEntity> failureMechanismSectionsSet = CreateEmptyTestDbSet<FailureMechanismSectionEntity>();
            DbSet<FailureMechanismSectionPointEntity> failureMechanismSectionPointsSet = CreateEmptyTestDbSet<FailureMechanismSectionPointEntity>();
            DbSet<PipingSectionResultEntity> pipingSectionResultsSet = CreateEmptyTestDbSet<PipingSectionResultEntity>();
            DbSet<DikeProfileEntity> dikeProfileEntities = CreateEmptyTestDbSet<DikeProfileEntity>();
            DbSet<GrassCoverErosionInwardsFailureMechanismMetaEntity> grassCoverErosionInwardsMetaEntities = CreateEmptyTestDbSet<GrassCoverErosionInwardsFailureMechanismMetaEntity>();
            DbSet<GrassCoverErosionInwardsSectionResultEntity> grassCoverErosionInwardsSectionResultsSet = CreateEmptyTestDbSet<GrassCoverErosionInwardsSectionResultEntity>();
            DbSet<HeightStructuresSectionResultEntity> heightStructuresSectionResultsSet = CreateEmptyTestDbSet<HeightStructuresSectionResultEntity>();
            DbSet<StrengthStabilityLengthwiseConstructionSectionResultEntity> strengthStabilityLengthwiseConstructionSectionResultsSet = CreateEmptyTestDbSet<StrengthStabilityLengthwiseConstructionSectionResultEntity>();
            DbSet<TechnicalInnovationSectionResultEntity> technicalInnovationSectionResultsSet = CreateEmptyTestDbSet<TechnicalInnovationSectionResultEntity>();
            DbSet<WaterPressureAsphaltCoverSectionResultEntity> waterPressureAsphaltCoverSectionResultsSet = CreateEmptyTestDbSet<WaterPressureAsphaltCoverSectionResultEntity>();
            DbSet<ClosingStructureSectionResultEntity> closingStructureSectionResultsSet = CreateEmptyTestDbSet<ClosingStructureSectionResultEntity>();
            DbSet<MacrostabilityInwardsSectionResultEntity> macrostabilityInwardsSectionResultsSet = CreateEmptyTestDbSet<MacrostabilityInwardsSectionResultEntity>();
            DbSet<MacrostabilityOutwardsSectionResultEntity> macrostabilityOutwardsSectionResultsSet = CreateEmptyTestDbSet<MacrostabilityOutwardsSectionResultEntity>();
            DbSet<WaveImpactAsphaltCoverSectionResultEntity> waveImpactAsphaltCoverSectionResultsSet = CreateEmptyTestDbSet<WaveImpactAsphaltCoverSectionResultEntity>();
            DbSet<GrassCoverErosionOutwardsSectionResultEntity> grassCoverErosionOutwardsSectionResultsSet = CreateEmptyTestDbSet<GrassCoverErosionOutwardsSectionResultEntity>();
            DbSet<GrassCoverSlipOffInwardsSectionResultEntity> grassCoverSlipOffInwardsSectionResultsSet = CreateEmptyTestDbSet<GrassCoverSlipOffInwardsSectionResultEntity>();
            DbSet<GrassCoverSlipOffOutwardsSectionResultEntity> grassCoverSlipOffOutwardsSectionResultsSet = CreateEmptyTestDbSet<GrassCoverSlipOffOutwardsSectionResultEntity>();
            DbSet<MicrostabilitySectionResultEntity> microstabilitySectionResultsSet = CreateEmptyTestDbSet<MicrostabilitySectionResultEntity>();
            DbSet<PipingStructureSectionResultEntity> pipingStructureSectionResultsSet = CreateEmptyTestDbSet<PipingStructureSectionResultEntity>();
            DbSet<DuneErosionSectionResultEntity> duneErosionSectionResultsSet = CreateEmptyTestDbSet<DuneErosionSectionResultEntity>();
            DbSet<StabilityStoneCoverSectionResultEntity> stabilityStoneCoverSectionResultsSet = CreateEmptyTestDbSet<StabilityStoneCoverSectionResultEntity>();
            DbSet<StrengthStabilityPointConstructionSectionResultEntity> strengthStabilityPointConstructionSectionResultsSet = CreateEmptyTestDbSet<StrengthStabilityPointConstructionSectionResultEntity>();
            DbSet<AssessmentSectionEntity> assessmentSectionsSet = CreateEmptyTestDbSet<AssessmentSectionEntity>();
            DbSet<ReferenceLinePointEntity> referenceLinesSet = CreateEmptyTestDbSet<ReferenceLinePointEntity>();
            DbSet<CalculationGroupEntity> calculationGroupsSet = CreateEmptyTestDbSet<CalculationGroupEntity>();
            DbSet<PipingCalculationEntity> pipingCalculationsSet = CreateEmptyTestDbSet<PipingCalculationEntity>();
            DbSet<PipingCalculationOutputEntity> pipingCalculationsOutputsSet = CreateEmptyTestDbSet<PipingCalculationOutputEntity>();
            DbSet<PipingSemiProbabilisticOutputEntity> pipingSemiProbabilisticOutputsSet = CreateEmptyTestDbSet<PipingSemiProbabilisticOutputEntity>();
            DbSet<StochasticSoilModelEntity> stochasticSoilModelsSet = CreateEmptyTestDbSet<StochasticSoilModelEntity>();
            DbSet<StochasticSoilModelSegmentPointEntity> soilModelSegmentPointsSet = CreateEmptyTestDbSet<StochasticSoilModelSegmentPointEntity>();
            DbSet<StochasticSoilProfileEntity> stochasticSoilProfilesSet = CreateEmptyTestDbSet<StochasticSoilProfileEntity>();
            DbSet<SoilProfileEntity> soilProfilesSet = CreateEmptyTestDbSet<SoilProfileEntity>();
            DbSet<SoilLayerEntity> soilLayersSet = CreateEmptyTestDbSet<SoilLayerEntity>();
            DbSet<SurfaceLineEntity> surfaceLinesSet = CreateEmptyTestDbSet<SurfaceLineEntity>();
            DbSet<SurfaceLinePointEntity> surfaceLinePointsSet = CreateEmptyTestDbSet<SurfaceLinePointEntity>();
            DbSet<CharacteristicPointEntity> characteristicPointsSet = CreateEmptyTestDbSet<CharacteristicPointEntity>();
            DbSet<PipingFailureMechanismMetaEntity> failureMechanismMetaSet = CreateEmptyTestDbSet<PipingFailureMechanismMetaEntity>();

            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            ringtoetsEntities.Stub(r => r.ProjectEntities).Return(projectsSet);
            ringtoetsEntities.Stub(r => r.HydraulicLocationEntities).Return(hydraylicLocationsSet);
            ringtoetsEntities.Stub(r => r.FailureMechanismEntities).Return(failureMechanismsSet);
            ringtoetsEntities.Stub(r => r.FailureMechanismSectionEntities).Return(failureMechanismSectionsSet);
            ringtoetsEntities.Stub(r => r.FailureMechanismSectionPointEntities).Return(failureMechanismSectionPointsSet);
            ringtoetsEntities.Stub(r => r.PipingSectionResultEntities).Return(pipingSectionResultsSet);
            ringtoetsEntities.Stub(r => r.DikeProfileEntities).Return(dikeProfileEntities);
            ringtoetsEntities.Stub(r => r.GrassCoverErosionInwardsFailureMechanismMetaEntities).Return(grassCoverErosionInwardsMetaEntities);
            ringtoetsEntities.Stub(r => r.GrassCoverErosionInwardsSectionResultEntities).Return(grassCoverErosionInwardsSectionResultsSet);
            ringtoetsEntities.Stub(r => r.HeightStructuresSectionResultEntities).Return(heightStructuresSectionResultsSet);
            ringtoetsEntities.Stub(r => r.StrengthStabilityLengthwiseConstructionSectionResultEntities).Return(strengthStabilityLengthwiseConstructionSectionResultsSet);
            ringtoetsEntities.Stub(r => r.TechnicalInnovationSectionResultEntities).Return(technicalInnovationSectionResultsSet);
            ringtoetsEntities.Stub(r => r.WaterPressureAsphaltCoverSectionResultEntities).Return(waterPressureAsphaltCoverSectionResultsSet);
            ringtoetsEntities.Stub(r => r.ClosingStructureSectionResultEntities).Return(closingStructureSectionResultsSet);
            ringtoetsEntities.Stub(r => r.MacrostabilityInwardsSectionResultEntities).Return(macrostabilityInwardsSectionResultsSet);
            ringtoetsEntities.Stub(r => r.MacrostabilityOutwardsSectionResultEntities).Return(macrostabilityOutwardsSectionResultsSet);
            ringtoetsEntities.Stub(r => r.WaveImpactAsphaltCoverSectionResultEntities).Return(waveImpactAsphaltCoverSectionResultsSet);
            ringtoetsEntities.Stub(r => r.GrassCoverErosionOutwardsSectionResultEntities).Return(grassCoverErosionOutwardsSectionResultsSet);
            ringtoetsEntities.Stub(r => r.GrassCoverSlipOffInwardsSectionResultEntities).Return(grassCoverSlipOffInwardsSectionResultsSet);
            ringtoetsEntities.Stub(r => r.GrassCoverSlipOffOutwardsSectionResultEntities).Return(grassCoverSlipOffOutwardsSectionResultsSet);
            ringtoetsEntities.Stub(r => r.MicrostabilitySectionResultEntities).Return(microstabilitySectionResultsSet);
            ringtoetsEntities.Stub(r => r.PipingStructureSectionResultEntities).Return(pipingStructureSectionResultsSet);
            ringtoetsEntities.Stub(r => r.DuneErosionSectionResultEntities).Return(duneErosionSectionResultsSet);
            ringtoetsEntities.Stub(r => r.StabilityStoneCoverSectionResultEntities).Return(stabilityStoneCoverSectionResultsSet);
            ringtoetsEntities.Stub(r => r.StrengthStabilityPointConstructionSectionResultEntities).Return(strengthStabilityPointConstructionSectionResultsSet);
            ringtoetsEntities.Stub(r => r.AssessmentSectionEntities).Return(assessmentSectionsSet);
            ringtoetsEntities.Stub(r => r.ReferenceLinePointEntities).Return(referenceLinesSet);
            ringtoetsEntities.Stub(r => r.CalculationGroupEntities).Return(calculationGroupsSet);
            ringtoetsEntities.Stub(r => r.PipingCalculationEntities).Return(pipingCalculationsSet);
            ringtoetsEntities.Stub(r => r.PipingCalculationOutputEntities).Return(pipingCalculationsOutputsSet);
            ringtoetsEntities.Stub(r => r.PipingSemiProbabilisticOutputEntities).Return(pipingSemiProbabilisticOutputsSet);
            ringtoetsEntities.Stub(r => r.StochasticSoilModelEntities).Return(stochasticSoilModelsSet);
            ringtoetsEntities.Stub(r => r.StochasticSoilModelSegmentPointEntities).Return(soilModelSegmentPointsSet);
            ringtoetsEntities.Stub(r => r.StochasticSoilProfileEntities).Return(stochasticSoilProfilesSet);
            ringtoetsEntities.Stub(r => r.SoilProfileEntities).Return(soilProfilesSet);
            ringtoetsEntities.Stub(r => r.SoilLayerEntities).Return(soilLayersSet);
            ringtoetsEntities.Stub(r => r.SurfaceLineEntities).Return(surfaceLinesSet);
            ringtoetsEntities.Stub(r => r.SurfaceLinePointEntities).Return(surfaceLinePointsSet);
            ringtoetsEntities.Stub(r => r.CharacteristicPointEntities).Return(characteristicPointsSet);
            ringtoetsEntities.Stub(r => r.PipingFailureMechanismMetaEntities).Return(failureMechanismMetaSet);
            return ringtoetsEntities;
        }

        private static DbSet<T> CreateEmptyTestDbSet<T>() where T : class
        {
            return new TestDbSet<T>(new ObservableCollection<T>());
        }
    }
}