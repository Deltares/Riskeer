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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instance of <see cref="ExportableAssessmentSection"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionFactory
    {
        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanismSection"/> based on <paramref name="failureMechanismSections"/>.
        /// </summary>
        /// <param name="failureMechanismSections">The collection of <see cref="FailureMechanismSection"/>
        /// to create a collection of <see cref="ExportableFailureMechanismSection"/> for.</param>
        /// <returns>A collection of <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSections"/> is <c>null</c>.</exception>
        public static IEnumerable<ExportableFailureMechanismSection> CreateExportableFailureMechanismSections(IEnumerable<FailureMechanismSection> failureMechanismSections)
        {
            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            var exportableFailureMechanismSections = new List<ExportableFailureMechanismSection>();

            double startDistance = 0;
            foreach (FailureMechanismSection section in failureMechanismSections)
            {
                double endDistance = startDistance + Math2D.Length(section.Points);

                exportableFailureMechanismSections.Add(new ExportableFailureMechanismSection(section.Points,
                                                                                             startDistance,
                                                                                             endDistance));
                startDistance = endDistance;
            }

            return exportableFailureMechanismSections;
        }
    }
}