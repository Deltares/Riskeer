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

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Interface for a soil profile for which its properties have been adapted using a surface line.
    /// </summary>
    public interface IMacroStabilityInwardsSoilProfileUnderSurfaceLine
    {
        /// <summary>
        /// Gets the layers in the profile.
        /// </summary>
        IEnumerable<IMacroStabilityInwardsSoilLayer2D> Layers { get; }

        /// <summary>
        /// Gets the preconsolidation stresses in the profile.
        /// </summary>
        IEnumerable<IMacroStabilityInwardsPreconsolidationStress> PreconsolidationStresses { get; }
    }
}