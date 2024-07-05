// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ExportableFailureMechanismSection"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionFactory
    {
        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanismSection"/> based on its input arguments.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created
        /// <see cref="ExportableFailureMechanismSection"/>.</param>
        /// <param name="section">The <see cref="FailureMechanismSection"/> to create the
        /// <see cref="ExportableFailureMechanismSection"/> with.</param>
        /// <param name="startDistance">The start distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <returns>An <see cref="ExportableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="idGenerator"/>, <paramref name="registry"/>
        /// or <paramref name="section"/> is <c>null</c>.</exception>
        public static ExportableFailureMechanismSection CreateExportableFailureMechanismSection(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry, FailureMechanismSection section,
            double startDistance)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (registry.Contains(section))
            {
                return registry.Get(section);
            }

            double endDistance = startDistance + section.Length;
            var exportableSection = new ExportableFailureMechanismSection(
                idGenerator.GetUniqueId(Resources.ExportableFailureMechanismSection_IdPrefix),
                section.Points, startDistance, endDistance);

            registry.Register(section, exportableSection);
            return exportableSection;
        }
    }
}