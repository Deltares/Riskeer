// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/>
    /// </summary>
    public static class ExportableFailureMechanismTestHelper
    {
        /// <summary>
        /// Asserts the default failure mechanism.
        /// </summary>
        /// <param name="geometry">The expected geometry of the section it contains.</param>
        /// <param name="failureMechanismCode">The expected <see cref="ExportableFailureMechanismType"/>.</param>
        /// <param name="group">The expected <see cref="ExportableFailureMechanismGroup"/>.</param>
        /// <param name="failureMechanismAssemblyMethod">The expected <see cref="ExportableAssemblyMethod"/> which is used
        /// to generate a failure mechanism assembly result.</param>
        /// <param name="exportableFailureMechanism">The <see cref="ExportableFailureMechanism{TFailureMechanismAssemblyResult}"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The geometry defined by <paramref name="geometry"/> does not match with the section contained in <paramref name="exportableFailureMechanism"/>.</item>
        /// <item>The values in <paramref name="exportableFailureMechanism"/> do not match with
        /// <paramref name="failureMechanismCode"/>, <paramref name="group"/> or <see cref="failureMechanismAssemblyMethod"/>.</item>
        /// </list></exception>
        public static void AssertDefaultFailureMechanismWithProbability(IEnumerable<Point2D> geometry,
                                                                        ExportableFailureMechanismType failureMechanismCode,
                                                                        ExportableFailureMechanismGroup group,
                                                                        ExportableAssemblyMethod failureMechanismAssemblyMethod,
                                                                        ExportableFailureMechanism<ExportableFailureMechanismAssemblyResultWithProbability> exportableFailureMechanism)
        {
            Assert.AreEqual(group, exportableFailureMechanism.Group);
            Assert.AreEqual(failureMechanismCode, exportableFailureMechanism.Code);

            ExportableFailureMechanismAssemblyResultWithProbability failureMechanismAssemblyResult = exportableFailureMechanism.FailureMechanismAssembly;
            Assert.AreEqual(failureMechanismAssemblyMethod, failureMechanismAssemblyResult.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, failureMechanismAssemblyResult.AssemblyCategory);
            Assert.AreEqual(0, failureMechanismAssemblyResult.Probability);

            var exportableFailureMechanismSectionAssembly =
                (ExportableAggregatedFailureMechanismSectionAssemblyWithCombinedProbabilityResult) exportableFailureMechanism.SectionAssemblyResults.Single();
            ExportableSectionAssemblyResultWithProbability combinedAssembly = exportableFailureMechanismSectionAssembly.CombinedAssembly;
            Assert.AreEqual(ExportableAssemblyMethod.WBI0A1, combinedAssembly.AssemblyMethod);
            Assert.AreEqual(FailureMechanismAssemblyCategoryGroup.NotApplicable, combinedAssembly.AssemblyCategory);
            Assert.AreEqual(0, combinedAssembly.Probability);

            ExportableFailureMechanismSection failureMechanismSection = exportableFailureMechanismSectionAssembly.FailureMechanismSection;
            Assert.AreSame(geometry, failureMechanismSection.Geometry);
            Assert.AreEqual(0, failureMechanismSection.StartDistance);
            Assert.AreEqual(Math2D.Length(geometry), failureMechanismSection.EndDistance);
        }
    }
}