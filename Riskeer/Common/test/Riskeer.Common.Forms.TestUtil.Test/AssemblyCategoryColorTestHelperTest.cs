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
using System.Linq;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;

namespace Riskeer.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class AssemblyCategoryColorTestHelperTest
    {
        [Test]
        public void FailureMechanismSectionAssemblyCategoryGroupColorCases_Always_ReturnsExpectedCases()
        {
            // Call
            IEnumerable<TestCaseData> testCases = AssemblyCategoryColorTestHelper.FailureMechanismSectionAssemblyCategoryGroupColorCases.ToArray();

            // Assert
            var expectedCases = new[]
            {
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.NotApplicable, Color.FromArgb(255, 255, 255)),
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.None, Color.FromArgb(255, 255, 255)),
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.Iv, Color.FromArgb(0, 255, 0)),
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IIv, Color.FromArgb(118, 147, 60)),
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IIIv, Color.FromArgb(255, 255, 0)),
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.IVv, Color.FromArgb(204, 192, 218)),
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.Vv, Color.FromArgb(255, 153, 0)),
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.VIv, Color.FromArgb(255, 0, 0)),
                new TestCaseData(FailureMechanismSectionAssemblyCategoryGroup.VIIv, Color.FromArgb(255, 255, 255))
            };

            Assert.AreEqual(expectedCases.Length, testCases.Count());
            for (var i = 0; i < testCases.Count(); i++)
            {
                Assert.AreEqual(expectedCases[i].Arguments[0], testCases.ElementAt(i).Arguments[0]);
                Assert.AreEqual(expectedCases[i].Arguments[1], testCases.ElementAt(i).Arguments[1]);
            }
        }

        [Test]
        public void AssessmentSectionAssemblyCategoryGroupColorCases_Always_ReturnsExpectedCases()
        {
            // Call
            IEnumerable<TestCaseData> testCases = AssemblyCategoryColorTestHelper.AssessmentSectionAssemblyCategoryGroupColorCases.ToArray();

            // Assert
            var expectedCases = new[]
            {
                new TestCaseData(AssessmentSectionAssemblyCategoryGroup.APlus, Color.FromArgb(0, 255, 0)),
                new TestCaseData(AssessmentSectionAssemblyCategoryGroup.A, Color.FromArgb(118, 147, 60)),
                new TestCaseData(AssessmentSectionAssemblyCategoryGroup.B, Color.FromArgb(255, 255, 0)),
                new TestCaseData(AssessmentSectionAssemblyCategoryGroup.C, Color.FromArgb(255, 153, 0)),
                new TestCaseData(AssessmentSectionAssemblyCategoryGroup.D, Color.FromArgb(255, 0, 0)),
                new TestCaseData(AssessmentSectionAssemblyCategoryGroup.None, Color.White),
                new TestCaseData(AssessmentSectionAssemblyCategoryGroup.NotAssessed, Color.White),
                new TestCaseData(AssessmentSectionAssemblyCategoryGroup.NotApplicable, Color.White)
            };

            Assert.AreEqual(expectedCases.Length, testCases.Count());
            for (var i = 0; i < testCases.Count(); i++)
            {
                Assert.AreEqual(expectedCases[i].Arguments[0], testCases.ElementAt(i).Arguments[0]);
                Assert.AreEqual(expectedCases[i].Arguments[1], testCases.ElementAt(i).Arguments[1]);
            }
        }

        [Test]
        public void FailureMechanismAsseblyCategoryGroupColorCases_Always_ReturnsExpectedCases()
        {
            IEnumerable<TestCaseData> testCases = AssemblyCategoryColorTestHelper.FailureMechanismAssemblyCategoryGroupColorCases.ToArray();

            // Assert
            var expectedCases = new[]
            {
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.NotApplicable, Color.FromArgb(255, 255, 255)),
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.None, Color.FromArgb(255, 255, 255)),
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.It, Color.FromArgb(0, 255, 0)),
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.IIt, Color.FromArgb(118, 147, 60)),
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.IIIt, Color.FromArgb(255, 255, 0)),
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.IVt, Color.FromArgb(204, 192, 218)),
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.Vt, Color.FromArgb(255, 153, 0)),
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.VIt, Color.FromArgb(255, 0, 0)),
                new TestCaseData(FailureMechanismAssemblyCategoryGroup.VIIt, Color.FromArgb(255, 255, 255))
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