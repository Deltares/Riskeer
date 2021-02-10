// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Core.Common.Base.Data;
using NUnit.Framework;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Class for asserting <see cref="RoundedDouble"/> instances.
    /// </summary>
    public static class RoundedDoubleTestHelper
    {
        /// <summary>
        /// Asserts whether <paramref name="expectedValue"/> matches <paramref name="actualValue"/>.
        /// </summary>
        /// <param name="expectedValue">The expected <c>double</c> value.</param>
        /// <param name="actualValue">The actual <see cref="RoundedDouble"/> instance.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="expectedValue"/> doesn't match
        /// <paramref name="actualValue"/>.</exception>
        public static void AssertRoundedDouble(double? expectedValue, RoundedDouble actualValue)
        {
            Assert.IsTrue(expectedValue.HasValue);
            Assert.AreEqual(expectedValue.Value, actualValue, actualValue.GetAccuracy());
        }

        /// <summary>
        /// Asserts whether <paramref name="expectedValue"/> matches <paramref name="actualValue"/>.
        /// </summary>
        /// <param name="expectedValue">The expected <c>double</c> value.</param>
        /// <param name="actualValue">The actual <see cref="RoundedDouble"/> instance.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="expectedValue"/> doesn't match
        /// <paramref name="actualValue"/>.</exception>
        public static void AssertRoundedDouble(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }
    }
}