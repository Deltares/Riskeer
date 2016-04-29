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
using Application.Ringtoets.Storage.Read;
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// This partial class describes the read operation for a <see cref="Project"/> based on the
    /// <see cref="ProjectEntity"/>.
    /// </summary>
    public partial class ProjectEntity
    {
        /// <summary>
        /// Reads the <see cref="ProjectEntity"/> and use the information to construct a <see cref="Project"/>.
        /// </summary>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="Project"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public Project Read(ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            var project = new Project
            {
                StorageId = ProjectEntityId,
                Description = Description
            };

            foreach (var assessmentSectionEntity in AssessmentSectionEntities)
            {
                project.Items.Add(assessmentSectionEntity.Read(collector));
            }

            return project;
        } 
    }
}