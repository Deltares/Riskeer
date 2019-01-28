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

using Core.Common.Base.Data;

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// Class that holds all the static wave conditions input parameters.
    /// </summary>
    public class GeneralWaveConditionsInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralWaveConditionsInput"/>.
        /// </summary>
        /// <param name="a">The 'a' parameter used in wave conditions calculations.</param>
        /// <param name="b">The 'b' parameter used in wave conditions calculations.</param>
        /// <param name="c">The 'c' parameter used in wave conditions calculations.</param>
        public GeneralWaveConditionsInput(double a, double b, double c)
        {
            A = new RoundedDouble(2, a);
            B = new RoundedDouble(2, b);
            C = new RoundedDouble(2, c);
        }

        /// <summary>
        /// Gets the 'a' parameter used in wave conditions calculations.
        /// </summary>
        public RoundedDouble A { get; }

        /// <summary>
        /// Gets the 'b' parameter used in wave conditions calculations.
        /// </summary>
        public RoundedDouble B { get; }

        /// <summary>
        /// Gets the 'c' parameter used in wave conditions calculations.
        /// </summary>
        public RoundedDouble C { get; }
    }
}