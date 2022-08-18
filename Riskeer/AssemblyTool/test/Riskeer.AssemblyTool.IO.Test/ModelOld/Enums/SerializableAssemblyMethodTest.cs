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
using Riskeer.AssemblyTool.IO.ModelOld.Enums;

namespace Riskeer.AssemblyTool.IO.Test.ModelOld.Enums
{
    [TestFixture]
    public class SerializableAssemblyMethodTest : EnumWithXmlEnumNameTestFixture<SerializableAssemblyMethod>
    {
        protected override IDictionary<SerializableAssemblyMethod, int> ExpectedValueForEnumValues =>
            new Dictionary<SerializableAssemblyMethod, int>
            {
                {
                    SerializableAssemblyMethod.BOI0A1, 1
                },
                {
                    SerializableAssemblyMethod.BOI0A2, 2
                },
                {
                    SerializableAssemblyMethod.BOI0B1, 3
                },
                {
                    SerializableAssemblyMethod.BOI0C1, 4
                },
                {
                    SerializableAssemblyMethod.BOI0C2, 5
                },
                {
                    SerializableAssemblyMethod.BOI1A1, 6
                },
                {
                    SerializableAssemblyMethod.BOI1A2, 7
                },
                {
                    SerializableAssemblyMethod.Manual, 8
                },
                {
                    SerializableAssemblyMethod.BOI2A1, 9
                },
                {
                    SerializableAssemblyMethod.BOI2B1, 10
                },
                {
                    SerializableAssemblyMethod.BOI3A1, 11
                },
                {
                    SerializableAssemblyMethod.BOI3B1, 12
                },
                {
                    SerializableAssemblyMethod.BOI3C1, 13
                }
            };

        protected override IDictionary<SerializableAssemblyMethod, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<SerializableAssemblyMethod, string>
            {
                {
                    SerializableAssemblyMethod.BOI0A1, "BOI-0A-1"
                },
                {
                    SerializableAssemblyMethod.BOI0A2, "BOI-0A-2"
                },
                {
                    SerializableAssemblyMethod.BOI0B1, "BOI-0B-1"
                },
                {
                    SerializableAssemblyMethod.BOI0C1, "BOI-0C-1"
                },
                {
                    SerializableAssemblyMethod.BOI0C2, "BOI-0C-2"
                },
                {
                    SerializableAssemblyMethod.BOI1A1, "BOI-1A-1"
                },
                {
                    SerializableAssemblyMethod.BOI1A2, "BOI-1A-2"
                },
                {
                    SerializableAssemblyMethod.Manual, "HANDMTG"
                },
                {
                    SerializableAssemblyMethod.BOI2A1, "BOI-2A-1"
                },
                {
                    SerializableAssemblyMethod.BOI2B1, "BOI-2B-1"
                },
                {
                    SerializableAssemblyMethod.BOI3A1, "BOI-3A-1"
                },
                {
                    SerializableAssemblyMethod.BOI3B1, "BOI-3B-1"
                },
                {
                    SerializableAssemblyMethod.BOI3C1, "BOI-3C-1"
                }
            };
    }
}