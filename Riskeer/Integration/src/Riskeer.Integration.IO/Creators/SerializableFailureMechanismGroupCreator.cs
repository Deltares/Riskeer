// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create <see cref="SerializableFailureMechanismGroup"/>
    /// </summary>
    public static class SerializableFailureMechanismGroupCreator
    {
        /// <summary>
        /// Creates a <see cref="SerializableFailureMechanismGroup"/> based on <paramref name="failureMechanismGroup"/>.
        /// </summary>
        /// <param name="failureMechanismGroup">The <see cref="ExportableFailureMechanismGroup"/> to
        /// create a <see cref="SerializableFailureMechanismGroup"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="failureMechanismGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="failureMechanismGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static SerializableFailureMechanismGroup Create(ExportableFailureMechanismGroup failureMechanismGroup)
        {
            if (!Enum.IsDefined(typeof(ExportableFailureMechanismGroup), failureMechanismGroup))
            {
                throw new InvalidEnumArgumentException(nameof(failureMechanismGroup),
                                                       (int) failureMechanismGroup,
                                                       typeof(ExportableFailureMechanismGroup));
            }

            switch (failureMechanismGroup)
            {
                case ExportableFailureMechanismGroup.Group1:
                    return SerializableFailureMechanismGroup.Group1;
                case ExportableFailureMechanismGroup.Group2:
                    return SerializableFailureMechanismGroup.Group2;
                case ExportableFailureMechanismGroup.Group3:
                    return SerializableFailureMechanismGroup.Group3;
                case ExportableFailureMechanismGroup.Group4:
                    return SerializableFailureMechanismGroup.Group4;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}