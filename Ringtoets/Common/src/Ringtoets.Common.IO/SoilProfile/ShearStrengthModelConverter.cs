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
using System.Globalization;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class provides helpers for converting values from the D-Soil Model database into
    /// <see cref="ShearStrengthModel"/>.
    /// </summary>
    public static class ShearStrengthModelConverter
    {
        /// <summary>
        /// Converts a nullable <see cref="double"/> into a <see cref="ShearStrengthModel"/>.
        /// </summary>
        /// <param name="shearStrengthModelValue">The value to convert.</param>
        /// <returns>The <see cref="ShearStrengthModel"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="shearStrengthModelValue"/> 
        /// could not be converted.</exception>
        public static ShearStrengthModel Convert(double? shearStrengthModelValue)
        {
            if (shearStrengthModelValue == null)
            {
                return ShearStrengthModel.None;
            }

            if (shearStrengthModelValue.Equals(0.0))
            {
                return ShearStrengthModel.None;
            }
            if (shearStrengthModelValue.Equals(1.0))
            {
                return ShearStrengthModel.SuCalculated;
            }
            if (shearStrengthModelValue.Equals(2.0))
            {
                return ShearStrengthModel.CPhi;
            }
            if (shearStrengthModelValue.Equals(3.0))
            {
                return ShearStrengthModel.CPhiOrSuCalculated;
            }

            string message = $"Cannot convert a value of {shearStrengthModelValue.Value.ToString(CultureInfo.CurrentCulture)} to {typeof(ShearStrengthModel)}.";
            throw new ArgumentException(message);
        }
    }
}