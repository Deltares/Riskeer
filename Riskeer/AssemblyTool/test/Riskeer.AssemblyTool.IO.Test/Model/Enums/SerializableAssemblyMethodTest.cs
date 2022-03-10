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
using Riskeer.AssemblyTool.IO.Model.Enums;

namespace Riskeer.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class SerializableAssemblyMethodTest : EnumWithXmlEnumNameTestFixture<SerializableAssemblyMethod>
    {
        protected override IDictionary<SerializableAssemblyMethod, int> ExpectedValueForEnumValues =>
            new Dictionary<SerializableAssemblyMethod, int>
            {
                {
                    SerializableAssemblyMethod.WBI0A2, 1
                },
                {
                    SerializableAssemblyMethod.WBI1B1, 2
                },
                {
                    SerializableAssemblyMethod.WBI2B1, 3
                },
                {
                    SerializableAssemblyMethod.WBI3A1, 4
                },
                {
                    SerializableAssemblyMethod.WBI3B1, 5
                },
                {
                    SerializableAssemblyMethod.WBI3C1, 6
                },
                {
                    SerializableAssemblyMethod.Manual, 7
                }
            };

        protected override IDictionary<SerializableAssemblyMethod, string> ExpectedDisplayNameForEnumValues =>
            new Dictionary<SerializableAssemblyMethod, string>
            {
                {
                    SerializableAssemblyMethod.WBI0A2, "WBI-0A-2"
                },
                {
                    SerializableAssemblyMethod.WBI1B1, "WBI-1B-1"
                },
                {
                    SerializableAssemblyMethod.WBI2B1, "WBI-2B-1"
                },
                {
                    SerializableAssemblyMethod.WBI3A1, "WBI-3A-1"
                },
                {
                    SerializableAssemblyMethod.WBI3B1, "WBI-3B-1"
                },
                {
                    SerializableAssemblyMethod.WBI3C1, "WBI-3C-1"
                },
                {
                    SerializableAssemblyMethod.Manual, "Handmatig"
                }
            };
    }
}