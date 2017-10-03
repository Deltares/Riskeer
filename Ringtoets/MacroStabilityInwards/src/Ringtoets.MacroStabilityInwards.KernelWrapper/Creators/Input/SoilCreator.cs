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
using System.ComponentModel;
using System.Linq;
using Deltares.WTIStability.Data.Geo;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="Soil"/> instances which are required by <see cref="IUpliftVanKernel"/>.
    /// </summary>
    internal static class SoilCreator
    {
        /// <summary>
        /// Creates a <see cref="Soil"/> based on information contained in the profile <paramref name="profile"/>,
        /// which can be used by <see cref="IUpliftVanKernel"/>.
        /// </summary>
        /// <param name="profile">The <see cref="UpliftVanSoilProfile"/> from which to take the information.</param>
        /// <returns>A new <see cref="Soil"/> with information taken from the <see cref="profile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="profile"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="UpliftVanSoilLayer.ShearStrengthModel"/>
        /// or <see cref="UpliftVanSoilLayer.DilatancyType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="UpliftVanSoilLayer.ShearStrengthModel"/>
        /// or <see cref="UpliftVanSoilLayer.DilatancyType"/> is a valid value but unsupported.</exception>
        public static Soil[] Create(UpliftVanSoilProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return profile.Layers.Select(l => new Soil(l.MaterialName)
            {
                UsePop = l.UsePop,
                ShearStrengthModel = ConvertShearStrengthModel(l.ShearStrengthModel),
                AbovePhreaticLevel = l.AbovePhreaticLevel,
                BelowPhreaticLevel = l.BelowPhreaticLevel,
                Cohesion = l.Cohesion,
                FrictionAngle = l.FrictionAngle,
                RatioCuPc = l.ShearStrengthRatio,
                StrengthIncreaseExponent = l.StrengthIncreaseExponent,
                PoP = l.Pop,
                DilatancyType = ConvertDilatancyType(l.DilatancyType)
            }).ToArray();
        }

        /// <summary>
        /// Converts a <see cref="UpliftVanShearStrengthModel"/> into a <see cref="ShearStrengthModel"/>.
        /// </summary>
        /// <param name="shearStrengthModel">The <see cref="UpliftVanShearStrengthModel"/> to convert.</param>
        /// <returns>A <see cref="ShearStrengthModel"/> based on <paramref name="shearStrengthModel"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// is a valid value but unsupported.</exception>
        private static ShearStrengthModel ConvertShearStrengthModel(UpliftVanShearStrengthModel shearStrengthModel)
        {
            if (!Enum.IsDefined(typeof(UpliftVanShearStrengthModel), shearStrengthModel))
            {
                throw new InvalidEnumArgumentException(nameof(shearStrengthModel),
                                                       (int) shearStrengthModel,
                                                       typeof(UpliftVanShearStrengthModel));
            }

            switch (shearStrengthModel)
            {
                case UpliftVanShearStrengthModel.SuCalculated:
                    return ShearStrengthModel.CuCalculated;
                case UpliftVanShearStrengthModel.CPhi:
                    return ShearStrengthModel.CPhi;
                case UpliftVanShearStrengthModel.CPhiOrSuCalculated:
                    return ShearStrengthModel.CPhiOrCuCalculated;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="UpliftVanDilatancyType"/> into a <see cref="DilatancyType"/>.
        /// </summary>
        /// <param name="dilatancyType">The <see cref="UpliftVanDilatancyType"/> to convert.</param>
        /// <returns>A <see cref="DilatancyType"/> based on <paramref name="dilatancyType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="dilatancyType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="dilatancyType"/>
        /// is a valid value but unsupported.</exception>
        private static DilatancyType ConvertDilatancyType(UpliftVanDilatancyType dilatancyType)
        {
            if (!Enum.IsDefined(typeof(UpliftVanDilatancyType), dilatancyType))
            {
                throw new InvalidEnumArgumentException(nameof(dilatancyType),
                                                       (int) dilatancyType,
                                                       typeof(UpliftVanDilatancyType));
            }

            switch (dilatancyType)
            {
                case UpliftVanDilatancyType.Phi:
                    return DilatancyType.Phi;
                case UpliftVanDilatancyType.Zero:
                    return DilatancyType.Zero;
                case UpliftVanDilatancyType.MinusPhi:
                    return DilatancyType.MinusPhi;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}