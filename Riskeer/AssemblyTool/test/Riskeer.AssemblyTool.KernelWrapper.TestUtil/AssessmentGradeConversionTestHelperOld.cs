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

using System.Collections.Generic;
using Assembly.Kernel.Old.Model;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class that can be used to assert the conversions of <see cref="EAssessmentGrade"/>.
    /// </summary>
    public static class AssessmentGradeConversionTestHelperOld
    {
        /// <summary>
        /// Gets a collection of test cases to test the conversion between <see cref="EAssessmentGrade"/>
        /// and <see cref="AssessmentSectionAssemblyGroup"/>.
        /// </summary>
        public static IEnumerable<TestCaseData> AssessmentGradeConversionCases
        {
            get
            {
                yield return new TestCaseData(EAssessmentGrade.APlus, AssessmentSectionAssemblyGroup.APlus);
                yield return new TestCaseData(EAssessmentGrade.A, AssessmentSectionAssemblyGroup.A);
                yield return new TestCaseData(EAssessmentGrade.B, AssessmentSectionAssemblyGroup.B);
                yield return new TestCaseData(EAssessmentGrade.C, AssessmentSectionAssemblyGroup.C);
                yield return new TestCaseData(EAssessmentGrade.D, AssessmentSectionAssemblyGroup.D);
                yield return new TestCaseData(EAssessmentGrade.Gr, AssessmentSectionAssemblyGroup.None);
                yield return new TestCaseData(EAssessmentGrade.Nvt, AssessmentSectionAssemblyGroup.NotApplicable);
                yield return new TestCaseData(EAssessmentGrade.Ngo, AssessmentSectionAssemblyGroup.NotAssessed);
            }
        }
    }
}