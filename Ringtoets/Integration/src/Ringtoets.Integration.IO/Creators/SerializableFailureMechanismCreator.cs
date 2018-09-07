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
using Ringtoets.AssemblyTool.IO.Model;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Helpers;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="SerializableFailureMechanism"/>
    /// </summary>
    public static class SerializableFailureMechanismCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanism"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate an id for the <see cref="SerializableFailureMechanism"/>.</param>
        /// <param name="serializableTotalAssembly">The <see cref="SerializableTotalAssemblyResult"/>
        /// the <see cref="SerializableFailureMechanism"/> belongs to.</param>
        /// <param name="failureMechanism">The <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// without a probability assembly result to create a <see cref="SerializableFailureMechanism"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanism"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableFailureMechanism Create(IdentifierGenerator idGenerator,
                                                          SerializableTotalAssemblyResult serializableTotalAssembly,
                                                          ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> failureMechanism)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableTotalAssembly == null)
            {
                throw new ArgumentNullException(nameof(serializableTotalAssembly));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return new SerializableFailureMechanism(idGenerator.GetNewId(Resources.SerializableFailureMechanismCreator_IdPrefix),
                                                    serializableTotalAssembly,
                                                    SerializableFailureMechanismTypeCreator.Create(failureMechanism.Code),
                                                    SerializableFailureMechanismGroupCreator.Create(failureMechanism.Group),
                                                    SerializableFailureMechanismResultCreator.Create(failureMechanism.FailureMechanismAssembly));
        }

        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanism"/> based on its input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate an id for the <see cref="SerializableFailureMechanism"/>.</param>
        /// <param name="serializableTotalAssembly">The <see cref="SerializableTotalAssemblyResult"/>
        /// the <see cref="SerializableFailureMechanism"/> belongs to.</param>
        /// <param name="failureMechanism">The <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
        /// with a probability assembly result to create a <see cref="SerializableFailureMechanism"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanism"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static SerializableFailureMechanism Create(IdentifierGenerator idGenerator,
                                                          SerializableTotalAssemblyResult serializableTotalAssembly,
                                                          ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> failureMechanism)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (serializableTotalAssembly == null)
            {
                throw new ArgumentNullException(nameof(serializableTotalAssembly));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            return new SerializableFailureMechanism(idGenerator.GetNewId(Resources.SerializableFailureMechanismCreator_IdPrefix),
                                                    serializableTotalAssembly,
                                                    SerializableFailureMechanismTypeCreator.Create(failureMechanism.Code),
                                                    SerializableFailureMechanismGroupCreator.Create(failureMechanism.Group),
                                                    SerializableFailureMechanismResultCreator.Create(failureMechanism.FailureMechanismAssembly));
        }
    }
}