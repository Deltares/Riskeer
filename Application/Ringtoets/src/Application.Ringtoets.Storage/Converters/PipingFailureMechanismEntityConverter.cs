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
    /// <summary>
    /// Converter for <see cref="FailureMechanismEntity"/> to <see cref="IFailureMechanism"/> 
    /// and <see cref="IFailureMechanism"/> to <see cref="FailureMechanismEntity"/>.
    /// </summary>
    public class PipingFailureMechanismEntityConverter : IEntityConverter<PipingFailureMechanism, FailureMechanismEntity>
    {
        public PipingFailureMechanism ConvertEntityToModel(FailureMechanismEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (entity.FailureMechanismType != (int) FailureMechanismType.DikesPipingFailureMechanism)
            {
                throw new ArgumentException(@"Incorrect modelType", "entity");
            }

            var failureMechanism = new PipingFailureMechanism
            {
                StorageId = entity.FailureMechanismEntityId
            };

            return failureMechanism;
        }

        public void ConvertModelToEntity(PipingFailureMechanism modelObject, FailureMechanismEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entity.FailureMechanismEntityId = modelObject.StorageId;
            entity.FailureMechanismType = (int) FailureMechanismType.DikesPipingFailureMechanism;
        }
    }
}