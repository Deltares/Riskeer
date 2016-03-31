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
            var pSet = DbTestSet.GetDbTestSet(new ObservableCollection<ProjectEntity>());
            var hlSet = DbTestSet.GetDbTestSet(new ObservableCollection<HydraulicLocationEntity>());
            var fmSet = DbTestSet.GetDbTestSet(new ObservableCollection<FailureMechanismEntity>());
            var dasSet = DbTestSet.GetDbTestSet(new ObservableCollection<AssessmentSectionEntity>());
            var rlpSet = DbTestSet.GetDbTestSet(new ObservableCollection<ReferenceLinePointEntity>());
            ringtoetsEntities.Stub(r => r.ProjectEntities).Return(pSet);
            ringtoetsEntities.Stub(r => r.HydraulicLocationEntities).Return(hlSet);
            ringtoetsEntities.Stub(r => r.FailureMechanismEntities).Return(fmSet);
            ringtoetsEntities.Stub(r => r.AssessmentSectionEntities).Return(dasSet);
            ringtoetsEntities.Stub(r => r.ReferenceLinePointEntities).Return(rlpSet);
            return ringtoetsEntities;
        }
    }
}