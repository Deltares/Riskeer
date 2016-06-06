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
            DbSet<AssessmentSectionEntity> assessmentSectionsSet = CreateEmptyTestDbSet<AssessmentSectionEntity>();
            DbSet<ReferenceLinePointEntity> referenceLinesSet = CreateEmptyTestDbSet<ReferenceLinePointEntity>();
            DbSet<CalculationGroupEntity> calculationGroupsSet = CreateEmptyTestDbSet<CalculationGroupEntity>();
            DbSet<PipingCalculationEntity> pipingCalculationsSet = CreateEmptyTestDbSet<PipingCalculationEntity>();
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
            ringtoetsEntities.Stub(r => r.AssessmentSectionEntities).Return(assessmentSectionsSet);
            ringtoetsEntities.Stub(r => r.ReferenceLinePointEntities).Return(referenceLinesSet);
            ringtoetsEntities.Stub(r => r.CalculationGroupEntities).Return(calculationGroupsSet);
            ringtoetsEntities.Stub(r => r.PipingCalculationEntities).Return(pipingCalculationsSet);
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