// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using Core.Common.TestUtil;
using Core.Components.Gis.Data.Categories;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Data.Categories
{
    [TestFixture]
    public class RangeCriteriaTest
    {
        [Test]
        public void Constructor_ValidArguments_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(21);

            var rangeCriteriaOperator = random.NextEnumValue<RangeCriteriaOperator>();
            double lowerBound = random.NextDouble();
            double upperBound = 1 + random.NextDouble();

            // Call
            var criteria = new RangeCriteria(rangeCriteriaOperator, lowerBound, upperBound);

            // Assert
            Assert.IsInstanceOf<IMapCriteria>(criteria);
            Assert.AreEqual(rangeCriteriaOperator, criteria.RangeCriteriaOperator);
            Assert.AreEqual(lowerBound, criteria.LowerBound);
            Assert.AreEqual(upperBound, criteria.UpperBound);
        }

        [Test]
        public void Constructor_InvalidRangeCriteriaOperator_ThrownInvalidEnumArgumentException()
        {
            // Setup
            var random = new Random(21);
            const RangeCriteriaOperator invalidOperator = (RangeCriteriaOperator) 99999;

            // Call
            TestDelegate call  = () => new RangeCriteria(invalidOperator, random.NextDouble(), random.NextDouble());

            // Assert
            string expectedMessage = $"The value of argument 'rangeCriteriaOperator' ({invalidOperator}) is invalid for Enum type '{nameof(RangeCriteriaOperator)}'.";
            string parameterName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(call, expectedMessage).ParamName;
            Assert.AreEqual("rangeCriteriaOperator", parameterName);
        }

        [Test]
        [TestCase(10, 10)]
        [TestCase(11, 10)]
        public void Constructor_InvalidLowerBound_ThrowsArgumentException(double lowerBound, double upperBound)
        {
            // Setup
            var random = new Random(21);

            // Call
            TestDelegate call = () => new RangeCriteria(random.NextEnumValue<RangeCriteriaOperator>(), lowerBound, upperBound);

            // Assert
            const string expectedMessage = "Lower bound cannot be larger or equal than upper bound.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }
    }
}