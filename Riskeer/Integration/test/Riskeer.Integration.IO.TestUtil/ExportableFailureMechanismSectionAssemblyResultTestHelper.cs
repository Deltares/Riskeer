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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionAssemblyResultTestHelper
    {
        /// <summary>
        /// Asserts a collection of <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/>
        /// against the assembly result.
        /// </summary>
        /// <param name="expectedAssemblyResult">The expected <see cref="FailureMechanismSectionAssemblyResult"/>.</param>
        /// <param name="sections">The actual exportable sections.</param>
        /// <param name="results">The actual exportable assembly results.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The number of <paramref name="sections"/> and number of <paramref name="results"/> do not match.</item>
        /// <item>The values between <paramref name="expectedAssemblyResult"/> and <paramref name="results"/>
        /// do not match.</item>
        /// </list></exception>
        public static void AssertExportableFailureMechanismSectionResults(FailureMechanismSectionAssemblyResult expectedAssemblyResult,
                                                                          IEnumerable<ExportableFailureMechanismSection> sections,
                                                                          IEnumerable<ExportableFailureMechanismSectionAssemblyWithProbabilityResult> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableFailureMechanismSectionAssemblyWithProbabilityResult actualExportableAssemblyResult = results.ElementAt(i);

                Assert.AreSame(section, actualExportableAssemblyResult.FailureMechanismSection);
                Assert.AreEqual(expectedAssemblyResult.FailureMechanismSectionAssemblyGroup, actualExportableAssemblyResult.AssemblyGroup);
                Assert.AreEqual(expectedAssemblyResult.SectionProbability, actualExportableAssemblyResult.Probability);
            }
        }
    }
}