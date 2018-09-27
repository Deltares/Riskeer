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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Helper class to parse the breakwater and foreshore properties of a calculation input object into required objects 
    /// for performing calculations with Hydra-Ring.
    /// </summary>
    public static class HydraRingInputParser
    {
        /// <summary>
        /// Parses the foreshore geometry of a calculation input object into a <see cref="IEnumerable{T}"/> of 
        /// <see cref="HydraRingForelandPoint"/>.
        /// </summary>
        /// <param name="input">A calculation input object that implements <see cref="IUseForeshore"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="HydraRingForelandPoint"/>.</returns>
        public static IEnumerable<HydraRingForelandPoint> ParseForeshore(IUseForeshore input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)).ToArray() : new HydraRingForelandPoint[0];
        }

        /// <summary>
        /// Parses the breakwater properties of a calculation input object into a <see cref="HydraRingBreakWater"/> object.
        /// </summary>
        /// <param name="input">A calculation input object that implements <see cref="IUseBreakWater"/>.</param>
        /// <returns>A <see cref="HydraRingBreakWater"/> object, <c>null</c> if <see cref="IUseBreakWater.UseBreakWater"/>
        /// is <c>false</c></returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when the break water type is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when the break water type is a valid value, but unsupported.</exception>
        public static HydraRingBreakWater ParseBreakWater(IUseBreakWater input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater(ConvertBreakWaterType(input.BreakWater.Type), input.BreakWater.Height) : null;
        }

        /// <summary>
        /// Converts <paramref name="type"/> into an integer value to be used by Hydra-Ring.
        /// </summary>
        /// <param name="type">The <see cref="BreakWaterType"/> to convert.</param>
        /// <returns>The integer value to be used by Hydra-Ring.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="type"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="type"/> is a valid value, but unsupported.</exception>
        private static int ConvertBreakWaterType(BreakWaterType type)
        {
            if (!Enum.IsDefined(type.GetType(), type))
            {
                throw new InvalidEnumArgumentException(nameof(type), (int) type, type.GetType());
            }

            switch (type)
            {
                case BreakWaterType.Caisson:
                    return 1;
                case BreakWaterType.Wall:
                    return 2;
                case BreakWaterType.Dam:
                    return 3;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}