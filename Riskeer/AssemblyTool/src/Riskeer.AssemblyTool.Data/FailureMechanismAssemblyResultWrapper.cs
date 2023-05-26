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

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// Wrapper class to link the used <see cref="Data.AssemblyMethod"/> to the <see cref="AssemblyResult"/>.
    /// </summary>
    public class FailureMechanismAssemblyResultWrapper
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismAssemblyResultWrapper"/>.
        /// </summary>
        /// <param name="assemblyResult">The assembly result.</param>
        /// <param name="assemblyMethod">The <see cref="AssemblyMethod"/> that is used to assemble the probability.</param>
        public FailureMechanismAssemblyResultWrapper(double assemblyResult, AssemblyMethod assemblyMethod)
        {
            AssemblyResult = assemblyResult;
            AssemblyMethod = assemblyMethod;
        }

        /// <summary>
        /// Gets the wrapped assembly result.
        /// </summary>
        public double AssemblyResult { get; }

        /// <summary>
        /// Gets the <see cref="AssemblyMethod"/> that is used to assemble the probability.
        /// </summary>
        public AssemblyMethod AssemblyMethod { get; }
    }
}