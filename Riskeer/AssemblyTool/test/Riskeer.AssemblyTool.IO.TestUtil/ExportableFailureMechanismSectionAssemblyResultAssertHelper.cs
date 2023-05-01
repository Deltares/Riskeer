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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Integration.IO.Converters;

namespace Riskeer.AssemblyTool.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionAssemblyResultAssertHelper
    {
        /// <summary>
        /// Asserts a collection of <see cref="ExportableFailureMechanismSectionAssemblyResult"/>
        /// against the assembly result.
        /// </summary>
        /// <param name="expectedAssemblyResultWrapper">The expected <see cref="FailureMechanismSectionAssemblyResultWrapper"/>.</param>
        /// <param name="sections">The actual exportable sections.</param>
        /// <param name="results">The actual exportable assembly results.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The number of <paramref name="sections"/> and number of <paramref name="results"/> do not match.</item>
        /// <item>The id does not contain the expected value.</item>
        /// <item>The values between <paramref name="expectedAssemblyResultWrapper"/> and <paramref name="results"/>
        /// do not match.</item>
        /// </list>
        /// </exception>
        public static void AssertExportableFailureMechanismSectionResults(FailureMechanismSectionAssemblyResultWrapper expectedAssemblyResultWrapper,
                                                                          IEnumerable<ExportableFailureMechanismSection> sections,
                                                                          IEnumerable<ExportableFailureMechanismSectionAssemblyResult> results)
        {
            int expectedNrOfResults = sections.Count();
            Assert.AreEqual(expectedNrOfResults, results.Count());

            for (var i = 0; i < expectedNrOfResults; i++)
            {
                ExportableFailureMechanismSection section = sections.ElementAt(i);
                ExportableFailureMechanismSectionAssemblyResult actualExportableAssemblyResult = results.ElementAt(i);

                AssertExportableFailureMechanismSectionResult(expectedAssemblyResultWrapper, actualExportableAssemblyResult, section, i);
            }
        }

        /// <summary>
        /// Asserts the <see cref="ExportableFailureMechanismSectionAssemblyResult"/> against the assembly result.
        /// </summary>
        /// <param name="expectedAssemblyResultWrapper">The expected <see cref="FailureMechanismSectionAssemblyResultWrapper"/>.</param>
        /// <param name="actualExportableAssemblyResult">The actual <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.</param>
        /// <param name="section">The used <see cref="ExportableFailureMechanismSection"/>.</param>
        /// <param name="i">The number of the section result.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The id does not contain the expected value.</item>
        /// <item>The values between <paramref name="expectedAssemblyResultWrapper"/> and <paramref name="actualExportableAssemblyResult"/>
        /// do not match.</item>
        /// </list>
        /// </exception>
        public static void AssertExportableFailureMechanismSectionResult(FailureMechanismSectionAssemblyResultWrapper expectedAssemblyResultWrapper,
                                                                         ExportableFailureMechanismSectionAssemblyResult actualExportableAssemblyResult,
                                                                         ExportableFailureMechanismSection section, int i = 0)
        {
            Assert.AreEqual($"Fa.{i}", actualExportableAssemblyResult.Id);
            Assert.AreSame(section, actualExportableAssemblyResult.FailureMechanismSection);
            FailureMechanismSectionAssemblyResult expectedAssemblyResult = expectedAssemblyResultWrapper.AssemblyResult;
            Assert.AreEqual(ExportableFailureMechanismSectionAssemblyGroupConverter.ConvertTo(expectedAssemblyResult.FailureMechanismSectionAssemblyGroup),
                            actualExportableAssemblyResult.AssemblyGroup);
            Assert.AreEqual(expectedAssemblyResult.SectionProbability, actualExportableAssemblyResult.Probability);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(expectedAssemblyResultWrapper.AssemblyGroupMethod),
                            actualExportableAssemblyResult.AssemblyGroupAssemblyMethod);
            Assert.AreEqual(ExportableAssemblyMethodConverter.ConvertTo(expectedAssemblyResultWrapper.ProbabilityMethod),
                            actualExportableAssemblyResult.ProbabilityAssemblyMethod);
        }
    }
}