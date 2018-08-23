// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export a collection
    /// of <see cref="ExportableCombinedSectionAssembly"/>.
    /// </summary>
    public class ExportableCombinedSectionAssemblyCollection
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        /// <param name="sections">The sections belonging to this collection of <see cref="ExportableCombinedSectionAssembly"/>.</param>
        /// <param name="combinedSectionAssemblyResults">The collection of <see cref="ExportableCombinedSectionAssembly"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public ExportableCombinedSectionAssemblyCollection(IEnumerable<ExportableCombinedFailureMechanismSection> sections,
                                                           IEnumerable<ExportableCombinedSectionAssembly> combinedSectionAssemblyResults)
        {
            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            if (combinedSectionAssemblyResults == null)
            {
                throw new ArgumentNullException(nameof(combinedSectionAssemblyResults));
            }

            Sections = sections;
            CombinedSectionAssemblyResults = combinedSectionAssemblyResults;
        }

        /// <summary>
        /// Gets the sections belonging to this collection of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        public IEnumerable<ExportableCombinedFailureMechanismSection> Sections { get; }

        /// <summary>
        /// Gets the collection of <see cref="ExportableCombinedSectionAssembly"/>.
        /// </summary>
        public IEnumerable<ExportableCombinedSectionAssembly> CombinedSectionAssemblyResults { get; }
    }
}