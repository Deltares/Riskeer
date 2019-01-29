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
using Core.Common.Base.Data;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Probabilistics
{
    [TestFixture]
    public class TruncatedNormalDistributionTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(15)]
        public void DefaultConstructor_ExpectedValues(int numberOfDecimalPlaces)
        {
            // Call
            var distribution = new TruncatedNormalDistribution(numberOfDecimalPlaces);

            // Assert
            Assert.IsInstanceOf<NormalDistribution>(distribution);
            Assert.AreEqual(0.0, distribution.Mean.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, distribution.StandardDeviation.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.LowerBoundary.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.LowerBoundary.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, distribution.UpperBoundary.Value);
            Assert.AreEqual(numberOfDecimalPlaces, distribution.UpperBoundary.NumberOfDecimalPlaces);
        }

        [Test]
        public void LowerBoundary_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces()
        {
            const double value = 1.23456789;
            const int numberOfDecimalPlaces = 4;
            var distribution = new TruncatedNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.LowerBoundary = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.LowerBoundary.NumberOfDecimalPlaces);
            Assert.AreEqual(value, distribution.LowerBoundary, distribution.LowerBoundary.GetAccuracy());
        }

        [Test]
        public void UpperBoundary_SetNewValue_GetValueRoundedToGivenNumberOfDecimalPlaces()
        {
            const double value = 1.23456789;
            const int numberOfDecimalPlaces = 4;
            var distribution = new TruncatedNormalDistribution(numberOfDecimalPlaces);

            // Call
            distribution.UpperBoundary = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(numberOfDecimalPlaces, distribution.UpperBoundary.NumberOfDecimalPlaces);
            Assert.AreEqual(value, distribution.UpperBoundary, distribution.UpperBoundary.GetAccuracy());
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var random = new Random(21);
            var original = new TruncatedNormalDistribution(random.Next(1, 16))
            {
                Mean = random.NextRoundedDouble(),
                StandardDeviation = random.NextRoundedDouble(),
                LowerBoundary = random.NextRoundedDouble(),
                UpperBoundary = random.NextRoundedDouble()
            };

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, DistributionAssert.AreEqual);
        }
    }
}