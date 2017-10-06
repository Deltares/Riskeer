﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input
{
    /// <summary>
    /// A soil profile for which its properties have been adapted to perform a calculation.
    /// </summary>
    public class UpliftVanSoilProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanSoilProfile"/>.
        /// </summary>
        /// <param name="layers">The layers in the profile.</param>
        /// <param name="preconsolidationStresses">The preconsolidation stresses in the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public UpliftVanSoilProfile(IEnumerable<UpliftVanSoilLayer> layers, IEnumerable<UpliftVanPreconsolidationStress> preconsolidationStresses)
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
        public IEnumerable<UpliftVanSoilLayer> Layers { get; }

        /// <summary>
        /// Gets the preconsolidation stresses in the profile.
        /// </summary>
        public IEnumerable<UpliftVanPreconsolidationStress> PreconsolidationStresses { get; }
    }
}