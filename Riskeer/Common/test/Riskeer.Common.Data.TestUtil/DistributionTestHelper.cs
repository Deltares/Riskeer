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

using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Class that can be used to test stochastic distributions
    /// </summary>
    public static class DistributionTestHelper
    {
        /// <summary>
        /// Asserts whether the values of a distribution are correctly set 
        /// to another distribution.
        /// </summary>
        /// <param name="distributionToAssert">The distribution to assert.</param>
        /// <param name="setDistribution">The distribution which was used to set the properties.</param>
        /// <param name="expectedDistribution">The expected distribution.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="distributionToAssert"/> and <paramref name="setDistribution"/>
        /// are the same reference.</item>
        /// <item>The values of the <paramref name="setDistribution"/> do not match with the 
        /// <paramref name="expectedDistribution"/>.</item>
        /// </list></exception>
        public static void AssertDistributionCorrectlySet(IDistribution distributionToAssert,
                                                          IDistribution setDistribution,
                                                          IDistribution expectedDistribution)
        {
            Assert.AreNotSame(setDistribution, distributionToAssert);
            DistributionAssert.AreEqual(expectedDistribution, distributionToAssert);
        }

        /// <summary>
        /// Asserts whether the values of a distribution are correctly set 
        /// to another distribution.
        /// </summary>
        /// <param name="distributionToAssert">The distribution to assert.</param>
        /// <param name="setDistribution">The distribution which was used to set the properties.</param>
        /// <param name="expectedDistribution">The expected distribution.</param>
        /// <exception cref="AssertionException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="distributionToAssert"/> and <paramref name="setDistribution"/>
        /// are the same reference.</item>
        /// <item>The values of the <paramref name="setDistribution"/> do not match with the 
        /// <paramref name="expectedDistribution"/>.</item>
        /// </list></exception>
        public static void AssertDistributionCorrectlySet(IVariationCoefficientDistribution distributionToAssert,
                                                          IVariationCoefficientDistribution setDistribution,
                                                          IVariationCoefficientDistribution expectedDistribution)
        {
            Assert.AreNotSame(setDistribution, distributionToAssert);
            DistributionAssert.AreEqual(expectedDistribution, distributionToAssert);
        }
    }
}