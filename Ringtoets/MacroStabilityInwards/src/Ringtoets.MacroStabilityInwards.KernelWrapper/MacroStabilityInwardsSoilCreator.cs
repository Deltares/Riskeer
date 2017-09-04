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
using System.Linq;
using Deltares.WTIStability.Data.Geo;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// Creates <see cref="Soil"/> instances which are required by the <see cref="MacroStabilityInwardsCalculator"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilCreator
    {
        /// <summary>
        /// Creates a <see cref="Soil"/> based on information contained in the profiled <paramref name="profile"/>,
        /// which can be used in the <see cref="MacroStabilityInwardsCalculator"/>.
        /// </summary>
        /// <param name="profile">The <see cref="MacroStabilityInwardsSoilProfileUnderSurfaceLine"/> from
        /// which to take the information.</param>
        /// <returns>A new <see cref="Soil"/> with information taken from the <see cref="profile"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <see cref="MacroStabilityInwardsShearStrengthModel"/>
        /// has an type that can't be converted to <see cref="ShearStrengthModel"/>.</exception>
        public static Soil[] Create(MacroStabilityInwardsSoilProfileUnderSurfaceLine profile)
        {
            return profile.LayersUnderSurfaceLine.Select(l => new Soil(l.Properties.MaterialName)
            {
                UsePop = l.Properties.UsePop,
                ShearStrengthModel = ConvertShearStrengthModel(l.Properties.ShearStrengthModel)
            }).ToArray();
        }

        private static ShearStrengthModel ConvertShearStrengthModel(MacroStabilityInwardsShearStrengthModel shearStrengthModel)
        {
            switch (shearStrengthModel)
            {
                case MacroStabilityInwardsShearStrengthModel.None:
                    return ShearStrengthModel.None;
                case MacroStabilityInwardsShearStrengthModel.SuCalculated:
                    return ShearStrengthModel.CuCalculated;
                case MacroStabilityInwardsShearStrengthModel.CPhi:
                    return ShearStrengthModel.CPhi;
                case MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated:
                    return ShearStrengthModel.CPhiOrCuCalculated;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}