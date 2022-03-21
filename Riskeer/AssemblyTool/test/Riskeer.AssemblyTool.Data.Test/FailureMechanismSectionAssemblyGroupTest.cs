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

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class FailureMechanismSectionAssemblyGroupTest : EnumWithResourcesDisplayNameTestFixture<FailureMechanismSectionAssemblyGroup>
    {
        protected override IDictionary<FailureMechanismSectionAssemblyGroup, int> ExpectedValueForEnumValues =>
            new Dictionary<FailureMechanismSectionAssemblyGroup, int>
            {
                {
                    FailureMechanismSectionAssemblyGroup.NotDominant, 1
                },
                {
                    FailureMechanismSectionAssemblyGroup.III, 2
                },
                {
                    FailureMechanismSectionAssemblyGroup.II, 3
                },
                {
                    FailureMechanismSectionAssemblyGroup.I, 4
                },
                {
                    FailureMechanismSectionAssemblyGroup.Zero, 5
                },
                {
                    FailureMechanismSectionAssemblyGroup.IMin, 6
                },
                {
                    FailureMechanismSectionAssemblyGroup.IIMin, 7
                },
                {
                    FailureMechanismSectionAssemblyGroup.IIIMin, 8
                },
                {
                    FailureMechanismSectionAssemblyGroup.Dominant, 9
                },
                {
                    FailureMechanismSectionAssemblyGroup.Gr, 10
                },
                {
                    FailureMechanismSectionAssemblyGroup.Nr, 11
                }
            };

        protected override IDictionary<FailureMechanismSectionAssemblyGroup, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<FailureMechanismSectionAssemblyGroup, string>
            {
                {
                    FailureMechanismSectionAssemblyGroup.NotDominant, "NDo"
                },
                {
                    FailureMechanismSectionAssemblyGroup.III, "+III"
                },
                {
                    FailureMechanismSectionAssemblyGroup.II, "+II"
                },
                {
                    FailureMechanismSectionAssemblyGroup.I, "+I"
                },
                {
                    FailureMechanismSectionAssemblyGroup.Zero, "0"
                },
                {
                    FailureMechanismSectionAssemblyGroup.IMin, "-I"
                },
                {
                    FailureMechanismSectionAssemblyGroup.IIMin, "-II"
                },
                {
                    FailureMechanismSectionAssemblyGroup.IIIMin, "-III"
                },
                {
                    FailureMechanismSectionAssemblyGroup.Dominant, "Do"
                },
                {
                    FailureMechanismSectionAssemblyGroup.Gr, ""
                },
                {
                    FailureMechanismSectionAssemblyGroup.Nr, "NR"
                }
            };
    }
}