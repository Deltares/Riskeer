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

using System.Collections.Generic;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Persistors
{
    public class HydraulicLocationEntityPersistor : ICollectionPersistor<HydraulicLocationEntity, ICollection<HydraulicBoundaryLocation>>
    {
        private readonly IRingtoetsEntities ringtoetsContext;

        public HydraulicLocationEntityPersistor(IRingtoetsEntities ringtoetsContext)
        {
            this.ringtoetsContext = ringtoetsContext;
        }

        public void RemoveUnModifiedEntries(ICollection<HydraulicLocationEntity> parentNavigationProperty)
        {
            throw new System.NotImplementedException();
        }

        public void PerformPostSaveActions()
        {
            throw new System.NotImplementedException();
        }

        public void LoadModel(HydraulicLocationEntity entity, ICollection<HydraulicBoundaryLocation> model)
        {
            throw new System.NotImplementedException();
        }

        public void InsertModel(ICollection<HydraulicLocationEntity> parentNavigationProperty, ICollection<HydraulicBoundaryLocation> model)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateModel(ICollection<HydraulicLocationEntity> parentNavigationProperty, ICollection<HydraulicBoundaryLocation> model)
        {
            throw new System.NotImplementedException();
        }

        public void ConvertEntityToModel(HydraulicLocationEntity entity, ICollection<HydraulicBoundaryLocation> model)
        {
            throw new System.NotImplementedException();
        }

        public void ConvertModelToEntity(ICollection<HydraulicBoundaryLocation> model, HydraulicLocationEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
