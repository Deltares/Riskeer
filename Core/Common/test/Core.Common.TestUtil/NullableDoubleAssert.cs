// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Core.Common.TestUtil
{
    /// <summary>
    /// Class with assertion helpers for nullable doubles.
    /// </summary>
    public static class NullableDoubleAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> is not <c>null</c> and within <paramref name="delta"/> of
        /// <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected value.</param>
        /// <param name="actual">The actual nullable value.</param>
        /// <param name="delta">The tolerated difference.</param>
        public static void AreEqual(double expected, double? actual, double delta)
        {
            NUnit.Framework.Legacy.ClassicAssert.IsNotNull(actual);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(expected, actual.Value, delta);
        }
    }
}
