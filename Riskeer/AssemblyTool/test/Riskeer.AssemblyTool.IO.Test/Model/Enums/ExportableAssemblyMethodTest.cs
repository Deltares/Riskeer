// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
    public class ExportableAssemblyMethodTest : EnumWithResourcesDisplayNameTestFixture<ExportableAssemblyMethod>
    {
        protected override IDictionary<ExportableAssemblyMethod, int> ExpectedValueForEnumValues =>
            new Dictionary<ExportableAssemblyMethod, int>
            {
                {
                    ExportableAssemblyMethod.BOI0A1, 1
                },
                {
                    ExportableAssemblyMethod.BOI0A2, 2
                },
                {
                    ExportableAssemblyMethod.BOI0B1, 3
                },
                {
                    ExportableAssemblyMethod.BOI0C1, 4
                },
                {
                    ExportableAssemblyMethod.BOI0C2, 5
                },
                {
                    ExportableAssemblyMethod.BOI1A1, 6
                },
                {
                    ExportableAssemblyMethod.BOI1A2, 7
                },
                {
                    ExportableAssemblyMethod.Manual, 8
                },
                {
                    ExportableAssemblyMethod.BOI2A1, 9
                },
                {
                    ExportableAssemblyMethod.BOI2B1, 10
                },
                {
                    ExportableAssemblyMethod.BOI3A1, 11
                },
                {
                    ExportableAssemblyMethod.BOI3B1, 12
                },
                {
                    ExportableAssemblyMethod.BOI3C1, 13
                }
            };

        protected override IDictionary<ExportableAssemblyMethod, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<ExportableAssemblyMethod, string>
            {
                {
                    ExportableAssemblyMethod.BOI0A1, "BOI-0A-1"
                },
                {
                    ExportableAssemblyMethod.BOI0A2, "BOI-0A-2"
                },
                {
                    ExportableAssemblyMethod.BOI0B1, "BOI-0B-1"
                },
                {
                    ExportableAssemblyMethod.BOI0C1, "BOI-0C-1"
                },
                {
                    ExportableAssemblyMethod.BOI0C2, "BOI-0C-2"
                },
                {
                    ExportableAssemblyMethod.BOI1A1, "BOI-1A-1"
                },
                {
                    ExportableAssemblyMethod.BOI1A2, "BOI-1A-2"
                },
                {
                    ExportableAssemblyMethod.Manual, "HANDMATIG"
                },
                {
                    ExportableAssemblyMethod.BOI2A1, "BOI-2A-1"
                },
                {
                    ExportableAssemblyMethod.BOI2B1, "BOI-2B-1"
                },
                {
                    ExportableAssemblyMethod.BOI3A1, "BOI-3A-1"
                },
                {
                    ExportableAssemblyMethod.BOI3B1, "BOI-3B-1"
                },
                {
                    ExportableAssemblyMethod.BOI3C1, "BOI-3C-1"
                }
            };
    }
}