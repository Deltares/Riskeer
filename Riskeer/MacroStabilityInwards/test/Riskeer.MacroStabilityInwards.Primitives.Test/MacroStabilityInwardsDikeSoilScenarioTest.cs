// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsDikeSoilScenarioTest : EnumWithResourcesDisplayNameTestFixture<MacroStabilityInwardsDikeSoilScenario>
    {
        protected override IDictionary<MacroStabilityInwardsDikeSoilScenario, string> ExpectedDisplayNameForEnumValues
        {
            get
            {
                return new Dictionary<MacroStabilityInwardsDikeSoilScenario, string>
                {
                    {
                        MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, "Klei dijk op klei (geval 1A)"
                    },
                    {
                        MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, "Zand dijk op klei (geval 2A)"
                    },
                    {
                        MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, "Klei dijk op zand (geval 1B)"
                    },
                    {
                        MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, "Zand dijk op zand (geval 2B)"
                    }
                };
            }
        }

        protected override IDictionary<MacroStabilityInwardsDikeSoilScenario, int> ExpectedValueForEnumValues
        {
            get
            {
                return new Dictionary<MacroStabilityInwardsDikeSoilScenario, int>
                {
                    {
                        MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay, 1
                    },
                    {
                        MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay, 2
                    },
                    {
                        MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand, 3
                    },
                    {
                        MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand, 4
                    }
                };
            }
        }
    }
}