// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Ringtoets.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.AssemblyTool.IO.Test.Model.Enums
{
    [TestFixture]
    public class AssemblyMethodTest : EnumWithXmlEnumNameTestFixture<AssemblyMethod>
    {
        protected override IDictionary<AssemblyMethod, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<AssemblyMethod, int>
                {
                    {
                        AssemblyMethod.WBI0E1, 1
                    },
                    {
                        AssemblyMethod.WBI0E3, 2
                    },
                    {
                        AssemblyMethod.WBI0G1, 3
                    },
                    {
                        AssemblyMethod.WBI0G3, 4
                    },
                    {
                        AssemblyMethod.WBI0G4, 5
                    },
                    {
                        AssemblyMethod.WBI0G5, 6
                    },
                    {
                        AssemblyMethod.WBI0G6, 7
                    },
                    {
                        AssemblyMethod.WBI0T1, 8
                    },
                    {
                        AssemblyMethod.WBI0T3, 9
                    },
                    {
                        AssemblyMethod.WBI0T4, 10
                    },
                    {
                        AssemblyMethod.WBI0T5, 11
                    },
                    {
                        AssemblyMethod.WBI0T6, 12
                    },
                    {
                        AssemblyMethod.WBI0T7, 13
                    },
                    {
                        AssemblyMethod.WBI0A1, 14
                    },
                    {
                        AssemblyMethod.WBI1A1, 15
                    },
                    {
                        AssemblyMethod.WBI1B1, 16
                    },
                    {
                        AssemblyMethod.WBI2A1, 17
                    },
                    {
                        AssemblyMethod.WBI2B1, 18
                    },
                    {
                        AssemblyMethod.WBI2C1, 19
                    },
                    {
                        AssemblyMethod.WBI3A1, 20
                    },
                    {
                        AssemblyMethod.WBI3B1, 21
                    },
                    {
                        AssemblyMethod.WBI3C1, 22
                    }
                };
            }
        }

        protected override IDictionary<AssemblyMethod, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<AssemblyMethod, string>
                {
                    {
                        AssemblyMethod.WBI0E1, "WBI-0E-1"
                    },
                    {
                        AssemblyMethod.WBI0E3, "WBI-0E-3"
                    },
                    {
                        AssemblyMethod.WBI0G1, "WBI-0G-1"
                    },
                    {
                        AssemblyMethod.WBI0G3, "WBI-0G-3"
                    },
                    {
                        AssemblyMethod.WBI0G4, "WBI-0G-4"
                    },
                    {
                        AssemblyMethod.WBI0G5, "WBI-0G-5"
                    },
                    {
                        AssemblyMethod.WBI0G6, "WBI-0G-6"
                    },
                    {
                        AssemblyMethod.WBI0T1, "WBI-0T-1"
                    },
                    {
                        AssemblyMethod.WBI0T3, "WBI-0T-3"
                    },
                    {
                        AssemblyMethod.WBI0T4, "WBI-0T-4"
                    },
                    {
                        AssemblyMethod.WBI0T5, "WBI-0T-5"
                    },
                    {
                        AssemblyMethod.WBI0T6, "WBI-0T-6"
                    },
                    {
                        AssemblyMethod.WBI0T7, "WBI-0T-7"
                    },
                    {
                        AssemblyMethod.WBI0A1, "WBI-0A-1"
                    },
                    {
                        AssemblyMethod.WBI1A1, "WBI-1A-1"
                    },
                    {
                        AssemblyMethod.WBI1B1, "WBI-1B-1"
                    },
                    {
                        AssemblyMethod.WBI2A1, "WBI-2A-1"
                    },
                    {
                        AssemblyMethod.WBI2B1, "WBI-2B-1"
                    },
                    {
                        AssemblyMethod.WBI2C1, "WBI-2C-1"
                    },
                    {
                        AssemblyMethod.WBI3A1, "WBI-3A-1"
                    },
                    {
                        AssemblyMethod.WBI3B1, "WBI-3B-1"
                    },
                    {
                        AssemblyMethod.WBI3C1, "WBI-3C-1"
                    }
                };
            }
        }
    }
}