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

using System.Collections.Generic;
using System.Linq;
using Assembly.Kernel.Model;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Test
{
    [TestFixture]
    public class AssessmentGradeConversionTestHelperTest
    {
        [Test]
        public void AsssementGradeConversionCases_Always_ReturnsExpectedCases()
        {
            // Setup
            IEnumerable<TestCaseData> testCases = AssessmentGradeConversionTestHelper.AsssementGradeConversionCases.ToArray();

            // Assert
            var expectedCases = new[]
            {
                new TestCaseData(EAssessmentGrade.APlus, AssessmentSectionAssemblyCategoryGroup.APlus),
                new TestCaseData(EAssessmentGrade.A, AssessmentSectionAssemblyCategoryGroup.A),
                new TestCaseData(EAssessmentGrade.B, AssessmentSectionAssemblyCategoryGroup.B),
                new TestCaseData(EAssessmentGrade.C, AssessmentSectionAssemblyCategoryGroup.C),
                new TestCaseData(EAssessmentGrade.D, AssessmentSectionAssemblyCategoryGroup.D),
                new TestCaseData(EAssessmentGrade.Gr, AssessmentSectionAssemblyCategoryGroup.None),
                new TestCaseData(EAssessmentGrade.Nvt, AssessmentSectionAssemblyCategoryGroup.NotApplicable),
                new TestCaseData(EAssessmentGrade.Ngo, AssessmentSectionAssemblyCategoryGroup.NotAssessed)
            };

            Assert.AreEqual(expectedCases.Length, testCases.Count());
            for (var i = 0; i < testCases.Count(); i++)
            {
                Assert.AreEqual(expectedCases[i].Arguments[0], testCases.ElementAt(i).Arguments[0]);
                Assert.AreEqual(expectedCases[i].Arguments[1], testCases.ElementAt(i).Arguments[1]);
            }
        }
    }
}