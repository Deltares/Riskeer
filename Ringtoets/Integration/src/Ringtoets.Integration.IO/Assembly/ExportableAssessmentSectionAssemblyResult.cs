﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using Riskeer.AssemblyTool.Data;

namespace Riskeer.Integration.IO.Assembly
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
        /// <param name="assemblyMethod">The method that was used to assemble this result.</param>
        /// <param name="assemblyCategory">The assembly result.</param>
        public ExportableAssessmentSectionAssemblyResult(ExportableAssemblyMethod assemblyMethod,
                                                         AssessmentSectionAssemblyCategoryGroup assemblyCategory)
        {
            AssemblyMethod = assemblyMethod;
            AssemblyCategory = assemblyCategory;
        }

        /// <summary>
        /// Gets the assembly method that was used to assemble the assembly result.
        /// </summary>
        public ExportableAssemblyMethod AssemblyMethod { get; }

        /// <summary>
        /// Gets the assembly category.
        /// </summary>
        public AssessmentSectionAssemblyCategoryGroup AssemblyCategory { get; }
    }
}