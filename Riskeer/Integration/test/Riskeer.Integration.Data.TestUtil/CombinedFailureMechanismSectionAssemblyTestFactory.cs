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

using System;
using System.Linq;
using Core.Common.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailurePath;

namespace Riskeer.Integration.Data.TestUtil
{
    /// <summary>
    /// Creates <see cref="CombinedFailureMechanismSectionAssembly"/> for test purposes.
    /// </summary>
    public static class CombinedFailureMechanismSectionAssemblyTestFactory
    {
        /// <summary>
        /// Creates a <see cref="CombinedFailureMechanismSectionAssembly"/> bases on the failure mechanisms
        /// in <paramref name="assessmentSection"/> that are 'in assembly'.
        /// </summary>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to get the failure mechanisms from.</param>
        /// <param name="seed">The seed to create random values.</param>
        /// <returns>The created <see cref="CombinedFailureMechanismSectionAssembly"/>.</returns>
        public static CombinedFailureMechanismSectionAssembly Create(IAssessmentSection assessmentSection, int seed)
        {
            var random = new Random(seed);
            return new CombinedFailureMechanismSectionAssembly(
                new CombinedAssemblyFailureMechanismSection(random.NextDouble(), random.NextDouble(), random.NextEnumValue<FailureMechanismSectionAssemblyGroup>()),
                assessmentSection.GetFailureMechanisms()
                                 .Concat<IFailureMechanism>(assessmentSection.SpecificFailurePaths)
                                 .Where(fm => fm.InAssembly)
                                 .Select(fm => random.NextEnumValue<FailureMechanismSectionAssemblyGroup>())
                                 .ToArray());
        }
    }
}