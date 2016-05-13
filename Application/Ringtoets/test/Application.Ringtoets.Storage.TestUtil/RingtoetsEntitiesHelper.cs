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
using Application.Ringtoets.Storage.DbContext;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.TestUtil
{
    public static class RingtoetsEntitiesHelper
    {
        public static IRingtoetsEntities Create(MockRepository mockRepository)
        {
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            var pSet = new TestDbSet<ProjectEntity>(new ObservableCollection<ProjectEntity>());
            var hlSet = new TestDbSet<HydraulicLocationEntity>(new ObservableCollection<HydraulicLocationEntity>());
            var fmSet = new TestDbSet<FailureMechanismEntity>(new ObservableCollection<FailureMechanismEntity>());
            var fmsSet = new TestDbSet<FailureMechanismSectionEntity>(new ObservableCollection<FailureMechanismSectionEntity>());
            var fmspSet = new TestDbSet<FailureMechanismSectionPointEntity>(new ObservableCollection<FailureMechanismSectionPointEntity>());
            var dasSet = new TestDbSet<AssessmentSectionEntity>(new ObservableCollection<AssessmentSectionEntity>());
            var rlpSet = new TestDbSet<ReferenceLinePointEntity>(new ObservableCollection<ReferenceLinePointEntity>());
            var ssmSet = new TestDbSet<StochasticSoilModelEntity>(new ObservableCollection<StochasticSoilModelEntity>());
            var sspSet = new TestDbSet<StochasticSoilProfileEntity>(new ObservableCollection<StochasticSoilProfileEntity>());
            var spSet = new TestDbSet<SoilProfileEntity>(new ObservableCollection<SoilProfileEntity>());
            var slSet = new TestDbSet<SoilLayerEntity>(new ObservableCollection<SoilLayerEntity>());
            ringtoetsEntities.Stub(r => r.ProjectEntities).Return(pSet);
            ringtoetsEntities.Stub(r => r.HydraulicLocationEntities).Return(hlSet);
            ringtoetsEntities.Stub(r => r.FailureMechanismEntities).Return(fmSet);
            ringtoetsEntities.Stub(r => r.FailureMechanismSectionEntities).Return(fmsSet);
            ringtoetsEntities.Stub(r => r.FailureMechanismSectionPointEntities).Return(fmspSet);
            ringtoetsEntities.Stub(r => r.AssessmentSectionEntities).Return(dasSet);
            ringtoetsEntities.Stub(r => r.ReferenceLinePointEntities).Return(rlpSet);
            ringtoetsEntities.Stub(r => r.StochasticSoilModelEntities).Return(ssmSet);
            ringtoetsEntities.Stub(r => r.StochasticSoilProfileEntities).Return(sspSet);
            ringtoetsEntities.Stub(r => r.SoilProfileEntities).Return(spSet);
            ringtoetsEntities.Stub(r => r.SoilLayerEntities).Return(slSet);
            return ringtoetsEntities;
        }
    }
}