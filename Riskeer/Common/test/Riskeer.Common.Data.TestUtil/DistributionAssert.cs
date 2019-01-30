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
using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Class to assert the properties of the probabilistic distributions.
    /// </summary>
    public static class DistributionAssert
    {
        /// <summary>
        /// Determines if the properties of the actual <see cref="IDistribution"/> are the same 
        /// as the expected <see cref="IDistribution"/>.
        /// </summary>
        /// <param name="expectedDistribution">The expected <see cref="IDistribution"/>.</param>
        /// <param name="actualDistribution">The actual <see cref="IDistribution"/>.</param>
        /// <exception cref="AssertionException">Thrown when the following differences are found between 
        /// the <paramref name="expectedDistribution"/> and <paramref name="actualDistribution"/>:
        /// <list type="bullet">
        /// <item>The probabilistic distribution types.</item>
        /// <item>The values for the mean and/or the standard deviation.</item>
        /// <item>The precision for the mean and/or the standard deviation.</item>
        /// </list></exception>
        public static void AreEqual(IDistribution expectedDistribution, IDistribution actualDistribution)
        {
            Assert.AreEqual(expectedDistribution.GetType(), actualDistribution.GetType());

            AreEqualValue(expectedDistribution.Mean, actualDistribution.Mean);
            AreEqualValue(expectedDistribution.StandardDeviation, actualDistribution.StandardDeviation);
        }

        /// <summary>
        /// Determines if the properties of the actual <see cref="LogNormalDistribution"/> are the same 
        /// as the expected <see cref="LogNormalDistribution"/>.
        /// </summary>
        /// <param name="expectedDistribution">The expected <see cref="IDistribution"/>.</param>
        /// <param name="actualDistribution">The actual <see cref="IDistribution"/>.</param>
        /// <exception cref="AssertionException">Thrown when the following differences are found between 
        /// the <paramref name="expectedDistribution"/> and <paramref name="actualDistribution"/>:
        /// <list type="bullet">
        /// <item>The probabilistic distribution types.</item>
        /// <item>The values for the mean, the standard deviation and/or the shift.</item>
        /// <item>The precision for the mean, the standard deviation and/or the shift.</item>
        /// </list></exception>
        public static void AreEqual(LogNormalDistribution expectedDistribution, LogNormalDistribution actualDistribution)
        {
            AreEqual((IDistribution) expectedDistribution, actualDistribution);

            AreEqualValue(expectedDistribution.Shift, actualDistribution.Shift);
        }

        /// <summary>
        /// Determines if the properties of the actual <see cref="TruncatedNormalDistribution"/> are the same 
        /// as the expected <see cref="TruncatedNormalDistribution"/>.
        /// </summary>
        /// <param name="expectedDistribution">The expected <see cref="TruncatedNormalDistribution"/>.</param>
        /// <param name="actualDistribution">The actual <see cref="TruncatedNormalDistribution"/>.</param>
        /// <exception cref="AssertionException">Thrown when the following differences are found between 
        /// the <paramref name="expectedDistribution"/> and <paramref name="actualDistribution"/>:
        /// <list type="bullet">
        /// <item>The probabilistic distribution types.</item>
        /// <item>The values for the mean, the standard deviation, the lower boundary and/or the upper boundary.</item>
        /// <item>The precision for the mean, the standard deviation, the lower boundary and/or the upper boundary.</item>
        /// </list></exception>
        public static void AreEqual(TruncatedNormalDistribution expectedDistribution, TruncatedNormalDistribution actualDistribution)
        {
            AreEqual((IDistribution) expectedDistribution, actualDistribution);

            AreEqualValue(expectedDistribution.LowerBoundary, actualDistribution.LowerBoundary);
            AreEqualValue(expectedDistribution.UpperBoundary, actualDistribution.UpperBoundary);
        }

        /// <summary>
        /// Determines if the properties of the actual <see cref="IVariationCoefficientDistribution"/> are the same as 
        /// the expected <see cref="IVariationCoefficientDistribution"/>.
        /// </summary>
        /// <param name="expectedDistribution">The expected <see cref="IVariationCoefficientDistribution"/>.</param>
        /// <param name="actualDistribution">The actual <see cref="IVariationCoefficientDistribution"/>.</param>
        /// <exception cref="AssertionException">Thrown when the following differences are found between 
        /// the <paramref name="expectedDistribution"/> and <paramref name="actualDistribution"/>:
        /// <list type="bullet">
        /// <item>The probabilistic distribution types.</item>
        /// <item>The values for the mean and/or the variation.</item>
        /// <item>The precision for the mean and/or the variation.</item>
        /// </list></exception>
        public static void AreEqual(IVariationCoefficientDistribution expectedDistribution, IVariationCoefficientDistribution actualDistribution)
        {
            Assert.AreEqual(expectedDistribution.GetType(), actualDistribution.GetType());

            AreEqualValue(expectedDistribution.Mean, actualDistribution.Mean);
            AreEqualValue(expectedDistribution.CoefficientOfVariation, actualDistribution.CoefficientOfVariation);
        }

        /// <summary>
        /// Determines if the properties of the actual <see cref="VariationCoefficientLogNormalDistribution"/> are the same as 
        /// the expected <see cref="VariationCoefficientLogNormalDistribution"/>.
        /// </summary>
        /// <param name="expectedDistribution">The expected <see cref="VariationCoefficientLogNormalDistribution"/>.</param>
        /// <param name="actualDistribution">The actual <see cref="VariationCoefficientLogNormalDistribution"/>.</param>
        /// <exception cref="AssertionException">Thrown when the following differences are found between 
        /// the <paramref name="expectedDistribution"/> and <paramref name="actualDistribution"/>:
        /// <list type="bullet">
        /// <item>The probabilistic distribution types.</item>
        /// <item>The values for the mean and/or the variation.</item>
        /// <item>The precision for the mean, variation and/or shift.</item>
        /// </list></exception>
        public static void AreEqual(VariationCoefficientLogNormalDistribution expectedDistribution, VariationCoefficientLogNormalDistribution actualDistribution)
        {
            AreEqual((IVariationCoefficientDistribution) expectedDistribution, actualDistribution);
            AreEqualValue(expectedDistribution.Shift, actualDistribution.Shift);
        }

        private static void AreEqualValue(RoundedDouble expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue.NumberOfDecimalPlaces, actualValue.NumberOfDecimalPlaces);
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}