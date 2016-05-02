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
    /// Extension methods for <see cref="FailureMechanismSection"/> related to creating a <see cref="FailureMechanismSectionEntity"/>.
    /// </summary>
    public static class FailureMechanismSectionCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismSectionEntity"/> based on the information of the <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="section">The section to create a database entity for.</param>
        /// <param name="collector">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public static FailureMechanismSectionEntity Create(this FailureMechanismSection section, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            var failureMechanismSectionEntity = new FailureMechanismSectionEntity
            {
                Name = section.Name,
            };

            CreateFailureMechanismSectionPoints(section, failureMechanismSectionEntity);

            collector.Create(failureMechanismSectionEntity, section);

            return failureMechanismSectionEntity;
        }

        private static void CreateFailureMechanismSectionPoints(FailureMechanismSection section, FailureMechanismSectionEntity failureMechanismSectionEntity)
        {
            var i = 0;
            foreach (var point2D in section.Points)
            {
                failureMechanismSectionEntity.FailureMechanismSectionPointEntities.Add(point2D.CreateFailureMechanismSectionPoint(i++));
            }
        }
    }
}