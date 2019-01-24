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

using NUnit.Framework;
using Ringtoets.Integration.IO.Assembly;
using Riskeer.AssemblyTool.Data;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert <see cref="ExportableSectionAssemblyResult"/>.
    /// </summary>
    public static class ExportableSectionAssemblyResultTestHelper
    {
        /// <summary>
        /// Asserts an <see cref="ExportableSectionAssemblyResultWithProbability"/>
        /// against the assembly result and the method which was used to generate the result.
        /// </summary>
        /// <param name="assemblyResult">The expected <see cref="FailureMechanismSectionAssembly"/>.</param>
        /// <param name="assemblyMethod">The <see cref="ExportableAssemblyMethod"/> which was
        /// used to generate the result.</param>
        /// <param name="exportableSectionAssemblyResult">The <see cref="ExportableSectionAssemblyResultWithProbability"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The values between <paramref name="assemblyResult"/> and <paramref name="exportableSectionAssemblyResult"/>
        /// do not match.</item>
        /// <item>The <paramref name="assemblyMethod"/> does not match.</item>
        /// </list></exception>
        public static void AssertExportableSectionAssemblyResult(FailureMechanismSectionAssembly assemblyResult,
                                                                 ExportableAssemblyMethod assemblyMethod,
                                                                 ExportableSectionAssemblyResultWithProbability exportableSectionAssemblyResult)
        {
            AssertExportableSectionAssemblyResult(assemblyResult.Group, assemblyMethod, exportableSectionAssemblyResult);
            Assert.AreEqual(assemblyResult.Probability, exportableSectionAssemblyResult.Probability);
        }

        /// <summary>
        /// Asserts an <see cref="ExportableSectionAssemblyResult"/>
        /// against the assembly result and the method which was used to generate the result.
        /// </summary>
        /// <param name="assemblyResult">The expected <see cref="FailureMechanismSectionAssembly"/>.</param>
        /// <param name="assemblyMethod">The <see cref="ExportableAssemblyMethod"/> which was
        /// used to generate the result.</param>
        /// <param name="exportableSectionAssemblyResult">The <see cref="ExportableSectionAssemblyResultWithProbability"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The values between <paramref name="assemblyResult"/> and <paramref name="exportableSectionAssemblyResult"/>
        /// do not match.</item>
        /// <item>The <paramref name="assemblyMethod"/> does not match.</item>
        /// </list></exception>
        public static void AssertExportableSectionAssemblyResult(FailureMechanismSectionAssemblyCategoryGroup assemblyResult,
                                                                 ExportableAssemblyMethod assemblyMethod,
                                                                 ExportableSectionAssemblyResult exportableSectionAssemblyResult)
        {
            Assert.AreEqual(assemblyMethod, exportableSectionAssemblyResult.AssemblyMethod);
            Assert.AreEqual(assemblyResult, exportableSectionAssemblyResult.AssemblyCategory);
        }
    }
}