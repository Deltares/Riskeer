// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Collections.Generic;
using Riskeer.AssemblyTool.IO.Helpers;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class that holds all the information to export a collection of failure mechanism sections.
    /// </summary>
    public class ExportableFailureMechanismSectionCollection
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismSectionCollection"/>.
        /// </summary>
        /// <param name="id">The id of the failure mechanism section collection.</param>
        /// <param name="sections">The collection of <see cref="ExportableFailureMechanismSection"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sections"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public ExportableFailureMechanismSectionCollection(string id, IEnumerable<ExportableFailureMechanismSection> sections)
        {
            IdValidationHelper.ThrowIfInvalid(id);

            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            Id = id;
            Sections = sections;
        }

        /// <summary>
        /// Gets the id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the collection of exportable failure mechanism sections.
        /// </summary>
        public IEnumerable<ExportableFailureMechanismSection> Sections { get; }
    }
}