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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ExportableAssemblyMethod"/>.
    /// </summary>
    public static class ExportableAssemblyMethodFactory
    {
        /// <summary>
        /// Creates a <see cref="ExportableAssemblyMethod"/> based on
        /// <paramref name="assemblyMethod"/>.
        /// </summary>
        /// <param name="assemblyMethod">The <see cref="AssemblyMethod"/>
        /// to create a <see cref="ExportableAssemblyMethod"/> for.</param>
        /// <returns>An <see cref="ExportableAssemblyMethod"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyMethod"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyMethod"/>
        /// is a valid value, but unsupported.</exception>
        public static ExportableAssemblyMethod Create(AssemblyMethod assemblyMethod)
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
                case AssemblyMethod.BOI0A2:
                    return ExportableAssemblyMethod.BOI0A2;
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
                case AssemblyMethod.BOI2B1:
                    return ExportableAssemblyMethod.BOI2B1;
                case AssemblyMethod.BOI3A1:
                    return ExportableAssemblyMethod.BOI3A1;
                case AssemblyMethod.BOI3B1:
                    return ExportableAssemblyMethod.BOI3B1;
                case AssemblyMethod.BOI3C1:
                    return ExportableAssemblyMethod.BOI3C1;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}