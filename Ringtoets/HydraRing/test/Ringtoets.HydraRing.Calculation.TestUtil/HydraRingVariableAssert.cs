// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;

namespace Ringtoets.HydraRing.Calculation.TestUtil
{
    /// <summary>
    /// Assert methods for <see cref="HydraRingVariable"/>.
    /// </summary>
    public static class HydraRingVariableAssert
    {
        /// <summary>
        /// Asserts whether or not <paramref name="expected"/> and <paramref name="actual"/> are the same.
        /// </summary>
        /// <param name="expected">The array of expected <see cref="HydraRingVariable"/>.</param>
        /// <param name="actual">The array of actual <see cref="HydraRingVariable"/>.</param>
        public static void AreEqual(HydraRingVariable[] expected, HydraRingVariable[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                AreEqual(expected[i], actual[i]);
            }
        }

        private static void AreEqual(HydraRingVariable expected, HydraRingVariable actual)
        {
            Assert.AreEqual(expected.Value, actual.Value, 1e-6);
            Assert.AreEqual(expected.DeviationType, actual.DeviationType);
            Assert.AreEqual(expected.DistributionType, actual.DistributionType);
            Assert.AreEqual(expected.Mean, actual.Mean, 1e-6);
            Assert.AreEqual(expected.Shift, actual.Shift, 1e-6);
            Assert.AreEqual(expected.Variability, actual.Variability, 1e-6);
            Assert.AreEqual(expected.VariableId, actual.VariableId, 1e-6);
        }
    }
}