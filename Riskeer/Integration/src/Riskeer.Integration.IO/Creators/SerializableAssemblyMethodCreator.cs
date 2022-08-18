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
using System.ComponentModel;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.ModelOld.Enums;

namespace Riskeer.Integration.IO.Creators
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
                case ExportableAssemblyMethod.BOI0A1:
                    return SerializableAssemblyMethod.BOI0A1;
                case ExportableAssemblyMethod.BOI0A2:
                    return SerializableAssemblyMethod.BOI0A2;
                case ExportableAssemblyMethod.BOI0B1:
                    return SerializableAssemblyMethod.BOI0B1;
                case ExportableAssemblyMethod.BOI0C1:
                    return SerializableAssemblyMethod.BOI0C1;
                case ExportableAssemblyMethod.BOI0C2:
                    return SerializableAssemblyMethod.BOI0C2;
                case ExportableAssemblyMethod.BOI1A1:
                    return SerializableAssemblyMethod.BOI1A1;
                case ExportableAssemblyMethod.BOI1A2:
                    return SerializableAssemblyMethod.BOI1A2;
                case ExportableAssemblyMethod.Manual:
                    return SerializableAssemblyMethod.Manual;
                case ExportableAssemblyMethod.BOI2A1:
                    return SerializableAssemblyMethod.BOI2A1;
                case ExportableAssemblyMethod.BOI2B1:
                    return SerializableAssemblyMethod.BOI2B1;
                case ExportableAssemblyMethod.BOI3A1:
                    return SerializableAssemblyMethod.BOI3A1;
                case ExportableAssemblyMethod.BOI3B1:
                    return SerializableAssemblyMethod.BOI3B1;
                case ExportableAssemblyMethod.BOI3C1:
                    return SerializableAssemblyMethod.BOI3C1;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}