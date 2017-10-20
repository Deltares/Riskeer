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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using WtiStabilitySoil = Deltares.WTIStability.Data.Geo.Soil;
using WtiStabilityDilatancyType = Deltares.WTIStability.Data.Geo.DilatancyType;
using WtiStabilityShearStrengthModel = Deltares.WTIStability.Data.Geo.ShearStrengthModel;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="WtiStabilitySoil"/> instances which are required in a calculation.
    /// </summary>
    internal static class SoilCreator
    {
        /// <summary>
        /// Creates a <see cref="WtiStabilitySoil"/> based on information contained in the profile <paramref name="profile"/>,
        /// which can be used in a calculation.
        /// </summary>
        /// <param name="profile">The <see cref="SoilProfile"/> from which to take the information.</param>
        /// <returns>A new <see cref="WtiStabilitySoil"/> with information taken from the <see cref="profile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="profile"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="ShearStrengthModel"/>
        /// or <see cref="DilatancyType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="ShearStrengthModel"/>
        /// or <see cref="DilatancyType"/> is a valid value but unsupported.</exception>
        public static WtiStabilitySoil[] Create(SoilProfile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            return profile.Layers.Select(l => new WtiStabilitySoil(l.MaterialName)
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
        /// Converts a <see cref="ShearStrengthModel"/> into a <see cref="WtiStabilityShearStrengthModel"/>.
        /// </summary>
        /// <param name="shearStrengthModel">The <see cref="ShearStrengthModel"/> to convert.</param>
        /// <returns>A <see cref="WtiStabilityShearStrengthModel"/> based on <paramref name="shearStrengthModel"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="shearStrengthModel"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="shearStrengthModel"/>
        /// is a valid value but unsupported.</exception>
        private static WtiStabilityShearStrengthModel ConvertShearStrengthModel(ShearStrengthModel shearStrengthModel)
        {
            if (!Enum.IsDefined(typeof(ShearStrengthModel), shearStrengthModel))
            {
                throw new InvalidEnumArgumentException(nameof(shearStrengthModel),
                                                       (int) shearStrengthModel,
                                                       typeof(ShearStrengthModel));
            }

            switch (shearStrengthModel)
            {
                case ShearStrengthModel.SuCalculated:
                    return WtiStabilityShearStrengthModel.CuCalculated;
                case ShearStrengthModel.CPhi:
                    return WtiStabilityShearStrengthModel.CPhi;
                case ShearStrengthModel.CPhiOrSuCalculated:
                    return WtiStabilityShearStrengthModel.CPhiOrCuCalculated;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="DilatancyType"/> into a <see cref="WtiStabilityDilatancyType"/>.
        /// </summary>
        /// <param name="dilatancyType">The <see cref="DilatancyType"/> to convert.</param>
        /// <returns>A <see cref="WtiStabilityDilatancyType"/> based on <paramref name="dilatancyType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="dilatancyType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="dilatancyType"/>
        /// is a valid value but unsupported.</exception>
        private static WtiStabilityDilatancyType ConvertDilatancyType(DilatancyType dilatancyType)
        {
            if (!Enum.IsDefined(typeof(DilatancyType), dilatancyType))
            {
                throw new InvalidEnumArgumentException(nameof(dilatancyType),
                                                       (int) dilatancyType,
                                                       typeof(DilatancyType));
            }

            switch (dilatancyType)
            {
                case DilatancyType.Phi:
                    return WtiStabilityDilatancyType.Phi;
                case DilatancyType.Zero:
                    return WtiStabilityDilatancyType.Zero;
                case DilatancyType.MinusPhi:
                    return WtiStabilityDilatancyType.MinusPhi;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}