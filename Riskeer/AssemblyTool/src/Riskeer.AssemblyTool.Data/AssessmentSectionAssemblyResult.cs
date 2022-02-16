﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    /// Assembly result of an assessment section.
    /// </summary>
    public class AssessmentSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="AssessmentSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="probability">The failure probability of the assessment section.</param>
        /// <param name="assemblyCategoryGroup">The <see cref="AssessmentSectionAssemblyCategoryGroup"/>.</param>
        public AssessmentSectionAssemblyResult(double probability,
                                               AssessmentSectionAssemblyCategoryGroup assemblyCategoryGroup)
        {
            Probability = probability;
            AssemblyCategoryGroup = assemblyCategoryGroup;
        }

        /// <summary>
        /// Gets the probability of the profile.
        /// </summary>
        public double Probability { get; }

        /// <summary>
        /// Gets the <see cref="AssessmentSectionAssemblyCategoryGroup"/>.
        /// </summary>
        public AssessmentSectionAssemblyCategoryGroup AssemblyCategoryGroup { get; }
    }
}