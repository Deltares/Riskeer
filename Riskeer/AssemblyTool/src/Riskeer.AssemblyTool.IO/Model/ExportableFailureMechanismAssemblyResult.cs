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

using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class that holds the information to export the assembly result of a failure mechanism.
    /// </summary>
    public class ExportableFailureMechanismAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismAssemblyResult"/>.
        /// </summary>
        /// <param name="probability">The probability of the assembly result.</param>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        public ExportableFailureMechanismAssemblyResult(double probability, ExportableAssemblyMethod assemblyMethod)
        {
            Probability = probability;
            AssemblyMethod = assemblyMethod;
        }

        /// <summary>
        /// Gets the probability of the assembly result of this failure mechanism.
        /// </summary>
        public double Probability { get; }

        /// <summary>
        /// Gets the method that was used to assemble this result.
        /// </summary>
        public ExportableAssemblyMethod AssemblyMethod { get; }
    }
}