// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    public class ExportableAssessmentSectionAssemblyGroupTest : EnumWithResourcesDisplayNameTestFixture<ExportableAssessmentSectionAssemblyGroup>
    {
        protected override IDictionary<ExportableAssessmentSectionAssemblyGroup, int> ExpectedValueForEnumValues =>
            new Dictionary<ExportableAssessmentSectionAssemblyGroup, int>
            {
                {
                    ExportableAssessmentSectionAssemblyGroup.APlus, 1
                },
                {
                    ExportableAssessmentSectionAssemblyGroup.A, 2
                },
                {
                    ExportableAssessmentSectionAssemblyGroup.B, 3
                },
                {
                    ExportableAssessmentSectionAssemblyGroup.C, 4
                },
                {
                    ExportableAssessmentSectionAssemblyGroup.D, 5
                }
            };

        protected override IDictionary<ExportableAssessmentSectionAssemblyGroup, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<ExportableAssessmentSectionAssemblyGroup, string>
            {
                {
                    ExportableAssessmentSectionAssemblyGroup.APlus, "A+"
                },
                {
                    ExportableAssessmentSectionAssemblyGroup.A, "A"
                },
                {
                    ExportableAssessmentSectionAssemblyGroup.B, "B"
                },
                {
                    ExportableAssessmentSectionAssemblyGroup.C, "C"
                },
                {
                    ExportableAssessmentSectionAssemblyGroup.D, "D"
                }
            };
    }
}