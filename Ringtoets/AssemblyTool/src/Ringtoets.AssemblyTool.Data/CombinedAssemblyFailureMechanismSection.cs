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

namespace Riskeer.AssemblyTool.Data
{
    /// <summary>
    /// The failure mechanism section of a combined assembly.
    /// </summary>
    public class CombinedAssemblyFailureMechanismSection
    {
        /// <summary>
        /// Creates a new instance of <see cref="CombinedAssemblyFailureMechanismSection"/>.
        /// </summary>
        /// <param name="sectionStart">The start of the section from the beginning of the reference line in meters.</param>
        /// <param name="sectionEnd">The end of the section from the beginning of the reference line in meters.</param>
        /// <param name="categoryGroup">The category group assembly.</param>
        public CombinedAssemblyFailureMechanismSection(double sectionStart, double sectionEnd, FailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            SectionStart = sectionStart;
            SectionEnd = sectionEnd;
            CategoryGroup = categoryGroup;
        }

        /// <summary>
        /// Gets the start of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public double SectionStart { get; }

        /// <summary>
        /// Gets the end of the section from the beginning of the reference line.
        /// [m]
        /// </summary>
        public double SectionEnd { get; }

        /// <summary>
        /// Gets the category group assembly.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup CategoryGroup { get; }
    }
}