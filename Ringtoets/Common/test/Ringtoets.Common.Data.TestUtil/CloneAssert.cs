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

using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Class to assert clones of specific objects.
    /// </summary>
    public static class CloneAssert
    {
        /// <summary>
        /// Determines if the actual object is a clone of the expected <see cref="NormalDistribution"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="NormalDistribution"/>.</param>
        /// <param name="actualObject">The actual object.</param>
        /// <exception cref="AssertionException">Thrown when the provided parameters are no clones.</exception>
        public static void AreClones(NormalDistribution expected, object actualObject)
        {
            Assert.AreNotSame(expected, actualObject);
            Assert.IsInstanceOf<NormalDistribution>(actualObject);

            var actual = (NormalDistribution) actualObject;
            Assert.AreEqual(expected.Mean, actual.Mean);
            Assert.AreEqual(expected.StandardDeviation, actual.StandardDeviation);
        }

        /// <summary>
        /// Determines if the actual object is a clone of the expected <see cref="LogNormalDistribution"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="LogNormalDistribution"/>.</param>
        /// <param name="actualObject">The actual object.</param>
        /// <exception cref="AssertionException">Thrown when the provided parameters are no clones.</exception>
        public static void AreClones(LogNormalDistribution expected, object actualObject)
        {
            Assert.AreNotSame(expected, actualObject);
            Assert.IsInstanceOf<LogNormalDistribution>(actualObject);

            var actual = (LogNormalDistribution) actualObject;
            Assert.AreEqual(expected.Mean, actual.Mean);
            Assert.AreEqual(expected.StandardDeviation, actual.StandardDeviation);
            Assert.AreEqual(expected.Shift, actual.Shift);
        }

        /// <summary>
        /// Determines if the actual object is a clone of the expected <see cref="TruncatedNormalDistribution"/>.
        /// </summary>
        /// <param name="expected">The expected <see cref="TruncatedNormalDistribution"/>.</param>
        /// <param name="actualObject">The actual object.</param>
        /// <exception cref="AssertionException">Thrown when the provided parameters are no clones.</exception>
        public static void AreClones(TruncatedNormalDistribution expected, object actualObject)
        {
            Assert.AreNotSame(expected, actualObject);
            Assert.IsInstanceOf<TruncatedNormalDistribution>(actualObject);

            var actual = (TruncatedNormalDistribution) actualObject;
            Assert.AreEqual(expected.Mean, actual.Mean);
            Assert.AreEqual(expected.StandardDeviation, actual.StandardDeviation);
            Assert.AreEqual(expected.LowerBoundary, actual.LowerBoundary);
            Assert.AreEqual(expected.UpperBoundary, actual.UpperBoundary);
        }
    }
}