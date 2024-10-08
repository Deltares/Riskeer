﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ExportableFailureMechanismSectionCollection"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionCollectionFactory
    {
        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanismSectionCollection"/> based on a collection of
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created items.</param>
        /// <param name="sections">The collection of <see cref="FailureMechanismSection"/> to create the
        /// <see cref="ExportableFailureMechanismSectionCollection"/> with.</param>
        /// <returns>An <see cref="ExportableFailureMechanismSectionCollection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static ExportableFailureMechanismSectionCollection CreateExportableFailureMechanismSectionCollection(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry, IEnumerable<FailureMechanismSection> sections)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            var exportableSections = new List<ExportableFailureMechanismSection>();
            double startDistance = 0;
            foreach (FailureMechanismSection section in sections)
            {
                ExportableFailureMechanismSection exportableFailureMechanismSection =
                    ExportableFailureMechanismSectionFactory.CreateExportableFailureMechanismSection(idGenerator, registry, section, startDistance);
                exportableSections.Add(exportableFailureMechanismSection);

                startDistance = exportableFailureMechanismSection.EndDistance;
            }

            return new ExportableFailureMechanismSectionCollection(
                idGenerator.GetUniqueId(Resources.ExportableFailureMechanismSectionCollection_IdPrefix),
                exportableSections);
        }
    }
}