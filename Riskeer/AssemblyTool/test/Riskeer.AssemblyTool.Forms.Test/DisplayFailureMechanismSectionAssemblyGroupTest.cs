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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.AssemblyTool.Forms.Test
{
    [TestFixture]
    public class DisplayFailureMechanismSectionAssemblyGroupTest : EnumWithResourcesDisplayNameTestFixture<DisplayFailureMechanismSectionAssemblyGroup>
    {
        protected override IDictionary<DisplayFailureMechanismSectionAssemblyGroup, int> ExpectedValueForEnumValues =>
            new Dictionary<DisplayFailureMechanismSectionAssemblyGroup, int>
            {
                {
                    DisplayFailureMechanismSectionAssemblyGroup.ND, 1
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.III, 2
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.II, 3
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.I, 4
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.ZeroPlus, 5
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.Zero, 6
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.IMin, 7
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.IIMin, 8
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.IIIMin, 9
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.D, 10
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.GR, 11
                }
            };

        protected override IDictionary<DisplayFailureMechanismSectionAssemblyGroup, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<DisplayFailureMechanismSectionAssemblyGroup, string>
            {
                {
                    DisplayFailureMechanismSectionAssemblyGroup.ND, "ND"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.III, "+III"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.II, "+II"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.I, "+I"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.ZeroPlus, "+0"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.Zero, "0"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.IMin, "-I"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.IIMin, "-II"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.IIIMin, "-III"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.D, "D"
                },
                {
                    DisplayFailureMechanismSectionAssemblyGroup.GR, ""
                }
            };
    }
}