// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="SerializableFailureMechanismSection"/>.
    /// </summary>
    public static class SerializableFailureMechanismSectionCreator
    {
        /// <summary>
        /// Creates a <see cref="SerializableFailureMechanismSection"/> based on
        /// its input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate an id
        /// for the <see cref="SerializableFailureMechanismSection"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/>
        /// this section belongs to.</param>
        /// <param name="section">The <see cref="ExportableFailureMechanismSection"/>
        /// to create a <see cref="SerializableFailureMechanismSection"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableFailureMechanismSection Create(IdentifierGenerator idGenerator,
                                                                 SerializableFailureMechanismSectionCollection serializableCollection,
                                                                 ExportableFailureMechanismSection section)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableCollection == null)
            {
                throw new ArgumentNullException(nameof(serializableCollection));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            return new SerializableFailureMechanismSection(idGenerator.GetNewId(Resources.SerializableFailureMechanismSection_IdPrefix),
                                                           serializableCollection,
                                                           section.StartDistance,
                                                           section.EndDistance,
                                                           section.Geometry,
                                                           SerializableFailureMechanismSectionType.FailureMechanism);
        }

        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanismSection"/> based on
        /// its input parameters.
        /// </summary>
        /// <param name="idGenerator">The id generator to generate an id
        /// for <see cref="SerializableFailureMechanismSection"/>.</param>
        /// <param name="serializableCollection">The <see cref="SerializableFailureMechanismSectionCollection"/>
        /// this section belongs to.</param>
        /// <param name="section">The <see cref="ExportableCombinedFailureMechanismSection"/>
        /// to create a <see cref="SerializableFailureMechanismSection"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableFailureMechanismSection Create(IdentifierGenerator idGenerator,
                                                                 SerializableFailureMechanismSectionCollection serializableCollection,
                                                                 ExportableCombinedFailureMechanismSection section)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableCollection == null)
            {
                throw new ArgumentNullException(nameof(serializableCollection));
            }

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            return new SerializableFailureMechanismSection(idGenerator.GetNewId(Resources.SerializableFailureMechanismSection_IdPrefix),
                                                           serializableCollection,
                                                           section.StartDistance,
                                                           section.EndDistance,
                                                           section.Geometry,
                                                           SerializableFailureMechanismSectionType.Combined,
                                                           SerializableAssemblyMethodCreator.Create(section.AssemblyMethod));
        }
    }
}