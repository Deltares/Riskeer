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
using System.Linq;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Factories;

namespace Ringtoets.Integration.IO.Helpers
{
    public static class ExportableFailureMechanismSectionHelper
    {
        /// <summary>
        /// Creates a lookup between failure mechanism section results and the corresponding
        /// <see cref="ExportableFailureMechanismSection"/>.
        /// </summary>
        /// <typeparam name="TSectionResult">The type of <see cref="FailureMechanismSectionResult"/></typeparam>
        /// <param name="failureMechanismSectionResults">The failure mechanism sections results to create a
        /// <see cref="ExportableFailureMechanismSection"/> for.</param>
        /// <returns>A <see cref="IDictionary{TKey,TValue}"/> between the failure mechanism section results
        /// and <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResults"/> is <c>null</c>.n</exception>
        public static IDictionary<TSectionResult, ExportableFailureMechanismSection> CreateFailureMechanismSectionResultLookup<TSectionResult>(
            IEnumerable<TSectionResult> failureMechanismSectionResults)
            where TSectionResult : FailureMechanismSectionResult
        {
            if (failureMechanismSectionResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResults));
            }

            IEnumerable<FailureMechanismSection> sections = failureMechanismSectionResults.Select(sr => sr.Section);
            IEnumerable<ExportableFailureMechanismSection> exportableFailureMechanismSections =
                ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSections(sections);

            return failureMechanismSectionResults.Zip(exportableFailureMechanismSections, (sectionResult, exportableSection) => new
                                                 {
                                                     key = sectionResult,
                                                     value = exportableSection
                                                 })
                                                 .ToDictionary(pair => pair.key, pair => pair.value);
        }
    }
}