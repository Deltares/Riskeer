// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Riskeer.AssemblyTool.Data.Old;

namespace Riskeer.Integration.IO.Assembly
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
        /// <param name="isManual">Indicator whether <paramref name="probability"/> is manually set.</param>
        public ExportableFailureMechanismAssemblyResult(double probability, bool isManual)
        {
            Probability = probability;
            IsManual = isManual;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyGroup">The assembly result.</param>
        public ExportableFailureMechanismAssemblyResult(ExportableAssemblyMethod assemblyMethod,
                                                        FailureMechanismAssemblyCategoryGroup assemblyGroup)
        {
            AssemblyMethod = assemblyMethod;
            AssemblyGroup = assemblyGroup;
        }

        /// <summary>
        /// Gets the probability of the assembly result of this failure mechanism.
        /// </summary>
        public double Probability { get; }
        
        /// <summary>
        /// Gets the indicator whether the <see cref="Probability"/> is manually set.
        /// </summary>
        public bool IsManual { get; }

        /// <summary>
        /// Gets the assembly method that was used to assemble the assembly result.
        /// </summary>
        public ExportableAssemblyMethod AssemblyMethod { get; }

        /// <summary>
        /// Gets the assembly group result.
        /// </summary>
        public FailureMechanismAssemblyCategoryGroup AssemblyGroup { get; }
    }
}