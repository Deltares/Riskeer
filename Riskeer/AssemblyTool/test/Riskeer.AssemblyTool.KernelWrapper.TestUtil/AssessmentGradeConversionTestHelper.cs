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
using Assembly.Kernel.Model;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class that can be used to assert the conversions of <see cref="EAssessmentGrade"/>.
    /// </summary>
    public static class AssessmentGradeConversionTestHelper
    {
        /// <summary>
        /// Gets a collection of test cases to test the conversion between <see cref="EAssessmentGrade"/>
        /// and <see cref="AssessmentSectionAssemblyCategoryGroup"/>.
        /// </summary>
        public static IEnumerable<TestCaseData> AsssementGradeConversionCases
        {
            get
            {
                yield return new TestCaseData(EAssessmentGrade.APlus, AssessmentSectionAssemblyCategoryGroup.APlus);
                yield return new TestCaseData(EAssessmentGrade.A, AssessmentSectionAssemblyCategoryGroup.A);
                yield return new TestCaseData(EAssessmentGrade.B, AssessmentSectionAssemblyCategoryGroup.B);
                yield return new TestCaseData(EAssessmentGrade.C, AssessmentSectionAssemblyCategoryGroup.C);
                yield return new TestCaseData(EAssessmentGrade.D, AssessmentSectionAssemblyCategoryGroup.D);
                yield return new TestCaseData(EAssessmentGrade.Gr, AssessmentSectionAssemblyCategoryGroup.None);
                yield return new TestCaseData(EAssessmentGrade.Nvt, AssessmentSectionAssemblyCategoryGroup.NotApplicable);
                yield return new TestCaseData(EAssessmentGrade.Ngo, AssessmentSectionAssemblyCategoryGroup.NotAssessed);
            }
        }
    }
}