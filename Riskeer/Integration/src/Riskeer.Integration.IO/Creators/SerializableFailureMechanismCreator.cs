﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Creators
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
        /// <param name="failureMechanism">The <see cref="ExportableFailureMechanism"/>
        /// to create a <see cref="SerializableFailureMechanism"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanism"/></returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyCreatorException">Thrown when <paramref name="failureMechanism"/> is invalid to
        /// create a serializable counterpart for.</exception>
        public static SerializableFailureMechanism Create(IdentifierGenerator idGenerator,
                                                          SerializableTotalAssemblyResult serializableTotalAssembly,
                                                          ExportableFailureMechanism failureMechanism)
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
                                                    SerializableFailureMechanismResultCreator.Create(failureMechanism.FailureMechanismAssembly));
        }
    }
}