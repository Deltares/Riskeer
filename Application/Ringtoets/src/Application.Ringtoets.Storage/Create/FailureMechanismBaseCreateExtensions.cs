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
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="FailureMechanismBase{T}"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class FailureMechanismBaseCreateExtensions
    {
        /// <summary>
        /// Creates <see cref="FailureMechanismSectionEntity"/> instances based on the information of the <see cref="FailureMechanismBase{T}"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database failure mechanism section entities for.</param>
        /// <param name="collector">The object keeping track of create operations.</param>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to which to add the created entities.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="collector"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// </list>
        /// </exception>
        internal static void AddEntitiesForFailureMechanismSections(this FailureMechanismBase<FailureMechanismSectionResult> mechanism, CreateConversionCollector collector, FailureMechanismEntity entity)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            foreach (var failureMechanismSection in mechanism.Sections)
            {
                entity.FailureMechanismSectionEntities.Add(failureMechanismSection.Create(collector));
            }
        }
    }
}