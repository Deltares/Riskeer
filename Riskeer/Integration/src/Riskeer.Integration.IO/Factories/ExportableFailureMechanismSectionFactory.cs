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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.Assembly;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;
using Riskeer.Integration.Util;

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
            var exportableSection = new ExportableFailureMechanismSection(idGenerator.GetUniqueId(Resources.ExportableFailureMechanismSection_IdPrefix),
                                                                          section.Points, startDistance, endDistance);
            registry.Register(section, exportableSection);
            return exportableSection;
        }

        /// <summary>
        /// Creates a <see cref="ExportableCombinedFailureMechanismSection"/> based on its input arguments.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created
        /// <see cref="ExportableCombinedFailureMechanismSection"/>.</param>
        /// <param name="referenceLine">The <see cref="ReferenceLine"/> the section results belong to.</param>
        /// <param name="assemblyResult">The <see cref="CombinedFailureMechanismSectionAssemblyResult"/> to create the section with.</param>
        /// <returns>An <see cref="ExportableCombinedFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static ExportableCombinedFailureMechanismSection CreateExportableCombinedFailureMechanismSection(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry,
            ReferenceLine referenceLine, CombinedFailureMechanismSectionAssemblyResult assemblyResult)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            if (assemblyResult == null)
            {
                throw new ArgumentNullException(nameof(assemblyResult));
            }

            if (registry.Contains(assemblyResult))
            {
                return registry.Get(assemblyResult);
            }

            var exportableSection = new ExportableCombinedFailureMechanismSection(
                idGenerator.GetUniqueId(Resources.ExportableFailureMechanismSection_IdPrefix),
                FailureMechanismSectionHelper.GetFailureMechanismSectionGeometry(referenceLine, assemblyResult.SectionStart, assemblyResult.SectionEnd),
                assemblyResult.SectionStart, assemblyResult.SectionEnd,
                ExportableAssemblyMethodConverter.ConvertTo(assemblyResult.CommonSectionAssemblyMethod));

            registry.Register(assemblyResult, exportableSection);
            return exportableSection;
        }
    }
}