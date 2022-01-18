// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;

namespace Riskeer.Common.Forms.Providers
{
    /// <summary>
    /// Provider for length effect properties.
    /// </summary>
    public class LengthEffectProvider : ILengthEffectProvider
    {
        private readonly Func<bool> getUseLengthEffectFunc;
        private readonly Func<double> getSectionNFunc;

        /// <summary>
        /// Creates a new instance of <see cref="LengthEffectProvider"/>.
        /// </summary>
        /// <param name="getUseLengthEffectFunc">The <see cref="Func{TResult}"/> to get whether
        /// the length effect should be used.</param>
        /// <param name="getSectionNFunc">The <see cref="Func{TResult}"/> to get the section n.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public LengthEffectProvider(Func<bool> getUseLengthEffectFunc, Func<double> getSectionNFunc)
        {
            if (getUseLengthEffectFunc == null)
            {
                throw new ArgumentNullException(nameof(getUseLengthEffectFunc));
            }

            if (getSectionNFunc == null)
            {
                throw new ArgumentNullException(nameof(getSectionNFunc));
            }

            this.getUseLengthEffectFunc = getUseLengthEffectFunc;
            this.getSectionNFunc = getSectionNFunc;
        }

        public bool UseLengthEffect => getUseLengthEffectFunc();

        public double SectionN => getSectionNFunc();
    }
}