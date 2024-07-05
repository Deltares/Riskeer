// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class ExportableFailureMechanismSectionAssemblyGroupTest : EnumWithResourcesDisplayNameTestFixture<ExportableFailureMechanismSectionAssemblyGroup>
    {
        protected override IDictionary<ExportableFailureMechanismSectionAssemblyGroup, int> ExpectedValueForEnumValues =>
            new Dictionary<ExportableFailureMechanismSectionAssemblyGroup, int>
            {
                {
                    ExportableFailureMechanismSectionAssemblyGroup.NotDominant, 1
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.III, 2
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.II, 3
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.I, 4
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.Zero, 5
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.IMin, 6
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.IIMin, 7
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.IIIMin, 8
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.Dominant, 9
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.NoResult, 10
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.NotRelevant, 11
                }
            };

        protected override IDictionary<ExportableFailureMechanismSectionAssemblyGroup, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<ExportableFailureMechanismSectionAssemblyGroup, string>
            {
                {
                    ExportableFailureMechanismSectionAssemblyGroup.NotDominant, "NDo"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.III, "+III"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.II, "+II"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.I, "+I"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.Zero, "0"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.IMin, "-I"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.IIMin, "-II"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.IIIMin, "-III"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.Dominant, "Do"
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.NoResult, ""
                },
                {
                    ExportableFailureMechanismSectionAssemblyGroup.NotRelevant, "NR"
                }
            };
    }
}