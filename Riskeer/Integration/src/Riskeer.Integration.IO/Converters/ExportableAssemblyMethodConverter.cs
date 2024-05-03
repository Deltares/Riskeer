// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.Integration.IO.Converters
{
    /// <summary>
    /// Converter for converting <see cref="AssemblyMethod"/> into <see cref="ExportableAssemblyMethod"/>.
    /// </summary>
    public static class ExportableAssemblyMethodConverter
    {
        /// <summary>
        /// Converts an <see cref="AssemblyMethod"/> into an <see cref="ExportableAssemblyMethod"/>.
        /// </summary>
        /// <param name="assemblyMethod">The <see cref="AssemblyMethod"/>
        /// to convert into an <see cref="ExportableAssemblyMethod"/>.</param>
        /// <returns>An <see cref="ExportableAssemblyMethod"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyMethod"/>
        /// is a valid value, but unsupported.</exception>
        public static ExportableAssemblyMethod ConvertTo(AssemblyMethod assemblyMethod)
        {
            if (!Enum.IsDefined(typeof(AssemblyMethod), assemblyMethod))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyMethod),
                                                       (int) assemblyMethod,
                                                       typeof(AssemblyMethod));
            }

            switch (assemblyMethod)
            {
                case AssemblyMethod.BOI0A1:
                    return ExportableAssemblyMethod.BOI0A1;
                case AssemblyMethod.BOI0B1:
                    return ExportableAssemblyMethod.BOI0B1;
                case AssemblyMethod.BOI0C1:
                    return ExportableAssemblyMethod.BOI0C1;
                case AssemblyMethod.BOI0C2:
                    return ExportableAssemblyMethod.BOI0C2;
                case AssemblyMethod.BOI1A1:
                    return ExportableAssemblyMethod.BOI1A1;
                case AssemblyMethod.BOI1A2:
                    return ExportableAssemblyMethod.BOI1A2;
                case AssemblyMethod.Manual:
                    return ExportableAssemblyMethod.Manual;
                case AssemblyMethod.BOI2A1:
                    return ExportableAssemblyMethod.BOI2A1;
                case AssemblyMethod.BOI2A2:
                    return ExportableAssemblyMethod.BOI2A2;
                case AssemblyMethod.BOI2B1:
                    return ExportableAssemblyMethod.BOI2B1;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}