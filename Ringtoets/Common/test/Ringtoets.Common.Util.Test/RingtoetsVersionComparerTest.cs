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

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Ringtoets.Common.Util.Test
{
    [TestFixture]
    public class RingtoetsVersionComparerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var comparer = new RingtoetsVersionComparer();

            // Assert
            Assert.IsInstanceOf<IComparer>(comparer);
            Assert.IsInstanceOf<IComparer<string>>(comparer);
        }

        [Test]
        [TestCase("", "0")]
        [TestCase(0, 1)]
        [TestCase(1.0, 1.1)]
        public void CompareObject_FirstLessThanSecond_ReturnsLessThanZero(object first, object second)
        {
            // Setup
            var comparer = new RingtoetsVersionComparer();

            // Call
            int compare = comparer.Compare(first, second);

            // Assert
            Assert.Less(compare, 0);
        }

        [Test]
        [TestCase("0", "")]
        [TestCase(1, 0)]
        [TestCase(1.1, 1.0)]
        public void CompareObject_FirstGreaterThanSecond_ReturnsMoreThanZero(object first, object second)
        {
            // Setup
            var comparer = new RingtoetsVersionComparer();

            // Call
            int compare = comparer.Compare(first, second);

            // Assert
            Assert.Greater(compare, 0);
        }

        [Test]
        [TestCase("", "")]
        [TestCase(1, 1)]
        [TestCase(1.1, 1.1)]
        public void CompareObject_FirstEqualToSecond_ReturnsZero(object first, object second)
        {
            // Setup
            var comparer = new RingtoetsVersionComparer();

            // Call
            int compare = comparer.Compare(first, second);

            // Assert
            Assert.AreEqual(0, compare);
        }

        [Test]
        [TestCase(null, "")]
        [TestCase("", "0")]
        [TestCase("0", "1")]
        [TestCase("1.0", "1.1")]
        public void CompareString_FirstLessThanSecond_ReturnsLessThanZero(string first, string second)
        {
            // Setup
            var comparer = new RingtoetsVersionComparer();

            // Call
            int compare = comparer.Compare(first, second);

            // Assert
            Assert.Less(compare, 0);
        }

        [Test]
        [TestCase("", null)]
        [TestCase("0", "")]
        [TestCase("1", "0")]
        [TestCase("1.1", "1.0")]
        public void CompareString_FirstGreaterThanSecond_ReturnsGreaterThanZero(string first, string second)
        {
            // Setup
            var comparer = new RingtoetsVersionComparer();

            // Call
            int compare = comparer.Compare(first, second);

            // Assert
            Assert.Greater(compare, 0);
        }

        [Test]
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("1", "1")]
        [TestCase("1.1", "1.1")]
        public void CompareString_FirstEqualToSecond_ReturnsZero(string first, string second)
        {
            // Setup
            var comparer = new RingtoetsVersionComparer();

            // Call
            int compare = comparer.Compare(first, second);

            // Assert
            Assert.AreEqual(0, compare);
        }
    }
}