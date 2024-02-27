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

namespace Riskeer.AssemblyTool.Data.Test
{
    [TestFixture]
    public class AssemblyMethodTest : EnumValuesTestFixture<AssemblyMethod, int>
    {
        protected override IDictionary<AssemblyMethod, int> ExpectedValueForEnumValues =>
            new Dictionary<AssemblyMethod, int>
            {
                {
                    AssemblyMethod.BOI0A1, 1
                },
                {
                    AssemblyMethod.BOI0A2, 2
                },
                {
                    AssemblyMethod.BOI0B1, 3
                },
                {
                    AssemblyMethod.BOI0C1, 4
                },
                {
                    AssemblyMethod.BOI0C2, 5
                },
                {
                    AssemblyMethod.BOI1A1, 6
                },
                {
                    AssemblyMethod.BOI1A2, 7
                },
                {
                    AssemblyMethod.Manual, 8
                },
                {
                    AssemblyMethod.BOI2A1, 9
                },
                {
                    AssemblyMethod.BOI2A2, 10
                },
                {
                    AssemblyMethod.BOI2B1, 11
                }
            };
    }
}