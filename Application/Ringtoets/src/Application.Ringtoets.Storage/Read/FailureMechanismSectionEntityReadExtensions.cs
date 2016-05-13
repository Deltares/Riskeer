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

using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="FailureMechanismSection"/> based on the
    /// <see cref="FailureMechanismSectionEntity"/>.
    /// </summary>
    internal static class FailureMechanismSectionEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="FailureMechanismSectionEntity"/> and use the information to construct a <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> to create <see cref="FailureMechanismSection"/> for.</param>
        /// <returns>A new <see cref="FailureMechanismSection"/>.</returns>
        internal static FailureMechanismSection Read(this FailureMechanismSectionEntity entity)
        {
            var points = entity.FailureMechanismSectionPointEntities
                               .OrderBy(fmsp => fmsp.Order)
                               .Select(failureMechanismSectionPointEntity => failureMechanismSectionPointEntity.Read())
                               .ToList();

            var mechanismSection = new FailureMechanismSection(entity.Name, points)
            {
                StorageId = entity.FailureMechanismSectionEntityId
            };

            return mechanismSection;
        }
    }
}