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
using System.Drawing;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Class that can be used to assert the color corresponding to assembly category groups.
    /// </summary>
    public static class AssemblyCategoryColorTestHelper
    {
        /// <summary>
        /// Gets a collection of test cases to test the colors belonging to various
        /// <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> values.
        /// </summary>
        public static IEnumerable<TestCaseData> FailureMechanismSectionAssemblyCategoryGroupColorCases
        {
            get
            {
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, Color.FromArgb(255, 255, 255));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.None, Color.FromArgb(255, 255, 255));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.Iv, Color.FromArgb(0, 255, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IIv, Color.FromArgb(118, 147, 60));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IIIv, Color.FromArgb(255, 255, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IVv, Color.FromArgb(204, 192, 218));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.Vv, Color.FromArgb(255, 153, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.VIv, Color.FromArgb(255, 0, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.VIIv, Color.FromArgb(255, 255, 255));
            }
        }

        /// <summary>
        /// Gets a collection of test cases to test the colors belonging to various
        /// <see cref="AssessmentSectionAssemblyCategoryGroup"/> values.
        /// </summary>
        public static IEnumerable<TestCaseData> AssessmentSectionAssemblyCategoryGroupColorCases
        {
            get
            {
                yield return new TestCaseData(AssessmentSectionAssemblyCategoryGroup.APlus, Color.FromArgb(0, 255, 0));
                yield return new TestCaseData(AssessmentSectionAssemblyCategoryGroup.A, Color.FromArgb(118, 147, 60));
                yield return new TestCaseData(AssessmentSectionAssemblyCategoryGroup.B, Color.FromArgb(255, 255, 0));
                yield return new TestCaseData(AssessmentSectionAssemblyCategoryGroup.C, Color.FromArgb(255, 153, 0));
                yield return new TestCaseData(AssessmentSectionAssemblyCategoryGroup.D, Color.FromArgb(255, 0, 0));
                yield return new TestCaseData(AssessmentSectionAssemblyCategoryGroup.None, Color.White);
                yield return new TestCaseData(AssessmentSectionAssemblyCategoryGroup.NotAssessed, Color.White);
                yield return new TestCaseData(AssessmentSectionAssemblyCategoryGroup.NotApplicable, Color.White);
            }
        }

        /// <summary>
        /// Gets a collection of test cases to test the colors belong to various 
        /// <see cref="FailureMechanismAssemblyCategoryGroup"/> values.
        /// </summary>
        public static IEnumerable<TestCaseData> FailureMechanismAssemblyCategoryGroupColorCases
        {
            get
            {
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.NotApplicable, Color.FromArgb(255, 255, 255));
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.None, Color.FromArgb(255, 255, 255));
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.It, Color.FromArgb(0, 255, 0));
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.IIt, Color.FromArgb(118, 147, 60));
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.IIIt, Color.FromArgb(255, 255, 0));
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.IVt, Color.FromArgb(204, 192, 218));
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.Vt, Color.FromArgb(255, 153, 0));
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.VIt, Color.FromArgb(255, 0, 0));
                yield return new TestCaseData(FailureMechanismAssemblyCategoryGroup.VIIt, Color.FromArgb(255, 255, 255));
            }
        }
    }
}