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

using System;
using Core.Common.TestUtil;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.Data.Assembly;

namespace Riskeer.Integration.Data.TestUtil
{
    /// <summary>
    /// Creates <see cref="CombinedFailureMechanismSectionAssemblyResult"/> for test purposes.
    /// </summary>
    public static class CombinedFailureMechanismSectionAssemblyResultTestFactory
    {
        /// <summary>
        /// Creates a default instance of <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <returns>A <see cref="CombinedFailureMechanismSectionAssemblyResult"/>.</returns>
        public static CombinedFailureMechanismSectionAssemblyResult Create()
        {
            var random = new Random(21);
            double sectionStart = random.NextDouble();
            double sectionEnd = random.NextDouble();
            var totalResult = random.NextEnumValue<FailureMechanismSectionAssemblyGroup>();
            var commonSectionAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var failureMechanismResultsAssemblyMethod = random.NextEnumValue<AssemblyMethod>();
            var combinedSectionResultAssemblyMethod = random.NextEnumValue<AssemblyMethod>();

            return new CombinedFailureMechanismSectionAssemblyResult(sectionStart, sectionEnd, totalResult, commonSectionAssemblyMethod,
                                                                     failureMechanismResultsAssemblyMethod, combinedSectionResultAssemblyMethod,
                                                                     new CombinedFailureMechanismSectionAssemblyResult.ConstructionProperties());
        }
    }
}