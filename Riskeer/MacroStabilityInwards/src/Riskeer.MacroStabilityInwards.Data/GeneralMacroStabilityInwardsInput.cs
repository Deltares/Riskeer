﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// Class that holds all the overarching macro stability inwards calculation
    /// input parameters, e.g. the values that apply for all calculations.
    /// </summary>
    public class GeneralMacroStabilityInwardsInput : IGeneralMacroStabilityInwardsWaternetInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralMacroStabilityInwardsInput"/>.
        /// </summary>
        public GeneralMacroStabilityInwardsInput()
        {
            ModelFactor = 1.06;
            WaterVolumetricWeight = 9.81;
        }

        /// <summary>
        /// Gets the model factor used to calculate a reliability from a stability factor.
        /// </summary>
        public double ModelFactor { get; }

        /// <summary>
        /// Gets whether the length effect should be applied in the section.
        /// </summary>
        public bool ApplyLengthEffectInSection => true;

        /// <summary>
        /// Gets the volumetric weight of water.
        /// </summary>
        public double WaterVolumetricWeight { get; }
    }
}