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
using System.ComponentModel;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create <see cref="SerializableFailureMechanismType"/>
    /// </summary>
    public static class SerializableFailureMechanismTypeCreator
    {
        /// <summary>
        /// Creates a <see cref="SerializableFailureMechanismType"/> based on its input.
        /// </summary>
        /// <param name="failureMechanismType">The <see cref="ExportableFailureMechanismType"/> to
        /// create a <see cref="SerializableFailureMechanismType"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="failureMechanismType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="failureMechanismType"/>
        /// is a valid value, but unsupported.</exception>
        public static SerializableFailureMechanismType Create(ExportableFailureMechanismType failureMechanismType)
        {
            if (!Enum.IsDefined(typeof(ExportableFailureMechanismType), failureMechanismType))
            {
                throw new InvalidEnumArgumentException(nameof(failureMechanismType),
                                                       (int) failureMechanismType,
                                                       typeof(ExportableFailureMechanismType));
            }

            switch (failureMechanismType)
            {
                case ExportableFailureMechanismType.STBI:
                    return SerializableFailureMechanismType.STBI;
                case ExportableFailureMechanismType.STBU:
                    return SerializableFailureMechanismType.STBU;
                case ExportableFailureMechanismType.STPH:
                    return SerializableFailureMechanismType.STPH;
                case ExportableFailureMechanismType.STMI:
                    return SerializableFailureMechanismType.STMI;
                case ExportableFailureMechanismType.AGK:
                    return SerializableFailureMechanismType.AGK;
                case ExportableFailureMechanismType.AWO:
                    return SerializableFailureMechanismType.AWO;
                case ExportableFailureMechanismType.GEBU:
                    return SerializableFailureMechanismType.GEBU;
                case ExportableFailureMechanismType.GABU:
                    return SerializableFailureMechanismType.GABU;
                case ExportableFailureMechanismType.GEKB:
                    return SerializableFailureMechanismType.GEKB;
                case ExportableFailureMechanismType.GABI:
                    return SerializableFailureMechanismType.GABI;
                case ExportableFailureMechanismType.ZST:
                    return SerializableFailureMechanismType.ZST;
                case ExportableFailureMechanismType.DA:
                    return SerializableFailureMechanismType.DA;
                case ExportableFailureMechanismType.HTKW:
                    return SerializableFailureMechanismType.HTKW;
                case ExportableFailureMechanismType.BSKW:
                    return SerializableFailureMechanismType.BSKW;
                case ExportableFailureMechanismType.PKW:
                    return SerializableFailureMechanismType.PKW;
                case ExportableFailureMechanismType.STKWp:
                    return SerializableFailureMechanismType.STKWp;
                case ExportableFailureMechanismType.STKWl:
                    return SerializableFailureMechanismType.STKWl;
                case ExportableFailureMechanismType.INN:
                    return SerializableFailureMechanismType.INN;
                default:
                    throw new NotSupportedException();
            }

        }
    }
}