﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
    /// Class that can be used to assert the color corresponding to assembly groups.
    /// </summary>
    public static class AssemblyGroupColorTestHelper
    {
        /// <summary>
        /// Gets a collection of test cases to test the colors belonging to various
        /// <see cref="FailureMechanismSectionAssemblyGroup"/> values.
        /// </summary>
        public static IEnumerable<TestCaseData> FailureMechanismSectionAssemblyGroupColorCases
        {
            get
            {
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.ND, Color.FromArgb(192, 192, 192));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.III, Color.FromArgb(34, 139, 34));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.II, Color.FromArgb(146, 208, 80));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.I, Color.FromArgb(198, 224, 180));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.ZeroPlus, Color.FromArgb(255, 255, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.Zero, Color.FromArgb(255, 165, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.IMin, Color.FromArgb(255, 0, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.IIMin, Color.FromArgb(178, 34, 34));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.IIIMin, Color.FromArgb(128, 0, 0));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.D, Color.FromArgb(255, 90, 172));
                yield return new TestCaseData(FailureMechanismSectionAssemblyGroup.Gr, Color.FromArgb(255, 255, 255));
            }
        }
    }
}