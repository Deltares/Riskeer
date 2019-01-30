// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.Integration.IO.Assembly;

namespace Riskeer.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
    /// </summary>
    public static class ExportableFailureMechanismTestHelper
    {
        /// <summary>
        /// Asserts a default failure mechanism that contains assembly results with a probability.
        /// </summary>
        /// <param name="expectedGeometry">The expected geometry of the section it contains.</param>
        /// <param name="expectedFailureMechanismCode">The expected <see cref="ExportableFailureMechanismType"/>.</param>
        /// <param name="expectedGroup">The expected <see cref="ExportableFailureMechanismGroup"/>.</param>
        /// <param name="expectedFailureMechanismAssemblyMethod">The expected <see cref="ExportableAssemblyMethod"/> which is used
        /// to generate a failure mechanism assembly result.</param>
        /// <param name="exportableFailureMechanism">The <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The geometry defined by <paramref name="expectedGeometry"/> does not match with the section contained in
        /// <paramref name="exportableFailureMechanism"/>.</item>
        /// <item>The values in <paramref name="exportableFailureMechanism"/> do not match with
        /// <paramref name="expectedFailureMechanismCode"/>, <paramref name="expectedGroup"/>
        /// or <see cref="expectedFailureMechanismAssemblyMethod"/>.</item>
        /// </list></exception>
        public static void AssertDefaultFailureMechanismWithProbability(IEnumerable<Point2D> expectedGeometry,
                                                                        ExportableFailureMechanismType expectedFailureMechanismCode,
                                                                        ExportableFailureMechanismGroup expectedGroup,
                                                                        ExportableAssemblyMethod expectedFailureMechanismAssemblyMethod,
                                                                        ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism)
        {
            Assert.AreEqual(expectedGroup, exportableFailureMechanism.Group);
            Assert.AreEqual(expectedFailureMechanismCode, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(expectedFailureMechanismAssemblyMethod, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(0, failureMechanismAssemblyResult.Probability);

            var exportableFailureMechanismSectionAssembly =
                (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult) exportableFailureMechanism.SectionAssemblyResults.Single();
            ExportableSectionAssemblyResultWithProbability combinedAssembly = exportableFailureMechanismSectionAssembly.CombinedAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI0A1, combinedAssembly.AssemblyMethod);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, combinedAssembly.AssemblyCategory);
            Assert.AreEqual(0, combinedAssembly.Probability);

            ExportableFailureMechanismSection failureMechanismSection = exportableFailureMechanismSectionAssembly.FailureMechanismSection;
            Assert.AreSame(expectedGeometry, failureMechanismSection.Geometry);
            Assert.AreEqual(0, failureMechanismSection.StartDistance);
            Assert.AreEqual(Math2D.Length(expectedGeometry), failureMechanismSection.EndDistance);
        }

        /// <summary>
        /// Asserts a default failure mechanism that contains assembly results without a probability.
        /// </summary>
        /// <param name="expectedGeometry">The expected geometry of the section it contains.</param>
        /// <param name="expectedFailureMechanismCode">The expected <see cref="ExportableFailureMechanismType"/>.</param>
        /// <param name="expectedGroup">The expected <see cref="ExportableFailureMechanismGroup"/>.</param>
        /// <param name="expectedFailureMechanismAssemblyMethod">The expected <see cref="ExportableAssemblyMethod"/> which is used
        /// to generate a failure mechanism assembly result.</param>
        /// <param name="exportableFailureMechanism">The <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The geometry defined by <paramref name="expectedGeometry"/> does not match with the section contained in
        /// <paramref name="exportableFailureMechanism"/>.</item>
        /// <item>The values in <paramref name="exportableFailureMechanism"/> do not match with
        /// <paramref name="expectedFailureMechanismCode"/>, <paramref name="expectedGroup"/>
        /// or <see cref="expectedFailureMechanismAssemblyMethod"/>.</item>
        /// </list></exception>
        public static void AssertDefaultFailureMechanismWithoutProbability(IEnumerable<Point2D> expectedGeometry,
                                                                           ExportableFailureMechanismType expectedFailureMechanismCode,
                                                                           ExportableFailureMechanismGroup expectedGroup,
                                                                           ExportableAssemblyMethod expectedFailureMechanismAssemblyMethod,
                                                                           ExportableFailureMechanism<ExportableFailureMechanismAssemblyResult> exportableFailureMechanism)
        {
            Assert.AreEqual(expectedGroup, exportableFailureMechanism.Group);
            Assert.AreEqual(expectedFailureMechanismCode, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResult failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(expectedFailureMechanismAssemblyMethod, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);

            var exportableFailureMechanismSectionAssembly =
                (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedResult) exportableFailureMechanism.SectionAssemblyResults.Single();
            ExportableSectionAssemblyResult combinedAssembly = exportableFailureMechanismSectionAssembly.CombinedAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI0A1, combinedAssembly.AssemblyMethod);
            Assert.AreEqual(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, combinedAssembly.AssemblyCategory);

            ExportableFailureMechanismSection failureMechanismSection = exportableFailureMechanismSectionAssembly.FailureMechanismSection;
            Assert.AreSame(expectedGeometry, failureMechanismSection.Geometry);
            Assert.AreEqual(0, failureMechanismSection.StartDistance);
            Assert.AreEqual(Math2D.Length(expectedGeometry), failureMechanismSection.EndDistance);
        }
    }
}