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
    /// Assembly result of a failure mechanism section.
    /// </summary>
    public class FailureMechanismSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="profileProbability">The probability of the failure mechanism section, expressed for profile.</param>
        /// <param name="sectionProbability">The probability of the failure mechanism section, expressed for the section.</param>
        /// <param name="n">The length effect of the failure mechanism section.</param>
        /// <param name="failureMechanismSectionAssemblyGroup">The <see cref="Data.FailureMechanismSectionAssemblyGroup"/>.</param>
        public FailureMechanismSectionAssemblyResult(double profileProbability, double sectionProbability, double n,
                                                     FailureMechanismSectionAssemblyGroup failureMechanismSectionAssemblyGroup)
        {
            ProfileProbability = profileProbability;
            SectionProbability = sectionProbability;
            N = n;
            FailureMechanismSectionAssemblyGroup = failureMechanismSectionAssemblyGroup;
        }

        /// <summary>
        /// Gets the probability of the profile.
        /// </summary>
        public double ProfileProbability { get; }

        /// <summary>
        /// Gets the probability of the section.
        /// </summary>
        public double SectionProbability { get; }

        /// <summary>
        /// Gets the length effect 'N'.
        /// </summary>
        public double N { get; }

        /// <summary>
        /// Gets the <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        public FailureMechanismSectionAssemblyGroup FailureMechanismSectionAssemblyGroup { get; }
    }
}