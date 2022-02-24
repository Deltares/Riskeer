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
        /// <see cref="AssessmentSectionAssemblyGroup"/> values.
        /// </summary>
        public static IEnumerable<TestCaseData> AssessmentSectionAssemblyCategoryGroupColorCases
        {
            get
            {
                yield return new TestCaseData(AssessmentSectionAssemblyGroup.APlus, Color.FromArgb(0, 255, 0));
                yield return new TestCaseData(AssessmentSectionAssemblyGroup.A, Color.FromArgb(118, 147, 60));
                yield return new TestCaseData(AssessmentSectionAssemblyGroup.B, Color.FromArgb(255, 255, 0));
                yield return new TestCaseData(AssessmentSectionAssemblyGroup.C, Color.FromArgb(255, 153, 0));
                yield return new TestCaseData(AssessmentSectionAssemblyGroup.D, Color.FromArgb(255, 0, 0));
                yield return new TestCaseData(AssessmentSectionAssemblyGroup.None, Color.White);
                yield return new TestCaseData(AssessmentSectionAssemblyGroup.NotAssessed, Color.White);
                yield return new TestCaseData(AssessmentSectionAssemblyGroup.NotApplicable, Color.White);
            }
        }
    }
}