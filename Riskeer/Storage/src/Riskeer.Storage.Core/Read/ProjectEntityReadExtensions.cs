// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="RingtoetsProject"/> based on the
    /// <see cref="ProjectEntity"/>.
    /// </summary>
    internal static class ProjectEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="ProjectEntity"/> and use the information to construct a <see cref="RingtoetsProject"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> to create <see cref="RingtoetsProject"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="RingtoetsProject"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static RingtoetsProject Read(this ProjectEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            var project = new RingtoetsProject
            {
                Description = entity.Description
            };

            foreach (AssessmentSectionEntity assessmentSectionEntity in entity.AssessmentSectionEntities.OrderBy(ase => ase.Order))
            {
                project.AssessmentSections.Add(assessmentSectionEntity.Read(collector));
            }

            return project;
        }
    }
}