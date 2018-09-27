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

using System;
using System.ComponentModel;
using Ringtoets.Common.Data.IllustrationPoints;
using HydraRingCombinationType = Ringtoets.HydraRing.Calculation.Data.Output.IllustrationPoints.CombinationType;

namespace Ringtoets.Common.Service.IllustrationPoints
{
    /// <summary>
    /// The converter that converts <see cref="HydraRingCombinationType"/> data into <see cref="CombinationType"/> data.
    /// </summary>
    public static class CombinationTypeConverter
    {
        /// <summary>
        /// Creates a <see cref="CombinationType"/> based on <see cref="HydraRingCombinationType"/>.
        /// </summary>
        /// <param name="hydraRingCombinationType">The <see cref="HydraRingCombinationType"/>
        /// to convert.</param>
        /// <returns>The <see cref="CombinationType"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when
        /// <paramref name="hydraRingCombinationType"/> has an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when a valid value of 
        /// <paramref name="hydraRingCombinationType"/> cannot be converted.</exception>
        public static CombinationType Convert(HydraRingCombinationType hydraRingCombinationType)
        {
            if (!Enum.IsDefined(typeof(HydraRingCombinationType), hydraRingCombinationType))
            {
                throw new InvalidEnumArgumentException(nameof(hydraRingCombinationType),
                                                       (int) hydraRingCombinationType,
                                                       typeof(HydraRingCombinationType));
            }

            switch (hydraRingCombinationType)
            {
                case HydraRingCombinationType.Or:
                    return CombinationType.Or;
                case HydraRingCombinationType.And:
                    return CombinationType.And;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}