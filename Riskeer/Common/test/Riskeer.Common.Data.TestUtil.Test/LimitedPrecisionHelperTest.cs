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

using System;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;

namespace Riskeer.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class LimitedPrecisionHelperTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GetAccuracy_RoundedDouble_ReturnsAccuracy(int precision)
        {
            // Setup
            var roundedDouble = new RoundedDouble(precision);

            // Call
            double accuracy = roundedDouble.GetAccuracy();

            // Assert
            double expectedPrecision = 0.5 * Math.Pow(10.0, -precision);
            Assert.AreEqual(expectedPrecision, accuracy);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GetAccuracy_Distribution_ReturnsAccuracy(int precision)
        {
            // Setup
            var distribution = new SimpleDistribution
            {
                Mean = new RoundedDouble(precision)
            };

            // Call
            double accuracy = distribution.GetAccuracy();

            // Assert
            double expectedPrecision = 0.5 * Math.Pow(10.0, -precision);
            Assert.AreEqual(expectedPrecision, accuracy);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GetAccuracy_VariationCoefficientDistribution_ReturnsAccuracy(int precision)
        {
            // Setup
            var distribution = new SimpleVariationCoefficientDistribution
            {
                Mean = new RoundedDouble(precision)
            };

            // Call
            double accuracy = distribution.GetAccuracy();

            // Assert
            double expectedPrecision = 0.5 * Math.Pow(10.0, -precision);
            Assert.AreEqual(expectedPrecision, accuracy);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        public void GetAccuracy_RoundedPoint2DCollection_ReturnsAccuracy(int precision)
        {
            // Setup
            var roundedCollectoin = new RoundedPoint2DCollection(precision, Enumerable.Empty<Point2D>());

            // Call
            double accuracy = roundedCollectoin.GetAccuracy();

            // Assert
            double expectedPrecision = 0.5 * Math.Pow(10.0, -precision);
            Assert.AreEqual(expectedPrecision, accuracy);
        }

        private class SimpleDistribution : IDistribution
        {
            public RoundedDouble Mean { get; set; }
            public RoundedDouble StandardDeviation { get; set; }

            public object Clone()
            {
                throw new NotImplementedException();
            }
        }

        private class SimpleVariationCoefficientDistribution : IVariationCoefficientDistribution
        {
            public RoundedDouble Mean { get; set; }
            public RoundedDouble CoefficientOfVariation { get; set; }

            public object Clone()
            {
                throw new NotImplementedException();
            }
        }
    }
}