// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Extension methods for <see cref="Random"/>.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Generates a new pseudo-random number between <paramref name="lowerLimit"/> and <paramref name="upperLimit"/>.
        /// </summary>
        /// <param name="random">A pseudo-random number generator.</param>
        /// <param name="lowerLimit">The lower limit of the new random number.</param>
        /// <param name="upperLimit">The upper limit of the new random number.</param>
        /// <returns>A new random number.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="lowerLimit"/> is larger than <paramref name="upperLimit"/>.</exception>
        /// <exception cref="NotFiniteNumberException">Thrown when the generated value is not finite.</exception>
        public static double NextDouble(this Random random, double lowerLimit, double upperLimit)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (lowerLimit > upperLimit)
            {
                throw new ArgumentException("lowerLimit is larger than upperLimit");
            }

            double difference = upperLimit - lowerLimit;
            double randomValue = lowerLimit + random.NextDouble() * difference;
            if (double.IsInfinity(randomValue) || double.IsNaN(randomValue))
            {
                string message = $"Creating a new random value with lower limit {lowerLimit} " +
                                 $"and upper limit {upperLimit} did not result in a finite value.";
                throw new NotFiniteNumberException(message, randomValue);
            }

            return randomValue;
        }

        /// <summary>
        /// Returns a random boolean value.
        /// </summary>
        /// <param name="random">A pseudo-random number generator.</param>
        /// <returns>A new random boolean value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is <c>null</c>.</exception>
        public static bool NextBoolean(this Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return Convert.ToBoolean(random.Next(0, 2));
        }

        /// <summary>
        /// Returns a random <see cref="RoundedDouble"/> value.
        /// </summary>
        /// <param name="random">A pseudo-random number generator.</param>
        /// <returns>A new random <see cref="RoundedDouble"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is <c>null</c>.</exception>
        public static RoundedDouble NextRoundedDouble(this Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return (RoundedDouble) random.NextDouble();
        }

        /// <summary>
        /// Generates a new pseudo-random number between <paramref name="lowerLimit"/> and <paramref name="upperLimit"/>.
        /// </summary>
        /// <param name="random">A pseudo-random number generator.</param>
        /// <param name="lowerLimit">The lower limit of the new random number.</param>
        /// <param name="upperLimit">The upper limit of the new random number.</param>
        /// <returns>A new random <see cref="RoundedDouble"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="lowerLimit"/> is larger than <paramref name="upperLimit"/>.</exception>
        /// <exception cref="NotFiniteNumberException">Thrown when the generated value is not finite.</exception>
        public static RoundedDouble NextRoundedDouble(this Random random, double lowerLimit, double upperLimit)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            return (RoundedDouble) random.NextDouble(lowerLimit, upperLimit);
        }

        /// <summary>
        /// Returns a random value of <typeparamref name="TEnum"/>.
        /// </summary>
        /// <typeparam name="TEnum">The <see cref="Enum"/> to use.</typeparam>
        /// <param name="random">A pseudo-random number generator.</param>
        /// <returns>>A new random value of type <typeparamref name="TEnum"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="random"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <typeparamref name="TEnum"/> is not an <see cref="Enum"/>.</exception>
        public static TEnum NextEnumValue<TEnum>(this Random random)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var enumValues = (TEnum[]) Enum.GetValues(typeof(TEnum));
            return enumValues[random.Next(enumValues.Length)];
        }

        /// <summary>
        /// Returns a random value of <typeparamref name="TEnum"/> from <paramref name="enumValues"/>.
        /// </summary>
        /// <typeparam name="TEnum">The <see cref="Enum"/> to use.</typeparam>
        /// <param name="random">A pseudo-random number generator.</param>
        /// <param name="enumValues">A collection of valid enum values to return a random element from.</param>
        /// <returns>A random value of type <typeparamref name="TEnum"/> from <paramref name="enumValues"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="enumValues"/> is empty or
        /// of <typeparamref name="TEnum"/> is not an <see cref="Enum"/>.</exception>
        public static TEnum NextEnumValue<TEnum>(this Random random, IEnumerable<TEnum> enumValues)
        {
            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            if (enumValues == null)
            {
                throw new ArgumentNullException(nameof(enumValues));
            }

            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException($"'{typeof(TEnum).Name}' is not an enum.");
            }

            if (!enumValues.Any())
            {
                throw new ArgumentException($"'{nameof(enumValues)}' cannot be an empty collection.");
            }

            return enumValues.ElementAt(random.Next(enumValues.Count()));
        }
    }
}