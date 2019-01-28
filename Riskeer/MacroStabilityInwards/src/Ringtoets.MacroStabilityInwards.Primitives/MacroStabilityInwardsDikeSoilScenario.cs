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

using Core.Common.Util.Attributes;
using Ringtoets.MacroStabilityInwards.Primitives.Properties;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// The dike soil scenario types.
    /// </summary>
    public enum MacroStabilityInwardsDikeSoilScenario
    {
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClayDikeOnClay_DisplayName))]
        ClayDikeOnClay = 1,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SandDikeOnClay_DisplayName))]
        SandDikeOnClay = 2,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.ClayDikeOnSand_DisplayName))]
        ClayDikeOnSand = 3,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.SandDikeOnSand_DisplayName))]
        SandDikeOnSand = 4
    }
}