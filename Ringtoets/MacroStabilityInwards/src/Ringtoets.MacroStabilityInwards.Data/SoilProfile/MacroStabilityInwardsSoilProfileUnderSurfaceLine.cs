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

using System;
using System.Collections.Generic;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.SoilProfile
{
    /// <summary>
    /// A soil profile for which its properties have been adapted using a surface line.
    /// </summary>
    public class MacroStabilityInwardsSoilProfileUnderSurfaceLine : IMacroStabilityInwardsSoilProfileUnderSurfaceLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilProfileUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="layers">The layers in the profile.</param>
        /// <param name="preconsolidationStresses">The preconsolidation stresses defined for the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilProfileUnderSurfaceLine(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers,
                                                                IEnumerable<IMacroStabilityInwardsPreconsolidationStress> preconsolidationStresses)
        {
            if (layers == null)
            {
                throw new ArgumentNullException(nameof(layers));
            }

            if (preconsolidationStresses == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStresses));
            }

            Layers = layers;
            PreconsolidationStresses = preconsolidationStresses;
        }

        public IEnumerable<MacroStabilityInwardsSoilLayer2D> Layers { get; }

        public IEnumerable<IMacroStabilityInwardsPreconsolidationStress> PreconsolidationStresses { get; }
    }
}