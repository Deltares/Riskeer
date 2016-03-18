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

using System;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Converters
{
    public class FailureMechanismEntityConverter<T> : IEntityConverter<T, FailureMechanismEntity> where T : IFailureMechanism
    {
        public T ConvertEntityToModel(FailureMechanismEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            T failureMechanism = Activator.CreateInstance<T>();
            failureMechanism.StorageId = entity.FailureMechanismEntityId;

            return failureMechanism;
        }

        public void ConvertModelToEntity(T modelObject, FailureMechanismEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            entity.FailureMechanismEntityId = modelObject.StorageId;
            if (modelObject is PipingFailureMechanism)
            {
                entity.FailureMechanismType = (int)FailureMechanismType.DikesPipingFailureMechanism;
            }
        }
    }
}