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

using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class that holds all the information to export the assembly result
    /// of an assessment section.
    /// </summary>
    public class ExportableAssessmentSectionAssemblyResult
    {
        /// <summary>
        /// Creates an instance of <see cref="ExportableAssessmentSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyGroup">The group of this assembly result.</param>
        /// <param name="probability">The probability of this assembly result.</param>
        /// <param name="assemblyGroupAssemblyMethod">The method that was used to assemble the assembly group of this result.</param>
        /// <param name="probabilityAssemblyMethod">The method that was used to assemble the probability of this result.</param>
        public ExportableAssessmentSectionAssemblyResult(ExportableAssessmentSectionAssemblyGroup assemblyGroup, double probability,
                                                         ExportableAssemblyMethod assemblyGroupAssemblyMethod,
                                                         ExportableAssemblyMethod probabilityAssemblyMethod)
        {
            AssemblyGroup = assemblyGroup;
            Probability = probability;
            AssemblyGroupAssemblyMethod = assemblyGroupAssemblyMethod;
            ProbabilityAssemblyMethod = probabilityAssemblyMethod;
        }

        /// <summary>
        /// Gets the group of this assembly result.
        /// </summary>
        public ExportableAssessmentSectionAssemblyGroup AssemblyGroup { get; }

        /// <summary>
        /// Gets the probability of this assembly result.
        /// </summary>
        public double Probability { get; }

        /// <summary>
        /// Gets the assembly method that was used to assemble the assembly group of this result.
        /// </summary>
        public ExportableAssemblyMethod AssemblyGroupAssemblyMethod { get; }

        /// <summary>
        /// Gets the assembly method that was used to assemble the probability of this result.
        /// </summary>
        public ExportableAssemblyMethod ProbabilityAssemblyMethod { get; }
    }
}