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

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// Extension methods for the <see cref="WaveConditionsInputStepSize"/> enum.
    /// </summary>
    public static class WaveConditionsInputStepSizeExtensions
    {
        /// <summary>
        /// Gets the real value associated with the given <paramref name="stepSize"/>.
        /// </summary>
        /// <param name="stepSize">The step size to get a real value for.</param>
        /// <returns>The real value of <paramref name="stepSize"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="stepSize"/> 
        /// is not a valid enum value of <see cref="WaveConditionsInputStepSize"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="stepSize"/>
        /// is not supported for the conversion.</exception>
        public static double AsValue(this WaveConditionsInputStepSize stepSize)
        {
            if (!Enum.IsDefined(typeof(WaveConditionsInputStepSize), stepSize))
            {
                throw new InvalidEnumArgumentException(nameof(stepSize), (int) stepSize, typeof(WaveConditionsInputStepSize));
            }

            switch (stepSize)
            {
                case WaveConditionsInputStepSize.Half:
                    return 0.5;
                case WaveConditionsInputStepSize.One:
                    return 1.0;
                case WaveConditionsInputStepSize.Two:
                    return 2.0;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}