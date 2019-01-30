﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.Helpers
{
    /// <summary>
    /// Class containing helper methods for <see cref="ExportableFailureMechanismSection"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionHelper
    {
        /// <summary>
        /// Creates a lookup between failure mechanism section results and the corresponding
        /// <see cref="ExportableFailureMechanismSection"/>.
        /// </summary>
        /// <typeparam name="TSectionResult">The type of <see cref="FailureMechanismSectionResult"/>.</typeparam>
        /// <param name="failureMechanismSectionResults">The failure mechanism sections results to create a
        /// <see cref="ExportableFailureMechanismSection"/> for.</param>
        /// <returns>A <see cref="IDictionary{TKey,TValue}"/> between the failure mechanism section results
        /// and <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionResults"/> is <c>null</c>.</exception>
        public static IDictionary<TSectionResult, ExportableFailureMechanismSection> CreateFailureMechanismSectionResultLookup<TSectionResult>(
            IEnumerable<TSectionResult> failureMechanismSectionResults)
            where TSectionResult : FailureMechanismSectionResult
        {
            if (failureMechanismSectionResults == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionResults));
            }

            var failureMechanismSectionsLookup = new Dictionary<TSectionResult, ExportableFailureMechanismSection>();

            double startDistance = 0;
            foreach (TSectionResult sectionResult in failureMechanismSectionResults)
            {
                FailureMechanismSection failureMechanismSection = sectionResult.Section;
                double endDistance = startDistance + failureMechanismSection.Length;

                failureMechanismSectionsLookup[sectionResult] = new ExportableFailureMechanismSection(failureMechanismSection.Points,
                                                                                                      startDistance,
                                                                                                      endDistance);

                startDistance = endDistance;
            }

            return failureMechanismSectionsLookup;
        }
    }
}