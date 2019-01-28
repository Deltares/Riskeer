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
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.TestUtil.Test
{
    [TestFixture]
    public class HydraRingDataEqualityHelperTest
    {
        [Test]
        public void AreEqual_EqualHydraRingVariables_DoesNotThrowAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            HydraRingDataEqualityHelper.AreEqual(expected, actual);
        }

        [Test]
        public void AreEqual_DifferentNumberOfHydraRingVariables_ThrowsAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            Assert.Throws<AssertionException>(() => HydraRingDataEqualityHelper.AreEqual(expected, actual));
        }

        [Test]
        public void AreEqual_DifferentVariableIds_ThrowsAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(111, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            Assert.Throws<AssertionException>(() => HydraRingDataEqualityHelper.AreEqual(expected, actual));
        }

        [Test]
        public void AreEqual_DifferentDistributionTypes_ThrowsAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Deterministic, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            Assert.Throws<AssertionException>(() => HydraRingDataEqualityHelper.AreEqual(expected, actual));
        }

        [Test]
        public void AreEqual_DifferentValues_ThrowsAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 22.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            Assert.Throws<AssertionException>(() => HydraRingDataEqualityHelper.AreEqual(expected, actual));
        }

        [Test]
        public void AreEqual_DifferentDeviationTypes_ThrowsAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            Assert.Throws<AssertionException>(() => HydraRingDataEqualityHelper.AreEqual(expected, actual));
        }

        [Test]
        public void AreEqual_DifferentMeans_ThrowsAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.33, 4.4, 5.5, 6.6, 7.7)
            };

            Assert.Throws<AssertionException>(() => HydraRingDataEqualityHelper.AreEqual(expected, actual));
        }

        [Test]
        public void AreEqual_DifferentVariabilities_ThrowsAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.44, 5.5, 6.6, 7.7)
            };

            Assert.Throws<AssertionException>(() => HydraRingDataEqualityHelper.AreEqual(expected, actual));
        }

        [Test]
        public void AreEqual_DifferentShifts_ThrowsAssertionException()
        {
            var expected = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7)
            };

            var actual = new HydraRingVariable[]
            {
                new TestHydraRingVariable(1, HydraRingDistributionType.LogNormal, HydraRingDeviationType.Variation, 2.2, 3.3, 4.4, 5.5, 6.6, 7.7),
                new TestHydraRingVariable(11, HydraRingDistributionType.Normal, HydraRingDeviationType.Standard, 2.2, 3.3, 4.4, 5.55, 6.6, 7.7)
            };

            Assert.Throws<AssertionException>(() => HydraRingDataEqualityHelper.AreEqual(expected, actual));
        }
    }
}