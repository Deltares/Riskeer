// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Collections.Generic;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input
{
    /// <summary>
    /// A soil profile for which its properties have been adapted to perform a calculation.
    /// </summary>
    public class SoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilProfile"/>.
        /// </summary>
        /// <param name="layers">The layers in the profile.</param>
        /// <param name="preconsolidationStresses">The preconsolidation stresses in the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SoilProfile(IEnumerable<SoilLayer> layers, IEnumerable<PreconsolidationStress> preconsolidationStresses)
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

        /// <summary>
        /// Gets the layers in the profile.
        /// </summary>
        public IEnumerable<SoilLayer> Layers { get; }

        /// <summary>
        /// Gets the preconsolidation stresses in the profile.
        /// </summary>
        public IEnumerable<PreconsolidationStress> PreconsolidationStresses { get; }
    }
}