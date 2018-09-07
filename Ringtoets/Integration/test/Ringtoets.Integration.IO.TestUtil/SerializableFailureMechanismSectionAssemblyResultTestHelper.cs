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

using NUnit.Framework;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Assembly;
using Ringtoets.Integration.IO.Creators;

namespace Ringtoets.Integration.IO.TestUtil
{
    /// <summary>
    /// Helper class to assert a <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
    /// </summary>
    public static class SerializableFailureMechanismSectionAssemblyResultTestHelper
    {
        /// <summary>
        /// Asserts a <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
        /// against an <see cref="ExportableSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="expectedResult">The <see cref="ExportableSectionAssemblyResult"/> to assert against.</param>
        /// <param name="expectedAssessmentType">The expected <see cref="SerializableAssessmentType"/>
        /// of the <see cref="SerializableFailureMechanismSectionAssemblyResult"/>.</param>
        /// <param name="actualResult">The <see cref="SerializableFailureMechanismSectionAssemblyResult"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The assembly methods do not match,</item>
        /// <item>The failure mechanism category group assembly results do not match,</item>
        /// <item>The failure mechanism section assembly type do not match,</item>
        /// <item>The probability of <paramref name="actualResult"/> has a value.</item>
        /// </list></exception>
        public static void AssertAssemblyResult(ExportableSectionAssemblyResult expectedResult,
                                                SerializableAssessmentType expectedAssessmentType,
                                                SerializableFailureMechanismSectionAssemblyResult actualResult)
        {
            Assert.AreEqual(SerializableFailureMechanismSectionCategoryGroupCreator.Create(expectedResult.AssemblyCategory),
                            actualResult.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedResult.AssemblyMethod),
                            actualResult.AssemblyMethod);
            Assert.AreEqual(expectedAssessmentType, actualResult.AssessmentType);
            Assert.IsNull(actualResult.Probability);
        }

        /// <summary>
        /// Asserts a <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
        /// against an <see cref="ExportableSectionAssemblyResultWithProbability"/>.
        /// </summary>
        /// <param name="expectedResult">The <see cref="ExportableSectionAssemblyResultWithProbability"/> to assert against.</param>
        /// <param name="expectedAssessmentType">The expected <see cref="SerializableAssessmentType"/>
        /// of the <see cref="SerializableFailureMechanismSectionAssemblyResult"/>.</param>
        /// <param name="actualResult">The <see cref="SerializableFailureMechanismSectionAssemblyResult"/> to assert.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The assembly methods do not match,</item>
        /// <item>The failure mechanism category group assembly results do not match,</item>
        /// <item>The failure mechanism section assembly type do not match,</item>
        /// <item>The probability of <paramref name="actualResult"/> does not match.</item>
        /// </list></exception>
        public static void AssertAssemblyResult(ExportableSectionAssemblyResultWithProbability expectedResult,
                                                SerializableAssessmentType expectedAssessmentType,
                                                SerializableFailureMechanismSectionAssemblyResult actualResult)
        {
            Assert.AreEqual(SerializableFailureMechanismSectionCategoryGroupCreator.Create(expectedResult.AssemblyCategory),
                            actualResult.CategoryGroup);
            Assert.AreEqual(SerializableAssemblyMethodCreator.Create(expectedResult.AssemblyMethod),
                            actualResult.AssemblyMethod);
            Assert.AreEqual(expectedAssessmentType, actualResult.AssessmentType);
            Assert.AreEqual(expectedResult.Probability, actualResult.Probability);
        }
    }
}