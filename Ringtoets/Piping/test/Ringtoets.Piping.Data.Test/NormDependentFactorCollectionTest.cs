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

using System;
using System.Linq;
using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class NormDependentFactorCollectionTest
    {
        [Test]
        public void Constructor_WithoutPoints_ThrowArgumentNullException()
        {
            // Call
            TestDelegate test = () => new NormDependentFactorCollection(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void Constructor_WithTooFewPoints_ThrowArgumentException(int pointCount)
        {
            // Setup
            Tuple<int, double>[] points = Enumerable.Repeat(Tuple.Create(1,0.2), pointCount).ToArray();

            // Call
            TestDelegate test = () => new NormDependentFactorCollection(points);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        [TestCase(-5)]
        [TestCase(0)]
        [TestCase(31)]
        [TestCase(50)]
        public void GetFactorFromNorm_NormOutsideRange_ThrowArgumentOutOfRangeException(int norm)
        {
            // Setup
            Tuple<int, double>[] points = {
                Tuple.Create(1, 0.2),
                Tuple.Create(30, 0.5)
            };
            var collection = new NormDependentFactorCollection(points);

            // Call
            TestDelegate test = () => collection.GetFactorFromNorm(norm);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(test);
        }

        [Test]
        [TestCase(1, 0.2)]
        [TestCase(10, 0.403098)]
        [TestCase(20, 0.464236)]
        [TestCase(30, 0.5)]
        [TestCase(40, 0.556317)]
        [TestCase(50, 0.6)]
        public void GetFactorFromNorm_NormInsideRange_ReturnsInterpolatedValue(int norm, double expectedValue)
        {
            // Setup
            Tuple<int, double>[] points = {
                Tuple.Create(1, 0.2),
                Tuple.Create(30, 0.5), 
                Tuple.Create(50, 0.6)
            };
            var collection = new NormDependentFactorCollection(points);

            // Call
            double result = collection.GetFactorFromNorm(norm);

            // Assert
            Assert.AreEqual(expectedValue, result, 1e-6);
        }
    }
}