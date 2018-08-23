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

using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.Integration.IO.Assembly
{
    /// <summary>
    /// Class that holds all the information to export an assembly result with a probability for a failure mechanism section.
    /// </summary>
    public class ExportableSectionAssemblyResultWithProbability : ExportableSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyCategory">The assembly result of this section.</param>
        /// <param name="probability">The probability of the assembly result of this section.</param>
        public ExportableSectionAssemblyResultWithProbability(ExportableAssemblyMethod assemblyMethod,
                                                              FailureMechanismSectionAssemblyCategoryGroup assemblyCategory,
                                                              double probability)
            : base(assemblyMethod, assemblyCategory)
        {
            Probability = probability;
        }

        /// <summary>
        /// Gets the probability of the assembly result of this section.
        /// </summary>
        public double Probability { get; }
    }
}