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
using Ringtoets.Integration.IO.Assembly;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create <see cref="SerializableAssemblyMethod"/>.
    /// </summary>
    public static class SerializableAssemblyMethodCreator
    {
        /// <summary>
        /// Creates a <see cref="SerializableAssemblyMethod"/> based on
        /// <paramref name="assemblyMethod"/>.
        /// </summary>
        /// <param name="assemblyMethod">The <see cref="ExportableAssemblyMethod"/>
        /// to create a <see cref="SerializableAssemblyMethod"/> for.</param>
        /// <returns>A <see cref="SerializableAssemblyMethod"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyMethod"/>
        /// is a valid value, but unsupported.</exception>
        public static SerializableAssemblyMethod Create(ExportableAssemblyMethod assemblyMethod)
        {
            if (!Enum.IsDefined(typeof(ExportableAssemblyMethod), assemblyMethod))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyMethod),
                                                       (int) assemblyMethod,
                                                       typeof(ExportableAssemblyMethod));
            }

            switch (assemblyMethod)
            {
                case ExportableAssemblyMethod.WBI0E1:
                    return SerializableAssemblyMethod.WBI0E1;
                case ExportableAssemblyMethod.WBI0E3:
                    return SerializableAssemblyMethod.WBI0E3;
                case ExportableAssemblyMethod.WBI0G1:
                    return SerializableAssemblyMethod.WBI0G1;
                case ExportableAssemblyMethod.WBI0G3:
                    return SerializableAssemblyMethod.WBI0G3;
                case ExportableAssemblyMethod.WBI0G4:
                    return SerializableAssemblyMethod.WBI0G4;
                case ExportableAssemblyMethod.WBI0G5:
                    return SerializableAssemblyMethod.WBI0G5;
                case ExportableAssemblyMethod.WBI0G6:
                    return SerializableAssemblyMethod.WBI0G6;
                case ExportableAssemblyMethod.WBI0T1:
                    return SerializableAssemblyMethod.WBI0T1;
                case ExportableAssemblyMethod.WBI0T3:
                    return SerializableAssemblyMethod.WBI0T3;
                case ExportableAssemblyMethod.WBI0T4:
                    return SerializableAssemblyMethod.WBI0T4;
                case ExportableAssemblyMethod.WBI0T5:
                    return SerializableAssemblyMethod.WBI0T5;
                case ExportableAssemblyMethod.WBI0T6:
                    return SerializableAssemblyMethod.WBI0T6;
                case ExportableAssemblyMethod.WBI0T7:
                    return SerializableAssemblyMethod.WBI0T7;
                case ExportableAssemblyMethod.WBI0A1:
                    return SerializableAssemblyMethod.WBI0A1;
                case ExportableAssemblyMethod.WBI1A1:
                    return SerializableAssemblyMethod.WBI1A1;
                case ExportableAssemblyMethod.WBI1B1:
                    return SerializableAssemblyMethod.WBI1B1;
                case ExportableAssemblyMethod.WBI2A1:
                    return SerializableAssemblyMethod.WBI2A1;
                case ExportableAssemblyMethod.WBI2B1:
                    return SerializableAssemblyMethod.WBI2B1;
                case ExportableAssemblyMethod.WBI2C1:
                    return SerializableAssemblyMethod.WBI2C1;
                case ExportableAssemblyMethod.WBI3A1:
                    return SerializableAssemblyMethod.WBI3A1;
                case ExportableAssemblyMethod.WBI3B1:
                    return SerializableAssemblyMethod.WBI3B1;
                case ExportableAssemblyMethod.WBI3C1:
                    return SerializableAssemblyMethod.WBI3C1;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}